using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class ApiResourceSharingRuleModel
    {
        public string ResourceSharingRuleId { get; }

        public string SharingToVendorId { get; }

        public string SharingToSystemId { get; }

        public string ResourceSharingRuleName { get; }

        public string Query { get; }

        public string RoslynScript { get; }

        public bool IsEnable { get; }

        public ApiResourceSharingRuleModel(string sharingToVendorId, string sharingToSystemId, string resourceSharingRuleName, string query, string roslynScript, bool isEnable)
        {
            SharingToVendorId = sharingToVendorId;
            SharingToSystemId = sharingToSystemId;
            ResourceSharingRuleName = resourceSharingRuleName;
            Query = query;
            RoslynScript = roslynScript;
            IsEnable = isEnable;
        }
    }
}
