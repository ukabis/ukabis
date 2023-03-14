using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiResourceSharing : IEntity
    {
        public ControllerId ControllerId { get; set; }

        public VendorId SharingFromVendorId { get; set; }

        public SystemId SharingFromSystemId { get; set; }

        public IReadOnlyList<ApiResourceSharingRule> ResourceSharingRuleList { get; set; }

        public ApiResourceSharing()
        {
        }

        public ApiResourceSharing(ControllerId controllerId, VendorId sharingFromVendorId, SystemId sharingFromSystemId, List<ApiResourceSharingRule> resourceSharingRuleList)
        {
            ControllerId = controllerId;
            SharingFromVendorId = sharingFromVendorId;
            SharingFromSystemId = sharingFromSystemId;
            ResourceSharingRuleList = resourceSharingRuleList;
        }
    }
}
