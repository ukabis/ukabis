using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    /// <summary>
    /// データの暗号化の管理機能のインターフェースの実装です。
    /// </summary>
    class CryptographyManagementInterface : ICryptographyManagementInterface
    {
        private ICryptographyManagementApplicationService _service = UnityCore.Resolve<ICryptographyManagementApplicationService>();


        /// <summary>
        /// 指定されたシステムIDに対応する秘密鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        public async Task<byte[]> Decrypt(string systemId, byte[] encryptData)
            => (await _service.Decrypt(new SystemId(systemId), new EncryptData(encryptData)))?.Content;

        /// <summary>
        /// 指定されたシステムIDに対応する公開鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        public async Task<PublicKeyModel> GetPublicKey(string systemId)
        {
            PublicKeyModel result = null;
            var publicKey = await _service.GetPublicKey(new SystemId(systemId));

            if (publicKey != null)
            {
                // 返却データを作成
                result = new PublicKeyModel
                {
                    Exponent = publicKey.Parameters.Exponent,
                    Modulus = publicKey.Parameters.Modulus
                };
            }

            return result;
        }

        /// <summary>
        /// 共通鍵を登録します。
        /// 共通鍵IDと暗号化に必要なパラメータを付加した共通鍵の情報を返します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="commonKey">キーを含む共通鍵の情報</param>
        /// <returns>暗号化パラメータを付加した共通鍵の情報</returns>
        public CommonKeyModel RegisterCommonKey(string systemId, CommonKeyModel commonKey)
        {
            CommonKeyModel result = null;

            // 共通鍵パラメーター作成
            var parameters = new CommonKeyParameters(commonKey.Key, commonKey.IV,
                commonKey.KeySize, commonKey.CipherMode, commonKey.PaddingMode);

            // 共通鍵を登録
            var newCommonKey = CommonKey.CreateNewCommonKey(systemId, commonKey.ExpirationDate, parameters);
            var registeredCommonKey = _service.RegisterCommonKey(newCommonKey);

            if (registeredCommonKey != null)
            {
                // 返却データを作成
                result = new CommonKeyModel
                {
                    CommonKeyId = registeredCommonKey.CommonKeyId.Value,
                    ExpirationDate = registeredCommonKey.ExpirationDate.Value,
                    IV = registeredCommonKey.Parameters.IV,
                    KeySize = registeredCommonKey.Parameters.KeySize,
                    CipherMode = registeredCommonKey.Parameters.CipherMode,
                    PaddingMode = registeredCommonKey.Parameters.PaddingMode
                };
            }

            return result;
        }
    }
}
