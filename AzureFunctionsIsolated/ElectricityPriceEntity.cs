using Azure.Data.Tables;

namespace SaveElecticityPrices_Isolated
{
    internal class ElectricityPriceEntity
    {
        public string Region { get; set; }
        public string PartitionKey { get; set; } // Date
        public string RowKey { get; set; } // Region + Hour (e.g., "SE1_00")
        public double PriceEur { get; set; }
        public double PriceSek { get; set; }
        public int KMeans { get; set; }
        public DateTime Timestamp { get; set; }

        public ElectricityPriceEntity() { }

        public ElectricityPriceEntity(string region, string partitionKey, string rowkey, double priceEur, double priceSek, int kmeans, DateTime timestamp)
        {
            Region = region;
            PartitionKey = partitionKey;
            RowKey = rowkey;
            PriceEur = priceEur;
            PriceSek = priceSek;
            KMeans = kmeans;
            Timestamp = timestamp;
        }
    }











    //internal class ElectricityPriceEntity
    //{
    //    public string Region { get; set; }

    //    public string PartitionKey { get; set; }

    //    public string RowKey { get; set; }

    //    public double Price { get; set; }

    //    public DateTime TimeStamp { get; set; }

    //    public ElectricityPriceEntity()
    //    {

    //    }
    //    public ElectricityPriceEntity(string region, string partitionKey, string rowkey, double price, DateTime timestamp)
    //    {
    //        Region = region;
    //        PartitionKey = partitionKey;
    //        RowKey = rowkey;
    //        Price = price;
    //        TimeStamp = timestamp;
    //    }
    //}
}