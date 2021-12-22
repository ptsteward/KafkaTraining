using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using TimeOff.Models;

/// <summary>
/// Timeoff Manager
/// </summary>
/// <seealso cref="https://thecloudblog.net/post/event-driven-architecture-with-apache-kafka-for-.net-developers-part-2-event-consumer/"/>
class TimeOffManager
{
    public record KafkaMessage(string Key, int Partition, LeaveApplicationReceived Message);

    private static async Task Main(string[] args)
    {
        //todo: add and parse settings from json config file
        var adminConfig = new AdminClientConfig { BootstrapServers = "localhost:9092" };
        var schemaRegistryConfig = new SchemaRegistryConfig { Url = "http://localhost:8081" };

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "manager",
            EnableAutoCommit = false,
            EnableAutoOffsetStore = false,
            // Read messages from start if no commit exists.
            AutoOffsetReset = AutoOffsetReset.Earliest,
            MaxPollIntervalMs = 10000,
            SessionTimeoutMs = 10000
        };

        
        var leaveApplicationReceivedMessages = new Queue<KafkaMessage>();

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var consumer = new ConsumerBuilder<string, LeaveApplicationReceived>(consumerConfig)
            .SetValueDeserializer(new ProtobufDeserializer<LeaveApplicationReceived>().AsSyncOverAsync())
            .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}"))
            .Build();
        {
            try
            {
                consumer.Subscribe("leave-applications");
                Console.WriteLine("Consumer loop started...\n");
                while (true)
                {
                    try
                    {
                        // We will give the process 1 second to commit the message and store its offset.
                        var result = consumer.Consume(TimeSpan.FromMilliseconds(consumerConfig.MaxPollIntervalMs - 1000 ?? 250000));
                        var leaveRequest = result?.Message?.Value;
                        if (result == null || leaveRequest == null)
                        {
                            continue;
                        }

                        
                        leaveApplicationReceivedMessages.Enqueue(new KafkaMessage(result.Message.Key, result.Partition.Value, result.Message.Value));

                        consumer.Commit(result);
                        consumer.StoreOffset(result);

                        Console.WriteLine($"Received message: {result.Message.Key} Value: {result.Message.Value}");
                        // todo: do something with messages?
                    }
                    catch (ConsumeException e) when (!e.Error.IsFatal)
                    {
                        Console.WriteLine($"Non fatal error: {e}");
                    }
                }
            }
            finally
            {
                consumer.Close();
            }
        }
    }
}