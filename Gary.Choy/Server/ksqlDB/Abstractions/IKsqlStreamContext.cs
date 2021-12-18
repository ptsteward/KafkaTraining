using System.Collections.Generic;
using System.Threading;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlStreamContext
    {
        IAsyncEnumerable<T> ExecuteQueryAsync<T>(string queryName, IReadOnlyDictionary<string, string> options, CancellationToken token = default);
    }
}