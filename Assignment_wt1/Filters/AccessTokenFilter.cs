using Assignment_wt1.Interfaces;
using Assignment_wt1.Models;
using Assignment_wt1.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace Assignment_wt1.Filters
{
    public class AccessTokenFilter : IAsyncActionFilter
    {
        private readonly ISessionHandler _sessionHandler;
        private readonly ILogger _logger;
        private readonly IOptionsMonitor<CookieAuthenticationOptions> _cookieOptionsMonitor;
        private readonly IAuthService _authService;

        public AccessTokenFilter(ISessionHandler sessionHandler, ILogger<AccessTokenFilter> logger, IOptionsMonitor<CookieAuthenticationOptions> cookieOptionsMonitor, IAuthService authService)
        {
            _sessionHandler = sessionHandler;
            _logger = logger;
            _cookieOptionsMonitor = cookieOptionsMonitor;
            _authService = authService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                // Get Access_token from Session
                var accessToken = _sessionHandler.GetSession("Access_token")?.ToString();
                if (accessToken == null)
                {
                    _logger.LogWarning("Access token not present in session.");
                    context.Result = new RedirectResult("/");
                    _sessionHandler.ClearSession();
                    return;
                }

                // Get Username from Auth Cookie
                var userId = await _sessionHandler.GetLoginCookie();
                if (userId == null)
                {
                    _logger.LogWarning("User ID not present in cookie.");
                    context.Result = new RedirectResult("/");
                    _sessionHandler.ClearSession();
                    return;
                }

                // Get Username from Session and compare it
                var username = _sessionHandler.GetSession("Username")?.ToString();
                if (username == null || username != userId)
                {
                    _logger.LogWarning("User ID in session does not match the one in the cookie.");
                    context.Result = new RedirectResult("/Error");
                    _sessionHandler.ClearSession();
                    return;
                }

                // Get Refresh_token from Session
                var refreshToken = _sessionHandler.GetSession("Refresh_token")?.ToString();
                if (refreshToken == null)
                {
                    _logger.LogWarning("Refresh token not present in session.");
                    context.Result = new RedirectResult("/");
                    _sessionHandler.ClearSession();
                    return;
                }

                // Get Access Token expiration from Session
                DateTime createdAt = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(_sessionHandler.GetSessionCount("Expires_in"));

                // Make sure Access_token is valid through the request.
                int expirationBuffer = 5; 
                DateTime expiresIn = createdAt.AddSeconds(_sessionHandler.GetSessionCount("Created_at") - expirationBuffer);

                // Refresh Access_token if about to expire
                if (DateTime.UtcNow > expiresIn)
                {
                    await _authService.HandleRefreshToken(refreshToken);

                    await next();
                }

                await next();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing access token filter.");
                context.Result = new RedirectResult("/Error");
                return;
            }
        }
    }
}
