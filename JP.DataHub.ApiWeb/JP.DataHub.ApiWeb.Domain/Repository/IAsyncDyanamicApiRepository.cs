using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IAsyncDyanamicApiRepository
    {
        void Request(AsyncRequestId requestId, JsonDocument requestBody, JsonDocument asyncLog);
        void RequestOnCache(AsyncRequestId requestId, JsonDocument requestBody, JsonDocument asyncLog, ControllerId controllerId);

        AsyncApiResult GetStatus(AsyncRequestId asyncRequestId);
        HttpResponseMessage GetResult(AsyncRequestId asyncRequestId);
        bool SetResult(Stream content, string blobPath, string accept);
        bool SetResultOverwrite(Stream content, string blobPath, string accept);
        bool SetResultOverwrite(string content, string blobPath, string accept);
        AsyncRequestId GetRequestIdFromCache(OpenId openId, ControllerId controllerId);
    }
}
