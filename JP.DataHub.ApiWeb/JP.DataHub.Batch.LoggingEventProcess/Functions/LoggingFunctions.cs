using Azure.Messaging.EventHubs;
using JP.DataHub.Batch.LoggingEventProcess.Models;
using JP.DataHub.Batch.LoggingEventProcess.Services.Interfaces;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Core.Storage;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Interception.Utilities;

namespace JP.DataHub.Batch.LoggingEventProcess.Functions
{
    public class LoggingFunctions
    {
        protected readonly ILogger Logger;
        protected readonly IUnityContainer unityContainer;
        protected readonly ILoggingService _loggingService;
        protected readonly string Container;
        protected readonly BlobStorageClient blobClient;
        protected readonly IAsyncPolicy retryPolicyAsync;

        public LoggingFunctions(
            ILoggingService loggingService,
            ILoggerFactory loggerFactory,
            IConfiguration config
            )
        {
            _loggingService = loggingService;
            unityContainer = LoggingEventProcessUnityContainer.Resolve<IUnityContainer>();
            Logger = loggerFactory.CreateLogger<LoggingFunctions>();
            Container = config.GetValue<string>("LoggingEventProcessSetting:RootPath");
            blobClient = new BlobStorageClient(config.GetValue<string>("ConnectionStrings:LogBackUpStorage"),
                config.GetValue<string>("LoggingEventProcessSetting:ContainerName"), config.GetValue<string>("LoggingEventProcessSetting:RootPath"));
            retryPolicyAsync = Policy.Handle<Exception>()
                .WaitAndRetryAsync(config.GetValue<int>("LoggingEventProcessSetting:MaxNumberOfAttempts", 5), i => TimeSpan.FromSeconds(config.GetValue<double>("LoggingEventProcessSetting:RetryDelaySec", 60)));
        }

