using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_DriveOutDocumentAction : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
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
        }

        [TestMethod]
        public void MethodTypeChack()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30410.ToString(), actualMessage["error_code"]);

            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH);
            result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30410.ToString(), actualMessage["error_code"]);

            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30410.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void isEnableHistoryChack()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.IsDocumentHistory = new IsDocumentHistory(false);
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotImplemented);

            action.IsDocumentHistory = null;
            result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        public void idChack()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.Query = null;
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30402.ToString(), actualMessage["error_code"]);

            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            action.Query = new QueryStringVO(query);
            result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.BadRequest);
            actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30402.ToString(), actualMessage["error_code"]);

        }

        [TestMethod]
        public void HistoryNotFound()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            //JsonSearchResult repositoryData = new JsonSearchResult("", 0);
            List<JsonDocument> repositoryData = new List<JsonDocument>();

            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(repositoryData);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotFound);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30412.ToString(), actualMessage["error_code"]);
        }

        [TestMethod]
        public void 正常系()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();

            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse("{}")));
            DocumentHistories documentHistories = new DocumentHistories();
            var dhistory = new DocumentHistory("key", 1, DateTime.Now, "", DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid()), "");
            documentHistories.DocumentVersions.Add(dhistory);

            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(documentHistories);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()))
                .Callback<DocumentKey, DocumentHistory>((documentKey, documentHistory) =>
                {
                    documentKey.Id.Is("hoge");
                    documentHistory.LocationType.Is(DocumentHistory.StorageLocationType.LowPerformance);
                }).Returns(documentHistories);
            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockHistoryDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Returns(new RegisterOnceResult("hoge", new Dictionary<string, object>() { { "Container", "fuga" } }));
            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            perRequestDataContainer.XgetInternalAllField.IsTrue();
            mockDataRepository.Verify(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()), Times.Once);
            mockDataRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Once);
        }

        [TestMethod]
        public void ExecuteAction_キャッシュ有_キャッシュ削除エラー_NotImplementedException()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Throws(new NotImplementedException());
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateAction();
            action.CacheInfo = new CacheInfo(true, 0, "cacheKey");

            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse("{}")));
            DocumentHistories documentHistories = new DocumentHistories();
            var dhistory = new DocumentHistory("key", 1, DateTime.Now, "", DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid()), "");
            documentHistories.DocumentVersions.Add(dhistory);

            var keyRepository1 = "keyRepository1";
            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(documentHistories);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()))
                .Callback<DocumentKey, DocumentHistory>((documentKey, documentHistory) =>
                {
                    documentKey.Id.Is("hoge");
                    documentHistory.LocationType.Is(DocumentHistory.StorageLocationType.LowPerformance);
                }).Returns(documentHistories);
            mockDataRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository1);

            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockHistoryDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Returns(new RegisterOnceResult("hoge", new Dictionary<string, object>() { { "Container", "fuga" } }));
            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;


            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NoContent);

            mockDataRepository.Verify(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()), Times.Once);
            mockDataRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Once);

            mockCache.Verify(x => x.RemoveFirstMatch(It.Is<string>(it =>
                it == CacheManager.CreateKey(
                    "DynamicApiAction",
                    ProviderVendorId,
                    ProviderSystemId,
                    ControllerId)
            )), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_キャッシュ有_キャッシュ削除エラー_Exception()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.RemoveFirstMatch(It.IsAny<string>()))
                .Throws(new Exception());
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var action = CreateAction();
            action.CacheInfo = new CacheInfo(true, 0, "cacheKey");

            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryOnce(It.IsAny<QueryParam>())).Returns(new JsonDocument(JToken.Parse("{}")));
            DocumentHistories documentHistories = new DocumentHistories();
            var dhistory = new DocumentHistory("key", 1, DateTime.Now, "", DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid()), "");
            documentHistories.DocumentVersions.Add(dhistory);

            var keyRepository1 = "keyRepository1";
            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(documentHistories);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()))
                .Callback<DocumentKey, DocumentHistory>((documentKey, documentHistory) =>
                {
                    documentKey.Id.Is("hoge");
                    documentHistory.LocationType.Is(DocumentHistory.StorageLocationType.LowPerformance);
                }).Returns(documentHistories);
            mockDataRepository.Setup(x => x.KeyManagement.GetCacheKey(It.IsAny<QueryParam>(), It.IsAny<IPerRequestDataContainer>(), It.IsAny<IResourceVersionRepository>())).Returns(keyRepository1);

            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            mockHistoryDataRepository.Setup(x => x.RegisterOnce(It.IsAny<RegisterParam>())).Returns(new RegisterOnceResult("hoge", new Dictionary<string, object>() { { "Container", "fuga" } }));
            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;


            AssertEx.Catch<AggregateException>(() => action.ExecuteAction());

            mockDataRepository.Verify(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>()), Times.Once);
            mockDataRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Once);

            mockCache.Verify(x => x.RemoveFirstMatch(It.Is<string>(it =>
                it == CacheManager.CreateKey(
                    "DynamicApiAction",
                    ProviderVendorId,
                    ProviderSystemId,
                    ControllerId)
            )), Times.Exactly(1));

        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            //JsonSearchResult repositoryData = new JsonSearchResult("", 0);
            List<JsonDocument> repositoryData = new List<JsonDocument>();

            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(repositoryData);

            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();
            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotImplemented);
            result.Headers.Contains("X-DocumentHistory").IsFalse();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E30501.ToString(), actualMessage["error_code"]);
            mockHistoryDataRepository.Verify(x => x.DeleteOnce(It.IsAny<DeleteParam>()), Times.Never);
        }


        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();
        private Guid ProviderVendorId = Guid.NewGuid();
        private Guid ProviderSystemId = Guid.NewGuid();

        private DriveOutDocumentAction CreateAction()
        {
            DriveOutDocumentAction action = UnityCore.Resolve<DriveOutDocumentAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.ProviderSystemId = new SystemId(ProviderSystemId.ToString());
            action.ProviderVendorId = new VendorId(ProviderVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/Test/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.DriveOutDocument);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            query.Add(new QueryStringKey("id"), new QueryStringValue("hoge"));

            action.Query = new QueryStringVO(query);
            action.Accept = new Accept("*/*");
            action.IsEnableBlockchain = new IsEnableBlockchain(false);
            action.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>() { new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "connectionstring", false } }) });
            action.IsDocumentHistory = new IsDocumentHistory(true);

            //JsonSearchResult repositoryData = new JsonSearchResult("", 1);
            List<JsonDocument> repositoryData = new List<JsonDocument>();
            var mockDataRepository = new Mock<INewDynamicApiDataStoreRepository>();

            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockDataRepository.Object
            });
            mockDataRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>())).Returns(repositoryData);
            DocumentHistories documentHistories = new DocumentHistories();
            documentHistories.DocumentVersions.Add(new DocumentHistory("", 1, DateTime.Now, "", DocumentHistory.StorageLocationType.HighPerformance, new RepositoryKeyInfo(Guid.NewGuid(), Guid.NewGuid()), ""));

            mockDataRepository.Setup(x => x.DocumentVersionRepository.GetDocumentVersion(It.IsAny<DocumentKey>())).Returns(documentHistories);
            mockDataRepository.Setup(x => x.DocumentVersionRepository.UpdateDocumentVersion(It.IsAny<DocumentKey>(), It.IsAny<DocumentHistory>())).Returns(documentHistories);

            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();


            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;

            return action;
        }
    }
}
