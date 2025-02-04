using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<User> _userManager;

        public ArticleController(IArticleService articleService, UserManager<User> userManager)
        {
            _articleService = articleService;
            _userManager = userManager;
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
            var article = _articleService.GetArticleById(articleID);
            article.Views++;
            _articleService.UpdateArticle(article);

            return View(article);
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
        
        // Also removes a like if the user has already liked the article.
        [HttpPost]
        public async Task<IActionResult> LikeArticle(int articleId)
        {
            // Gets the logged in user
            var loggedInUser = await _userManager.GetUserAsync(User);

            // If the user isn't logged in the return value is -1 (which triggers an error message in the view)
            if (loggedInUser == null)
                return Json(-1);

            // Add or remove like to the datbase, depending on whether or not the user has liked
            // the article before.
            await _articleService.AddRemoveLikeAsync(articleId, loggedInUser.Id);

            // Get the updated like count
            var updatedLikes = await _articleService.GetLikeCountAsync(articleId);

            return Json(updatedLikes);
        }

    }
}
