using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    /// <summary>
    /// ArrayDataがnullプロパティあり
    /// </summary>
    public class ResourceSharingModel : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string key1 { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string kid { get; set; }
    }
}
