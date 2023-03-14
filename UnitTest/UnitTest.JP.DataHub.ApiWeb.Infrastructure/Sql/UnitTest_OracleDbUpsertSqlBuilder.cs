using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using Unity.Injection;
using Unity.Interception.ContainerIntegration;
using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
using Unity.Interception.PolicyInjection;
using Unity.Resolution;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    [TestClass]
    public class UnitTest_OracleDbUpsertSqlBuilder : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private string _tableName = Guid.NewGuid().ToString();


        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterType<IUpsertSqlBuilder, OracleDbUpsertSqlBuilder>(RepositoryType.OracleDb.ToCode(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<RegisterParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null)));
        }


        [TestMethod]
        [TestCase(false)]
        [TestCase(true)]
        public void BuildUp_楽観排他ON_ETAGなし()
        {
            TestContext.Run((bool isEtagModel) =>
            {
                var id = Guid.NewGuid().ToString();
                var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'STR_VALUE': 'abc',
    'STR_NULL': null,
    'INT_VALUE': 123,
    'DBL_VALUE': 123.456,
    'NUM_NULL': null,
    'OBJ_VALUE': {{ 'key': 'value' }},
    'OBJ_NULL': null,
    'ARY_VALUE': [ 'value1', 'value2' ],
    'ARY_NULL': null,
    'BOL_VALUE': true,
    'BOL_NULL': null,
    'DAT_VALUE': '2018-11-12T10:20:39',
    'DAT_NULL': null
}}");

                var repositoryName = RepositoryType.OracleDb.ToCode();
                var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema(isEtagModel), new IsOptimisticConcurrency(true));
                var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
                var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
                mockContainerDynamicSeparation
                    .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                    .Returns(_tableName);

                var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                    repositoryName,
                    new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

                builder.BuildUp();
                builder.Sql.Is($@"MERGE INTO ""{_tableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"", :p_9 AS ""p_9"", :p_10 AS ""p_10"", :p_11 AS ""p_11"", :p_12 AS ""p_12"", :p_13 AS ""p_13"", :p_14 AS ""p_14"", :p_15 AS ""p_15"" FROM dual
) X ON (""{_tableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""STR_VALUE"" = X.""p_3"", ""STR_NULL"" = X.""p_4"", ""INT_VALUE"" = X.""p_5"", ""DBL_VALUE"" = X.""p_6"", ""NUM_NULL"" = X.""p_7"", ""OBJ_VALUE"" = X.""p_8"", ""OBJ_NULL"" = X.""p_9"", ""ARY_VALUE"" = X.""p_10"", ""ARY_NULL"" = X.""p_11"", ""BOL_VALUE"" = X.""p_12"", ""BOL_NULL"" = X.""p_13"", ""DAT_VALUE"" = X.""p_14"", ""DAT_NULL"" = X.""p_15"", ""_etag"" = ""_etag"" + 1
WHERE ""{_tableName}"".""_etag"" = :p_16 
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""STR_VALUE"", ""STR_NULL"", ""INT_VALUE"", ""DBL_VALUE"", ""NUM_NULL"", ""OBJ_VALUE"", ""OBJ_NULL"", ""ARY_VALUE"", ""ARY_NULL"", ""BOL_VALUE"", ""BOL_NULL"", ""DAT_VALUE"", ""DAT_NULL"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"", X.""p_9"", X.""p_10"", X.""p_11"", X.""p_12"", X.""p_13"", X.""p_14"", X.""p_15"")
");
                builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
                {
                    { ":p_1", 1L },
                    { ":p_2", id },
                    { ":p_3", "abc" },
                    { ":p_4", null },
                    { ":p_5", 123L },
                    { ":p_6", 123.456 },
                    { ":p_7", null },
                    { ":p_8", JToken.Parse("{ 'key': 'value' }").ToString() },
                    { ":p_9", null },
                    { ":p_10", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                    { ":p_11", null },
                    { ":p_12", true },
                    { ":p_13", null },
                    { ":p_14", DateTime.Parse("2018-11-12T10:20:39") },
                    { ":p_15", null },
                    { ":p_16", 0L }
                });
            });
        }

        [TestMethod]
        [TestCase("5", false)]
        [TestCase("5", true)]
        [TestCase("'5'", false)]
        [TestCase("'5'", true)]
        public void BuildUp_楽観排他ON_ETAGあり()
        {
            TestContext.Run((string etagValue, bool isEtagModel) =>
            {
                var id = Guid.NewGuid().ToString();
                var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'STR_VALUE': 'abc',
    'STR_NULL': null,
    'INT_VALUE': 123,
    'DBL_VALUE': 123.456,
    'NUM_NULL': null,
    'OBJ_VALUE': {{ 'key': 'value' }},
    'OBJ_NULL': null,
    'ARY_VALUE': [ 'value1', 'value2' ],
    'ARY_NULL': null,
    'BOL_VALUE': true,
    'BOL_NULL': null,
    'DAT_VALUE': '2018-11-12T10:20:39',
    'DAT_NULL': null,
    '_etag': {etagValue}
}}");

                var repositoryName = RepositoryType.OracleDb.ToCode();
                var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema(isEtagModel), new IsOptimisticConcurrency(true));
                var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
                var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
                mockContainerDynamicSeparation
                    .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                    .Returns(_tableName);

                var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                    repositoryName,
                    new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

                builder.BuildUp();
                builder.Sql.Is($@"MERGE INTO ""{_tableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"", :p_9 AS ""p_9"", :p_10 AS ""p_10"", :p_11 AS ""p_11"", :p_12 AS ""p_12"", :p_13 AS ""p_13"", :p_14 AS ""p_14"", :p_15 AS ""p_15"" FROM dual
) X ON (""{_tableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""STR_VALUE"" = X.""p_3"", ""STR_NULL"" = X.""p_4"", ""INT_VALUE"" = X.""p_5"", ""DBL_VALUE"" = X.""p_6"", ""NUM_NULL"" = X.""p_7"", ""OBJ_VALUE"" = X.""p_8"", ""OBJ_NULL"" = X.""p_9"", ""ARY_VALUE"" = X.""p_10"", ""ARY_NULL"" = X.""p_11"", ""BOL_VALUE"" = X.""p_12"", ""BOL_NULL"" = X.""p_13"", ""DAT_VALUE"" = X.""p_14"", ""DAT_NULL"" = X.""p_15"", ""_etag"" = ""_etag"" + 1
WHERE ""{_tableName}"".""_etag"" = :p_16 
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""STR_VALUE"", ""STR_NULL"", ""INT_VALUE"", ""DBL_VALUE"", ""NUM_NULL"", ""OBJ_VALUE"", ""OBJ_NULL"", ""ARY_VALUE"", ""ARY_NULL"", ""BOL_VALUE"", ""BOL_NULL"", ""DAT_VALUE"", ""DAT_NULL"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"", X.""p_9"", X.""p_10"", X.""p_11"", X.""p_12"", X.""p_13"", X.""p_14"", X.""p_15"")
");
                builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
                {
                    { ":p_1", 1L },
                    { ":p_2", id },
                    { ":p_3", "abc" },
                    { ":p_4", null },
                    { ":p_5", 123L },
                    { ":p_6", 123.456 },
                    { ":p_7", null },
                    { ":p_8", JToken.Parse("{ 'key': 'value' }").ToString() },
                    { ":p_9", null },
                    { ":p_10", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                    { ":p_11", null },
                    { ":p_12", true },
                    { ":p_13", null },
                    { ":p_14", DateTime.Parse("2018-11-12T10:20:39") },
                    { ":p_15", null },
                    { ":p_16", 5L }
                });
            });
        }


        [TestMethod]
        public void BuildUp_楽観排他OFF_ETAGなし()
        {
            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'STR_VALUE': 'abc',
    'STR_NULL': null,
    'INT_VALUE': 123,
    'DBL_VALUE': 123.456,
    'NUM_NULL': null,
    'OBJ_VALUE': {{ 'key': 'value' }},
    'OBJ_NULL': null,
    'ARY_VALUE': [ 'value1', 'value2' ],
    'ARY_NULL': null,
    'BOL_VALUE': true,
    'BOL_NULL': null,
    'DAT_VALUE': '2018-11-12T10:20:39',
    'DAT_NULL': null
}}");

            var repositoryName = RepositoryType.OracleDb.ToCode();
            var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema());
            var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                repositoryName,
                new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

            builder.BuildUp();
            builder.Sql.Is($@"MERGE INTO ""{_tableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"", :p_9 AS ""p_9"", :p_10 AS ""p_10"", :p_11 AS ""p_11"", :p_12 AS ""p_12"", :p_13 AS ""p_13"", :p_14 AS ""p_14"", :p_15 AS ""p_15"" FROM dual
) X ON (""{_tableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""STR_VALUE"" = X.""p_3"", ""STR_NULL"" = X.""p_4"", ""INT_VALUE"" = X.""p_5"", ""DBL_VALUE"" = X.""p_6"", ""NUM_NULL"" = X.""p_7"", ""OBJ_VALUE"" = X.""p_8"", ""OBJ_NULL"" = X.""p_9"", ""ARY_VALUE"" = X.""p_10"", ""ARY_NULL"" = X.""p_11"", ""BOL_VALUE"" = X.""p_12"", ""BOL_NULL"" = X.""p_13"", ""DAT_VALUE"" = X.""p_14"", ""DAT_NULL"" = X.""p_15"", ""_etag"" = ""_etag"" + 1
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""STR_VALUE"", ""STR_NULL"", ""INT_VALUE"", ""DBL_VALUE"", ""NUM_NULL"", ""OBJ_VALUE"", ""OBJ_NULL"", ""ARY_VALUE"", ""ARY_NULL"", ""BOL_VALUE"", ""BOL_NULL"", ""DAT_VALUE"", ""DAT_NULL"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"", X.""p_9"", X.""p_10"", X.""p_11"", X.""p_12"", X.""p_13"", X.""p_14"", X.""p_15"")
");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { ":p_1", 1L },
                { ":p_2", id },
                { ":p_3", "abc" },
                { ":p_4", null },
                { ":p_5", 123L },
                { ":p_6", 123.456 },
                { ":p_7", null },
                { ":p_8", JToken.Parse("{ 'key': 'value' }").ToString() },
                { ":p_9", null },
                { ":p_10", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                { ":p_11", null },
                { ":p_12", true },
                { ":p_13", null },
                { ":p_14", DateTime.Parse("2018-11-12T10:20:39") },
                { ":p_15", null }
            });
        }

        [TestMethod]
        public void BuildUp_楽観排他OFF_ETAGあり()
        {
            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'STR_VALUE': 'abc',
    'STR_NULL': null,
    'INT_VALUE': 123,
    'DBL_VALUE': 123.456,
    'NUM_NULL': null,
    'OBJ_VALUE': {{ 'key': 'value' }},
    'OBJ_NULL': null,
    'ARY_VALUE': [ 'value1', 'value2' ],
    'ARY_NULL': null,
    'BOL_VALUE': true,
    'BOL_NULL': null,
    'DAT_VALUE': '2018-11-12T10:20:39',
    'DAT_NULL': null,
    '_etag': '5'
}}");

            var repositoryName = RepositoryType.OracleDb.ToCode();
            var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema());
            var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                repositoryName,
                new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

            builder.BuildUp();
            builder.Sql.Is($@"MERGE INTO ""{_tableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"", :p_9 AS ""p_9"", :p_10 AS ""p_10"", :p_11 AS ""p_11"", :p_12 AS ""p_12"", :p_13 AS ""p_13"", :p_14 AS ""p_14"", :p_15 AS ""p_15"" FROM dual
) X ON (""{_tableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""STR_VALUE"" = X.""p_3"", ""STR_NULL"" = X.""p_4"", ""INT_VALUE"" = X.""p_5"", ""DBL_VALUE"" = X.""p_6"", ""NUM_NULL"" = X.""p_7"", ""OBJ_VALUE"" = X.""p_8"", ""OBJ_NULL"" = X.""p_9"", ""ARY_VALUE"" = X.""p_10"", ""ARY_NULL"" = X.""p_11"", ""BOL_VALUE"" = X.""p_12"", ""BOL_NULL"" = X.""p_13"", ""DAT_VALUE"" = X.""p_14"", ""DAT_NULL"" = X.""p_15"", ""_etag"" = ""_etag"" + 1
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""STR_VALUE"", ""STR_NULL"", ""INT_VALUE"", ""DBL_VALUE"", ""NUM_NULL"", ""OBJ_VALUE"", ""OBJ_NULL"", ""ARY_VALUE"", ""ARY_NULL"", ""BOL_VALUE"", ""BOL_NULL"", ""DAT_VALUE"", ""DAT_NULL"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"", X.""p_9"", X.""p_10"", X.""p_11"", X.""p_12"", X.""p_13"", X.""p_14"", X.""p_15"")
");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { ":p_1", 1L },
                { ":p_2", id },
                { ":p_3", "abc" },
                { ":p_4", null },
                { ":p_5", 123L },
                { ":p_6", 123.456 },
                { ":p_7", null },
                { ":p_8", JToken.Parse("{ 'key': 'value' }").ToString() },
                { ":p_9", null },
                { ":p_10", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                { ":p_11", null },
                { ":p_12", true },
                { ":p_13", null },
                { ":p_14", DateTime.Parse("2018-11-12T10:20:39") },
                { ":p_15", null }
            });
        }

        [TestMethod]
        public void BuildUp_楽観排他OFF_省略項目あり_Update()
        {
            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'STR_NULL': 'abc',
    'NUM_NULL': 123,
    'OBJ_NULL': {{ 'key': 'value' }},
    'ARY_NULL': [ 'value1', 'value2' ],
    'BOL_NULL': false,
    'DAT_NULL': '2018-11-12T10:20:39',
    '_etag': '5'
}}");

            var repositoryName = RepositoryType.OracleDb.ToCode();
            var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema(), new ActionTypeVO(ActionType.Update));
            var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                repositoryName,
                new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

            builder.BuildUp();
            builder.Sql.Is($@"MERGE INTO ""{_tableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"" FROM dual
) X ON (""{_tableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""STR_NULL"" = X.""p_3"", ""NUM_NULL"" = X.""p_4"", ""OBJ_NULL"" = X.""p_5"", ""ARY_NULL"" = X.""p_6"", ""BOL_NULL"" = X.""p_7"", ""DAT_NULL"" = X.""p_8"", ""_etag"" = ""_etag"" + 1
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""STR_NULL"", ""NUM_NULL"", ""OBJ_NULL"", ""ARY_NULL"", ""BOL_NULL"", ""DAT_NULL"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"")
");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { ":p_1", 1L },
                { ":p_2", id },
                { ":p_3", "abc" },
                { ":p_4", 123L },
                { ":p_5", JToken.Parse("{ 'key': 'value' }").ToString() },
                { ":p_6", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                { ":p_7", false },
                { ":p_8", DateTime.Parse("2018-11-12T10:20:39") }
            });
        }

        [TestMethod]
        public void BuildUp_楽観排他OFF_省略項目あり_Regist()
        {
            var id = Guid.NewGuid().ToString();
            var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'STR_VALUE': 'abc',
    'STR_NULL': 'def',
    '_etag': '5'
}}");

            var repositoryName = RepositoryType.OracleDb.ToCode();
            var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema(), new ActionTypeVO(ActionType.Regist));
            var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                repositoryName,
                new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

            builder.BuildUp();
            builder.Sql.Is($@"MERGE INTO ""{_tableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"", :p_9 AS ""p_9"", :p_10 AS ""p_10"", :p_11 AS ""p_11"", :p_12 AS ""p_12"", :p_13 AS ""p_13"", :p_14 AS ""p_14"", :p_15 AS ""p_15"" FROM dual
) X ON (""{_tableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""STR_VALUE"" = X.""p_3"", ""STR_NULL"" = X.""p_4"", ""INT_VALUE"" = X.""p_5"", ""DBL_VALUE"" = X.""p_6"", ""NUM_NULL"" = X.""p_7"", ""OBJ_VALUE"" = X.""p_8"", ""OBJ_NULL"" = X.""p_9"", ""ARY_VALUE"" = X.""p_10"", ""ARY_NULL"" = X.""p_11"", ""BOL_VALUE"" = X.""p_12"", ""BOL_NULL"" = X.""p_13"", ""DAT_VALUE"" = X.""p_14"", ""DAT_NULL"" = X.""p_15"", ""_etag"" = ""_etag"" + 1
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""STR_VALUE"", ""STR_NULL"", ""INT_VALUE"", ""DBL_VALUE"", ""NUM_NULL"", ""OBJ_VALUE"", ""OBJ_NULL"", ""ARY_VALUE"", ""ARY_NULL"", ""BOL_VALUE"", ""BOL_NULL"", ""DAT_VALUE"", ""DAT_NULL"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"", X.""p_9"", X.""p_10"", X.""p_11"", X.""p_12"", X.""p_13"", X.""p_14"", X.""p_15"")
");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { ":p_1", 1L },
                { ":p_2", id },
                { ":p_3", "abc" },
                { ":p_4", "def" },
                { ":p_5", null },
                { ":p_6", null },
                { ":p_7", null },
                { ":p_8", null },
                { ":p_9", null },
                { ":p_10", null },
                { ":p_11", null },
                { ":p_12", null },
                { ":p_13", null },
                { ":p_14", null },
                { ":p_15", null }
            });
        }

        [TestMethod]
        public void BuildUp_バージョン情報()
        {
            var versionTableName = "VersionInfo";
            var id = Guid.NewGuid().ToString();
            var reguserId = Guid.NewGuid().ToString();
            var regdate = DateTime.Now.ToString("yyyy/MM/ddTHH:mm:ss.FFFFFFFZ");
            var upduserId = Guid.NewGuid().ToString();
            var upddate = DateTime.Now.ToString("yyyy/MM/ddTHH:mm:ss.FFFFFFFZ");
            var documentversions = $@"
[
    {{
      'version': 1,
      '_Reguser_Id': '{reguserId}',
      '_Regdate': '{regdate}',
      'is_current': true
    }}
]";
            var json = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'currentversion': 1,
    'documentversions': {JToken.Parse(documentversions).ToString()},
    '_Reguser_Id': '{reguserId}',
    '_Regdate': '{regdate}',
    '_Upduser_Id': '{upduserId}',
    '_Upddate': '{upddate}',
    '_etag': '5',
    'hoge': 'fuga'
}}");
            var expectedVersionInfo = JToken.Parse($@"
{{ 
    'id': '{id}', 
    'currentversion': 1,
    'documentversions': {JToken.Parse(documentversions).ToString()},
    '_Reguser_Id': '{reguserId}',
    '_Regdate': '{regdate}',
    '_Upduser_Id': '{upduserId}',
    '_Upddate': '{upddate}',
    'hoge': 'fuga'
}}");

            var repositoryName = RepositoryType.OracleDb.ToCode();
            var registerParam = ValueObjectUtil.Create<RegisterParam>(json, GetDataSchema(), new OperationInfo(OperationInfo.OperationType.VersionInfo));
            var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var builder = UnityContainer.Resolve<IUpsertSqlBuilder>(
                repositoryName,
                new ParameterOverride("registerParam", new InjectionParameter<RegisterParam>(registerParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)));

            builder.BuildUp();
            builder.Sql.Is($@"MERGE INTO ""{versionTableName}""
USING (
    SELECT :p_1 AS ""p_1"", :p_2 AS ""p_2"", :p_3 AS ""p_3"", :p_4 AS ""p_4"", :p_5 AS ""p_5"", :p_6 AS ""p_6"", :p_7 AS ""p_7"", :p_8 AS ""p_8"" FROM dual
) X ON (""{versionTableName}"".""id"" = :p_2)
WHEN MATCHED THEN
UPDATE SET ""currentversion"" = X.""p_3"", ""_Reguser_Id"" = X.""p_4"", ""_Regdate"" = X.""p_5"", ""_Upduser_Id"" = X.""p_6"", ""_Upddate"" = X.""p_7"", ""versioninfo"" = X.""p_8"", ""_etag"" = ""_etag"" + 1
WHERE ""{versionTableName}"".""_etag"" = :p_9 
WHEN NOT MATCHED THEN
INSERT (""_etag"", ""id"", ""currentversion"", ""_Reguser_Id"", ""_Regdate"", ""_Upduser_Id"", ""_Upddate"", ""versioninfo"") VALUES (X.""p_1"", X.""p_2"", X.""p_3"", X.""p_4"", X.""p_5"", X.""p_6"", X.""p_7"", X.""p_8"")
");

            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { ":p_1", 1L },
                { ":p_2", id },
                { ":p_3", 1L },
                { ":p_4", reguserId },
                { ":p_5", regdate },
                { ":p_6", upduserId },
                { ":p_7", upddate },
                { ":p_8", expectedVersionInfo.ToString() },
                { ":p_9", 5L }
            });
        }

        private DataSchema GetDataSchema(bool isEtagModel = false)
        {
            if (isEtagModel)
            {
                return new DataSchema($@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '_etag':        {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}");
            }
            else
            {
                return new DataSchema($@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }}
    }},
    'additionalProperties': false
}}");
            }
        }
    }
}
