using Microsoft.AspNetCore.Mvc;

namespace Assignment_wt1.Controllers
{
    [Route("/Error")]
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View();
        }
    }
}
