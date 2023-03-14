using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/UserResourceDefine", typeof(UserResourceShareModel))]
    public interface IStaticUserResourceDefineApi : IResource
    {
        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<UserResourceShareModel>> GetList();

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<UserResourceShareRegistreResponseModel> Register(UserResourceShareModel model);

        [WebApiDelete("Delete?user_resource_group_id={user_resource_group_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string user_resource_group_id);

    }
}
