using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Logging
{
    public interface ILoggingInterceptor
    {
        bool IsNoLogging { get; }
        Task BeforeTask { get; }
        Task AfterTask { get; }

        Stream Before(string loggingId, DateTime request_time, string http_method_type, string relative_url, Stream request_body, string query_string, Dictionary<string, List<string>> header, string media_type, string content_type, long? content_length);
        (int StatusCode, Dictionary<string, List<string>> Header, Stream Stream) After(string loggingId, TimeSpan execute_time, int statusCode, Dictionary<string, List<string>> header, Stream stream);
    }
}
