using Assignment_wt1.Filters;
using Assignment_wt1.Interfaces;
using Assignment_wt1.Models.GroupModels;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Assignment_wt1.Controllers
{
    /// <summary>
    /// Controller class for handling groups. Is protected by AccessTokenFilter.
    /// </summary>
    [TypeFilter(typeof(AccessTokenFilter))]
    [Route("/groups")]
    public class GroupsController : Controller
    {
        private readonly IGroupsService _groupsService;
        private readonly ISessionHandler _sessionHandler;

        public GroupsController(IGroupsService groupsService, ISessionHandler sessionHandler)
        {
            _groupsService = groupsService;
            _sessionHandler = sessionHandler;
        }

        public async Task<IActionResult> Groups()
        {
            try
            {
                CurrentUserModel currentUser = await _groupsService.GetGitlabGroups(_sessionHandler.GetSession("Access_Token") ?? "");
                return View(currentUser);
            }
            catch (Exception e)
            {
                return Redirect("/");
            }
        }

        /// <summary>
        /// When a user POST a request to load more groups or projects.
        /// If there is a provided path, more projects will be added to the user, and groups if no path is provided.
        /// </summary>
        /// <param name="User">CurrentUserModel to fill with new data</param>
        /// <param name="FullPath">Path to a specific group for loading projects</param>
        /// <returns>The same CurrentUserModel, but with more data added.</returns>
        [HttpPost]
        public async Task<IActionResult> Groups(string User, string FullPath)
        {
            try
            {
                var user = JsonSerializer.Deserialize<CurrentUserModel>(User);
                if (FullPath == null)
                {
                    user = await _groupsService.LoadMoreGroups(_sessionHandler.GetSession("Access_Token") ?? "", user);
                } else
                {
                    user = await _groupsService.LoadMoreProjects(_sessionHandler.GetSession("Access_Token") ?? "", user, FullPath);
                }

                return View(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Redirect("/");
            }
        }
    }
}
