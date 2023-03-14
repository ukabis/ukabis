using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Consts;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    // .NET6
    [Log]
    internal interface IVendorSystemAccessTokenClientRepository
    {
        /// <summary>
        /// 指定されたClientIdのClient情報を取得します。
        /// </summary>
        /// <param name="clientId">ClientId</param>
        /// <returns>Client情報</returns>
        [CacheArg("clientId")]
        [CacheEntity(CacheConst.CACHE_ENTITY_CLIENT, CacheConst.CACHE_ENTITY_VENDOR, CacheConst.CACHE_ENTITY_SYSTEM)]
        [Cache("AccessToken", "AccessTokenAuth")]
        IEnumerable<ClientVendorSystem> GetByClientId(ClientId clientId);
    }
}
