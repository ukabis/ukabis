using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/LoggingTest", typeof(AreaUnitModel))]
    public interface ILoggingApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("Gateway")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Gateway();

        [WebApi("GetRoslyn")]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetRoslyn();

        [WebApiPost("RegisterRoslyn")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> RegisterRoslyn(AreaUnitModel model);

        [WebApiDelete("DeleteRoslyn")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteRoslyn();
    }
}
