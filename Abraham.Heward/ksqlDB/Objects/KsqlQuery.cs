using Newtonsoft.Json;
using System.Collections.Generic;

namespace KafkaTesting.ksqlDB.Objects
{
    public class KsqlQuery
    {
        [JsonProperty("sql")]
        public string Ksql { get; init; } = string.Empty;

        [JsonProperty("streamsProperties")]
        public IReadOnlyDictionary<string, string> StreamProperties { get; init; } = new Dictionary<string, string>();
    }
}