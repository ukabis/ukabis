using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Model;
using JP.DataHub.ManageApi.Service.Repository;
using Microsoft.Extensions.Configuration;

namespace JP.DataHub.ManageApi.Service.Impl
{
    internal class CacheService : AbstractService, ICacheService
    {
        private Lazy<ICacheManager> _lazyCacheManager = new Lazy<ICacheManager>(() => UnityCore.Resolve<ICacheManager>());
        private ICacheManager _cacheManager { get => _lazyCacheManager.Value; }
        private Lazy<ICache> _lazyDynamicApiCache = new Lazy<ICache>(() => UnityCore.Resolve<ICache>("DynamicApi"));
        private ICache _dynamicApiCache { get => _lazyDynamicApiCache.Value; }
        private Lazy<ICache> _lazyOpenIdTokenCache = new Lazy<ICache>(() => UnityCore.Resolve<IConfiguration>().GetValue<bool>("AppConfig:EnableOpenidTokenCache", false) ? 
        UnityCore.Resolve<ICache>("OpenIdTokenCache") : UnityCore.Resolve<ICache>());
        private ICache _openIdTokenCache { get => _lazyOpenIdTokenCache.Value; }

        public void ClearEntityCache(string entity) {
            _cacheManager.FireEntity(entity);
            _cacheManager.FireEntity(_dynamicApiCache,entity);;
        }
        public void ClearIdCache(string id) {
            _cacheManager.FireId(id,"");
            _cacheManager.FireId(_dynamicApiCache,id, "");
        }
        public void ClearOpenIdCache(string id) {
            var idToSignatureKey = CacheManager.CreateKey("Global - AuthenticateRequest - OidToSignature", id);
            var signature = _openIdTokenCache.Get<Tuple<string, DateTime>>(idToSignatureKey, out bool isNullValue);
            if (string.IsNullOrEmpty(signature?.Item1) == false)
            {
                //ここで削除したトークンを受け付けた場合は認証不可とするため 無効化リストとしてキャッシュに登録する(すでに有効期限切れを除く)
                var removedTokenExpire = signature.Item2 - DateTime.Now.ToUniversalTime();
                if (removedTokenExpire.Milliseconds > 0)
                {
                    _openIdTokenCache.Add(CacheManager.CreateKey("Global - AuthenticateRequest - InvalidSignature", id, signature.Item1), signature.Item1, removedTokenExpire);
                }

                _openIdTokenCache.Remove(CacheManager.CreateKey("Global - AuthenticateRequest", signature));
                _openIdTokenCache.Remove(idToSignatureKey);
            }
        }
        public void ClearInvalidOpenIdCache(string openId) {
            if (string.IsNullOrEmpty(openId))
            {
                return;
            }
            _openIdTokenCache.RemoveFirstMatch(CacheManager.CreateKey("Global - AuthenticateRequest - InvalidSignature", openId));
        }
    }
}
