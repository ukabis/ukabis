using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.ApplicationService.Impl;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_CryptographyManagementApplicationService : UnitTestBase
    {
        private UnityContainer container;
        private Mock<IAsymmetricKeyRepository> mockRepo1;
        private Mock<ICommonKeyRepository> mockRepo2;
        private ICryptographyManagementApplicationService testClass;

        private string systemId = Guid.NewGuid().ToString();
        private string systemId2 = Guid.NewGuid().ToString();
        private byte[] plainData = Encoding.UTF8.GetBytes("plainData");
        private PublicKey publicKey;


        public UnitTest_CryptographyManagementApplicationService()
        {
            publicKey = new PublicKey(Guid.NewGuid().ToString(), new SystemId(systemId), new byte[] { 10, 20, 30 }, new byte[] { 40, 50, 60 });

            // モックの作成
            mockRepo1 = new Mock<IAsymmetricKeyRepository>();
            mockRepo1.Setup(s => s.Get(It.Is<SystemId>(x => x.Value == systemId))).ReturnsAsync(publicKey);
            mockRepo1.Setup(s => s.Create(It.IsAny<SystemId>())).ReturnsAsync(publicKey);
            mockRepo1.Setup(s => s.Decrypt(It.IsAny<SystemId>(), It.IsAny<EncryptData>())).ReturnsAsync(new PlainData(plainData));

            mockRepo2 = new Mock<ICommonKeyRepository>();

            // Unityの初期化
            container = new UnityContainer();
            container.RegisterType<ICryptographyManagementApplicationService, CryptographyManagementApplicationService>();
            container.RegisterInstance(mockRepo1.Object);
            container.RegisterInstance(mockRepo2.Object);
            UnityCore.UnityContainer = container;

            // テスト対象のインスタンスを作成
            testClass = container.Resolve<ICryptographyManagementApplicationService>();
        }

        [TestMethod]
        public async Task Decrypt()
        {
            byte[] data = Encoding.UTF8.GetBytes("testData");

            // テスト対象のメソッド実行
            var result = await testClass.Decrypt(new SystemId(systemId), new EncryptData(data));

            // モックの呼び出しを検証
            mockRepo1.Verify(s => s.Decrypt(
                It.Is<SystemId>(x => x.Value == systemId),
                It.Is<EncryptData>(x => x.Content.SequenceEqual(data))));

            // 結果をチェック
            result.Content.Is(plainData);
        }

        [TestMethod]
        public async Task GetPublicKey()
        {
            string systemId2 = Guid.NewGuid().ToString();

            // テスト対象のメソッド実行
            var result = await testClass.GetPublicKey(new SystemId(systemId));

            // モックの呼び出しを検証
            mockRepo1.Verify(s => s.Get(It.Is<SystemId>(x => x.Value == systemId)));

            // 結果をチェック
            result.IsStructuralEqual(publicKey);
        }

        [TestMethod]
        public async Task GetPublicKey_Create()
        {
            // テスト対象のメソッド実行
            var result = await testClass.GetPublicKey(new SystemId(systemId2));

            // モックの呼び出しを検証
            mockRepo1.Verify(s => s.Get(It.Is<SystemId>(x => x.Value == systemId2)));
            mockRepo1.Verify(s => s.Create(It.Is<SystemId>(x => x.Value == systemId2)));

            // 結果をチェック
            result.IsStructuralEqual(publicKey);
        }

        [TestMethod]
        public void RegisterCommonKey()
        {
            // 登録データ作成
            byte[] key = Encoding.UTF8.GetBytes("testKey");
            var parameters = new CommonKeyParameters(key, null, 128, CipherMode.CBC, PaddingMode.PKCS7);
            var commonKey = CommonKey.CreateNewCommonKey(systemId, null, parameters);

            // テスト対象のメソッド実行
            var result = testClass.RegisterCommonKey(commonKey);

            // モックの呼び出しを検証
            mockRepo2.Verify(s => s.Register(It.Is<CommonKey>(x => x == commonKey)));

            // 結果をチェック
            result.IsStructuralEqual(commonKey);
        }
    }
}
