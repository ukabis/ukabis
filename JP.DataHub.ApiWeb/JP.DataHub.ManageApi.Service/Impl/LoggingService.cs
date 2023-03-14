using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class LoggingService : AbstractService, ILoggingService
    {
        private Lazy<ILoggingRepository> _lazyLoggingRepository = new Lazy<ILoggingRepository>(() => UnityCore.Resolve<ILoggingRepository>());
        private ILoggingRepository _loggingRepository { get => _lazyLoggingRepository.Value; }

        /// <summary>
        /// LogIdに紐づくLogging情報を一件取得する
        /// </summary>
        /// <param name="logId">LogId</param>
        /// <returns>Logging情報</returns>
        public LoggingQueryModel GetLogging(string logId)
        {
            return _loggingRepository.GetLogging(logId);
        }

        /// <summary>
        /// LogIdに紐づくLogging情報をからRequestBodyを取得する
        /// </summary>
        /// <param name="logId">logId</param>
        /// <returns>RequestBody</returns>
        public HttpResponseMessage GetRequestBody(string logId)
        {
            var result = _loggingRepository.GetRequestBody(logId);
            return ConvertHttpResponseMessage(result.Stream,result.MimeType);
        }

        /// <summary>
        /// LogIdに紐づくLogging情報をからResponseBoodyを取得する
        /// </summary>
        /// <param name="logId">logId</param>
        /// <returns>ResponseBoody</returns>
        public HttpResponseMessage GetResponseBody(string logId)
        {
            var result = _loggingRepository.GetResponseBody(logId);
            return ConvertHttpResponseMessage(result.Stream, result.MimeType);
        }

        private HttpResponseMessage ConvertHttpResponseMessage(Stream  stream, string mimeType)
        {
            HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StreamContent(stream)
            };
            result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
            return result;
        }
    }
}
