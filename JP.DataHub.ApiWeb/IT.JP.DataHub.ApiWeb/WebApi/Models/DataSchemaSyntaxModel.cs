using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class DataSchemaSyntaxModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Cd { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SelectCd { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CdFrom { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AbcWhere123 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OrderBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? No { get; set; }
    }
}
