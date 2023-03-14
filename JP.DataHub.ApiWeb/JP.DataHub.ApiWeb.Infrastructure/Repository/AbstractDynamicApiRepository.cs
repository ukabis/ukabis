using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using AutoMapper;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.SQL;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Infrastructure.Database.Authority;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Domain.ActionInjector;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.Com.Settings;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using JP.DataHub.Com.Sql;
using Dapper.Oracle;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    [CacheKey]
    internal class AbstractDynamicApiRepository : AbstractRepository
    {
        #region キャッシュ定義
        ///CACHE_KEY_API_LIST・isActiveOnly=true用のキャッシュキー
        [CacheKey(CacheKeyType.Entity, "vendor", "system", "controller", "api", "dataschema")]
        public static string CACHE_KEY_API_LIST_ACTIVE_ONLY = "DynamicApiHelpRepository-AllApiFullList-ActiveOnly";

        ///CACHE_KEY_API_LIST・isActiveOnly=false用のキャッシュキー
        [CacheKey(CacheKeyType.Entity, "vendor", "system", "controller", "api", "dataschema")]
        public static string CACHE_KEY_API_LIST_ALL = "DynamicApiHelpRepository-AllApiFullList-All";

        ///CACHE_KEY_APISIMPLE_LIST_ALL
        [CacheKey(CacheKeyType.Entity, "vendor", "system", "controller", "api", "dataschema")]
        public static string CACHE_KEY_APISIMPLE_LIST_ALL = "DynamicApiHelpRepository-AllApiSimpleList-All";

        [CacheKey(CacheKeyType.Entity, "vendor", "system", "controller", "api", "dataschema")]
        public static string CACHE_KEY_IPFILTER_UNION_NEWVERSION_CONTROLLER = "DynamicApiRepository-IPFilter-Contoroller-NewVersion";

        [CacheKey(CacheKeyType.Entity, "vendor", "system", "controller", "api", "dataschema")]
        public static string CACHE_KEY_ALLAPIIDENTIFIER_LIST = "DynamicApiRepository-AllApiIdentifier-List";

        [CacheKey(CacheKeyType.EntityWithKey, "vendor", "system", "controller", "api", "dataschema")]
        public static string CACHE_KEY_ALLAPIENTITY_CONTROLLER_LIST = "DynamicApiRepository-AllApiEntity-Controller-List";

        [CacheKey(CacheKeyType.Entity, "vendor", "system", "controller", "api", "dataschema", "repository")]
        public static string CACHE_KEY_ALL_SECONDARYREPOSITORY_LIST = "DynamicApiRepository-AllSecondaryRepository-List";

        [CacheKey(CacheKeyType.EntityWithKey, "vendor", "system")]
        public static string CACHE_KEY_CHECK_VENDOR_SYSTEM_COMBINATION = "DynamicApiRepository-CheckVendorSystemCombination";

        [CacheKey(CacheKeyType.EntityWithKey, "controller", "vendor", "system", "api", "repository")]
        public static string CACHE_KEY_API_LIST_DESCRIPTION = "DynamicApiHelpRepository-ApiDescription";

        [CacheKey(CacheKeyType.EntityWithKey, "dataschema")]
        public static string CACHE_KEY_API_LIST_SCHEMA_DESCRIPTION = "DynamicApiHelpRepository-GetSchemaDescription";

        [CacheKey(CacheKeyType.Entity, "controller", "api", "repository")]
        public static string CACHE_KEY_GET_SECONDARY_REPOSITORY = "DynamicApiHelpRepository-GetSecondRepository";

        public static string CACHE_KEY_CLEAR_STATICCACHE_TIME = "DynamicApiRepository-ClearStaticCacheTime";
        public static string CACHE_KEY_CLEAR_STATICCACHE_TIME_DB = "DynamicApiRepository-ClearStaticCacheTime-DB";
        #endregion

        private readonly JPDataHubLogger _log = new JPDataHubLogger(typeof(AbstractDynamicApiRepository));

        private static Lazy<TimeSpan> _cacheExpirationTimeSpan = new Lazy<TimeSpan>(() => UnityCore.Resolve<TimeSpan>("DynamicApiCacheExpirationTimeSpan"));

        /// <summary>
        /// CacheTimeの有効期限
        /// </summary>
        protected static TimeSpan CacheExpirationTimeSpan { get => _cacheExpirationTimeSpan.Value; }

        private static Lazy<bool> _enableResourceVersion = new Lazy<bool>(() => UnityCore.Resolve<bool>("EnableResourceVersion"));

        /// <summary>
        /// リソースのバージョンを使用するかの全体設定
        /// </summary>
        protected static bool EnableResourceVersion => _enableResourceVersion.Value;

        /// <summary>
        /// DB接続用インスタンス
        /// </summary>
        protected IJPDataHubDbConnection DbConnection => UnityCore.Resolve<IJPDataHubDbConnection>("DynamicApi");

        protected virtual ICache Cache => _lazyCache.Value;
        private Lazy<ICache> _lazyCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("DynamicApi"));

        protected static Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AllApiEntity, AllApiEntity>();
                cfg.CreateMap<AllApiEntity, AllApiEntityIdentifier>();
                cfg.CreateMap<AllApiRepositoryIncludePhysicalRepositoryModel, AllApiRepositoryModel>().ReverseMap();
            });
            return mappingConfig.CreateMapper();
        });

        protected static IMapper Mapper { get => _mapper.Value; }

        #region StaticCache
        protected bool IsDynamicApiStaticCache => _isDynamicApiStaticCache.Value;
        protected readonly Lazy<bool> _isDynamicApiStaticCache = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsDynamicApiStaticCache"));

        protected string StaticCacheTimeServer { get => _staticCacheTimeServer.Value; }
        protected Lazy<string> _staticCacheTimeServer = new Lazy<string>(() => UnityCore.Resolve<string>("StaticCacheClusterSyncTimeServer"));
        
        protected int StaticCacheCheckInterval { get => _staticCacheCheckInterval.Value; }
        protected Lazy<int> _staticCacheCheckInterval = new Lazy<int>(() => UnityCore.Resolve<int>("StaticCacheClusterSyncCheckInterval"));

        protected TimeSpan DynamicApiStaticCacheExpirationTimeSpan { get => _dynamicApiStaticCacheExpirationTimeSpan.Value; }
        protected Lazy<TimeSpan> _dynamicApiStaticCacheExpirationTimeSpan = new Lazy<TimeSpan>(() => UnityCore.Resolve<TimeSpan>("DynamicApiStaticCacheExpirationTimeSpan"));
       
        private static StaticCacheControl<List<AllApiRepositoryIncludePhysicalRepositoryModel>> scSecondRepository = new StaticCacheControl<List<AllApiRepositoryIncludePhysicalRepositoryModel>>();
        private static StaticCacheControl<List<AllApiEntity>> scApiSimple = new StaticCacheControl<List<AllApiEntity>>();
        private static StaticCacheControl<List<AllApiEntity>> scApiFull = new StaticCacheControl<List<AllApiEntity>>();
        private static StaticCacheControl<ApiTreeNode> scApiTree = new StaticCacheControl<ApiTreeNode>();
        protected static StaticCacheControl<List<VendorVO>> scVendor = new StaticCacheControl<List<VendorVO>>();

        private static string scLastRefreshTime;
        private static string scExpirationTime;
        private static string scNextCheckTime;
        private static object lockStaticCacheRefresh = new object();
        private static object lockStaticCacheCheck = new object();


        public Task RefreshStaticCache(string time) => RefreshStaticCache(time, true);

        private Task RefreshStaticCache(string time, bool save)
        {
            if (IsDynamicApiStaticCache)
            {
                // StaticCacheをリフレッシュ(待機するかは呼び出し元に委ねる)
                return Task.Run(() =>
                {
                    var acquiredLock = false;
                    try
                    {
                        // ロックが取得できない(=別スレッドが既にリフレッシュ中)なら何もしない
                        Monitor.TryEnter(lockStaticCacheRefresh, 0, ref acquiredLock);
                        if (acquiredLock)
                        {
                            _log.Debug($"RefreshStaticCache start");

                            // リフレッシュ
                            scSecondRepository.Refresh(() => GetRepositoryFromDB<AllApiRepositoryIncludePhysicalRepositoryModel>(null));
                            scApiSimple.Refresh(() => GetApiSimpleListFromDB(true, null));
                            scApiFull.Refresh(() => GetApiFullListFromDB(true, null));
                            scApiTree.Refresh(() => GetApiTree());
                            scVendor.Refresh(() => GetVendorAllFromDB());

                            scLastRefreshTime = time;
                            scExpirationTime = (DateTime.Parse(time) + DynamicApiStaticCacheExpirationTimeSpan).ToString("yyyy/MM/dd HH:mm:ss.fff");
                            _log.Debug($"Static cache refreshed. (Time={scLastRefreshTime} Expiration={scExpirationTime})");

                            // リフレッシュ日時をTimeServerに反映
                            if (save)
                            {
                                Cache.Remove(CACHE_KEY_CLEAR_STATICCACHE_TIME);
                                Cache.Remove(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB);
                                switch (StaticCacheTimeServer)
                                {
                                    case "db":
                                        PutClearStaticCacheToDB(scLastRefreshTime);
                                        break;
                                    case "redis":
                                        PutClearStaticCacheToCache(scLastRefreshTime);
                                        break;
                                }
                            }

                            _log.Debug($"RefreshStaticCache finished");
                        }
                        else
                        {
                            _log.Warn("Static cache refresh are already running in another process.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Unexpected errors occured on RefreshStaticCache.", ex);
                    }
                    finally
                    {
                        if (acquiredLock)
                        {
                            Monitor.Exit(lockStaticCacheRefresh);
                        }
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// これはAPIが呼び出されたときに必ずコールされる
        /// ①期限切れチェック
        /// StaticCacheの有効期限が切れていればリフレッシュを行う。
        /// バックグラウンドで実行されるため完了までは古いキャッシュが使用される。
        /// ②TimeServerチェック
        /// TimeServerの時間が他インスタンスにより更新されていればこのインスタンスのStaticCacheもリフレッシュする。
        /// バックグラウンドで実行されるため完了までは古いキャッシュが使用される。
        /// </summary>
        public void CheckStaticCacheTime()
        {
            try
            {
                if (!IsDynamicApiStaticCache)
                {
                    return;
                }

                var now = DateTime.UtcNow;
                _log.Debug($"CheckStaticCacheTime enter");
                _log.Debug($"TimeServer={StaticCacheTimeServer} NOW={now:yyyy/MM/dd HH:mm:ss.fff} NextCheckTime={scNextCheckTime} ExpirationTime={scExpirationTime}");

                // 期限切れならリフレッシュ
                if (scExpirationTime == null || DateTime.Parse(scExpirationTime) < now)
                {
                    // 待機せずバックグランドで更新
                    _ = RefreshStaticCache(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff"), true);
                    return;
                }

                // 定期チェック予定時刻以前であれば何もしない
                if (scNextCheckTime != null && DateTime.Parse(scNextCheckTime) > now)
                {
                    _log.Debug($"CheckStaticCacheTime exit");
                    return;
                }

                var acquiredLock = false;
                try
                {
                    // ロックが取得できない(=別リクエストが既にチェック中)なら何もしない
                    Monitor.TryEnter(lockStaticCacheCheck, 0, ref acquiredLock);
                    if (acquiredLock)
                    {
                        string tmp = null;
                        switch (StaticCacheTimeServer)
                        {
                            case "db":
                                tmp = GetClearStaticCacheFromDB();
                                break;
                            case "redis":
                                tmp = GetClearStaticCacheFromCache();
                                break;
                        }
                        if (scLastRefreshTime == null || scLastRefreshTime != tmp)
                        {
                            // 待機せずバックグランドで更新
                            _ = RefreshStaticCache(tmp ?? now.ToString("yyyy/MM/dd HH:mm:ss.fff"), tmp != null ? false : true);
                        }
                        scNextCheckTime = now.AddSeconds(StaticCacheCheckInterval).ToString("yyyy/MM/dd HH:mm:ss.fff");
                    }
                    else
                    {
                        _log.Warn("Static cache checks are already running in another process.");
                    }
                }
                finally
                {
                    if (acquiredLock)
                    {
                        Monitor.Exit(lockStaticCacheCheck);
                    }
                }
            }
            finally
            {
                _log.Debug($"CheckStaticCacheTime leave");
            }
        }

        private string GetClearStaticCacheFromCache()
        {
            _log.Debug($"GetClearStaticCacheFromCache");
            return Cache.Get<string>(CACHE_KEY_CLEAR_STATICCACHE_TIME, out bool isNullValue);
        }

        private void PutClearStaticCacheToCache(string time)
        {
            _log.Debug($"PutClearStaticCacheToCache");
            // RedisにStaticCacheを消した時間を持つ場合、その値は全サーバーに伝搬させたいため1万時間保持するようにした（消してほしくない）
            Cache.Add(CACHE_KEY_CLEAR_STATICCACHE_TIME, time, 10000, 0, 0);
        }

        private string GetClearStaticCacheFromDB()
        {
            _log.Debug($"GetClearStaticCacheFromDB");
            // DBを観に行く方。ただし毎回DBを見てしまっては無駄に負荷が高くなってしまうので、n秒間に１回にする
            return GetConst(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB);
        }

        private void PutClearStaticCacheToDB(string time)
        {
            _log.Debug($"PutClearStaticCacheToDB");
            SetConst(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB, time);
            Cache.Add(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB, time, StaticCacheCheckInterval);
        }

        #endregion


        /// <summary>
        /// 全ベンダー情報の取得
        /// </summary>
        /// <returns>全ベンダー情報</returns>
        public List<VendorVO> GetVendorAllFromDB()
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
--GetVendorAll
SELECT
    v.vendor_id
    ,v.vendor_name
    ,v.is_data_offer
    ,v.is_data_use
    ,v.is_enable
FROM
    VENDOR v /*WITH(NOLOCK)*/
WHERE
    v.is_active = 1
";
            }
            else
            {
                sql = @"
--GetVendorAll
SELECT
    v.vendor_id
    ,v.vendor_name
    ,v.is_data_offer
    ,v.is_data_use
    ,v.is_enable
FROM
    Vendor v WITH(NOLOCK)
WHERE
    v.is_active = 1
";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            return DbConnection.Query<DB_Vendor>(twowaySql.Sql).ToList().Select(x => new VendorVO(x.vendor_id, x.vendor_name, x.is_data_offer, x.is_data_use, x.is_enable)).ToList();
        }


        protected List<AllApiEntity> GetAllApiWithVersionApi() => Cache.Get<List<AllApiEntity>>(CACHE_KEY_IPFILTER_UNION_NEWVERSION_CONTROLLER, CacheExpirationTimeSpan, () =>
        {
            var apiList = GetAllApi();
            var enableControllerApiList = apiList.Where(x => x.is_enable_controller == true);
            var enableRepositoryGroupList = enableControllerApiList.ToList();
            var addTransparentApiList = AddTransparentApi(enableRepositoryGroupList);
            return addTransparentApiList.Where(x => x.is_enable_api == true).ToList();
        });

        protected List<AllApiEntity> GetAllApiWithVersionApiSimple()
        {
            var apiList = GetApiSimpleList(true);
            var enableRepositoryGroupList = apiList.ToList();
            var addTransparentApiList = AddTransparentApi(enableRepositoryGroupList).ToList();
            return addTransparentApiList.ToList();
        }

        private List<AllApiEntity> GetAllApi(bool isActiveOnly = true)
        {
            var result = Cache.Get<List<AllApiEntity>>(GetIsActiveOnlyKey(isActiveOnly), CacheExpirationTimeSpan, () =>
            {
                var allApiEntitiyList = GetApiFullList(isActiveOnly);
                var allRepositoryList = GetRepositoryWithoutPhysical();
                allApiEntitiyList.ForEach(allApiEntity =>
                {
                    allApiEntity.all_repository_model_list = GetAllRepositoryModelList(allApiEntity, allRepositoryList);
                });

                return allApiEntitiyList;
            });

            return result;
        }

        protected List<AllApiEntity> GetControllerApiWithVersionApi(Guid controllerId)
        {
            var resourceApiCache = PerRequestDataContainer.ResourceApiCache as ConcurrentDictionary<Guid, object>;
            if (resourceApiCache == null)
            {
                lock (PerRequestDataContainer) // Reference, Notify非同期処理の際にtaskが衝突する可能性があるためlock
                {
                    PerRequestDataContainer.ResourceApiCache = resourceApiCache = new ConcurrentDictionary<Guid, object>();
                }
            }
            List<AllApiEntity> hit = null;
            if (resourceApiCache.Keys.Contains(controllerId) == true)
            {
                hit = resourceApiCache[controllerId] as List<AllApiEntity>;
            }
            if (hit == null)
            {
                Func<List<AllApiEntity>> func = new Func<List<AllApiEntity>>(() =>
                {
                    var apiList = GetControllerApi(controllerId);
                    var enableControllerApiList = apiList.Where(x => x.is_enable_controller == true);
                    //var enableRepositoryGroupList = this.InvalidRepositoryGroupExclude(enableControllerApiList);
                    var enableRepositoryGroupList = enableControllerApiList.ToList();
                    var addTransparentApiList = AddTransparentApi(enableRepositoryGroupList).Where(x => x.is_enable_api == true).ToList();
                    var attachfileApi = addTransparentApiList.Where(x => x.action_type_cd == ActionType.AttachFileUpload.ToCode()).FirstOrDefault();
                    if (attachfileApi != null)
                    {
                        var attachfileRepository = attachfileApi.all_repository_model_list
                        .Where(x => x.repository_type_cd == RepositoryType.AttachFileBlob.ToCode()).FirstOrDefault();
                        addTransparentApiList.ForEach(x => x.attachfile_blob_repository_model = attachfileRepository);
                    }
                    return addTransparentApiList;
                });
                if (IsDynamicApiStaticCache == true)
                {
                    hit = func();
                }
                else
                {
                    hit = Cache.Get<List<AllApiEntity>>(CacheManager.CreateKey(CACHE_KEY_ALLAPIENTITY_CONTROLLER_LIST, controllerId), CacheExpirationTimeSpan, () => func());
                }
                resourceApiCache.TryAdd(controllerId, hit);
            }
            return hit;
        }

        private List<AllApiRepositoryModel> GetAllRepositoryModelList(AllApiEntity allApiEntity, IEnumerable<AllApiRepositoryModel> allRepositoryList)
        {
            List<AllApiRepositoryModel> result = new List<AllApiRepositoryModel>();
            //対象APIのもののみ取得
            var apiAllRepositoryList = allRepositoryList.Where(r => allApiEntity.api_id == r.api_id).ToList();

            //セカンダリのis_primary=trueがあれば、is_primary=trueのリポジトリ、apiEntityのリポジトリ、その他の順で詰める
            //セカンダリのプライマリ
            var first = apiAllRepositoryList.Where(x => !x.is_primary).Where(x => x.is_secondary_primary).FirstOrDefault();
            if (first != null)
            {
                result.Add(first);
            }
            //セカンダリのプライマリ
            var second = apiAllRepositoryList.Where(x => x.is_primary).FirstOrDefault();
            if (second != null)
            {
                result.Add(second);
            }
            //セカンダリのセカンダリ
            var third = apiAllRepositoryList.Where(x => !x.is_primary).Where(x => !x.is_primary);
            if (third.Any())
            {
                result.AddRange(third.ToList());
            }
            // 履歴用
            if (allApiEntity.is_document_history == true && apiAllRepositoryList?.Count != 0)
            {
                allApiEntity.history_repository_model = apiAllRepositoryList.Where(x => x.is_primary == false && x.api_id == allApiEntity.api_id && x.repository_type_cd == "dhs").FirstOrDefault();
            }

            return result;
        }

        protected List<AllApiEntity> GetControllerApi(Guid controllerId)
        {
            var allApiEntitiyList = GetApiFullList(true, controllerId);
            var allApiRepositoryList = GetAllApiRepositoryModel(controllerId);

            allApiEntitiyList.ForEach(allApiEntity =>
            {
                allApiEntity.all_repository_model_list = GetAllRepositoryModelList(allApiEntity, allApiRepositoryList);
            });
            return allApiEntitiyList;
        }

        private AllApiEntity FindApiEntityByIdentifier(
            AllApiEntityIdentifier identifier,
            HttpMethodType httpMethodType,
            RequestRelativeUri requestRelativeUri,
            GetQuery getQuery = null,
            List<Guid> exclusiveApiId = null)
        {
            // APIで定義してあるものを取得(HttpMethodTypeが合致するものだけど。URLがヒットするのはSQLでは難しいため）
            List<AllApiEntity> api_list = GetControllerApiWithVersionApi(identifier.controller_id);

            // APIリストから除外リストを作成
            if (exclusiveApiId != null)
            {
                api_list = api_list.Where(x => exclusiveApiId.Contains(x.api_id) == false).ToList();
            }

            // APIのマッチングを探す
            string normalizedRelativeUri = requestRelativeUri.Value.NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);
            foreach (var api in api_list)
            {
                if (api.IsMatch(httpMethodType, normalizedRelativeUri, getQuery?.Value, splitUrl) == true)
                {
                    return api;
                }
            }

            // 指定したURLがController相当と解釈した場合、デフォルトのメソッドとして処理が可能か？
            // デフォルトのメソッドとは "Default" とする
            normalizedRelativeUri = requestRelativeUri.Value.NormalizeUrlRelative() + "/" + "Default";
            splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);
            foreach (var api in api_list)
            {
                if (api.IsMatch(httpMethodType, normalizedRelativeUri, getQuery?.Value, splitUrl) == true)
                {
                    return api;
                }
            }

            return null;
        }

        protected AllApiEntity FindApiEntityByUrlFast(
            HttpMethodType httpMethodType,
            RequestRelativeUri requestRelativeUri,
            GetQuery getQuery = null,
            List<Guid> exclusiveApiId = null)
        {
            ApiTreeNode root = PerRequestDataContainer.ApiTreeCache as ApiTreeNode;
            if (root == null)
            {
                lock (PerRequestDataContainer) // Reference, Notify非同期処理の際にtaskが衝突する可能性があるためlock
                {
                    // APIで定義してあるものを取得(HttpMethodTypeが合致するものだけど。URLがヒットするのはSQLでは難しいため）
                    if (IsDynamicApiStaticCache == true && scApiTree.TryGet(out var staticCache))
                    {
                        PerRequestDataContainer.ApiTreeCache = root = staticCache;
                    }
                    else
                    {
                        PerRequestDataContainer.ApiTreeCache = root = Cache.Get<ApiTreeNode>(CACHE_KEY_ALLAPIIDENTIFIER_LIST, CacheExpirationTimeSpan, () => GetApiTree());
                    }
                }
            }

            // APIリストから除外リストを作成
            if (exclusiveApiId != null)
            {
                exclusiveApiId.ForEach(x => root.Remove(x));
            }

            // 通常の呼び出し
            string normalizedRelativeUri = requestRelativeUri.Value.NormalizeUrlRelative();
            string[] splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);
            var identifier = root.FindApiIdentifier(normalizedRelativeUri, splitUrl, httpMethodType, getQuery);
            if (identifier != null)
            {
                return FindApiEntityByIdentifier(identifier, httpMethodType, requestRelativeUri, getQuery, exclusiveApiId);
            }
            // 指定したURLがController相当と解釈した場合、デフォルトのメソッドとして処理が可能か？
            // デフォルトのメソッドとは "Default" とする
            normalizedRelativeUri = requestRelativeUri.Value.NormalizeUrlRelative() + "/" + "Default";
            splitUrl = UriUtil.SplitRelativeUrl(normalizedRelativeUri);
            identifier = root.FindApiIdentifier(normalizedRelativeUri, splitUrl, httpMethodType, getQuery);
            if (identifier != null)
            {
                return FindApiEntityByIdentifier(identifier, httpMethodType, requestRelativeUri, getQuery, exclusiveApiId);
            }

            return null;
        }

        private List<AllApiRepositoryModel> GetRepositoryWithoutPhysical()
        {
            return Mapper.Map<List<AllApiRepositoryModel>>(GetRepository());
        }

        private List<AllApiRepositoryIncludePhysicalRepositoryModel> GetRepository()
        {
            if (IsDynamicApiStaticCache == true && scSecondRepository.TryGet(out var staticCache))
            {
                return staticCache;
            }
            else
            {
                return GetRepositoryFromChache();
            }
        }

        private List<AllApiRepositoryIncludePhysicalRepositoryModel> GetRepositoryByControllerId(Guid controllerId)
        {
            if (IsDynamicApiStaticCache == true)
            {
                return GetRepository()?.Where(x => x.controller_id == controllerId).ToList();
            }
            else
            {
                return GetRepositoryFromDB<AllApiRepositoryIncludePhysicalRepositoryModel>(controllerId);
            }
        }

        private List<AllApiRepositoryIncludePhysicalRepositoryModel> GetRepositoryFromChache()
        {
            return Cache.Get<List<AllApiRepositoryIncludePhysicalRepositoryModel>>(CACHE_KEY_GET_SECONDARY_REPOSITORY, CacheExpirationTimeSpan, () => GetRepositoryFromDB<AllApiRepositoryIncludePhysicalRepositoryModel>(null));
        }

        private List<T> GetRepositoryFromDB<T>(Guid? controllerId)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql_base = "";
            var sql_controller_id = "";
            if (dbSettings.Type == "Oracle")
            {
                sql_base = @"
--GetRepository
SELECT
    a.api_id               AS api_id
    ,a.repository_group_id AS repository_group_id
    ,pr.connection_string  AS repository_connection_string
    ,rg.repository_type_cd AS repository_type_cd
    ,pr.is_full            AS is_full
    ,1                     AS is_primary
    ,0                     AS is_secondary_primary
    ,pr.physical_repository_id AS physical_repository_id
    ,c.controller_id     AS controller_id
FROM
    controller c
    INNER JOIN api a ON c.controller_id=a.controller_id AND a.is_active=1
    INNER JOIN repository_group rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1
    INNER JOIN physical_repository pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
WHERE
    c.is_active=1
    {0}
UNION ALL
SELECT
    a.api_id               AS api_id
    ,secrg.repository_group_id AS repository_group_id
    ,pr.connection_string  AS repository_connection_string
    ,rg.repository_type_cd AS repository_type_cd
    ,pr.is_full            AS is_full
    ,0                     AS is_primary
    ,secrg.is_primary      AS is_secondary_primary
    ,pr.physical_repository_id AS physical_repository_id
    ,c.controller_id     AS controller_id
FROM
    controller c
    INNER JOIN api a ON c.controller_id=a.controller_id AND a.is_active=1
    INNER JOIN secondary_repository_map secrg ON a.api_id=secrg.api_id AND secrg.is_active=1
    INNER JOIN repository_group rg ON secrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
    INNER JOIN physical_repository pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
WHERE
    c.is_active=1
    {0}
";
                sql_controller_id = @"AND c.controller_id= /*ds controller_id*/'1' ";
            }
            else
            {
                sql_base = @"
--GetRepository
SELECT
    a.api_id               AS api_id
    ,a.repository_group_id AS repository_group_id
    ,pr.connection_string  AS repository_connection_string
    ,rg.repository_type_cd AS repository_type_cd
    ,pr.is_full            AS is_full
    ,1                     AS is_primary
    ,0                     AS is_secondary_primary
    ,pr.physical_repository_id AS physical_repository_id
    ,c.controller_id     AS controller_id
FROM
    Controller c
    INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active=1
    INNER JOIN RepositoryGroup rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1
    INNER JOIN PhysicalRepository pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
WHERE
    c.is_active=1
    {0}
UNION ALL
SELECT
    a.api_id               AS api_id
    ,secrg.repository_group_id AS repository_group_id
    ,pr.connection_string  AS repository_connection_string
    ,rg.repository_type_cd AS repository_type_cd
    ,pr.is_full            AS is_full
    ,0                     AS is_primary
    ,secrg.is_primary      AS is_secondary_primary
    ,pr.physical_repository_id AS physical_repository_id
    ,c.controller_id     AS controller_id
FROM
    Controller c
    INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active=1
    INNER JOIN SecondaryRepositoryMap secrg ON a.api_id=secrg.api_id AND secrg.is_active=1
    INNER JOIN RepositoryGroup rg ON secrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
    INNER JOIN PhysicalRepository pr ON rg.repository_group_id=pr.repository_group_id AND pr.is_active=1
WHERE
    c.is_active=1
    {0}
";
                sql_controller_id = @"AND c.controller_id=@controller_id";
            }
            //リポジトリグループ、セカンダリリポジトリグループのPhysicalRepositoryを軸に取得する
            string sql = string.Format(sql_base, controllerId == null ? "" : sql_controller_id);

            var param = new { controller_id = controllerId };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            if (controllerId == null)
                return DbConnection.Query<T>(twowaySql.Sql).ToList();
            else
                return DbConnection.Query<T>(twowaySql.Sql, dynParams).ToList();
        }

        private List<AllApiEntity> GetApiFullList(bool isActiveOnly, Guid? controllerId = null)
        {
            if (IsDynamicApiStaticCache == true && scApiFull.TryGet(out var list))
            {
                return controllerId == null ? list : list.Where(x => x.controller_id == controllerId.Value).ToList();
            }
            else
            {
                return GetApiFullListFromDB(isActiveOnly, controllerId);
            }
        }

        private List<AllApiEntity> GetApiFullListFromDB(bool isActiveOnly, Guid? controllerId = null)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = $@"
--GetApiFullList!
SELECT
	*
FROM
	(
SELECT
         c.vendor_id              AS vendor_id
        ,c.system_id              AS system_id
        ,c.controller_id          AS controller_id
        ,c.controller_description AS controller_description
        ,c.url                    AS controller_relative_url
        ,c.is_vendor              AS is_vendor
        ,c.is_person              AS is_person
        ,c.is_enable              AS is_enable_controller
        ,c.controller_repository_key as controller_repository_key
        ,c.controller_schema_id   AS controller_schema_id
        ,c.controller_partition_key AS partition_key
        ,c.is_visible_agreement AS is_visible_agreement
        ,c.is_toppage           AS is_toppage
        ,c.reg_username AS controller_reg_username
        ,c.upd_username AS controller_upd_username
        ,c.reg_date     AS controller_reg_date
        ,c.upd_date     AS controller_upd_date
        ,c.is_active    AS controller_is_active
        ,c.is_enable_attachfile    AS is_enable_attachfile
        ,c.is_use_blob_cache       AS is_use_blob_cache
        ,c.is_optimistic_concurrency   AS is_optimistic_concurrency
        ,c.is_enable_blockchain AS is_enable_blockchain
        ,c.is_document_history AS is_document_history
        ,c.is_container_dynamic_separation AS is_container_dynamic_separation
        ,c.is_enable_resource_version AS is_enable_resource_version
        ,a.api_id                      AS api_id
        ,a.api_description             AS api_description
        ,a.url                         AS method_name
        ,a.method_type                 AS method_type
        ,a.is_admin_authentication     AS is_admin_authentication
        ,a.is_header_authentication    AS is_header_authentication
        ,a.is_vendor_system_authentication_allow_null    AS is_vendor_system_authentication_allow_null
        ,a.is_openid_authentication    AS is_openid_authentication
        ,a.post_data_type              AS post_data_type
        ,a.query                       AS query
        ,a.query_type_cd              AS query_type_cd
        ,a.is_enable                   AS is_enable_api
        ,a.is_hidden                    AS is_hidden
        ,a.gateway_url                 AS gateway_url
        ,a.gateway_credential_username AS gateway_credential_username
        ,a.gateway_credential_password AS gateway_credential_password
        ,a.is_over_partition           AS is_over_partition
        ,a.repository_group_id         AS repository_group_id
        ,a.script                      AS script
        ,a.action_type_cd              AS action_type_cd
        ,a.script_type_cd              AS script_type_cd
        ,a.actiontype_version
        ,a.reg_username                AS api_reg_username
        ,a.upd_username                AS api_upd_username
        ,a.reg_date                    AS api_reg_date
        ,a.upd_date                    AS api_upd_date
        ,a.is_active                   AS api_is_active
        ,a.is_transparent_api          AS is_transparent_api
        ,a.is_skip_jsonschema_validation AS is_skip_jsonschema_validation
        ,a.is_openid_authentication_allow_null AS is_openid_authentication_allow_null
        ,rg.repository_type_cd AS repository_type_cd
        ,rg.is_enable AS is_enable_repository
        ,a.is_cache         AS is_cache
        ,a.cache_minute AS cache_minute
        ,a.cache_key AS cache_key
        ,a.is_accesskey AS is_accesskey
        ,a.is_automatic_id AS is_automatic_id
        ,a.gateway_relay_header AS gateway_relay_header
        ,a.is_internal_call_only       AS is_internal_call_only
        ,a.internal_call_keyword       AS internal_call_keyword
        ,a.is_clientcert_authentication AS is_clientcert_authentication
        ,a.is_otherresource_sqlaccess   AS is_otherresource_sqlaccess
        ,dsreq.data_schema_id AS request_schema_id
        ,dsreq.data_schema    AS request_schema
        ,dsreq.schema_name    AS request_schema_name
        ,dsreq.vendor_id AS request_vendor_id
        ,dsreq.reg_date AS request_reg_date
        ,dsreq.upd_date AS request_upd_date
        ,dsreq.is_active AS request_is_active
        ,dsreq.schema_description AS request_schema_description
        ,dsres.data_schema_id AS response_schema_id
        ,dsres.data_schema    AS response_schema
        ,dsres.schema_name    AS response_schema_name
        ,dsres.vendor_id AS response_vendor_id
        ,dsres.reg_date AS response_reg_date
        ,dsres.upd_date AS response_upd_date
        ,dsres.is_active AS response_is_active
        ,dsres.schema_description AS response_schema_description
        ,dsurl.data_schema_id AS url_schema_id
        ,dsurl.data_schema    AS url_schema
        ,dsurl.schema_name    AS url_schema_name
        ,dsurl.vendor_id AS url_vendor_id
        ,dsurl.reg_date AS url_reg_date
        ,dsurl.upd_date AS url_upd_date
        ,dsurl.is_active AS url_is_active
        ,dsurl.schema_description AS url_schema_description
        ,dscontroller.data_schema    AS controller_schema
        ,dscontroller.reg_date AS controller_schema_reg_date
        ,dscontroller.upd_date AS controller_schema_upd_date
        ,dscontroller.is_active AS controller_schema_is_active
        ,dscontroller.schema_description AS controller_schema_description
        ,at.action_type_name
	    ,TO_CHAR(cp.public_start_datetime,'yyyy/MM/dd HH:mm:ss') AS public_start_datetime
	    ,TO_CHAR(cp.public_end_datetime,'yyyy/MM/dd HH:mm:ss') AS public_end_datetime
        ,(
            SELECT
                COUNT(*)
            FROM
                secondary_repository_map srm
                INNER JOIN repository_group rg ON rg.repository_group_id=srm.repository_group_id AND rg.is_active=1
            WHERE
                srm.api_id=a.api_id
                AND srm.is_active=1
                AND rg.is_enable=0
        ) AS secondary_repository_disable_count
		,rsg.is_require_consent AS is_require_consent
		,rsg.resource_group_id AS resource_group_id
		,tg.terms_group_code AS terms_group_code
        FROM
            CONTROLLER c
            INNER JOIN API a ON c.controller_id=a.controller_id AND a.is_active IN /*ds isActiveList*/1 
            INNER JOIN ACTION_TYPE at ON a.action_type_cd=at.action_type_cd AND at.is_active=1
	        INNER JOIN VENDOR vender ON c.vendor_id=vender.vendor_id AND vender.is_active  IN /*ds isActiveList*/1  AND vender.is_enable IN /*ds isActiveList*/1 
	        INNER JOIN SYSTEM system ON c.system_id=system.system_id AND system.is_active  IN /*ds isActiveList*/1  AND system.is_enable IN /*ds isActiveList*/1 
            LEFT OUTER JOIN REPOSITORY_GROUP rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1 AND rg.is_enable = 1
            LEFT OUTER JOIN DATA_SCHEMA dsreq ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active IN /*ds isActiveList*/1 
            LEFT OUTER JOIN DATA_SCHEMA dsres ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active IN /*ds isActiveList*/1 
            LEFT OUTER JOIN DATA_SCHEMA dsurl ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active IN /*ds isActiveList*/1 
            LEFT OUTER JOIN DATA_SCHEMA dscontroller ON c.controller_schema_id=dscontroller.data_schema_id AND dscontroller.is_active IN /*ds isActiveList*/1 
            LEFT OUTER JOIN CONTROLLER_PRICIES cp ON c.controller_id=cp.controller_id AND cp.is_active IN /*ds isActiveList*/1 
			LEFT OUTER JOIN CONTROLLER_RESOURCE_GROUP crg ON c.controller_id=crg.controller_id AND crg.is_active = 1
			LEFT OUTER JOIN RESOURCE_GROUP rsg ON crg.resource_group_id=rsg.resource_group_id AND rsg.is_active = 1
			LEFT OUTER JOIN TERMS_GROUP tg ON rsg.terms_group_code=tg.terms_group_code AND tg.is_active = 1
        WHERE
/*ds if ControllerId != null*/
            c.controller_id = /*ds ControllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
            c.is_active = /*ds isActiveList*/1 AND
           (a.action_type_cd = 'gtw'  OR is_transparent_api = 1 OR (a.action_type_cd <> 'gtw' AND rg.repository_group_id IS NOT NULL ))
    ) x
WHERE
	x.secondary_repository_disable_count=0
ORDER BY
    x.controller_relative_url
";
            }
            else
            {
                sql = $@"
--GetApiFullList!
SELECT
	*
FROM
	(
SELECT
         c.vendor_id              AS vendor_id
        ,c.system_id              AS system_id
        ,c.controller_id          AS controller_id
        ,c.controller_description AS controller_description
        ,c.url                    AS controller_relative_url
        ,c.is_vendor              AS is_vendor
        ,c.is_person              AS is_person
        ,c.is_enable              AS is_enable_controller
        ,c.controller_repository_key as controller_repository_key
        ,c.controller_schema_id   AS controller_schema_id
        ,c.controller_partition_key AS partition_key
        ,c.is_visible_agreement AS is_visible_agreement
        ,c.is_toppage           AS is_toppage
        ,c.reg_username AS controller_reg_username
        ,c.upd_username AS controller_upd_username
        ,c.reg_date     AS controller_reg_date
        ,c.upd_date     AS controller_upd_date
        ,c.is_active    AS controller_is_active
        ,c.is_enable_attachfile    AS is_enable_attachfile
        ,c.is_use_blob_cache       AS is_use_blob_cache
        ,c.is_optimistic_concurrency   AS is_optimistic_concurrency
        ,c.is_enable_blockchain AS is_enable_blockchain
        ,c.is_document_history AS is_document_history
        ,c.is_container_dynamic_separation AS is_container_dynamic_separation
        ,c.is_enable_resource_version AS is_enable_resource_version
        ,a.api_id                      AS api_id
        ,a.api_description             AS api_description
        ,a.url                         AS method_name
        ,a.method_type                 AS method_type
        ,a.is_admin_authentication     AS is_admin_authentication
        ,a.is_header_authentication    AS is_header_authentication
        ,a.is_vendor_system_authentication_allow_null    AS is_vendor_system_authentication_allow_null
        ,a.is_openid_authentication    AS is_openid_authentication
        ,a.post_data_type              AS post_data_type
        ,a.query                       AS query
        ,a.query_type_cd              AS query_type_cd
        ,a.is_enable                   AS is_enable_api
        ,a.is_hidden                    AS is_hidden
        ,a.gateway_url                 AS gateway_url
        ,a.gateway_credential_username AS gateway_credential_username
        ,a.gateway_credential_password AS gateway_credential_password
        ,a.is_over_partition           AS is_over_partition
        ,a.repository_group_id         AS repository_group_id
        ,a.script                      AS script
        ,a.action_type_cd              AS action_type_cd
        ,a.script_type_cd              AS script_type_cd
        ,a.actiontype_version
        ,a.reg_username                AS api_reg_username
        ,a.upd_username                AS api_upd_username
        ,a.reg_date                    AS api_reg_date
        ,a.upd_date                    AS api_upd_date
        ,a.is_active                   AS api_is_active
        ,a.is_transparent_api          AS is_transparent_api
        ,a.is_skip_jsonschema_validation AS is_skip_jsonschema_validation
        ,a.is_openid_authentication_allow_null AS is_openid_authentication_allow_null
        ,rg.repository_type_cd AS repository_type_cd
        ,rg.is_enable AS is_enable_repository
        ,a.is_cache         AS is_cache
        ,a.cache_minute AS cache_minute
        ,a.cache_key AS cache_key
        ,a.is_accesskey AS is_accesskey
        ,a.is_automatic_id AS is_automatic_id
        ,a.gateway_relay_header AS gateway_relay_header
        ,a.is_internal_call_only       AS is_internal_call_only
        ,a.internal_call_keyword       AS internal_call_keyword
        ,a.is_clientcert_authentication AS is_clientcert_authentication
        ,a.is_otherresource_sqlaccess   AS is_otherresource_sqlaccess
        ,dsreq.data_schema_id AS request_schema_id
        ,dsreq.data_schema    AS request_schema
        ,dsreq.schema_name    AS request_schema_name
        ,dsreq.vendor_id AS request_vendor_id
        ,dsreq.reg_date AS request_reg_date
        ,dsreq.upd_date AS request_upd_date
        ,dsreq.is_active AS request_is_active
        ,dsreq.schema_description AS request_schema_description
        ,dsres.data_schema_id AS response_schema_id
        ,dsres.data_schema    AS response_schema
        ,dsres.schema_name    AS response_schema_name
        ,dsres.vendor_id AS response_vendor_id
        ,dsres.reg_date AS response_reg_date
        ,dsres.upd_date AS response_upd_date
        ,dsres.is_active AS response_is_active
        ,dsres.schema_description AS response_schema_description
        ,dsurl.data_schema_id AS url_schema_id
        ,dsurl.data_schema    AS url_schema
        ,dsurl.schema_name    AS url_schema_name
        ,dsurl.vendor_id AS url_vendor_id
        ,dsurl.reg_date AS url_reg_date
        ,dsurl.upd_date AS url_upd_date
        ,dsurl.is_active AS url_is_active
        ,dsurl.schema_description AS url_schema_description
        ,dscontroller.data_schema    AS controller_schema
        ,dscontroller.reg_date AS controller_schema_reg_date
        ,dscontroller.upd_date AS controller_schema_upd_date
        ,dscontroller.is_active AS controller_schema_is_active
        ,dscontroller.schema_description AS controller_schema_description
        ,at.action_type_name
	    ,FORMAT(cp.public_start_datetime,'yyyy/MM/dd HH:mm:ss') AS public_start_datetime
	    ,FORMAT(cp.public_end_datetime,'yyyy/MM/dd HH:mm:ss') AS public_end_datetime
        ,(
            SELECT
                COUNT(*)
            FROM
                SecondaryRepositoryMap srm
                INNER JOIN RepositoryGroup rg ON rg.repository_group_id=srm.repository_group_id AND rg.is_active=1
            WHERE
                srm.api_id=a.api_id
                AND srm.is_active=1
                AND rg.is_enable=0
        ) AS secondary_repository_disable_count
		,rsg.is_require_consent AS is_require_consent
		,rsg.resource_group_id AS resource_group_id
		,tg.terms_group_code AS terms_group_code
        FROM
            Controller c
            INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active IN @isActiveList
            INNER JOIN ActionType at ON a.action_type_cd=at.action_type_cd AND at.is_active=1
	        INNER JOIN Vendor vender ON c.vendor_id=vender.vendor_id AND vender.is_active  IN @isActiveList AND vender.is_enable IN @isActiveList
	        INNER JOIN System system ON c.system_id=system.system_id AND system.is_active  IN @isActiveList AND system.is_enable IN @isActiveList
            LEFT OUTER JOIN RepositoryGroup rg ON a.repository_group_id=rg.repository_group_id AND rg.is_active=1 AND rg.is_enable = 1
            LEFT OUTER JOIN DataSchema dsreq ON a.request_schema_id=dsreq.data_schema_id AND dsreq.is_active IN @isActiveList
            LEFT OUTER JOIN DataSchema dsres ON a.response_schema_id=dsres.data_schema_id AND dsres.is_active IN @isActiveList
            LEFT OUTER JOIN DataSchema dsurl ON a.url_schema_id=dsurl.data_schema_id AND dsurl.is_active IN @isActiveList
            LEFT OUTER JOIN DataSchema dscontroller ON c.controller_schema_id=dscontroller.data_schema_id AND dscontroller.is_active IN @isActiveList
            LEFT OUTER JOIN ControllerPricies cp ON c.controller_id=cp.controller_id AND cp.is_active IN @isActiveList
			LEFT OUTER JOIN ControllerResourceGroup crg ON c.controller_id=crg.controller_id AND crg.is_active = 1
			LEFT OUTER JOIN ResourceGroup rsg ON crg.resource_group_id=rsg.resource_group_id AND rsg.is_active = 1
			LEFT OUTER JOIN TermsGroup tg ON rsg.terms_group_code=tg.terms_group_code AND tg.is_active = 1
        WHERE
/*ds if ControllerId != null*/
            c.controller_id = /*ds ControllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
            c.is_active IN @isActiveList AND
           (a.action_type_cd = 'gtw'  OR is_transparent_api = 1 OR (a.action_type_cd <> 'gtw' AND rg.repository_group_id IS NOT NULL ))
    ) x
WHERE
	x.secondary_repository_disable_count=0
ORDER BY
    x.controller_relative_url
";
            }

            bool? isActiveListForOracle = true;
            var isActiveListForSQLServer = new List<bool>() { true };
            if (!isActiveOnly)
            {
                isActiveListForOracle = false;
                isActiveListForSQLServer.Add(false);

            }
            var dict = new Dictionary<string, object>()
            {
                { "ControllerId", controllerId },
                //{ "EnableResourceVersion", EnableResourceVersion },
                { "isActiveList", dbSettings.Type == "Oracle" ? isActiveListForOracle : isActiveListForSQLServer }
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(dict);
            return DbConnection.Query<AllApiEntity>(twowaySql.Sql, dynParams).ToList();
        }

        private List<AllApiEntity> GetApiSimpleList(bool isActiveOnly, Guid? controllerId = null)
        {
            if (IsDynamicApiStaticCache == true && scApiSimple.TryGet(out var staticCache))
            {
                return staticCache;
            }
            else
            {
                //呼び出し元でキャッシュしているためここでキャッシュする必要はない。
                return GetApiSimpleListFromDB(isActiveOnly, controllerId);
                //return GetApiSimpleListFromCache(isActiveOnly, controllerId);
            }
        }

        private List<AllApiEntity> GetApiSimpleListFromCache(bool isActiveOnly, Guid? controllerId = null)
        {
            return Cache.Get<List<AllApiEntity>>(isActiveOnly == true && controllerId == null ? CACHE_KEY_APISIMPLE_LIST_ALL : CacheManager.CreateKey(CACHE_KEY_APISIMPLE_LIST_ALL, isActiveOnly, controllerId), CacheExpirationTimeSpan, () => GetApiSimpleListFromDB(isActiveOnly, controllerId));
        }

        private List<AllApiEntity> GetApiSimpleListFromDB(bool isActiveOnly, Guid? controllerId = null)
        {
            //関連INDEX
            //SQLの修正をする際は関連INDEXの確認も行うこと
            //IDX_API_GetVendorApi2
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = $@"
--GetApiSimple
SELECT
	*
FROM
	(
		SELECT
			c.controller_id
			,c.is_enable AS is_enable_controller
			,c.url AS controller_relative_url
			,c.is_enable_attachfile
			,c.is_enable_blockchain
			,c.is_document_history
			,c.is_enable_resource_version
			,a.api_id
			,a.is_enable AS is_enable_api
			,a.url AS method_name
			,a.method_type AS method_type
			,a.query_type_cd AS query_type_cd
			,a.is_transparent_api
            ,a.action_type_cd
			,rg.repository_group_id
			,(
				SELECT
					COUNT(*)
				FROM
					secondary_repository_map srm
					INNER JOIN repository_group rg ON rg.repository_group_id=srm.repository_group_id AND rg.is_active=1
				WHERE
					srm.api_id=a.api_id
					AND srm.is_active=1
					AND rg.is_enable=0
			) secondary_repository_disable_count
		FROM
			controller c
			INNER JOIN api a ON c.controller_id=a.controller_id AND a.is_active IN /*ds isActiveList*/1 
            LEFT OUTER JOIN repository_group rg ON a.repository_group_id = rg.repository_group_id AND rg.is_active = 1 AND rg.is_enable = 1
		WHERE
/*ds if ControllerId != null*/
            c.controller_id = /*ds ControllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
/*ds if isActiveOnly == true*/
			c.is_enable=1 AND
			a.is_enable=1 AND
/*ds end if*/
			c.is_active = /*ds isActiveList*/1 AND
           (a.action_type_cd = 'gtw' OR is_transparent_api = 1 OR (a.action_type_cd <> 'gtw' AND rg.repository_group_id IS NOT NULL ))
	) a
WHERE
	a.secondary_repository_disable_count=0
ORDER BY controller_relative_url
";
            }
            else
            {
                sql = $@"
--GetApiSimple
SELECT
	*
FROM
	(
		SELECT
			c.controller_id
			,c.is_enable AS is_enable_controller
			,c.url AS controller_relative_url
			,c.is_enable_attachfile
			,c.is_enable_blockchain
			,c.is_document_history
			,c.is_enable_resource_version
			,a.api_id
			,a.is_enable AS is_enable_api
			,a.url AS method_name
			,a.method_type AS method_type
			,a.query_type_cd AS query_type_cd
			,a.is_transparent_api
            ,a.action_type_cd
			,rg.repository_group_id
			,(
				SELECT
					COUNT(*)
				FROM
					SECONDARY_REPOSITORY_MAP srm
					INNER JOIN REPOSITORY_GROUP rg ON rg.repository_group_id=srm.repository_group_id AND rg.is_active=1
				WHERE
					srm.api_id=a.api_id
					AND srm.is_active=1
					AND rg.is_enable=0
			) AS secondary_repository_disable_count
		FROM
			CONTROLLER c
			INNER JOIN API a ON c.controller_id=a.controller_id AND a.is_active IN @isActiveList
            LEFT OUTER JOIN REPOSITORY_GROUP rg ON a.repository_group_id = rg.repository_group_id AND rg.is_active = 1 AND rg.is_enable = 1
		WHERE
/*ds if ControllerId != null*/
            c.controller_id = /*ds ControllerId*/'00000000-0000-0000-0000-000000000000' AND
/*ds end if*/
			c.is_enable=1 AND
			a.is_enable=1 AND
			c.is_active IN @isActiveList AND
           (a.action_type_cd = 'gtw' OR is_transparent_api = 1 OR (a.action_type_cd <> 'gtw' AND rg.repository_group_id IS NOT NULL ))
	) a
WHERE
	a.secondary_repository_disable_count=0
ORDER BY controller_relative_url
";
            }

            bool? isActiveListForOracle = true;
            var isActiveListForSQLServer = new List<bool>() { true };
            if (!isActiveOnly)
            {
                isActiveListForOracle = false;
                isActiveListForSQLServer.Add(false);
            }
            var dict = new Dictionary<string, object>()
            {
                { "ControllerId", controllerId },
                { "isActiveList", dbSettings.Type == "Oracle" ? isActiveListForOracle : isActiveListForSQLServer },
                { "isActiveOnly", isActiveOnly }
            };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(dict);
            return DbConnection.Query<AllApiEntity>(twowaySql.Sql, dynParams).ToList();
        }


        protected IEnumerable<AllApiRepositoryModel> GetAllApiRepositoryModel(Guid controllerId)
        {
            var allRepositoryList = GetRepositoryByControllerId(controllerId);
            List<AllApiRepositoryModel> allApiRepositoryModelList = new List<AllApiRepositoryModel>();
            foreach (var allRepositoryKey in allRepositoryList.GroupBy(x => new { x.api_id, x.repository_group_id }).Select(x => new { apiId = x.Key.api_id, repositoryGroupId = x.Key.repository_group_id }))
            {
                var repositoryGroups = allRepositoryList.Where(x => x.repository_group_id == allRepositoryKey.repositoryGroupId).Where(x => x.api_id == allRepositoryKey.apiId).ToList();
                AllApiRepositoryModel rg = new AllApiRepositoryModel()
                {
                    api_id = repositoryGroups[0].api_id,
                    repository_group_id = repositoryGroups[0].repository_group_id,
                    repository_type_cd = repositoryGroups[0].repository_type_cd,
                    is_primary = repositoryGroups[0].is_primary,
                    is_secondary_primary = repositoryGroups[0].is_secondary_primary,
                    physical_repository_list = new List<AllApiPhysicalRepositoryModel>()
                };
                foreach (var pr in repositoryGroups)
                {
                    rg.physical_repository_list.Add(new AllApiPhysicalRepositoryModel() { repository_connection_string = pr.repository_connection_string, is_full = pr.is_full, PhysicalRepositoryId = pr.physical_repository_id });
                }
                allApiRepositoryModelList.Add(rg);
            }
            return allApiRepositoryModelList;
        }

        private string GetIsActiveOnlyKey(bool isActiveOnly)
        {
            if (isActiveOnly)
            {
                return CACHE_KEY_API_LIST_ACTIVE_ONLY;
            }
            else
            {
                return CACHE_KEY_API_LIST_ALL;
            }
        }

        private IEnumerable<AllApiEntity> AddTransparentApi(IEnumerable<AllApiEntity> apiList)
        {
            // 全体で一致するAPIを検索するとAPI数の増加の影響をそのまま受けるので、
            // Controller単位に分けて検索する
            var apiListDivideByController = apiList.ToLookup(item => item.controller_id);

            // 透過APIを作成できない場合はエラーとなるように、透過APIは作成したAPIに一致するものだけ返す
            foreach (var apiGroup in apiListDivideByController)
            {
                var notTransparentApiList = apiGroup.Where(x => (!x.is_transparent_api)).ToList();

                foreach (var api in notTransparentApiList)
                {
                    yield return api;
                }

                var transparentApiList = CreateTransparentApi(
                    notTransparentApiList.Where(x => x.is_enable_api).ToList()
                ).ToList();
                foreach (var transparentApi in transparentApiList)
                {
                    var targetApi = apiGroup.FirstOrDefault(x =>
                        (x.is_transparent_api
                         && (x.method_name == transparentApi.method_name)
                         && (String.Compare(x.method_type, transparentApi.method_type,
                                 StringComparison.OrdinalIgnoreCase) == 0)));
                    if (targetApi != null)
                    {
                        OverWriteTransparentApi(targetApi, transparentApi);
                        yield return targetApi;
                    }
                    else
                    {
                        yield return transparentApi;
                    }
                }

                //履歴用透過APIの作成
                var postApiList = apiGroup.Where(x => x.method_type.ToLower() == HttpMethodType.MethodTypeEnum.POST.ToString().ToLower());
                var criteriaApi = postApiList.Where(x => (!x.is_transparent_api)).OrderBy(x => x.controller_relative_url).ThenBy(x => x.method_name).FirstOrDefault();
                var transparentDocumentHistoryApiList = CreateDocumentHistoryTransparentApi(apiGroup.Where(x => (x.is_transparent_api) && x.is_document_history).ToList());
                foreach (var documentHistoryApi in transparentDocumentHistoryApiList)
                {
                    //履歴透過APIに履歴機能を使用する非透過APIのリポジトリを追加する
                    if (criteriaApi != null && criteriaApi.all_repository_model_list != null)
                        documentHistoryApi.all_repository_model_list.AddRange(criteriaApi.all_repository_model_list);
                    yield return documentHistoryApi;
                }

                //履歴機能を使用する非透過APIに履歴用リポジトリを追加する
                var transHistoryApi = apiGroup.Where(x => x.is_document_history && x.action_type_name?.ToLower() == nameof(ActionType.GetDocumentHistory).ToLower()).FirstOrDefault();
                if (transHistoryApi != null)
                {
                    var documentHistoryApiIds = transparentDocumentHistoryApiList.Select(x => x.api_id.ToString());
                    var notHistoryApi = apiGroup.Where(x => documentHistoryApiIds.Contains(x.api_id.ToString()) == false);
                    foreach (var api in notHistoryApi)
                    {
                        api.all_repository_model_list.AddRange(transHistoryApi.all_repository_model_list);
                    }
                }

                //添付ファイル向けの透過APIの作成
                var transparentAttachFileApiList = CreateTransparentAttachFileApi(apiGroup.Where(x => (x.is_transparent_api) && x.is_enable_attachfile).ToList());
                foreach (var attachFileApi in transparentAttachFileApiList)
                {
                    yield return attachFileApi;
                }

                //ブロックチェーン向けの透過APIの作成
                var targetBlockchainApi = apiGroup.Where(x => x.method_type.ToLower() == HttpMethodType.MethodTypeEnum.POST.ToString().ToLower() && !x.is_transparent_api).OrderBy(x => x.controller_relative_url).ThenBy(x => x.method_name).FirstOrDefault();
                if (targetBlockchainApi != null)
                {
                    var transparentBlockchainApiList = CreateTransparentBlockchainApi(apiGroup.Where(x => x.is_transparent_api && x.is_enable_blockchain).ToList(), targetBlockchainApi);
                    foreach (var blockChainApi in transparentBlockchainApiList)
                    {
                        yield return blockChainApi;
                    }
                }

                //添付ファイル用履歴APIの作成
                var attachFileDocumentHistoryApi = apiGroup.Where(x => x.is_transparent_api && x.is_enable_attachfile && x.is_document_history);
                if (attachFileDocumentHistoryApi.Any() == true)
                {
                    //var postApi = apiGroup.Where(x => x.method_type.ToLower() == HttpMethodType.MethodType.Post.ToString().ToLower() && !x.is_transparent_api).OrderBy(x => x.controller_relative_url).ThenBy(x => x.method_name).FirstOrDefault();
                    var targetAttachFileDocumentHistoryApiList = CreateTransparentDocumentHistoryTransparentApi(attachFileDocumentHistoryApi, attachFileDocumentHistoryApi.FirstOrDefault(x => x.action_type_cd == "aup"));
                    foreach (var fileDocHisApi in targetAttachFileDocumentHistoryApiList)
                    {
                        yield return fileDocHisApi;
                    }
                }

            }
        }

        /// <summary>
        /// 無効なリポジトリグループまたはセカンダリリポジトリグループのAPIを除外する
        /// </summary>
        /// <param name="apiList"></param>
        /// <returns></returns>
        private IEnumerable<AllApiEntity> InvalidRepositoryGroupExclude(IEnumerable<AllApiEntity> apiList)
        {
            // 全てのセカンダリリポジトリグループを取得する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT
    a.api_id               AS api_id
    ,rg.is_enable          AS is_enable
FROM
    controller c
    INNER JOIN api a ON c.controller_id=a.controller_id AND a.is_active=1
    INNER JOIN secondary_repository_map secrg ON a.api_id=secrg.api_id AND secrg.is_active=1
    INNER JOIN repository_group rg ON secrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
WHERE
    c.is_active=1";
            }
            else
            {
                sql = @"
SELECT
    a.api_id               AS api_id
    ,rg.is_enable          AS is_enable
FROM
    Controller c
    INNER JOIN Api a ON c.controller_id=a.controller_id AND a.is_active=1
    INNER JOIN SecondaryRepositoryMap secrg ON a.api_id=secrg.api_id AND secrg.is_active=1
    INNER JOIN RepositoryGroup rg ON secrg.repository_group_id=rg.repository_group_id AND rg.is_active=1
WHERE
    c.is_active=1";
            }
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, null);
            var allSecondaryRepositoryList = Cache.Get<List<AllApiRepositoryModel>>(CACHE_KEY_ALL_SECONDARYREPOSITORY_LIST, CacheExpirationTimeSpan, () => DbConnection.Query<AllApiRepositoryModel>(
                twowaySql.Sql).ToList());
            var ret = apiList.ToList();
            // リポジトリグループが無い場合はOK(Gateway等)
            // リポジトリグループが有効
            ret = ret.Where(x => x.is_enable_repository == null || x.is_enable_repository.Value == true).ToList();
            foreach (var api in apiList)
            {
                // セカンダリリポジトリグループが全て有効か
                var secRep = allSecondaryRepositoryList.Where(x => x.api_id == api.api_id);
                if (secRep != null && secRep.Where(x => !x.is_enable).Count() != 0)
                {
                    ret.Remove(api);
                }
            }
            return ret;
        }

        private void OverWriteTransparentApi(AllApiEntity api, AllApiEntity transparentApi)
        {
            // 透過APIからリポジトリの情報を引き継ぎ
            api.repository_group_id = transparentApi.repository_group_id;
            api.repository_type_cd = transparentApi.repository_type_cd;
            api.all_repository_model_list = transparentApi.all_repository_model_list;

            // 透過APIで作成した情報
            api.is_cache = transparentApi.is_cache;
            // is_admin_authenticationはDBの値を優先
            api.action_type_cd = transparentApi.action_type_cd;
            // method_typeは同じはず
            // api_idはDBの値を優先
            api.api_description = transparentApi.api_description;
            // method_nameは同じはず
            api.post_data_type = transparentApi.post_data_type;
            api.query = transparentApi.query;
            api.ActionInjector = transparentApi.ActionInjector;
            // is_accesskeyはDBの値を優先
            api.alias_method_name = transparentApi.alias_method_name;
            api.is_nomatch_querystring = transparentApi.is_nomatch_querystring;
        }

        /// <summary>
        /// DynamicAPIとして透過的なAPIを作成する
        /// </summary>
        /// <param name="apiList"></param>
        /// <returns></returns>
        private IEnumerable<AllApiEntity> CreateTransparentApi(IEnumerable<AllApiEntity> apiList)
        {
            var tempList = apiList.Where(x => x.method_type.ToLower() == HttpMethodType.MethodTypeEnum.POST.ToString().ToLower()).OrderBy(x => x.controller_relative_url).ThenBy(x => x.method_name).ToList();
            if (!tempList.Any())
            {
                tempList = apiList.Where(x => x.action_type_cd == "gtw").OrderBy(x => x.controller_relative_url).ThenBy(x => x.method_name).ToList();
            }

            foreach (var controller_id in tempList.Select(x => x.controller_id).Distinct())
            {
                var data = tempList.Where(x => x.controller_id == controller_id).FirstOrDefault();

                if (data.is_enable_resource_version && EnableResourceVersion)
                {
                    // SetNewVersion
                    var snv = Mapper.Map<AllApiEntity>(data);
                    snv.is_cache = false;
                    snv.is_admin_authentication = false;
                    snv.action_type_cd = "quy";
                    snv.method_type = "POST";
                    snv.api_id = Guid.NewGuid();
                    snv.api_description = null;
                    snv.method_name = "SetNewVersion";
                    snv.post_data_type = "";
                    snv.query = "";
                    snv.ActionInjector = typeof(Domain.ActionInjector.SetNewVersionActionInjector);
                    snv.is_accesskey = false;
                    yield return snv;

                    // GetCurrentVersion
                    var gcv = Mapper.Map<AllApiEntity>(data);
                    gcv.is_cache = false;
                    gcv.is_admin_authentication = false;
                    gcv.action_type_cd = "quy";
                    gcv.api_id = Guid.NewGuid();
                    gcv.api_description = null;
                    gcv.method_type = "GET";
                    gcv.method_name = "GetCurrentVersion";
                    gcv.post_data_type = "";
                    gcv.ActionInjector = typeof(Domain.ActionInjector.GetCurrentVersionActionInjector);
                    gcv.is_accesskey = false;
                    yield return gcv;

                    // CreateRegisterVersion
                    var crv = Mapper.Map<AllApiEntity>(data);
                    crv.is_cache = false;
                    crv.is_admin_authentication = false;
                    crv.action_type_cd = "quy";
                    crv.api_id = Guid.NewGuid();
                    crv.api_description = null;
                    crv.method_type = "POST";
                    crv.method_name = "CreateRegisterVersion";
                    crv.post_data_type = "";
                    crv.ActionInjector = typeof(Domain.ActionInjector.CreateRegisterVersionActionInjector);
                    crv.is_accesskey = false;
                    yield return crv;

                    // CompleteRegisterVersion
                    var orv = Mapper.Map<AllApiEntity>(data);
                    orv.is_cache = false;
                    orv.is_admin_authentication = false;
                    orv.action_type_cd = "quy";
                    orv.api_id = Guid.NewGuid();
                    orv.api_description = null;
                    orv.method_type = "POST";
                    orv.method_name = "CompleteRegisterVersion";
                    orv.post_data_type = "";
                    orv.ActionInjector = typeof(Domain.ActionInjector.CompleteRegisterVersionActionInjector);
                    orv.is_accesskey = false;
                    yield return orv;

                    // GetRegisterVersion
                    var grv = Mapper.Map<AllApiEntity>(data);
                    grv.is_cache = false;
                    grv.is_admin_authentication = false;
                    grv.action_type_cd = "quy";
                    grv.api_id = Guid.NewGuid();
                    grv.api_description = null;
                    grv.method_type = "GET";
                    grv.method_name = "GetRegisterVersion";
                    grv.post_data_type = "";
                    grv.ActionInjector = typeof(Domain.ActionInjector.GetRegisterVersionActionInjector);
                    grv.is_accesskey = false;
                    yield return grv;

                    // GetVersionInfo
                    var gvi = Mapper.Map<AllApiEntity>(data);
                    gvi.is_cache = false;
                    gvi.is_admin_authentication = false;
                    gvi.action_type_cd = "quy";
                    gvi.api_id = Guid.NewGuid();
                    gvi.api_description = null;
                    gvi.method_type = "GET";
                    gvi.method_name = "GetVersionInfo";
                    gvi.post_data_type = "";
                    gvi.ActionInjector = typeof(Domain.ActionInjector.GetVersionInfoInjector);
                    gvi.is_accesskey = false;
                    yield return gvi;
                }

                // GetCount
                var gc = Mapper.Map<AllApiEntity>(data);
                gc.is_cache = false;
                gc.is_admin_authentication = false;
                gc.action_type_cd = "quy";
                gc.api_id = Guid.NewGuid();
                gc.api_description = null;
                gc.method_type = "GET";
                gc.method_name = "GetCount";
                gc.post_data_type = "array";
                gc.query = "$count=true";
                gc.ActionInjector = typeof(Domain.ActionInjector.GetCountActionInjector);
                gc.is_accesskey = false;
                yield return gc;

                // OData
                var oda = Mapper.Map<AllApiEntity>(data);
                oda.is_cache = false;
                oda.is_admin_authentication = false;
                oda.action_type_cd = "oda";
                oda.api_id = Guid.NewGuid();
                oda.api_description = null;
                oda.method_type = "GET";
                oda.method_name = "OData";
                oda.alias_method_name = "Default";
                oda.post_data_type = "array";
                oda.ActionInjector = null;
                oda.is_accesskey = false;
                oda.is_nomatch_querystring = true;
                yield return oda;

                // ODataDelete
                var odadel = Mapper.Map<AllApiEntity>(data);
                odadel.is_cache = false;
                odadel.is_admin_authentication = false;
                odadel.action_type_cd = "odd";
                odadel.api_id = Guid.NewGuid();
                odadel.api_description = null;
                odadel.method_type = "DELETE";
                odadel.method_name = "ODataDelete";
                odadel.post_data_type = "array";
                odadel.ActionInjector = null;
                odadel.is_accesskey = false;
                odadel.is_nomatch_querystring = true;
                yield return odadel;

                // ODataPatch
                var odp = Mapper.Map<AllApiEntity>(data);
                odp.is_cache = false;
                odp.is_admin_authentication = false;
                odp.action_type_cd = "odp";
                odp.api_id = Guid.NewGuid();
                odp.api_description = null;
                odp.method_type = "PATCH";
                odp.method_name = "ODataPatch";
                odp.post_data_type = "array";
                odp.ActionInjector = null;
                odp.is_accesskey = false;
                odp.is_nomatch_querystring = true;
                yield return odp;

                // RegisterRawData
                var rrd = Mapper.Map<AllApiEntity>(data);
                rrd.action_type_cd = "rrd";
                rrd.is_cache = false;
                rrd.is_admin_authentication = true;
                rrd.api_id = Guid.NewGuid();
                rrd.api_description = null;
                rrd.method_type = "POST";
                rrd.method_name = "RegisterRawData";
                rrd.post_data_type = "";
                rrd.ActionInjector = null;
                rrd.is_accesskey = false;
                rrd.is_nomatch_querystring = true;
                rrd.is_openid_authentication = false;
                rrd.is_hidden = true;
                rrd.is_header_authentication = true;
                rrd.script = null;
                yield return rrd;

                // ODataRawData
                var ord = Mapper.Map<AllApiEntity>(data);
                ord.action_type_cd = "ord";
                ord.is_cache = false;
                ord.is_admin_authentication = true;
                ord.api_id = Guid.NewGuid();
                ord.api_description = null;
                ord.method_type = "GET";
                ord.method_name = "ODataRawData";
                ord.post_data_type = "array";
                ord.ActionInjector = null;
                ord.is_accesskey = false;
                ord.is_nomatch_querystring = true;
                ord.is_openid_authentication = false;
                ord.is_hidden = true;
                ord.is_header_authentication = true;
                ord.script = null;
                yield return ord;

                // AdaptResourceSchema
                var ars = Mapper.Map<AllApiEntity>(data);
                ars.action_type_cd = "ars";
                ars.is_cache = false;
                ars.is_admin_authentication = false;
                ars.api_id = Guid.NewGuid();
                ars.method_type = "POST";
                ars.method_name = "AdaptResourceSchema";
                ars.post_data_type = "";
                ars.ActionInjector = null;
                ars.is_accesskey = false;
                ars.is_nomatch_querystring = true;
                ars.is_openid_authentication = false;
                ars.is_vendor = false;
                ars.is_hidden = true;
                ars.is_header_authentication = true;
                yield return ars;

                // GetResourceSchema
                var grs = Mapper.Map<AllApiEntity>(data);
                grs.action_type_cd = "grs";
                grs.is_cache = false;
                grs.is_admin_authentication = false;
                grs.api_id = Guid.NewGuid();
                grs.method_type = "GET";
                grs.method_name = "GetResourceSchema";
                grs.post_data_type = "";
                grs.ActionInjector = null;
                grs.is_accesskey = false;
                grs.is_nomatch_querystring = true;
                grs.is_openid_authentication = false;
                grs.is_vendor = false;
                grs.is_hidden = true;
                grs.is_header_authentication = true;
                yield return grs;
            }
            yield break;
        }


        #region CreateDocumentHistoryTransparentApi

        private class DocumentTransparentApi
        {
            public string Name { get; set; }
            public Func<AllApiEntity, AllApiEntity> Func { get; set; }
            public bool Used { get; set; }
        }

        private List<DocumentTransparentApi> listDocmentTransparentApi = new List<DocumentTransparentApi>()
        {
            new DocumentTransparentApi(){ Name = "GetDocumentVersion", Func = new Func<AllApiEntity, AllApiEntity>(OverrideGetDocumentVersion), Used = false },
            new DocumentTransparentApi(){ Name = "GetDocumentHistory", Func = new Func<AllApiEntity, AllApiEntity>(OverrideGetDocumentHistory), Used = false },
            new DocumentTransparentApi(){ Name = "DriveOutDocument", Func = new Func<AllApiEntity, AllApiEntity>(OverrideDriveOutDocument), Used = false },
            new DocumentTransparentApi(){ Name =  "ReturnDocument", Func = new Func<AllApiEntity, AllApiEntity>(OverrideReturnDocument), Used = false },
            new DocumentTransparentApi(){ Name = "HistoryThrowAway", Func = new Func<AllApiEntity, AllApiEntity>(OverrideHistoryThrowAway), Used = false },
        };

        private IEnumerable<AllApiEntity> CreateDocumentHistoryTransparentApi(IEnumerable<AllApiEntity> apiList)
        {
            AllApiEntity apiReference = null;
            listDocmentTransparentApi.ForEach(x => x.Used = false);
            foreach (var api in apiList)
            {
                var hit = listDocmentTransparentApi.Where(x => x.Name == api.method_name).FirstOrDefault();
                if (hit != null)
                {
                    apiReference = api;
                    hit.Used = true;
                    yield return hit.Func(api);
                }
            }
            if (apiReference != null)
            {
                foreach (var noMappedApi in listDocmentTransparentApi.Where(x => x.Used == false).ToList())
                {
                    yield return noMappedApi.Func(apiReference);
                }
            }
        }

        private static AllApiEntity OverrideGetDocumentVersion(AllApiEntity data)
        {
            var gdv = Mapper.Map<AllApiEntity>(data);
            gdv.is_cache = false;
            gdv.is_admin_authentication = false;
            gdv.action_type_cd = "gdv";
            gdv.api_description = null;
            gdv.method_type = "GET";
            gdv.method_name = "GetDocumentVersion/{id}";
            gdv.post_data_type = "";
            gdv.ActionInjector = null;
            gdv.is_accesskey = false;
            gdv.is_nomatch_querystring = true;
            gdv.controller_repository_key = data.controller_repository_key;
            return gdv;
        }

        private static AllApiEntity OverrideGetDocumentHistory(AllApiEntity data)
        {
            var gdh = Mapper.Map<AllApiEntity>(data);
            gdh.is_cache = false;
            gdh.is_admin_authentication = false;
            gdh.action_type_cd = "gdh";
            gdh.api_description = null;
            gdh.method_type = "GET";
            gdh.method_name = "GetDocumentHistory/{id}?version={version}";
            gdh.post_data_type = "";
            gdh.ActionInjector = null;
            gdh.is_accesskey = false;
            gdh.is_nomatch_querystring = true;
            return gdh;
        }

        private static AllApiEntity OverrideDriveOutDocument(AllApiEntity data)
        {
            var dod = Mapper.Map<AllApiEntity>(data);
            dod.is_cache = false;
            dod.is_admin_authentication = false;
            dod.action_type_cd = "dod";
            dod.api_description = null;
            dod.method_type = "GET";
            dod.method_name = "DriveOutDocument/{id}";
            dod.post_data_type = "";
            dod.ActionInjector = null;
            dod.is_accesskey = false;
            dod.is_nomatch_querystring = true;
            return dod;
        }

        private static AllApiEntity OverrideReturnDocument(AllApiEntity data)
        {
            var rtd = Mapper.Map<AllApiEntity>(data);
            rtd.is_cache = false;
            rtd.is_admin_authentication = false;
            rtd.action_type_cd = "rtd";
            rtd.api_description = null;
            rtd.method_type = "GET";
            rtd.method_name = "ReturnDocument/{id}";
            rtd.post_data_type = "";
            rtd.ActionInjector = null;
            rtd.is_accesskey = false;
            rtd.is_nomatch_querystring = true;
            return rtd;
        }

        private static AllApiEntity OverrideHistoryThrowAway(AllApiEntity data)
        {
            var hta = Mapper.Map<AllApiEntity>(data);
            hta.is_cache = false;
            hta.is_admin_authentication = true;
            hta.action_type_cd = "hta";
            hta.api_description = null;
            hta.method_type = "DELETE";
            hta.method_name = "HistoryThrowAway/{id}";
            hta.post_data_type = "";
            hta.ActionInjector = null;
            hta.is_accesskey = false;
            hta.is_nomatch_querystring = true;
            return hta;
        }

        #endregion

        #region CreateAttachfileTransparentApi

        /// <summary>
        /// DynamicAPIとして透過的なAttachFileAPIを作成する
        /// </summary>
        /// <param name="apiList"></param>
        /// <returns></returns>
        private IEnumerable<AllApiEntity> CreateTransparentAttachFileApi(IEnumerable<AllApiEntity> apiList)
        {
            foreach (var api in apiList)
            {
                switch (api.method_name)
                {
                    case "CreateAttachFile":
                        yield return OverrideCreateAttachFile(api);
                        break;
                    case "UploadAttachFile/{FileId}":
                        yield return OverrideUploadAttachFile(api);
                        break;
                    case "GetAttachFile/{FileId}":
                        yield return OverrideGetAttachFile(api);
                        break;
                    case "DeleteAttachFile/{FileId}":
                        yield return OverrideDeleteAttachFile(api);
                        break;
                    case "GetAttachFileMeta/{FileId}":
                        yield return OverrideGetAttachFileMeta(api);
                        break;
                    case "GetAttachFileMetaList":
                        yield return OverrideGetAttachFileMetaList(api);
                        break;
                    default:
                        break;
                }
            }
            yield break;
        }
        private AllApiEntity OverrideCreateAttachFile(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);
            transparentApi.controller_repository_key = ConvertAttachFileRepositoryKey(transparentApi.controller_repository_key);
            transparentApi.ActionInjector = typeof(Domain.ActionInjector.CreateAttachFileActionInjector);
            return transparentApi;
        }
        private AllApiEntity OverrideUploadAttachFile(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);
            transparentApi.controller_repository_key = ConvertAttachFileRepositoryKey(transparentApi.controller_repository_key);
            transparentApi.is_nomatch_querystring = true;
            return transparentApi;
        }

        private AllApiEntity OverrideGetAttachFile(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);
            transparentApi.controller_repository_key = ConvertAttachFileRepositoryKey(transparentApi.controller_repository_key);
            transparentApi.is_nomatch_querystring = true;
            return transparentApi;
        }

        private AllApiEntity OverrideDeleteAttachFile(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);
            transparentApi.controller_repository_key = ConvertAttachFileRepositoryKey(transparentApi.controller_repository_key);
            transparentApi.is_nomatch_querystring = true;
            return transparentApi;
        }
        private AllApiEntity OverrideGetAttachFileMeta(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);
            transparentApi.controller_repository_key = ConvertAttachFileRepositoryKey(transparentApi.controller_repository_key);
            transparentApi.ActionInjector = typeof(Domain.ActionInjector.GetAttachFileMetaActionInjector);
            transparentApi.action_type_cd = "oda";
            return transparentApi;
        }
        private AllApiEntity OverrideGetAttachFileMetaList(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);
            transparentApi.controller_repository_key = ConvertAttachFileRepositoryKey(transparentApi.controller_repository_key);
            transparentApi.ActionInjector = typeof(Domain.ActionInjector.GetAttachFileMetaListActionInjector);
            transparentApi.is_nomatch_querystring = true;
            transparentApi.action_type_cd = "oda";
            return transparentApi;
        }
        private string ConvertAttachFileRepositoryKey(string repositoryKey)
        {
            if (string.IsNullOrEmpty(repositoryKey))
            {
                return repositoryKey;
            }

            Regex reg = new Regex(@"\{.+?\}");
            string result = reg.Replace(repositoryKey, "");
            result += result.EndsWith("/") ? "" : "/";
            result += $"{{FileId}}";
            return result;
        }
        #endregion


        #region CreateBlockchainTransparentApi
        /// <summary>
        /// DynamicAPIとして透過的なBlockchainAPIを作成する
        /// </summary>
        /// <param name="apiList"></param>
        /// <param name="targetApi"></param>
        /// <returns></returns>
        private IEnumerable<AllApiEntity> CreateTransparentBlockchainApi(IEnumerable<AllApiEntity> apiList, AllApiEntity targetApi)
        {
            foreach (var api in apiList)
            {
                switch (api.method_name)
                {
                    case "ValidateWithBlockchain/{id}":
                        yield return OverrideValidateWithBlockchain(api, targetApi);
                        break;
                    case "ValidateAttachFileWithBlockchain/{fileid}":
                        if (targetApi.is_enable_attachfile)
                        {
                            yield return OverrideValidateAttachFileWithBlockchain(api);
                        }
                        break;
                    default:
                        break;
                }
            }
            yield break;
        }

        private AllApiEntity OverrideValidateWithBlockchain(AllApiEntity data, AllApiEntity targetApi)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);

            // 透過APIにリポジトリの情報を引き継ぎ
            transparentApi.repository_group_id = targetApi.repository_group_id;
            transparentApi.repository_type_cd = targetApi.repository_type_cd;
            transparentApi.all_repository_model_list = targetApi.all_repository_model_list;

            transparentApi.ActionInjector = typeof(ValidateWithBlockchainActionInjector);
            return transparentApi;
        }
        private AllApiEntity OverrideValidateAttachFileWithBlockchain(AllApiEntity data)
        {
            var transparentApi = Mapper.Map<AllApiEntity>(data);

            transparentApi.ActionInjector = typeof(ValidateAttachFileWithBlockchainActionInjector);
            return transparentApi;
        }
        #endregion

        #region CreateAttachfileDocumentHistoryTransparentApi

        private IEnumerable<AllApiEntity> CreateTransparentDocumentHistoryTransparentApi(IEnumerable<AllApiEntity> apiList, AllApiEntity targetApi)
        {
            foreach (var api in apiList)
            {
                switch (api.method_name)
                {
                    case "GetAttachFileDocumentVersion/{id}":
                        yield return OverrideGetAttachFileDocumentVersion(api);
                        break;
                    case "GetAttachFileDocumentHistory/{FileId}?version={version}":
                        yield return OverrideGetAttachFileDocumentHistory(api);
                        break;
                    case "DriveOutAttachFileDocument/{FileId}":
                        yield return OverrideDriveOutAttachFileDocument(api);
                        break;
                    case "ReturnAttachFileDocument/{FileId}":
                        yield return OverrideReturnAttachFileDocument(api);
                        break;
                    default:
                        break;
                }
            }
            yield break;
        }

        private AllApiEntity OverrideGetAttachFileDocumentVersion(AllApiEntity data)
        {
            var gdv = Mapper.Map<AllApiEntity>(data);
            gdv.is_cache = false;
            gdv.is_admin_authentication = false;
            gdv.action_type_cd = "gdv";
            gdv.api_id = Guid.NewGuid();
            gdv.api_description = null;
            gdv.method_type = "GET";
            gdv.method_name = "GetAttachFileDocumentVersion/{id}";
            gdv.post_data_type = "";
            gdv.ActionInjector = null;
            gdv.is_accesskey = false;
            gdv.is_nomatch_querystring = true;
            return gdv;
        }

        private AllApiEntity OverrideGetAttachFileDocumentHistory(AllApiEntity data)
        {
            var gdh = Mapper.Map<AllApiEntity>(data);
            gdh.is_cache = false;
            gdh.is_admin_authentication = false;
            gdh.action_type_cd = "gah";
            gdh.api_id = Guid.NewGuid();
            gdh.api_description = null;
            gdh.method_type = "GET";
            gdh.method_name = "GetAttachFileDocumentHistory/{FileId}?version={version}";
            gdh.post_data_type = "";
            gdh.ActionInjector = null;
            gdh.is_accesskey = false;
            gdh.is_nomatch_querystring = true;
            gdh.controller_repository_key = ConvertAttachFileRepositoryKey(gdh.controller_repository_key);
            return gdh;
        }


        private AllApiEntity OverrideDriveOutAttachFileDocument(AllApiEntity data)
        {
            var dod = Mapper.Map<AllApiEntity>(data);
            dod.is_cache = false;
            dod.is_admin_authentication = false;
            dod.action_type_cd = "dad";
            dod.api_id = Guid.NewGuid();
            dod.api_description = null;
            dod.method_type = "GET";
            dod.method_name = "DriveOutAttachFileDocument/{FileId}";
            dod.post_data_type = "";
            dod.ActionInjector = null;
            dod.is_accesskey = false;
            dod.is_nomatch_querystring = true;
            dod.controller_repository_key = ConvertAttachFileRepositoryKey(dod.controller_repository_key);
            return dod;
        }

        private AllApiEntity OverrideReturnAttachFileDocument(AllApiEntity data)
        {
            var rtd = Mapper.Map<AllApiEntity>(data);
            rtd.is_cache = false;
            rtd.is_admin_authentication = false;
            rtd.action_type_cd = "rad";
            rtd.api_id = Guid.NewGuid();
            rtd.api_description = null;
            rtd.method_type = "GET";
            rtd.method_name = "ReturnAttachFileDocument/{FileId}";
            rtd.post_data_type = "";
            rtd.ActionInjector = null;
            rtd.is_accesskey = false;
            rtd.is_nomatch_querystring = true;
            rtd.controller_repository_key = ConvertAttachFileRepositoryKey(rtd.controller_repository_key);
            return rtd;
        }

        #endregion

        private string GetConst(string key)
        {
            //リポジトリグループ、セカンダリリポジトリグループのPhysicalRepositoryを軸に取得する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = string.Format(@"-- DynamicAPIRepository Const
SELECT const_value FROM const WHERE const_key= /*ds key*/'1' 
");
            }
            else
            {
                sql = string.Format(@"-- DynamicAPIRepository Const
SELECT [const_value] FROM Const WHERE [const_key]=@key
");
            }
            var param = new { key = key };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            return DbConnection.Query<string>(twowaySql.Sql, dynParams).FirstOrDefault();
        }

        private void SetConst(string key, string val)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = string.Format(@"
MERGE INTO const a
    USING (SELECT /*ds const_key*/'1' AS const_key, /*ds const_value*/'1' AS const_value FROM DUAL) b
    ON ( a.const_key = b.const_key )
    WHEN MATCHED THEN
        UPDATE SET
            const_value=b.const_value
    WHEN NOT MATCHED THEN
        INSERT (const_key,const_value)
            VALUES(b.const_key,b.const_value)
");
            }
            else
            {
                sql = string.Format(@"
MERGE INTO Const AS a
    USING (SELECT @const_key AS const_key,@const_value AS const_value ) AS b
    ON ( a.const_key = b.const_key )
    WHEN MATCHED THEN
        UPDATE SET
            const_value=b.const_value
    WHEN NOT MATCHED THEN
        INSERT (const_key,const_value)
            VALUES(b.const_key,b.const_value)
;
");
            }
            var param = new { const_key = key, const_value = val };
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param).SetNClob(nameof(param.const_value));
            var result = DbConnection.Execute(twowaySql.Sql, dynParams);
        }


        private ApiTreeNode GetApiTree()
        {
            List<AllApiEntityIdentifier> apiList = GetAllApiWithVersionApiSimple()
                .Select(x => Mapper.Map<AllApiEntityIdentifier>(x))
                .ToList();

            ApiTreeNode tree = new ApiTreeNode();
            apiList.ForEach(x => tree.AddApiEntity(x));
            return tree;
        }
    }
}
