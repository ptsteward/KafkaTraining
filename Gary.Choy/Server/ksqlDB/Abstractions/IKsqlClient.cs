using KafkaTesting.ksqlDB.Objects;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTesting.ksqlDB.Abstractions
{
    public interface IKsqlClient
    {
        Task<HttpStatusCode> CloseStreamAsync(string queryId, CancellationToken token = default);
        Task<Stream> ExecuteQueryAsync(KsqlQuery query, CancellationToken token = default);
    }
}