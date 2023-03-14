using AutoMapper;
using JP.DataHub.Batch.LoggingEventProcess.Models;
using JP.DataHub.Batch.LoggingEventProcess.Repository.Interfaces;
using JP.DataHub.Batch.LoggingEventProcess.Services.Interfaces;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Unity;
using static JP.DataHub.Batch.LoggingEventProcess.Models.LoggingInfoModel;

namespace JP.DataHub.Batch.LoggingEventProcess.Services.Impl
{
    public class LoggingService :ILoggingService
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILogger<LoggingService> Logger;
        private readonly IConfiguration config;

        private Lazy<ILoggingRepository> _lazyLoggingRepository = new(() => UnityCore.Resolve<ILoggingRepository>());
        private ILoggingRepository _loggingRepository { get => _lazyLoggingRepository.Value; }

        public LoggingService(
            ILogger<LoggingService> logger)
        {
            unityContainer = LoggingEventProcessUnityContainer.Resolve<IUnityContainer>();
            config = LoggingEventProcessUnityContainer.Resolve<IConfiguration>();
            Logger = logger;
        }

        public ProviderVendorSystemModel GetProviderVendorSystemId(string apiId)
            => _loggingRepository.GetProviderVendorSystemId(apiId);

        public void SendSummaryEvent(SendSummaryEventModel eventModel)
        {
            _loggingRepository.SendSummaryEvent(eventModel);
        }

        public void RegistLoggingInfo(LoggingInfoModel loggingInfoModel)
        {
            switch (loggingInfoModel.RegisterType)
            {
                case RegisterTypeEnum.BeginOnly:
                    _loggingRepository.RegistBeginLoggingInfo(loggingInfoModel);
                    break;
                case RegisterTypeEnum.Request:
                    _loggingRepository.RegistRequestLoggingInfo(loggingInfoModel);
                    break;
                case RegisterTypeEnum.Response:
                    _loggingRepository.RegistResponseLoggingInfo(loggingInfoModel);
                    break;
                default:
                    _loggingRepository.RegistLoggingInfo(loggingInfoModel);
                    break;
            }
        }
    }
}
