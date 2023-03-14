using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Injection;
using Unity.Resolution;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Infrastructure.Database.Consts;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.ResourceSchemaAdapter;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.ResourceSchemaAdapter
{
    [TestClass]
    public class UnitTest_SqlServerResourceSchemaAdapter : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private UnityContainer UnityContainer;
        private DataSchema BaseSchema = new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }}
    }},
    ""additionalProperties"": false
}}");
        private DataSchema EtagSchema = new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }},
        ""_etag"": {{ ""type"": ""string"" }}
    }},
    ""additionalProperties"": false
}}");

        private string TableName = Guid.NewGuid().ToString();

        [TestInitialize]
        public void TestInitialize()
        {
            UnityContainer = new UnityContainer();
            UnityContainer.RegisterType<IResourceSchemaAdapter, SqlServerResourceSchemaAdapter>(RepositoryType.SQLServer2.ToCode(),
                new InjectionConstructor(
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<ControllerId>(null),
                    new InjectionParameter<DataSchema>(null)));

            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation.Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>())).Returns(TableName);
            UnityContainer.RegisterInstance<IContainerDynamicSeparationRepository>(mockContainerDynamicSeparation.Object);

            var mockDataStoreRepository = new Mock<INewDynamicApiDataStoreRdbmsRepository>();
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRdbmsRepository>(RepositoryType.SQLServer2.ToCode(), mockDataStoreRepository.Object);

            UnityContainer.RegisterInstance<IConfiguration>(Configuration);

            UnityCore.UnityContainer = UnityContainer;

            var dataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(dataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);
        }


        #region IsAdaptable & Adapt

        [TestMethod]
        public void IsAdaptable_OK_既存テーブルなし()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(false, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(BaseSchema)));

            adapter.IsAdaptable(out var rfc7807).IsTrue();
            rfc7807.IsNull();

            adapter.Adapt();
            Verify(mock, 1, 1, 0, 16); // TABLE:1, INDEX(MAX以外): 7, INDEX(管理項目): 8
        }

        [TestMethod]
        public void IsAdaptable_OK_変更なし()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(BaseSchema)));

            adapter.IsAdaptable(out var rfc7807).IsTrue();
            rfc7807.IsNull();

            adapter.Adapt();
            Verify(mock, 1, 0, 0, 0);
        }

        [TestMethod]
        public void IsAdaptable_OK_プロパティ追加()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""ADD_STR1"": {{ ""type"": ""string"" }},
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""ADD_STR2"": {{ ""type"": [""string"", ""null""], ""maxLength"": 100 }},
        ""ADD_NUM1"": {{ ""type"": ""number"" }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""ADD_NUM"": {{ ""type"": [""number"", ""null""] }},
        ""ADD_OBJ1"": {{ ""type"": ""object"" }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ADD_OBJ2"": {{ ""type"": [""object"", ""null""] }},
        ""ADD_ARY1"": {{ ""type"": ""array"" }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""ADD_ARY2"": {{ ""type"": [""array"", ""null""] }},
        ""ADD_BOL1"": {{ ""type"": ""boolean"" }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""ADD_BOL2"": {{ ""type"": [""boolean"", ""null""] }},
        ""ADD_DAT1"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }},
        ""ADD_DAT2"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }}
    }},
    ""additionalProperties"": false
}}"))));

            adapter.IsAdaptable(out var rfc7807).IsTrue();
            rfc7807.IsNull();

            adapter.Adapt();
            Verify(mock, 1, 0, 1, 7); // TABLE:1, INDEX(追加項目(MAX以外)): 7
        }

        [TestMethod]
        [TestCase("abcdefghijklmnopqrstuvwxyz")]
        [TestCase("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
        [TestCase("0123456789")]
        [TestCase("記号!#$%&\\'()*+,-./:;<=>?[\\\\]^_`{{|}}~")]
        [TestCase("ROWID_")]
        [TestCase("_ROWID")]
        public void IsAdaptable_OK_列名()
        {
            TestContext.Run<string>((name) =>
            {
                var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
                var mock = SetupTableColumn(true, repositoryInfo, 100, 40);

                var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                    RepositoryType.SQLServer2.ToCode(),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                    new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }},
        ""{name}"": {{ ""type"": ""string"", ""maxLength"": 100 }},
    }},
    ""additionalProperties"": false
}}"))));

                adapter.IsAdaptable(out var rfc7807).IsTrue();
                rfc7807.IsNull();

                adapter.Adapt();
                Verify(mock, 1, 0, 1, 1); // TABLE:1, INDEX(追加項目(MAX以外)): 1
            });
        }

        [TestMethod]
        public void IsAdaptable_OK_etagあり()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(false, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(EtagSchema)));

            adapter.IsAdaptable(out var rfc7807).IsTrue();
            rfc7807.IsNull();

            adapter.Adapt();
            Verify(mock, 1, 1, 0, 16); // TABLE:1, INDEX(MAX以外): 7, INDEX(管理項目): 8
        }

        [TestMethod]
        public void IsAdaptable_NG_リソースモデルなし()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(null)));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50409");
            Verify(mock, 0, 0, 0, 0);
        }

        [TestMethod]
        public void IsAdaptable_NG_AllowAdditionalProperties()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }}
    }}
}}"))));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50410");
            rfc7807.Errors.Count.Is(1);
            rfc7807.Errors.First().Key.Is("");
            Assert.IsTrue(rfc7807.Errors.First().Value[0].Contains("additionalProperties"));
            Assert.IsTrue(rfc7807.Errors.First().Value.Length == 1);
            Verify(mock, 1, 0, 0, 0);
        }

        [TestMethod]
        [TestCase("allOf")]
        [TestCase("anyOf")]
        [TestCase("oneOf")]
        public void IsAdaptable_NG_複合スキーマ()
        {
            TestContext.Run<string>((name) =>
            {
                var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
                var mock = SetupTableColumn(true, repositoryInfo);

                var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                    RepositoryType.SQLServer2.ToCode(),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                    new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }}
    }},
    ""{name}"": [ {{ ""type"": ""string"" }} ],
    ""additionalProperties"": false
}}"))));

                adapter.IsAdaptable(out var rfc7807).IsFalse();
                rfc7807.IsNotNull();
                rfc7807.ErrorCode.Is("E50410");
                rfc7807.Errors.Count.Is(1);
                rfc7807.Errors.First().Key.Is("");
                Assert.IsTrue(rfc7807.Errors.First().Value[0].Contains(name));
                Assert.IsTrue(rfc7807.Errors.First().Value.Length == 1);
                Verify(mock, 1, 0, 0, 0);
            });
        }

        [TestMethod]
        public void IsAdaptable_NG_列なし()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""additionalProperties"": false
}}"))));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50410");
            Assert.IsTrue(rfc7807.Errors[""].Length == 1);
            Verify(mock, 1, 0, 0, 0);
        }

        [TestMethod]
        public void IsAdaptable_NG_列数オーバー()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo, 1);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(BaseSchema)));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50410");
            Assert.IsTrue(rfc7807.Errors[""].Length == 1);
            Verify(mock, 1, 0, 0, 0);
        }

        [TestMethod]
        [TestCase("!#$%&'()*+,-./:;<=>?[\\\\]^_`{|}~\\\"", "string")]        // "を含む
        [TestCase("!#$%&'()*+,-./:;<=>?[\\\\]^_`{|}~@", "string")]        // @を含む
        [TestCase("1234567890123456789012345678901234567890$", "string")] // TooLong
        [TestCase("AAA\0", "string")] // ヌル文字を含む
        [TestCase("ADD_COL", "null")] // 型なし
        public void IsAdaptable_NG_列名と型()
        {
            TestContext.Run<string, string>((name, type) =>
            {
                var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
                var mock = SetupTableColumn(true, repositoryInfo, 100, 40);

                var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                    RepositoryType.SQLServer2.ToCode(),
                    new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                    new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                    new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }},
        ""{name}"": {{ ""type"": ""{type}"" }},
    }},
    ""additionalProperties"": false
}}"))));

                adapter.IsAdaptable(out var rfc7807).IsFalse();
                rfc7807.IsNotNull();
                rfc7807.ErrorCode.Is("E50410");
                rfc7807.Errors.Count.Is(1);
                rfc7807.Errors.First().Key.Is(name.Replace("\\\\", "\\").Replace("\\\"", "\""));
                Assert.IsTrue(rfc7807.Errors.First().Value.Length == 1);
                Verify(mock, 1, 0, 0, 0);
            });
        }

        [TestMethod]
        public void IsAdaptable_NG_列型なし()
        {
            var columnName = Guid.NewGuid().ToString();
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo, 100, 40);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }},
        ""{columnName}"": {{ ""format"": ""date-time"" }}
    }},
    ""additionalProperties"": false
}}"))));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50410");
            rfc7807.Errors.Count.Is(1);
            rfc7807.Errors.First().Key.Is(columnName);
            Assert.IsTrue(rfc7807.Errors.First().Value.Length == 1);
            Verify(mock, 1, 0, 0, 0);
        }

        [TestMethod]
        public void IsAdaptable_NG_既存プロパティ削除()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": [""string"", ""null""] }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": [""number"", ""null""] }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": [""object"", ""null""] }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": [""array"", ""null""] }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": [""boolean"", ""null""] }},
        ""DAT_VALUE"": {{ ""type"": ""string"", ""format"": ""date-time"" }},
        ""DAT_NULL"": {{ ""type"": [""string"", ""null""], ""format"": ""date-time"" }},
    }},
    ""additionalProperties"": false
}}"))));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50410");
            rfc7807.Errors.Count.Is(1);
            rfc7807.Errors.First().Key.Is("DBL_VALUE");
            Assert.IsTrue(rfc7807.Errors.First().Value.Length == 1);
            Verify(mock, 1, 0, 0, 0);
        }

        [TestMethod]
        public void IsAdaptable_NG_既存プロパティ型変更()
        {
            var repositoryInfo = new RepositoryInfo(RepositoryType.SQLServer2.ToCode(), new Dictionary<string, bool> { { Guid.NewGuid().ToString(), false } });
            var mock = SetupTableColumn(true, repositoryInfo);

            var adapter = UnityContainer.Resolve<IResourceSchemaAdapter>(
                RepositoryType.SQLServer2.ToCode(),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("controllerId", new InjectionParameter<ControllerId>(new ControllerId(Guid.NewGuid().ToString()))),
                new ParameterOverride("controllerSchema", new InjectionParameter<DataSchema>(new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""STR_VALUE"": {{ ""type"": ""string"", ""maxLength"": 100 }},
        ""STR_NULL"": {{ ""type"": ""string"" }},
        ""INT_VALUE"": {{ ""type"": ""number"" }},
        ""DBL_VALUE"": {{ ""type"": ""number"" }},
        ""NUM_NULL"": {{ ""type"": ""number"" }},
        ""OBJ_VALUE"": {{ ""type"": ""object"" }},
        ""OBJ_NULL"": {{ ""type"": ""object"" }},
        ""ARY_VALUE"": {{ ""type"": ""array"" }},
        ""ARY_NULL"": {{ ""type"": ""array"" }},
        ""BOL_VALUE"": {{ ""type"": ""boolean"" }},
        ""BOL_NULL"": {{ ""type"": ""boolean"" }},
        ""DAT_VALUE"": {{ ""type"": ""string"" }},
        ""DAT_NULL"": {{ ""type"": ""string"", ""format"": ""date-time"" }}
    }},
    ""additionalProperties"": false
}}"))));

            adapter.IsAdaptable(out var rfc7807).IsFalse();
            rfc7807.IsNotNull();
            rfc7807.ErrorCode.Is("E50410");
            rfc7807.Errors.Count.Is(1);
            rfc7807.Errors.First().Key.Is("DAT_VALUE");
            Assert.IsTrue(rfc7807.Errors.First().Value.Length == 1);
            Verify(mock, 1, 0, 0, 0);
        }

        #endregion


        private Mock<INewDynamicApiDataStoreRdbmsRepository> SetupTableColumn(bool exists, RepositoryInfo repositoryInfo, int maxColumns = 100, int maxColumnNameBytes = 100)
        {
            var tableColumns = new List<RdbmsTableColumn>();
            if (exists)
            {
                tableColumns.Add(new RdbmsTableColumn("id", "nvarchar"));
                tableColumns.Add(new RdbmsTableColumn("STR_VALUE", "nvarchar"));
                tableColumns.Add(new RdbmsTableColumn("STR_NULL", "nvarchar(MAX)"));
                tableColumns.Add(new RdbmsTableColumn("INT_VALUE", "numeric"));
                tableColumns.Add(new RdbmsTableColumn("DBL_VALUE", "numeric"));
                tableColumns.Add(new RdbmsTableColumn("NUM_NULL", "numeric"));
                tableColumns.Add(new RdbmsTableColumn("OBJ_VALUE", "nvarchar(MAX)"));
                tableColumns.Add(new RdbmsTableColumn("OBJ_NULL", "nvarchar(MAX)"));
                tableColumns.Add(new RdbmsTableColumn("ARY_VALUE", "nvarchar(MAX)"));
                tableColumns.Add(new RdbmsTableColumn("ARY_NULL", "nvarchar(MAX)"));
                tableColumns.Add(new RdbmsTableColumn("BOL_VALUE", "bit"));
                tableColumns.Add(new RdbmsTableColumn("BOL_NULL", "bit"));
                tableColumns.Add(new RdbmsTableColumn("DAT_VALUE", "datetime"));
                tableColumns.Add(new RdbmsTableColumn("DAT_NULL", "datetime"));
                tableColumns.Add(new RdbmsTableColumn("_Version", "int"));
                tableColumns.Add(new RdbmsTableColumn("_Vendor_Id", "uniqueidentifier"));
                tableColumns.Add(new RdbmsTableColumn("_System_Id", "uniqueidentifier"));
                tableColumns.Add(new RdbmsTableColumn("_Owner_Id", "uniqueidentifier"));
                tableColumns.Add(new RdbmsTableColumn("_Reguser_Id", "uniqueidentifier"));
                tableColumns.Add(new RdbmsTableColumn("_Regdate", "datetime"));
                tableColumns.Add(new RdbmsTableColumn("_Upduser_Id", "uniqueidentifier"));
                tableColumns.Add(new RdbmsTableColumn("_Upddate", "datetime"));
                tableColumns.Add(new RdbmsTableColumn("_etag", "int"));
            }

            var mockDataStoreRepository = new Mock<INewDynamicApiDataStoreRdbmsRepository>();
            mockDataStoreRepository.SetupGet(x => x.RepositoryInfo).Returns(repositoryInfo);
            mockDataStoreRepository.SetupGet(x => x.TableColumnNamePattern).Returns(SqlServerConsts.TableColumnNamePattern);
            mockDataStoreRepository.SetupGet(x => x.TableMaxColumns).Returns(maxColumns);
            mockDataStoreRepository.SetupGet(x => x.TableColumnNameMaxBytes).Returns(maxColumnNameBytes);
            mockDataStoreRepository.Setup(x => x.GetTableColumns(It.IsAny<string>())).Returns(tableColumns);
            mockDataStoreRepository
                .Setup(x => x.CreateTable(It.IsAny<string>(), It.IsAny<IEnumerable<RdbmsTableColumn>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()))
                .Callback<string, IEnumerable<RdbmsTableColumn>, IEnumerable<string>, string>((tableName, columns, primaryKeyColumns, primaryKeyName) =>
                {
                    tableName.Is(TableName);
                    primaryKeyColumns.Count().Is(1);
                    primaryKeyColumns.First().Is("id");
                    primaryKeyName.IsNull();
                });
            mockDataStoreRepository
                .Setup(x => x.AddTableColumns(It.IsAny<string>(), It.IsAny<IEnumerable<RdbmsTableColumn>>()))
                .Callback<string, IEnumerable<RdbmsTableColumn>>((tableName, columns) =>
                {
                    tableName.Is(TableName);
                });
            mockDataStoreRepository
                .Setup(x => x.CreateIndex(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((tableName, columnName, indexName) =>
                {
                    tableName.Is(TableName);
                    indexName.IsNull();
                });
            UnityContainer.RegisterInstance<INewDynamicApiDataStoreRdbmsRepository>(RepositoryType.SQLServer2.ToCode(), mockDataStoreRepository.Object);

            return mockDataStoreRepository;
        }

        private void Verify(Mock<INewDynamicApiDataStoreRdbmsRepository> mock, int getColumnsCount, int createTableCount, int alterTableCount, int createIndexCount)
        {
            mock.Verify(x => x.GetTableColumns(It.IsAny<string>()), Times.Exactly(getColumnsCount));
            mock.Verify(x => x.CreateTable(It.IsAny<string>(), It.IsAny<IEnumerable<RdbmsTableColumn>>(), It.IsAny<IEnumerable<string>>(), It.IsAny<string>()), Times.Exactly(createTableCount));
            mock.Verify(x => x.AddTableColumns(It.IsAny<string>(), It.IsAny<IEnumerable<RdbmsTableColumn>>()), Times.Exactly(alterTableCount));
            mock.Verify(x => x.CreateIndex(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(createIndexCount));
        }
    }
}
