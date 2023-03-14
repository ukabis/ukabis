using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class AccreditationCertificationMapModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AccreditationCertificationMapId { get; set; }
        public string CertificationNameId { get; set; }
        public string AccreditationId { get; set; }
    }
}
