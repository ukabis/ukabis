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
    public class TrailFunctionsForOci: TrailFunctions
    {
        public TrailFunctionsForOci(
            IProrcessMananagementService procMngSrvc,
            ILoggerFactory loggerFactory,
            IConfiguration config
            ): base(procMngSrvc, loggerFactory, config)
        {
        }

        public async Task<int> ProcessTrailEventMessage(string message)
        {
            var writeCount = 0;
            Logger.LogInformation($"JobStart ProcessTrailEventMessage ");

            try
            {
                var models = JsonConvert.DeserializeObject<List<TrailEventModel>>(message);
                foreach (var m in models)
                {
                    if (String.IsNullOrEmpty(m.TrailId))
                    {
                        Logger.LogInformation("TrailId is null");
                        return writeCount;
                    }

                    await retryPolicyAsync.ExecuteAsync(async () => {
                        try
                        {
                            await _prorcessMnanagementService.Execute(m, m.TrailId, Logger);
                            writeCount++;
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"{ex.Message} {ex.StackTrace}");
                            throw;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error Trail functions:{ex.Message} eventdata={message}");
                throw;
            }

            return writeCount;
        }
    }
}