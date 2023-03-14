using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
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
    
    [MessagePackObject(true)]
    [Serializable]
    public class GetShipmentAndArrivalResultCompanyModel
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string CompanyName { get; set; }
        public List<NameLangModel> CompanyNameLang { get; set; }
        public string OfficeName { get; set; }
        public List<NameLangModel> OfficeNameLang { get; set; }
        public string GlnCode { get; set; }
    }
}
