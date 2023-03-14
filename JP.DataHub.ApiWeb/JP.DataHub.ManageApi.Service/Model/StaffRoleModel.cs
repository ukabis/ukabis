using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class StaffRoleModel
    {
        public string StaffId { get; set; }

        public string Account { get; set; }

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public bool IsActive { get; set; }

        public DateTime RegDate { get; set; }

        public string StaffRoleId { get; set; }

        public string EmailAddress { get; set; }

        public string VendorId { get; set; }

        public string VendorName { get; set; }
    }
}
