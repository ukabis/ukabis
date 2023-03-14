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
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.Infrastructure.Database.Consts;
using JP.DataHub.UnitTest.Com;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    [TestClass]
    public class UnitTest_OracleDbODataPatchSqlBuilder : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private string _tableName = Guid.NewGuid().ToString();


        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterInstance(Configuration);

            UnityContainer.RegisterType<IODataPatchSqlBuilder, OracleDbODataPatchSqlBuilder>(RepositoryType.OracleDb.ToCode(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<QueryParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null),
                    new InjectionParameter<JToken>(null)));

            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
        }


        [TestMethod]
        public void BuildUp_追加条件なし()
        {
            var dateTimeValue = DateTime.Parse("2021-06-16T11:12:13");
            var odataFilterCondition = Guid.NewGuid().ToString();
            var odataFilterParameters = new Dictionary<string, object>()
            {
                { $":{Guid.NewGuid()}", Guid.NewGuid().ToString() },
                { $":{Guid.NewGuid()}", 1 },
                { $":{Guid.NewGuid()}", 123.5 },
                { $":{Guid.NewGuid()}", dateTimeValue }
            };
            var nativeQuery = new NativeQuery($"SELECT * FROM {{TABLE_NAME}} WHERE \"COLUMN1\" = '{odataFilterCondition}'", odataFilterParameters);
            var patchData = JToken.Parse($@"
{{ 
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
            var queryParam = ValueObjectUtil.Create<QueryParam>(GetDataSchema(), nativeQuery);
            var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var builder = UnityContainer.Resolve<IODataPatchSqlBuilder>(
                repositoryName,
                new ParameterOverride("queryParam", new InjectionParameter<QueryParam>(queryParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)),
                new ParameterOverride("patchData", new InjectionParameter<JToken>(patchData)));

            builder.BuildUp();
            builder.Sql.Is($@"UPDATE ""{_tableName}""
SET ""STR_VALUE"" = :odp_1, ""STR_NULL"" = :odp_2, ""INT_VALUE"" = :odp_3, ""DBL_VALUE"" = :odp_4, ""NUM_NULL"" = :odp_5, ""OBJ_VALUE"" = :odp_6, ""OBJ_NULL"" = :odp_7, ""ARY_VALUE"" = :odp_8, ""ARY_NULL"" = :odp_9, ""BOL_VALUE"" = :odp_10, ""BOL_NULL"" = :odp_11, ""DAT_VALUE"" = :odp_12, ""DAT_NULL"" = :odp_13, ""_etag"" = ""_etag"" + 1
WHERE ""COLUMN1"" = '{odataFilterCondition}'
");

            var expectedParameters = new Dictionary<string, object>()
            {
                { ":odp_1", "abc" },
                { ":odp_2", null },
                { ":odp_3", 123L },
                { ":odp_4", 123.456 },
                { ":odp_5", null },
                { ":odp_6", JToken.Parse("{ 'key': 'value' }").ToString() },
                { ":odp_7", null },
                { ":odp_8", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                { ":odp_9", null },
                { ":odp_10", true },
                { ":odp_11", null },
                { ":odp_12", DateTime.Parse("2018-11-12T10:20:39") },
                { ":odp_13", null }
            };
            foreach (var key in odataFilterParameters.Keys)
            {
                expectedParameters.Add(key, odataFilterParameters[key]);
            }
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(expectedParameters);
        }

        [TestMethod]
        [TestCase("STR_NULL")]
        [TestCase(JsonPropertyConst.OWNERID)]
        public void BuildUp_追加条件あり()
        {
            TestContext.Run((string columnName) =>
            {
                var dateTimeValue = DateTime.Parse("2021-06-16T11:12:13");
                var odataFilterCondition = Guid.NewGuid().ToString();
                var odataFilterParameters = new Dictionary<string, object>()
                {
                    { $":{Guid.NewGuid()}", Guid.NewGuid().ToString() },
                    { $":{Guid.NewGuid()}", 1 },
                    { $":{Guid.NewGuid()}", 123.5 },
                    { $":{Guid.NewGuid()}", dateTimeValue }
                };
                var nativeQuery = new NativeQuery($"SELECT * FROM {{TABLE_NAME}} WHERE \"COLUMN1\" = '{odataFilterCondition}'", odataFilterParameters);
                var patchData = JToken.Parse($@"
{{ 
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
    '_Where': {{
        'ColumnName': '{columnName}',
        'Operator': 'in',
        'Object': [ 'AA', 'BB', 'CC' ]
    }}
}}");

                var repositoryName = RepositoryType.OracleDb.ToCode();
                var queryParam = ValueObjectUtil.Create<QueryParam>(GetDataSchema(), nativeQuery);
                var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
                var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
                mockContainerDynamicSeparation
                    .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                    .Returns(_tableName);

                var builder = UnityContainer.Resolve<IODataPatchSqlBuilder>(
                    repositoryName,
                    new ParameterOverride("queryParam", new InjectionParameter<QueryParam>(queryParam)),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)),
                    new ParameterOverride("patchData", new InjectionParameter<JToken>(patchData)));

                builder.BuildUp();
                builder.Sql.Is($@"UPDATE ""{_tableName}""
SET ""STR_VALUE"" = :odp_1, ""STR_NULL"" = :odp_2, ""INT_VALUE"" = :odp_3, ""DBL_VALUE"" = :odp_4, ""NUM_NULL"" = :odp_5, ""OBJ_VALUE"" = :odp_6, ""OBJ_NULL"" = :odp_7, ""ARY_VALUE"" = :odp_8, ""ARY_NULL"" = :odp_9, ""BOL_VALUE"" = :odp_10, ""BOL_NULL"" = :odp_11, ""DAT_VALUE"" = :odp_12, ""DAT_NULL"" = :odp_13, ""_etag"" = ""_etag"" + 1
WHERE ""COLUMN1"" = '{odataFilterCondition}' AND ""{columnName}"" in (:odpex_1,:odpex_2,:odpex_3)
");

                var expectedParameters = new Dictionary<string, object>()
                {
                    { ":odp_1", "abc" },
                    { ":odp_2", null },
                    { ":odp_3", 123L },
                    { ":odp_4", 123.456 },
                    { ":odp_5", null },
                    { ":odp_6", JToken.Parse("{ 'key': 'value' }").ToString() },
                    { ":odp_7", null },
                    { ":odp_8", JToken.Parse("[ 'value1', 'value2' ]").ToString() },
                    { ":odp_9", null },
                    { ":odp_10", true },
                    { ":odp_11", null },
                    { ":odp_12", DateTime.Parse("2018-11-12T10:20:39") },
                    { ":odp_13", null }
                };
                foreach (var key in odataFilterParameters.Keys)
                {
                    expectedParameters.Add(key, odataFilterParameters[key]);
                }
                expectedParameters.Add(":odpex_1", "AA");
                expectedParameters.Add(":odpex_2", "BB");
                expectedParameters.Add(":odpex_3", "CC");
                builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(expectedParameters);
            });
        }

        [TestMethod]
        [TestCase("STR_VALUE", "in", "'hoge'")]                        // Object不正(配列でない)
        public void BuildUp_追加条件あり_条件不正()
        {
            TestContext.Run((string columnName, string @operator, string @object) =>
            {
                var odataFilterCondition = Guid.NewGuid().ToString();
                var odataFilterParameters = new Dictionary<string, object>()
                {
                    { $"@{Guid.NewGuid()}", Guid.NewGuid().ToString() }
                };
                var nativeQuery = new NativeQuery($"SELECT * FROM {{TABLE_NAME}} WHERE \"COLUMN1\" = '{Guid.NewGuid()}'", new Dictionary<string, object>());
                var patchData = JToken.Parse($@"
{{ 
    'STR_VALUE': 'abc',
    '_Where': {{
        'ColumnName': '{columnName}',
        'Operator': '{@operator}',
        'Object': {@object}
    }}
}}");

                var repositoryName = RepositoryType.OracleDb.ToCode();
                var queryParam = ValueObjectUtil.Create<QueryParam>(GetDataSchema(), nativeQuery);
                var repositoryInfo = new RepositoryInfo(repositoryName, new Dictionary<string, bool>() { { Guid.NewGuid().ToString(), false } });
                var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
                mockContainerDynamicSeparation
                    .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                    .Returns(_tableName);

                var builder = UnityContainer.Resolve<IODataPatchSqlBuilder>(
                    repositoryName,
                    new ParameterOverride("queryParam", new InjectionParameter<QueryParam>(queryParam)),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object)),
                    new ParameterOverride("patchData", new InjectionParameter<JToken>(patchData)));

                var ex = AssertEx.Throws<Rfc7807Exception>(() => builder.BuildUp());
                (ex.Rfc7807 as RFC7807ProblemDetailExtendErrors).ErrorCode.Is("E10440");
            });
        }


        private DataSchema GetDataSchema()
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
