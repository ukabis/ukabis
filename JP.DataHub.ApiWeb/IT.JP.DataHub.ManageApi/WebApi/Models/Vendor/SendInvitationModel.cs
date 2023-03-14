using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Vendor
{
    public class AddInvitedUserModel
    {
        public string InvitationId { get; set; }
        public string OpenId { get; set; }
        public string MailAddress { get; set; }
    }
    public class SendInvitationModel
    {
        public string VendorId { get; set; }
        public string RoleId { get; set; }
        public string MailAddress { get; set; }
    }
}
