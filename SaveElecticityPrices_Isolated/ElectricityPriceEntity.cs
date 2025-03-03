using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveElecticityPrices_Isolated
{
    internal class ElectricityPriceEntity
    {
        public string Region { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public double Price { get; set; }

        public DateTime TimeStamp { get; set; }

        public ElectricityPriceEntity()
        {

        }
        public ElectricityPriceEntity(string region,string partitionKey,string rowkey,double price, DateTime timestamp)
        {
            Region = region;
            PartitionKey = partitionKey;
            RowKey = rowkey;
            Price = price;
            TimeStamp = timestamp;
        }
    }
}