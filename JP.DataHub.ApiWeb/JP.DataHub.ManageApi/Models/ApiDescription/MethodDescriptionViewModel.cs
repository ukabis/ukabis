using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.ApiDescription
{
    public class MethodDescriptionViewModel
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
        public IEnumerable<MethodLinkViewModel> MethodLinks { get; set; }
        public IEnumerable<SampleCodeViewModel> SampleCodes { get; set; }
        public bool IsVisibleSigninuserOnly { get; set; }
        public bool IsEnable { get; set; }
        public bool IsHidden { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
