using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/SelectBuildNone", typeof(AreaUnitModelEx))]
    public interface ISelectBuildNoneApi : ICommonResource<AreaUnitModelEx>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModelEx>> GetAll();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModelEx model);
    }
}
