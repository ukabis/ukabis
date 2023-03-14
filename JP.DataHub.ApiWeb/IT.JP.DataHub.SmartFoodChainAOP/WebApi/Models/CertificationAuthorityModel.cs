using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CertificationAuthorityModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificationAuthorityId { get; set; }
        public string CertificationAuthorityName { get; set; }
    }
}
