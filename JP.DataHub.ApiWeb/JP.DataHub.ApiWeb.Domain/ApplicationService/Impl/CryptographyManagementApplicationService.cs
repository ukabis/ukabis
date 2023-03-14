using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Unity.Attributes;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    /// <summary>
    /// データの暗号化の管理機能を提供するアプリケーションサービスです。
    /// </summary>
    [Log]
    class CryptographyManagementApplicationService : ICryptographyManagementApplicationService
    {
        private IAsymmetricKeyRepository AsymmetricKeyRepository = UnityCore.Resolve<IAsymmetricKeyRepository>();
        private ICommonKeyRepository CommonKeyRepository = UnityCore.Resolve<ICommonKeyRepository>();


        /// <summary>
        /// 指定されたシステムIDに対応する秘密鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        public async Task<PlainData> Decrypt(SystemId systemId, EncryptData encryptData)
            => await AsymmetricKeyRepository.Decrypt(systemId, encryptData);

        /// <summary>
        /// 指定されたシステムIDに対応する公開鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        public async Task<PublicKey> GetPublicKey(SystemId systemId)
        {
            // リポジトリから公開鍵情報を取得
            var publicKey = await AsymmetricKeyRepository.Get(systemId);
            // 存在しなければRSAキーペアを作成
            if (publicKey == null)
            {
                publicKey = await AsymmetricKeyRepository.Create(systemId);
            }

            return publicKey;
        }

        /// <summary>
        /// 共通鍵を登録します。
        /// 共通鍵IDと暗号化に必要なパラメータを付加した共通鍵の情報を返します。
        /// </summary>
        /// <param name="commonKey">キーを含む共通鍵の情報</param>
        /// <returns>暗号化パラメータを付加した共通鍵の情報</returns>
        public CommonKey RegisterCommonKey(CommonKey commonKey)
        {
            // 共通鍵を登録
            CommonKeyRepository.Register(commonKey);
            return commonKey;
        }
    }
}
