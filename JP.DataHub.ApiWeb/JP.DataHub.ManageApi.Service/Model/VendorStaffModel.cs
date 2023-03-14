using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class VendorStaffModel
    {
        public string StaffId { get; set; }

        public string RoleId { get; set; }

        public string Account { get; set; }

        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public Guid? StaffRoleId { get; set; }
    }
}
