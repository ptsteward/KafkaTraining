using KafkaTesting.ksqlDB.Abstractions;
using KafkaTesting.ksqlDB.Objects;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaTesting.ksqlDB
{
    public class KsqlClient : IKsqlClient
    {
        private const string KsqlMediaType = "application/vnd.ksqlapi.delimited.v1";

        private readonly HttpClient client;

        public KsqlClient(HttpClient client) => this.client = client;

        public async Task<Stream> ExecuteQueryAsync(KsqlQuery query, CancellationToken token = default)
        {
            var body = JsonConvert.SerializeObject(query);
            var message = BuildOpenRequestMessage("/query-stream", body);
            var response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, token);
            Console.WriteLine($"Cient: {response.StatusCode}");
            if (!response.IsSuccessStatusCode)
                Console.WriteLine($"Client: {await response.Content.ReadAsStringAsync()}");
            return await response.Content.ReadAsStreamAsync();
        }

        public async Task<HttpStatusCode> CloseStreamAsync(string queryId, CancellationToken token = default)
        {
            var body = JsonConvert.SerializeObject(new { queryId });
            var message = BuildCloseRequestMessage("/close-query", body);
            using var response = await client.SendAsync(message, token);
            return response.StatusCode;
        }

        private HttpRequestMessage BuildOpenRequestMessage(string requestUri, string body)
            => new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(body, Encoding.UTF8, KsqlMediaType),
                Headers = { { "accept", MediaTypeWithQualityHeaderValue.Parse(KsqlMediaType).ToString() } },
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };

        private HttpRequestMessage BuildCloseRequestMessage(string requestUri, string body)
            => new HttpRequestMessage(HttpMethod.Post, requestUri)
            {
                Content = new StringContent(body, Encoding.UTF8, MediaTypeNames.Application.Json),
                Version = HttpVersion.Version20,
                VersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
            };
    }
}