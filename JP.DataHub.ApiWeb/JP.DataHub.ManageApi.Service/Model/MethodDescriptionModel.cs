using MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class MethodDescriptionModel
    {
        public Guid MethodId { get; set; }
        public string HttpMethod { get; set; }
        public string RelativePath { get; set; }
        public string ActionType { get; set; }
        public string Description { get; set; }
        public bool AuthVendorSystem { get; set; }
        public bool AuthOpenId { get; set; }
        public bool AuthAdmin { get; set; }
        public bool IsVendorSystemAuthenticationAllowNull { get; set; }
        public bool PostArray { get; set; }

        public Guid? UrlSchemaId { get; set; }
        public Guid? RequestSchemaId { get; set; }
        public Guid? ResponseSchemaId { get; set; }
        public IEnumerable<MethodLinkModel> MethodLinks { get; set; }
        public IEnumerable<SampleCode> SampleCodes { get; set; }
        public string ApiRelativePath { get; set; }
        public bool IsVisibleSigninuserOnly { get; set; }
        public bool IsEnable { get; set; }
        public bool IsHidden { get; set; }
        public bool IsActive { get; set; }
        public DateTime UpdDate { get; set; }
    }
}
