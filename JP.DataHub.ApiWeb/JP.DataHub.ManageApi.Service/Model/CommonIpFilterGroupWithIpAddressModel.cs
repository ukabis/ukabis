using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class CommonIpFilterGroupWithIpAddressModel
    {
        public string CommonIpFilterGroupId { get; set; }
        public string CommonIpFilterId { get; set; }
        public string IpAddress { get; set; }
        public bool IsEnable {  get; set; }
    }
}
