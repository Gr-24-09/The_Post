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

        public IActionResult DeleteArticle(int articleID)
        {
            _articleService.DeleteArticle(articleID);
            return RedirectToAction("Index");
        }

        public IActionResult EditArticle(int articleID)
        {
            var article = _articleService.GetArticleById(articleID);

            if(article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        public IActionResult EditArticle(Article updatedArticle)
        {
            if (updatedArticle == null)
            {
                return BadRequest();
            }
            try
            {
                _articleService.UpdateArticle(updatedArticle);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }           
        }

        [HttpPost]
        public async Task<IActionResult> AddArticle(AddArticleVM model)
        {
            model.AvailableCategories = _articleService.GetAllCategoriesSelectList();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (model.ImageLink == null || model.ImageLink.Length == 0)
                {
                    return Content("File not selected");
                }
                else if (!model.ImageLink.ContentType.StartsWith("image/"))
                {
                    throw new ArgumentException("File is not an image");
                }
                string imageUrl = await _articleService.UploadFileToContainer(model);

                var article = new Article
                {
                    HeadLine = model.HeadLine,
                    LinkText = model.LinkText,
                    ContentSummary = model.ContentSummary,
                    Content = _articleService.GetProcessedArticleContent(model.Content),
                    ImageLink = imageUrl,
                    Categories = _articleService.GetSelectedCategories(model.SelectedCategoryIds)
                };

                _articleService.CreateArticle(article);

                TempData["ArticleMessage"] = "The article has been added successfully.";
                return RedirectToAction("ArticleAdded");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("ImageLink", ex.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
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
