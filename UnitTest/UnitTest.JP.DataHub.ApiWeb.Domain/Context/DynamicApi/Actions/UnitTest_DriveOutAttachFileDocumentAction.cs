using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
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
    public class UnitTest_DriveOutAttachFileDocumentAction : UnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>("multiThread", perRequestDataContainer);

            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);

            var mockRepository = new Mock<IDynamicApiRepository>();
            mockRepository.Setup(x => x.GetSchemaModelById(It.IsAny<DataSchemaId>())).Returns(new DataSchema(""));
            UnityContainer.RegisterInstance(mockRepository.Object);
        }

        [TestMethod]
        public void ExecuteAction_異常系_履歴設定無し()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: false, versionKey: VersionKeyLatest, documentkey: normalDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.NotImplemented);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30501.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_異常系_FileIdなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey);
            var dic = new Dictionary<QueryStringKey, QueryStringValue>();
            action.Query = new QueryStringVO(dic);

            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30402.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_正常系_キー無し_履歴notfound()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: noUploadDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.NotFound);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
        }



        [TestMethod]
        public void ExecuteAction_正常系()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: normalDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.NoContent);

            action.ControllerSchema.IsNotNull();
            action.OperationInfo.IsAttachFileOperation.IsTrue();
            action.OperationInfo.AttachFileOperation.IsMetaQuery.IsFalse();
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: normalDocumentKey);
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotImplemented);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            actualMessage["error_code"].Is("E30501");
        }



        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Guid ProviderVendorId = Guid.NewGuid();
        private Guid ProviderSystemId = Guid.NewGuid();
        private readonly string noUploadDocumentKey = Guid.NewGuid().ToString();
        private readonly string normalDocumentKey = Guid.NewGuid().ToString();
        private readonly string nouploadversionKey = Guid.NewGuid().ToString();
        private readonly string normalversionKey = Guid.NewGuid().ToString();
        private readonly string latestversionKey = Guid.NewGuid().ToString();

        private readonly string VersionKeyNew = Guid.NewGuid().ToString();
        private readonly string VersionKeyLatest = Guid.NewGuid().ToString();
        private readonly string VersionKeySecond = Guid.NewGuid().ToString();
        private readonly string VersionKeyInitial = Guid.NewGuid().ToString();
        private readonly string VersionKeyInitialDelete = Guid.NewGuid().ToString();

        private readonly string high_physicalrepositoryId = Guid.NewGuid().ToString();
        private readonly string high_repositoryGroupId = Guid.NewGuid().ToString();
        private readonly string low_physicalrepositoryId = Guid.NewGuid().ToString();
        private readonly string low_repositoryGroupId = Guid.NewGuid().ToString();
        private DocumentHistories SavedDocumentHisotry;


        private DriveOutAttachFileDocumentAction CreateAction(bool isDocumentHistory = true, string versionKey = null, string documentkey = null, string attachFileKey = null)
        {
            DriveOutAttachFileDocumentAction action = UnityCore.Resolve<DriveOutAttachFileDocumentAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(ProviderSystemId.ToString());
            action.ProviderVendorId = new VendorId(ProviderVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/Test/{FileId}");
            action.ActionType = new ActionTypeVO(ActionType.DriveOutDocument);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            query.Add(new QueryStringKey("FileId"), new QueryStringValue(documentkey ?? "hoge"));
            if (versionKey != null)
            {
                query.Add(new QueryStringKey("version"), new QueryStringValue(versionKey));
            }
            if (attachFileKey != null)
            {
                query.Add(new QueryStringKey("Key"), new QueryStringValue(attachFileKey));
            }

            action.Query = new QueryStringVO(query);
            action.Accept = new Accept("*/*");
            RepositoryInfo metaRepositoryInfo = new RepositoryInfo("afm", new Dictionary<string, bool>() { { "connectionstring", false } });
            RepositoryInfo fileRepositoryInfo = new RepositoryInfo("afb", new Dictionary<string, bool>() { { "connectionstring", false } });
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>()
            {
                metaRepositoryInfo,fileRepositoryInfo
            });
            action.IsEnableAttachFile = new IsEnableAttachFile(true);
            action.IsDocumentHistory = new IsDocumentHistory(isDocumentHistory);

            //JsonSearchResult repositoryData = new JsonSearchResult("", 1);
            List<JsonDocument> repositoryData = new List<JsonDocument>();
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var mockFileRepository = new Mock<IDynamicApiAttachFileRepository>();

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Callback<RegisterParam>((regparam) =>
            {
                regparam.Json.IsNotNull();
                regparam.Json[nameof(DynamicApiAttachFileInformation.IsUploaded)].ToString().Is(false.ToString());
            });

            mockDataRepository.Setup(x => x.QueryEnumerable(It.Is<QueryParam>(_ => _.QueryString.Dic[new QueryStringKey("FileId")].Value == normalDocumentKey))).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(GetInfoJson(attachFileKey, true, documentkey))) });
            mockDataRepository.Setup(x => x.QueryEnumerable(It.Is<QueryParam>(_ => _.QueryString.Dic[new QueryStringKey("FileId")].Value == nouploadversionKey))).Returns((IEnumerable<JsonDocument>)null);
            mockDataRepository.Setup(x => x.QueryOnce(It.Is<QueryParam>(_ => _.QueryString.Dic[new QueryStringKey("FileId")].Value == normalDocumentKey))).Returns(new JsonDocument(JToken.Parse(GetInfoJson(attachFileKey, true, documentkey))));
            mockDataRepository.Setup(x => x.QueryOnce(It.Is<QueryParam>(_ => _.QueryString.Dic[new QueryStringKey("FileId")].Value == nouploadversionKey))).Returns((JsonDocument)null);
            mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hoge")));
            mockFileRepository.Setup(x => x.DeleteAttachFile(It.IsAny<DynamicApiAttachFileInformation>()));

            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            var documentMetainfo = new DocumentHistoryAttachFileMetaData("image/jpeg", attachFileKey ?? null);
            RepositoryKeyInfo mockMetaDataRepositoryKeyInfo = new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid());

            var resultDocumentVersionLatest = new DocumentHistory(VersionKeyLatest, 3, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.HighPerformance, mockMetaDataRepositoryKeyInfo, "location", null, documentMetainfo);
            var resultDocumentVersionSecond = new DocumentHistory(VersionKeySecond, 2, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.Delete, null, null, null, documentMetainfo);
            var resultDocumentVersionInitial = new DocumentHistory(VersionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.Parse(high_repositoryGroupId), Guid.Parse(high_physicalrepositoryId)), "location", null, documentMetainfo);
            var resultDocumentVersionInitialUploaded = new DocumentHistory(VersionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.LowPerformance, new RepositoryKeyInfo(Guid.Parse(low_repositoryGroupId), Guid.Parse(low_physicalrepositoryId)), "location", null, documentMetainfo);
            var resultDocumentVersionInitialDelete = new DocumentHistory(VersionKeyInitialDelete, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.Delete, null, null, null, documentMetainfo);
            var resultNormalHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionLatest }, { resultDocumentVersionSecond }, { resultDocumentVersionInitialUploaded } });
            var resultInitialHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionInitial } });
            var resultInitialDeleteHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionInitialDelete } });
            var resultDeletedHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionSecond }, { resultDocumentVersionInitial } });
            var resultDriveoutedHistory = new DocumentHistories(new List<DocumentHistory>() { { resultDocumentVersionInitial } });

            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.Is<DocumentKey>(_ => _.Id == noUploadDocumentKey))).Returns((DocumentHistories)null);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.Is<DocumentKey>(_ => _.Id == normalDocumentKey))).Returns(resultNormalHistory);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.SaveDocumentVersion(It.Is<DocumentKey>(_ => _.Id == noUploadDocumentKey), It.IsAny<RepositoryKeyInfo>(), It.Is<bool>(y => !y)))
                .Returns((DocumentKey dockey, RepositoryKeyInfo keyinfo, bool isdelete) =>
                {
                    SavedDocumentHisotry = resultInitialHistory;
                    return resultInitialHistory;
                });

            mockDataRepository.Setup(x => x.DocumentVersionRepository.SaveDocumentVersion(It.Is<DocumentKey>(_ => _.Id == noUploadDocumentKey), It.IsAny<RepositoryKeyInfo>(), It.Is<bool>(y => y)))
                .Returns((DocumentKey dockey, RepositoryKeyInfo keyinfo, bool isdelete) =>
                {
                    SavedDocumentHisotry = resultInitialDeleteHistory;
                    return resultInitialDeleteHistory;
                });
            mockDataRepository.Setup(x => x.DocumentVersionRepository.SaveDocumentVersion(It.Is<DocumentKey>(_ => _.Id == normalDocumentKey), It.IsAny<RepositoryKeyInfo>(), It.IsAny<DocumentHistory>(), It.IsAny<bool>()))
                    .Returns((DocumentKey dockey, RepositoryKeyInfo keyinfo, DocumentHistory history, bool isdelete) =>
                    {
                        var newHis = new DocumentHistory(VersionKeyNew, 4, DateTime.Now, Guid.NewGuid().ToString(), isdelete ? DocumentHistory.StorageLocationType.Delete : history.LocationType, null, null);

                        resultNormalHistory.DocumentVersions.Add(newHis); ;
                        SavedDocumentHisotry = resultNormalHistory;
                        return resultNormalHistory;
                    });
            mockDataRepository.Setup(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>())).Returns(resultNormalHistory);

            var mockHistoryDataRepository = new Mock<INewBlobDataStoreRepository>();
            mockHistoryDataRepository.Setup(_ => _.GetUriWithSharedAccessSignature(It.IsAny<QueryParam>())).Returns(new Uri("http://localhost/"));
            mockHistoryDataRepository.Setup(_ => _.QueryToStream(It.IsAny<QueryParam>())).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hoge")));

            var registerResult = new RegisterOnceResult("hoge.jpg", new Dictionary<string, object> { { "Container", "hogecontainername" } });
            mockHistoryDataRepository.Setup(_ => _.RegisterOnce(It.IsAny<RegisterParam>())).Returns(registerResult);

            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;

            return action;
        }

        private string GetInfoJson(string key = null, bool isUpload = true, string fileId = null)
        {
            var jsonSrt = new DynamicApiAttachFileInformation(fileId, "filename", "image/jpeg", 100, "hoge/fuga/piyo.jpeg", key, false, null, null, isUpload).Serialize();
            var json = JToken.Parse(jsonSrt);
            json.LastOrDefault().AddAfterSelf(new JProperty("id", "hogeId1"));
            json[nameof(DynamicApiAttachFileInformation.Key)] = key;
            return json.ToString();
        }
    }
}
