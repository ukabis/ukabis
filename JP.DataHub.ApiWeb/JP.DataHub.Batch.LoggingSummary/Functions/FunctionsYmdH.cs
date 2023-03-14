using JP.DataHub.Batch.LoggingSummary.Models;
using JP.DataHub.Batch.LoggingSummary.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Batch.LoggingSummary.Functions
{
    public class FunctionsYmdH
    {
        private readonly ILogger Logger;
        private readonly IUnityContainer unityContainer;
        private readonly ILoggingService _loggingService;

        public FunctionsYmdH(
            ILoggingService loggingService,
            ILoggerFactory loggerFactory,
            IConfiguration config
            )
        {
            _loggingService = loggingService;
            unityContainer = LoggingSummaryUnityContainer.Resolve<IUnityContainer>();
            Logger = loggerFactory.CreateLogger<FunctionsVendor>();
        }

        public static void SummaryVendorSystemYmdH(ReciveEventModel reciveEvent, ILogger logger, ILoggingService loggingService)
        {
            logger.LogInformation($"SummaryVendorSystemYmdH START VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
            //VendorId,SystemIdを必要としないAPIがあるため、集計できない。
            if (string.IsNullOrEmpty(reciveEvent.SystemId) || string.IsNullOrEmpty(reciveEvent.VendorId))
            {
                return;
            }

            loggingService.SummaryVendorSystemYmdH(new SummaryCommandModel
            {
                ApiId = reciveEvent.ApiId,
                ControllerId = reciveEvent.ControllerId,
                VendorId = reciveEvent.VendorId,
                SystemId = reciveEvent.SystemId,
                ProviderSystemId = reciveEvent.ProviderSystemId,
                ProviderVendorId = reciveEvent.ProviderVendorId,
                RequestDate = DateTime.Parse(reciveEvent.RequestDate.ToString("yyyy/MM/dd HH:00:00"))
            });
            logger.LogInformation($"SummaryVendorSystemYmdH END VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
        }
        public static void SummaryProviderVendorSystemYmdH(ReciveEventModel reciveEvent, ILogger logger, ILoggingService loggingService)
        {
            logger.LogInformation($"SummaryProviderVendorSystemYmdH START VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
            //APIの情報をリアルタイムで取り込んでいないためまたは、透過APIはAPIIDがいい加減の為提供元のVendorの特定ができないため
            if (string.IsNullOrEmpty(reciveEvent.ProviderSystemId) || string.IsNullOrEmpty(reciveEvent.ProviderVendorId))
            {
                return;
            }

            loggingService.SummaryProviderVendorSystemYmdH(new SummaryCommandModel
            {
                ApiId = reciveEvent.ApiId,
                ControllerId = reciveEvent.ControllerId,
                VendorId = reciveEvent.VendorId,
                SystemId = reciveEvent.SystemId,
                ProviderSystemId = reciveEvent.ProviderSystemId,
                ProviderVendorId = reciveEvent.ProviderVendorId,
                RequestDate = DateTime.Parse(reciveEvent.RequestDate.ToString("yyyy/MM/dd HH:00:00"))
            });
            logger.LogInformation($"SummaryProviderVendorSystemYmdH END VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId{reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
        }
    }
}
