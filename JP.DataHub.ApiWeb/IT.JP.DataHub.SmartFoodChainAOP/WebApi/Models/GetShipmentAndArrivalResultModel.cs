using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class GetShipmentAndArrivalResultModel
    {
        public string ProductCode { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public GetShipmentAndArrivalResultCompanyModel Company { get; set; }
        public int PackageQuantity { get; set; }
        public string ShipmentId { get; set; }
        public string ArrivalId { get; set; }
        public string PreviousShipmentId { get; set; }
    }

    public class GetShipmentAndArrivalResultCompanyModel
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string CompanyName { get; set; }
        public string OfficeName { get; set; }
        public string GlnCode { get; set; }
        public List<NameLangModel> CompanyNameLang { get; set; }
        public List<NameLangModel> OfficeNameLang { get; set; }
    }
}
