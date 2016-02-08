using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Authorization;

namespace CookieAuthenticationMiddleware.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secure()
        {
            ViewData["Message"] = "This is a secured page";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
