using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.User;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi
{
    [WebApiResource("/Api/Users", typeof(OpenIdUserModel))]
    public interface IUsersApi
    {
        [WebApi("Get?userId={userId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<OpenIdUserModel> Get(string userId);

        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<OpenIdUserModel>> GetAll();

        [WebApi("GetFullAccess?userId={userId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<OpenIdUserModel> GetFullAccess(string userId);

        [WebApiPost("Post")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<OpenIdUserModel> Post(OpenIdUserRequestModel model);

        [WebApiDelete("Delete?userId={userId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string userId);
    }
}
