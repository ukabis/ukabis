using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/AopCache", typeof(AreaUnitModel))]
    public interface IAopCacheApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("Get?key={key}&value1={value1}&value2={value2}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<FieldPolygonIdModel> GetEx(string key, string value1, string value2);
    }
}
