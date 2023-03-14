using System;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class ResourceSharingModel
    {
        public string ResourceSharingRuleId { get; set; }

        public string SharingFromVendorId { get; set; }
        public string SharingFromSystemId { get; set; }
        public string ApiId { get; set; }
        public string SharingToVendorId { get; set; }
        public string SharingToSystemId { get; set; }
        public string ResourceSharingRuleName { get; set; }
        public string Query { get; set; }
        public string RoslynScript { get; set; }
        public bool IsEnable { get; set; } = true;
    }
}
