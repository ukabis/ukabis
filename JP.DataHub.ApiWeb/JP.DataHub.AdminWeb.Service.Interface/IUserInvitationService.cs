using JP.DataHub.Com.Net.Http;
using JP.DataHub.AdminWeb.WebAPI.Models;

namespace JP.DataHub.AdminWeb.Service.Interface
{
    public interface IUserInvitationService
    {
        WebApiResponseResult AddInvitedUser(AddInvitedUserModel model);
        Task<WebApiResponseResult> AddInvitedUserAsync(AddInvitedUserModel model);

        WebApiResponseResult SendInvitation(SendInvitationModel model);
        Task<WebApiResponseResult> SendInvitationAsync(SendInvitationModel model);
    }
}
