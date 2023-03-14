using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("IgnoreOverride:/Manage/ResourceGroup", typeof(ResourceGroupModel))]
    public interface IStaticResourceGroupApi : IResource
    {
        [WebApi("GetList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ResourceGroupModel>> GetList();

        [WebApiPost("Register")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceGroupRegisterResponseModel> Register(ResourceGroupModel model);

        [WebApiDelete("Delete?resource_group_id={resource_group_id}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Delete(string resource_group_id);

    }
}
