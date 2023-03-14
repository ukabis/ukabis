using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    public class DynamicApiLoggingInfo
    {
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DynamicApiLoggingInfo, DynamicApiLoggingInfo>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });

        private static IMapper s_mapper => s_lazyMapper.Value;

        public string LogId { get; set; }
        public string ControllerId { get; set; }
        public string ApiId { get; set; }
        public string Url { get; set; }
        public string QueryString { get; set; }
        public string RequestContentType { get; set; }
        public Stream RequestBodyStream { get; set; }
        public long RequestContentLength { get; set; }
        public Stream ResponseBodyStream { get; set; }
        public DateTime RequestDate { get; set; }
        public Dictionary<string, List<string>> RequestHeaders { get; set; }
        public string HttpMethodType { get; set; }
        public string ClientIpAddress { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public string OpenId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }

        /// <summary>
        /// ResponseBodyを記録するか
        /// </summary>
        public bool IsSaveResponseBody { get; set; } = true;

        public enum LoggingEventStatusEnum
        {
            /// <summary>
            /// リクエスト時のイベント
            /// </summary>
            Request = 2,
            /// <summary>
            /// レスポンス時のイベント
            /// </summary>
            Response = 3,
        }

        public LoggingEventStatusEnum LoggingEventStatus { get; set; } = LoggingEventStatusEnum.Request;

        public DynamicApiLoggingInfo Clone(LoggingEventStatusEnum loggingEventStatusEnum)
        {
            var copy = new DynamicApiLoggingInfo();
            copy = s_mapper.Map<DynamicApiLoggingInfo>(this);
            copy.LoggingEventStatus = loggingEventStatusEnum;
            return copy;
        }

    }
}