using JP.DataHub.Com.TimeZone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.LoggingEventProcess.Models
{
    public class LoggingInfoModel
    {
        public string LogId { get; set; }

        public string HttpStatusCode { get; set; }

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

        public string RequestHeaders { get; set; }

        public string ResponseHeaders { get; set; }

        public string ControllerId { get; set; }

        public string ApiId { get; set; }

        public string ClientIpAddress { get; set; }

        public TimeSpan ExecuteTime { get; set; }

        public DateTime RequestDate { get; set; }

        public bool IsInternalCall { get; set; }

        public string ProviderVendorId { get; set; }

        public string ProviderSystemId { get; set; }

        public DateTime RequestDateYmd
        {
            get { return RequestDate.ConvertToJst().TruncateHour(); }

        }
        public DateTime RequestDateYmdH
        {
            get { return RequestDate.ConvertToJst().TruncateMinute(); }
        }
        public DateTime RequestDateYmdHM
        {
            get { return RequestDate.ConvertToJst().TruncateSecond(); }
        }

        public RegisterTypeEnum RegisterType { get; set; }

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
            /// 全部入り
            /// </summary>
            All = 9,
        }
    }
}
