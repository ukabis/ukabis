using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.Com.Unity;
using Microsoft.AspNetCore.Mvc;
using System;

namespace JP.DataHub.ApiWeb.Controllers
{
    [Admin]
    [ApiController]
    [Route("Api/[controller]/[action]")]
    [ManageApi("99389C0E-AB16-45C9-85BC-6CF3664F9030")]
    public class CacheController : AbstractController
    {
        private Lazy<ICacheInterface> _lazyCacheInterface = new Lazy<ICacheInterface>(() => UnityCore.Resolve<ICacheInterface>());
        private ICacheInterface _cache { get => _lazyCacheInterface.Value; }

        /// <summary>
        /// StaticCacheのキャッシュ内容を消去する
        /// </summary>
        [HttpDelete]
        [ManageAction("D2DADBD2-10A4-4A6B-9505-7CBA55A3B7B2")]
        [Admin]
        public void ClearStaticCache()
        {
            _cache.RefreshStaticCache();
        }
    }
}