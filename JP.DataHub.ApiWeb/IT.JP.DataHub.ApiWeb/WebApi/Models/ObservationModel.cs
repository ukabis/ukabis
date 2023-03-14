using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ObservationModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string phenomenonTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string phenomenonEndTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string resultTime { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string result { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string thingId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string datastreamId { get; set; }
    }
}
