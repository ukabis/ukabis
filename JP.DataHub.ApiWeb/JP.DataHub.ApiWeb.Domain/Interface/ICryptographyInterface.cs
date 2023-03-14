
namespace JP.DataHub.ApiWeb.Domain.Interface
{
    /// <summary>
    /// データの暗号化機能のインターフェースです。
    /// </summary>
    public interface ICryptographyInterface
    {
        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        byte[] Decrypt(string systemId, string commonKeyId, byte[] encryptData);

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainData">平文データ</param>
        /// <returns>暗号化データ</returns>
        byte[] Encrypt(string systemId, string commonKeyId, byte[] plainData);

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptStream">暗号化データストリーム</param>
        /// <returns>平文データストリーム</returns>
        Stream GetDecryptStream(string systemId, string commonKeyId, Stream encryptStream);

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainStream">平文データストリーム</param>
        /// <returns>暗号化データストリーム</returns>
        Stream GetEncryptStream(string systemId, string commonKeyId, Stream plainStream);
    }
}
