using Microsoft.AspNetCore.Mvc;

namespace The_Post.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Local()
        {
            return View();
        }

        public IActionResult Sweden()
        {
            return View();
        }

        public IActionResult World()
        {
            return View();
        }

        public IActionResult Weather()
        {
            return View();
        }

        public IActionResult Economy()
        {
            return View();
        }

        public IActionResult Sports()
        {
            return View();
        }
    }
}
