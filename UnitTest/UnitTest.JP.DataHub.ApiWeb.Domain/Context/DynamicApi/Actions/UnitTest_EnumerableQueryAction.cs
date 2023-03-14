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
using JP.DataHub.Com.DDD;
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
    public class UnitTest_EnumerableQueryAction : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ有()
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var expectResult = new List<string>()
            {
                new JObject
                {
                    ["field"] = "value"
                }.ToString(Formatting.None)
            };
            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult.Select(x => new JsonDocument(JToken.Parse(x))));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            var resultContent = result.Content as EnumerableQueryContent;
            var resultList = resultContent.GetContent().ToList();
            resultList.IsStructuralEqual(expectResult);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ無()
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            var expectResult = new List<string>();

            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult.Select(x => new JsonDocument(JToken.Parse(x))));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            var resultContent = result.Content as EnumerableQueryContent;
            var resultList = resultContent.GetContent().ToList();
            resultList.IsStructuralEqual(expectResult);

            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_NotImplementedException()
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });

            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Throws(new NotImplementedException());

            AssertEx.Catch<NotImplementedException>(() =>
            {
                var result = action.ExecuteAction();
                var resultContent = result.Content as EnumerableQueryContent;
                var resultList = resultContent.GetContent().ToList();
            });

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ有()
        {
            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var expectResult = new List<string>()
            {
                new JObject
                {
                    ["field"] = "value"
                }.ToString(Formatting.None)
            };
            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult.Select(x => new JsonDocument(JToken.Parse(x))));

            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            var resultContent = result.Content as EnumerableQueryContent;
            var resultList = resultContent.GetContent().ToList();
            resultList.IsStructuralEqual(expectResult);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            // 2つ目は無視
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(0));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ無_2つ目データ有()
        {
            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var expectResult1 = new List<string>();
            var expectResult2 = new List<string>()
            {
                new JObject
                {
                    ["field"] = "value"
                }.ToString(Formatting.None)
            };
            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult1.Select(x => new JsonDocument(JToken.Parse(x))));

            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult2.Select(x => new JsonDocument(JToken.Parse(x))));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            var resultContent = result.Content as EnumerableQueryContent;
            var resultList = resultContent.GetContent().ToList();
            expectResult2.IsStructuralEqual(resultList);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目データ無_2つ目データ無()
        {
            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            var expectResult = new List<string>();
            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult.Select(x => new JsonDocument(JToken.Parse(x))));

            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Returns(expectResult.Select(x => new JsonDocument(JToken.Parse(x))));

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            var resultContent = result.Content as EnumerableQueryContent;
            var resultList = resultContent.GetContent().ToList();
            resultList.IsStructuralEqual(expectResult);

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
        }


        [TestMethod]
        public void ExecuteAction_リポジトリ2つ_1つ目NotImplementedException_2つ目NotImplementedException()
        {
            var mockRepository1 = new Mock<INewDynamicApiDataStoreRepository>();
            var mockRepository2 = new Mock<INewDynamicApiDataStoreRepository>();

            var action = CreateEnumerableQueryAction();
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository1.Object,
                mockRepository2.Object
            });

            QueryParam queryParam = ValueObjectUtil.Create<QueryParam>(action);
            mockRepository1.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Throws(new NotImplementedException());

            mockRepository2.Setup(x => x.QueryEnumerable(It.IsAny<QueryParam>()))
                .Callback<QueryParam>(dynamicApiAction =>
                {
                    dynamicApiAction.IsStructuralEqual(queryParam);
                })
                .Throws(new NotImplementedException());



            AssertEx.Catch<NotImplementedException>(() =>
            {
                var result = action.ExecuteAction();
                var resultContent = result.Content as EnumerableQueryContent;
                var resultList = resultContent.GetContent().ToList();
            });

            // 引数はCallbackで判定
            mockRepository1.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(1));
            mockRepository2.Verify(x => x.QueryEnumerable(It.IsAny<QueryParam>()), Times.Exactly(0));
        }

        private Guid ApiId = Guid.NewGuid();
        private Guid ControllerId = Guid.NewGuid();
        private Guid VendorId = Guid.NewGuid();
        private Guid SystemId = Guid.NewGuid();

        private EnumerableQueryAction CreateEnumerableQueryAction()
        {
            EnumerableQueryAction action = UnityCore.Resolve<EnumerableQueryAction>();
            action.ApiId = new ApiId(ApiId.ToString());
            action.ControllerId = new ControllerId(ControllerId.ToString());
            action.SystemId = new SystemId(SystemId.ToString());
            action.VendorId = new VendorId(VendorId.ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.EnumerableQuery);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);

            action.PostDataType = new PostDataType("");
            action.ApiQuery = new ApiQuery("");

            return action;
        }
    }
}
