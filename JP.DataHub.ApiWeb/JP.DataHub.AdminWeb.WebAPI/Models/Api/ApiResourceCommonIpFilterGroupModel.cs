using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models.Api
{
    public class ApiResourceCommonIpFilterGroupModel
    {
        public string ControllerId { get; set; }

        public string CommonIpFilterGroupId { get; set; }

        public string CommonIpFilterGroupName { get; set; }

        public bool IsActive { get; set; }
    }
}
