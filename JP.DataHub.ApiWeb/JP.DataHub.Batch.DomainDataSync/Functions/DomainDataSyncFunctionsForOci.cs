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
    public class DomainDataSyncFunctionsForOci: DomainDataSyncFunctions
    {
        public DomainDataSyncFunctionsForOci(
            ISyncUsecase syncUsecase,
            ILoggerFactory loggerFactory,
            IConfiguration config
            ): base(syncUsecase, loggerFactory, config)
        {
        }

        /// <summary>
        /// Topic受信(for oci function)
        /// </summary>
        /// <param name="message">管理画面でDBに変更があった時に飛んでくるTopic</param>
        /// <remarks>ServiceBusTriggerの属性定義はApp.config参照</remarks>
        public async Task<int> ProcessServiceBusQueue(string serviceBusMessage)
        {
            var writeCount = 0;
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
                    return -1;
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
            
            return writeCount;
        }

        /// <summary>
        /// 定期実行
        /// </summary>
        /// <param name="timerInfo">定期実行周期</param>
        /// <remarks>TimerTriggerの属性定義はApp.config参照</remarks>
        public async Task<int> ProcessTimer(string message)
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
            Logger.LogInformation($"{ nameof(ProcessTimer) } End");
            return 0;
        }

        /// <summary>
        /// 全件同期
        /// </summary>
        /// <remarks>更新日付が一致しないもののみ同期する。</remarks>
        public async Task<int> ProcessQueueMessage(string message)
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
            Logger.LogInformation($"{ nameof(ProcessQueueMessage) } AllSync End massage={message}");
            return 0;
        }

        /// <summary>
        /// 全件同期
        /// </summary>
        /// <remarks>更新日付にかかわらず同期する。</remarks>
        public async Task<int> ProcessForceQueueMessage(string message)
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
            Logger.LogInformation($"{ nameof(ProcessForceQueueMessage) } AllSyncForce End massage={message}");
            return 0;
        }
    }
}