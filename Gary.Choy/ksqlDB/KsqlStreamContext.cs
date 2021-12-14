using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Objects;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTesting.ksqlDB
{
    public class KsqlStreamContext : IKsqlStreamContext
    {
        private readonly IKsqlQueryReader queryReader;
        private readonly IKsqlClient client;
        private readonly IKsqlStreamParser streamParser;

        public KsqlStreamContext(IKsqlQueryReader queryReader, IKsqlClient client, IKsqlStreamParser streamParser)
        {
            this.queryReader = queryReader;
            this.client = client;
            this.streamParser = streamParser;
        }

        public async IAsyncEnumerable<T> ExecuteQueryAsync<T>(string queryName, IReadOnlyDictionary<string, string> options, [EnumeratorCancellation] CancellationToken token = default)
        {
            var query = await queryReader.GetKsqlQueryAsync(queryName, options);
            if (string.IsNullOrEmpty(query.Ksql))
                yield return default!;

            await using var stream = await client.ExecuteQueryAsync(query, token);
            using var reader = new StreamReader(stream);

            var header = await GetHeaderAsync(reader);

            try
            {
                await foreach (var item in streamParser.ParseStreamAsync<T>(reader, header))
                    yield return item;
            }
            finally
            {
                reader?.Dispose();
                await client.CloseStreamAsync(header.QueryId);
            }
        }

        private async Task<StreamHeader> GetHeaderAsync(StreamReader reader)
        {
            var header = await streamParser.ParseStreamHeaderAsync(reader);
            return header;
        }
    }
}