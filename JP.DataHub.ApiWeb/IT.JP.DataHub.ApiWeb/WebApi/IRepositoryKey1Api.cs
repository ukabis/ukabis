using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/RepositoryKey1", typeof(AreaUnitModel))]
    public interface IRepositoryKey1Api : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("RegisterAutoKey")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegisterAutoKey(AreaUnitModel model);
    }
}
