using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.TimeZone;

namespace JP.DataHub.ApiWeb.Core.Model
{
    public class LoggingInfo
    {
        public string LogId { get; }

        public string HttpStatusCode { get; }

        public string RequestContentType { get; }

        public long RequestContentLength { get; }

        public string RequestBody { get; }

        public string ResponseContentType { get; }

        public long ResponseContentLength { get; }

        public string ResponseBody { get; }

        public string VendorId { get; }

        public string SystemId { get; }

        public string OpenId { get; }

        public string ControllerName { get; }

        public string ActionName { get; }

        public string HttpMethodType { get; }

        public string Url { get; }

        public string QueryString { get; }

        public string RequestHeaders { get; }

        public string ResponseHeaders { get; }

        public string ControllerId { get; }

        public string ApiId { get; }

        public string ClientIpAddress { get; }

        public TimeSpan ExecuteTime { get; }

        public DateTime RequestDate { get; }

        public bool IsInternalCall { get; set; }

        public string ProviderVendorId { get; }

        public string ProviderSystemId { get; }

        public DateTime RequestDateYmd
        {
            get { return RequestDate.ConvertToJst().Date; }

        }
        public DateTime RequestDateYmdH
        {
            get { return RequestDate.ConvertToJst().TruncateHour(); }
        }
        public DateTime RequestDateYmdHM
        {
            get { return RequestDate.ConvertToJst().TruncateMinute(); }
        }

        public enum RegisterTypeEnum
        {
            /// <summary>
            /// 開始時のイベントのみ
            /// </summary>
            BeginOnly = 1,
            /// <summary>
            /// リクエスト時のイベント
            /// </summary>
            Request = 2,
            /// <summary>
            /// レスポンス時のイベント
            /// </summary>
            Response = 3,
            /// <summary>
            /// 全部
            /// </summary>
            All = 9,
        }

        public RegisterTypeEnum RegisterType { get; set; }

        public LoggingInfo(
            string logId, string httpStatusCode, string requestContentType, long requestContentLength, string requestBody, 
            string responseContentType, long responseContentLength, string responseBody, string vendorId, string systemId, string openId, 
            string controllerName, string actionName, string httpMethodType, string url, string queryString, string requestHeaders, string responseHeaders, 
            string controllerId, string apiId, string clientIpAddress, TimeSpan executeTime, DateTime requestDate, bool isInternalCall,
            string providerVendorId, string providerSystemId, RegisterTypeEnum registerType)
        {
            LogId = logId;
            HttpStatusCode = httpStatusCode;
            RequestContentType = requestContentType;
            RequestContentLength = requestContentLength;
            RequestBody = requestBody;
            ResponseContentType = responseContentType;
            ResponseContentLength = responseContentLength;
            ResponseBody = responseBody;
            VendorId = vendorId;
            SystemId = systemId;
            OpenId = openId;
            ControllerName = controllerName;
            ActionName = actionName;
            HttpMethodType = httpMethodType;
            Url = url;
            QueryString = queryString;
            RequestHeaders = requestHeaders;
            ResponseHeaders = responseHeaders;
            ControllerId = controllerId;
            ApiId = apiId;
            ClientIpAddress = clientIpAddress;
            ExecuteTime = executeTime;
            RequestDate = requestDate;
            IsInternalCall = isInternalCall;
            ProviderVendorId = providerVendorId;
            ProviderSystemId = providerSystemId;
            RegisterType = registerType;
        }
    }
}
