using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ApplicationModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string[] Manager { get; set; }
        public bool IsEnable { get; set; }
    }

    public class ApplicationIsManagerModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationId { get; set; }
    }
}
