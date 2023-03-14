using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Domain.Context.AttachFile;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;
using JP.DataHub.Infrastructure.Database.AttachFile;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_AttachFileRepository : UnitTestBase
    {
        private const string CACHE_KEY_ATTACH_FILE_STORAGE_CONNECTIONSTRING = "AttachFileStorageConnectionString";

        private static string s_blobConnectionString = "DefaultEndpointsProtocol=https;AccountName=;AccountKey=;EndpointSuffix=core.windows.net";
        private static string s_tmpPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance<string>("AttachFileTmpPath", s_tmpPath);
            UnityContainer.RegisterInstance("AttachFile", new Mock<IJPDataHubDbConnection>());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            //作成したファイルの削除
            Directory.Delete(s_tmpPath, true);
        }

        [TestMethod]
        public void AttachFileRepository_TempFileUpload_正常系_削除ファイルなし()
        {
            var mockCache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);
            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            InputStream inputStream = new InputStream(stream);
            IsAppendStream isAppendStream = new IsAppendStream(false);
            AppendPosition appendPosition = new AppendPosition(0);

            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            var result = testClass.TempFileUpload(fileId, fileName, inputStream, isAppendStream, appendPosition);

            MemoryStream resultStream = new MemoryStream();
            FileInfo fileInfo = new FileInfo(result.Value);
            fileInfo.OpenRead().CopyTo(resultStream);

            resultStream.ToArray().IsStructuralEqual(stream.ToArray());
        }

        [TestMethod]
        public void AttachFileRepository_TempFileUpload_正常系_削除ファイルあり()
        {
            var mockCache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            InputStream inputStream = new InputStream(stream);
            IsAppendStream isAppendStream = new IsAppendStream(false);
            AppendPosition appendPosition = new AppendPosition(0);

            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            //削除ファイルを設置
            var tmpresult = testClass.TempFileUpload(fileId, fileName, inputStream, isAppendStream, appendPosition);
            var result = testClass.TempFileUpload(fileId, fileName, inputStream, isAppendStream, appendPosition);

            MemoryStream resultStream = new MemoryStream();
            FileInfo fileInfo = new FileInfo(result.Value);
            fileInfo.OpenRead().CopyTo(resultStream);

            resultStream.ToArray().IsStructuralEqual(stream.ToArray());
        }

        [TestMethod]
        public void AttachFileRepository_TempFileUpload_正常系_ファイル追加()
        {
            var mockCache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            InputStream inputStream = new InputStream(stream);
            IsAppendStream isAppendStream = new IsAppendStream(false);
            AppendPosition appendPosition = new AppendPosition(0);
            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            //削除ファイルを設置
            var tmpresult = testClass.TempFileUpload(fileId, fileName, inputStream, isAppendStream, appendPosition);
            var result = testClass.TempFileUpload(fileId, fileName, inputStream, new IsAppendStream(true), new AppendPosition(inputStream.Value.Length));

            MemoryStream resultStream = new MemoryStream();
            FileInfo fileInfo = new FileInfo(result.Value);
            fileInfo.OpenRead().CopyTo(resultStream);
            var anserArray = new List<byte>() { };
            anserArray.AddRange(stream.ToArray());
            anserArray.AddRange(stream.ToArray());
            resultStream.ToArray().IsStructuralEqual(anserArray.ToArray());
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void AttachFileRepository_TempFileUpload_異常系_分割アップロード順序不備()
        {
            var mockCache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            InputStream inputStream = new InputStream(stream);
            IsAppendStream isAppendStream = new IsAppendStream(false);
            AppendPosition appendPosition = new AppendPosition(0);
            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            //削除ファイルを設置
            var tmpresult = testClass.TempFileUpload(fileId, fileName, inputStream, isAppendStream, appendPosition);
            var result = testClass.TempFileUpload(fileId, fileName, inputStream, new IsAppendStream(true), new AppendPosition(inputStream.Value.Length + 1));

            MemoryStream resultStream = new MemoryStream();
            FileInfo fileInfo = new FileInfo(result.Value);
            fileInfo.OpenRead().CopyTo(resultStream);
            var anserArray = new List<byte>() { };
            anserArray.AddRange(stream.ToArray());
            anserArray.AddRange(stream.ToArray());
            resultStream.ToArray().IsStructuralEqual(anserArray.ToArray());
        }


        [TestMethod]
        public void AttachFileRepository_Upload_正常系()
        {
            string vid = "7EE5156B-B0C8-4BDA-9384-DCB22377BC4B";
            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            AttachFileStorageId attachFileStorageId = new AttachFileStorageId("AAA");
            VendorId vendorId = new VendorId(Guid.Parse(vid));
            InputStream inputStream = new InputStream(stream);
            ContentType contentType = new ContentType("image/jpeg");
            var key = CacheManager.CreateKey(CACHE_KEY_ATTACH_FILE_STORAGE_CONNECTIONSTRING, attachFileStorageId);
            var mock = new Mock<IJPDataHubDbConnection>();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<DB_AttachFileStorage>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>())).Returns(new DB_AttachFileStorage() { connection_string = s_blobConnectionString });

            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            var tmpResult = testClass.TempFileUpload(fileId, fileName, inputStream, new IsAppendStream(false), new AppendPosition(0));

            testClass.Upload(tmpResult, fileId, fileName, vendorId, contentType, attachFileStorageId);
            var result = testClass.GetFileStream(vendorId, fileId, fileName, attachFileStorageId);

            //ファイルがBlobにアップロードされていること
            result.Length.IsStructuralEqual(inputStream.Value.Length);

            //TMPファイルが削除されていること
            File.Exists(tmpResult.Value).IsFalse();


        }

        [TestMethod]
        public void AttachFileRepository_GetFileStream_正常系()
        {
            string vid = "7EE5156B-B0C8-4BDA-9384-DCB22377BC4B";
            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            AttachFileStorageId attachFileStorageId = new AttachFileStorageId("AAA");
            VendorId vendorId = new VendorId(Guid.Parse(vid));
            InputStream inputStream = new InputStream(stream);
            ContentType contentType = new ContentType("image/jpeg");
            var key = CacheManager.CreateKey(CACHE_KEY_ATTACH_FILE_STORAGE_CONNECTIONSTRING, attachFileStorageId);
            var mock = new Mock<IJPDataHubDbConnection>();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<DB_AttachFileStorage>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>())).Returns(new DB_AttachFileStorage() { connection_string = s_blobConnectionString });

            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            var tmpResult = testClass.TempFileUpload(fileId, fileName, inputStream, new IsAppendStream(false), new AppendPosition(0));

            testClass.Upload(tmpResult, fileId, fileName, vendorId, contentType, attachFileStorageId);
            var result = testClass.GetFileStream(vendorId, fileId, fileName, attachFileStorageId);

            result.Length.IsStructuralEqual(inputStream.Value.Length);
        }


        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void AttachFileRepository_DeleteFile_正常系()
        {
            string vid = "7EE5156B-B0C8-4BDA-9384-DCB22377BC4B";
            MemoryStream stream = CreateTestStream(new MemoryStream());
            FileId fileId = new FileId(Guid.NewGuid());
            FileName fileName = new FileName("test");
            AttachFileStorageId attachFileStorageId = new AttachFileStorageId("AAA");
            VendorId vendorId = new VendorId(Guid.Parse(vid));
            InputStream inputStream = new InputStream(stream);
            ContentType contentType = new ContentType("image/jpeg");
            var key = CacheManager.CreateKey(CACHE_KEY_ATTACH_FILE_STORAGE_CONNECTIONSTRING, attachFileStorageId);

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<DB_AttachFileStorage>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>())).Returns(new DB_AttachFileStorage() { connection_string = s_blobConnectionString });
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var requestResult = (RequestResult)typeof(RequestResult).GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { });
            requestResult.GetType().GetProperty("HttpStatusCode").SetValue(requestResult, 404);
            var exception = new StorageException(requestResult, "", new Exception());

            var content = "hogehoge";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

            var mockBlob = new Mock<IBlobStorage>();
            mockBlob.SetupSequence(x => x.GetBlockBlobAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((CloudBlockBlob)new MockCloudBlockBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), ms, "text/plain"))
                .Throws(exception);
            UnityContainer.RegisterInstance<IBlobStorage>("AttachFileBlobStorage", mockBlob.Object);

            var testClass = UnityContainer.Resolve<IAttachFileRepository>();
            var tmpResult = testClass.TempFileUpload(fileId, fileName, inputStream, new IsAppendStream(false), new AppendPosition(0));

            testClass.Upload(tmpResult, fileId, fileName, vendorId, contentType, attachFileStorageId);
            testClass.DeleteFile(vendorId, fileId, fileName, attachFileStorageId);
            var result = testClass.GetFileStream(vendorId, fileId, fileName, attachFileStorageId);
        }



        private static MemoryStream CreateTestStream(MemoryStream memoryStream)
        {
            for (int i = 0; i < 10000; i++)
            {
                memoryStream.WriteByte((byte)(i % 32));
            }
            return memoryStream;
        }
    }
}
