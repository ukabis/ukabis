using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/UserGroup", typeof(UserGroupModel))]
    public interface IStaticUserGroupApi : IResource
    {
        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<UserGroupModel>> GetList();

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UserGroupRegisterResponseModel> Register(UserGroupModel model);

        [WebApiDelete("Delete?user_group_id={user_group_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string user_group_id);

    }
}
