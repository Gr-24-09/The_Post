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

        public async Task<IActionResult> CategoryView(string categoryName)
        {
            var model = new CategoryPageVM()
            {
                //Articles = articles,
                Category = categoryName
            };
            
            // Redirects to the weather action if category is weather
            if (categoryName == "Weather")
                return RedirectToAction("Weather", "Article");

            var articles = _articleService.GetAllArticlesByCategoryName(categoryName);
            model.Articles = articles;

            if (categoryName == "Economy")
            {
                try
                {
                    var date = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                    //var regions = new List<string> { "SE1", "SE2", "SE3", "SE4" };
                    var hours = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
                    var allPrices = new List<TableEntity>();

                    foreach (var hour in hours) //iterating through all the hours
                    {
                        var prices = await GetHistoricalElectricityPrices(hour, date);
                        allPrices.AddRange(prices);
                    }
                    if (allPrices.Any())
                    {
                        model.HistoricalElectricityPrices = allPrices;  // Assign prices to the view model
                    }

                }
                catch (Exception ex)
                {
                    // Log any errors that occur during the query
                    Console.WriteLine($"Error querying historical electricity prices: {ex.Message}");
                    model.NoHistoricalData = true;  // Flag to indicate that there was an error
                }
            }
            return View(model);
        }   
        

        private async Task<List<TableEntity>> GetHistoricalElectricityPrices(string hour, string date)
        {
            //var partitionKey = date;
            if (string.IsNullOrEmpty(hour) || string.IsNullOrEmpty(date))
            {
                // Handle the case where region is null or empty
                Console.WriteLine("Hour is null or empty. Skipping the operation.");

                return new List<TableEntity>();
            }
                var partitionKey = date;

                var rowKey = hour;

                //var hourNumber = int.Parse(hour);  //Directly parsing the hour

            //var regionBase = region.Substring(0, region.Length - 1); // "SE"
            //var regionNumber = int.Parse(region.Substring(region.Length - 1)); // "1" -> 1

              //string nextHour = (hourNumber + 1).ToString("00");

            // Increment the numeric part to get the next region
                //regionNumber++;

            // Combine the base and the incremented numeric part to get the next region (e.g., "SE1" becomes "SE2")
               //string nextRegion = regionBase + (regionNumber+1).ToString();

            // Create the query filter
            var filter = TableClient.CreateQueryFilter($"PartitionKey eq {partitionKey} and RowKey eq {rowKey}");

            var prices = new List<TableEntity>();
            await foreach (var entity in _tableClient.QueryAsync<TableEntity>(filter))
            {
                prices.Add(entity);
            }

            return prices;
        }


        [HttpGet]
        public async Task<IActionResult> HistoricalElectricityPrices(string date)
        {
            var model = new HistoricalElectricityPricesViewModel();
            var regions = new List<String> { "SE1", "SE2", "SE3", "SE4" };
            var hours = new List<string> { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23" };
                

            var allPrices = new List<TableEntity>();
          
            try
            {
                var queryDate = string.IsNullOrEmpty(date) ? DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd") : date;
                //var regions = new List<String> { "SE1","SE2","SE3","SE4" };

                //var allPrices = new List<TableEntity>();
                if (hours != null && hours.Any())
                { 
                    foreach (var hour in hours)  //we had regions before.Replaced region with hour
                {
                    var prices = await GetHistoricalElectricityPrices(hour, queryDate);
                    allPrices.AddRange(prices); // Add fetched prices to the list
                    }
                 }

                if (allPrices.Any())
                {
                    model.ElectricityPrices = allPrices;
                    model.SelectedDate = queryDate;
                }
                else
                {
                    model.NoHistoricalData = true;
                }
            }
            catch (Exception ex)
            {
                // Log error if anything goes wrong
                Console.WriteLine($"Error querying historical electricity prices: {ex.Message}");
                model.ErrorOccurred = true;
            }
            return View(model);
        }

    }
    }

