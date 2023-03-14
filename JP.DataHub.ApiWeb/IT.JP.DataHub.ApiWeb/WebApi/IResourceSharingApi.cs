using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/ResourceSharing2", typeof(ResourceSharingModel))]
    public interface IResourceSharingApi : ICommonResource<ResourceSharingModel>
    {
        [WebApi("GetQuery")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<ResourceSharingModel> GetQuery();

        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<ResourceSharingModel>> GetAll();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(ResourceSharingModel model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<ResourceSharingModel> model);
    }
}
