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
    public class ArrivalModel
    {
        public string ArrivalId { get; set; }
        public string ShipmentTypeCode { get; set; }
        public DateTime ArrivalDate { get; set; }
        public string InvoiceCode { get; set; }
        public string ShipmentId { get; set; }
        public string ShipmentCompanyId { get; set; }
        public string ShipmentOfficeId { get; set; }
        public string ShipmentGln { get; set; }
        public string ArrivalCompanyId { get; set; }
        public string ArrivalOfficeId { get; set; }
        public string ArrivalGln { get; set; }
        public List<ArrivalProductModel> ArrivalProduct { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class ArrivalProductModel
    {
        public string ProductCode { get; set; }
        public int Quantity { get; set; }
        public int DamageQuantity { get; set; }
        public string InvoiceCode { get; set; }
        public string CropCode { get; set; }
        public string BreedCode { get; set; }
        public string BrandCode { get; set; }
        public string GradeCode { get; set; }
        public string SizeCode { get; set; }
        public int PackageQuantity { get; set; }
        public int SinglePackageWeight { get; set; }
        public string CapacityUnitCode { get; set; }
        public int ReceivePackageQuantity { get; set; }
        public int DamagePackageQuantity { get; set; }
    }
}
