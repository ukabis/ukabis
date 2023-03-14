using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    /// <summary>
    /// データの暗号化の管理機能のインターフェースです。
    /// </summary>
    public interface ICryptographyManagementInterface
    {
        /// <summary>
        /// 指定されたシステムIDに対応する秘密鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        Task<byte[]> Decrypt(string systemId, byte[] encryptData);

        /// <summary>
        /// 指定されたシステムIDに対応する公開鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        Task<PublicKeyModel> GetPublicKey(string systemId);

        /// <summary>
        /// 共通鍵を登録します。
        /// 共通鍵IDと暗号化に必要なパラメータを付加した共通鍵の情報を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKey">キーを含む共通鍵の情報</param>
        /// <returns>暗号化パラメータを付加した共通鍵の情報</returns>
        CommonKeyModel RegisterCommonKey(string systemId, CommonKeyModel commonKey);
    }
}
