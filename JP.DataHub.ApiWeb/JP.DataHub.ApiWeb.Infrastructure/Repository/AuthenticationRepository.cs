using Microsoft.Extensions.Configuration;
using Dapper;
using MsgPack = MessagePack;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.SQL;
using JP.DataHub.ApiWeb.Core.Cache;
using JP.DataHub.ApiWeb.Core.Cache.Attributes;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.Com.Settings;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.Com.Sql;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    [CacheKey]
    internal class AuthenticationRepository : AbstractRepository, IAuthenticationRepository
    {
        [MsgPack.MessagePackObject]
        public class VendorSystemFunction
        {
            [MsgPack.Key(0)]
            public Guid vendor_id { get; set; }
            [MsgPack.Key(1)]
            public string vendor_name { get; set; }
            [MsgPack.Key(2)]
            public Guid system_id { get; set; }
            [MsgPack.Key(3)]
            public string system_name { get; set; }
            [MsgPack.Key(4)]
            public string function_name { get; set; }
            [MsgPack.Key(5)]
            public bool is_data_offer { get; set; }
            [MsgPack.Key(6)]
            public bool is_data_use { get; set; }
            [MsgPack.Key(7)]
            public bool is_enable { get; set; }
        }

        [CacheKey(CacheKeyType.EntityWithKey, "vendor", "system")]
        public static string CACHE_KEY_LOGIN_VENDOR_SYSTEM_LIST = "AuthenticationRepository-Login-Vendor-System-List";
        [CacheKey(CacheKeyType.Entity, "vendor", "system")]
        public static string CACHE_KEY_GETVENDORSYSTEMBASELIST = "AuthenticationRepository-GetVendorSystemBaseList";
        [CacheKey(CacheKeyType.Entity, "vendor", "system")]
        public static string CACHE_KEY_GETVENDORSYSTEMFUNCLIST = "AuthenticationRepository-GetVendorSystemFuncList";

        [CacheKey(CacheKeyType.EntityWithKey, "vendor", "system")]
        public static string CACHE_KEY_ADMINKEYWORD = "AuthenticationRepository-IsAdmin";

        public static string CACHE_KEY_CLEAR_STATICCACHE_TIME = "AuthenticationRepository-ClearStaticCacheTime";
        public static string CACHE_KEY_CLEAR_STATICCACHE_TIME_DB = "AuthenticationRepository-ClearStaticCacheTime-DB";


		private readonly JPDataHubLogger _log = new JPDataHubLogger(typeof(AuthenticationRepository));

		protected IJPDataHubDbConnection DbConnection => UnityCore.Resolve<IJPDataHubDbConnection>("Authority");

        protected ICache Cache => _lazyCache.Value;
        private Lazy<ICache> _lazyCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("Authority"));

        protected bool IsCheckAuthenticationVendorSystemFunc { get => _isCheckAuthenticationVendorSystemFunc.Value; }
        private Lazy<bool> _isCheckAuthenticationVendorSystemFunc = new Lazy<bool>(() => UnityCore.Resolve<bool>("Authentication.IsCheck.VendorSystemFunc"));

		#region StaticCache
		protected bool IsAuthenticationStaticCache => _isAuthenticationStaticCache.Value;
		protected readonly Lazy<bool> _isAuthenticationStaticCache = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsAuthenticationStaticCache"));

		protected string StaticCacheTimeServer { get => _staticCacheTimeServer.Value; }
		protected Lazy<string> _staticCacheTimeServer = new Lazy<string>(() => UnityCore.Resolve<string>("StaticCacheClusterSyncTimeServer"));

		protected int StaticCacheCheckInterval { get => _staticCacheCheckInterval.Value; }
		protected Lazy<int> _staticCacheCheckInterval = new Lazy<int>(() => UnityCore.Resolve<int>("StaticCacheClusterSyncCheckInterval"));

		protected TimeSpan AuthenticationCacheExpirationTimeSpan { get => _authenticationCacheExpirationTimeSpan.Value; }
		protected Lazy<TimeSpan> _authenticationCacheExpirationTimeSpan = new Lazy<TimeSpan>(() => UnityCore.Resolve<TimeSpan>("AuthenticationCacheExpirationTimeSpan"));
	
		private static StaticCacheControl<List<VendorSystemFunction>> scVendorSystemFunc = new StaticCacheControl<List<VendorSystemFunction>>();

		private static string scLastRefreshTime;
		private static string scExpirationTime;
		private static string scNextCheckTime;
		private static object lockStaticCacheRefresh = new object();
		private static object lockStaticCacheCheck = new object();


		public Task RefreshStaticCache(string time) => RefreshStaticCache(time, true);

		private Task RefreshStaticCache(string time, bool save)
		{
			if (IsAuthenticationStaticCache)
			{
				// StaticCacheをリフレッシュ(待機しない)
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
							scVendorSystemFunc.Refresh(() => GetVendorSystemFuncFromDB(null, null));

							scLastRefreshTime = time;
							scExpirationTime = (DateTime.Parse(time) + AuthenticationCacheExpirationTimeSpan).ToString("yyyy/MM/dd HH:mm:ss.fff");
							_log.Debug($"StaticCacheLastClearTime={scLastRefreshTime} StaticCacheExpirationTime={scExpirationTime}");

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
				if (!IsAuthenticationStaticCache)
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
					_ = RefreshStaticCache(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff"), (scExpirationTime != null));
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
							_ = RefreshStaticCache(tmp ?? DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff"), tmp != null ? false : true);
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
			return Cache.Get<string>(CACHE_KEY_CLEAR_STATICCACHE_TIME, out bool isNullValue);
		}

        private void PutClearStaticCacheToCache(string time)
        {
            // RedisにStaticCacheを消した時間を持つ場合、その値は全サーバーに伝搬させたいため1万時間保持するようにした（消してほしくない）
            Cache.Add(CACHE_KEY_CLEAR_STATICCACHE_TIME, time, 10000, 0, 0);
        }

        private string GetClearStaticCacheFromDB()
        {
            // DBを観に行く方。ただし毎回DBを見てしまっては無駄に負荷が高くなってしまうので、n秒間に１回にする
            return GetConst(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB);
        }

		private void PutClearStaticCacheToDB(string time)
		{
			SetConst(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB, time);
			Cache.Add(CACHE_KEY_CLEAR_STATICCACHE_TIME_DB, time, StaticCacheCheckInterval);
		}

        #endregion

        public User Login(VendorId vendorId, SystemId systemId, UserId userId = null)
        {
            var vendor_id = vendorId?.ToGuid;
            var system_id = systemId?.ToGuid;
            if (vendor_id == null)
            {
                throw new NotFoundException("vendor");
            }
            if (system_id == null)
            {
                throw new NotFoundException("system");
            }

            List<VendorSystemFunction> result = GetVendorSystemFunc(vendorId, systemId);
            if (result?.Any() != true)
            {
                throw new NotFoundException("vendor");
            }
            var vendor = Vendor.Create
            (
                new VendorId(result[0].vendor_id.ToString()),
                new VendorName(result[0].vendor_name),
                new IsDataOffer(result[0].is_data_offer),
                new IsDataUse(result[0].is_data_use),
                new IsEnable(result[0].is_enable)
            );
            var system = new SystemEntity(new SystemId(result[0].system_id.ToString()), new SystemName(result[0].system_name));
            if (IsCheckAuthenticationVendorSystemFunc == true)
            {
                vendor.ApiFunction = new FunctionNames(result.Select(x => x.function_name).Where(x => x != null).ToList());
            }
            return new User(vendor, system, userId);
        }

        /// <summary>
        /// CacheTimeの有効期限
        /// </summary>
        protected TimeSpan CacheExpirationTimeSpan { get => _cacheExpirationTimeSpan.Value; }
        protected Lazy<TimeSpan> _cacheExpirationTimeSpan = new Lazy<TimeSpan>(() => UnityCore.Resolve<TimeSpan>("AuthenticationCacheExpirationTimeSpan"));

		private List<VendorSystemFunction> GetVendorSystemFunc(VendorId vendorId, SystemId systemId)
		{
			if (IsAuthenticationStaticCache == true && scVendorSystemFunc.TryGet(out var list))
			{
				return vendorId == null ? list : list?.Where(x => x.vendor_id == vendorId.ToGuid && x.system_id == systemId.ToGuid).ToList();
			}
			else
			{
				return GetVendorSystemFuncFromCache(vendorId, systemId);
			}
		}

        private List<VendorSystemFunction> GetVendorSystemFuncFromCache(VendorId vendorId, SystemId systemId)
        {
            return Cache.Get<List<VendorSystemFunction>>(vendorId == null && systemId == null ? CACHE_KEY_LOGIN_VENDOR_SYSTEM_LIST : CacheManager.CreateKey(CACHE_KEY_LOGIN_VENDOR_SYSTEM_LIST, vendorId, systemId), CacheExpirationTimeSpan, () => GetVendorSystemFuncFromDB(vendorId, systemId));
        }

        private List<VendorSystemFunction> GetVendorSystemFuncFromDB(VendorId vendorId, SystemId systemId)
        {
            var vendor_id = vendorId?.ToGuid;
            var system_id = systemId?.ToGuid;
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                //with(nolock)を削除した(SQL ServerのSQLを参照)
                sql = @"
--VendorSystemFunc
SELECT
	v.vendor_id
	,v.vendor_name
	,v.is_enable
	,v.is_data_offer
	,v.is_data_use
	,s.system_id
	,s.system_name
/*ds if func != null*/
	,f.function_name
/*ds end if*/
FROM
	VENDOR v
	INNER JOIN SYSTEM s  ON v.vendor_id=s.vendor_id AND s.is_enable=1 AND s.is_active=1
/*ds if func != null*/
	LEFT OUTER JOIN SYSTEM_FUNC sf  ON s.system_id=sf.system_id AND sf.is_active=1
	INNER JOIN FUNC f ON sf.function_id=f.function_id AND f.is_enable=1 AND f.is_active=1
/*ds end if*/
WHERE
	v.is_enable=1
	AND v.is_active=1
/*ds if vendor_id != null*/
	AND v.vendor_id = /*ds vendor_id*/'00000000-0000-0000-0000-000000000000' 
	AND s.system_id = /*ds system_id*/'00000000-0000-0000-0000-000000000000' 
/*ds end if*/
";
            }
            else
            {
                sql = @"
--VendorSystemFunc
SELECT
	v.vendor_id
	,v.vendor_name
	,v.is_enable
	,v.is_data_offer
	,v.is_data_use
	,s.system_id
	,s.system_name
/*ds if func != null*/
	,f.function_name
/*ds end if*/
FROM
	Vendor v with(nolock)
	INNER JOIN System s with(nolock) ON v.vendor_id=s.vendor_id AND s.is_enable=1 AND s.is_active=1
/*ds if func != null*/
	LEFT OUTER JOIN SystemFunc sf with(nolock) ON s.system_id=sf.system_id AND sf.is_active=1
	INNER JOIN Func f with(nolock) ON sf.function_id=f.function_id AND f.is_enable=1 AND f.is_active=1
/*ds end if*/
WHERE
	v.is_enable=1
	AND v.is_active=1
/*ds if vendor_id != null*/
	AND v.vendor_id =@vendor_id 
	AND s.system_id =@system_id
/*ds end if*/
";
            }

            var dict = new Dictionary<string, object>();
            dict.Add("func", IsCheckAuthenticationVendorSystemFunc ? "true" : null);
            dict.Add("vendor_id", vendor_id);
            dict.Add("system_id", system_id);
            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, dict);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(new { vendor_id, system_id });
            return DbConnection.Query<VendorSystemFunction>(twowaySql.Sql, dynParams).ToList();
        }

        public AdminAuthResult IsAdmin(AdminKeyword adminKeyword, SystemId systemId)
        {
            string key = adminKeyword != null ? adminKeyword.Value : null;
            var verifyKey = UnityCore.Resolve<IConfiguration>().GetValue<string>("AppConfig:AdminKeyword");
            if (!string.IsNullOrEmpty(systemId?.Value))
            {
                var cacheKey = CacheManager.CreateKey(
                    CACHE_KEY_ADMINKEYWORD,
                    systemId.Value
                );
                var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
                var sql = "";
                if (dbSettings.Type == "Oracle")
                {
                    sql = @"
SELECT
	SA.admin_secret  AdminSecret
FROM       SYSTEM_ADMIN SA
INNER JOIN SYSTEM SY
  ON SA.system_id = SY.system_id
    AND SY.is_enable = 1
    AND SY.is_active = 1
INNER JOIN VENDOR VE
  ON SY.vendor_id = VE.vendor_id
    AND VE.is_enable = 1
    AND VE.is_active = 1
WHERE SA.is_enable = 1
  AND SA.is_active = 1
  AND SA.system_id = /*ds systemId*/'1' 
";
                }
                else
                {
                    sql = @"
SELECT
	SA.admin_secret  AS AdminSecret
FROM       [SystemAdmin]     AS SA
INNER JOIN [System]     AS SY
  ON SA.system_id = SY.system_id
    AND SY.is_enable = 1
    AND SY.is_active = 1
INNER JOIN [Vendor]     AS VE
  ON SY.vendor_id = VE.vendor_id
    AND VE.is_enable = 1
    AND VE.is_active = 1
WHERE SA.is_enable = 1
  AND SA.is_active = 1
  AND SA.system_id = @systemId
";
                }
                var param = new { systemId = systemId.Value };
                var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
                var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
                var adminSecret = Cache.Get<string>(
                    cacheKey,
                    CacheExpirationTimeSpan,
                    () => DbConnection.QuerySingleOrDefault<string>(twowaySql.Sql, dynParams));
                if (string.IsNullOrEmpty(adminSecret))
                {
                    return new AdminAuthResult(false);
                }
                verifyKey = adminSecret;
            }

            return new AdminAuthResult(key == verifyKey);
        }

        /// <summary>
        /// ファンクションを更新する
        /// </summary>
        /// <param name="function">ファンクション情報</param>
        /// <returns>ファンクションID</returns>
        public async Task<FunctionId> MergeFunction(Function function)
        {
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            string updateSql;
            string insertSql;
            if (dbSettings.Type == "Oracle")
            {
                updateSql = @"
UPDATE FUNC SET
	function_name   = /*ds FunctionName*/'1' ,
	function_detail = /*ds FunctionDetail*/'1' ,
	upd_date        = SYS_EXTRACT_UTC(SYSTIMESTAMP),
	upd_username    = /*ds UserId*/'1' 
WHERE function_id = /*ds FunctionId*/'1' 
";
                insertSql = @"
INSERT INTO FUNC
(function_id
,function_name
,function_detail
,is_enable
,reg_date
,reg_username
,upd_date
,upd_username
,is_active)
VALUES
(NEWID()
,/*ds FunctionName*/'1' 
,/*ds FunctionDetail*/'1' 
,/*ds IsEnable*/1 
,SYS_EXTRACT_UTC(SYSTIMESTAMP)
,/*ds UserId*/'1' 
,SYS_EXTRACT_UTC(SYSTIMESTAMP)
,/*ds UserId*/'1' 
,1)
";
            }
            else
            {
                updateSql = @"
UPDATE [Func] SET
	function_name   = @FunctionName,
	function_detail = @FunctionDetail,
	upd_date        = GETUTCDATE(),
	upd_username    = @UserId
WHERE function_id = @FunctionId
";
                insertSql = @"
INSERT INTO [Func]
(function_id
,function_name
,function_detail
,is_enable
,reg_date
,reg_username
,upd_date
,upd_username
,is_active)
VALUES
(NEWID()
,@FunctionName
,@FunctionDetail
,@IsEnable
,GETUTCDATE()
,@UserId
,GETUTCDATE()
,@UserId
,1)
";
            }

            var sql = function.FunctionId == null
                ? insertSql
                : updateSql;

            var param = new
            {
                FunctionId = function.FunctionId,
                FunctionName = function.FunctionName,
                FunctionDetail = function.FunctionDetail,
                IsEnable = function.IsEnable,
                UserId = base.PerRequestDataContainer.OpenId
            };

            var twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            var dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var result = DbConnection.Execute(twowaySql.Sql, dynParams);

            if (dbSettings.Type == "Oracle")
            {
                sql = @"
SELECT function_id FROM FUNC WHERE function_name = /*ds FunctionName*/'1' 
";

            }
            else
            {
                updateSql = @"
SELECT function_id FROM [Func] WHERE function_name = @FunctionName ;
";
            }
            twowaySql = new TwowaySqlParser(dbSettings.GetDbType(), sql, param);
            dynParams = dbSettings.GetParameters().AddDynamicParams(param);
            var funcId = await DbConnection.Connection.ExecuteScalarAsync<Guid>(twowaySql.Sql, dynParams);

            return new FunctionId(funcId.ToString());
        }

        private string GetConst(string key)
        {
            //リポジトリグループ、セカンダリリポジトリグループのPhysicalRepositoryを軸に取得する
            var dbSettings = UnityCore.Resolve<Com.Settings.DatabaseSettings>();
            var sql = "";
            if (dbSettings.Type == "Oracle")
            {
                sql = string.Format(@"-- AuthenticationRepository Const
SELECT const_value FROM CONST WHERE const_key= /*ds key*/'1' 
");
            }
            else
            {
                sql = string.Format(@"-- AuthenticationRepository Const
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
MERGE INTO CONST a
    USING (SELECT /*ds const_key*/'1' const_key, /*ds const_value*/'1' const_value FROM DUAL) b
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
    }
}