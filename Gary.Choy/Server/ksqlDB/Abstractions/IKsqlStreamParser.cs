using KafkaTesting.ksqlDB.Objects;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlStreamParser
    {
        IAsyncEnumerable<T> ParseStreamAsync<T>(StreamReader reader, StreamHeader header);
        Task<StreamHeader> ParseStreamHeaderAsync(StreamReader reader);
    }
}
