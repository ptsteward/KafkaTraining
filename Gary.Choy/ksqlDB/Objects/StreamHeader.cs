using System;

namespace KafkaTesting.ksqlDB.Objects
{
    public class StreamHeader
    {
        public string QueryId { get; set; } = string.Empty;
        public string[] ColumnNames { get; set; } = Array.Empty<string>();
        public string[] ColumnTypes { get; set; } = Array.Empty<string>();
    }
}