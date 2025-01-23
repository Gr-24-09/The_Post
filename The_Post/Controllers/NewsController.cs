using Microsoft.AspNetCore.Mvc;
using The_Post.Data;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class NewsController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _db;

        public NewsController(IArticleService articleService, ApplicationDbContext db)
        {
            _articleService = articleService;
            _db = db;
        }
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
