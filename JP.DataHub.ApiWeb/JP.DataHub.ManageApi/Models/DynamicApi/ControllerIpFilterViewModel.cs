using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class ControllerIpFilterViewModel
    {
        public string ControllerIpFilterId { get; set; }

        public string ControllerId { get; set; }

        public string IpAddress { get; set; }

        public bool IsEnable { get; set; }

        public bool IsActive { get; set; }
    }
}
