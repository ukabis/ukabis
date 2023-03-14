using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_SqlServerDataStoreRepositoryQueryOnly : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterInstance(Configuration);
        }


        [TestMethod]
        public void Query()
        {
            // 入力値
            string sql = @"SELECT c.[action_type_cd], c.[action_type_name] FROM @tableName c
WHERE c.[action_type_cd] = 'quy' or c.[action_type_cd] = 'reg' ORDER BY c.[action_type_cd]";
            Guid physicalRepositoryId = Guid.NewGuid();
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            string vendorId = Guid.NewGuid().ToString();
            string systemId = Guid.NewGuid().ToString();
            string tableName = "Table1";

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<dynamic>(sql.Replace("@tableName", $"[{tableName}]")))
                .Returns(new[] { new { action_type_cd = "quy" }, new { action_type_cd = "reg" } });
            var repMock = CreateRepositoryMock(physicalRepositoryId, controllerId, vendorId, systemId, tableName);
            var perRequestDataContainer = new PerRequestDataContainer { VendorId = vendorId, SystemId = systemId };

            // テスト対象のインスタンスを作成
            var testClass = CreateTestTarget(mock, perRequestDataContainer, repMock, physicalRepositoryId);

            // パラメータ作成
            var param = new QueryParam(null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null,
                new NativeQuery(sql), null, null, null, null, null, null, null, null, controllerId: controllerId);

            // テストメソッド実行
            var result = testClass.Query(param, out XResponseContinuation responseContinuation);

            // 結果の検証
            result.Count().Is(2);
            result.First().Value["action_type_cd"].Is("quy");
            result.Last().Value["action_type_cd"].Is("reg");
        }

        [TestMethod]
        public void Query_NotFound()
        {
            // 入力値
            string sql = @"SELECT c.[action_type_cd], c.[action_type_name] FROM @tableName c
WHERE c.[action_type_cd] = 'quy'";
            Guid physicalRepositoryId = Guid.NewGuid();
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            string vendorId = Guid.NewGuid().ToString();
            string systemId = Guid.NewGuid().ToString();
            string tableName = "Table1";

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<dynamic>(sql.Replace("@tableName", $"[{tableName}]"))).Returns(new object[0]);
            var repMock = CreateRepositoryMock(physicalRepositoryId, controllerId, vendorId, systemId, tableName);
            var perRequestDataContainer = new PerRequestDataContainer { VendorId = vendorId, SystemId = systemId };

            // テスト対象のインスタンスを作成
            var testClass = CreateTestTarget(mock, perRequestDataContainer, repMock, physicalRepositoryId);

            // パラメータ作成
            var param = new QueryParam(null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null,
                new NativeQuery(sql), null, null, null, null, null, null, null, null, controllerId: controllerId);

            // テストメソッド実行
            var result = testClass.Query(param, out XResponseContinuation responseContinuation);

            // 結果の検証
            result.IsNull();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Query_SqlException()
        {
            // 入力値
            string sql = @"SELECT c.[action_type_cd], c.[action_type_name] FROM @tableName c
WHERE c.[action_type_cd] = 'quy'";
            Guid physicalRepositoryId = Guid.NewGuid();
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            string vendorId = Guid.NewGuid().ToString();
            string systemId = Guid.NewGuid().ToString();
            string tableName = "Table1";

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            var repMock = CreateRepositoryMock(physicalRepositoryId, controllerId, vendorId, systemId, tableName);
            mock.Setup(x => x.Query<dynamic>(sql.Replace("@tableName", $"[{tableName}]"))).Throws(new Exception());
            var perRequestDataContainer = new PerRequestDataContainer { VendorId = vendorId, SystemId = systemId };

            // テスト対象のインスタンスを作成
            var testClass = CreateTestTarget(mock, perRequestDataContainer, repMock, physicalRepositoryId);

            // パラメータ作成
            var param = new QueryParam(null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null,
                new NativeQuery(sql), null, null, null, null, null, null, null, null, controllerId: controllerId);

            // テストメソッド実行
            var result = testClass.Query(param, out XResponseContinuation responseContinuation);
        }

        [TestMethod]
        public void QueryEnumerable()
        {
            // 入力値
            string sql = @"SELECT action_type_cd, action_type_name FROM ActionType
WHERE action_type_cd = @p1 or action_type_cd = @p2 ORDER BY action_type_cd";

            var queryDic = new Dictionary<QueryStringKey, QueryStringValue>
            {
                { new QueryStringKey("p1"), new QueryStringValue("quy") },
                { new QueryStringKey("p2"), new QueryStringValue("reg") }
            };

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<dynamic>(sql, It.IsAny<object>())).Returns(new[] { new { action_type_cd = "quy" }, new { action_type_cd = "reg" } });

            // テスト対象のインスタンスを作成
            var testClass = new SqlServerDataStoreRepositoryQueryOnly(new Func<string, IJPDataHubDbConnection>((cs) => mock.Object), null, null);
            testClass.RepositoryInfo = new RepositoryInfo("ssd", new Dictionary<string, bool> { { "connStr", false } }); ;

            // パラメータ作成
            var param = new QueryParam(null, null, null, null, null, null, null, null,
                new ApiQuery(sql), null, null, null, null, null, null, null, null,
                new QueryStringVO(queryDic), null, null, null, null, null, null, null, null, null, null, null);

            // テストメソッド実行
            var result = testClass.QueryEnumerable(param);

            // 結果の検証
            result.Count().Is(2);
            result.First().Value["action_type_cd"].Is("quy");
            result.Last().Value["action_type_cd"].Is("reg");
        }

        [TestMethod]
        public void QueryEnumerable_NotFound()
        {
            // 入力値
            string sql = @"SELECT action_type_cd, action_type_name FROM ActionType
WHERE action_type_cd = @p1";

            var queryDic = new Dictionary<QueryStringKey, QueryStringValue>
            {
                { new QueryStringKey("p1"), new QueryStringValue("1234") }
            };

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<dynamic>(sql, It.IsAny<object>())).Returns(new object[0]);

            // テスト対象のインスタンスを作成
            var testClass = new SqlServerDataStoreRepositoryQueryOnly(new Func<string, IJPDataHubDbConnection>((cs) => mock.Object), null, null);
            testClass.RepositoryInfo = new RepositoryInfo("ssd", new Dictionary<string, bool> { { "connStr", false } }); ;

            // パラメータ作成
            var param = new QueryParam(null, null, null, null, null, null, null, null,
                new ApiQuery(sql), null, null, null, null, null, null, null, null,
                new QueryStringVO(queryDic), null, null, null, null, null, null, null, null, null, null, null);

            // テストメソッド実行
            var result = testClass.QueryEnumerable(param);

            // 結果の検証
            result.IsNull();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void QueryEnumerable_SqlException()
        {
            // 入力値
            string sql = @"SELECT action_type_cd, action_type_name FROM ActionType
WHERE action_type_cd = @p1";

            var queryDic = new Dictionary<QueryStringKey, QueryStringValue>();

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<dynamic>(sql, It.IsAny<object>())).Throws(new Exception());

            // テスト対象のインスタンスを作成
            var testClass = new SqlServerDataStoreRepositoryQueryOnly(new Func<string, IJPDataHubDbConnection>((cs) => mock.Object), null, null);
            testClass.RepositoryInfo = new RepositoryInfo("ssd", new Dictionary<string, bool> { { "connStr", false } }); ;

            // パラメータ作成
            var param = new QueryParam(null, null, null, null, null, null, null, null,
                new ApiQuery(sql), null, null, null, null, null, null, null, null,
                new QueryStringVO(queryDic), null, null, null, null, null, null, null, null, null, null, null);

            // テストメソッド実行
            testClass.QueryEnumerable(param);
        }

        private Mock<IContainerDynamicSeparationRepository> CreateRepositoryMock(Guid physicalRepositoryId, ControllerId controllerId, string vendorId, string systemId, string tableName)
        {
            var repMock = new Mock<IContainerDynamicSeparationRepository>();
            repMock.Setup(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(p1 => p1.Value == physicalRepositoryId.ToString()),
                controllerId, It.Is<VendorId>(p2 => p2.Value == vendorId), It.Is<SystemId>(p3 => p3.Value == systemId), It.IsAny<OpenId>())).Returns(tableName);
            return repMock;
        }

        private SqlServerDataStoreRepositoryQueryOnly CreateTestTarget(Mock<IJPDataHubDbConnection> mock,
            PerRequestDataContainer perRequestDataContainer, Mock<IContainerDynamicSeparationRepository> repMock, Guid physicalRepositoryId)
        {
            var testClass = new SqlServerDataStoreRepositoryQueryOnly(new Func<string, IJPDataHubDbConnection>((cs) => mock.Object),
                perRequestDataContainer, repMock.Object);
            testClass.RepositoryInfo = new RepositoryInfo(Guid.NewGuid(), "ssd",
                new List<Tuple<string, bool, Guid?>> { new Tuple<string, bool, Guid?>("connStr", false, physicalRepositoryId) });
            return testClass;
        }
    }
}
