using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ParentCertificationNameModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ParentCertificationNameId { get; set; }
        public string ParentCertificationName { get; set; }
        public bool IsPublic { get; set; }
        public string CertificationAuthorityId { get; set; }
        public string Url { get; set; }
        public string CountryCode { get; set; }
        public int OrderNo { get; set; }
        public bool IsEnable { get; set; }
    }
}
