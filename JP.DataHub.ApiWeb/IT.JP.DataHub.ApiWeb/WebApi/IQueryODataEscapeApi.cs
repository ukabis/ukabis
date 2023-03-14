using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/QueryODataEscapeTest", typeof(AreaUnitModel))]
    public interface IQueryODataEscapeApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApi("GetByODataQuery")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByODataQuery();
    }
}
