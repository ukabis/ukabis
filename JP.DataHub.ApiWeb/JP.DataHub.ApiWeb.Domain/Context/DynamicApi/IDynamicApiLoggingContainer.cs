using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    public interface IDynamicApiLoggingContainer
    {
        void LoggingBegin(DynamicApiLoggingInfo dynamicApiLoggingInfo, bool isIncludeRequest = false);
        void LoggingRequest(string loggingId, long size, Stream Contents);
        void LoggingResponse(string loggingId, long size, string contentsType, Stream contents, Dictionary<string, List<string>> headers, System.Net.HttpStatusCode httpStatusCode);
    }
}