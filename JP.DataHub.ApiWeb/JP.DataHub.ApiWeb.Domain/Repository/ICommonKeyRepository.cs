using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    /// <summary>
    /// 共通鍵のリポジトリのインターフェースです。
    /// </summary>
    interface ICommonKeyRepository
    {
        /// <summary>
        /// 指定されたシステムID、共通鍵IDに対応する共通鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <returns>共通鍵情報</returns>
        CommonKey Get(SystemId systemId, CommonKeyId commonKeyId);

        /// <summary>
        /// 共通鍵を登録します。
        /// </summary>
        /// <param name="commonKey">共通鍵情報</param>
        void Register(CommonKey commonKey);
    }
}
