using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using JP.DataHub.Batch.LoggingEventProcess.Models;
using JP.DataHub.Batch.LoggingEventProcess.Services.Interfaces;
using System.Text;
using JP.DataHub.Infrastructure.Core.Storage;

namespace JP.DataHub.Batch.LoggingEventProcess.Functions
{
    public class LoggingFunctionsForOci: LoggingFunctions
    {
        protected readonly OciObjectStorageClient objectStorageClient;

        public LoggingFunctionsForOci(
            ILoggingService loggingService,
            ILoggerFactory loggerFactory,
            IConfiguration config
            ) : base(loggingService, loggerFactory, config)
        {
            objectStorageClient = new OciObjectStorageClient(
                config.GetValue<string>("OciCredential:ConfigurationFilePath"),
                config.GetValue<string>("OracleObjectStorage:NamespaceName"),
                config.GetValue<string>("OracleObjectStorage:BucketName"),
                config.GetValue<string>("OracleObjectStorage:RootPath"));
        }

        public async Task<int> ProcessLoggingEventMessageAsync(string eventMessages)
        {
            var writeCount = 0;
            try
            {
                var logEvents = new List<LoggingEventModel>();
                var loggingEvents = JsonConvert.DeserializeObject<List<LoggingEventModel>>(eventMessages);

                foreach (var m in loggingEvents)
                {
                    if (!string.IsNullOrEmpty(m.LogId))
                    {
                        Logger.LogInformation($"Logging Event Part:{m.PartitionKey} logid:{m.LogId} Url:{m.HttpMethodType} {m.Url} seq:{m.SequenceNumber} enqueueTime:{m.EnqueuedTime} offset:{m.Offset}");
                        logEvents.Add(m);
                }
                }

                if (logEvents.Count == 0)
                {
                    Logger.LogInformation("logid is null");
                    return 0;
                }
                else
                {
                    writeCount = logEvents.Count;
                }

                await retryPolicyAsync.ExecuteAsync(async () => {
                    try
                    {
                        foreach (var logEvent in logEvents)
                        {
                            await SaveBackUpFile(logEvent, Logger);
                        }

                        Console.Error.WriteLine("[DEBUG] log Execute...");
                        await Execute(logEvents, Logger);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"{ex.Message} {ex.StackTrace}");
                        throw;
                    }
                });
            }
            catch (Exception e)
            {
                Logger.LogCritical($"Error LoggingEventProcess:{e.Message}");
                throw;
            }

            return writeCount;
        }

        public Task SaveBackUpFile(LoggingEventModel logEvent, ILogger Logger)
        {
            Logger.LogInformation($"SaveBackUpFile InstanceId={logEvent.InstanceId} apiid={logEvent.ApiId} Date={logEvent.RequestDate}");

            var objectName = $"{Container}/{logEvent.RequestDate.ToString("yyyy/MM/dd/HH")}/{logEvent.LogId}_{logEvent.LoggingEventStatus}.json";
            objectStorageClient.CopyTo(objectName, new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(logEvent))));
            Logger.LogInformation($"SaveBackUpFile InstanceId={logEvent.InstanceId} End");
            return Task.CompletedTask;
        }
    }
}