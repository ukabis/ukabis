using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    /// <summary>
    /// 非対称鍵のリポジトリのインターフェースです。
    /// </summary>
    interface IAsymmetricKeyRepository
    {
        /// <summary>
        /// 指定されたシステムIDに対応するRSAキーペアを作成します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        Task<PublicKey> Create(SystemId systemId);

        /// <summary>
        /// 指定されたシステムIDに対応する秘密鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        Task<PlainData> Decrypt(SystemId systemId, EncryptData encryptData);

        /// <summary>
        /// 指定されたシステムIDに対応する公開鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        Task<PublicKey> Get(SystemId systemId);
    }
}
