using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    [TestClass]
    public class UnitTest_CommonKey : UnitTestBase
    {
        [TestMethod]
        public void CreateNewCommonKey()
        {
            string systemId = Guid.NewGuid().ToString();
            byte[] key = Encoding.UTF8.GetBytes("testKey");
            // 暗号化パラメーター作成
            var parameters = new CommonKeyParameters(key, null, 192, CipherMode.CTS, PaddingMode.None);

            // テスト実行
            var result = CommonKey.CreateNewCommonKey(systemId, null, parameters);

            // テスト結果検証
            result.CommonKeyId.Value.IsNotNull();
            result.SystemId.Value.Is(systemId);
            var now = DateTime.UtcNow;
            ((result.ExpirationDate.Value - now).TotalSeconds > 0).Is(true, $"exp: {result.ExpirationDate.Value}, now: {now}");
            result.Parameters.Key.Is(key);
            result.Parameters.IV.IsNotNull();
            result.Parameters.KeySize.Is(192);
            result.Parameters.CipherMode.Is(CipherMode.CTS);
            result.Parameters.PaddingMode.Is(PaddingMode.None);
        }

        [TestMethod]
        public void CreateNewCommonKey2()
        {
            string systemId = Guid.NewGuid().ToString();
            byte[] key = Encoding.UTF8.GetBytes("testKey");
            byte[] iv = new byte[0];
            DateTime dateTime = DateTime.UtcNow.AddMinutes(10);
            // 暗号化パラメーター作成
            var parameters = new CommonKeyParameters(key, iv, 192, CipherMode.CTS, PaddingMode.None);

            // テスト実行
            var result = CommonKey.CreateNewCommonKey(systemId, dateTime, parameters);

            // テスト結果検証
            result.ExpirationDate.Value.Is(dateTime);
            result.Parameters.IV.IsNotNull();
        }

        [TestMethod]
        public void CreateNewCommonKey3()
        {
            string systemId = Guid.NewGuid().ToString();
            byte[] key = Encoding.UTF8.GetBytes("testKey");
            byte[] iv = Encoding.UTF8.GetBytes("testIV");
            // 暗号化パラメーター作成
            var parameters = new CommonKeyParameters(key, iv, 192, CipherMode.CTS, PaddingMode.None);

            // テスト実行
            var result = CommonKey.CreateNewCommonKey(systemId, null, parameters);

            // テスト結果検証
            result.Parameters.IV.Is(iv);
        }

        [TestMethod]
        public void EncryptDecrypt()
        {
            string systemId = Guid.NewGuid().ToString();
            string testData = "{ \"item1\": \"暗号化テスト1234567890\" }";

            // 暗号化パラメーター作成
            var parameters = CreateCommonKeyParameters();

            // テスト対象のインスタンス作成
            var target = new CommonKey(new CommonKeyId(), new SystemId(systemId), new ExpirationDate(DateTime.Now), parameters);

            // テスト実行
            var encryptData = target.Encrypt(new PlainData(Encoding.UTF8.GetBytes(testData)));
            var plainData = target.Decrypt(encryptData);
            string result = Encoding.UTF8.GetString(plainData.Content);

            // テスト結果検証
            result.Is(testData);
        }

        [TestMethod]
        public void EncryptDecrypt_Fail()
        {
            string systemId = Guid.NewGuid().ToString();
            string testData = "{ \"item1\": \"暗号化テスト1234567890\" }";

            // 暗号化パラメーター作成
            var parameters = new CommonKeyParameters(Encoding.UTF8.GetBytes("hogehogehogehoge"), null, 128, CipherMode.CBC, PaddingMode.PKCS7);
            var parameters2 = new CommonKeyParameters(Encoding.UTF8.GetBytes("fugafugafugafuga"), null, 128, CipherMode.CBC, PaddingMode.PKCS7);

            // テスト対象のインスタンス作成
            var target = new CommonKey(new CommonKeyId(), new SystemId(systemId), new ExpirationDate(DateTime.Now), parameters);
            var target2 = new CommonKey(new CommonKeyId(), new SystemId(systemId), new ExpirationDate(DateTime.Now), parameters2);
            while (Convert.ToBase64String(parameters.Key) == Convert.ToBase64String(parameters2.Key) &&
                Convert.ToBase64String(parameters.IV) == Convert.ToBase64String(parameters2.IV))
            {
                parameters2 = CreateCommonKeyParameters();
            }

            // テスト実行
            var encryptData = target.Encrypt(new PlainData(Encoding.UTF8.GetBytes(testData)));
            // テスト実行＆結果検証
            AssertEx.Throws<CryptographicException>(() => target2.Decrypt(encryptData));
        }

        [TestMethod]
        public void GetEncryptDecryptStream()
        {
            string systemId = Guid.NewGuid().ToString();
            string testData = "{ \"item1\": \"暗号化テスト1234567890\" }";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testData);
            writer.Flush();
            stream.Position = 0;

            // 暗号化パラメーター作成
            var parameters = CreateCommonKeyParameters();

            // テスト対象のインスタンス作成
            var target = new CommonKey(new CommonKeyId(), new SystemId(systemId), new ExpirationDate(DateTime.Now), parameters);

            // テスト実行
            var encryptStream = target.GetEncryptStream(new PlainStream(stream));
            var plainStream = target.GetDecryptStream(encryptStream);

            using (var reader = new StreamReader(plainStream.Value))
            {
                string result = reader.ReadToEnd();
                // テスト結果検証
                result.Is(testData);
            }

            writer.Dispose();
        }

        [TestMethod]
        public void GetEncryptDecryptStream_Fail()
        {
            string systemId = Guid.NewGuid().ToString();
            string testData = "{ \"item1\": \"暗号化テスト1234567890\" }";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(testData);
            writer.Flush();
            stream.Position = 0;

            // 暗号化パラメーター作成
            var parameters = CreateCommonKeyParameters();
            var parameters2 = CreateCommonKeyParameters();
            while (Convert.ToBase64String(parameters.Key) == Convert.ToBase64String(parameters2.Key) &&
                Convert.ToBase64String(parameters.IV) == Convert.ToBase64String(parameters2.IV))
            {
                parameters2 = CreateCommonKeyParameters();
            }

            // テスト対象のインスタンス作成
            var target = new CommonKey(new CommonKeyId(), new SystemId(systemId), new ExpirationDate(DateTime.Now), parameters);
            var target2 = new CommonKey(new CommonKeyId(), new SystemId(systemId), new ExpirationDate(DateTime.Now), parameters2);

            // テスト実行
            var encryptStream = target.GetEncryptStream(new PlainStream(stream));
            var plainData = target2.GetDecryptStream(encryptStream);
            var reader = new StreamReader(plainData.Value);

            // テスト実行＆結果検証
            AssertEx.Throws<CryptographicException>(() => reader.ReadToEnd());

            stream.Dispose();
        }

        private CommonKeyParameters CreateCommonKeyParameters()
        {
            using (var csp = new AesCryptoServiceProvider())
            {
                csp.GenerateKey();
                csp.GenerateIV();
                return new CommonKeyParameters(csp.Key, csp.IV, 128, CipherMode.CBC, PaddingMode.PKCS7);
            }
        }
    }
}
