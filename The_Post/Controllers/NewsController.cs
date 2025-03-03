using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using The_Post.Data;
using The_Post.Models.VM;
using The_Post.Services;
using Azure.Data.Tables;

namespace The_Post.Controllers
{
    public class NewsController : Controller
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly TableClient _tableClient;

        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _db;


        public NewsController(IArticleService articleService, ApplicationDbContext db)
        {
            var connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string for Azure Storage is not set.");
            }

            _articleService = articleService;
            _db = db;

            _tableServiceClient = new TableServiceClient(connectionString);
            _tableClient = _tableServiceClient.GetTableClient("ElectricityPrices");
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CategoryView(string categoryName)
        {
            // Redirects to the weather action if category is weather
            if (categoryName == "Weather")
                return RedirectToAction("Weather", "Article");

            if (categoryName == "Economy")
            {
                return RedirectToAction("HistoricalPrices", new { region = "SE1", date = DateTime.UtcNow.ToString("yyyy-MM-dd") });
            }

            var articles = _articleService.GetAllArticlesByCategoryName(categoryName);

            var model = new CategoryPageVM()
            {
                Articles = articles,
                Category = categoryName
            };


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> HistoricalPrices(string region, string date)
        {
            // Query the table for historical prices
            var partitionKey = date;  // Partition key is the date
            var prices = new List<TableEntity>();

            await foreach (var entity in _tableClient.QueryAsync<TableEntity>(e => e.PartitionKey == partitionKey && e.RowKey.StartsWith(region)))
            {
                prices.Add(entity);
            }

            if (prices.Any())
            {
                return View(prices); // Assuming you'll display them in the view as a list of TableEntities
            }
            else
            {
                return View("NoHistoricalData"); // Create a NoData view to handle cases where no data is found
            }
        }
    }
}
