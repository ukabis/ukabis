using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/OptimisticConcurrency", typeof(AreaUnitModel))]
    public interface IOptimisticConcurrencyApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModel> model);
    }
}
