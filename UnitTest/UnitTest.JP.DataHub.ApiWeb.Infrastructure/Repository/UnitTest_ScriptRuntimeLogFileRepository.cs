using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Moq;
using Unity;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_ScriptRuntimeLogFileRepository : UnitTestBase
    {
        private static Guid s_vendorId = Guid.NewGuid();
        private static Guid s_systemId = Guid.NewGuid();
        private static Guid s_apiId = Guid.NewGuid();
        private static Guid s_scriptRuntimeLogId = Guid.NewGuid();
        private static Uri s_uri = new Uri("https://www.google.com");

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        public void ScriptRuntimeLogFileRepository_AppendAsync_正常系()
        {
            var mockBlob = new Mock<IBlobStorage>();
            mockBlob.Setup(x => x.AppendBlobAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<string>()))
                .ReturnsAsync(s_uri);
            UnityContainer.RegisterInstance<IBlobStorage>("ScriptRuntimeFileBlobStorage", mockBlob.Object);
            var repository = UnityContainer.Resolve<IScriptRuntimeLogFileRepository>();

            var result = repository.AppendAsync(new ScriptRuntimeLogAppendFile("Runtime.log", s_vendorId, s_apiId, "hogehoge", "text/plain")).Result;
            AssertEx.IsTrue(result.ToString().Equals(s_uri.ToString()));
        }

        [TestMethod]
        public void ScriptRuntimeLogFileRepository_Get_正常系()
        {
            var mockBlob = new Mock<IBlobStorage>();
            var name = "Runtime.log";
            var content = "hogehoge";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var contentType = "text/plain";
            mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.IsAny<string>()))
                //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), ms, contentType));
            UnityContainer.RegisterInstance<IBlobStorage>("ScriptRuntimeFileBlobStorage", mockBlob.Object);
            var repository = UnityContainer.Resolve<IScriptRuntimeLogFileRepository>();

            var result = repository.Get(s_scriptRuntimeLogId, s_vendorId);
            result.Content.ReadAsStringAsync().Result.Is(content);
            result.ContentType.Value.Is(contentType);
            result.ScriptRuntimeLogId.Is(s_scriptRuntimeLogId);
            result.Name.Value.Is(name);
            result.FilePath.Value.Is($"{s_vendorId.ToString()}\\{s_scriptRuntimeLogId.ToString()}");

        }

        [TestMethod]
        [ExpectedException(typeof(NotFoundException))]
        public void ScriptRuntimeLogFileRepository_Get_異常系_不正なファイルパス()
        {
            var mockBlob = new Mock<IBlobStorage>();
            var content = "hogehoge";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            //mockBlob.Setup(x => x.GetAppendBlob(It.IsAny<string>(), It.IsAny<string>()))
            //.Throws(new StorageException(new RequestResult(),exceptionMsg))
            //.Returns(new ScriptRuntimeLogGetFile(name,vendorId,scriptRuntimeLogId, ms,contentType));
            //.Returns(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), ms, contentType));
            //UnityContainer.RegisterInstance<IBlobStorage>("ScriptRuntimeFileBlobStorage", mockBlob.Object);
            var repository = UnityContainer.Resolve<IScriptRuntimeLogFileRepository>();

            var result = repository.Get(s_scriptRuntimeLogId, s_vendorId);
        }

        [TestMethod]
        [ExpectedException(typeof(StorageException))]
        public void ScriptRuntimeLogFileRepository_Get_異常系_何らかのエラー()
        {
            var mockBlob = new Mock<IBlobStorage>();
            var content = "hogehoge";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            var exceptionMsg = "exception test";
            var re = new RequestResult();
            mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new StorageException(re, exceptionMsg, new Exception("inner")));
            //.Returns(new ScriptRuntimeLogGetFile(name,vendorId,scriptRuntimeLogId, ms,contentType));
            //.Returns(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), ms, contentType));
            UnityContainer.RegisterInstance<IBlobStorage>("ScriptRuntimeFileBlobStorage", mockBlob.Object);
            var repository = UnityContainer.Resolve<IScriptRuntimeLogFileRepository>();

            var result = repository.Get(s_scriptRuntimeLogId, s_vendorId);
        }
    }
}
