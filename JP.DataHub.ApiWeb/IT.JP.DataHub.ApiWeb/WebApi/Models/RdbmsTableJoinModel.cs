using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class RdbmsTableJoinModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string STR_VALUE { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JOIN_KEY { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string JOIN_VALUE { get; set; }
    }
}
