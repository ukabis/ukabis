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
    public class AsyncDynamicApiFunctions
    {
        protected readonly IProrcessMananagementService _prorcessMnanagementService;
        protected readonly ILogger Logger;
        protected readonly IUnityContainer unityContainer;

        protected readonly IAsyncPolicy retryPolicyAsync;

        public AsyncDynamicApiFunctions(
            IProrcessMananagementService procMngSrvc, ILoggerFactory loggerFactory,
            IConfiguration config)
        {
            _prorcessMnanagementService = procMngSrvc;
            unityContainer = AsyncDyanamicApiUnityContainer.Resolve<IUnityContainer>();
            Logger = loggerFactory.CreateLogger<AsyncDynamicApiFunctions>();
            retryPolicyAsync = Policy.Handle<Exception>(e => e is not AsyncApiRetryOverException)
                .WaitAndRetryAsync(config.GetValue<int>("AsyncDynamicApiSetting:MaxNumberOfAttempts", 5), i => TimeSpan.FromSeconds(config.GetValue<double>("AsyncDynamicApiSetting:RetryDelaySec", 60)));
        }

        [Function(nameof(AsyncDynamicApiFunctions))]
        public async Task ProcessAsyncDynamicApiMessage(
            [EventHubTrigger("%AsyncDynamicApi_EventHubName%", Connection = "AzureEventHubConnectionStrings")] string[] input)
        {
            Logger.LogInformation($"{nameof(AsyncDynamicApiFunctions)} Process Start");

            foreach (var eventData in input)
            {
                var reciveMessage = JsonConvert.DeserializeObject<ReciveMessageModel>(eventData);

                Logger.LogInformation($"Start Async {eventData}");
                await retryPolicyAsync.ExecuteAsync(async () => {
                    try
                    {
                        await _prorcessMnanagementService.Execute(reciveMessage, Logger);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"{ex.Message} {ex.StackTrace}");
                        throw;
                    }
                });
            }
        }
    }
}
