using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using The_Post.Models;
using The_Post.Services;
using The_Post.Data;

namespace The_Post.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, IArticleService articleService, ApplicationDbContext db)
        {
            _logger = logger;            
            _articleService = articleService;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CookiesNotice()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
