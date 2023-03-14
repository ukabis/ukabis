using JP.DataHub.Batch.LoggingSummary.Models;
using JP.DataHub.Batch.LoggingSummary.Repository.Interfaces;
using JP.DataHub.Batch.LoggingSummary.Services.Interfaces;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Batch.LoggingSummary.Services.Impls
{
    public class LoggingService : ILoggingService
    {
        private readonly IUnityContainer unityContainer;
        private readonly ILogger<LoggingService> Logger;
        private readonly IConfiguration config;

        private Lazy<ILoggingRepository> _lazyLoggingRepository = new(() => UnityCore.Resolve<ILoggingRepository>());
        private ILoggingRepository _loggingRepository { get => _lazyLoggingRepository.Value; }


        public LoggingService(
            ILogger<LoggingService> logger)
        {
            unityContainer = LoggingSummaryUnityContainer.Resolve<IUnityContainer>();
            config = LoggingSummaryUnityContainer.Resolve<IConfiguration>();
            Logger = logger;
        }

        public void SummaryVendorSystemYmdHm(SummaryCommandModel commandModel)
        {
            _loggingRepository.SummaryVendorSystemYmdHm(commandModel);
        }

        public void SummaryProviderVendorSystemYmdHm(SummaryCommandModel commandModel)
        {
            _loggingRepository.SummaryProviderVendorSystemYmdHm(commandModel);
        }

        public void SummaryVendorSystemYmdH(SummaryCommandModel commandModel)
        {
            _loggingRepository.SummaryVendorSystemYmdH(commandModel);
        }

        public void SummaryProviderVendorSystemYmdH(SummaryCommandModel commandModel)
        {
            _loggingRepository.SummaryProviderVendorSystemYmdH(commandModel);
        }

        public void SummaryVendorSystemYmd(SummaryCommandModel commandModel)
        {
            _loggingRepository.SummaryVendorSystemYmd(commandModel);
        }

        public void SummaryProviderVendorSystemYmd(SummaryCommandModel commandModel)
        {
            _loggingRepository.SummaryProviderVendorSystemYmd(commandModel);
        }
    }
}
