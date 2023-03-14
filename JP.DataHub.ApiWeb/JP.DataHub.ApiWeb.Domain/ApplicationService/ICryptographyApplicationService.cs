using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService
{
    /// <summary>
    /// データの暗号化機能を提供するアプリケーションサービスのインターフェースです。
    /// </summary>
    interface ICryptographyApplicationService
    {
        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        PlainData Decrypt(SystemId systemId, CommonKeyId commonKeyId, EncryptData encryptData);

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainData">平文データ</param>
        /// <returns>暗号化データ</returns>
        EncryptData Encrypt(SystemId systemId, CommonKeyId commonKeyId, PlainData plainData);

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptStream">暗号化データストリーム</param>
        /// <returns>平文データストリーム</returns>
        PlainStream GetDecryptStream(SystemId systemId, CommonKeyId commonKeyId, EncryptStream encryptStream);

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainStream">平文データストリーム</param>
        /// <returns>暗号化データストリーム</returns>
        EncryptStream GetEncryptStream(SystemId systemId, CommonKeyId commonKeyId, PlainStream plainStream);
    }
}
