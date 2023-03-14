using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Api
{
    public class ApiDescriptionModel
    {
        public string ApiId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorUrl { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string SystemUrl { get; set; }
        public string ApiName { get; set; }
        public string RelativePath { get; set; }
        public string Description { get; set; }
        public bool IsStaticApi { get; set; }
        public bool PartitionByVendor { get; set; }
        public bool PartitionByPerson { get; set; }
        public bool IsData { get; set; }
        public bool IsBusinesslogic { get; set; }
        public bool IsPay { get; set; }
        public string FeeDescription { get; set; }
        public string ResourceCreateUser { get; set; }
        public string ResourceMaintainer { get; set; }
        public DateTime? ResourceCreateDate { get; set; }
        public DateTime? ResourceLatestDate { get; set; }
        public string UpdateFrequency { get; set; }
        public bool NeedContract { get; set; }
        public string ContactInformation { get; set; }
        public string Version { get; set; }
        public string AgreeDescription { get; set; }
        public IList<CategoryModel> Categories { get; set; }
        public IList<FieldModel> Fields { get; set; }
        public IList<TagModel> Tags { get; set; }
        public IList<MethodDescriptionModel> Methods { get; set; }
        public bool IsEnable { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
    public class MethodDescriptionModel
    {
        public string MethodId { get; set; }
        public string HttpMethod { get; set; }
        public string RelativePath { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public bool AuthVendorSystem { get; set; }
        public bool AuthOpenId { get; set; }
        public bool AuthAdmin { get; set; }
        public bool IsVendorSystemAuthenticationAllowNull { get; set; }
        public string UrlSchemaId { get; set; }
        public string RequestSchemaId { get; set; }
        public string ResponseSchemaId { get; set; }
        public IEnumerable<MethodLinkModel> MethodLinks { get; set; }
        public IEnumerable<SampleCodeModel> SampleCodes { get; set; }
        public bool IsVisibleSigninuserOnly { get; set; }
        public bool IsEnable { get; set; }
        public bool IsHidden { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
    public class MethodLinkModel : LinkBase
    {
        public string MethodId { get; set; }
        public string MethodLinkId { get; set; }
    }
    public class SampleCodeModel
    {
        public string MethodId { get; set; }
        public string SampleCodeId { get; set; }
        public string Language { get; set; }
        public int DisplayOrder { get; set; }
        public string Code { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }

}
