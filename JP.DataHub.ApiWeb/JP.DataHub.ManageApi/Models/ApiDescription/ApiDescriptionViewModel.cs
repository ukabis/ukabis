using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class ApiDescriptionViewModel
    {
        [JsonProperty("ControllerId")]
        public string ApiId { get; set; }
        public string VendorId { get; set; }
        public string VendorName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string VendorUrl { get; set; }
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SystemUrl { get; set; }
        [JsonProperty("ControllerName")]
        public string ApiName { get; set; }
        [JsonProperty("RelativeUrl")]
        public string RelativePath { get; set; }
        [JsonProperty("Documentation", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
        public bool IsStaticApi { get; set; }
        [JsonProperty("Vendor")]
        public bool PartitionByVendor { get; set; }
        [JsonProperty("Person")]
        public bool PartitionByPerson { get; set; }
        public bool IsData { get; set; }
        public bool IsBusinesslogic { get; set; }
        public bool IsPay { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string FeeDescription { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ResourceCreateUser { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ResourceMaintainer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ResourceCreateDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? ResourceLatestDate { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string UpdateFrequency { get; set; }
        public bool NeedContract { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ContactInformation { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string AgreeDescription { get; set; }
        [JsonProperty("CategoryList")]
        public IList<CategoryViewModel> Categories { get; set; }
        public IList<FieldViewModel> Fields { get; set; }
        public IList<TagViewModel> Tags { get; set; }
        [JsonProperty("ApiList")]
        public IList<MethodDescriptionViewModel> Methods { get; set; }
        [JsonProperty("Enable")]
        public bool IsEnable { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
