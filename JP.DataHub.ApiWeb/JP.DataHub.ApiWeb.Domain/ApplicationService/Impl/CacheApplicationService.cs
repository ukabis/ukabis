using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{

    class CacheApplicationService : ICacheApplicationService
    {
        private Lazy<IDynamicApiRepository> _lazyDynamicApiRepository = new(() => UnityCore.Resolve<IDynamicApiRepository>());
        private IDynamicApiRepository _dynamicApiRepository { get => _lazyDynamicApiRepository.Value; }
        private Lazy<IAuthenticationRepository> _lazyAuthentication = new(() => UnityCore.Resolve<IAuthenticationRepository>());
        private IAuthenticationRepository _authenticationRepository { get => _lazyAuthentication.Value; }
        protected bool IsAuthenticationStaticCache => _isAuthenticationStaticCache.Value;
        protected readonly Lazy<bool> _isAuthenticationStaticCache = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsAuthenticationStaticCache"));
        protected bool IsDynamicApiStaticCache => _isDynamicApiStaticCache.Value;
        protected readonly Lazy<bool> _isDynamicApiStaticCache = new Lazy<bool>(() => UnityCore.Resolve<bool>("IsDynamicApiStaticCache"));

        public bool IsStaticCache => (_isAuthenticationStaticCache.Value || _isDynamicApiStaticCache.Value);


        public void RefreshStaticCache()
        {
            var time = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff");

            // 待機せずバックグラウンドで実行
            _ = _authenticationRepository.RefreshStaticCache(time);
            _= _dynamicApiRepository.RefreshStaticCache(time);
        }

        public void CheckStaticCacheTime()
        {
            _authenticationRepository.CheckStaticCacheTime();
            _dynamicApiRepository.CheckStaticCacheTime();
        }
    }
}
