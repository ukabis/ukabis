using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_HistoryThrowAwayAction : UnitTestBase
    {
        private Guid _apiId = Guid.NewGuid();
        private Guid _controllerId = Guid.NewGuid();
        private Guid _vendorId = Guid.NewGuid();
        private Guid _systemId = Guid.NewGuid();
        private Guid _providerVendorId = Guid.NewGuid();
        private Guid _providerSystemId = Guid.NewGuid();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);
            UnityContainer.RegisterInstance(new Mock<ICache>().Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>("multiThread", perRequestDataContainer);
        }

        [TestMethod]
        public void OtherDelete()
        {
            var action = CreateAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.ExecuteAction().StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void NotHistory()
        {
            var action = CreateAction();
            action.IsDocumentHistory = new IsDocumentHistory(false);
            action.ExecuteAction().StatusCode.Is(HttpStatusCode.NotImplemented);
        }

        [TestMethod]
        public void NoId()
        {
            var action = CreateAction();
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            action.ExecuteAction().StatusCode.Is(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void IgnoreId()
        {
            var action = CreateAction();
            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            query.Add(new QueryStringKey("id"), new QueryStringValue("hoge"));
            action.Query = new QueryStringVO(query);
            action.ExecuteAction().StatusCode.Is(HttpStatusCode.NotFound);
        }

        [TestMethod]
        public void SuccessCase()
        {
            var action = CreateAction();
            action.ExecuteAction().StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void DocumentHistory_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", false);
            var action = CreateAction();
            action.IsDocumentHistory = new IsDocumentHistory(true);
            action.ExecuteAction().StatusCode.Is(HttpStatusCode.NotImplemented);
        }

        private HistoryThrowAwayAction CreateAction()
        {
            var action = UnityCore.Resolve<HistoryThrowAwayAction>();
            action.ApiId = new ApiId(_apiId.ToString());
            action.ControllerId = new ControllerId(_controllerId.ToString());
            action.SystemId = new SystemId(_systemId.ToString());
            action.VendorId = new VendorId(_vendorId.ToString());
            action.ProviderSystemId = new SystemId(_providerSystemId.ToString());
            action.ProviderVendorId = new VendorId(_providerVendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE);
            action.RepositoryKey = new RepositoryKey("/API/Private/Test/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.HistoryThrowAway);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            query.Add(new QueryStringKey("id"), new QueryStringValue("123"));

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
            mockDataRepository.Setup(x => x.DocumentVersionRepository.HistoryThrowAway(It.Is<DocumentKey>(y => y.Id == "123"))).Returns(true);

            var mockHistoryDataRepository = new Mock<INewDynamicApiDataStoreRepository>();


            action.HistoryEvacuationDataStoreRepository = mockHistoryDataRepository.Object;

            return action;
        }
    }
}
