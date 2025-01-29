using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService)
        {
            _articleService = articleService;
        }

        [Route("Articles")]
        public IActionResult Index()
        {
            var articles = _articleService.GetAllArticles();

            if (articles.IsNullOrEmpty())
            {
                articles = new List<Article>();
            }

            return View(articles);
        }

        public IActionResult ViewArticle(int articleID)
        {
            return View(_articleService.GetArticleById(articleID));
        }

        public IActionResult AddArticle()
        {
            var viewModel = new AddArticleVM()
            {
                AvailableCategories = _articleService.GetAllCategoriesSelectList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult AddArticle(AddArticleVM model)
        {
            if (ModelState.IsValid)
            {
                var article = new Article
                {
                    HeadLine = model.HeadLine,
                    LinkText = model.LinkText,
                    ContentSummary = model.ContentSummary,
                    Content = _articleService.GetProcessedArticleContent(model.Content),
                    ImageLink = model.ImageLink,
                    Categories = _articleService.GetSelectedCategories(model.SelectedCategoryIds)
                };

                _articleService.CreateArticle(article);

                TempData["ArticleMessage"] = "The article has been added to the database.";
            }
            else
                TempData["ArticleMessage"] = "Something went wrong";


            return RedirectToAction("ArticleAdded");
        }


        public IActionResult ArticleAdded()
        {
            return View();
        }
    }
}
