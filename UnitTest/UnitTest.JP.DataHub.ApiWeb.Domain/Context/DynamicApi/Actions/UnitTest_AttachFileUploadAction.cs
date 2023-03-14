using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
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
    public class UnitTest_AttachFileUploadAction : UnitTestBase
    {
        private static Accept s_defaultAccept = new Accept("*/*");
        private static string s_vendorId = "7519869f-54c9-496d-b83c-884091069856";
        private static string s_systemId = "FE652EE4-E0E5-4C28-AA0D-885FE5AC22BA";
        private static string s_fileId = "D261DF4B-0305-46AD-9FF5-12026BEE3F89";

        public TestContext TestContext { get; set; }

        private readonly string _versionKeyNew = Guid.NewGuid().ToString();
        private readonly string _versionKeyLatest = Guid.NewGuid().ToString();
        private readonly string _versionKeySecond = Guid.NewGuid().ToString();
        private readonly string _versionKeyInitial = Guid.NewGuid().ToString();
        private readonly string _versionKeyInitialDelete = Guid.NewGuid().ToString();

        private readonly string _normalDocumentKey = Guid.NewGuid().ToString();
        private readonly string _noUploadDocumentKey = Guid.NewGuid().ToString();
        private readonly string _high_physicalrepositoryId = Guid.NewGuid().ToString();
        private readonly string _high_repositoryGroupId = Guid.NewGuid().ToString();
        private readonly string _low_physicalrepositoryId = Guid.NewGuid().ToString();
        private readonly string _low_repositoryGroupId = Guid.NewGuid().ToString();

        private DocumentHistories _savedDocumentHisotry;
        private List<JToken> _blockchainEventList;
        private string _repositoryGroupId = Guid.NewGuid().ToString();
        Mock<IDocumentVersionRepository> _mockDocumentVersionRepository;
        Mock<INewDynamicApiDataStoreRepository> _mockDataRepository;


        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);

            //UploadOK/BlockList
            UnityContainer.RegisterInstance<bool>("IsEnableUploadContentCheck", false);
            UnityContainer.RegisterInstance<bool>("IsPriorityHigh_OKList", false);
            UnityContainer.RegisterInstance<bool>("IsUploadOk_NoExtensionFile", true);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);

            var mockRepository = new Mock<IDynamicApiRepository>();
            mockRepository.Setup(x => x.GetSchemaModelById(It.IsAny<DataSchemaId>())).Returns(new DataSchema(""));
            UnityContainer.RegisterInstance(mockRepository.Object);

            string okContentTypeList = null;
            string okExtensionList = null;
            string blockContentTypeList = null;
            string blockExtensionList = null;
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ContentTypeList", GetListFromConfigString(okContentTypeList));
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ExtensionList", GetListFromConfigString(okExtensionList));
            UnityContainer.RegisterInstance<List<string>>("BlockContentTypeList", GetListFromConfigString(blockContentTypeList));
            UnityContainer.RegisterInstance<List<string>>("BlockExtensionList", GetListFromConfigString(blockExtensionList));
        }

        [TestMethod]
        public void ExecuteAction_正常_履歴off()
        {
            var action = CreateAction();
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Headers.Contains("X-DocumentHistory").IsFalse();

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_正常_履歴on_履歴未登録()
        {
            var action = CreateAction(isDocumentHistory: true, fileId: _noUploadDocumentKey);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);

            var historyHeader = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(result.Headers.GetValues("X-DocumentHistory").Single()).Single();
            historyHeader.IsNotNull();
            historyHeader.isSelfHistory.IsTrue();
            historyHeader.resourcePath.Is(action.ControllerRelativeUrl.Value);
            historyHeader.documents.Count().Is(1);
            historyHeader.documents.Single().documentKey.Is(_noUploadDocumentKey);
            historyHeader.documents.Single().versionKey.Is(_versionKeyInitial);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _savedDocumentHisotry.DocumentVersions.Count().Is(1);
            _savedDocumentHisotry.DocumentVersions.Last().VersionKey.Is(_versionKeyInitial);
            _savedDocumentHisotry.DocumentVersions.Last().LocationType.Is(DocumentHistory.StorageLocationType.HighPerformance);
        }


        [TestMethod]
        public void ExecuteAction_正常_履歴on_履歴登録済()
        {
            var action = CreateAction(isDocumentHistory: true, fileId: _normalDocumentKey, isUploaded: true);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);

            var historyHeader = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(result.Headers.GetValues("X-DocumentHistory").Single()).Single();
            historyHeader.IsNotNull();
            historyHeader.isSelfHistory.IsTrue();
            historyHeader.resourcePath.Is(action.ControllerRelativeUrl.Value);
            historyHeader.documents.Count().Is(1);
            historyHeader.documents.Single().documentKey.Is(_normalDocumentKey);
            historyHeader.documents.Single().versionKey.Is(_versionKeyNew);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _savedDocumentHisotry.DocumentVersions.Count().Is(4);
            _savedDocumentHisotry.DocumentVersions.Last().VersionKey.Is(_versionKeyNew);
            _savedDocumentHisotry.DocumentVersions.Last().LocationType.Is(DocumentHistory.StorageLocationType.LowPerformance);
        }


        [TestMethod]
        public void ExecuteAction_正常_Blockchainあり()
        {
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, JToken, RepositoryType, string>((id, token, type, version) =>
                {
                    _blockchainEventList.Add(token);
                });
            _blockchainEventList = new List<JToken>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

            var action = CreateAction();
            action.IsEnableBlockchain = new IsEnableBlockchain(true);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);

            mockBlockchainEventhub.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.AtLeast(1));
            Assert.IsTrue(_blockchainEventList.Count() > 0); // タイミングによっては2なので1以上で見る
            var expected = GetInfoJson(null, true).ToJson();
            _blockchainEventList.ElementAt(0).Is(expected);
            var field = action.GetType().GetField("TaskPublishEventBlockchain", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance);
            var taskPubblockchain = field.GetValue(action) as Task;
            taskPubblockchain.IsNotNull();

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_正常_分割途中()
        {
            var action = CreateAction();
            action.ContentRange = new ContentRange("bytes 0-1023/146515");
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson())));
            mockFileRepository.Setup(x => x.UploadAttachFile(It.IsAny<DynamicApiAttachFileInformation>(), It.IsAny<DynamicApiAttachFileInputStream>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            mockDataRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Never);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_正常_分割終了()
        {
            var action = CreateAction();
            action.ContentRange = new ContentRange("bytes 7000-7999/8000");
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson())));
            mockFileRepository.Setup(x => x.UploadAttachFile(It.IsAny<DynamicApiAttachFileInformation>(), It.IsAny<DynamicApiAttachFileInputStream>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            mockDataRepository.Verify(x => x.RegisterOnce(It.IsAny<RegisterParam>()), Times.Once);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_異常_コンテンツなし()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void ExecuteAction_異常_メソッドタイプ違反()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
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

        }

        [TestMethod]
        public void ExecuteAction_パラメータ異常_FileID不正()
        {
            var action = CreateAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue("hogehoge") } });
            var result = action.ExecuteAction();
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
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(null));
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ExecuteAction_異常_外部添付ファイル()
        {
            var action = CreateAction();
            var meta = GetInfoJson(null, false, s_fileId).ToJson();
            meta[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() }, Guid.NewGuid().ToString()).ToJson();
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(meta));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            result.Content.ReadAsStringAsync().Result.ToJson()["error_code"].Value<string>().Is("E20412");
        }

        [TestMethod]
        public void ExecuteAction_異常_設定無効()
        {
            var action = CreateAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        [TestCase("hoge.jpg")]
        [TestCase("hoge.jpg.bak")]
        [TestCase("hoge..bak")]
        [TestCase("hoge.6ad27147-364b-412e-809f-8d6765c62e5e.bak")]
        public void PublishEventBlockchain_正常系()
        {
            TestContext.Run((string filename) =>
            {
                var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
                mockBlockchainEventhub.Setup(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                    .Returns(true)
                    .Callback<string, JToken, RepositoryType, string>((id, token, type, version) =>
                    {
                        _blockchainEventList.Add(token);
                    });
                _blockchainEventList = new List<JToken>();

                UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

                var action = CreateAction();
                action.IsEnableBlockchain = new IsEnableBlockchain(true);
                var json = JToken.Parse(GetInfoJson()).ReplaceField("FilePath", filename);
                var info = DynamicApiAttachFileInformation.PerseFromJToken(json);
                action.GetType().InvokeMember("PublishEventBlockchain", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, action, new object[] { json, info });

                mockBlockchainEventhub.Verify(x => x.Register(It.IsAny<string>(), It.IsAny<JToken>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.AtLeast(1));
                _blockchainEventList.Count().Is(1);
                _blockchainEventList.ElementAt(0)["RepositoryGroupId"].ToString().Is(_repositoryGroupId);
                var expectRegExpPatFileName = new Regex(@"\.([A-Za-z0-9\-]{36})\.bak$");
                var mt = expectRegExpPatFileName.Match(_blockchainEventList.ElementAt(0)["FilePath"].ToString());
                mt.Success.IsTrue();
                mt.Groups.Count.Is(2);
                Guid.TryParse(mt.Groups[1].Value, out _).IsTrue();
            });
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var action = CreateAction(null, true, null, false);
            action.ContentRange = new ContentRange("bytes 7000-7999/8000");
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson())));
            mockFileRepository.Setup(x => x.UploadAttachFile(It.IsAny<DynamicApiAttachFileInformation>(), It.IsAny<DynamicApiAttachFileInputStream>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            _mockDocumentVersionRepository.Verify(x => x.GetDocumentVersion(It.IsAny<DocumentKey>()), Times.Never);
            _mockDocumentVersionRepository.Verify(x => x.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()), Times.Never);
            _mockDocumentVersionRepository.Verify(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>()), Times.Never);
        }

        private AttachFileUploadAction CreateAction(string key = null, bool isDocumentHistory = false, string fileId = null, bool isUploaded = false)
        {
            AttachFileUploadAction action = UnityCore.Resolve<AttachFileUploadAction>();
            action.ApiId = new ApiId(Guid.NewGuid().ToString());
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.SystemId = new SystemId(s_systemId);
            action.VendorId = new VendorId(s_vendorId);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            action.RepositoryKey = new RepositoryKey("/API/Private/AttachfileTest/{FileId}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/AttachfileTest/UploadAttachFile");

            action.ActionType = new ActionTypeVO(ActionType.AttachFileUpload);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.Accept = s_defaultAccept;
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(fileId ?? s_fileId) } });
            action.Contents = new Contents(new MemoryStream());
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            action.AttachFileBlobRepositoryGroupId = new RepositoryGroupId(_repositoryGroupId);
            _mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                _mockDataRepository.Object
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;
            _mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>()));
            mockFileRepository.Setup(x => x.UploadAttachFile(It.IsAny<DynamicApiAttachFileInformation>(), It.IsAny<DynamicApiAttachFileInputStream>()));
            mockFileRepository.Setup(x => x.CopyFile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));

            _mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(GetInfoJson(fileId: fileId, isUploaded: isUploaded))) });
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson(key, fileId: fileId, isUploaded: isUploaded))));
            _mockDataRepository.Setup(x => x.RepositoryInfo).Returns(new RepositoryInfo(RepositoryType.CosmosDB.ToCode(), new Dictionary<string, bool>()));


            RepositoryInfo metaDataRepositoryInfo = new RepositoryInfo(RepositoryType.AttachFileMetaCosmosDb.ToCode(), new Dictionary<string, bool>());
            RepositoryInfo fileDataRepositoryInfo = new RepositoryInfo(RepositoryType.AttachFileBlob.ToCode(), new Dictionary<string, bool>());
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>()
            {
                { metaDataRepositoryInfo }  ,
                { fileDataRepositoryInfo }
            });

            action.IsDocumentHistory = new IsDocumentHistory(isDocumentHistory);
            if (isDocumentHistory)
            {

                var mockHistoryEvacuationRepository = new Mock<INewDynamicApiDataStoreRepository>();
                var registerResult = new RegisterOnceResult("hoge.jpg", new Dictionary<string, object> { { "Container", "hogecontainername" } });
                mockHistoryEvacuationRepository.Setup(_ => _.RegisterOnce(It.IsAny<RegisterParam>())).Returns(registerResult);
                action.HistoryEvacuationDataStoreRepository = mockHistoryEvacuationRepository.Object;

                _mockDocumentVersionRepository = new Mock<IDocumentVersionRepository>();
                var documentMetainfo = new DocumentHistoryAttachFileMetaData("image/jpeg", "Key");
                RepositoryKeyInfo mockMetaDataRepositoryKeyInfo = new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid());
                var resultDocumentVersionLatest = new DocumentHistory(_versionKeyLatest, 3, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.HighPerformance, mockMetaDataRepositoryKeyInfo, "location", null, documentMetainfo);
                var resultDocumentVersionSecond = new DocumentHistory(_versionKeySecond, 2, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.Delete, null, null, null, documentMetainfo);
                var resultDocumentVersionInitial = new DocumentHistory(_versionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.Parse(_high_repositoryGroupId), Guid.Parse(_high_physicalrepositoryId)), "location", null, documentMetainfo);
                var resultDocumentVersionInitialUploaded = new DocumentHistory(_versionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.LowPerformance, new RepositoryKeyInfo(Guid.Parse(_low_repositoryGroupId), Guid.Parse(_low_physicalrepositoryId)), "location", null, documentMetainfo);
                var resultDocumentVersionInitialDelete = new DocumentHistory(_versionKeyInitialDelete, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.Delete, null, null, null, documentMetainfo);
                var resultNormalHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionLatest }, { resultDocumentVersionSecond }, { resultDocumentVersionInitial } });
                var resultInitialHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionInitial } });
                var resultInitialDeleteHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionInitialDelete } });
                var resultDeletedHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionSecond }, { resultDocumentVersionInitial } });
                var resultDriveoutedHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionInitial } });
                _mockDocumentVersionRepository.Setup(_ => _.GetDocumentVersion(It.Is<DocumentKey>(x => x.Id == _normalDocumentKey))).Returns(resultNormalHistory);
                _mockDocumentVersionRepository.Setup(_ => _.GetDocumentVersion(It.Is<DocumentKey>(x => x.Id == _noUploadDocumentKey))).Returns((DocumentHistories)null);
                _mockDocumentVersionRepository.Setup(_ => _.SaveDocumentVersion(It.Is<DocumentKey>(x => x.Id == _noUploadDocumentKey), It.IsAny<RepositoryKeyInfo>(), It.Is<bool>(x => !x)))
                    .Returns((DocumentKey dockey, RepositoryKeyInfo keyinfo, bool isdelete) =>
                    {
                        _savedDocumentHisotry = resultInitialHistory;
                        return resultInitialHistory;
                    });

                _mockDocumentVersionRepository.Setup(_ => _.SaveDocumentVersion(It.Is<DocumentKey>(x => x.Id == _noUploadDocumentKey), It.IsAny<RepositoryKeyInfo>(), It.Is<bool>(x => x)))
                    .Returns((DocumentKey dockey, RepositoryKeyInfo keyinfo, bool isdelete) =>
                    {
                        _savedDocumentHisotry = resultInitialDeleteHistory;
                        return resultInitialDeleteHistory;
                    });
                _mockDocumentVersionRepository.Setup(_ => _.SaveDocumentVersion(It.Is<DocumentKey>(x => x.Id == _normalDocumentKey), It.IsAny<RepositoryKeyInfo>(), It.IsAny<DocumentHistory>(), It.IsAny<bool>()))
                        .Returns((DocumentKey dockey, RepositoryKeyInfo keyinfo, DocumentHistory history, bool isdelete) =>
                        {
                            var newHis = new DocumentHistory(_versionKeyNew, 4, DateTime.Now, Guid.NewGuid().ToString(), isdelete ? DocumentHistory.StorageLocationType.Delete : history.LocationType, null, null);

                            resultNormalHistory.DocumentVersions.Add(newHis); ;
                            _savedDocumentHisotry = resultNormalHistory;
                            return resultNormalHistory;
                        });
                _mockDocumentVersionRepository.SetupGet(_ => _.RepositoryKeyInfo).Returns(mockMetaDataRepositoryKeyInfo);
                _mockDataRepository.SetupGet(_ => _.DocumentVersionRepository).Returns(_mockDocumentVersionRepository.Object);
            }
            mockFileRepository.Setup(x => x.GetUriWithSharedAccessSignature(It.IsAny<DynamicApiAttachFileInformation>()))
            .Returns(new Uri("http://localhost/"));


            return action;
        }

        private string GetInfoJson(string key = null, bool isUploaded = false, string fileId = null)
        {
            var jsonStr = new DynamicApiAttachFileInformation(fileId ?? s_fileId, "filename", "json", 100, "hoge/fuga/piyo.jpeg", key, false, null, null, false).Serialize();
            var json = JToken.Parse(jsonStr);
            json["IsUploaded"] = isUploaded;
            json.LastOrDefault().AddAfterSelf(new JProperty("id", "hogeId1"));
            json[nameof(DynamicApiAttachFileInformation.Key)] = key;
            return json.ToString();
        }

        public static object[] OKリストで_ブロック_OKチェック_コンテントタイプ = new[]
        {
            //大文字小文字混在
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "Text/Html", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TexT/PLane", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/PdF", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/Json",  UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            //ワイルドカード効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/Jsoooooon", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JS*", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            //中間一致しないか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/JsonHoge", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //正規表現効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/pde", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "APPlication/pda", UploadOK_ContentTypeList = "text/plane, ApplicaTion/PDf, APPLIcation/JSON,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
        };

        public static object[] ブロックリストで_ブロック_OKチェック_コンテントタイプ = new[]
        {
            //大文字小文字混在
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTml", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "Text/javascRIPT", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //OKのやつ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TexT/PLane", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            //ワイルドカード効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "Text/javascRIPThoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //正規表現効いているか
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/pdE", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "application/pDa", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //中間一致しないか確認
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/hTmlA", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
        };

        public static object[] ブロックリストでブロック_OKチェック_コンテントタイプ = new[]
        {
            //ブロックしないやつ:OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.jpg", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            //コンテントタイプjpeg(ブロックしない)と、ブロックするファイルタイプ:ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge.exe", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプhtml(ブロックする)と、ブロックしないファイルタイプ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TexT/HTmL", RequestFileName = "hoge.jpEg", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプhtml(ブロックする)と、ブロックするファイルタイプ
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "TExt/HtMl", RequestFileName = "hoge.Exe", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックしないやつで、拡張子無し：NG → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックで、拡張子無し：NG → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/JavaScripT", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = false, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックで、拡張子無し：OK → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "teXt/JavaScripT", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがブロックしないやつで、拡張子無し：OK → OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg", RequestFileName = "hoge", UploadBlock_ContentTypeList = "text/HTML,text/javasc*,application/pd[e|a]", UploadBlock_ExtensionList = "exe,vb[s|e],c.*.m", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
        };

        public static object[] OKリストとブロックリスト_コンテントタイプと拡張子組み合わせ = new[]
        {
            //OKリスト、ブロックリストには同じものを入れる(text/plane, txt) フラグで変わるかチェック
            //OKリストにしかないもの
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/jpeg",  UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane",  UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK}},
            //ブロックリストにしかない
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/html", UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane",UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane",  IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
            //コンテントタイプがOK、ブロックどちらにもある :  OKリスト優先 → OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane",  UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane",  UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane", IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            //コンテントタイプがOK、ブロックどちらにもある、ブロックリスト優先 :  ブロックリスト優先 → ブロック
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "text/plane",  UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane",  UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane",  IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411" } },
            //コンテントタイプがOKリストに無く、ブロックリストにもない : ブロックリスト優先 → ブロックリストに無いから、OK
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/hoge",  UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane",  IsPriorityHigh_OKList = false, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.OK } },
            //コンテントタイプがOKリストに無く、ブロックリストにもない : OKリスト優先 → OKリストに無いから、ブロック（OKされていない）
            new object[] { new AttachFileOKBlockTestData { RequestContentType = "image/hoge",  UploadOK_ContentTypeList = "ApplicaTion/PDf, APPLIcation/JSON, IMage/jp*, IMAge/pn[g|k], text/plane", UploadBlock_ContentTypeList = "text/HTML,text/javasc*, text/plane",  IsPriorityHigh_OKList = true, IsUploadOk_NoExtensionFile = true, ExpectStatusCode = HttpStatusCode.BadRequest, ExpectErrorCodeWheError = "E20411"}},
        };

        [TestMethod]
        [TestCaseSource(nameof(OKリストで_ブロック_OKチェック_コンテントタイプ))]
        [TestCaseSource(nameof(ブロックリストで_ブロック_OKチェック_コンテントタイプ))]
        [TestCaseSource(nameof(OKリストとブロックリスト_コンテントタイプと拡張子組み合わせ))]
        public void UploadAttachFile_正常系_コンテントOKリスト_ブロックリストによる制限のチェック()
        {
            TestContext.Run((AttachFileOKBlockTestData testSource) => TestExecute(testSource));
        }

        private void TestExecute(AttachFileOKBlockTestData testSource)
        {
            UnityContainer.RegisterInstance<bool>("IsEnableUploadContentCheck", testSource.IsEnableUploadContentCheck);
            UnityContainer.RegisterInstance<bool>("IsPriorityHigh_OKList", testSource.IsPriorityHigh_OKList);
            UnityContainer.RegisterInstance<bool>("IsUploadOk_NoExtensionFile", testSource.IsUploadOk_NoExtensionFile);
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ContentTypeList", new List<string>());
            UnityContainer.RegisterInstance<List<string>>("UploadOK_ExtensionList", new List<string>());
            UnityContainer.RegisterInstance<List<string>>("BlockContentTypeList", new List<string>());
            UnityContainer.RegisterInstance<List<string>>("BlockExtensionList", new List<string>());

            if (!string.IsNullOrEmpty(testSource.UploadOK_ContentTypeList))
            {
                UnityContainer.RegisterInstance<List<string>>("UploadOK_ContentTypeList", GetListFromConfigString(testSource.UploadOK_ContentTypeList));
            }
            if (!string.IsNullOrEmpty(testSource.UploadOK_ExtensionList))
            {
                UnityContainer.RegisterInstance<List<string>>("UploadOK_ExtensionList", GetListFromConfigString(testSource.UploadOK_ExtensionList));
            }
            if (!string.IsNullOrEmpty(testSource.UploadBlock_ContentTypeList))
            {
                UnityContainer.RegisterInstance<List<string>>("BlockContentTypeList", GetListFromConfigString(testSource.UploadBlock_ContentTypeList));
            }
            if (!string.IsNullOrEmpty(testSource.UploadBlock_ExtensionList))
            {
                UnityContainer.RegisterInstance<List<string>>("BlockExtensionList", GetListFromConfigString(testSource.UploadBlock_ExtensionList));
            }

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // 通常Content-Type変数名
            var contenttypename = "Content-Type";
            var collection = new ServiceCollection();
            var provider = collection.BuildServiceProvider();
            var httpContext = new DefaultHttpContext() { RequestServices = provider };

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);
            UnityCore.RegisterInstance(mockHttpContextAccessor.Object);

            var headers = httpContext.Request.Headers;
            headers.Remove("Authorization");
            headers.Add(contenttypename, new Microsoft.Extensions.Primitives.StringValues(testSource.RequestContentType));

            var action = CreateAction();
            System.Diagnostics.Debug.Print($"{contenttypename}:{JsonConvert.SerializeObject(testSource)}");
            // 処理実行
            var result = action.ExecuteAction();
            System.Diagnostics.Debug.Print($"result:{result.StatusCode} expect:{testSource.ExpectStatusCode}");

            // チェック
            result.StatusCode.Is(testSource.ExpectStatusCode);
            if (!result.IsSuccessStatusCode)
            {
                result.Content.ReadAsStringAsync().Result.Contains(testSource.ExpectErrorCodeWheError).IsTrue();
            }

            //Content-Typeが大文字小文字無視して処理されるか
            contenttypename = "content-type";
            collection = new ServiceCollection();
            provider = collection.BuildServiceProvider();
            httpContext = new DefaultHttpContext() { RequestServices = provider };

            mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.SetupGet(x => x.HttpContext).Returns(httpContext);
            UnityCore.RegisterInstance(mockHttpContextAccessor.Object);

            headers = httpContext.Request.Headers;
            headers.Remove("Authorization");
            headers.Add(contenttypename, new Microsoft.Extensions.Primitives.StringValues(testSource.RequestContentType));

            //処理実行
            System.Diagnostics.Debug.Print($"{contenttypename}:{JsonConvert.SerializeObject(testSource)}");
            result = action.ExecuteAction();
            System.Diagnostics.Debug.Print($"result:{result.StatusCode} expect:{testSource.ExpectStatusCode}");

            //チェック
            result.StatusCode.Is(testSource.ExpectStatusCode);
            if (!result.IsSuccessStatusCode)
            {
                result.Content.ReadAsStringAsync().Result.Contains(testSource.ExpectErrorCodeWheError).IsTrue();
            }
        }

        private static List<string> GetListFromConfigString(string configValue)
        {
            var ret = new List<string>();
            if (configValue != null)
            {
                var cfg = configValue.Split(',');
                foreach (var c1 in cfg)
                {
                    var c = c1.Replace(" ", "");
                    c = c.ToLower();
                    ret.Add(c);
                }
            }
            return ret;
        }

        public class AttachFileOKBlockTestData
        {
            public string RequestContentType { get; set; }
            public string RequestFileName { get; set; }
            public bool IsEnableUploadContentCheck { get; set; } = true;
            public string UploadOK_ContentTypeList { get; set; }
            public string UploadOK_ExtensionList { get; set; }
            public string UploadBlock_ContentTypeList { get; set; }
            public string UploadBlock_ExtensionList { get; set; }
            public bool IsPriorityHigh_OKList { get; set; }
            public bool IsUploadOk_NoExtensionFile { get; set; }
            public HttpStatusCode ExpectStatusCode { get; set; }
            public string ExpectErrorCodeWheError { get; set; }
        }
    }
}
