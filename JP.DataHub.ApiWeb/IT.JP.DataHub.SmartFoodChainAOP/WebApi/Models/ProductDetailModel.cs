using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class ProductDetailModel
    {
        public string ProductCode { get; set; }
        public string GtinCode { get; set; }
        public ProductDetailFDAModel? FDA { get; set; }
        public int Quantity { get; set; }
    }

    public class ProductDetailFDAModel
    {
        public string FoodRegistrationNo { get; set; }
    }
}
