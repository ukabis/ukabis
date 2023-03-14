using AutoMapper;
using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Models.Logging;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.ManageApi.Service.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Net.Http;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("59D98A34-DB72-460B-BA3E-30D04E653053")]
    public class LoggingController : AbstractController
    {
        private Lazy<ILoggingService> _lazyLoggingService = new Lazy<ILoggingService>(() => UnityCore.Resolve<ILoggingService>());
        private ILoggingService _loggingService { get => _lazyLoggingService.Value; }
        private static Lazy<IMapper> s_lazyMapper = new Lazy<IMapper>(() =>
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LoggingQueryViewModel, LoggingQueryModel>().ReverseMap();
            }).CreateMapper();
        });
        private static IMapper s_mapper { get { return s_lazyMapper.Value; } }
        /// <summary>
        /// Logging情報を1件取得します。
        /// </summary>
        /// <param name="logId">logId</param>
        /// <returns>Logging情報</returns>
        [HttpGet]
        [ManageAction("1C026B9E-507E-4E9C-A8EB-AF57168F1C41")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public ActionResult<LoggingQueryViewModel> GetLog(string logId)
        {
            return s_mapper.Map<LoggingQueryViewModel>(_loggingService.GetLogging(logId));
        }

        /// <summary>
        /// Logging情報からRequestBodyを取得します。
        /// </summary>
        /// <param name="logId">logId</param>
        /// <returns>RequestBody</returns>
        [HttpGet]
        [ManageAction("0269A34F-1805-4575-B069-5F842D904607")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public HttpResponseMessage GetRequestBody(string logId)
        {
            return _loggingService.GetRequestBody(logId);
        }

        /// <summary>
        /// Logging情報からResponseBodyを取得します。
        /// </summary>
        /// <param name="logId">logId</param>
        /// <returns>ResponseBody</returns>
        [HttpGet]
        [ManageAction("9DA82A77-1B36-4295-A901-B40F9BF16FA0")]
        [ExceptionFilter(typeof(NotFoundException), HttpStatusCode.NotFound)]
        public HttpResponseMessage GetResponseBody(string logId)
        {
            return _loggingService.GetResponseBody(logId);
        }
    }
}