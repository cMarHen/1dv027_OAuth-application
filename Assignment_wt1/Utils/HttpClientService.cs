using Assignment_wt1.Interfaces;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using System.Net;

namespace Assignment_wt1.Utils
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _httpClient;

        public HttpClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Makes a POST http request.
        /// </summary>
        /// <param name="uri">The Uri to request.</param>
        /// <param name="parameters">Parameters to send.</param>
        /// <param name="headers">Headers to send.</param>
        /// <returns>HttpResponseMessage from the request.</returns>
        public async Task<HttpResponseMessage> PostAsync(string uri, IDictionary<string, string> parameters, IDictionary<string, string>? headers = null)
        {
            var req = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new FormUrlEncodedContent(parameters)
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }

            return await _httpClient.SendAsync(req);
        }

        /// <summary>
        /// Makes a GET http request.
        /// </summary>
        /// <param name="uri">The Uri to request.</param>
        /// <param name="headers">Headers to send.</param>
        /// <returns>HttpResponseMessage from the request.</returns>
        public async Task<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string> headers = null)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, uri);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    req.Headers.Add(header.Key, header.Value);
                }
            }

            return await _httpClient.SendAsync(req);
        }

        public async Task<HttpResponseMessage> PostGraphQLAsync(string uri, string query, IDictionary<string, string>? headers = null)
        {
            var graphQLClient = new GraphQLHttpClient(new GraphQLHttpClientOptions
            {
                EndPoint = new Uri(uri)
            }, new NewtonsoftJsonSerializer());

            var request = new GraphQLRequest
            {
                Query = query
            };
            
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    graphQLClient.HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = await graphQLClient.SendQueryAsync<dynamic>(request);

            return new HttpResponseMessage
            {
                StatusCode = response.Errors?.Any() == true ? HttpStatusCode.BadRequest : HttpStatusCode.OK,
                Content = new StringContent(response.Data?.ToString() ?? string.Empty)
            };
        }
    }
}
