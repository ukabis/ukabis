using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ReferenceNotifyODataModelEx : BaseModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? temp { get; set; }

        // NULLでもプロパティあり
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<ReferenceNotifySourceModelEx> Ref1 { get; set; }
    }
}
