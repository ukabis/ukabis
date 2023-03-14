// KeyClientがMockingできないため
#if false
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.KeyVault.WebKey;
using Microsoft.Practices.Unity;
using Microsoft.Rest.Azure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

using JP.DataHub.Domain.Context.Cryptography;
using JP.DataHub.Domain.Exceptions;
using JP.DataHub.Domain.Repository;
using JP.DataHub.Infrastructure.Repository;

namespace UnitTest.JP.DataHub.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_AsymmetricKeyRepository
    {
        private UnityContainer container;
        private Mock<IKeyVaultClient> mock;
        private IAsymmetricKeyRepository testClass;

        private string vaultBaseUrl = "https://jpdatahubdev.vault.azure.net:443";
        private string systemId = Guid.NewGuid().ToString();
        private string systemId2 = Guid.NewGuid().ToString();
        private string systemId3 = Guid.NewGuid().ToString();
        private string plainText = "{ \"item1\": \"暗号化テスト1234567890\" }";
        private string keyId;

        public UnitTest_AsymmetricKeyRepository()
        {
            ConfigurationManager.AppSettings["VaultBaseUrl"] = vaultBaseUrl;
            // モックのレスポンス作成
            keyId = $"{vaultBaseUrl}/keys/RsaKey-{systemId}/{Guid.NewGuid()}";
            var rsaParams = new RSAParameters { Exponent = new byte[] { 10, 20, 30 }, Modulus = new byte[] { 40, 50, 60 } };
            var keyBundle = new KeyBundle(new JsonWebKey(rsaParams) { Kid = keyId });
            var response = new AzureOperationResponse<KeyBundle> { Body = keyBundle };

            // モックの作成
            mock = new Mock<IKeyVaultClient>();
            mock.Setup(s => s.CreateKeyWithHttpMessagesAsync(It.IsAny<string>(), It.Is<string>(x => x == "RsaKey-" + systemId),
                It.IsAny<string>(), It.IsAny<int?>(),
                It.IsAny<IList<string>>(), It.IsAny<KeyAttributes>(),
                It.IsAny<IDictionary<string, string>>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            mock.Setup(s => s.CreateKeyWithHttpMessagesAsync(It.IsAny<string>(), It.Is<string>(x => x == "RsaKey-" + systemId2),
                It.IsAny<string>(), It.IsAny<int?>(),
                It.IsAny<IList<string>>(), It.IsAny<KeyAttributes>(),
                It.IsAny<IDictionary<string, string>>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Throws(new KeyVaultErrorException { Body = new KeyVaultError(new Error("OtherError")) });
            mock.Setup(s => s.GetKeyWithHttpMessagesAsync(It.IsAny<string>(), It.Is<string>(x => x == "RsaKey-" + systemId),
                It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            mock.Setup(s => s.GetKeyWithHttpMessagesAsync(It.IsAny<string>(), It.Is<string>(x => x == "RsaKey-" + systemId2),
                It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
                .Throws(new KeyVaultErrorException { Body = new KeyVaultError(new Error("KeyNotFound")) });
            mock.Setup(s => s.GetKeyWithHttpMessagesAsync(It.IsAny<string>(), It.Is<string>(x => x == "RsaKey-" + systemId3),
                It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()))
                .Throws(new KeyVaultErrorException { Body = new KeyVaultError(new Error("OtherError")) });

            // Unityの初期化
            container = new UnityContainer();
            container.RegisterType<IAsymmetricKeyRepository, AsymmetricKeyRepository>();
            container.RegisterInstance(mock.Object);

            // テスト対象のインスタンスを作成
            testClass = container.Resolve<IAsymmetricKeyRepository>();
        }

        [TestMethod]
        public async Task Create()
        {
            var publicKey = new PublicKey(keyId, new SystemId(systemId), new byte[] { 0, 10, 20, 30 }, new byte[] { 40, 50, 60 });

            // テスト対象のメソッド実行
            var result = await testClass.Create(new SystemId(systemId));

            // モックの呼び出しを検証
            mock.Verify(s => s.CreateKeyWithHttpMessagesAsync(
                It.Is<string>(x => x == vaultBaseUrl),
                It.Is<string>(x => x == "RsaKey-" + systemId),
                It.Is<string>(x => x == JsonWebKeyType.Rsa),
                It.IsAny<int?>(),
                It.IsAny<IList<string>>(), It.IsAny<KeyAttributes>(),
                It.IsAny<IDictionary<string, string>>(), It.IsAny<string>(),
                It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()));

            // 結果をチェック
            result.IsStructuralEqual(publicKey);
        }

        [TestMethod]
        [ExpectedException(typeof(AsymmetricKeyOperationException))]
        public async Task Create_Fail()
        {
            // テスト対象のメソッド実行
            var result = await testClass.Create(new SystemId(systemId2));
        }

        public async Task Decrypt()
        {
            // DecryptAsyncが拡張メソッドのためモックが設定できない

            // テスト対象のメソッド実行
            var result = await testClass.Decrypt(new SystemId(systemId), new EncryptData(new byte[] { 10, 20, 30 }));

            // 結果をチェック
            Encoding.UTF8.GetString(result.Content).Is(plainText);
        }

        [TestMethod]
        public async Task Get()
        {
            var publicKey = new PublicKey(keyId, new SystemId(systemId), new byte[] { 0, 10, 20, 30 }, new byte[] { 40, 50, 60 });

            // テスト対象のメソッド実行
            var result = await testClass.Get(new SystemId(systemId));

            // モックの呼び出しを検証
            mock.Verify(s => s.GetKeyWithHttpMessagesAsync(
                It.Is<string>(x => x == vaultBaseUrl),
                It.Is<string>(x => x == "RsaKey-" + systemId),
                It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()));

            // 結果をチェック
            result.IsStructuralEqual(publicKey);
        }

        [TestMethod]
        public async Task Get_NotFound()
        {
            // テスト対象のメソッド実行＆結果をチェック
            var result = await testClass.Get(new SystemId(systemId2));

            // モックの呼び出しを検証
            mock.Verify(s => s.GetKeyWithHttpMessagesAsync(
                It.Is<string>(x => x == vaultBaseUrl),
                It.Is<string>(x => x == "RsaKey-" + systemId2),
                It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(),
                It.IsAny<CancellationToken>()));

            // 結果をチェック
            result.IsNull();
        }

        [TestMethod]
        [ExpectedException(typeof(AsymmetricKeyOperationException))]
        public async Task Get_Fail()
        {
            // テスト対象のメソッド実行
            var result = await testClass.Get(new SystemId(systemId3));
        }
    }
}
#endif