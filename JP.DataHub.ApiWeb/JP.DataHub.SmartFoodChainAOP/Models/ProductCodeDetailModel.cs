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
    public class ProductCodeDetailModel
    {
        public string ProductCode { get; set; }

        public string GtinCode { get; set; }

        public int Quantity { get; set; }

        public ProductCodeFDAModel FDA { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class ProductCodeFDAModel
    {
        public string FoodRegistrationNo { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class TraceabilityModel
    {
        public string ProductCode { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public TraceabilityCompanyModel Company { get; set; }
        public int PackageQuantity { get; set; }
        public string ShipmentId { get; set; }
        public string ArrivalId { get; set; }
        public string PreviousShipmentId { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class TraceabilityCompanyModel
    {
        public string CompanyId { get; set; }
        public string OfficeId { get; set; }
        public string CompanyName { get; set; }
        public string OfficeName { get; set; }
    }
}