        [Function(nameof(LoggingFunctions))]
        public async Task ProcessLoggingEventMessageAsync([EventHubTrigger("%Logging_EventHubName%", Connection = "AzureEventHubConnectionStrings")] string[] eventHubMessages)
        {
            try
            {
                var logEvents = new List<LoggingEventModel>();
                foreach (var eventData in eventHubMessages)
                {
                    var message = JsonConvert.DeserializeObject<LoggingEventModel>(eventData);

                    if (!string.IsNullOrEmpty(message.LogId))
                    {
                        Logger.LogInformation($"Logging Event Part:{message.PartitionKey} logid:{message.LogId} Url:{message.HttpMethodType} {message.Url} seq:{message.SequenceNumber} enqueueTime:{message.EnqueuedTime} offset:{message.Offset}");
                        logEvents.Add(message);
                    }
                }

                if (logEvents.Count == 0)
                {
                    Logger.LogInformation("logid is null");
                    return;
                }

                await retryPolicyAsync.ExecuteAsync(async () => {
                    try
                    {
                        foreach (var logEvent in logEvents)
                        {
                            await SaveBackUpFile(logEvent, Logger);
                        }

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
        }

        public Task Execute(IEnumerable<LoggingEventModel> logEvents, ILogger Logger)
        {
            Logger.LogInformation($"JobStart logid={logEvents.First().LogId} logCount={logEvents.Count()}");

            try
            {
                RegisterLoggingInfo(logEvents, Logger);

                Logger.LogInformation($"Logging  InstanceId {logEvents.First().InstanceId} end");
            }
            catch (AggregateException e)
            {
                Logger.LogError($"Logging RegistFail message = {e.Message}");
                foreach (var innerException in e.InnerExceptions)
                {
                    Logger.LogError($"Logging RegistFail message = {innerException.Message}");
                    Logger.LogError($"Logging RegistFail stack = {innerException.StackTrace}");
                }

                throw;
            }
            catch (Exception e)
            {
                Logger.LogError($"Logging RegistFail message = {e.Message}");
                Logger.LogError($"Logging RegistFail stack = {e.StackTrace}");
                Console.Error.WriteLine($"[DEBUG] Logging RegistFail message = {e.Message}");
                Console.Error.WriteLine($"[DEBUG] Logging RegistFail stack = {e.StackTrace}");
                throw;
            }
            return Task.CompletedTask;
        }

        public Task SaveBackUpFile(LoggingEventModel logEvent, ILogger Logger)
        {
            Logger.LogInformation($"SaveBackUpFile InstanceId={logEvent.InstanceId} apiid={logEvent.ApiId} Date={logEvent.RequestDate}");

            var blobName = $"{Container}/{logEvent.RequestDate.ToString("yyyy/MM/dd/HH")}/{logEvent.LogId}_{logEvent.LoggingEventStatus}.json";
            blobClient.CopyTo(blobName, new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(logEvent))));
            Logger.LogInformation($"SaveBackUpFile InstanceId={logEvent.InstanceId} End");
            return Task.CompletedTask;
        }

        public void RegisterLoggingInfo(IEnumerable<LoggingEventModel> logEvents, ILogger Logger)
        {
            Logger.LogInformation($"RegistLoggingInfo InstanceId={logEvents.First().InstanceId} apiid={logEvents.First().ApiId} Date={logEvents.First().RequestDate}");

            foreach (var logEvent in logEvents)
            {
                var vendorSystem = GetVendorSystem(logEvent.ApiId);
                logEvent.ProviderSystemId = vendorSystem?.ProviderSystemId ?? "";
                logEvent.ProviderVendorId = vendorSystem?.ProviderVendorId ?? "";
            }
            var models = CreateLoggingInfoModels(logEvents, Logger);
            models.ForEach(x => _loggingService.RegistLoggingInfo(x));
            Logger.LogInformation($"RegistLoggingInfo End");

            foreach (var summaryEventGroup in logEvents.Select(l => new SendSummaryEventModel
            {
                ApiId = l.ApiId,
                ControllerId = l.ControllerId,
                ProviderSystemId = l.ProviderSystemId,
                ProviderVendorId = l.ProviderVendorId,
                SystemId = l.SystemId,
                VendorId = l.VendorId,
                RequestDate = l.RequestDate.TruncateSecond()

            }).GroupBy(s => new { s.ApiId, s.ProviderSystemId, s.SystemId, s.RequestDate }))
            {
                var summaryEvent = summaryEventGroup.First();
                _loggingService.SendSummaryEvent(summaryEvent);
                Logger.LogInformation($"Sent Event ApiId={summaryEvent.ApiId} ProviderSystemId={summaryEvent.ProviderSystemId} SystemId={summaryEvent.SystemId} Date={summaryEvent.RequestDate}");
            }

            return;

            ProviderVendorSystemModel GetVendorSystem(string apiId)
            {
                if (string.IsNullOrEmpty(apiId)) return null;
                return _loggingService.GetProviderVendorSystemId(apiId);
            }
        }

        public ProviderVendorSystemModel GetVendorSystem(string apiId, ILogger Logger)
        {
            Logger.LogInformation($"GetVendorSystem apiid={apiId}");
            return _loggingService.GetProviderVendorSystemId(apiId);
        }

        private IEnumerable<LoggingInfoModel> CreateLoggingInfoModels(IEnumerable<LoggingEventModel> logEvents, ILogger Logger)
        {
            List<LoggingInfoModel> result = new List<LoggingInfoModel>();

            foreach (var logId in logEvents.GroupBy(x => x.LogId).Select(x => x.Key))
            {
                var logs = logEvents.Where(x => x.LogId == logId);
                LoggingInfoModel.RegisterTypeEnum registerType = LoggingInfoModel.RegisterTypeEnum.All;
                LoggingEventModel margeLogEvent = null;
                foreach (var log in logs.OrderBy(x => x.LoggingEventStatus))
                {
                    if (margeLogEvent == null)
                    {
                        margeLogEvent = log;
                    }
                    else
                    {
                        margeLogEvent = MargeLogEvent(margeLogEvent, log);
                    }

                    //開始
                    if (log.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.Begin)
                    {
                        registerType = LoggingInfoModel.RegisterTypeEnum.BeginOnly;
                    }
                    //リクエスト
                    if (log.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.Request)
                    {
                        registerType = LoggingInfoModel.RegisterTypeEnum.Request;
                    }
                    //リクエスト⇒レスポンスは全部入り
                    if (registerType == LoggingInfoModel.RegisterTypeEnum.Request && log.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.Response)
                    {
                        registerType = LoggingInfoModel.RegisterTypeEnum.All;
                    }
                    //レスポンス
                    else if (log.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.Response)
                    {
                        registerType = LoggingInfoModel.RegisterTypeEnum.Response;
                    }
                    //全部入り
                    if (log.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.All)
                    {
                        registerType = LoggingInfoModel.RegisterTypeEnum.All;
                    }
                }
                result.Add(EditLoggingModel(margeLogEvent, registerType, Logger));
            }
            return result;
        }


        private static LoggingEventModel MargeLogEvent(LoggingEventModel source, LoggingEventModel target)
        {
            if (target.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.Request)
            {
                source.RequestContentLength = target.RequestContentLength;
                source.RequestBody = target.RequestBody;
            }
            else if (target.LoggingEventStatus == LoggingEventModel.LoggingEventStatusEnum.Response)
            {
                source.HttpStatusCode = target.HttpStatusCode;
                source.ExecuteTime = target.ExecuteTime;
                source.ResponseBody = target.ResponseBody;
                source.ResponseContentLength = target.ResponseContentLength;
                source.ResponseContentType = target.ResponseContentType;
                source.ResponseHeaders = target.ResponseHeaders;
            }
            return source;
        }


        public LoggingInfoModel EditLoggingModel(LoggingEventModel loggingEventModel, LoggingInfoModel.RegisterTypeEnum registerType, ILogger Logger)
        {
            Logger.LogInformation($"EditLoggingModel apiid={loggingEventModel.ApiId} Date={loggingEventModel.RequestDate}");
            ProviderVendorSystemModel vendorSystem = null;
            if (!string.IsNullOrEmpty(loggingEventModel.ApiId))
            {
                vendorSystem = GetVendorSystem(loggingEventModel.ApiId, Logger);
            }
            else
            {
                Logger.LogInformation(
                    $"logid = {loggingEventModel.LogId} url={loggingEventModel.Url} apiId is null or empty");
            }

            return new LoggingInfoModel
            {
                LogId = loggingEventModel.LogId,
                ActionName = loggingEventModel.ActionName,
                ApiId = loggingEventModel.ApiId,
                ClientIpAddress = loggingEventModel.ClientIpAddress,
                ControllerId = loggingEventModel.ControllerId,
                ControllerName = loggingEventModel.ControllerName,
                ExecuteTime = loggingEventModel.ExecuteTime,
                HttpMethodType = loggingEventModel.HttpMethodType,
                HttpStatusCode = ((int)loggingEventModel.HttpStatusCode).ToString(),
                OpenId = loggingEventModel.OpenId,
                ProviderSystemId = vendorSystem?.ProviderSystemId ?? "",
                ProviderVendorId = vendorSystem?.ProviderVendorId ?? "",
                QueryString = loggingEventModel.QueryString,
                RequestBody = loggingEventModel.RequestBody,
                RequestContentLength = loggingEventModel.RequestContentLength,
                RequestContentType = loggingEventModel.RequestContentType,
                RequestDate = loggingEventModel.RequestDate,
                RequestHeaders = JsonConvert.SerializeObject(loggingEventModel.RequestHeaders),
                ResponseBody = loggingEventModel.ResponseBody,
                ResponseContentLength = loggingEventModel.ResponseContentLength,
                ResponseContentType = loggingEventModel.ResponseContentType,
                ResponseHeaders = JsonConvert.SerializeObject(loggingEventModel.ResponseHeaders),
                SystemId = loggingEventModel.SystemId,
                Url = loggingEventModel.Url,
                VendorId = loggingEventModel.VendorId,
                IsInternalCall = loggingEventModel.IsInternalCall
            };

        }
    }
}