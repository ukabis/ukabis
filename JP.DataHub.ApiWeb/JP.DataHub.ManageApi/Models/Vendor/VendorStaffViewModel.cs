using System;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class VendorStaffViewModel
    {
        public string StaffId { get; set; }

        public string RoleId { get; set; }

        public string Account { get; set; }

        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public Guid? StaffRoleId { get; set; }
    }
}
