using Microsoft.AspNetCore.Mvc;

namespace Assignment_wt1.Controllers
{
    [Route("/")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
