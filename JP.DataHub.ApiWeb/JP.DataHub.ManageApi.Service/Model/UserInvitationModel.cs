using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class UserInvitationModel
    {
        public string UserInvitationId { get; set; }

        public string MailAddress { get; set; }

        public string VendorId { get; set; }

        public string InvitationDate { get; set; }

        public string ExpireDate { get; set; }

        public string RegisterStaffId { get; set; }

        public string RegisterAccount { get; set; }

        public bool IsUsed { get; set; }

        public string RoleId { get; set; }

        public string StaffId { get; set; }
    }
}
