using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class NoAdditionalPropertiesModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitCode { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AreaUnitName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ConversionSquareMeters { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? test { get; set; }
    }
}
