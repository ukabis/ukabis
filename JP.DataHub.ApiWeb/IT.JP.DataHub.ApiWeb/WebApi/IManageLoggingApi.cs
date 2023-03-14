using System.Collections.Generic;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.WebApi
{
    [WebApiResource("/Manage/Logging", typeof(ApiModel))]
    public interface IManageLoggingApi : ICommonResource<ApiModel>
    {
        [WebApi("GetLog?logId={logId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<LoggingModel> GetLog(string logId);

        [WebApiPost("GetRequestBody?logId={logId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<LoggingModel> GetRequestBody(string logId);

        [WebApiPost("GetResponseBody?logId={logId}", true)]
        [AutoGenerateReturnModel]
        WebApiRequestModel<LoggingModel> GetResponseBody(string logId);
    }
}
