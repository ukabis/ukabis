using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.ManageApi.Service.Model;

namespace JP.DataHub.ManageApi.Service.Repository
{
    internal interface IUserInvitationRepository
    {
        string RegisterUserInvitation(SendUserInvitationModel model);
        UserInvitationModel Get(string user_invitation_id);
        void Invited(string user_invitation_id);
    }
}
