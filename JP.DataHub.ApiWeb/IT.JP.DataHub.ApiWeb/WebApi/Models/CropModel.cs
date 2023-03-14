using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class CropModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CropCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CropName { get; set; }
    }
}
