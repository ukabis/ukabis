using System.Security.Cryptography;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Keys.Cryptography;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Repository;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    /// <summary>
    /// 非対称鍵のリポジトリです。
    /// </summary>
    class AsymmetricKeyRepository : IAsymmetricKeyRepository
    {
        private static readonly IConfiguration s_configuration = UnityCore.Resolve<IConfiguration>();

        private static string KeyVaultUri => s_configuration.GetValue<string>("KeyVault:Uri");
        private static string TenantId => s_configuration.GetValue<string>("KeyVault:TenantId");
        private static string ClientId => s_configuration.GetValue<string>("KeyVault:ClientId");
        private static string ClientSecret => s_configuration.GetValue<string>("KeyVault:ClientSecret");

        private Lazy<KeyClient> _lazyKeyClient = new Lazy<KeyClient>(() =>
        {
            var credential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
            return new KeyClient(new Uri(KeyVaultUri), credential);
        });
        private KeyClient KeyClient => _lazyKeyClient.Value;


        /// <summary>
        /// 指定されたシステムIDに対応するRSAキーペアを作成します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        public async Task<PublicKey> Create(SystemId systemId)
        {
            try
            {
                // キーペアを作成
                var result = await KeyClient.CreateKeyAsync(GetKeyName(systemId), KeyType.Rsa);

                // 公開鍵情報を作成
                return CreatePublicKey(systemId, result.Value);
            }
            catch (RequestFailedException ex)
            {
                throw new AsymmetricKeyOperationException(ex.Message);
            }
        }

        /// <summary>
        /// 指定されたシステムIDに対応する秘密鍵で暗号化データを復号します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <param name="encryptData">暗号化データ</param>
        /// <returns>平文データ</returns>
        public async Task<PlainData> Decrypt(SystemId systemId, EncryptData encryptData)
        {
            try
            {
                // 公開鍵を取得
                var result = await KeyClient.GetKeyAsync(GetKeyName(systemId));
                var rsa = result.Value.Key.ToRSA();

                // 秘密鍵で復号
                var cryptoClient = CreateCryptographyClient(result.Value.Id);
                var decryptResult = await cryptoClient.DecryptAsync(EncryptionAlgorithm.RsaOaep, encryptData.Content);
                return new PlainData(decryptResult.Plaintext);
            }
            catch (RequestFailedException ex)
            {
                // 存在しない場合はnullを返却する
                if (ex.ErrorCode == "KeyNotFound")
                {
                    return null;
                }
                else
                {
                    throw new AsymmetricKeyOperationException(ex.Message);
                }
            }
        }

        /// <summary>
        /// 指定されたシステムIDに対応する公開鍵を取得します。
        /// </summary>
        /// <param name="systemId">システムID</param>
        /// <returns>公開鍵情報</returns>
        public async Task<PublicKey> Get(SystemId systemId)
        {
            try
            {
                // 公開鍵を取得
                var result = await KeyClient.GetKeyAsync(GetKeyName(systemId) + "x");

                // 公開鍵情報を作成
                return CreatePublicKey(systemId, result.Value);
            }
            catch (RequestFailedException ex)
            {
                // 存在しない場合はnullを返却する
                if (ex.ErrorCode == "KeyNotFound")
                {
                    return null;
                }
                else
                {
                    throw new AsymmetricKeyOperationException(ex.Message);
                }
            }
        }


        private string GetKeyName(SystemId systemId) => "RsaKey-" + systemId.Value;

        private PublicKey CreatePublicKey(SystemId systemId, KeyVaultKey keyVaultKey)
        {
            var rsa = keyVaultKey.Key.ToRSA();
            var rsaParams = rsa.ExportParameters(false);
            return new PublicKey(keyVaultKey.Key.Id, systemId, rsaParams.Exponent, rsaParams.Modulus);
        }


        private CryptographyClient CreateCryptographyClient(Uri keyId)
        {
            var credential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
            return new CryptographyClient(keyId, credential);
        }
    }
}