using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/API/Management/ScriptRuntimeLog", typeof(string))]
    public interface IScriptRuntimeLogManagementApi : ICommonResource<string>
    {
        [WebApi("Get/{logId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel GetLogMetaData(string logId);

        [WebApiDelete("Delete/{logId}")]
        [AutoGenerateReturnModel]
        WebApiRequestModel DeleteLogMetaData(string logId);
    }
}
