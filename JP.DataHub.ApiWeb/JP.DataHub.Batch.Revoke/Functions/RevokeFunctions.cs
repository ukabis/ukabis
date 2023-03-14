using JP.DataHub.Batch.Revoke.Models;
using JP.DataHub.Batch.Revoke.Services;
using JP.DataHub.Com.Unity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace JP.DataHub.Batch.Revoke.Functions
{
    public class RevokeFunctions
    {
        protected readonly IUnityContainer unityContainer;
        protected readonly string Container;
        protected readonly IRevokeService _revokeService;        
        protected readonly RevokeSettings _revokeSettings = UnityCore.Resolve<RevokeSettings>();

        protected readonly ILogger<RevokeFunctions> _logger;

        public RevokeFunctions(
            IRevokeService revokeService,
            ILogger<RevokeFunctions> logger
            )
        {
            this.AutoInjection();
            _revokeService = revokeService;
            _logger = logger;
        }

        [Function(nameof(RevokeFunctions))]
        public async Task ProcessRevokeEventMessage([EventHubTrigger("%Revoke_EventHubName%", Connection = "AzureEventHubConnectionStrings")] string[] eventHubMessages)
        {
            _logger.LogInformation($"JobStart ProcessRevokeEventMessage ");
            var reciveMessage = new RevokeNotifyModel();

            foreach (var eventData in eventHubMessages)
            {
                reciveMessage = JsonConvert.DeserializeObject<RevokeNotifyModel>(eventData);
                Policy.Handle<Exception>().WaitAndRetry(_revokeSettings.MaxNumberOfAttempts, i => TimeSpan.FromSeconds(_revokeSettings.RetryDelaySec)).Execute(() => {
                    try
                    {
                        _revokeService.Revoke(reciveMessage);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"error revoke functions. eventdata={eventData} ex={ex}");
                        throw;
                    }
                });
            }
        }
    }
}