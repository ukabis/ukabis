using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    /// <summary>
    /// データの暗号化の管理機能を提供するアプリケーションサービスのインターフェースです。
    /// </summary>
    interface ICryptographyManagementApplicationService
    {
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
        Task<PublicKey> GetPublicKey(SystemId systemId);

        /// <summary>
        /// 共通鍵を登録します。
        /// 共通鍵IDと暗号化に必要なパラメータを付加した共通鍵の情報を返します。
        /// </summary>
        /// <param name="commonKey">キーを含む共通鍵の情報</param>
        /// <returns>暗号化パラメータを付加した共通鍵の情報</returns>
        CommonKey RegisterCommonKey(CommonKey commonKey);
    }
}
