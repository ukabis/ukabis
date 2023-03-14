using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.ApplicationService.Impl;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_CryptographyApplicationService : UnitTestBase
    {
        private UnityContainer container;
        private Mock<ICommonKeyRepository> mockRepo;
        private ICryptographyApplicationService testClass;

        private string systemId = Guid.NewGuid().ToString();
        private string systemId2 = Guid.NewGuid().ToString();
        private string commonKeyId = Guid.NewGuid().ToString();
        private PlainData plainData;

        public UnitTest_CryptographyApplicationService()
        {
            string testData = "{ \"item1\": \"暗号化テスト1234567890\" }";
            plainData = new PlainData(Encoding.UTF8.GetBytes(testData));
            var commonKey = CommonKey.CreateNewCommonKey(systemId, null, CreateCommonKeyParameters());

            // モックの作成
            mockRepo = new Mock<ICommonKeyRepository>();
            mockRepo.Setup(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)
                )).Returns(commonKey);

            // Unityの初期化
            container = new UnityContainer();
            container.RegisterType<ICryptographyApplicationService, CryptographyApplicationService>();
            container.RegisterInstance(mockRepo.Object);
            UnityCore.UnityContainer = container;

            // テスト対象のインスタンスを作成
            testClass = container.Resolve<ICryptographyApplicationService>();
        }

        [TestMethod]
        public void EncryptDecrypt()
        {
            // テスト対象のメソッド実行
            var encryptData = testClass.Encrypt(new SystemId(systemId), new CommonKeyId(commonKeyId), plainData);
            var result = testClass.Decrypt(new SystemId(systemId), new CommonKeyId(commonKeyId), encryptData);

            // モックの呼び出しを検証
            mockRepo.Verify(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)));

            // 結果をチェック
            result.Content.Is(plainData.Content);
        }

        [TestMethod]
        public void Encrypt_Fail()
        {
            // テスト対象のメソッド実行＆結果をチェック
            AssertEx.Throws<CommonKeyNotFoundException>(() => testClass.Encrypt(new SystemId(systemId2), new CommonKeyId(commonKeyId), plainData));

            // モックの呼び出しを検証
            mockRepo.Verify(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId2),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)));
        }

        [TestMethod]
        public void Decrypt_Fail()
        {
            var encryptData = new EncryptData(new byte[] { 10, 20, 30 });

            // テスト対象のメソッド実行＆結果をチェック
            AssertEx.Throws<CommonKeyNotFoundException>(() => testClass.Decrypt(new SystemId(systemId2), new CommonKeyId(commonKeyId), encryptData));

            // モックの呼び出しを検証
            mockRepo.Verify(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId2),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)));
        }

        [TestMethod]
        public void GetEncryptDecryptStream()
        {
            string testData = "{ \"item1\": \"暗号化テスト1234567890\" }";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testData);
            writer.Flush();
            stream.Position = 0;

            // テスト対象のメソッド実行
            var encryptStream = testClass.GetEncryptStream(new SystemId(systemId), new CommonKeyId(commonKeyId), new PlainStream(stream));
            var plainStream = testClass.GetDecryptStream(new SystemId(systemId), new CommonKeyId(commonKeyId), encryptStream);

            // モックの呼び出しを検証
            mockRepo.Verify(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)));

            using (var reader = new StreamReader(plainStream.Value))
            {
                string result = reader.ReadToEnd();
                // 結果をチェック
                result.Is(testData);
            }

            writer.Dispose();
        }

        [TestMethod]
        public void GetEncryptStream_Fail()
        {
            var stream = new MemoryStream();

            // テスト対象のメソッド実行＆結果をチェック
            AssertEx.Throws<CommonKeyNotFoundException>(() => testClass.GetEncryptStream(new SystemId(systemId2), new CommonKeyId(commonKeyId), new PlainStream(stream)));

            // モックの呼び出しを検証
            mockRepo.Verify(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId2),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)));
        }

        [TestMethod]
        public void GetDecryptStream_Fail()
        {
            var stream = new MemoryStream();

            // テスト対象のメソッド実行＆結果をチェック
            AssertEx.Throws<CommonKeyNotFoundException>(() => testClass.GetDecryptStream(new SystemId(systemId2), new CommonKeyId(commonKeyId), new EncryptStream(stream)));

            // モックの呼び出しを検証
            mockRepo.Verify(s => s.Get(
                It.Is<SystemId>(x => x.Value == systemId2),
                It.Is<CommonKeyId>(x => x.Value == commonKeyId)));
        }

        private CommonKeyParameters CreateCommonKeyParameters()
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                csp.GenerateKey();
                return new CommonKeyParameters(csp.Key, null, 128, CipherMode.CBC, PaddingMode.PKCS7);
            }
        }
    }
}
