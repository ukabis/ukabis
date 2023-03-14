using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Unity;
using Polly;
using Polly.Retry;
using Newtonsoft.Json;
using JP.DataHub.Batch.AsyncDynamicApi.Services.Interfaces;
using JP.DataHub.Batch.AsyncDynamicApi.Models;
using JP.DataHub.Batch.AsyncDynamicApi.Exceptions;

namespace JP.DataHub.Batch.AsyncDynamicApi.Functions
{
    public class AsyncDynamicApiFunctionsForOci: AsyncDynamicApiFunctions
    {
        public AsyncDynamicApiFunctionsForOci(
            IProrcessMananagementService procMngSrvc, ILoggerFactory loggerFactory,
            IConfiguration config): base(procMngSrvc, loggerFactory, config)
        {
        }
        
        public async Task<int> ProcessAsyncDynamicApiMessage(string eventMessage)
        {
            var writeCount = 0;
            Logger.LogInformation($"{nameof(AsyncDynamicApiFunctions)} Process Start");

            try
            {
                Logger.LogInformation($"Start Async {eventMessage}");
                var messageModel = JsonConvert.DeserializeObject<ReciveMessageModel>(eventMessage);

                await retryPolicyAsync.ExecuteAsync(async () => {
                    try
                    {
                        await _prorcessMnanagementService.Execute(messageModel, Logger);
                        writeCount++;
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"{ex.Message} {ex.StackTrace}");
                        throw;
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogCritical($"Error LoggingEventProcess:{ex.Message}");
                throw;
            }

            return writeCount;
        }
    }
}
