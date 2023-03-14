using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class ApiResourceSharingModel
    {
        public string ControllerId { get; set; }

        public string SharingFromVendorId { get; set; }

        public string SharingFromSystemId { get; set; }

        public List<ApiResourceSharingRuleModel> ResourceSharingRuleList { get; set; }
    }
}
