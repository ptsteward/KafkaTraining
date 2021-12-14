using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Extensions;
using KafkaTesting.ksqlDB.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace KafkaTesting.ksqlDB
{
    public class KsqlQueryReader : IKsqlQueryReader
    {
        private readonly Assembly? ksqlAssembly;
        private readonly string ksqlRootNamespace;
        private readonly string ksqlNamespace;

        public KsqlQueryReader(Assembly ksqlQueryAssembly, string ksqlNamespace = "KsqlQueries")
        {
            this.ksqlAssembly = ksqlQueryAssembly ?? throw new ArgumentNullException(nameof(ksqlQueryAssembly));
            ksqlRootNamespace = this.ksqlAssembly?.GetName().Name ?? string.Empty;
            this.ksqlNamespace = ksqlNamespace;
        }

        public async Task<KsqlQuery> GetKsqlQueryAsync(string queryName, IReadOnlyDictionary<string, string> options)
        {
            var query = await GetRawQueryStringAsync(queryName);
            if (string.IsNullOrEmpty(query)) return new KsqlQuery();

            return new KsqlQuery()
            {
                Ksql = query,
                StreamProperties = options
            };
        }

        private async Task<string> GetRawQueryStringAsync(string queryName)
        {
            var resourceName = $"{ksqlRootNamespace}.{ksqlNamespace}.{queryName}.ksql";

            using var stream = ksqlAssembly!.GetManifestResourceStream(resourceName);
            if (stream is null) return string.Empty;

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
