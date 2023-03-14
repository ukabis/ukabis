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
    public class RevokeFunctionsForOci: RevokeFunctions
    {
        public RevokeFunctionsForOci(
            IRevokeService revokeService,
            ILogger<RevokeFunctionsForOci> logger
            ): base(revokeService, logger)
        {
        }

        public async Task<int> ProcessRevokeEventMessage(string messages)
        {
            var writeCount = 0;
            _logger.LogInformation($"JobStart ProcessRevokeEventMessage ");

            try
            {
                var models = JsonConvert.DeserializeObject<List<RevokeNotifyModel>>(messages);
                foreach (var m in models)
                {
                    Policy.Handle<Exception>().WaitAndRetry(_revokeSettings.MaxNumberOfAttempts, i => TimeSpan.FromSeconds(_revokeSettings.RetryDelaySec)).Execute(() => {
                        try
                        {
                            _revokeService.Revoke(m);
                            writeCount++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"error revoke functions. eventdata={messages} ex={ex}");
                            throw;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error revoke functions:{ex.Message} eventdata={messages}");
                throw;
            }

            return writeCount;
        }
    }
}