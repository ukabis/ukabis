using JP.DataHub.Batch.TrailEventProcess.Models;
using JP.DataHub.Batch.TrailEventProcess.Services.Interfaces;
using JP.DataHub.Com.Unity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Batch.TrailEventProcess.Functions
{
    public class TrailFunctions
    {
        protected readonly ILogger Logger;
        protected readonly IUnityContainer unityContainer;
        protected readonly IProrcessMananagementService _prorcessMnanagementService;
        protected readonly string Container;
        protected readonly IAsyncPolicy retryPolicyAsync;

        public TrailFunctions(
            IProrcessMananagementService procMngSrvc,
            ILoggerFactory loggerFactory,
            IConfiguration config
            )
        {
            _prorcessMnanagementService = procMngSrvc;
            unityContainer = TrailEventProcessUnityContainer.Resolve<IUnityContainer>();
            Logger = loggerFactory.CreateLogger<TrailFunctions>();
            Container = config.GetValue<string>("TrailEventProcessSetting:TrailBackupPath");
            retryPolicyAsync = Policy.Handle<Exception>()
                .WaitAndRetryAsync(config.GetValue<int>("TrailEventProcessSetting:MaxNumberOfAttempts", 5), i => TimeSpan.FromSeconds(config.GetValue<double>("TrailEventProcessSetting:RetryDelaySec", 60)));
        }

        [Function(nameof(TrailFunctions))]
        public async Task ProcessTrailEventMessage([ServiceBusTrigger(queueName: "%ServiceBusQueueName%", Connection = "AzureEventHubConnectionStrings")] string serviceBusMessageing)
        {
            Logger.LogInformation($"JobStart ProcessTrailEventMessage ");
            var reciveMessage = JsonConvert.DeserializeObject<TrailEventModel>(serviceBusMessageing);

            if (String.IsNullOrEmpty(reciveMessage.TrailId))
            {
                Logger.LogInformation("TrailId is null");
                return;
            }

            await retryPolicyAsync.ExecuteAsync(async () => {
                try
                {
                    await _prorcessMnanagementService.Execute(reciveMessage, reciveMessage.TrailId, Logger);
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