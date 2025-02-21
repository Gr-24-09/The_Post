using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using The_Post.Data;
using The_Post.Models.VM;
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

        public  IActionResult CategoryView(string categoryName)
        {
            // Redirects to the weather action if category is weather
            if (categoryName == "Weather")
                return RedirectToAction("Weather", "Article");

            var articles = _articleService.GetAllArticlesByCategoryName(categoryName);

            var model = new CategoryPageVM()
            {
                Articles = articles,
                Category = categoryName
            };

            
            return View(model);
        }
    }
}
