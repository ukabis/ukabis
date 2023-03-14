using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class ControllerCommonIpFilterGroupModel
    {
        public string ControllerId { get; set; }

        public string CommonIpFilterGroupId { get; set; }

        public string CommonIpFilterGroupName { get; set; }

        public bool IsActive { get; set; }
    }
}
