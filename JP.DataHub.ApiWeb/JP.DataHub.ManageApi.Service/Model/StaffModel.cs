using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class StaffModel
    {
        public string StaffId { get; set; }
        public string Account { get; set; }
        public string VendorId { get; set; }
        public string EmailAddress { get; set; }
        public string StaffRoleId { get; set; }
        public string RoleId { get; set; }
    }
}
