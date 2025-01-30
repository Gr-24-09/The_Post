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

        public  IActionResult Local()
        {
            var local = _articleService.GetAllArticlesByCategory(1);
            return View(local);
        }

        public IActionResult Sweden()
        {
            var sweden = _articleService.GetAllArticlesByCategory(2);
            return View(sweden);
                
        }

        public IActionResult World()
        {
            var world = _articleService.GetAllArticlesByCategory(3);
            return View(world);
        }

        public IActionResult Weather()
        {
            var weather = _articleService.GetAllArticlesByCategory(4);
            return View(weather);
        }

        public IActionResult Economy()
        {
            var economy = _articleService.GetAllArticlesByCategory(5);
            return View(economy);
        }

        public IActionResult Sports()
        {
            var sports = _articleService.GetAllArticlesByCategory(6);
            return View(sports);
        }
    }
}
