using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class AccreditationModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AccreditationId { get; set; }
        public string AccreditationName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyNameKana { get; set; }
        public string Url { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentNameKana { get; set; }
        public string ManagerName { get; set; }
        public string ManagerNameKana { get; set; }
        public string ManagerMailAddress { get; set; }
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string GroupId { get; set; }
    }
}
