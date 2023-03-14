using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cosmos=Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_NewCosmosDbDataStoreRepository : UnitTestBase
    {
        private static readonly XRequestContinuation xRequestContinuation = new XRequestContinuation("DUMMY");
        private static readonly NativeQuery nativeQuery = new NativeQuery("SELECT * FROM c ", new Dictionary<string, object>());
        private static readonly NativeQuery nativeQueryWhere = new NativeQuery("SELECT * FROM c where id = 1", new Dictionary<string, object>());
        private static readonly string DELETE_DEFAULT_QUERY = "select c.id,c._self,c._partitionkey";
        private static readonly ApiQuery apiQuery = new ApiQuery("$filter=id eq '1'");
        private static readonly ApiQuery apiQueryDelete = new ApiQuery("SELECT * FROM c");
        private static readonly ApiQuery apiQueryDeleteWhere = new ApiQuery("SELECT * FROM c where id = 1");
        private static readonly string[] SelectEscapeKeywords = new string[] { "ASC", "AS", "AND", "BY", "BETWEEN", "CASE", "CAST", "CONVERT", "CROSS", "DESC", "DISTINCT", "ELSE", "END", "EXISTS", "FOR", "FROM", "GROUP", "HAVING", "IN", "INNER", "INSERT", "INTO", "IS", "JOIN", "LEFT", "LIKE", "NOT", "ON", "OR", "ORDER", "OUTER", "RIGHT", "SELECT", "SET", "THEN", "TOP", "UPDATE", "VALUE", "WHEN", "WHERE", "WITH" };

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>();
            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", false);
        }

        [TestMethod]
        public void QueryOnce_OK_0件() => ExecuteQueryOnce(0);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation無() => ExecuteQueryOnce(1);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation有() => ExecuteQueryOnce(1, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation無_NativeQuery有() => ExecuteQueryOnce(1, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation有_NativeQuery有() => ExecuteQueryOnce(1, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation無_ApiQuery有() => ExecuteQueryOnce(1, apiQuery: apiQuery);

        [TestMethod]
        public void QueryOnce_OK_1件_XRequestContinuation有_ApiQuery有() => ExecuteQueryOnce(1, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation無() => ExecuteQueryOnce(10);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation有() => ExecuteQueryOnce(10, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation無_NativeQuery有() => ExecuteQueryOnce(10, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation有_NativeQuery有() => ExecuteQueryOnce(10, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation無_ApiQuery有() => ExecuteQueryOnce(10, apiQuery: apiQuery);

        [TestMethod]
        public void QueryOnce_OK_複数件_XRequestContinuation有_ApiQuery有() => ExecuteQueryOnce(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_0件() => ExecuteQueryEnumerable(0);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation無() => ExecuteQueryEnumerable(1);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation有() => ExecuteQueryEnumerable(1, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation無_NativeQuery有() => ExecuteQueryEnumerable(1, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation有_NativeQuery有() => ExecuteQueryEnumerable(1, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation無_ApiQuery有() => ExecuteQueryEnumerable(1, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_1件_XRequestContinuation有_ApiQuery有() => ExecuteQueryEnumerable(1, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation無() => ExecuteQueryEnumerable(10);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation有() => ExecuteQueryEnumerable(10, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation無_NativeQuery有() => ExecuteQueryEnumerable(10, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation有_NativeQuery有() => ExecuteQueryEnumerable(10, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation無_ApiQuery有() => ExecuteQueryEnumerable(10, apiQuery: apiQuery);

        [TestMethod]
        public void QueryEnumerable_OK_複数件_XRequestContinuation有_ApiQuery有() => ExecuteQueryEnumerable(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_0件() => ExecuteQuery(0);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無() => ExecuteQuery(1);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無_NativeQuery有() => ExecuteQuery(1, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有_NativeQuery有() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無_NativeQuery有_NativeQueryが改行コードのみ() => ExecuteQuery(1, nativeQuery: new NativeQuery("\r\n", new Dictionary<string, object>()));

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有_NativeQuery有_NativeQueryが改行コードのみ() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation, nativeQuery: new NativeQuery("\r\n", new Dictionary<string, object>()));

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation無_ApiQuery有() => ExecuteQuery(1, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_1件_XRequestContinuation有_ApiQuery有() => ExecuteQuery(1, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無() => ExecuteQuery(10);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無_NativeQuery有() => ExecuteQuery(10, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_NativeQuery有() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無_NativeQuery有_NativeQueryが改行コードのみ() => ExecuteQuery(10, nativeQuery: new NativeQuery("\r\n", new Dictionary<string, object>()));

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_NativeQuery有_NativeQueryが改行コードのみ() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation, nativeQuery: new NativeQuery("\r\n", new Dictionary<string, object>()));

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation無_ApiQuery有() => ExecuteQuery(10, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_ApiQuery有() => ExecuteQuery(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery);

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_ApiQuery有_selectCount有_XRequestContinuationNeedsTopCount有効()
        {
            // ページング処理が実行される

            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", true);
            var selectCount = new SelectCount(10);
            ExecuteQuery(10, xRequestContinuation: xRequestContinuation, apiQuery: apiQuery, selectCount: selectCount);
        }

        [TestMethod]
        public void Query_OK_複数件_XRequestContinuation有_ApiQuery有_selectCount無_XRequestContinuationNeedsTopCount有効()
        {
            //ページング処理が実行されない

            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", true);

            var expectCount = 10;
            var xRequestContinuation = UnitTest_NewCosmosDbDataStoreRepository.xRequestContinuation;
            var apiQuery = UnitTest_NewCosmosDbDataStoreRepository.apiQuery;

            // ExecuteQueryの中身を外出し
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocument(mockJPDataHubDocumentDB, targetData, null); // xRequestContinuationを指定しているが、XRequestContinuationNeedsTopCount有効_selectCount無なので、無視される
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery), out XResponseContinuation xResponseContinuation).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);
            xResponseContinuation?.ContinuationString.Is(xRequestContinuation?.ContinuationString);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, null); // xRequestContinuationを指定しているが、XRequestContinuationNeedsTopCount有効_selectCount無なので、無視される

        }

        [TestMethod]
        public void RegisterOnce_OK()
        {
            var target = CreateDummyRepository();

            var expectCounts = new int[] { 0, 1 }; // データが0, 1件のケース
            foreach (var expectCount in expectCounts)
            {
                var json = expectCount == 0 ? new JObject() : new JObject { ["id"] = 1 };
                var targetId = json["id"]?.ToString();

                // MockでconnectionClientを上書き
                var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
                mockJPDataHubDocumentDB.Setup(x => x.UpsertDocument(
                    It.IsAny<JToken>(),
                    It.IsAny<Cosmos.ItemRequestOptions>()))
                    .Returns(targetId);
                SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

                // 対象メソッド実行
                var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(new Mock<IDynamicApiAction>().Object, json));

                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.Value.Is(targetId);

                // 呼ばれたか確認
                mockJPDataHubDocumentDB.Verify(x => x.UpsertDocument(
                    It.IsAny<JToken>(),
                    It.IsAny<Cosmos.ItemRequestOptions>()),
                    Times.Exactly(1));
            }
        }

        [TestMethod]
        public void RegisterOnce_OK_IsOverrideId()
        {
            var target = CreateDummyRepository();

            var expectCounts = new int[] { 0, 1 }; // データが0, 1件のケース
            foreach (var expectCount in expectCounts)
            {
                var json = expectCount == 0 ? new JObject() : new JObject { ["id"] = 1 };
                var targetId = json["id"]?.ToString();

                // MockでconnectionClientを上書き
                var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
                mockJPDataHubDocumentDB.Setup(x => x.UpsertDocument(
                    It.IsAny<JToken>(),
                    It.IsAny<Cosmos.ItemRequestOptions>()))
                    .Returns(targetId);
                SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

                // ダミーのActionを作成
                var dummyAction = new Mock<IDynamicApiAction>()
                    .SetupProperty(x => x.PartitionKey, new PartitionKey(""))
                    .SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/Public/UnitTest/RegisterOnce"))
                    .SetupProperty(x => x.IsVendor, new IsVendor(false))
                    .SetupProperty(x => x.VendorId, new VendorId(Guid.NewGuid().ToString()))
                    .SetupProperty(x => x.SystemId, new SystemId(Guid.NewGuid().ToString()))
                    .SetupProperty(x => x.OpenId, new OpenId(Guid.NewGuid().ToString()))
                    .SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false))
                    .SetupProperty(x => x.IsOverrideId, new IsOverrideId(true))
                    .SetupProperty(x => x.IsPerson, new IsPerson(false))
                    .Object;

                // 対象メソッド実行
                var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(dummyAction, json));

                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.Value.Is(targetId);

                // 呼ばれたか確認
                mockJPDataHubDocumentDB.Verify(x => x.UpsertDocument(
                    It.IsAny<JToken>(),
                    It.IsAny<Cosmos.ItemRequestOptions>()),
                    Times.Exactly(1));
            }
        }

        [TestMethod]
        public void DeleteOnce_OK()
        {
            var target = CreateDummyRepository();

            var expectCounts = new int[] { 0, 1 }; // データが0, 1件のケース
            foreach (var expectCount in expectCounts)
            {
                JToken json = expectCount == 0 ? new JObject() : new JObject { ["id"] = 1 };

                // MockでconnectionClientを上書き
                var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
                mockJPDataHubDocumentDB.Setup(x => x.DeleteDocument(
                    It.IsAny<JToken>(),
                    It.IsAny<JToken>()))
                    .Returns(true);
                SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

                // 対象メソッド実行
                target.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object, json));

                // 呼ばれたか確認
                mockJPDataHubDocumentDB.Verify(x => x.DeleteDocument(
                    It.IsAny<JToken>(),
                    It.IsAny<JToken>()),
                    Times.Exactly(1));
            }
        }

        [TestMethod]
        public void Delete_OK()
        {
            // 対象メソッド実行（Deleteは未実装）
            AssertEx.Catch<NotImplementedException>(() =>
            {
                var target = CreateDummyRepository();
                target.Delete(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object));
            });
        }

        [TestMethod]
        public void GetInternalAddWhereString_OK()
        {
            var target = CreateDummyRepository();

            // 対象メソッド実行
            var result = target.GetInternalAddWhereString(ValueObjectUtil.Create<QueryParam>(new Mock<IDynamicApiAction>().Object), out _);
            result.Is("c._Type=@_Type ");
        }

        [TestMethod]
        public void GetInternalAddWhereString_OK_All()
        {
            var target = CreateDummyRepository();

            // ダミーのActionを作成
            var dummyAction = new Mock<IDynamicApiAction>()
                        .SetupProperty(x => x.Query, new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>() {
                            { new QueryStringKey("_Type"), new QueryStringValue("AA") },
                            { new QueryStringKey("_Version"), new QueryStringValue("BB") },
                            { new QueryStringKey("_partitionkey"), new QueryStringValue("CC") },
                            { new QueryStringKey("_Owner_Id"), new QueryStringValue("DD") },
                            { new QueryStringKey("_ResourceSharing_Condition"), new QueryStringValue("EE") }
                        }))
                        .Object;

            // 対象メソッド実行
            var result = target.GetInternalAddWhereString(ValueObjectUtil.Create<QueryParam>(dummyAction), out _);
            result.Is("c._Type=@_Type AND c._Version=@_Version AND c._partitionkey=@_partitionkey AND c._Owner_Id=@_Owner_Id AND EE ");
        }

        [TestMethod]
        public void QueryOnce_Delete_OK() => ExecuteQueryOnceDelete(1, ActionType.DeleteData);

        [TestMethod]
        public void QueryOnce_Delete_OK_ApiQuery有_where無() => ExecuteQueryOnceDelete(1, ActionType.DeleteData, apiQuery: apiQueryDelete);

        [TestMethod]
        public void QueryOnce_Delete_OK_ApiQuery有_where有() => ExecuteQueryOnceDelete(1, ActionType.DeleteData, apiQuery: apiQueryDeleteWhere, where: "id = 1");

        [TestMethod]
        public void QueryOnce_Delete_OK_NativeQuery有_where無() => ExecuteQueryOnceDelete(1, ActionType.DeleteData, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_Delete_OK_NativeQuery有_where有() => ExecuteQueryOnceDelete(1, ActionType.DeleteData, nativeQuery: nativeQueryWhere, where: "id = 1");

        [TestMethod]
        public void QueryOnce_ODataDelete_OK() => ExecuteQueryOnceDelete(1, ActionType.ODataDelete);

        [TestMethod]
        public void QueryOnce_ODataDelete_OK_ApiQuery有_where無() => ExecuteQueryOnceDelete(1, ActionType.ODataDelete, apiQuery: apiQueryDelete);

        [TestMethod]
        public void QueryOnce_ODataDelete_OK_ApiQuery有_where有() => ExecuteQueryOnceDelete(1, ActionType.ODataDelete, apiQuery: apiQueryDeleteWhere, where: "id = 1");

        [TestMethod]
        public void QueryOnce_ODataDelete_OK_NativeQuery有_where無() => ExecuteQueryOnceDelete(1, ActionType.ODataDelete, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryOnce_ODataDelete_OK_NativeQuery有_where有() => ExecuteQueryOnceDelete(1, ActionType.ODataDelete, nativeQuery: nativeQueryWhere, where: "id = 1");

        [TestMethod]
        public void QueryOnce_DocumentHistoryDelete_OK() => ExecuteQueryOnceDelete(1, ActionType.DeleteData, isDocumentHistory: true);

        [TestMethod]
        public void QueryEnumerable_Delete_OK() => ExecuteQueryEnumerableDelete(1, ActionType.DeleteData);

        [TestMethod]
        public void QueryEnumerable_Delete_OK_ApiQuery有_where無() => ExecuteQueryEnumerableDelete(1, ActionType.DeleteData, apiQuery: apiQueryDelete);

        [TestMethod]
        public void QueryEnumerable_Delete_OK_ApiQuery有_where有() => ExecuteQueryEnumerableDelete(1, ActionType.DeleteData, apiQuery: apiQueryDeleteWhere, where: "id = 1");

        [TestMethod]
        public void QueryEnumerable_Delete_OK_NativeQuery有_where無() => ExecuteQueryEnumerableDelete(1, ActionType.DeleteData, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_Delete_OK_NativeQuery有_where有() => ExecuteQueryEnumerableDelete(1, ActionType.DeleteData, nativeQuery: nativeQueryWhere, where: "id = 1");

        [TestMethod]
        public void QueryEnumerable_ODataDelete_OK() => ExecuteQueryEnumerableDelete(1, ActionType.ODataDelete);

        [TestMethod]
        public void QueryOEnumerable_ODataDelete_OK_ApiQuery有_where無() => ExecuteQueryEnumerableDelete(1, ActionType.ODataDelete, apiQuery: apiQueryDelete);

        [TestMethod]
        public void QueryEnumerable_ODataDelete_OK_ApiQuery有_where有() => ExecuteQueryEnumerableDelete(1, ActionType.ODataDelete, apiQuery: apiQueryDeleteWhere, where: "id = 1");

        [TestMethod]
        public void QueryEnumerable_ODataDelete_OK_NativeQuery有_where無() => ExecuteQueryEnumerableDelete(1, ActionType.ODataDelete, nativeQuery: nativeQuery);

        [TestMethod]
        public void QueryEnumerable_ODataDelete_OK_NativeQuery有_where有() => ExecuteQueryEnumerableDelete(1, ActionType.ODataDelete, nativeQuery: nativeQueryWhere, where: "id = 1");

        [TestMethod]
        public void QueryEnumerable_DocumentHistoryDelete_OK() => ExecuteQueryEnumerableDelete(1, ActionType.DeleteData, isDocumentHistory: true);

        [TestMethod]
        public void Query_Delete_OK() => ExecuteQueryDelete(1, ActionType.DeleteData);

        [TestMethod]
        public void Query_Delete_OK_ApiQuery有_where無() => ExecuteQueryDelete(1, ActionType.DeleteData, apiQuery: apiQueryDelete);

        [TestMethod]
        public void Query_Delete_OK_ApiQuery有_where有() => ExecuteQueryDelete(1, ActionType.DeleteData, apiQuery: apiQueryDeleteWhere, where: "id = 1");

        [TestMethod]
        public void Query_Delete_OK_NativeQuery有_where無() => ExecuteQueryDelete(1, ActionType.DeleteData, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_Delete_OK_NativeQuery有_where有() => ExecuteQueryDelete(1, ActionType.DeleteData, nativeQuery: nativeQueryWhere, where: "id = 1");

        [TestMethod]
        public void Query_ODataDelete_OK() => ExecuteQueryDelete(1, ActionType.ODataDelete);

        [TestMethod]
        public void Query_ODataDelete_OK_ApiQuery有_where無() => ExecuteQueryDelete(1, ActionType.ODataDelete, apiQuery: apiQueryDelete);

        [TestMethod]
        public void Query_ODataDelete_OK_ApiQuery有_where有() => ExecuteQueryDelete(1, ActionType.ODataDelete, apiQuery: apiQueryDeleteWhere, where: "id = 1");

        [TestMethod]
        public void Query_ODataDelete_OK_NativeQuery有_where無() => ExecuteQueryDelete(1, ActionType.ODataDelete, nativeQuery: nativeQuery);

        [TestMethod]
        public void Query_ODataDelete_OK_NativeQuery有_where有() => ExecuteQueryDelete(1, ActionType.ODataDelete, nativeQuery: nativeQueryWhere, where: "id = 1");

        [TestMethod]
        public void Query_DocumentHistoryDelete_OK() => ExecuteQueryDelete(1, ActionType.DeleteData, isDocumentHistory: true);

        /// <summary>
        /// QueryOnceのテストを実行する
        /// </summary>
        /// <param name="expectCount"></param>
        /// <param name="xRequestContinuation"></param>
        /// <param name="nativeQuery"></param>
        /// <param name="apiQuery"></param>
        private void ExecuteQueryOnce(int expectCount, XRequestContinuation xRequestContinuation = null, NativeQuery nativeQuery = null, ApiQuery apiQuery = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocument(mockJPDataHubDocumentDB, targetData, xRequestContinuation);
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.QueryOnce(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery));
            if (targetData.Count() == 0)
            {
                // データが0件の場合結果がnullで返却されること
                result.IsNull();
            }
            else
            {
                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.IsStructuralEqual(expectData.FirstOrDefault());
            }

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, xRequestContinuation);
        }

        private void ExecuteQueryOnceDelete(int expectCount, ActionType actionType, NativeQuery nativeQuery = null, ApiQuery apiQuery = null, string where = null, bool isDocumentHistory = false)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateDeleteTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocumentDelete(mockJPDataHubDocumentDB, targetData, where ?? "", isDocumentHistory);
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyDeleteAction(actionType, isDocumentHistory, apiQuery);
            // 対象メソッド実行
            var result = target.QueryOnce(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery, null));

            // Mockの期待値が返却されること
            result.IsStructuralEqual(expectData.FirstOrDefault());

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, null);
        }

        /// <summary>
        /// QueryEnumerableのテストを実行する
        /// </summary>
        /// <param name="expectCount"></param>
        /// <param name="xRequestContinuation"></param>
        /// <param name="nativeQuery"></param>
        /// <param name="apiQuery"></param>
        private void ExecuteQueryEnumerable(int expectCount, XRequestContinuation xRequestContinuation = null, NativeQuery nativeQuery = null, ApiQuery apiQuery = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocument(mockJPDataHubDocumentDB, targetData, xRequestContinuation);
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery)).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, xRequestContinuation);
        }

        private void ExecuteQueryEnumerableDelete(int expectCount, ActionType actionType, NativeQuery nativeQuery = null, ApiQuery apiQuery = null, string where = null, bool isDocumentHistory = false)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateDeleteTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocumentDelete(mockJPDataHubDocumentDB, targetData, where ?? "", isDocumentHistory);
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyDeleteAction(actionType, isDocumentHistory, apiQuery);

            // 対象メソッド実行
            var result = target.QueryEnumerable(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery, null)).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, null);
        }

        /// <summary>
        /// Queryのテストを実行する
        /// </summary>
        /// <param name="expectCount"></param>
        /// <param name="xRequestContinuation"></param>
        /// <param name="nativeQuery"></param>
        /// <param name="apiQuery"></param>
        /// <param name="selectCount"></param>
        private void ExecuteQuery(int expectCount, XRequestContinuation xRequestContinuation = null, NativeQuery nativeQuery = null, ApiQuery apiQuery = null, SelectCount selectCount = null)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateQueryTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocument(mockJPDataHubDocumentDB, targetData, xRequestContinuation);
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(xRequestContinuation, apiQuery);

            // 対象メソッド実行
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery, selectCount), out XResponseContinuation xResponseContinuation).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);
            xResponseContinuation?.ContinuationString.Is(xRequestContinuation?.ContinuationString);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, xRequestContinuation);
        }

        private void ExecuteQueryDelete(int expectCount, ActionType actionType, NativeQuery nativeQuery = null, ApiQuery apiQuery = null, string where = null, bool isDocumentHistory = false)
        {
            var target = CreateDummyRepository();
            var (targetData, expectData) = CreateDeleteTestData(expectCount);

            // MockでconnectionClientを上書き
            var mockJPDataHubDocumentDB = new Mock<IJPDataHubCosmosDb>();
            SetUpQueryDocumentDelete(mockJPDataHubDocumentDB, targetData, where ?? "", isDocumentHistory);
            SetDummyConnectionClient(mockJPDataHubDocumentDB.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyDeleteAction(actionType, isDocumentHistory, apiQuery);

            // 対象メソッド実行
            var result = target.Query(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery, null), out XResponseContinuation xResponseContinuation).ToList();

            // Mockの期待値が返却されること
            result.Count.Is(expectCount);
            result.IsStructuralEqual(expectData);

            // 呼ばれたか確認
            VerifyQueryDocument(mockJPDataHubDocumentDB, null);
        }

        /// <summary>
        /// ダミーのRepositoryを作成する
        /// </summary>
        /// <returns></returns>
        private NewCosmosDbDataStoreRepository CreateDummyRepository()
        {
            var target = UnityContainer.Resolve<NewCosmosDbDataStoreRepository>();
            target.RepositoryInfo = new RepositoryInfo("ddb", new Dictionary<string, bool>() { { "connectionstring", false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(
                It.IsAny<RepositoryKey>(),
                It.IsAny<XVersion>()))
                .Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;
            return target;
        }

        /// <summary>
        /// テスト用データを作成する（Query系用）
        /// </summary>
        /// <returns></returns>
        private (IEnumerable<JToken>, IEnumerable<JsonDocument>) CreateQueryTestData(int expectCount)
        {
            var targetData = Enumerable.Range(1, expectCount)
                .Select(x => (JToken)new JObject { ["id"] = x })
                .ToList();

            var expectData = Enumerable.Range(1, expectCount)
                .Select(x => new JsonDocument(new JObject { ["id"] = x }))
                .ToList();
            return (targetData, expectData);
        }

        /// <summary>
        /// テスト用データを作成する（Delete系用）
        /// </summary>
        /// <returns></returns>
        private (IEnumerable<JToken>, IEnumerable<JsonDocument>) CreateDeleteTestData(int expectCount)
        {
            var targetData = Enumerable.Range(1, expectCount)
                .Select(x => (JToken)new JObject { ["id"] = x, ["_self"] = "self-" + x, ["_partitionkey"] = "partitionkey-" + x })
                .ToList();

            var expectData = Enumerable.Range(1, expectCount)
                .Select(x => new JsonDocument(new JObject { ["id"] = x, ["_self"] = "self-" + x, ["_partitionkey"] = "partitionkey-" + x }))
                .ToList();
            return (targetData, expectData);
        }

        /// <summary>
        /// ダミーのActionを作成する（Query系用）
        /// </summary>
        /// <returns></returns>
        private IDynamicApiAction CreateDummyQueryAction(XRequestContinuation xRequestContinuation = null, ApiQuery apiQuery = null)
        {
            return new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ApiQuery, apiQuery ?? new ApiQuery(""))
                .SetupProperty(x => x.XRequestContinuation, xRequestContinuation)
                .SetupProperty(x => x.ActionType, new ActionTypeVO(ActionType.Query))
                .Object;
        }

        /// <summary>
        /// ダミーのActionを作成する（Delete系用）
        /// </summary>
        /// <returns></returns>
        private IDynamicApiAction CreateDummyDeleteAction(ActionType actionType, bool isDocumentHistory, ApiQuery apiQuery = null)
        {
            return new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.ApiQuery, apiQuery ?? new ApiQuery(""))
                .SetupProperty(x => x.XRequestContinuation, null)
                .SetupProperty(x => x.ActionType, new ActionTypeVO(actionType))
                .SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/TEST"))
                .SetupProperty(x => x.IsDocumentHistory, new IsDocumentHistory(isDocumentHistory))
                .Object;
        }

        /// <summary>
        /// IJPDataHubCosmosDbのMockをセットアップする
        /// </summary>
        /// <param name="mockJPDataHubDocumentDB"></param>
        /// <param name="targetData"></param>
        /// <param name="xRequestContinuation"></param>
        private void SetUpQueryDocument(Mock<IJPDataHubCosmosDb> mockJPDataHubDocumentDB, IEnumerable<JToken> targetData, XRequestContinuation xRequestContinuation)
        {
            if (xRequestContinuation == null)
            {
                mockJPDataHubDocumentDB.Setup(x => x.QueryDocument(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<bool>()))
                    .Returns(targetData);
            }
            else
            {
                string responseContinuation = xRequestContinuation.ContinuationString;
                mockJPDataHubDocumentDB.Setup(x => x.QueryDocumentContinuation(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    out responseContinuation))
                    .Returns(targetData.ToList());
            }
        }

        /// <summary>
        /// IJPDataHubCosmosDbのMockをセットアップする
        /// </summary>
        /// <param name="mockJPDataHubDocumentDB"></param>
        /// <param name="targetData"></param>
        /// <param name="where"></param>
        private void SetUpQueryDocumentDelete(Mock<IJPDataHubCosmosDb> mockJPDataHubDocumentDB, IEnumerable<JToken> targetData, string where, bool isDocumentHistory)
        {
            if (isDocumentHistory)
            {
                mockJPDataHubDocumentDB.Setup(x => x.QueryDocument(
     It.Is<string>(y => y.StartsWith("SELECT *  FROM c") && y.Contains("where")),
     It.IsAny<IDictionary<string, object>>(),
     It.IsAny<bool>()))
 .Returns(targetData);
            }
            else
            {
                mockJPDataHubDocumentDB.Setup(x => x.QueryDocument(
                        It.Is<string>(y => y.StartsWith(DELETE_DEFAULT_QUERY) && y.Contains("where") && y.Contains(where)),
                        It.IsAny<IDictionary<string, object>>(),
                        It.IsAny<bool>()))
                    .Returns(targetData);
            }
        }

        /// <summary>
        /// IJPDataHubCosmosDbのMockのメソッドが呼ばれたか確認する
        /// </summary>
        /// <param name="mockJPDataHubDocumentDB"></param>
        /// <param name="xRequestContinuation"></param>
        private void VerifyQueryDocument(Mock<IJPDataHubCosmosDb> mockJPDataHubDocumentDB, XRequestContinuation xRequestContinuation)
        {
            if (xRequestContinuation == null)
            {
                mockJPDataHubDocumentDB.Verify(x => x.QueryDocument(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<bool>()),
                    Times.Exactly(1));
            }
            else
            {
                string responseContinuation = xRequestContinuation.ContinuationString;
                mockJPDataHubDocumentDB.Verify(x => x.QueryDocumentContinuation(
                    It.IsAny<string>(),
                    It.IsAny<IDictionary<string, object>>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    out responseContinuation),
                    Times.Exactly(1));
            }
        }

        /// <summary>
        /// connectionClient（静的フィールド）を上書きする
        /// </summary>
        /// <param name="jPDataHubCosmosDB"></param>
        private void SetDummyConnectionClient(IJPDataHubCosmosDb jPDataHubCosmosDB)
        {
            var field = typeof(NewCosmosDbDataStoreRepository).GetField("connectionClient", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, new ConcurrentDictionary<string, IJPDataHubCosmosDb>(new Dictionary<string, IJPDataHubCosmosDb>() { { "connectionstring", jPDataHubCosmosDB } }));
        }

        [TestMethod]
        public void BuildSelect_EscapeKeyword_Test()
        {
            var templeteSchema = @"
{
  'description': 'description',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
    '[REPLACEPROPERTYNAME]': {
      'title': 'エスケープ',
      'type': 'string'
    },
    'TestProperty': {
      'title': '既存',
      'type': 'string'
    }
  },
  'required': [
    'TestProperty'
  ]
}";
            SelectEscapeKeywords.ToList().ForEach(input =>
            {
                var dummyAction = new Mock<IDynamicApiAction>()
                    .SetupProperty(x => x.ApiQuery, new ApiQuery(""))
                    .SetupProperty(x => x.ActionType, new ActionTypeVO(ActionType.Query))
                    .SetupProperty(x => x.ResponseSchema, new DataSchema(templeteSchema.Replace("[REPLACEPROPERTYNAME]", input)))
                    .Object;

                var result = BuildSelect(ValueObjectUtil.Create<QueryParam>(dummyAction, nativeQuery));

                Assert.IsTrue(result.Contains($"c.TestProperty,"));
                Assert.IsFalse(result.Contains($"c.{input},"));
                Assert.IsTrue(result.Contains($"c[\"{input}\"],"));
            });
        }

        [TestMethod]
        public void QueryConditionBuild_EscapeKeyword_Test()
        {
            SelectEscapeKeywords.ToList().ForEach(input =>
            {
                var dummyAction = new Mock<IDynamicApiAction>()
                    .SetupProperty(x => x.ApiQuery, new ApiQuery(""))
                    .SetupProperty(x => x.ActionType, new ActionTypeVO(ActionType.Query))

                    .Object;

                var result = QueryConditionBuild(string.Empty, input);

                Assert.IsTrue(result.Contains($"c[\"{input}\"]"));
                Assert.IsFalse(result.Contains($"c.{input}"));
                Assert.IsTrue(result.Contains($"@_{input}"));
            });
        }

        private static string BuildSelect(QueryParam param)
        {
            var target = new NewCosmosDbDataStoreRepository { PerRequestDataContainer = new PerRequestDataContainer() };
            return (string)target.GetType().InvokeMember("BuildSelect", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, target, new object[] { param });
        }

        private static string QueryConditionBuild(string query, string conditionKey)
        {
            return (string)typeof(NewCosmosDbDataStoreRepository).InvokeMember("QueryConditionBuild", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static, null, null, new object[] { query, conditionKey });
        }
    }
}