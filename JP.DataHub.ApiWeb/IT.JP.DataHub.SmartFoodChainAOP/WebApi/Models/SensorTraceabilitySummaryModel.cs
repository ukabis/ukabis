using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class SensorTraceabilitySummaryModel
    {
        public string ManagementId { get; set; }
        public DateTime aggregationTime { get; set; }
        public string blobUrl { get; set; }
        public string blobContainer { get; set; }
        public string blobFileName { get; set; }
        public IEnumerable<string> ProductCodes { get; set; }
        public string arrivalId { get; set; }
        public int time { get; set; }
    }
}
