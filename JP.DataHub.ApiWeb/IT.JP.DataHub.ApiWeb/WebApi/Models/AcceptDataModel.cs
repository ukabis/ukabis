using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class AcceptDataModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Number { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? NumberOrNull { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? Integer { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? IntegerOrNull { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<AcceptDataArrayItem> AR { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<int> AR2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? Boolean { get; set; }
    }

    public class AcceptDataArrayItem
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ar1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ar2 { get; set; }
    }
}
