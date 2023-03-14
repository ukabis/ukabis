using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/RoslynScriptCacheApiForVendorWithMsgPack", typeof(AreaUnitModel))]
    public interface IRoslynScriptCacheApiForVendor : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetRoslynCacheDateTime?DateTimeNow={dateTimeNow}&CacheExpireSec={cacheExpireSec}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetRoslynCacheDateTime(string dateTimeNow, int cacheExpireSec);
    }
}
