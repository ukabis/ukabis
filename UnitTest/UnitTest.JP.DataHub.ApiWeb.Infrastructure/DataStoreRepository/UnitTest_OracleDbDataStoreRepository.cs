using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.ApiWeb.Infrastructure.Sql;
using JP.DataHub.Infrastructure.Database.Data;
using JP.DataHub.UnitTest.Com;
using System.Text.RegularExpressions;
using JP.DataHub.Com.Web.Authentication;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_OracleDbDataStoreRepository : UnitTestBase
    {
        private const string RepositoryName = "Oracle";

        private readonly int DocumentCount = 10;
        private readonly string ConnectionString = Guid.NewGuid().ToString();


        [TestInitialize]
        public override void TestInitialize()
        {
            var mockDb = new Mock<IJPDataHubRdbms>();
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            var mockDynamicApi = new Mock<IDynamicApiRepository>();

            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterType<INewDynamicApiDataStoreRdbmsRepository, OracleDbDataStoreRepository>();
            UnityContainer.RegisterInstance<IJPDataHubRdbms>(mockDb.Object);
            UnityContainer.RegisterInstance<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object);
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApi.Object);
            UnityContainer.RegisterInstance<bool>("XRequestContinuationNeedsTopCount", false);
            UnityContainer.RegisterInstance(Configuration);

            var dataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);
        }

        #region Query

        #region QueryOnce

        [TestMethod]
        public void QueryOnce_OK_0件() => ExecuteQueryOnce(0);

        [TestMethod]
        public void QueryOnce_OK_1件() => ExecuteQueryOnce(1);

        [TestMethod]
        public void QueryOnce_OK_複数件() => ExecuteQueryOnce(10);

        [TestMethod]
        public void QueryOnce_OK_任意null不許可項目() => ExecuteQueryOnce(1, true);

        [TestMethod]
        public void QueryOnce_OK_バージョン情報()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(new List<JToken>() { JToken.Parse($@"
{{ 
    ""versioninfo"": ""{{ 'key1': 'value1', 'key2': 'value2' }}"",
    ""_etag"": 1
}}") });
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction, new OperationInfo(OperationInfo.OperationType.VersionInfo), new NativeQuery(Guid.NewGuid().ToString(), new Dictionary<string, object>(), true));
            var result = target.QueryOnce(queryParam);
            result.Value.ToString().Is(JToken.Parse($@"
{{ 
    ""key1"": ""value1"", 
    ""key2"": ""value2"", 
    ""_etag"": ""1""
}}").ToString());

            // 呼ばれたか確認
            VerifyQueryDocument(mockDb, pagingInfo);
        }

        [TestMethod]
        public void QueryOnce_NG_SqlException_207_OracleDBTransientExceptionDetector_miss()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };

            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Throws(CreateSqlException((int)207)); // target
            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());
            var result = target.QueryOnce(queryParam);

            result.IsNull();

            // OracleDbTransientExceptionDetectorに含まれないのでretryはされない
            mockDb.Verify(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryOnce_NG_SqlException_OracleDbTransientExceptionDetector_hit()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };

            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Throws(CreateSqlException((int)-2)); // target
            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());

            var exception = AssertEx.Throws<SqlException>(() => target.QueryOnce(queryParam));
            // OracleDbTransientExceptionDetectorに含まれるのでretry
            mockDb.Verify(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()),
                Times.Exactly(3));

        }
        #endregion

        #region Query

        [TestMethod]
        public void Query_OK_0件() => ExecuteQuery(0);

        [TestMethod]
        public void Query_OK_1件() => ExecuteQuery(1);

        [TestMethod]
        public void Query_OK_複数件() => ExecuteQuery(10);

        [TestMethod]
        public void Query_OK_複数件_ページング_先頭ページ() => ExecuteQuery(new PagingInfo() { IsPaging = true, OffsetCount = 0, FetchCount = 3 });

        [TestMethod]
        public void Query_OK_複数件_ページング_中間ページ() => ExecuteQuery(new PagingInfo() { IsPaging = true, OffsetCount = 3, FetchCount = 3 });

        [TestMethod]
        public void Query_OK_複数件_ページング_最終ページ() => ExecuteQuery(new PagingInfo() { IsPaging = true, OffsetCount = 9, FetchCount = 3 });

        [TestMethod]
        public void Query_OK_任意null不許可項目() => ExecuteQuery(1, true);

        [TestMethod]
        public void Query_NG_SqlException_207_OracleDbTransientExceptionDetector_miss()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Throws(CreateSqlException((int)207)); // target

            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());
            var result = target.Query(queryParam, out var xResponseContinuation);

            result.Count().Is(0);
            xResponseContinuation.IsNull();

            // OracleDbTransientExceptionDetectorに含まれないのでretryはされない
            mockDb.Verify(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void Query_NG_SqlException_OracleDbTransientExceptionDetector_hit()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Throws(CreateSqlException((int)-2)); // target

            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());

            var exception = AssertEx.Throws<SqlException>(() => target.Query(queryParam, out var xResponseContinuation));
            // OracleDbTransientExceptionDetectorに含まれるのでretry
            mockDb.Verify(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()),
                Times.Exactly(3));
        }

        #endregion

        #region QueryEnumerable

        [TestMethod]
        public void QueryEnumerable_OK_0件() => ExecuteQueryEnumerable(0);

        [TestMethod]
        public void QueryEnumerable_OK_1件() => ExecuteQueryEnumerable(1);

        [TestMethod]
        public void QueryEnumerable_OK_複数件() => ExecuteQueryEnumerable(10);

        [TestMethod]
        public void QueryEnumerable_NG_SqlException_OracleDbTransientExceptionDetector_miss()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Throws(CreateSqlException((int)207)); // target

            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());
            var result = target.QueryEnumerable(queryParam).ToList();

            result.Count().Is(0);

            // OracleDbTransientExceptionDetectorに含まれないのでretryはされない
            mockDb.Verify(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void QueryEnumerable_NG_SqlException_OracleDbTransientExceptionDetector_hit()
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Throws(CreateSqlException((int)-2)); // target

            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());

            var exception = AssertEx.Throws<SqlException>(() => target.QueryEnumerable(queryParam).ToList());
            // OracleDbTransientExceptionDetectorに含まれるのでretry
            mockDb.Verify(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()),
                Times.Exactly(3));
        }

        #endregion


        /// <summary>
        /// QueryOnceのテストを実行する
        /// </summary>
        private void ExecuteQueryOnce(int expectCount, bool testDataWithNull = false)
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = expectCount };
            ExecuteQueryOnce(pagingInfo, testDataWithNull);
        }
        /// <summary>
        /// QueryOnceのテストを実行する
        /// </summary>
        private void ExecuteQueryOnce(PagingInfo pagingInfo = null, bool testDataWithNull = false)
        {
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            (var targetData, var expectData) = testDataWithNull
                ? CreateQueryTestDataWithNull((int)pagingInfo.FetchCount.Value)
                : CreateQueryTestData((int)pagingInfo.FetchCount.Value);
            (var target, var mockDb) = CreateDummyRepositoryForQuery(targetData);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());
            var result = target.QueryOnce(queryParam);

            if (targetData.Count() == 0)
            {
                // データが0件の場合結果がnullで返却されること
                result.IsNull();
            }
            else
            {
                // データが1件以上の場合Mockの1件目と同じ期待値が返却されること
                result.Value.ToString().Is(expectData.FirstOrDefault().Value.ToString());
            }

            // 呼ばれたか確認
            VerifyQueryDocument(mockDb, pagingInfo);
        }

        /// <summary>
        /// Queryのテストを実行する
        /// </summary>
        private void ExecuteQuery(int expectCount, bool testDataWithNull = false)
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = expectCount };
            ExecuteQuery(pagingInfo, testDataWithNull);
        }
        /// <summary>
        /// Queryのテストを実行する
        /// </summary>
        private void ExecuteQuery(PagingInfo pagingInfo = null, bool testDataWithNull = false)
        {
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            if (pagingInfo == null)
            {
                pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null };
            }
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            (var targetData, var expectData) = testDataWithNull
                ? CreateQueryTestDataWithNull((int)pagingInfo.FetchCount.Value)
                : CreateQueryTestData((int)pagingInfo.FetchCount.Value);
            (var target, var mockDb) = CreateDummyRepositoryForQuery(targetData);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());
            var result = target.Query(queryParam, out var xResponseContinuation);

            if (targetData.Count() == 0)
            {
                // データが0件の場合結果が0件であること
                result.Count().Is(0);
            }
            else
            {
                // データが1件以上の期待値が返却されること
                var expectedDataList = expectData.ToList();
                result.Count().Is(expectData.Count());
                for (var i = 0; i < result.Count(); i++)
                {
                    result[i].Value.ToString().Is(expectedDataList[i].Value.ToString());
                }
            }

            if (pagingInfo.IsPaging && pagingInfo.OffsetCount + pagingInfo.FetchCount < DocumentCount)
            {
                xResponseContinuation.ContinuationString.Is((pagingInfo.OffsetCount + pagingInfo.FetchCount).ToString());
            }
            else if (pagingInfo.IsPaging)
            {
                xResponseContinuation.ContinuationString.Is(" ");
            }
            else
            {
                xResponseContinuation.IsNull();
            }

            // 呼ばれたか確認
            VerifyQueryDocument(mockDb, pagingInfo);
        }

        /// <summary>
        /// QueryEnumerableのテストを実行する
        /// </summary>
        private void ExecuteQueryEnumerable(int expectCount, bool testDataWithNull = false)
        {
            var pagingInfo = new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = expectCount };
            ExecuteQueryEnumerable(pagingInfo, testDataWithNull);
        }
        /// <summary>
        /// QueryEnumerableのテストを実行する
        /// </summary>
        private void ExecuteQueryEnumerable(PagingInfo pagingInfo = null, bool testDataWithNull = false, int? sqlExceptionNum = null)
        {
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(pagingInfo);
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            (var targetData, var expectData) = testDataWithNull
                ? CreateQueryTestDataWithNull((int)pagingInfo.FetchCount.Value)
                : CreateQueryTestData((int)pagingInfo.FetchCount.Value);
            (var target, var mockDb) = CreateDummyRepositoryForQuery(targetData);

            // ダミーのActionを作成
            var dummyAction = CreateDummyQueryAction(pagingInfo);

            // 対象メソッド実行
            var queryParam = ValueObjectUtil.Create<QueryParam>(dummyAction);
            queryParam.GetType().GetProperty("ControllerSchema", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance).SetValue(queryParam, GetDataSchema());
            var result = target.QueryEnumerable(queryParam).ToList();

            if (targetData.Count() == 0)
            {
                // データが0件の場合結果がnullで返却されること
                result.Count().Is(0);
            }
            else
            {
                // データが1件以上の期待値が返却されること
                var expectedDataList = expectData.ToList();
                result.Count().Is(expectData.Count());
                for (var i = 0; i < result.Count(); i++)
                {
                    result[i].Value.ToString().Is(expectedDataList[i].Value.ToString());
                }
            }

            // 呼ばれたか確認
            VerifyQueryDocument(mockDb, pagingInfo);
        }


        /// <summary>
        /// Query用ダミーのRepositoryを作成する
        /// </summary>
        private (INewDynamicApiDataStoreRdbmsRepository, Mock<IJPDataHubRdbms>) CreateDummyRepositoryForQuery(IEnumerable<JToken> targetData)
        {
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y != "COUNT"),
                    It.IsAny<object>()))
                .Returns(targetData);
            mockDb.Setup(x => x.QueryDocument(
                    It.Is<string>(y => y == "COUNT"),
                    It.IsAny<object>()))
                .Returns(new List<JToken>() { JToken.Parse($"{{ Count: {DocumentCount} }}") });
            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            return (target, mockDb);
        }

        /// <summary>
        /// Query用ダミーのActionを作成する。
        /// </summary>
        /// <returns></returns>
        private IDynamicApiAction CreateDummyQueryAction(PagingInfo pagingInfo)
        {
            XRequestContinuation xRequestContinuation = null;
            if (pagingInfo.IsPaging)
            {
                xRequestContinuation = new XRequestContinuation(pagingInfo.OffsetCount.HasValue ? pagingInfo.OffsetCount.Value.ToString() : string.Empty);
            }

            return new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.XRequestContinuation, xRequestContinuation)
                .Object;
        }

        /// <summary>
        /// テスト用データを作成する（Query系用）
        /// </summary>
        /// <returns></returns>
        private (IEnumerable<JToken>, IEnumerable<JsonDocument>) CreateQueryTestData(int expectCount)
        {
            var targetData = Enumerable.Range(1, expectCount)
                .Select(x => (JToken)new JObject { ["id"] = $"id{x}", ["stringvalue"] = $"string{x}", ["intvalue"] = x, ["objectvalue"] = JToken.Parse($"{{ 'key{x}': 'value{x}' }}").ToString(), ["arrayvalue"] = JToken.Parse($"[ 'value{x}-1', 'value{x}-2']").ToString(), ["truevalue"] = true, ["falsevalue"] = false })
                .ToList();

            var expectData = Enumerable.Range(1, expectCount)
                .Select(x => new JsonDocument(new JObject { ["id"] = $"id{x}", ["stringvalue"] = $"string{x}", ["intvalue"] = x, ["objectvalue"] = JToken.Parse($"{{ 'key{x}': 'value{x}' }}"), ["arrayvalue"] = JToken.Parse($"[ 'value{x}-1', 'value{x}-2']"), ["truevalue"] = true, ["falsevalue"] = false }))
                .ToList();
            return (targetData, expectData);
        }

        /// <summary>
        /// テスト用データを作成する（Query系用）
        /// </summary>
        /// <returns></returns>
        private (IEnumerable<JToken>, IEnumerable<JsonDocument>) CreateQueryTestDataWithNull(int expectCount)
        {
            var targetData = Enumerable.Range(1, expectCount)
                .Select(x => (JToken)new JObject { ["id"] = $"id{x}", ["stringvalue"] = null, ["intvalue"] = null, ["objectvalue"] = null, ["arrayvalue"] = null, ["truevalue"] = null, ["falsevalue"] = null })
                .ToList();

            var expectData = Enumerable.Range(1, expectCount)
                .Select(x => new JsonDocument(new JObject { ["id"] = $"id{x}", ["falsevalue"] = null }))
                .ToList();
            return (targetData, expectData);
        }

        /// <summary>
        /// DBのMockのメソッドが呼ばれたか確認する
        /// </summary>
        private void VerifyQueryDocument(Mock<IJPDataHubRdbms> mockDb, PagingInfo pagingInfo)
        {
            mockDb.Verify(x => x.QueryDocument(
                It.Is<string>(y => y != "COUNT"),
                It.IsAny<object>()),
                Times.Exactly(1));
            mockDb.Verify(x => x.QueryDocument(
                It.Is<string>(y => y == "COUNT"),
                It.IsAny<object>()),
                Times.Exactly(pagingInfo.IsPaging ? 1 : 0));
        }

        #endregion

        #region Register

        [TestMethod]
        public void RegisterOnce_OK()
        {
            (var target, var mockDb) = CreateDummyRegisterRepository(true);

            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{
    'id': '{id}',
    '_Version': 1,
    'key': 'value'
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.IsOverrideId, new IsOverrideId(true));
            var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(action.Object, json));

            // idが返却されること
            result.Value.Is(id);

            // 呼ばれたか確認
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void RegisterOnce_OK_IsOverrideId()
        {
            (var target, var mockDb) = CreateDummyRegisterRepository(true);

            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{
    'id': '{id}',
    'key': 'value'
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.IsOverrideId, new IsOverrideId(true));
            var result = target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(action.Object, json));

            // idが返却されること
            result.Value.Is(id);

            // 呼ばれたか確認
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void RegisterOnce_OK_WithTypeAndPartitionKey()
        {
            (var target, var mockDb) = CreateDummyRegisterRepository(true);

            var id = Guid.NewGuid().ToString();
            var inputJson = JToken.Parse($@"
{{
    'id': '{id}',
    'key': 'value',
    '_Version': 1,
    '_Type': 'hoge',
    '_partitionkey': 'fuga'
}}");
            var expectedJson = JToken.Parse($@"
{{
    'id': '{id}',
    'key': 'value',
    '_Version': 1
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.IsOverrideId, new IsOverrideId(true));
            var param = ValueObjectUtil.Create<RegisterParam>(action.Object, inputJson);
            var result = target.RegisterOnce(param);

            // _Type/_partitionKeyが除去されていること
            param.Json.IsStructuralEqual(expectedJson);

            // idが返却されること
            result.Value.Is(id);

            // 呼ばれたか確認
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void RegisterOnce_楽観排他NG__OracleDbTransientExceptionDetector_miss()
        {
            (var target, var mockDb) = CreateDummyRegisterRepository(false);

            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{
    'id': '{id}',
    'key': 'value'
}}");

            // 対象メソッド実行
            var exception = AssertEx.Throws<ConflictException>(() => target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(new Mock<IDynamicApiAction>().Object, json)));

            // OracleDbTransientExceptionDetectorに含まれないのでretryはされない
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void RegisterOnce_NG_SqlException_OracleDbTransientExceptionDetector_hit()
        {
            var mockQuerySqlBuilder = new Mock<IUpsertSqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IUpsertSqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()))
                .Throws(CreateSqlException((int)-2)); // target
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);

            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{
    'id': '{id}',
    '_Version': 1,
    'key': 'value'
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>()
                .SetupProperty(x => x.IsOverrideId, new IsOverrideId(true));

            var exception = AssertEx.Throws<SqlException>(() => target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(action.Object, json)));
            // OracleDbTransientExceptionDetectorに含まれるのでretry
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
        }

        /// <summary>
        /// Upsert用ダミーのRepositoryを作成する
        /// </summary>
        private (INewDynamicApiDataStoreRepository, Mock<IJPDataHubRdbms>) CreateDummyRegisterRepository(bool success)
        {
            var mockQuerySqlBuilder = new Mock<IUpsertSqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IUpsertSqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>())).Returns(success ? 1 : 0);
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);

            return (target, mockDb);
        }

        #endregion

        #region Delete

        [TestMethod]
        public void DeleteOnce_OK()
        {
            (var target, var mockDb) = CreateDummyDeleteRepository();

            // 対象メソッド実行
            target.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object));

            // 呼ばれたか確認
            mockDb.Verify(x => x.DeleteDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DeleteOnce_OK_コールバックあり()
        {
            (var target, var mockDb) = CreateDummyDeleteRepository();

            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{
    'id': '{id}',
    'key': 'value',
    '_etag': '1'
}}");

            // 対象メソッド実行
            var callbackCalled = false;
            var callbackDelete = new Action<JToken, RepositoryType>((x, y) =>
            {
                callbackCalled = true;
                y.Is(RepositoryType.OracleDb);
                x.ToString().Is(JToken.Parse($@"
{{
    'id': '{id}',
    'key': 'value'
}}").ToString());
            });
            target.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object, callbackDelete, json));

            // 呼ばれたか確認
            mockDb.Verify(x => x.DeleteDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
            callbackCalled.IsTrue();
        }

        [TestMethod]
        public void DeleteOnce_NG_SqlException_OracleDbTransientExceptionDetector_hit()
        {
            var mockDeleteSqlBuilder = new Mock<IDeleteSqlBuilder>();
            mockDeleteSqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IDeleteSqlBuilder>(RepositoryName, mockDeleteSqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.DeleteDocument(It.IsAny<string>(), It.IsAny<object>()))
                .Throws(CreateSqlException((int)-2)); // target
            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            // 対象メソッド実行
            var exception = AssertEx.Throws<SqlException>(() => target.DeleteOnce(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object)));
            // OracleDbTransientExceptionDetectorに含まれるのでretry
            mockDb.Verify(x => x.DeleteDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
        }

        [TestMethod]
        public void Delete_OK()
        {
            // 対象メソッド実行（Deleteは未実装）
            AssertEx.Catch<NotImplementedException>(() =>
            {
                (var target, var mockDb) = CreateDummyDeleteRepository();
                target.Delete(ValueObjectUtil.Create<DeleteParam>(new Mock<IDynamicApiAction>().Object));
            });
        }


        /// <summary>
        /// Delete用ダミーのRepositoryを作成する
        /// </summary>
        private (INewDynamicApiDataStoreRdbmsRepository, Mock<IJPDataHubRdbms>) CreateDummyDeleteRepository()
        {
            var mockDeleteSqlBuilder = new Mock<IDeleteSqlBuilder>();
            mockDeleteSqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IDeleteSqlBuilder>(RepositoryName, mockDeleteSqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.DeleteDocument(It.IsAny<string>(), It.IsAny<object>())).Returns(1);
            UnityContainer.RegisterInstance("Oracle", mockDb.Object);

            return (target, mockDb);
        }

        #endregion

        #region ODataPatch

        [TestMethod]
        public void ODataPatch_OK()
        {
            var expectedAffectedCount = 10;
            (var target, var mockDb) = CreateDummyODataPatchRepository(expectedAffectedCount);

            var id = Guid.NewGuid().ToString();
            var patchData = JToken.Parse($@"
{{
    'key1': 'newvalue1',
    'key2': 'newvalue2'
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>();
            var result = target.ODataPatch(ValueObjectUtil.Create<QueryParam>(action.Object), patchData);
            result.Is(expectedAffectedCount);

            // 呼ばれたか確認
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ODataPatch_NG_SqlException_207_OracleDbTransientExceptionDetector_miss()
        {
            var expectedAffectedCount = 10;
            var mockSqlBuilder = new Mock<IODataPatchSqlBuilder>();
            mockSqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IODataPatchSqlBuilder>(RepositoryName, mockSqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()))
                .Throws(CreateSqlException((int)207)); // target
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);


            var id = Guid.NewGuid().ToString();
            var patchData = JToken.Parse($@"
{{
    'key1': 'newvalue1',
    'key2': 'newvalue2'
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>();
            var result = target.ODataPatch(ValueObjectUtil.Create<QueryParam>(action.Object), patchData);
            result.Is(0);

            // OracleDbTransientExceptionDetectorに含まれないのでretryはされない
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void ODataPatch_NG_SqlException_OracleDbTransientExceptionDetector_hit()
        {
            var expectedAffectedCount = 10;
            var mockSqlBuilder = new Mock<IODataPatchSqlBuilder>();
            mockSqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IODataPatchSqlBuilder>(RepositoryName, mockSqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()))
                .Throws(CreateSqlException((int)-2)); // target
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);


            var id = Guid.NewGuid().ToString();
            var patchData = JToken.Parse($@"
{{
    'key1': 'newvalue1',
    'key2': 'newvalue2'
}}");

            // 対象メソッド実行
            var action = new Mock<IDynamicApiAction>();
            var exception = AssertEx.Throws<SqlException>(() => target.ODataPatch(ValueObjectUtil.Create<QueryParam>(action.Object), patchData));
            // OracleDbTransientExceptionDetectorに含まれるのでretry
            mockDb.Verify(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(3));
        }

        /// <summary>
        /// ODataPatch用ダミーのRepositoryを作成する
        /// </summary>
        private (INewDynamicApiDataStoreRepository, Mock<IJPDataHubRdbms>) CreateDummyODataPatchRepository(int affectedCount)
        {
            var mockSqlBuilder = new Mock<IODataPatchSqlBuilder>();
            mockSqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IODataPatchSqlBuilder>(RepositoryName, mockSqlBuilder.Object);

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.UpsertDocument(It.IsAny<string>(), It.IsAny<object>())).Returns(affectedCount);
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);

            return (target, mockDb);
        }

        #endregion

        #region DDL

        [TestMethod]
        public void GetTableColumns()
        {
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null });
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var colummName1 = Guid.NewGuid().ToString();
            var colummType1 = Guid.NewGuid().ToString();
            var colummName2 = Guid.NewGuid().ToString();
            var colummType2 = Guid.NewGuid().ToString();

            var targetData = new List<JToken>()
            {
                JToken.Parse($"{{ 'COLUMN_NAME': '{colummName1}', 'DATA_TYPE': '{colummType1}' }}"),
                JToken.Parse($"{{ 'COLUMN_NAME': '{colummName2}', 'DATA_TYPE': '{colummType2}' }}")
            };

            (var target, var mockDb) = CreateDummyRepositoryForQuery(targetData);

            // 対象メソッド実行
            var result = target.GetTableColumns(Guid.NewGuid().ToString()).ToList();
            result.Count().Is(2);
            result[0].Name.Is(colummName1);
            result[0].DataType.Is(colummType1);
            result[1].Name.Is(colummName2);
            result[1].DataType.Is(colummType2);

            mockDb.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void CreateTable_PKあり_名前なし()
        {
            var tableName = Guid.NewGuid().ToString();
            var columns = new List<RdbmsTableColumn>()
            {
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true),  // NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false), // NOT NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString()),       // デフォルトあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, null, Guid.NewGuid().ToString()), // COLLATEあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) // デフォルト、COLLATEあり
            };
            var primaryKey = new List<string>() { columns.First().Name };

            var expectedSql = $@"CREATE TABLE ""{tableName}"" (
""{columns[0].Name}"" {columns[0].DataType} NULL,""{columns[1].Name}"" {columns[1].DataType} NOT NULL,""{columns[2].Name}"" {columns[2].DataType} DEFAULT '{columns[2].DefaultValue}' NOT NULL,""{columns[3].Name}"" {columns[3].DataType} COLLATE {columns[3].Collate} NOT NULL,""{columns[4].Name}"" {columns[4].DataType} COLLATE {columns[4].Collate} DEFAULT '{columns[4].DefaultValue}' NULL
,CONSTRAINT ""PK_{tableName}"" PRIMARY KEY (""{primaryKey.First()}"")
)";

            (var target, var mockDb) = CreateDummyRepositoryForDdl(expectedSql);

            // 対象メソッド実行
            target.CreateTable(tableName, columns, primaryKey, null);

            mockDb.Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void CreateTable_PK複数_名前あり()
        {
            var tableName = Guid.NewGuid().ToString();
            var columns = new List<RdbmsTableColumn>()
            {
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true),  // NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false), // NOT NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString()),       // デフォルトあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, null, Guid.NewGuid().ToString()), // COLLATEあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) // デフォルト、COLLATEあり
            };
            var primaryKey = new List<string>() { columns.First().Name, columns.Last().Name };
            var primaryKeyName = Guid.NewGuid().ToString();

            var expectedSql = $@"CREATE TABLE ""{tableName}"" (
""{columns[0].Name}"" {columns[0].DataType} NULL,""{columns[1].Name}"" {columns[1].DataType} NOT NULL,""{columns[2].Name}"" {columns[2].DataType} DEFAULT '{columns[2].DefaultValue}' NOT NULL,""{columns[3].Name}"" {columns[3].DataType} COLLATE {columns[3].Collate} NOT NULL,""{columns[4].Name}"" {columns[4].DataType} COLLATE {columns[4].Collate} DEFAULT '{columns[4].DefaultValue}' NULL
,CONSTRAINT ""{primaryKeyName}"" PRIMARY KEY (""{primaryKey.First()}"",""{primaryKey.Last()}"")
)";

            (var target, var mockDb) = CreateDummyRepositoryForDdl(expectedSql);

            // 対象メソッド実行
            target.CreateTable(tableName, columns, primaryKey, primaryKeyName);

            mockDb.Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void CreateTable_PKなし()
        {
            var tableName = Guid.NewGuid().ToString();
            var columns = new List<RdbmsTableColumn>()
            {
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true),  // NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false), // NOT NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString()),       // デフォルトあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, null, Guid.NewGuid().ToString()), // COLLATEあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) // デフォルト、COLLATEあり
            };

            var expectedSql = $@"CREATE TABLE ""{tableName}"" (
""{columns[0].Name}"" {columns[0].DataType} NULL,""{columns[1].Name}"" {columns[1].DataType} NOT NULL,""{columns[2].Name}"" {columns[2].DataType} DEFAULT '{columns[2].DefaultValue}' NOT NULL,""{columns[3].Name}"" {columns[3].DataType} COLLATE {columns[3].Collate} NOT NULL,""{columns[4].Name}"" {columns[4].DataType} COLLATE {columns[4].Collate} DEFAULT '{columns[4].DefaultValue}' NULL
)";

            HasDoubleQuotesFromCreateTable(expectedSql);

            (var target, var mockDb) = CreateDummyRepositoryForDdl(expectedSql);

            // 対象メソッド実行
            target.CreateTable(tableName, columns);

            mockDb.Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void CreateIndex_名前なし()
        {
            var tableName = Guid.NewGuid().ToString();
            var columnName = Guid.NewGuid().ToString();

            var expectedSql = $"CREATE INDEX \"IX_{tableName}_{columnName}\" ON \"{tableName}\" (\"{columnName}\")";

            (var target, var mockDb) = CreateDummyRepositoryForDdl(expectedSql);

            // 対象メソッド実行
            target.CreateIndex(tableName, columnName);

            mockDb.Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void CreateIndex_名前なし_名前あり()
        {
            var tableName = Guid.NewGuid().ToString();
            var columnName = Guid.NewGuid().ToString();
            var indexName = Guid.NewGuid().ToString();

            var expectedSql = $"CREATE INDEX \"{indexName}\" ON \"{tableName}\" (\"{columnName}\")";

            (var target, var mockDb) = CreateDummyRepositoryForDdl(expectedSql);

            // 対象メソッド実行
            target.CreateIndex(tableName, columnName, indexName);

            mockDb.Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(1));
        }

        [TestMethod]
        public void GetTableColumns_NG_SqlException_207()
        {
            var mockQuerySqlBuilder = new Mock<IQuerySqlBuilder>();
            mockQuerySqlBuilder.SetupGet(x => x.PagingInfo).Returns(new PagingInfo() { IsPaging = false, OffsetCount = null, FetchCount = null });
            mockQuerySqlBuilder.SetupGet(x => x.CountSql).Returns("COUNT");
            mockQuerySqlBuilder.SetupGet(x => x.SqlParameterList).Returns(new OracleDbSqlParameterList());
            UnityContainer.RegisterInstance<IQuerySqlBuilder>(RepositoryName, mockQuerySqlBuilder.Object);

            var colummName1 = Guid.NewGuid().ToString();
            var colummType1 = Guid.NewGuid().ToString();
            var colummName2 = Guid.NewGuid().ToString();
            var colummType2 = Guid.NewGuid().ToString();

            var targetData = new List<JToken>()
            {
                JToken.Parse($"{{ 'COLUMN_NAME': '{colummName1}', 'DATA_TYPE': '{colummType1}' }}"),
                JToken.Parse($"{{ 'COLUMN_NAME': '{colummName2}', 'DATA_TYPE': '{colummType2}' }}")
            };

            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            mockResourceVersionRepository.Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>())).Returns(new ResourceVersion(1));
            mockResourceVersionRepository.Setup(x => x.GetCurrentVersion(It.IsAny<RepositoryKey>())).Returns(new ResourceVersion(1));
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<object>()))
                .Throws(CreateSqlException((int)207));
            UnityContainer.RegisterInstance("Oracle", mockDb.Object);


            // 対象メソッド実行
            var result = target.GetTableColumns(Guid.NewGuid().ToString()).ToList();
            result.Count().Is(0);

            // OracleDbTransientExceptionDetectorに含まれないのでretryはされない
            mockDb.Verify(x => x.QueryDocument(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        [TestMethod]
        public void AddTableColumns()
        {
            var tableName = Guid.NewGuid().ToString();
            var columns = new List<RdbmsTableColumn>()
            {
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true),  // NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false), // NOT NULL
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, Guid.NewGuid().ToString()),       // デフォルトあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), false, null, Guid.NewGuid().ToString()), // COLLATEあり
                new RdbmsTableColumn(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), true, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) // デフォルト、COLLATEあり
            };

            var expectedSql = $@"ALTER TABLE ""{tableName}"" ADD ""{columns[0].Name}"" {columns[0].DataType} NULL,""{columns[1].Name}"" {columns[1].DataType} NOT NULL,""{columns[2].Name}"" {columns[2].DataType} DEFAULT '{columns[2].DefaultValue}' NOT NULL,""{columns[3].Name}"" {columns[3].DataType} COLLATE {columns[3].Collate} NOT NULL,""{columns[4].Name}"" {columns[4].DataType} COLLATE {columns[4].Collate} DEFAULT '{columns[4].DefaultValue}' NULL";

            HasDoubleQuotesFromAlterTable(expectedSql);

            (var target, var mockDb) = CreateDummyRepositoryForDdl(expectedSql);

            // 対象メソッド実行
            target.AddTableColumns(tableName, columns);

            mockDb.Verify(x => x.Execute(It.IsAny<string>()), Times.Exactly(1));
        }

        /// <summary>
        /// DDL用ダミーのRepositoryを作成する
        /// </summary>
        private (INewDynamicApiDataStoreRdbmsRepository, Mock<IJPDataHubRdbms>) CreateDummyRepositoryForDdl(string expectedSql)
        {
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRdbmsRepository>();
            target.RepositoryInfo = new RepositoryInfo(RepositoryType.OracleDb.ToCode(), new Dictionary<string, bool>() { { ConnectionString, false } });
            var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
            target.ResourceVersionRepository = mockResourceVersionRepository.Object;

            var mockDb = new Mock<IJPDataHubRdbms>();
            mockDb.Setup(x => x.Execute(It.IsAny<string>())).Callback<string>(x => x.Trim().Is(expectedSql));
            UnityContainer.RegisterInstance<IJPDataHubRdbms>("Oracle", mockDb.Object);

            return (target, mockDb);
        }

        #endregion


        private DataSchema GetDataSchema()
        {
            return new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""stringvalue"": {{ ""type"": ""string"" }},
        ""intvalue"": {{ ""type"": ""number"" }},
        ""objectvalue"": {{ ""type"": ""object"", ""properties"": {{ ""name"": {{ ""type"": ""string"" }} }} }},
        ""arrayvalue"": {{ ""type"": ""array"", ""items"": {{ ""type"": ""number"" }} }},
        ""truevalue"": {{ ""type"": ""boolean"" }},
        ""falsevalue"": {{ ""type"": ""boolean"" }}
    }},
    ""required"": [ ""falsevalue"" ]
}}");
        }

        private SqlException CreateSqlException(int number)
        {
            var collectionConstructor = typeof(SqlErrorCollection).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
            var errorCollection = (SqlErrorCollection)collectionConstructor.Invoke(null);
            var errorConstructor = typeof(SqlError).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int), typeof(uint), typeof(Exception) }, null);
            var error = errorConstructor.Invoke(new object[] { number, (byte)0, (byte)0, "server", "errMsg", "proccedure", 100, (uint)0, null });

            addMethod.Invoke(errorCollection, new[] { error });

            var constructor = typeof(SqlException).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) }, null);
            return (SqlException)constructor.Invoke(new object[] { null, errorCollection, new DataException(), Guid.NewGuid() });
        }

        /// <summary>ダブルクォートで囲まれたテーブル名のマッチ文字列</summary>
        private string HasDoubleQuotesTableName = "^\".*?\"";

        /// <summary>CREATE TABLE文のテーブル名がダブルクォートで囲まれているか確認します</summary>
        /// <param name="sql">sql</param>
        public void HasDoubleQuotesFromCreateTable(string sql)
        {
            sql = sql.Replace("CREATE TABLE ", "");
            string target = Regex.Match(sql, HasDoubleQuotesTableName).Value;

            // ダブルクォートで囲まれたテーブル名が抜き出せた事の確認
            Assert.IsTrue(target.Any());
        }

        /// <summary>ALTER TABLE文のテーブル名がダブルクォートで囲まれているか確認します</summary>
        /// <param name="sql">sql</param>
        public void HasDoubleQuotesFromAlterTable(string sql)
        {
            sql = sql.Replace("ALTER TABLE ", "");
            string target = Regex.Match(sql, HasDoubleQuotesTableName).Value;

            // ダブルクォートで囲まれたテーブル名が抜き出せた事の確認
            Assert.IsTrue(target.Any());
        }
    }
}
