using KafkaTesting.ksqlDB.Objects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlQueryReader
    {
        Task<KsqlQuery> GetKsqlQueryAsync(string queryName, IReadOnlyDictionary<string, string> options);
    }
}
