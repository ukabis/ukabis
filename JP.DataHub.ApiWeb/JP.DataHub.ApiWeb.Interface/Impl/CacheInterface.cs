using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    internal class CacheInterface : ICacheInterface
    {
        private ICacheApplicationService _cacheApplicationService = UnityCore.Resolve<ICacheApplicationService>();

        public bool IsStaticCache => _cacheApplicationService.IsStaticCache;

        public void RefreshStaticCache() => _cacheApplicationService.RefreshStaticCache();
        public void CheckStaticCacheTime() => _cacheApplicationService.CheckStaticCacheTime();
    }
}
