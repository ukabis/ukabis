using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    /// <summary>
    /// 共通鍵のリポジトリです。
    /// </summary>
    class CommonKeyRepository : ICommonKeyRepository
    {
        private static readonly string s_cacheKey = typeof(CommonKey).Namespace + ":" + nameof(CommonKey);

        private ICache Cache = UnityCore.Resolve<ICache>();


        /// <summary>
        /// 指定されたシステムID、共通鍵IDに対応する共通鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <returns>共通鍵情報</returns>
        public CommonKey Get(SystemId systemId, CommonKeyId commonKeyId)
        {
            // キャッシュから共通鍵情報を取得
            return Cache.Get<CommonKey>($"{s_cacheKey}:{systemId.Value}:{commonKeyId.Value}", out bool isNullValue);
        }

        /// <summary>
        /// 共通鍵を登録します。
        /// </summary>
        /// <param name="commonKey">共通鍵情報</param>
        public void Register(CommonKey commonKey)
        {
            // 共通鍵情報をキャッシュに登録
            Cache.Add($"{s_cacheKey}:{commonKey.SystemId.Value}:{commonKey.CommonKeyId.Value}",
                commonKey, commonKey.GetExpirationTimeSpanFromNow());
        }
    }
}
