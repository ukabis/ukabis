using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/IntegratedTest/RoslynScriptRuntimeLogApi", typeof(string))]
    public interface IRoslynScriptRuntimeLogApi : ICommonResource<string>
    {
        [WebApi("WriteLog")]
        [AutoGenerateReturnModel]
        WebApiRequestModel WriteLog();

        [WebApi("ExceptionLog")]
        [AutoGenerateReturnModel]
        WebApiRequestModel ExceptionLog();
    }
}
