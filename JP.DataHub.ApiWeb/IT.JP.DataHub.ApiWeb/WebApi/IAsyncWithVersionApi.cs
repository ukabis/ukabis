using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AsyncWithVersion", typeof(AreaUnitModel))]
    public interface IAsyncWithVersionApi : ICommonResource<AreaUnitModel>
    {
        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AsyncRequestResponseModel> RegistAsync(AreaUnitModel model);
    }
}
