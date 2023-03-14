using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/ScriptRuntimeLog", typeof(string))]
    public interface IScriptRuntimeLogApi : ICommonResource<string>
    {
        [WebApi("Get?logId={fileId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetLogFile(string fileId);

        [WebApiDelete("Delete?logId={fileId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteLogFile(string fileId);
    }
}
