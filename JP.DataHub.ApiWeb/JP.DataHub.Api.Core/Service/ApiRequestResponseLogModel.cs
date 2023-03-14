using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Service
{
    public class ApiRequestResponseLogModel
    {
        public string LogId { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string RequestContentType { get; set; }
        public long RequestContentLength { get; set; }
        public string RequestBody { get; set; }
        public Stream RequestBodyStream { get; set; }
        public Dictionary<string, List<string>> RequestHeaders { get; set; }
        public string ResponseContentType { get; set; }
        public long ResponseContentLength { get; set; }
        public string ResponseBody { get; set; }
        public Stream ResponseBodyStream { get; set; }
        public Dictionary<string, List<string>> ResponseHeaders { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string OpenId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string HttpMethodType { get; set; }
        public string Url { get; set; }
        public string QueryString { get; set; }
        public string ControllerId { get; set; }
        public string ApiId { get; set; }
        public string ClientIpAddress { get; set; }
        public TimeSpan ExecuteTime { get; set; }
        public DateTime RequestDate { get; set; }
        public bool IsInternalCall { get; set; }
    }
}