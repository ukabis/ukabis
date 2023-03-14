using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class JsonSchemaFormatModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TestId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Date { get; set; }
    }
}
