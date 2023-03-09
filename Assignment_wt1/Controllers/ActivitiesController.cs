using Assignment_wt1.Filters;
using Assignment_wt1.Interfaces;
using Assignment_wt1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_wt1.Controllers
{
    /// <summary>
    /// Controller class for handling activities. Is protected by AccessTokenFilter.
    /// </summary>
    [TypeFilter(typeof(AccessTokenFilter))]
    [Route("/activities")]
    public class ActivitiesController : Controller
    {
        private readonly IActivitiesService _activitiesService;

        public ActivitiesController(IActivitiesService activitiesService)
        {
            _activitiesService = activitiesService;
        }

        /// <summary>
        /// GET /activities.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Activities()
        {

            return View();
        }

        /// <summary>
        /// When a user POST a request of viewing Gitlab activities.
        /// </summary>
        /// <param name="count">Amount of activities to show.</param>
        /// <returns>View with view data</returns>
        [HttpPost]
        public async Task<IActionResult> Activities(int count)
        {
            try
            {
                List<GitlabActivities> viewData = await _activitiesService.GetGitlabActivities(count);

                return View(viewData);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Redirect("/Error");
            }
        }
    }
}
