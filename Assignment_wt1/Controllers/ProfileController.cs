using Assignment_wt1.Filters;
using Assignment_wt1.Interfaces;
using Assignment_wt1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_wt1.Controllers
{
    /// <summary>
    /// Controller class for handling profile. Is protected by AccessTokenFilter.
    /// </summary>
    [TypeFilter(typeof(AccessTokenFilter))]
    [Route("/profile")]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly ISessionHandler _sessionHandler;

        public ProfileController(IProfileService profileService, ISessionHandler sessionHandler)
        {
            _profileService = profileService;
            _sessionHandler = sessionHandler;
        }

        public async Task<IActionResult> Profile()
        {
            GitlabProfile viewData = await _profileService.GetGitlabProfile(_sessionHandler.GetSession("Access_Token") ?? "");
            return View(viewData);
        }
    }
}
