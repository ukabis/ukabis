using System;
using System.Collections.Generic;
using System.Net;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class LoggingEventModel
    {
        public string LogId { get; set; }

        public HttpStatusCode HttpStatusCode { get; set; }

        public string RequestContentType { get; set; }

        public long RequestContentLength { get; set; }

        public string RequestBody { get; set; }

        public string ResponseContentType { get; set; }

        public long ResponseContentLength { get; set; }

        public string ResponseBody { get; set; }

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

        public bool IsInternalCall { get; set; }

        public string ProviderVendorId { get; set; }

        public string ProviderSystemId { get; set; }

        public string PartitionKey { get; set; }

        public long SequenceNumber { get; set; }

        public DateTimeOffset EnqueuedTime { get; set; }

        public long Offset { get; set; }

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

        public LoggingEventStatusEnum LoggingEventStatus { get; set; } = LoggingEventStatusEnum.All;

        public string InstanceId { get { return $"{LogId}-{LoggingEventStatus}"; } }
    }
}
