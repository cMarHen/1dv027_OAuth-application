using Assignment_wt1.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Assignment_wt1.Utils
{
    public class SessionHandler : ISessionHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public SessionHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        /// <summary>
        /// Clear all data from session, along with login cookie.
        /// </summary>
        public void ClearSession()
        {
            _contextAccessor.HttpContext?.Session.Clear();
            _contextAccessor.HttpContext?.SignOutAsync();
        }

        /// <summary>
        /// Clear a specific session by key.
        /// </summary>
        /// <param name="key">Key to remove.</param>
        public void ClearSession(string key)
        {
            _contextAccessor.HttpContext?.Session.Remove(key);
        }

        /// <summary>
        /// Get session value from key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <returns></returns>
        public string GetSession(string key)
        {
            return _contextAccessor.HttpContext?.Session.GetString(key) ?? "";
        }

        /// <summary>
        /// Get session value as Integer from key.
        /// </summary>
        /// <param name="key">Key to get.</param>
        /// <returns></returns>
        public int GetSessionCount(string key)
        {
            return _contextAccessor.HttpContext?.Session.GetInt32(key) ?? -1;
        }

        /// <summary>
        /// Store a key, value pair in session where value is a String.
        /// </summary>
        /// <param name="key">The key to store.</param>
        /// <param name="value">The value as String to store with the key.</param>
        public void StorePairInSession(string key, string value)
        {
            _contextAccessor.HttpContext?.Session.SetString(key, value);
        }

        /// <summary>
        /// Store a key, value pair in session where value is an Integer.
        /// </summary>
        /// <param name="key">The key to store.</param>
        /// <param name="value">The value as Integer to store with the key.</param>
        public void StorePairInSession(string key, int value)
        {
            _contextAccessor.HttpContext?.Session.SetInt32(key, value);
        }

        // TODO: Encrypt!
        /// <summary>
        /// Store login state to the browser as a cookie containing a username.
        /// </summary>
        /// <param name="username">The username to store</param>
        /// <returns></returns>
        public async Task StoreLoginCookie(string username)
        {
            var claims = new List<Claim>
            {
                new Claim("Username", username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await _contextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        /// <summary>
        /// Get value of login cookie.
        /// </summary>
        /// <returns>Return the username of the logged in user.</returns>
        public async Task<string> GetLoginCookie()
        {
            var result = await _contextAccessor.HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (result?.Principal?.Claims != null)
            {
                var usernameClaim = result.Principal.Claims.FirstOrDefault(c => c.Type == "Username");
                if (usernameClaim != null)
                {
                    return usernameClaim.Value;
                }
            }
            return null;
        }
    }
}
