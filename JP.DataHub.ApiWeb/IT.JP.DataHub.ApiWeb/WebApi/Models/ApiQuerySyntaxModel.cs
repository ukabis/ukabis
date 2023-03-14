using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class ApiQuerySyntaxModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Cd { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? No { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<ApiQuerySyntaxChild> Children { get; set; }
    }

    public class ApiQuerySyntaxChild
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ChildCd { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ChildName { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? ChildNo { get; set; }
    }
}
