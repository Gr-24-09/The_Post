using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using The_Post.Models;
using The_Post.Models.API;
using The_Post.Models.VM;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly IRequestService _requestService;
        private readonly UserManager<User> _userManager;

        private readonly IEmployeeService _employeeService;
        public ArticleController(IArticleService articleService, IRequestService requestService, UserManager<User> userManager, IEmployeeService employeeService)
        {
            _articleService = articleService;
            _requestService = requestService;
            _userManager = userManager;
            _employeeService = employeeService;
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

        public async Task<IActionResult> ViewWeather()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);

            // Gets logged-in user's "weather cities"
            var currentCities = loggedInUser.WeatherCities?.Split(',').Where(city => !string.IsNullOrEmpty(city)).ToList() ?? new List<string>();

            // Adds the user's local city if not already included
            if (!currentCities.Contains(loggedInUser.City))
            {
                currentCities.Insert(0, loggedInUser.City); 
            }

            // If no current cities (or local city) an empty list is returned
            if (!currentCities.Any())
            {
                return View(new List<WeatherForecast>());
            }

            List<WeatherForecast> foreCasts = new List<WeatherForecast>();

            // Adds forecast to the foreCasts-list, for each city in currentCities
            foreach (string city in currentCities)
            {
                try
                {
                    var foreCast = await _requestService.GetForecastAsync(city);
                    if (foreCast != null)
                    {
                        foreCasts.Add(foreCast);
                    }
                }
                catch (Exception ex) 
                {
                    // Needs to be changed
                    foreCasts.Add(new WeatherForecast() { City = city, Summary = "Error fetching data" });
                }
            }

            return View(foreCasts);
        }


        // Adds a city to the user's string of cities, and returns a partial view with the new weather card 
        [HttpPost]
        public async Task<IActionResult> AddCity(string city)
        {
            if (string.IsNullOrWhiteSpace(city))
            {
                return BadRequest("No such city name.");
            }

            var user = await _userManager.GetUserAsync(User);

            // Gets the logged-in user's weather cities and puts them in a list
            var cityList = user.WeatherCities?.Split(',').Select(c => c.Trim()).ToList() ?? new List<string>();

            if (cityList.Contains(city, StringComparer.OrdinalIgnoreCase))
            {
                return BadRequest("The city is already displayed.");
            }
            // The new city is added
            cityList.Add(city);

            // Updated the user's Cities-string
            user.WeatherCities = string.Join(",", cityList);
            await _userManager.UpdateAsync(user);

            var weatherData = await _requestService.GetForecastAsync(city); // Gets weather data for the new city

            return PartialView("_WeatherPartial", weatherData);
        }

        // Removes a city from the logged-in user's Cities-string and returns the new updated list of forecasts
        [HttpPost]
        public async Task<IActionResult> RemoveCity(string city)
        {

            // Removes
            await _requestService.RemoveCity(city);

            // Gets the updated list of forecast-objects
            var weatherData = await _requestService.GetForecastsUserAsync();

            return PartialView("_WeatherListPartial", weatherData);
        }
    }
}
