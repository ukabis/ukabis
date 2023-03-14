using System.Collections.Generic;
using Newtonsoft.Json;

namespace JP.DataHub.Com.Net.Http.Models
{
    public class GetStatusResponseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string RequestDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string EndDate { get; set; }
    }
}
