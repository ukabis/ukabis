using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ShipmentModel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ShipmentId { get; set; }

        /// <summary>
        /// 出荷元
        /// </summary>
        public ShipmentCompanyModel Shipment { get; set; }

        /// <summary>
        /// 送り先
        /// </summary>
        public ShippingModel Shipping { get; set; }

        public DeliveryModel Delivery { get; set; }

        public bool? IsInHouseDelivery { get; set; }

        public List<ShipmentProductModel> ShipmentProducts { get; set; }

        public string InvoiceCode { get; set; }

        public DateTime? ShipmentDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public string ShipmentTypeCode { get; set; }

        public string ProducingAreaCode { get; set; }

        public List<string> FirstShipmentId { get; set; }

        public List<string> PreviousShipmentId { get; set; }

        public string Message { get; set; }

        //public Shipment_ShipmentFarmerModel ShipmentFarmer { get; set; }
    }

    public class ShipmentCompanyModel
    {
        public string ShipmentCompanyId { get; set; }

        public string ShipmentOfficeId { get; set; }

        public string ShipmentGln { get; set; }
    }

    public class ShippingModel
    {
        public string ShippingCompanyId { get; set; }

        public string ShippingOfficeId { get; set; }

        public string ShippingGln { get; set; }
    }

    public class DeliveryModel
    {
        public string DeliveryCompanyId { get; set; }

        public string DeliveryOfficeId { get; set; }

        public string ShipmentMethodCode { get; set; }
    }

    public class ShipmentProductModel
    {
        public string ProductCode { get; set; }

        public string InvoiceCode { get; set; }

        public string CropCode { get; set; }

        public string BreedCode { get; set; }

        public string BrandCode { get; set; }

        public string GradeCode { get; set; }

        public string SizeCode { get; set; }

        public int Quantity { get; set; }

        public int? PackageQuantity { get; set; }

        public int?  SinglePackageWeight{ get; set; }

        public string CapacityUnitCode { get; set; }

        public List<ShipmentProductDetailModel> ProductDetail { get; set; }

        public string Message { get; set; }

        public Guid? PackagingId { get; set; }

        public List<ShipmentArrivalProductMap> ArrivalProductMap { get; set; }

        public List<Guid> AttachFileId { get; set; }

        //public Shipment_FarmerShipmentProductModel FarmerShipmentProduct { get; set; }
    }

    public class ShipmentProductDetailModel
    {
        public Guid? ProductArivalId { get; set; }

        public string InvoiceCode { get; set; }
    }

    public class ShipmentArrivalProductMap
    {
        public string ArrivalId { get; set; }

        public string ArrivalProductCode { get; set; }
    }
}
