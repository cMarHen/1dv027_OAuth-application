using Assignment_wt1.Filters;
using Assignment_wt1.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_wt1.Controllers
{
    /// <summary>
    /// Controller class to handle OAuth sign in.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ISessionHandler _sessionHandler;
        public AuthController(IAuthService authService, ISessionHandler sessionHandler)
        {
            _authService = authService;
            _sessionHandler = sessionHandler;
        }

        public IAuthService Get_authService()
        {
            return _authService;
        }

        /// <summary>
        /// When called, OAuth sign in starts. User will be redirected to another page.
        /// </summary>
        /// <returns>Redirect()</returns>
        [Route("/login")]
        public IActionResult Login()
        {
            try
            {
                return Redirect(_authService.GetAuthUri());
            }
            catch (Exception e)
            {
                // If something went wrong, clear the session.
                _sessionHandler.ClearSession();
                return BadRequest();
            }
        }

        [Route("/logout")]
        public IActionResult Logout()
        {
            _sessionHandler.ClearSession();
            return Redirect("/");
        }

        /// <summary>
        /// When a user has successfully authenticated at OAuth provider, the provider redirects here.
        /// </summary>
        /// <param name="code">Auth code from OAuth provider</param>
        /// <returns>Redirect to /profile if success</returns>
        [Route("/callback")]
        [TypeFilter(typeof(CsrfOAuthActionFilter))]
        public async Task<IActionResult> Callback([FromQuery] string? code)
        {
            try
            {
                await _authService.HandleLogin(code);

                return Redirect("/Profile");
            }
            catch (Exception e)
            {
                // If something went wrong, clear the session.
                _sessionHandler.ClearSession();
                return BadRequest();
            }
        }
    }
}
