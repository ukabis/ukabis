using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static JP.DataHub.ApiWeb.Domain.Context.DynamicApi.DynamicApiLoggingInfo;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Logging
{
    public class ApiRequestResponseLogModel
    {
        public string LogId { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string RequestContentType { get; set; }

        public long RequestContentLength { get; set; }

        public string RequestBody { get; set; }
        public Stream RequestBodyStream { get; set; }

        public string ResponseContentType { get; set; }

        public long ResponseContentLength { get; set; }

        public string ResponseBody { get; set; }
        public Stream ResponseBodyStream { get; set; }

        public string VendorId { get; set; }

        public string SystemId { get; set; }

        public string OpenId { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string HttpMethodType { get; set; }

        public string Url { get; set; }

        public string QueryString { get; set; }

        public Dictionary<string, List<string>> RequestHeaders { get; set; }

        public Dictionary<string, List<string>> ResponseHeaders { get; set; }

        public string ControllerId { get; set; }

        public string ApiId { get; set; }

        public string ClientIpAddress { get; set; }

        public TimeSpan ExecuteTime { get; set; }

        public DateTime RequestDate { get; set; }

        public enum LoggingEventStatusEnum
        {
            /// <summary>
            /// 開始時のイベント
            /// </summary>
            Begin = 1,
            /// <summary>
            /// リクエスト時のイベント
            /// </summary>
            Request = 2,
            /// <summary>
            /// レスポンス時のイベント
            /// </summary>
            Response = 3,
            /// <summary>
            /// 全部入り
            /// </summary>
            All = 9,
        }

        public LoggingEventStatusEnum LoggingEventStatus { get; } = LoggingEventStatusEnum.All;

        public ApiRequestResponseLogModel(string logId, HttpStatusCode httpStatusCode, string requestContentType,
            long requestContentLength, string requestBody, string responseContentType, long responseContentLength,
            string responseBody, string vendorId, string systemId, string openId, string controllerName,
            string actionName, string httpMethodType, string url, string queryString,
            Dictionary<string, List<string>> requestHeaders, Dictionary<string, List<string>> responseHeaders,
            string controllerId, string apiId, string clientIpAddress
            , TimeSpan executeTime, DateTime requestDate, Stream requestBodyStream, Stream responseBodyStream,
            LoggingEventStatusEnum loggingEventStatus = LoggingEventStatusEnum.All
            )
        {
            this.LogId = logId;
            this.HttpStatusCode = httpStatusCode;
            this.RequestContentType = requestContentType;
            this.RequestContentLength = requestContentLength;
            this.RequestBody = requestBody;
            this.ResponseContentType = responseContentType;
            this.ResponseContentLength = responseContentLength;
            this.ResponseBody = responseBody;
            this.VendorId = vendorId;
            this.SystemId = systemId;
            this.OpenId = openId;
            this.ControllerName = controllerName;
            this.ActionName = actionName;
            this.HttpMethodType = httpMethodType;
            this.Url = url;
            this.QueryString = queryString;
            this.RequestHeaders = requestHeaders;
            this.ResponseHeaders = responseHeaders;
            this.ControllerId = controllerId;
            this.ApiId = apiId;
            this.ClientIpAddress = clientIpAddress;
            this.ExecuteTime = executeTime;
            this.RequestDate = requestDate;
            this.RequestBodyStream = requestBodyStream;
            this.ResponseBodyStream = responseBodyStream;
            this.LoggingEventStatus = loggingEventStatus;

        }
    }
}
