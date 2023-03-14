using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/Cache", typeof(AreaUnitModelEx))]
    public interface ICacheApi : ICommonResource<AreaUnitModelEx>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> GetAll();

        [WebApi("Get?item={item}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModelEx> GetByItem(string item);

        [WebApi("GetCacheSizeTest")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> GetCacheSizeTest();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModelEx model);
    }
}
