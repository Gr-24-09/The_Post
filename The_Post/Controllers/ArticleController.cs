using Microsoft.AspNetCore.Mvc;

namespace The_Post.Controllers
{
    public class ArticleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
