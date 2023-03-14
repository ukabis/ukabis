using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AsyncDynamicApi", typeof(AreaUnitModel))]
    public interface IAsyncDynamicApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApi("GetByAggregate")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByAggregate();
        
        [WebApi("GetAll500")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll500();

        [WebApi("Gateway/Get2GB")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GatewayGet2GB();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModel> model);
    }
}
