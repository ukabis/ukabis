using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using Unity;
using JP.DataHub.Com.Settings;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    [CacheKey]
    internal class ContainerDynamicSeparationRepository : AbstractRepository, IContainerDynamicSeparationRepository
    {
        [CacheKey(CacheKeyType.EntityWithKey, "repository")]
        public static string CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY = "ContainerDynamicSeparationRepository.GetContainerName";

        [CacheKey(CacheKeyType.EntityWithKey, "repository")]
        public static string CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY_ALL = "ContainerDynamicSeparationRepository.GetAllContainerNames";

        private readonly TimeSpan _cacheExpireTime = TimeSpan.Parse("24:00:00");


        private ICache _cache => _lazyCache.Value;
        private Lazy<ICache> _lazyCache = new(() => UnityCore.Resolve<ICache>());

        private IJPDataHubDbConnection _dbConnection => _lazyDbConnection.Value;
        private Lazy<IJPDataHubDbConnection> _lazyDbConnection = new(() => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi"));

#if Oracle
        private IStreamingServiceEventRepository _streamingServiceEventRepository => _lazyStreamingServiceEventRepository.Value;
        private Lazy<IStreamingServiceEventRepository> _lazyStreamingServiceEventRepository = new(() => UnityCore.Resolve<IStreamingServiceEventRepository>("DomainDataSyncOci"));
#else
        private IServiceBusEventRepository _serviceBusEventRepository => _lazyServiceBusEventRepository.Value;
        private Lazy<IServiceBusEventRepository> _lazyServiceBusEventRepository = new(() => UnityCore.Resolve<IServiceBusEventRepository>("DomainDataSync"));
#endif


        /// <summary>
        /// コンテナ名を取得する。存在しなければ作成する。
        /// </summary>
        public string GetOrRegisterContainerName(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId, OpenId openId = null)
        {
            if (openId == null)
            {
                openId = new OpenId(Guid.Empty.ToString());
            }

            return GetOrRegisterContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId, out _);
        }

        /// <summary>
        /// コンテナ名を取得する。存在しなければ作成する。
        /// </summary>
        public string GetOrRegisterContainerName(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId, OpenId openId, out bool isRegistered)
        {
            if (string.IsNullOrEmpty(physicalRepositoryId?.Value) ||
                string.IsNullOrEmpty(controllerId?.Value) ||
                string.IsNullOrEmpty(vendorId?.Value) ||
                string.IsNullOrEmpty(systemId?.Value) ||
                string.IsNullOrEmpty(openId?.Value))
            {
                throw new ArgumentException();
            }
            isRegistered = false;

            // 登録済ならコンテナ名を返す
            var containerName = GetContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId);
            if (!string.IsNullOrEmpty(containerName))
            {
                return containerName;
            }

            // 未登録なら登録して返す
            try
            {
                containerName = Register(physicalRepositoryId, controllerId, vendorId, systemId, openId);
                isRegistered = true;
                return containerName;
            }
            catch (SqlException ex) when (ex.Number == 2627)
            {
                // キー重複エラー時は再取得して返す
                _cache.Remove(CacheManager.CreateKey(CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY, physicalRepositoryId.Value, controllerId.Value, vendorId.Value, systemId.Value, openId.Value));
                return GetContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId);
            }
        }

        /// <summary>
        /// リソースの全コンテナ名を取得する。
        /// </summary>
        public IList<string> GetAllContainerNames(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
	c.container_name
FROM 
	container_dynamic_separation c
    INNER JOIN physical_repository p 
        ON c.physical_repository_id = p.physical_repository_id 
       AND p.is_active = 1 
    INNER JOIN repository_group r 
        ON p.repository_group_id = r.repository_group_id 
       AND r.is_active = 1 
       AND r.is_enable = 1
WHERE 
	c.physical_repository_id = /*ds physical_repository_id*/'' 
AND c.controller_id = /*ds controller_id*/'' 
AND c.is_active = 1";
            }
            else
            {
                sql = @"
SELECT 
	c.container_name
FROM 
	ContainerDynamicSeparation c
    INNER JOIN PhysicalRepository p 
        ON c.physical_repository_id = p.physical_repository_id 
       AND p.is_active = 1 
    INNER JOIN RepositoryGroup r 
        ON p.repository_group_id = r.repository_group_id 
       AND r.is_active = 1 
       AND r.is_enable = 1
WHERE 
	c.physical_repository_id = @physical_repository_id
AND c.controller_id = @controller_id
AND c.is_active = 1";
            }
            var param = new {
                physical_repository_id = physicalRepositoryId.Value,
                controller_id = controllerId.Value
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = this._cache.Get<List<string>>
            (
                CacheManager.CreateKey(CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY_ALL, physicalRepositoryId.Value, controllerId.Value),
                _cacheExpireTime,
                () => _dbConnection.Query<string>
                (
                    twowaySql.Sql, dynParams
                )
            );

            return result;
        }

        /// <summary>
        /// コンテナ名を取得する。
        /// </summary>
        private string GetContainerName(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId, OpenId openId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT 
	c.container_name
FROM 
	container_dynamic_separation c
    INNER JOIN physical_repository p 
        ON c.physical_repository_id = p.physical_repository_id 
       AND p.is_active = 1 
    INNER JOIN repository_group r 
        ON p.repository_group_id = r.repository_group_id 
       AND r.is_active = 1 
       AND r.is_enable = 1
WHERE 
	c.physical_repository_id = /*ds physical_repository_id*/'' 
AND c.controller_id = /*ds controller_id*/'' 
AND c.vendor_id = /*ds vendor_id*/'' 
AND c.system_id = /*ds system_id*/'' 
AND c.open_id = /*ds open_id*/'' 
AND c.is_active = 1";
            }
            else
            {
                sql = @"
SELECT 
	c.container_name
FROM 
	ContainerDynamicSeparation c
    INNER JOIN PhysicalRepository p 
        ON c.physical_repository_id = p.physical_repository_id 
       AND p.is_active = 1 
    INNER JOIN RepositoryGroup r 
        ON p.repository_group_id = r.repository_group_id 
       AND r.is_active = 1 
       AND r.is_enable = 1
WHERE 
	c.physical_repository_id = @physical_repository_id
AND c.controller_id = @controller_id
AND c.vendor_id = @vendor_id
AND c.system_id = @system_id
AND c.open_id = @open_id
AND c.is_active = 1";
            }
            var param = new
            {
                physical_repository_id = physicalRepositoryId.Value,
                controller_id = controllerId.Value,
                vendor_id = vendorId.Value,
                system_id = systemId.Value,
                open_id = openId.Value
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = this._cache.Get<string>
            (
                CacheManager.CreateKey(CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY, physicalRepositoryId.Value, controllerId.Value, vendorId.Value, systemId.Value, openId.Value),
                _cacheExpireTime,
                () => _dbConnection.QuerySingleOrDefault<string>
                (
                    twowaySql.Sql, dynParams
                )
            );

            return result;
        }

        /// <summary>
        /// コンテナ分離の情報を登録する。
        /// </summary>
        private string Register(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId, OpenId openId)
        {
            var containerDynamicSeparationId = Guid.NewGuid().ToString();
            var containerName = Guid.NewGuid().ToString();

            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
INSERT INTO Container_dynamic_separation
(
    container_dynamic_separation_id
    ,physical_repository_id
    ,controller_id
    ,vendor_id
    ,system_id
    ,open_id
    ,container_name
    ,reg_date
    ,reg_username
    ,upd_date
    ,upd_username
    ,is_active
)
VALUES
(
    /*ds container_dynamic_separation_id*/'' 
    ,/*ds physical_repository_id*/'' 
    ,/*ds controller_id*/'' 
    ,/*ds vendor_id*/'' 
    ,/*ds system_id*/'' 
    ,/*ds open_id*/'' 
    ,/*ds container_name*/'' 
    ,/*ds reg_date*/'' 
    ,/*ds reg_username*/'' 
    ,/*ds upd_date*/'' 
    ,/*ds upd_username*/'' 
    ,/*ds is_active*/'' 
)";
            }
            else
            {
                sql = @"
INSERT INTO ContainerDynamicSeparation
(
    container_dynamic_separation_id
    ,physical_repository_id
    ,controller_id
    ,vendor_id
    ,system_id
    ,open_id
    ,container_name
    ,reg_date
    ,reg_username
    ,upd_date
    ,upd_username
    ,is_active
)
VALUES
(
    @container_dynamic_separation_id
    ,@physical_repository_id
    ,@controller_id
    ,@vendor_id
    ,@system_id
    ,@open_id
    ,@container_name
    ,@reg_date
    ,@reg_username
    ,@upd_date
    ,@upd_username
    ,@is_active
)";
            }

            var now = this.PerRequestDataContainer.GetDateTimeUtil().GetUtc(this.PerRequestDataContainer.GetDateTimeUtil().LocalNow);

            // OpenID認証以外の場合はダミーのIDを使用
            var updUserId = string.IsNullOrWhiteSpace(this.PerRequestDataContainer.OpenId)
                ? "system"
                : this.PerRequestDataContainer.OpenId;

            var param = new
            {
                container_dynamic_separation_id = containerDynamicSeparationId,
                physical_repository_id = physicalRepositoryId.Value,
                controller_id = controllerId.Value,
                vendor_id = vendorId.Value,
                system_id = systemId.Value,
                open_id = openId.Value,
                container_name = containerName,
                reg_date = now,
                reg_username = updUserId,
                upd_date = now,
                upd_username = updUserId,
                is_active = true
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, new
            {
                container_dynamic_separation_id = true,
                physical_repository_id = true,
                controller_id = true,
                vendor_id = true,
                system_id = true,
                open_id = true,
                container_name = true,
                reg_date = true,
                reg_username = true,
                upd_date = true,
                upd_username = true,
                is_active = true,
            });
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            _dbConnection.Execute(twowaySql.Sql, dynParams);

            // キャッシュ削除
            _cache.Remove(CacheManager.CreateKey(CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY, physicalRepositoryId.Value, controllerId.Value, vendorId.Value, systemId.Value, openId.Value));
            _cache.Remove(CacheManager.CreateKey(CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY_ALL, physicalRepositoryId.Value, controllerId.Value));

            // DomainDataSync
#if Oracle
            _streamingServiceEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "ContainerDynamicSeparationSync", PkValue = containerDynamicSeparationId });
#else
            _serviceBusEventRepository.SendObjectAsync(new DomainDataSyncObject() { EventName = "ContainerDynamicSeparationSync", PkValue = containerDynamicSeparationId });
#endif

            return containerName;
        }
    }
}
