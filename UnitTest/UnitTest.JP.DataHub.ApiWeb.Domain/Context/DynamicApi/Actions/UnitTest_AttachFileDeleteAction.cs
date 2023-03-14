using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
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
    public class UnitTest_AttachFileDeleteAction : UnitTestBase
    {
        private static Accept s_defaultAccept = new Accept("*/*");
        private static string s_vendorId = "7519869f-54c9-496d-b83c-884091069856";
        private static string s_systemId = "FE652EE4-E0E5-4C28-AA0D-885FE5AC22BA";
        private static string s_fileId = "D261DF4B-0305-46AD-9FF5-12026BEE3F89";
        private static string s_reposiitoryGroupId = Guid.NewGuid().ToString();
        private static string s_dataId = Guid.NewGuid().ToString();

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
        private DynamicApiAttachFileInformation _dynamicApiAttachFileInformation = new DynamicApiAttachFileInformation(s_fileId, "filename", "json", 100, "hoge/fuga/piyo.jpeg", null, false, null, null, true);

        Mock<IDocumentVersionRepository> _mockDocumentVersionRepository;
        Mock<IDynamicApiAttachFileRepository> _mockFileRepository;
        Mock<INewDynamicApiDataStoreRepository> _mockDataRepository;


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var mockRepository = new Mock<IDynamicApiRepository>();
            mockRepository.Setup(x => x.GetSchemaModelById(It.IsAny<DataSchemaId>())).Returns(new DataSchema(""));
            UnityContainer.RegisterInstance(mockRepository.Object);
        }

        [TestMethod]
        public void ExecuteAction_正常_履歴OFF()
        {
            var action = CreateAction();

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
            result.Headers.Contains("X-DocumentHistory").IsFalse();

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _mockFileRepository.Verify(x => x.DeleteAttachFile(It.IsAny<DynamicApiAttachFileInformation>()), Times.Once);
        }

        [TestMethod]
        public void ExecuteAction_正常_外部添付ファイル()
        {
            var action = CreateAction();
            var meta = GetInfoJson(null, s_fileId).ToJson();
            meta[nameof(DynamicApiAttachFileInformation.IsExternalAttachFile)] = true;
            meta[nameof(DynamicApiAttachFileInformation.ExternalAttachFile)] = new ExternalAttachFileInfomation(Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() }, Guid.NewGuid().ToString()).ToJson();
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(meta));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
            result.Headers.Contains("X-DocumentHistory").IsFalse();

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _mockFileRepository.Verify(x => x.DeleteAttachFile(It.IsAny<DynamicApiAttachFileInformation>()), Times.Never);
        }

        [TestMethod]
        public void ExecuteAction_正常_履歴ON_履歴登録あり()
        {
            var action = CreateAction(isDocumentHistory: true, fileId: _normalDocumentKey);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
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
            _savedDocumentHisotry.DocumentVersions.Last().LocationType.Is(DocumentHistory.StorageLocationType.Delete);
        }

        [TestMethod]
        public void ExecuteAction_正常_履歴ON_履歴登録なし()
        {
            var action = CreateAction(isDocumentHistory: true, fileId: _noUploadDocumentKey);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
            var historyHeader = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(result.Headers.GetValues("X-DocumentHistory").Single()).Single();
            historyHeader.IsNotNull();
            historyHeader.isSelfHistory.IsTrue();
            historyHeader.resourcePath.Is(action.ControllerRelativeUrl.Value);
            historyHeader.documents.Count().Is(1);
            historyHeader.documents.Single().documentKey.Is(_noUploadDocumentKey);
            historyHeader.documents.Single().versionKey.Is(_versionKeyInitialDelete);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            _savedDocumentHisotry.DocumentVersions.Count().Is(1);
            _savedDocumentHisotry.DocumentVersions.Last().VersionKey.Is(_versionKeyInitialDelete);
            _savedDocumentHisotry.DocumentVersions.Last().LocationType.Is(DocumentHistory.StorageLocationType.Delete);
        }

        [TestMethod]
        public void ExecuteAction_正常_Blockchainあり()
        {
            var mockBlockchainEventhub = new Mock<IBlockchainEventHubStoreRepository>();
            mockBlockchainEventhub.Setup(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()))
                .Returns(true)
                .Callback<string, RepositoryType, string>((id, type, version) =>
                {
                    _blockchainEventList.Add(JToken.FromObject(id));
                });
            _blockchainEventList = new List<JToken>();

            UnityContainer.RegisterInstance(mockBlockchainEventhub.Object);

            var action = CreateAction();
            action.IsEnableBlockchain = new IsEnableBlockchain(true);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();

            mockBlockchainEventhub.Verify(x => x.Delete(It.IsAny<string>(), It.IsAny<RepositoryType>(), It.IsAny<string>()), Times.Exactly(2));
            _blockchainEventList.Count().Is(2);
            _blockchainEventList.ElementAt(0).ToString().ToLower().Is(s_fileId.ToLower());
            _blockchainEventList.ElementAt(1).ToString().ToLower().Is(s_fileId.ToLower());
        }

        [TestMethod]
        public void ExecuteAction_正常_Key有り()
        {
            string key = "hoge";
            var action = CreateAction(key);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(s_fileId) }, { new QueryStringKey("Key"), new QueryStringValue(key) } });

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
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
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.Delete(It.IsAny<DeleteParam>())).Returns(new List<string> { Guid.NewGuid().ToString() });
            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>());
            mockFileRepository.Setup(x => x.DeleteAttachFile(It.IsAny<DynamicApiAttachFileInformation>()));
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void ExecuteAction_異常_Key不正()
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
        public void ExecuteAction_異常_設定無効()
        {
            var action = CreateAction();
            action.IsEnableAttachFile = new IsEnableAttachFile(false);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var action = CreateAction(isDocumentHistory: true, fileId: _normalDocumentKey);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NoContent);
            result.Content.ReadAsStringAsync().Result.Is("");
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            _mockDocumentVersionRepository.Verify(x => x.GetDocumentVersion(It.IsAny<DocumentKey>()), Times.Never);
            _mockDocumentVersionRepository.Verify(x => x.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()), Times.Never);
            _mockDocumentVersionRepository.Verify(x => x.SaveDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<RepositoryKeyInfo>(), It.IsAny<bool>()), Times.Never);
        }

        private AttachFileDeleteAction CreateAction(string key = null, bool isDocumentHistory = false, string fileId = null)
        {
            AttachFileDeleteAction action = UnityCore.Resolve<AttachFileDeleteAction>();
            action.ApiId = new ApiId(Guid.NewGuid().ToString());
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.SystemId = new SystemId(s_systemId);
            action.VendorId = new VendorId(s_vendorId);
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);
            action.RepositoryKey = new RepositoryKey("/API/Private/AttachfileTest/{FileId}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/AttachfileTest/DeleteAttachFile");
            action.ActionType = new ActionTypeVO(ActionType.AttachFileDelete);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.Accept = s_defaultAccept;
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            action.IsDocumentHistory = new IsDocumentHistory(isDocumentHistory);

            action.AttachFileBlobRepositoryGroupId = new RepositoryGroupId(s_reposiitoryGroupId);

            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() { { new QueryStringKey("FileId"), new QueryStringValue(fileId ?? s_fileId) } });

            _mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            _mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                _mockDataRepository.Object
            });
            RepositoryInfo metaDataRepositoryInfo = new RepositoryInfo(RepositoryType.AttachFileMetaCosmosDb.ToCode(), new Dictionary<string, bool>());
            RepositoryInfo fileDataRepositoryInfo = new RepositoryInfo(RepositoryType.AttachFileBlob.ToCode(), new Dictionary<string, bool>());
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>()
            {
                { metaDataRepositoryInfo }  ,
                { fileDataRepositoryInfo }
            });

            _mockDataRepository.Setup(x => x.Delete(It.IsAny<DeleteParam>())).Returns(new List<string> { s_dataId });
            _mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(GetInfoJson(key, fileId))) });
            _mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson(key, fileId))));
            _mockDataRepository.Setup(x => x.RepositoryInfo).Returns(metaDataRepositoryInfo);

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
                var resultDocumentVersionInitial = new DocumentHistory(_versionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.LowPerformance, new RepositoryKeyInfo(Guid.Parse(_low_repositoryGroupId), Guid.Parse(_low_physicalrepositoryId)), "location", null, documentMetainfo);
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

            _mockFileRepository.Setup(x => x.DeleteAttachFile(It.IsAny<DynamicApiAttachFileInformation>()));
            _mockFileRepository.Setup(x => x.GetUriWithSharedAccessSignature(It.IsAny<DynamicApiAttachFileInformation>()))
                .Returns(new Uri("http://localhost/"));
            action.AttachFileDynamicApiDataStoreRepository = _mockFileRepository.Object;

            return action;
        }

        private string GetInfoJson(string key = null, string fileId = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                _dynamicApiAttachFileInformation = new DynamicApiAttachFileInformation(fileId ?? s_fileId, "filename", "json", 100, "hoge/fuga/piyo.jpeg", key, false, null, null, true);
            }
            var json = JToken.Parse(_dynamicApiAttachFileInformation.Serialize());
            json[nameof(DynamicApiAttachFileInformation.Key)] = key;
            return json.ToString();
        }
    }
}
