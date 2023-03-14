using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/QueryStringApi", typeof(AreaUnitModel))]
    public interface IQueryStringApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetByParameter{query}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByParameter(string query);

        [WebApi("GetByOptionParameter{query}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByOptionParameter(string query);

        [WebApi("GetByParameters{query}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByParameters(string query);
    }
}
