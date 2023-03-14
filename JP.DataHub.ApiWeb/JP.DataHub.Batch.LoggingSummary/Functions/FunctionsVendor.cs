using JP.DataHub.Batch.LoggingSummary.Models;
using JP.DataHub.Batch.LoggingSummary.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Batch.LoggingSummary.Functions
{
    public class FunctionsVendor
    {
        protected readonly ILogger Logger;
        protected readonly IUnityContainer unityContainer;
        protected readonly ILoggingService _loggingService;
        protected readonly IAsyncPolicy retryPolicyAsync;

        public FunctionsVendor(
            ILoggingService loggingService,
            ILoggerFactory loggerFactory,
            IConfiguration config
            )
        {
            _loggingService = loggingService;
            unityContainer = LoggingSummaryUnityContainer.Resolve<IUnityContainer>();
            Logger = loggerFactory.CreateLogger<FunctionsVendor>();
            retryPolicyAsync = Policy.Handle<Exception>()
                .WaitAndRetryAsync(config.GetValue<int>("LoggingSummarySetting:MaxNumberOfAttempts", 5), i => TimeSpan.FromSeconds(config.GetValue<double>("LoggingSummarySetting:RetryDelaySec", 60)));

        }

        [Function(nameof(FunctionsVendor))]
        public async Task ProcessSummaryMessageAsync([EventHubTrigger("%LoggingSummary_EventHubName%", Connection = "AzureEventHubConnectionStrings", ConsumerGroup = "%GroupVendor%")] string[] input)
        {
            foreach (var eventData in input)
            {
                var message = JsonConvert.DeserializeObject<ReciveEventModel>(eventData);
                string instanceId = $"TVendor_{message.GetHashCode().ToString()}";

                await retryPolicyAsync.ExecuteAsync(async () => {
                    try
                    {
                        Logger.LogInformation($"Started new instance with ID = {instanceId}.");
                        await Execute(message, Logger, _loggingService);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"{ex.Message} {ex.StackTrace}");
                        throw;
                    }
                });

                
            }
        }

        public Task Execute(ReciveEventModel eventMessage, ILogger logger, ILoggingService loggingService)
        {
            var result = new bool[2];

            result[0] = ExecuteVendor(eventMessage, logger, loggingService);
            result[1] = FunctionsProviderVendor.ExecuteProviderVendor(eventMessage, logger, loggingService);
            if (result.Any(x => x == false))
            {
                logger.LogError($"Summary fail");
                throw new Exception("Summary");
            }

            return Task.CompletedTask;
        }

        public static bool ExecuteVendor(ReciveEventModel eventMessage, ILogger logger, ILoggingService loggingService)
        {
            try
            {
                logger.LogInformation($"JobStart VendorId={eventMessage.VendorId} SystemId={eventMessage.SystemId} ProviderVendorId={eventMessage.ProviderVendorId} ProviderSystemId={eventMessage.ProviderSystemId} ApiId={eventMessage.ApiId}");

                FunctionsYmdHm.SummaryVendorSystemYmdHm(eventMessage, logger, loggingService);
                FunctionsYmdH.SummaryVendorSystemYmdH(eventMessage, logger, loggingService);
                FunctionsYmd.SummaryVendorSystemYmd(eventMessage, logger, loggingService);

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
