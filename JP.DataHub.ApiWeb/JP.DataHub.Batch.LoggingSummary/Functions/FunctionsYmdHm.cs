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
    public class FunctionsYmdHm
    {
        public static void SummaryVendorSystemYmdHm(ReciveEventModel reciveEvent, ILogger logger, ILoggingService loggingService)
        {
            logger.LogInformation($"SummaryVendorSystemYmdHm START VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
            //VendorId,SystemIdを必要としないAPIがあるため、集計できない。
            if (string.IsNullOrEmpty(reciveEvent.SystemId) || string.IsNullOrEmpty(reciveEvent.VendorId))
            {
                return;
            }

            loggingService.SummaryVendorSystemYmdHm(new SummaryCommandModel
            {
                ApiId = reciveEvent.ApiId,
                ControllerId = reciveEvent.ControllerId,
                VendorId = reciveEvent.VendorId,
                SystemId = reciveEvent.SystemId,
                ProviderSystemId = reciveEvent.ProviderSystemId,
                ProviderVendorId = reciveEvent.ProviderVendorId,
                RequestDate = DateTime.Parse(reciveEvent.RequestDate.ToString("yyyy/MM/dd HH:mm"))
            });
            logger.LogInformation($"SummaryVendorSystemYmdHm END VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
        }

        public static void SummaryProviderVendorSystemYmdHm(ReciveEventModel reciveEvent, ILogger logger, ILoggingService loggingService)
        {
            logger.LogInformation($"SummaryProviderVendorSystemYmdHm START VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
            //APIの情報をリアルタイムで取り込んでいないためまたは、透過APIはAPIIDがいい加減の為提供元のVendorの特定ができないため
            if (string.IsNullOrEmpty(reciveEvent.ProviderSystemId) || string.IsNullOrEmpty(reciveEvent.ProviderVendorId))
            {
                return;
            }

            loggingService.SummaryProviderVendorSystemYmdHm(new SummaryCommandModel
            {
                ApiId = reciveEvent.ApiId,
                ControllerId = reciveEvent.ControllerId,
                VendorId = reciveEvent.VendorId,
                SystemId = reciveEvent.SystemId,
                ProviderSystemId = reciveEvent.ProviderSystemId,
                ProviderVendorId = reciveEvent.ProviderVendorId,
                RequestDate = DateTime.Parse(reciveEvent.RequestDate.ToString("yyyy/MM/dd HH:mm"))
            });
            logger.LogInformation($"SummaryProviderVendorSystemYmdHm END VendorId={reciveEvent.VendorId} SystemId={reciveEvent.SystemId} ProviderVendorId={reciveEvent.ProviderVendorId} ProviderSystemId={reciveEvent.ProviderSystemId} ApiId={reciveEvent.ApiId}");
        }
    }
}
