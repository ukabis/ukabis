using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/VersionDisable", typeof(AreaUnitModel))]
    public interface IDisableVersionApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetByODataQuery?AreaUnitCode={areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetByODataQuery(string areaUnitCode);

        [WebApi("GetByQuery/{areaUnitCode}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<AreaUnitModel> GetByQuery(string areaUnitCode);

        [WebApiPost("AutoRegister")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> AutoRegister(AreaUnitModel model);
    }
}
