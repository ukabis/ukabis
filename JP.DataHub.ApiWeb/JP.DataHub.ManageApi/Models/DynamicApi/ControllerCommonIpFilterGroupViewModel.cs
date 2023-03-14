using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ControllerCommonIpFilterGroupViewModel
    {
        public string ControllerId { get; set; }

        public string CommonIpFilterGroupId { get; set; }

        public string CommonIpFilterGroupName { get; set; }

        public bool IsActive { get; set; }
    }
}
