using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/BlobCacheApi", typeof(AreaUnitModelEx))]
    public interface IBlobCacheApi : ICommonResource<AreaUnitModelEx>
    {
        [WebApi("Get?min={min}&max={max}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> GetByMinMax(int min, int max);

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModelEx model);

        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModelEx> model);
    }
}
