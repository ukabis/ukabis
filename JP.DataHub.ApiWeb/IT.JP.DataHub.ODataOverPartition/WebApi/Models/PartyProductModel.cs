using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ODataOverPartition.WebApi.Models
{
    public class PartyProductModel
    {
        public string GtinCode { get; set; }
        public string CodeType { get; set; }
        public string PartyId { get; set; }
        public bool IsOrganic { get; set; }
        public PartyProductProfileModel Profile { get; set; }
        public string RegistrationDate { get; set; }
        public string ProductName { get; set; }
    }
    public class PartyProductProfileModel
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
