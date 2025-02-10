using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using The_Post.Models;
using The_Post.Models.VM;
using The_Post.Services;
using System.ComponentModel.DataAnnotations;

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

        [HttpGet]
        public IActionResult AddArticle()
        {
            var viewModel = new AddArticleVM()
            {
                AvailableCategories = _articleService.GetAllCategoriesSelectList()
            };

            return View(viewModel);
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

        public IActionResult DeleteArticle(int articleID)
        {
            _articleService.DeleteArticle(articleID);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult EditArticle(int articleID)
        {
            var article = _articleService.GetArticleById(articleID);

            if(article == null)
            {
                return NotFound();
            }

            var vm = new EditArticleVM()
            {
                Id = article.Id,
                HeadLine = article.HeadLine,
                LinkText = article.LinkText,
                ContentSummary = article.ContentSummary,
                Content = article.Content,
                SelectedCategoryIds = article.Categories.Select(c => c.Id).ToList(),
                AvailableCategories = _articleService.GetAllCategoriesSelectList()
            };

            ViewBag.CurrentImage = article.ImageLink;
            TempData["ID"] = article.Id;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> EditArticle(EditArticleVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AvailableCategories = _articleService.GetAllCategoriesSelectList();               
                return View(vm);
            }
            try
            {
                var article = _articleService.GetArticleById(vm.Id);
                if (article == null)
                {
                    return NotFound("Can't find the article!");
                }

                article.HeadLine = vm.HeadLine;
                article.LinkText = vm.LinkText;
                article.ContentSummary = vm.ContentSummary;
                article.Content = _articleService.GetProcessedArticleContent(vm.Content);

                if (vm.ImageLink != null)
                {
                    if (!vm.ImageLink.ContentType.StartsWith("image/"))
                    {
                        ModelState.AddModelError("ImageLink", "File is not an image");
                        vm.AvailableCategories = _articleService.GetAllCategoriesSelectList();
                        return View(vm);
                    }

                    var imageUrl = await _articleService.UploadFileToContainer(new AddArticleVM { ImageLink = vm.ImageLink });
                    article.ImageLink = imageUrl;
                }

                article.Categories = _articleService.GetSelectedCategories(vm.SelectedCategoryIds);
                _articleService.UpdateArticle(article);

                TempData["Success"] = "Article updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                vm.AvailableCategories = _articleService.GetAllCategoriesSelectList();
                return View(vm);
            }           
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
