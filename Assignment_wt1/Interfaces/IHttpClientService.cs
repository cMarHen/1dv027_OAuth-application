namespace Assignment_wt1.Interfaces
{
    public interface IHttpClientService
    {
        Task<HttpResponseMessage> PostAsync(string uri, IDictionary<string, string> data, IDictionary<string, string> headers = null);
        Task<HttpResponseMessage> GetAsync(string uri, IDictionary<string, string> headers = null);
        public Task<HttpResponseMessage> PostGraphQLAsync(string uri, string query, IDictionary<string, string>? headers = null);
    }
}
