using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class SendUserInvitationModel
    {
        public string VendorId { get; set; }
        public string RoleId { get; set; }
        public string MailAddress { get; set; }
        public string RegistAccount { get; set; }
    }
}
