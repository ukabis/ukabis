using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_AttachFileDownloadAction : UnitTestBase
    {
        private static Accept s_defaultAccept = new Accept("*/*");
        private static string s_vendorId = "7519869f-54c9-496d-b83c-884091069856";
        private static string s_systemId = "FE652EE4-E0E5-4C28-AA0D-885FE5AC22BA";
        private static string s_fileId = "D261DF4B-0305-46AD-9FF5-12026BEE3F89";

        private string _dataSourceType = Guid.NewGuid().ToString();
        private Mock<IExternalAttachFileRepository> _mockExternalAttachFileRepository;
        private Mock<INewDynamicApiDataStoreRepository> _mockDataRepository;
        private Mock<IDynamicApiAttachFileRepository> _mockFileRepository;


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            _mockExternalAttachFileRepository = new Mock<IExternalAttachFileRepository>();
            UnityContainer.RegisterInstance(_dataSourceType, _mockExternalAttachFileRepository.Object);

            var mockRepository = new Mock<IDynamicApiRepository>();
            mockRepository.Setup(x => x.GetSchemaModelById(It.IsAny<DataSchemaId>())).Returns(new DataSchema(""));
            UnityContainer.RegisterInstance(mockRepository.Object);
        }


        [TestMethod]
        public void ExecuteAction_正常()
        {
            var action = CreateAction();
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _mockFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>()), Times.Once);
            _mockExternalAttachFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<ExternalAttachFileInfomation>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_正常_Keyあり()
        {
            string key = "hoge";
            var action = CreateAction(key);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) }, { new QueryStringKey("Key"), new QueryStringValue(key) } });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _mockFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>()), Times.Once);
            _mockExternalAttachFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<ExternalAttachFileInfomation>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_正常_外部添付ファイル()
        {
            _mockExternalAttachFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<ExternalAttachFileInfomation>())).Returns(new MemoryStream());

            string key = "hoge";
            var action = CreateAction(key);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) }, { new QueryStringKey("Key"), new QueryStringValue(key) } });
            var meta = GetInfoJson(key, false).ToJson();
            meta[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(_dataSourceType, new List<string>() { Guid.NewGuid().ToString() }, Guid.NewGuid().ToString()).ToJson();
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(meta));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _mockFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>()), Times.Never);
            _mockExternalAttachFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<ExternalAttachFileInfomation>()), Times.Once);
        }


        [TestMethod]
        public void ExecuteAction_異常_KeyMiss()
        {
            string key = "hoge";
            var action = CreateAction(key);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) }, { new QueryStringKey("Key"), new QueryStringValue("fuga") } });
            var result = action.ExecuteAction();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E20402.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常_Keyなし()
        {
            string key = "hoge";
            var action = CreateAction(key);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) } });
            var result = action.ExecuteAction();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E20402.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ExecuteAction_異常_コンテンツなし()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Returns(new MemoryStream());
            var result = action.ExecuteAction();
        }

        [TestMethod]
        public void ExecuteAction_異常_コンテンツなし_未アップロード()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(GetInfoJson(null, false)) });
            mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Returns(new MemoryStream());
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ExecuteAction_異常_Blobなし()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Throws(new NotFoundException());
            var result = action.ExecuteAction();
        }

        [TestMethod]
        public void ExecuteAction_異常_Blobなし_未アップロード()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(GetInfoJson(null, false)) });
            mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Throws(new NotFoundException());
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ExecuteAction_異常_メソッドタイプ違反()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_パラメータ異常_FileIDなし()
        {
            var action = CreateAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E20401.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_パラメータ異常_FileID不正()
        {
            var action = CreateAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue("hogehoge") } });
            var result = action.ExecuteAction();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E20401.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常_Metaなし()
        {
            var action = CreateAction();
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object,mockFileRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ExecuteAction_異常_外部添付ファイル()
        {
            var errorCode = ErrorCodeMessage.Code.E20415;
            _mockExternalAttachFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<ExternalAttachFileInfomation>()))
                .Throws(new ExternalAttachFileException(errorCode));

            string key = "hoge";
            var action = CreateAction(key);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) }, { new QueryStringKey("Key"), new QueryStringValue(key) } });
            var meta = GetInfoJson(key, false).ToJson();
            meta[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(_dataSourceType, new List<string>() { Guid.NewGuid().ToString() }, Guid.NewGuid().ToString()).ToJson();
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(meta));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.ToJson()["error_code"].Value<string>().Is(errorCode.ToString());

            _mockFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>()), Times.Never);
            _mockExternalAttachFileRepository.Verify(x => x.GetAttachFileFileStream(It.IsAny<ExternalAttachFileInfomation>()), Times.Once);
        }

        [TestMethod]
        public void ExecuteAction_異常_設定無効()
        {
            var action = CreateAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotImplemented);
        }

        private AttachFileDownloadAction CreateAction(string key = null)
        {
            AttachFileDownloadAction action = UnityCore.Resolve<AttachFileDownloadAction>();
            action.ApiId = new ApiId(Guid.NewGuid().ToString());
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.SystemId = new SystemId(s_systemId);
            action.VendorId = new VendorId(s_vendorId);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/AttachfileTest/{FileId}");
            action.ActionType = new ActionTypeVO(ActionType.AttachFileDownload);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.Accept = s_defaultAccept;
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) } });
            action.Contents = new Contents(new MemoryStream());
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            _mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            _mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                _mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = _mockFileRepository.Object;

            _mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(GetInfoJson(key))) });
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson(key))));
            _mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Returns(CreateTestStream(new MemoryStream()));

            return action;
        }

        private string GetInfoJson(string key = null, bool isUpload = true)
        {
            DynamicApiAttachFileInformation dynamicApiAttachFileInformation = new DynamicApiAttachFileInformation(s_fileId, "filename", "application/octet-stream", 100, "hoge/fuga/piyo.jpeg", key, false, null, null, isUpload);

            var json = JToken.Parse(JsonConvert.SerializeObject(dynamicApiAttachFileInformation));
            var fi = json.SelectToken("FileId");
            fi.Parent.Remove();
            json["FileId"] = s_fileId;
            return json.ToString();
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
