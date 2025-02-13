using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using The_Post.Models;
using The_Post.Services;
using The_Post.Data;
using The_Post.Models.VM;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            bool isCookiesAccepted = _articleService.IsCookiesAccepted();
            ViewBag.IsCookiesAccepted = isCookiesAccepted;
            ArticleQueriesVM obj = new ArticleQueriesVM();
            obj.GetFiveMostPopularArticles = _articleService.GetFiveMostPopularArticles();
            obj.GetEditorsChoiceArticles = _articleService.GetEditorsChoiceArticles();
            obj.TenLatestArticles = _articleService.TenLatestArticles();
            return View(obj);
           
        }
        

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult CookiesNotice()
        {
            bool isCookiesAccepted = _articleService.IsCookiesAccepted();
            ViewBag.IsCookiesAccepted = isCookiesAccepted;
            return View();
        }

        [HttpPost]
        public IActionResult AcceptCookies()
        {
            // Set the cookie to mark the user has accepted cookies
            _articleService.AcceptCookies();

            return NoContent();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
