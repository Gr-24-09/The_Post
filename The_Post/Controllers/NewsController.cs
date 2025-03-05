using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using The_Post.Data;
using The_Post.Models.VM;
using The_Post.Services;

namespace The_Post.Controllers
{
    public class NewsController : Controller
    {
        private readonly TableServiceClient _tableServiceClient;
        private readonly TableClient _tableClient;

        private readonly IArticleService _articleService;
        private readonly ApplicationDbContext _db;

        public NewsController(IArticleService articleService, ApplicationDbContext db, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("AzureTableStorage:ConnectionStrings:AzureWebJobsStorage").Value;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string for Azure Storage is not set."); // For testing purposes
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
                return RedirectToAction("HistoricalElectricityPrices", new { region = "SE1", date = "2025-03-03" });
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
        public async Task<IActionResult> HistoricalElectricityPrices(string region, string date)
        {
            var partitionKey = date;

            // Create next prefix by increasing the last character
            // e.g., "SE1" -> "SE2"
            string nextRegion = region[..^1] + (char)(region[^1] + 1);

            // Create a filter to query the table
            // eq = equals, ge = greater than or equal to, lt = less than
            // e.g., PartitionKey eq "2025-03-03" and RowKey ge "SE1" and RowKey lt "SE2"
            var filter = TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey} and RowKey ge {region} and RowKey lt {nextRegion}");

            var prices = new List<TableEntity>();

            await foreach (var entity in _tableClient.QueryAsync<TableEntity>(filter))
            {
                prices.Add(entity);
            }

            if (prices.Any())
            {
                return View(prices);
            }
            else
            {
                return View("NoHistoricalData"); // Create a NoData view to handle cases where no data is found
            }
        }
    }
}
