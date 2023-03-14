using JP.DataHub.Api.Core.Filters;
using JP.DataHub.Api.Core.Filters.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service;
using Microsoft.AspNetCore.Mvc;
using System;

namespace JP.DataHub.ManageApi.Controllers
{
    [Admin]
    [ApiController]
    [Route("Manage/[controller]/[action]")]
    [ManageApi("C3CB9551-C0E2-45D4-ACF3-AB7B1047FC53")]
    public class CacheController : AbstractController
    {
        private Lazy<ICacheService> _lazyCacheService = new Lazy<ICacheService>(() => UnityCore.Resolve<ICacheService>());
        private ICacheService _cacheService { get => _lazyCacheService.Value; }

        /// <summary>
        /// キャッシュ削除（Entity指定）
        /// </summary>
        /// <param name="entity">キャッシュから削除するEntity名</param>
        [HttpDelete]
        [ManageAction("2BE85B41-3FE7-4B04-86B4-D773AE612EE2")]
        public void ClearByEntity(string entity = null)
        {
            _cacheService.ClearEntityCache(entity);
        }

        /// <summary>
        /// キャッシュ削除（Id指定）
        /// </summary>
        /// <param name="cacheId">キャッシュから削除するId名</param>
        [HttpDelete]
        [ManageAction("D6885C2E-2C3C-412F-A2AB-5E3BFDD9A8E3")]
        public void ClearById(string cacheId = null)
        {
            _cacheService.ClearIdCache(cacheId);
        }

        /// <summary>
        /// OpenIdキャッシュの削除
        /// </summary>
        /// <param name="openId">OpenId（GUID形式）</param>
        [HttpDelete]
        [ManageAction("8AAAFFCF-012E-451E-9A1E-93ECD9010554")]
        [Admin]
        public void ClearOpenIdCache(string openId = null)
        {
            _cacheService.ClearOpenIdCache(openId);
        }

        /// <summary>
        /// OpenId無効リストキャッシュの削除
        /// </summary>
        /// <param name="openId">OpenId（GUID形式）</param>
        [HttpDelete]
        [ManageAction("73587FAD-A287-42E8-BB34-67F8818465F5")]
        [Admin]
        public void ClearInvalidOpenIdCache(string openId = null)
        {
            _cacheService.ClearInvalidOpenIdCache(openId);
        }

    }
}