using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JP.DataHub.Batch.LoggingSummary.Models;
using JP.DataHub.Batch.LoggingSummary.Services.Interfaces;
using Oci.StreamingService.Models;

namespace JP.DataHub.Batch.LoggingSummary.Functions
{
    public class FunctionsVendorForOci: FunctionsVendor
    {
        public FunctionsVendorForOci(
            ILoggingService loggingService,
            ILoggerFactory loggerFactory,
            IConfiguration config
        ) : base(loggingService, loggerFactory, config)
        {
        }

        public async Task<int> ProcessSummaryMessageAsync(string message)
        {
            var writeCount = 0;
            Logger.LogInformation($"Start Async {message}");

            var model = JsonConvert.DeserializeObject<ReciveEventModel>(message);
            string instanceId = $"TVendor_{model?.GetHashCode().ToString()}";

            await retryPolicyAsync.ExecuteAsync(async () => {
                try
                {
                    Logger.LogInformation($"Started new instance with ID = {instanceId}.");
                    await Execute(model, Logger, _loggingService);
                    writeCount++;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{ex.Message} {ex.StackTrace}");
                }
            });
            return writeCount;
        }
    }
}
