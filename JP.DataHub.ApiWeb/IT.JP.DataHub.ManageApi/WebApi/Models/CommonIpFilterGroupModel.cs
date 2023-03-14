using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class CommonIpFilterGroupModel
    {
        public string CommonIpFilterGroupId { get; set; }

        public string CommonIpFilterGroupName { get; set; }

        public List<CommonIpFilterModel> IpList { get; set; }
    }

    [Serializable]
    public class CommonIpFilterModel
    {
        public string CommonIpFilterId { get; set; }

        public string IpAddress { get; set; }

        public bool IsEnable { get; set; }

        public bool IsActive { get; set; }
    }
}
