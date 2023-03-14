﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models
{
    public class CompanyProductModel
    {
        public string GtinCode { get; set; }
        public string CodeType { get; set; }
        public string CompanyId { get; set; }
        public bool IsOrganic { get; set; }
        public ProductProfileModel Profile { get; set; }
        public string RegistrationDate { get; set; }
        public string ProductName { get; set; }
    }
    public class ProductProfileModel
    {
        public string BrandCode { get; set; }
        public string BreedCode { get; set; }
        public string CropCode { get; set; }
        public string GradeCode { get; set; }
        public string ProducingAreaCode { get; set; }
        public string CropTypeCode { get; set; }
        public string ProductTypeCode { get; set; }
        public string SizeCode { get; set; }
    }
}
