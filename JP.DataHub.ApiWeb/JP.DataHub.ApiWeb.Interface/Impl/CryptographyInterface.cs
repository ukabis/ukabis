using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Interface;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    /// <summary>
    /// データの暗号化機能のインターフェースの実装です。
    /// </summary>
    class CryptographyInterface : ICryptographyInterface
    {
        private ICryptographyApplicationService _service = UnityCore.Resolve<ICryptographyApplicationService>();


        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        public byte[] Decrypt(string systemId, string commonKeyId, byte[] encryptData)
            => _service.Decrypt(new SystemId(systemId), new CommonKeyId(commonKeyId), new EncryptData(encryptData)).Content;

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainData">平文データ</param>
        /// <returns>暗号化データ</returns>
        public byte[] Encrypt(string systemId, string commonKeyId, byte[] plainData)
            => _service.Encrypt(new SystemId(systemId), new CommonKeyId(commonKeyId), new PlainData(plainData)).Content;

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で暗号化データを復号するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="encryptStream">暗号化データストリーム</param>
        /// <returns>平文データストリーム</returns>
        public Stream GetDecryptStream(string systemId, string commonKeyId, Stream encryptStream)
            => _service.GetDecryptStream(new SystemId(systemId), new CommonKeyId(commonKeyId), new EncryptStream(encryptStream)).Value;

        /// <summary>
        /// 指定された共通鍵IDに対応する共通鍵で平文データを暗号化するストリームを取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKeyId">共通鍵ID</param>
        /// <param name="plainStream">平文データストリーム</param>
        /// <returns>暗号化データストリーム</returns>
        public Stream GetEncryptStream(string systemId, string commonKeyId, Stream plainStream)
            => _service.GetEncryptStream(new SystemId(systemId), new CommonKeyId(commonKeyId), new PlainStream(plainStream)).Value;
    }
}
