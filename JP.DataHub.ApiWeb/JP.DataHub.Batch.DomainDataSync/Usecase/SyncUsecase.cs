using JP.DataHub.Batch.DomainDataSync.Domain;
using JP.DataHub.Batch.DomainDataSync.Models;
using JP.DataHub.Batch.DomainDataSync.Repository;
using JP.DataHub.Batch.DomainDataSync.Usecase;
using JP.DataHub.Com.Unity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace JP.DataHub.Batch.DomainDataSync.Usecase
{
    public class SyncUsecase : ISyncUsecase
    {

        private readonly IUnityContainer unityContainer;
        private readonly ILogger<SyncUsecase> Logger;
        private readonly IConfiguration config;

        private Lazy<ISyncRepository> _lazySyncRepository = new(() => UnityCore.Resolve<ISyncRepository>());
        private ISyncRepository _syncRepository { get => _lazySyncRepository.Value; }
        /// <summary>
        /// ロガー設定
        /// </summary>
        /// <param name="logger">ログ出力する何か</param>
        public SyncUsecase(ILogger<SyncUsecase> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// イベント指定で同期
        /// </summary>
        /// <param name="message">受信Topic</param>
        public async Task Sync(DomainDataSyncEventModel message)
        {
            string eventName = message.EventName;
            string pkValue = message.PkValue;

            // 初期処理
            ISyncEntity entity = UnityCore.Resolve<ISyncRepository>(new ParameterOverride("logger", Logger)).Init();

            // 同期
            int syncCount = entity.Sync(eventName, pkValue);

            if (syncCount > 0)
            {
                Logger.LogInformation($"{ eventName } PkValue = { pkValue } { syncCount }件 同期完了");

                // キャッシュ削除
                await entity.ClearCache(eventName);
            }
        }

        /// <summary>
        /// 全件同期
        /// </summary>
        public async Task SyncAll(bool ignoreUpdateDate)
        {
            // 初期処理
            ISyncEntity entity = UnityCore.Resolve<ISyncRepository>(new ParameterOverride("logger", Logger)).Init();

            // 全件同期
            if (entity.SyncAll(ignoreUpdateDate))
            {
                // キャッシュ削除
                await entity.ClearCacheAll();
            }
        }
    }
}