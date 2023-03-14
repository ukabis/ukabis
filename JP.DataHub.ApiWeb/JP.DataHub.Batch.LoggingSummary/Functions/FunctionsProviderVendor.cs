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
    public class FunctionsProviderVendor
    {
        public static bool ExecuteProviderVendor(ReciveEventModel eventMessage, ILogger logger, ILoggingService loggingService)
        {
            try
            {
                logger.LogInformation($"JobStart VendorId={eventMessage.VendorId} SystemId={eventMessage.SystemId} ProviderVendorId={eventMessage.ProviderVendorId} ProviderSystemId={eventMessage.ProviderSystemId} ApiId={eventMessage.ApiId}");

                FunctionsYmdHm.SummaryProviderVendorSystemYmdHm(eventMessage, logger, loggingService);
                FunctionsYmdH.SummaryProviderVendorSystemYmdH(eventMessage, logger, loggingService);
                FunctionsYmd.SummaryProviderVendorSystemYmd(eventMessage, logger, loggingService);

                logger.LogInformation($"JobEnd VendorId={eventMessage.VendorId} SystemId={eventMessage.SystemId} ProviderVendorId={eventMessage.ProviderVendorId} ProviderSystemId={eventMessage.ProviderSystemId} ApiId={eventMessage.ApiId}");
                return true;
            }
            catch (AggregateException e)
            {
                logger.LogError($"Summary RegistFail message = {e.Message}");
                foreach (var innerException in e.InnerExceptions)
                {
                    logger.LogError($"Summary RegistFail message = {innerException.Message}");
                    logger.LogError($"Summary RegistFail stack = {innerException.StackTrace}");
                }

                throw;
            }
            catch (Exception e)
            {
                logger.LogError($"Summary RegistFail message = {e.Message}");
                logger.LogError($"Summary RegistFail stack = {e.StackTrace}");
                throw;
            }
        }
    }
}
