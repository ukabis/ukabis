using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/UpdateApi", typeof(AreaUnitModel))]
    public interface IUpdateApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("GetAll")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> GetAll();

        [WebApiPost("Regist")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<RegisterResponseModel> Regist(AreaUnitModel model);

        [WebApiPatch("Update/{key}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel Update(string key, List<AreaUnitModel> model);
    }
}
