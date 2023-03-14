using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi
{
    public class RegisterApiCommonIpFilterGroupModel
    {
        public string CommonIpFilterGroupId { get; set; }
        public bool IsActive { get; set; }
    }
    public class ApiCommonIpFilterGroupModel
    {
        public string CommonIpFilterGroupId { get; set; }

        public string CommonIpFilterGroupName { get; set; }
    }
}
