using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class AutoKey3DataModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key2 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key3 { get; set; }
    }
}
