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
    public class ProductModel
    {
        public string GtinCode { get; set; }
        public string CodeType { get; set; }
        public string CompanyId { get; set; }
        public bool IsOrganic { get; set; }
        public ProductProfileModel Profile { get; set; }
        public string RegistrationDate { get; set; }
        public string ProductName { get; set; }
    }
    
    [MessagePackObject(true)]
    [Serializable]
    public class ProductProfileModel
    {
        public string BrandCode { get; set; }
        public string BreedCode { get; set; }
        public string CropCode { get; set; }
        public string GradeCode { get; set; }
        public string ProducingAreaCode { get; set; }
        public string ProductTypeCode { get; set; }
        public string SizeCode { get; set; }
    }
}
