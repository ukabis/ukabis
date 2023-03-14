using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/Roslyn", typeof(AreaUnitModel))]
    public interface IRoslynScriptApi : ICommonResource<AreaUnitModel>
    {
        [WebApi("ScriptTestGetNameOnly")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> ScriptTestGetNameOnly();

        [WebApi("ScriptEntryPointWithAuth")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> ScriptEntryPointWithAuth();

        [WebApi("ScriptEntryPointWithoutAuth")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<AreaUnitModel>> ScriptEntryPointWithoutAuth();
        
        [WebApiPost("RegistList")]
        [AutoGenerateReturnModel]
        WebApiRequestModel<List<RegisterResponseModel>> RegistList(List<AreaUnitModel> model);
    }
}
