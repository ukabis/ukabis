using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JP.DataHub.Infrastructure.Core.Logging
{
    public class ApiRequestResponseLogModel
    {
        public string LogId { get; set; }

        public int HttpStatusCode { get; set; }

        public string RequestContentType { get; set; }

        public long? RequestContentLength { get; set; }

        public string RequestBody { get; set; }         // 実際にはパスが入る

        public string RequestHeaders { get; set; }      // 実際にはパスが入る


        public string ResponseContentType { get; set; }

        public long? ResponseContentLength { get; set; }

        public string ResponseBody { get; set; }    // 実際にはパスが入る

        public string ResponseHeaders { get; set; } // 実際にはパスが入る

        public string VendorId { get; set; }

        public string SystemId { get; set; }

        public string OpenId { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string HttpMethodType { get; set; }

        public string Url { get; set; }

        public string QueryString { get; set; }

        public string ResourceId { get; set; }

        public string ApiId { get; set; }
        public string ControllerId { get; set; }
        public string ActionId { get; set; }

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

        public string LoggingEventStatus { get => Status.ToString();  set { } }

        [JsonIgnore]
        public LoggingEventStatusEnum Status { get; set; } = LoggingEventStatusEnum.All;
    }
}
