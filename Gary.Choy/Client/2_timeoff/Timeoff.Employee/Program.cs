using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using System.Globalization;
using System.Net;
using TimeOff.Models;

/// <summary>
/// Timeoff Employee
/// </summary>
/// <seealso cref="https://thecloudblog.net/post/event-driven-architecture-with-apache-kafka-for-net-developers-part-1-event-producer/"/>
class TimeoffEmployee
{
    static async Task Main(string[] args)
    {
        //todo: add and parse settings from json config file
        var adminConfig = new AdminClientConfig { BootstrapServers = "localhost:9092" };
        var schemaRegistryConfig = new SchemaRegistryConfig { Url = "http://localhost:8081" };
        var producerConfig = new ProducerConfig
        {
            BootstrapServers = "localhost:9092",
            // Guarantees delivery of message to topic.
            EnableDeliveryReports = true,
            ClientId = Dns.GetHostName(),
            EnableIdempotence = true,
            MessageSendMaxRetries = 3
        };

        using var adminClient = new AdminClientBuilder(adminConfig).Build();
        try
        {
            await adminClient.CreateTopicsAsync(new[]
            {
                new TopicSpecification
                {
                    Name = "leave-applications",
                    ReplicationFactor = 1,
                    NumPartitions = 3
                }
            });
        }
        catch (CreateTopicsException e) when (e.Results.Select(r => r.Error.Code)
            .Any(el => el == ErrorCode.TopicAlreadyExists))
        {
            Console.WriteLine($"Topic {e.Results[0].Topic} already exists");
        }

        using var schemaRegistry = new CachedSchemaRegistryClient(schemaRegistryConfig);
        using var producer = new ProducerBuilder<string, LeaveApplicationReceived>(producerConfig)
            .SetValueSerializer(new ProtobufSerializer<LeaveApplicationReceived>(schemaRegistry))
            .Build();

        while (true)
        {
            var empEmail = ReadLine.Read("Enter your employee Email (e.g. none@example-company.com): ",
                "none@example.com").ToLowerInvariant();
            var empDepartment = ReadLine.Read("Enter your department code (HR, IT, OPS): ").ToUpperInvariant();
            var leaveDurationInHours =
                int.Parse(ReadLine.Read("Enter number of hours of leave requested (e.g. 8): ", "8"));
            var leaveStartDate = DateTime.ParseExact(ReadLine.Read("Enter vacation start date (dd-mm-yy): ",
                $"{DateTime.Today:dd-MM-yy}"), "dd-mm-yy", CultureInfo.InvariantCulture);

            var leaveApplication = new LeaveApplicationReceived
            {
                EmpDepartment = empDepartment,
                EmpEmail = empEmail,
                LeaveDurationInHours = leaveDurationInHours,
                LeaveStartDateTicks = leaveStartDate.Ticks
            };
            var partition = new TopicPartition(
                ApplicationConstants.LeaveApplicationsTopicName,
                new Partition((int)Enum.Parse<Departments>(empDepartment)));
            var result = await producer.ProduceAsync(partition,
                new Message<string, LeaveApplicationReceived>
                {
                    Key = $"{empEmail}-{DateTime.UtcNow.Ticks}",
                    Value = leaveApplication
                });
            Console.WriteLine(
                $"\nMsg: Your leave request is queued at offset {result.Offset.Value} in the Topic {result.Topic}:{result.Partition.Value}\n\n");
        }
    }

    public static class ApplicationConstants
    {
        public static string LeaveApplicationsTopicName => "leave-applications";
    }

    public enum Departments : byte
    {
        HR = 0,
        IT = 1,
        OPS = 2
    }

}


