using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class TemporaryCertificationModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TemporaryCertificationId { get; set; }
        public string CertificationQuestionnaireId { get; set; }
        public string FilePath { get; set; }
    }
}
