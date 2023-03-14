using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CertificationIssueHistoryModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CertificationIssueHistoryId { get; set; }
        public string CertificationApplyId { get; set; }
        public DateTime IssueDate { get; set; }
        public bool IsTemporary { get; set; }
        public string TemporaryCertificationId { get; set; }
    }
}
