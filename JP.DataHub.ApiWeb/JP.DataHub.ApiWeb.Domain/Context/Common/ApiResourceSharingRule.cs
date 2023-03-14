using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiResourceSharingRule : IValueObject
    {
        public ResourceSharingRuleId ResourceSharingRuleId { get; }

        public Guid SharingToVendorId { get; }

        public Guid SharingToSystemId { get; }

        public string ResourceSharingRuleName { get; }

        public string Query { get; }

        public string RoslynScript { get; }

        public bool IsEnable { get; }

        public ApiResourceSharingRule(Guid sharingToVendorId, Guid sharingToSystemId, string resourceSharingRuleName, string query, string roslynScript, bool isEnable)
        {
            SharingToVendorId = sharingToVendorId;
            SharingToSystemId = sharingToSystemId;
            ResourceSharingRuleName = resourceSharingRuleName;
            Query = query;
            RoslynScript = roslynScript;
            IsEnable = isEnable;
        }

        public static bool operator ==(ApiResourceSharingRule me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiResourceSharingRule me, object other) => !me?.Equals(other) == true;
    }
}
