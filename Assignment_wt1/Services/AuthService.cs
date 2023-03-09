
using System.Text.Json;
using Assignment_wt1.Interfaces;
using Assignment_wt1.Utils;
using Assignment_wt1.Models;

namespace Assignment_wt1.Services
{
    /// <summary>
    /// Service class to handle authentication.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientService _httpClientService;
        private readonly ISessionHandler _sessionHandler;
        private readonly ICodeGenerator _codeGenerator;

        public AuthService(IConfiguration config, IHttpClientService httpClientService, ISessionHandler sessionHandler, ICodeGenerator codeGenerator)
        {
            _config = config;
            _httpClientService = httpClientService;
            _sessionHandler = sessionHandler;
            _codeGenerator = codeGenerator;
        }

        /// <summary>
        /// When a user have a authentication token from OAuth provider, call this method to retrieve data.
        /// Will store cookies to authenticate and authorize the user through the application.
        /// </summary>
        /// <param name="code">Authentication code from OAuth provider.</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException"></exception>
        public async Task HandleLogin(string code)
        {
            try
            {
                // Set parameters
                var parameters = new Dictionary<string, string>
                {
                    { "client_id", _config["Gitlabconfig:ApplicationID"] },
                    { "client_secret", _config["Gitlabconfig:TokenSecret"] },
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", _config["Gitlabconfig:RedirectUri"] },
                    { "code_verifier", _sessionHandler.GetSession("code_verifier") }
                };

                HttpResponseMessage res = await _httpClientService.PostAsync(_config["Gitlabconfig:TokenUri"], parameters);

                string resContent = await res.Content.ReadAsStringAsync();

                // Deserialize the data to object
                GitlabAccessData data = JsonSerializer.Deserialize<GitlabAccessData>(resContent) ??
                    throw new Exception();

                IJwtHandler jwtHandler = new JwtHandler();
                Dictionary<string, string> list = jwtHandler.ExtractFieldsFromJwt(data.id_token, new List<string>() { "groups_direct", "sub" });

                // Populate session storage
                _sessionHandler.StorePairInSession("Access_Token", data.access_token ?? "");
                _sessionHandler.StorePairInSession("Id_Token", data.id_token ?? "");
                _sessionHandler.StorePairInSession("Username", list["sub"]);
                _sessionHandler.StorePairInSession("Refresh_token", data.refresh_token ?? "");
                _sessionHandler.StorePairInSession("Created_at", data.created_at);
                _sessionHandler.StorePairInSession("Expires_in", data.expires_in);

                // Set login cookie
                await _sessionHandler.StoreLoginCookie(list["sub"]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpRequestException(e.Message);
            }
        }

        /// <summary>
        /// Refreshes the access token using the provided refresh token and updates the session storage with the new tokens.
        /// </summary>
        /// <param name="refreshToken">Refresh token used to refresh access_token</param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">Thrown when the token refresh request fails.</exception>
        public async Task HandleRefreshToken(string refreshToken)
        {
            try
            {
                // Set parameters
                var parameters = new Dictionary<string, string>
                {
                    { "client_id", _config["Gitlabconfig:ApplicationID"] },
                    { "client_secret", _config["Gitlabconfig:TokenSecret"] },
                    { "refresh_token", refreshToken },
                    { "grant_type", "refresh_token" },
                    { "redirect_uri", _config["Gitlabconfig:RedirectUri"] },
                    { "code_verifier", _sessionHandler.GetSession("code_verifier") }
                };

                HttpResponseMessage res = await _httpClientService.PostAsync(_config["Gitlabconfig:TokenUri"], parameters);

                string resContent = await res.Content.ReadAsStringAsync();

                // Deserialize the data to object
                GitlabAccessData data = JsonSerializer.Deserialize<GitlabAccessData>(resContent) ??
                    throw new Exception();

                if (data.access_token == null)
                {
                    throw new Exception("New Access token is null");
                }

                // Update session storage
                _sessionHandler.StorePairInSession("Access_Token", data.access_token ?? "");
                _sessionHandler.StorePairInSession("Id_Token", data.id_token ?? "");
                _sessionHandler.StorePairInSession("Refresh_token", data.refresh_token ?? "");
                _sessionHandler.StorePairInSession("Created_at", data.created_at);
                _sessionHandler.StorePairInSession("Expires_in", data.expires_in);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new HttpRequestException(e.Message);
            }
        }

        /// <summary>
        ///     Gets a URI to authorize OAuth on GitLab
        ///     Example:
        ///     https://gitlab.lnu.se/oauth/authorize?client_id=ApplicationID&redirect_uri=RedirectUri&response_type=code&state=statement123&scope=ReqScopes
        /// </summary>
        /// <returns>A string representing authorization uri</returns>
        public string GetAuthUri()
        {
            GitlabAuthOptions authOptions = _config.GetSection("Gitlabconfig").Get<GitlabAuthOptions>();

            // Gather state
            var state = _codeGenerator.GenerateCode(20, 40);
            authOptions.State = state;
            _sessionHandler.StorePairInSession("oauth_state", state);

            // Gather code_verifier
            string codeVerifier = _codeGenerator.GenerateCode(43, 128);
            _sessionHandler.StorePairInSession("code_verifier", codeVerifier);
            authOptions.CodeChallenge = _codeGenerator.GenerateCodeChallenge(codeVerifier);

            return authOptions.ToString();
        }
    }
}
