using System;
using System.Text.Json.Serialization;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http.Json;

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
        public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
            }
            var yesterday = DateTime.UtcNow.AddDays(-1);

          //  var priceEntities = new List<ElectricityPriceEntity>();

            var connectionstring = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

            var tableServiceClient = new TableServiceClient(connectionstring);

            var tableClient = tableServiceClient.GetTableClient("ElectricityPrices");

            var prices = await FetchPricesFromApi(yesterday);

            if(prices==null)
            {
                _logger.LogError("Failed to fetch prices from the API.");
                return;
            }

            //foreach(var priceData in prices)
            //{
            //    var timeslot = DateTime.Parse(priceData.TimeStamp);

            //    var priceEntity = new ElectricityPriceEntity(
            //         partitionKey: priceData.Region,
            //         rowkey: timeslot.ToString("HH:mm"),
            //         price: priceData.Price,
            //         timestamp: DateTime.UtcNow,
            //         region: priceData.Region);

            //    priceEntities.Add(priceEntity);
            //}
           
            try
            {
                foreach (var data in prices)
                {
                    for (int hour = 0; hour < 24; hour++)
                    {
                        var timeslot = new DateTime(yesterday.Year, yesterday.Month, yesterday.Day, data.Hour, 0, 0);
                        var tableEntity = new TableEntity(yesterday.ToString("yyyy-MM-dd"), $"{data.Region}_{hour:00}")
                        //var tableEntity = new TableEntity(data.Region, timeslot.ToString("HH:mm"))
                       {

                          {"price_eur",data.price_eur },
                          {"price_sek",data.price_sek },
                          {"timestamp",DateTime.UtcNow},
                          {"region",data.Region },
                          {"hour",hour }
                  };

                        await tableClient.UpsertEntityAsync(tableEntity, TableUpdateMode.Replace);
                    }
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
                var priceEntities = new List<SERegion>();

                // Flattening each region (SE1, SE2, SE3, SE4)
                foreach (var region in new[] { priceData.SE1, priceData.SE2, priceData.SE3, priceData.SE4 })
                {
                        string regionName = region == priceData.SE1 ? "SE1" :
                                         region == priceData.SE2 ? "SE2" :
                                         region == priceData.SE3 ? "SE3" : "SE4";

                        foreach (var data in region)
                        {
                            data.Region = regionName;  // Ensure Region is never null
                            priceEntities.Add(data);
                        }  
                }
                return priceEntities;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data from the API: {ex.Message}");
                return null;
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
            public int Hour { get; set; }
            public float price_eur { get; set; }
            public float price_sek { get; set; }
            public int kmeans { get; set; }
            [JsonIgnore]
            public string Region { get; set; }
        }
    } 
}
