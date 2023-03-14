using JP.DataHub.AdminWeb.Service.Interface;
using JP.DataHub.AdminWeb.WebAPI.Models;
using JP.DataHub.AdminWeb.WebAPI.Resources;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Service.Core.Impl;

namespace JP.DataHub.AdminWeb.Service.Impl
{
    public class UserInvitationService : CommonCrudService, IUserInvitationService
    {
        public WebApiResponseResult SendInvitation(SendInvitationModel model)
            => BaseRepository.Register(GetDaoS<ILoginUser, IVendorResource, SendInvitationModel>(), model);
        public Task<WebApiResponseResult> SendInvitationAsync(SendInvitationModel model)
            => Task.Run(() => SendInvitation(model));

        public WebApiResponseResult AddInvitedUser(AddInvitedUserModel model)
            => BaseRepository.Register(GetDaoS<ILoginUser, IVendorResource, AddInvitedUserModel>(), model);
        public Task<WebApiResponseResult> AddInvitedUserAsync(AddInvitedUserModel model)
            => Task.Run(() => AddInvitedUser(model));
    }
}
