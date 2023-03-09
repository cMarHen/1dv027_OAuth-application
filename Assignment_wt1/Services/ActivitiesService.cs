using Assignment_wt1.Interfaces;
using Assignment_wt1.Models;
using Assignment_wt1.Utils;
using System.Text.Json;

namespace Assignment_wt1.Services
{
    /// <summary>
    /// Service class to handle activities.
    /// </summary>
    public class ActivitiesService : IActivitiesService
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private readonly ISessionHandler _sessionHandler;

        public ActivitiesService(IHttpClientService httpClientService, IConfiguration configuration, ISessionHandler sessionHandler)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            _sessionHandler = sessionHandler;
        }

        /// <summary>
        /// Get gitlab activities.
        /// </summary>
        /// <param name="desiredResults">Amount of activities to get.</param>
        /// <returns>A List of GitlabActivities.</returns>
        public async Task<List<GitlabActivities>> GetGitlabActivities(int desiredResults)
        {
            var token = _sessionHandler.GetSession("Access_Token");
            var baseUri = _configuration.GetValue<string>("GitlabLinks:Events");
            var maxPerPage = _configuration.GetValue<int>("GitlabLinks:Gitlab_max_per_page");

            var perPage = Math.Min(desiredResults, maxPerPage);
            var pages = (int)Math.Ceiling((double)desiredResults / perPage);

            var activities = new List<GitlabActivities>();

            // Loop to enable paging if more results is desired than per_page is providing.
            int resultsRemaining = desiredResults;
            for (int i = 1; i <= pages && resultsRemaining > 0; i++)
            {
                var pageUri = $"{baseUri}?per_page={perPage}&page={i}";
                var pageActivities = await GetActivities(pageUri, token);

                // TODO: Unnecessary data is fetched.
                int resultsToAdd = Math.Min(resultsRemaining, pageActivities.Count);
                activities.AddRange(pageActivities.Take(resultsToAdd));

                resultsRemaining -= resultsToAdd;
            }

            return activities;
        }

        /// <summary>
        /// Fetch data to get activities.
        /// </summary>
        /// <param name="uri">Uri to fetch from.</param>
        /// <param name="token">Access token used for authorization.</param>
        /// <returns>A List of gitlab activities.</returns>
        /// <exception cref="Exception">If data could not be fetched.</exception>
        private async Task<List<GitlabActivities>> GetActivities(string uri, string token)
        {
            var header = new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" }
            };

            var response = await _httpClientService.GetAsync(uri, header);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve Gitlab activities. Error {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var activities = JsonSerializer.Deserialize<List<GitlabActivities>>(content);

            return activities;
        }
    }
}
