using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// キャッシュのためのヘルパークラス
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public class CacheHelper
    {
        private static readonly int DEFAULT_CACHE_EXPIRATION_MAX_SECOND = 86400;
        private static readonly int DEFAULT_CACHE_EXPIRATION_SECOND = 1800;
        private static readonly int DEFAULT_CACHE_KEY_MAX_LENGTH = 1000;
        private static readonly int DEFAULT_CACHE_VALUE_MAX_SIZE = 1048576;

        private static IConfigurationSection s_appconfig = UnityCore.Resolve<IConfiguration>().GetSection("AppConfig");
        private static readonly int s_roslynScriptCacheExpirationDefaultSecond = s_appconfig.GetValue<int>("RoslynScriptCacheExpirationDefaultSecond", DEFAULT_CACHE_EXPIRATION_SECOND);
        private static readonly int s_roslynScriptCacheExpirationMaxSecond = s_appconfig.GetValue<int>("RoslynScriptCacheExpirationMaxSecond", DEFAULT_CACHE_EXPIRATION_MAX_SECOND);
        private static readonly int s_roslynScriptCacheKeyMaxLength = s_appconfig.GetValue<int>("RoslynScriptCacheKeyMaxLength", DEFAULT_CACHE_KEY_MAX_LENGTH);
        private static readonly int s_roslynScriptCacheValueMaxSize = s_appconfig.GetValue<int>("RoslynScriptCacheValueMaxSize", DEFAULT_CACHE_VALUE_MAX_SIZE);

        private ICache _cache => UnityCore.Resolve<ICache>("RoslynCache");
        private IPerRequestDataContainer _perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
        private IDynamicApiAction _apiAction = UnityCore.Resolve<IDynamicApiDataContainer>().baseApiAction;
        private bool _isRoslynScriptCacheEnable = UnityCore.Resolve<bool>("IsRoslynScriptCacheEnable");


        private string GetKeyPrefix()
        {
            var keyPrefix = $"{_apiAction.ApiId.Value}_";

            if (_apiAction.IsVendor.Value)
            {
                keyPrefix += $"{_apiAction.VendorId.Value}_{_apiAction.SystemId.Value}_";
            }

            if (_apiAction.IsPerson.Value)
            {
                keyPrefix += $"{_perRequestDataContainer.OpenId}_";
            }

            return keyPrefix;
        }

        /// <summary>
        /// <ja>キャッシュを追加する</ja>
        /// <en>Add cache value</en>
        /// </summary>
        /// <param name="key">
        /// <ja>キー</ja>
        /// <en>Key(Identifier)</en>
        /// </param>
        /// <param name="value">
        /// <ja>格納する値(すでに値が入っている場合は上書き)</ja>
        /// <en>Value to store(Overwrite if value already exists)</en>
        /// </param>
        /// <returns>
        /// </returns>
        public void Add(string key, object value) => Add(key, value, new TimeSpan(0, 0, s_roslynScriptCacheExpirationDefaultSecond));

        /// <summary>
        /// <ja>キャッシュを追加する</ja>
        /// <en>Add cache value</en>
        /// </summary>
        /// <param name="key">
        /// <ja>キー</ja>
        /// <en>Key(Identifier)</en>
        /// </param>
        /// <param name="value">
        /// <ja>格納する値(すでに値が入っている場合は上書き)</ja>
        /// <en>Value to store(Overwrite if value already exists)</en>
        /// </param>
        /// <param name="expiration">
        /// <ja>有効期限</ja>
        /// <en>Expiration</en>
        /// </param>
        /// <returns>
        /// </returns>
        public void Add(string key, object value, TimeSpan expiration)
        {
            if (!_isRoslynScriptCacheEnable)
            {
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (key.Length > s_roslynScriptCacheKeyMaxLength)
            {
                throw new Exception($"{nameof(key)} length should not be more than {s_roslynScriptCacheKeyMaxLength}");
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (TimeSpan.Compare(expiration, new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond)) == 1)
            {
                expiration = new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond);
            }

            if (_cache.Contains(GetKeyPrefix() + key))
            {
                _cache.Remove(GetKeyPrefix() + key);
            }

            _cache.Add(GetKeyPrefix() + key, value, expiration, s_roslynScriptCacheValueMaxSize);
        }

        /// <summary>
        /// <ja>キャッシュを取得する</ja>
        /// <en>Get Cache Value</en>
        /// </summary>
        /// <param name="key">
        /// <ja>キー</ja>
        /// <en>Key(Identifier)</en>
        /// </param>
        /// <return>
        /// <ja>格納されている値(ミスヒットの場合はNullを返す)</ja>
        /// <en>Stored value(Return Null if key does not hit)</en>
        /// </return>
        public T Get<T>(string key)
        {
            if (!_isRoslynScriptCacheEnable)
            {
                return default(T);
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            return _cache.Get<T>(GetKeyPrefix() + key, out bool isNullValue);
        }

        /// <summary>
        /// <ja>キャッシュを取得する(ミスヒットした場合関数を実行して戻り値をキャッシュに入れReturnする)</ja>
        /// <en>Get Cache Value</en>
        /// </summary>
        /// <param name="key">
        /// <ja>キー</ja>
        /// <en>Key(Identifier)</en>
        /// </param>
        /// <param name="expiration">
        /// <ja>有効期限</ja>
        /// <en>Expiration</en>
        /// </param>
        /// <param name="misshitAction">
        /// <ja>ミスヒットした場合処理する関数</ja>
        /// <en>Function to be called if key does not hit</en>
        /// </param>
        /// <return>
        /// <ja>格納されている値(ミスヒットの場合関数の実行結果を返す)</ja>
        /// <en>Stored value(Return Null if key does not hit)</en>
        /// </return>
        public T GetOrAdd<T>(string key, TimeSpan expiration, Func<T> misshitAction)
        {
            if (!_isRoslynScriptCacheEnable)
            {
                return default(T);
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (TimeSpan.Compare(expiration, new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond)) == 1)
            {
                expiration = new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond);
            }

            if (key.Length > s_roslynScriptCacheKeyMaxLength)
            {
                throw new Exception($"{nameof(key)} length should not be more than {s_roslynScriptCacheKeyMaxLength}");
            }

            var cacheValue = _cache.Get<T>(GetKeyPrefix() + key, out bool isNullValue);

            if (EqualityComparer<T>.Default.Equals(cacheValue, default(T)))
            {
                var returnValue = misshitAction.Invoke();

                _cache.Add(GetKeyPrefix() + key, returnValue, expiration, s_roslynScriptCacheValueMaxSize);

                return returnValue;
            }

            return cacheValue;
        }

        /// <summary>
        /// <ja>キャッシュを削除する</ja>
        /// <en></en>
        /// </summary>
        /// <param name="key">
        /// <ja>キー</ja>
        /// <en>Key(Identifier)</en>
        /// </param>
        /// <returns>
        /// </returns>
        public void Remove(string key)
        {
            if (!_isRoslynScriptCacheEnable)
            {
                return;
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            _cache.Remove(GetKeyPrefix() + key);
        }
    }
}