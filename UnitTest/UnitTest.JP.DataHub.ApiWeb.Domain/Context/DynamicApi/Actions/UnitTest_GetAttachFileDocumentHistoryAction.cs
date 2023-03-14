using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class UnitTest_GetAttachFileDocumentHistoryAction : UnitTestBase
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
            UnityContainer.RegisterInstance("DynamicApi", new Mock<ICache>().Object);
        }

        [TestMethod]
        public void ExecuteAction_正常系_削除済み()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeySecond, documentkey: normalDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.NotFound);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30407.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_正常系_キー無し_履歴参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.Headers.ContentType.MediaType.Is("image/jpeg");
            var content = result.Content.ReadAsStringAsync().Result;
            content.Is("hoge");
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
        public void ExecuteAction_異常系_versionなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey);
            var dic = new Dictionary<QueryStringKey, QueryStringValue>(action.Query.Dic);
            dic.Remove(new QueryStringKey("version"));
            action.Query = new QueryStringVO(dic);

            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30403.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_正常系_キー無し_履歴notfound()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: nouploadDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.NotFound);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30404.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_正常系_キー無し_version形式不正()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: "noexistversion", documentkey: normalDocumentKey);
            var dic = new Dictionary<QueryStringKey, QueryStringValue>(action.Query.Dic);
            dic.Remove(new QueryStringKey("version"));
            dic.Add(new QueryStringKey("version"), new QueryStringValue("invalidversion"));
            action.Query = new QueryStringVO(dic);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30405.ToString(), actualMessage["error_code"]);
        }


        [TestMethod]
        public void ExecuteAction_正常系_キー無し_versionsnotfound()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: Guid.NewGuid().ToString(), documentkey: normalDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.NotFound);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E30406.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_正常系_キー無し_最新参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: normalDocumentKey);
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.Headers.ContentType.MediaType.Is("image/jpeg");
            var content = result.Content.ReadAsStringAsync().Result;
            content.Is("hoge");

            action.Query.ContainKey("version").IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_正常系_キーあり_履歴参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey, attachFileKey: "Key");
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.Headers.ContentType.MediaType.Is("image/jpeg");
            var content = result.Content.ReadAsStringAsync().Result;
            content.Is("hoge");
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey, attachFileKey: "Key");
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.NotImplemented);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            actualMessage["error_code"].Is("E30501");
        }

        [TestMethod]
        public void ExecuteAction_正常系_キーあり_最新参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: normalDocumentKey, attachFileKey: "Key");
            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.Headers.ContentType.MediaType.Is("image/jpeg");
            var content = result.Content.ReadAsStringAsync().Result;
            content.Is("hoge");

            action.Query.ContainKey("version").IsFalse();
        }

        [TestMethod]
        public void ExecuteAction_異常系_キー誤り_履歴参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey, attachFileKey: "Key");
            var dic = new Dictionary<QueryStringKey, QueryStringValue>(action.Query.Dic);
            dic.Remove(new QueryStringKey("Key"));
            dic.Add(new QueryStringKey("Key"), new QueryStringValue("invalidKey"));
            action.Query = new QueryStringVO(dic);

            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E20402.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_異常系_キー誤り_最新参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: normalDocumentKey, attachFileKey: "Key");
            var dic = new Dictionary<QueryStringKey, QueryStringValue>(action.Query.Dic);
            dic.Remove(new QueryStringKey("Key"));
            dic.Add(new QueryStringKey("Key"), new QueryStringValue("invalidKey"));
            action.Query = new QueryStringVO(dic);

            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E20402.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_異常系_キー指定忘れ_履歴参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey, attachFileKey: "Key");
            var dic = new Dictionary<QueryStringKey, QueryStringValue>(action.Query.Dic);
            dic.Remove(new QueryStringKey("Key"));
            action.Query = new QueryStringVO(dic);

            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E20402.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void ExecuteAction_異常系_キー指定忘れ_最新参照()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyLatest, documentkey: normalDocumentKey, attachFileKey: "Key");
            var dic = new Dictionary<QueryStringKey, QueryStringValue>(action.Query.Dic);
            dic.Remove(new QueryStringKey("Key"));
            action.Query = new QueryStringVO(dic);

            var result = action.ExecuteAction();

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = result.Content.ReadAsStringAsync().Result;
            var actualMessage = JToken.Parse(content);
            Assert.AreEqual(ErrorCodeMessage.Code.E20402.ToString(), actualMessage["error_code"]);
        }



        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Guid ProviderVendorId = Guid.NewGuid();
        private Guid ProviderSystemId = Guid.NewGuid();
        private readonly string nouploadDocumentKey = Guid.NewGuid().ToString();
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


        private GetAttachFileDocumentHistoryAction CreateAction(bool isDocumentHistory = true, string versionKey = null, string documentkey = null, string attachFileKey = null, bool isUserAccessOK = true)
        {
            GetAttachFileDocumentHistoryAction action = UnityCore.Resolve<GetAttachFileDocumentHistoryAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(ProviderSystemId.ToString());
            action.ProviderVendorId = new VendorId(ProviderVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/Test/{FileId}");
            action.ActionType = new ActionTypeVO(ActionType.GetAttachFileDocumentHistory);
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
            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(new List<JsonDocument>() { new JsonDocument(JToken.Parse(GetInfoJson(attachFileKey, true, documentkey))) });
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse(GetInfoJson(attachFileKey, true, documentkey))));
            mockFileRepository.Setup(x => x.GetAttachFileFileStream(It.IsAny<DynamicApiAttachFileInformation>())).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hoge")));
            DocumentDataId documentDataId = null;
            mockDataRepository.Setup(x => x.KeyManagement.IsIdValid(It.IsAny<JToken>(), It.IsAny<RegisterParam>(), It.IsAny<IResourceVersionRepository>(), out documentDataId)).Returns(() =>
            {
                return isUserAccessOK;
            });
            action.AttachFileDynamicApiDataStoreRepository = mockFileRepository.Object;

            var documentMetainfo = new DocumentHistoryAttachFileMetaData("image/jpeg", attachFileKey ?? null);
            RepositoryKeyInfo mockMetaDataRepositoryKeyInfo = new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid());

            var resultDocumentVersionLatest = new DocumentHistory(VersionKeyLatest, 3, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.HighPerformance, mockMetaDataRepositoryKeyInfo, "location", null, documentMetainfo);
            var resultDocumentVersionSecond = new DocumentHistory(VersionKeySecond, 2, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.Delete, null, null, null, documentMetainfo);
            var resultDocumentVersionInitial = new DocumentHistory(VersionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.Parse(high_repositoryGroupId), Guid.Parse(high_physicalrepositoryId)), "location", null, documentMetainfo);
            var resultDocumentVersionInitialUploaded = new DocumentHistory(VersionKeyInitial, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.LowPerformance, new RepositoryKeyInfo(Guid.Parse(low_repositoryGroupId), Guid.Parse(low_physicalrepositoryId)), "location", null, documentMetainfo);
            var resultDocumentVersionInitialDelete = new DocumentHistory(VersionKeyInitialDelete, 1, DateTime.Now, Guid.NewGuid().ToString(), DocumentHistory.StorageLocationType.Delete, null, null, null, documentMetainfo);
            var resultNormalHistory = new DocumentHistories("hogeId1", "hogeType", "hogeEtag", "hogePartitionKey", "hogeRegUser", "hogeRegDate", "hogeUpdUser", "hogeUpdDate", new List<DocumentHistory>() { { resultDocumentVersionLatest }, { resultDocumentVersionSecond }, { resultDocumentVersionInitialUploaded } }, "hogeId1");
            var resultInitialHistory = new DocumentHistories("hogeId1", "hogeType", "hogeEtag", "hogePartitionKey", "hogeRegUser", "hogeRegDate", "hogeUpdUser", "hogeUpdDate", new List<DocumentHistory>() { { resultDocumentVersionInitial } }, "hogeId1");
            var resultInitialDeleteHistory = new DocumentHistories("hogeId1", "hogeType", "hogeEtag", "hogePartitionKey", "hogeRegUser", "hogeRegDate", "hogeUpdUser", "hogeUpdDate", new List<DocumentHistory>() { { resultDocumentVersionInitialDelete } }, "hogeId1");
            var resultDeletedHistory = new DocumentHistories("hogeId1", "hogeType", "hogeEtag", "hogePartitionKey", "hogeRegUser", "hogeRegDate", "hogeUpdUser", "hogeUpdDate", new List<DocumentHistory>() { { resultDocumentVersionSecond }, { resultDocumentVersionInitial } }, "hogeId1");
            var resultDriveoutedHistory = new DocumentHistories("hogeId1", "hogeType", "hogeEtag", "hogePartitionKey", "hogeRegUser", "hogeRegDate", "hogeUpdUser", "hogeUpdDate", new List<DocumentHistory>() { { resultDocumentVersionInitial } }, "hogeId1");

            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.Is<DocumentKey>(_ => _.Id == nouploadDocumentKey))).Returns((DocumentHistories)null);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.Is<DocumentKey>(_ => _.Id == normalDocumentKey))).Returns(resultNormalHistory);

            var mockHistoryDataRepository = new Mock<INewBlobDataStoreRepository>();
            mockHistoryDataRepository.Setup(_ => _.GetUriWithSharedAccessSignature(It.IsAny<QueryParam>())).Returns(new Uri("http://localhost/"));
            mockHistoryDataRepository.Setup(_ => _.QueryToStream(It.IsAny<QueryParam>())).Returns(new MemoryStream(System.Text.Encoding.UTF8.GetBytes("hoge")));

            action.HistoryEvacuationDataStoreRepository = (INewDynamicApiDataStoreRepository)mockHistoryDataRepository.Object;

            return action;
        }

        private string GetInfoJson(string key = null, bool isUpload = true, string fileId = null)
        {
            var dynamicApiAttachFileInformation = new DynamicApiAttachFileInformation(fileId, "filename", "image/jpeg", 100, "hoge/fuga/piyo.jpeg", key, false, null, null, isUpload);

            var json = JToken.Parse(JsonConvert.SerializeObject(dynamicApiAttachFileInformation));
            var fi = json.SelectToken("FileId");
            fi.Parent.Remove();
            json["FileId"] = fileId;
            json.LastOrDefault().AddAfterSelf(new JProperty("id", "hogeId1"));
            return json.ToString();
        }

        [TestMethod]
        public void ExecuteAction_異常系_ベンダー領域超え()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction(isDocumentHistory: true, versionKey: VersionKeyInitial, documentkey: normalDocumentKey, isUserAccessOK: false);
            var result = action.ExecuteAction();

            //NotFoundになること
            result.StatusCode.Is(HttpStatusCode.NotFound);
        }
    }
}
