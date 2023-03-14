using JP.DataHub.Batch.DomainDataSync.Models;
using JP.DataHub.Batch.DomainDataSync.Usecase;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using System.Runtime.Caching;
using Unity;

namespace JP.DataHub.Batch.DomainDataSync.Functions
{
    public class DomainDataSyncFunctions
    {
        protected readonly ILogger Logger;
        protected readonly IUnityContainer unityContainer;
        protected readonly ISyncUsecase _syncUsecase;
        protected readonly IAsyncPolicy retryPolicyAsync;

        public DomainDataSyncFunctions(
            ISyncUsecase syncUsecase,
            ILoggerFactory loggerFactory,
            IConfiguration config
            )
        {
            _syncUsecase = syncUsecase;
            unityContainer = DomainDataSyncUnityContainer.Resolve<IUnityContainer>();
            Logger = loggerFactory.CreateLogger<DomainDataSyncFunctions>();
            retryPolicyAsync = Policy.Handle<Exception>()
                .WaitAndRetryAsync(config.GetValue<int>("DomainDataSyncSetting:MaxNumberOfAttempts", 5), i => TimeSpan.FromSeconds(config.GetValue<double>("DomainDataSyncSetting:RetryDelaySec", 60)));
        }

        /// <summary>
        /// Topic受信
        /// </summary>
        /// <param name="message">管理画面でDBに変更があった時に飛んでくるTopic</param>
        /// <remarks>ServiceBusTriggerの属性定義はApp.config参照</remarks>
        [Function("ProcessServiceBusQueue")]
        public async Task ProcessServiceBusQueue([ServiceBusTrigger(queueName: "%ServiceBusQueueName%",  Connection = "AzureServiceBusConnectionStrings")]string serviceBusMessage)
        {
            var message = JsonConvert.DeserializeObject<DomainDataSyncEventModel>(serviceBusMessage);
            // 起動引数
            CheckServiceBusParameter(message);
            Logger.LogInformation($"{ nameof(ProcessServiceBusQueue) } EventName = { message.EventName } PkValue = { message.PkValue }");

            // 連続して同じPKが来た場合は無視
            MemoryCache cache = MemoryCache.Default;

            if (cache["EventName"] == null && cache["PkValue"] == null)
            {
                var policy = new CacheItemPolicy()
                {
                    SlidingExpiration = new TimeSpan(0, 0, 3)
                };
                cache.Add(new CacheItem("EventName", message.EventName), policy);
                cache.Add(new CacheItem("PkValue", message.PkValue), policy);
            }
            else
            {
                if (cache["PkValue"].ToString() == message.PkValue)
                {
                    Logger.LogInformation("同期処理が連続して予約されたためスキップします。");
                    return;
                }
            }
            
            await retryPolicyAsync.ExecuteAsync(async () => {
                try
                {
                    // 同期開始
                    await _syncUsecase.Sync(message);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{ex.Message} {ex.StackTrace}");
                    throw;
                }
            });
            
            return;
        }

        protected static void CheckServiceBusParameter(DomainDataSyncEventModel message)
        {
            if (string.IsNullOrEmpty(message.EventName))
            {
                throw new Exception("パラメータ：EventNameが存在しません。");
            }

            if (string.IsNullOrEmpty(message.PkValue))
            {
                throw new Exception("パラメータ：PkValueが存在しません。");
            }
        }

        /// <summary>
        /// 定期実行
        /// </summary>
        /// <param name="timerInfo">定期実行周期</param>
        /// <remarks>TimerTriggerの属性定義はApp.config参照</remarks>
        [Function("ProcessTimer")]
        public async Task ProcessTimer([TimerTrigger("%TimerTrigger%")] TimerInfo timerInfo)
        {
            Logger.LogInformation($"{ nameof(ProcessTimer) } Start Parameter Nothing");
            await retryPolicyAsync.ExecuteAsync(async () => {
                try
                {
                    // 同期開始
                    await _syncUsecase.SyncAll(false);
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{ex.Message} {ex.StackTrace}");
                    throw;
                }
            });
            Logger.LogInformation($"{nameof(ProcessTimer)} End");
            return;
        }

        /// <summary>
        /// 全件同期
        /// </summary>
        /// <remarks>更新日付が一致しないもののみ同期する。</remarks>
        [Function("ProcessQueueMessage")]
        public void ProcessQueueMessage([QueueTrigger(queueName: "%AllSyncQueueName%", Connection = "DomainDataSyncStorageConnectionStrings")]string message)
        {
            Logger.LogInformation($"{ nameof(ProcessQueueMessage) } AllSync Start massage={message}");
            try
            {
                // 同期開始
                _syncUsecase.SyncAll(false).Wait();
            }
            catch (Exception ex)
            {
                //手動実行前提のためリトライはさせずログを出して終了。
                Logger.LogCritical($"{ nameof(ProcessQueueMessage) }Sync Error:{ex}");
                throw;
            }
            Logger.LogInformation($"{nameof(ProcessQueueMessage)} AllSync End massage={message}");
            return;
        }

        /// <summary>
        /// 全件同期
        /// </summary>
        /// <remarks>更新日付にかかわらず同期する。</remarks>
        [Function("ProcessForceQueueMessage")]
        public void ProcessForceQueueMessage([QueueTrigger(queueName: "%AllSyncForceQueueName%", Connection = "DomainDataSyncStorageConnectionStrings")]string message)
        {
            Logger.LogInformation($"{ nameof(ProcessForceQueueMessage) } AllSyncForce Start massage={message}");
            try
            {
                // 同期開始
                _syncUsecase.SyncAll(true).Wait();
            }
            catch (Exception ex)
            {
                //手動実行前提のためリトライはさせずログを出して終了。
                Logger.LogCritical($"{ nameof(ProcessForceQueueMessage) }AllSyncForce Error:{ex}");
                throw;
            }
            Logger.LogInformation($"{nameof(ProcessForceQueueMessage)} AllSyncForce End massage={message}");
            return;
        }
    }
}