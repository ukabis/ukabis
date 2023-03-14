using System;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class SchemaModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SchemaName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JsonSchema { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime UpdDate { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? VendorId { get; set; }
    }
}
