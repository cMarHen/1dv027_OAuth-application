using Assignment_wt1.Interfaces;
using Assignment_wt1.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Assignment_wt1.Services
{
    /// <summary>
    /// Service class to handle profiles.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientService _httpClientService;

        public ProfileService(IConfiguration config, IHttpClientService httpClientService)
        {
            _config = config;
            _httpClientService = httpClientService;
        }

        /// <summary>
        /// To get profile data of the current authenticated user.
        /// </summary>
        /// <param name="token">Access token used to authorize request</param>
        /// <returns>GitlabProfile</returns>
        public async Task<GitlabProfile> GetGitlabProfile(string token)
        {
            try
            {
                var header = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" }
                };

                HttpResponseMessage res = await _httpClientService.GetAsync(_config.GetValue<string>("GitlabLinks:User"), header);  // TODO: USE VARIABLES!

                string resContent = await res.Content.ReadAsStringAsync();
                GitlabProfile profile = JsonSerializer.Deserialize<GitlabProfile>(resContent) ??
                    throw new Exception();

                return profile;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
