using System;
using System.Text.Json.Serialization;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaveElecticityPrices_Isolated
{
    public class SaveElectricityPrices
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public SaveElectricityPrices(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SaveElectricityPrices>();
            _httpClient = new HttpClient();
        }

        [Function("SaveElectricityPrices")]
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer) // Every minute
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }

            var yesterday = DateTime.UtcNow.AddDays(-1);
            var connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            var tableServiceClient = new TableServiceClient(connectionstring);
            var tableClient = tableServiceClient.GetTableClient("ElectricityPrices");

            var prices = await FetchPricesFromApi(yesterday);

            if (prices == null || prices.Count == 0)
            {
                _logger.LogError("Failed to fetch prices from the API.");
                return;
            }

            try
            {
                foreach (var data in prices)
                {
                    string rowKey = $"{data.Region}_{data.hour:00}";  // Använd rätt timme direkt

                    var tableEntity = new TableEntity(yesterday.ToString("yyyy-MM-dd"), rowKey)
                    {
                        {"price_eur", data.price_eur},
                        {"price_sek", data.price_sek},
                        {"region", data.Region},
                        {"hour", data.hour} // Använd rätt timme från API:t
                    };

                    // Save the entity to the table
                    await tableClient.UpsertEntityAsync(tableEntity, TableUpdateMode.Replace);
                }
                _logger.LogInformation($"ElectricityPrices for {yesterday.ToString("yyyy-MM-dd")} saved at {DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                // Log error message
                _logger.LogError($"Error saving electricity price data: {ex.Message}");
            }
        }

        private async Task<List<SERegion>> FetchPricesFromApi(DateTime date)
        {
            try
            {
                var url = $"https://spotprices.lexlink.se/espot/{date:yyyy-MM-dd}";
                var response = await _httpClient.GetStringAsync(url);

                var priceData = JsonSerializer.Deserialize<PriceData>(response);
                if (priceData == null)
                {
                    _logger.LogError("Deserialization failed or API returned null data.");
                    return new List<SERegion>();
                }

                var priceEntities = new List<SERegion>();

                var regions = new Dictionary<IEnumerable<SERegion>, string>
                {
                    { priceData.SE1 ?? Enumerable.Empty<SERegion>(), "SE1" },
                    { priceData.SE2 ?? Enumerable.Empty<SERegion>(), "SE2" },
                    { priceData.SE3 ?? Enumerable.Empty<SERegion>(), "SE3" },
                    { priceData.SE4 ?? Enumerable.Empty<SERegion>(), "SE4" }
                };

                foreach (var (regionData, regionName) in regions)
                {
                    foreach (var data in regionData)
                    {
                        data.Region = regionName;
                        priceEntities.Add(data);
                    }
                }

                //foreach (var entity in priceEntities)
                //{
                //    _logger.LogInformation($"Region: {entity.Region}, Hour: {entity.hour}, EUR Price: {entity.price_eur}, SEK Price: {entity.price_sek}");
                //}

                return priceEntities;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data from the API: {ex.Message}");
                return new List<SERegion>(); // Returnera en tom lista istället för null
            }
        }

        public class PriceData
        {
            public string Date { get; set; }
            public SERegion[] SE1 { get; set; }
            public SERegion[] SE2 { get; set; }
            public SERegion[] SE3 { get; set; }
            public SERegion[] SE4 { get; set; }
        }

        public class SERegion
        {
            public int hour { get; set; }
            public float price_eur { get; set; }
            public float price_sek { get; set; }
            public int kmeans { get; set; }
            [JsonIgnore]
            public string Region { get; set; }
        }
    }
}
