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
    public class ShipmentModel
    {
        public string ShipmentId { get; set; }

        /// <summary>
        /// 出荷元
        /// </summary>
        public ShipmentCompanyModel Shipment { get; set; }

        /// <summary>
        /// 送り先
        /// </summary>
        public ShippingModel Shipping { get; set; }

        public List<ShipmentProductModel> ShipmentProducts { get; set; }

        public DateTime ShipmentDate { get; set; }

        public List<string> PreviousShipmentId { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class ShipmentCompanyModel
    {
        public string ShipmentCompanyId { get; set; }

        public string ShipmentOfficeId { get; set; }

        public string ShipmentGln { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class ShippingModel
    {
        public string ShippingCompanyId { get; set; }

        public string ShippingOfficeId { get; set; }

        public string ShippingGln { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class ShipmentProductModel
    {
        public string ProductCode { get; set; }
        public int PackageQuantity { get; set; }
        public List<ArrivalProductMap> ArrivalProductMap { get; set; }
    }

    [MessagePackObject(true)]
    [Serializable]
    public class ArrivalProductMap
    {
        public string ArrivalId { get; set; }
        public string ArrivalProductCode { get; set; }
    }
}
