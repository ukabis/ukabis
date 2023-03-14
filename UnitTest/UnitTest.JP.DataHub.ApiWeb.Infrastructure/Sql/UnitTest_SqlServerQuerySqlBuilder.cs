using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace JP.DataHub.ApiWeb.Infrastructure.Sql
{
    [TestClass]
    public class UnitTest_SqlServerQuerySqlBuilder : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private string _tableName = Guid.NewGuid().ToString();
        private string _physicalRepositoryId = Guid.NewGuid().ToString();

        private PerRequestDataContainer _perRequestDataContainer = null;

        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = new UnityContainer();
            UnityContainer = UnityCore.UnityContainer;

            UnityContainer.RegisterInstance(Configuration);

            UnityContainer.RegisterType<IQuerySqlBuilder, SqlServerQuerySqlBuilder>(RepositoryType.SQLServer2.ToCode(), new Interceptor<InterfaceInterceptor>(), new InterceptionBehavior<PolicyInjectionBehavior>(),
                new InjectionConstructor(
                    new InjectionParameter<QueryParam>(null),
                    new InjectionParameter<RepositoryInfo>(null),
                    new InjectionParameter<IResourceVersionRepository>(null),
                    new InjectionParameter<IContainerDynamicSeparationRepository>(null),
                    new InjectionParameter<IDynamicApiRepository>(null),
                    new InjectionParameter<bool>(false)));

            _perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IDataContainer>(_perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(_perRequestDataContainer);
        }


        #region BuildUp

        #region OData

        [TestMethod]
        public void BuildUp_OData_ページングなし_TOPなし()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" }
            });
            builder.IsCustomSql.IsFalse();
        }
        [TestMethod]
        public void BuildUp_OData_ページングなし_TOPなし_認可()
        {
            List<string> openIds = new List<string>() { Guid.NewGuid().ToString() };
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery,new XUserResourceSharing(openIds));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" }
            });
            builder.IsCustomSql.IsFalse();
        }
        [TestMethod]
        public void BuildUp_OData_ページングなし_TOPあり()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", 0L },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングなし_TOPあり_SKIPあり()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var skip = 5;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top), new SkipCount(skip));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", (long)skip },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングなし_TOPなし_SKIPあり()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var skip = 5;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SkipCount(skip));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", (long)skip }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_1ページ目()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top), new XRequestContinuation(""));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", 0L },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_1ページ目_SKIPあり()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var skip = 5;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top), new SkipCount(skip), new XRequestContinuation(""));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", (long)skip },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_2ページ目以降()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var requestContinuation = "5";
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top), new XRequestContinuation(requestContinuation));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", long.Parse(requestContinuation) },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_2ページ目以降_SKIPあり()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var skip = 5;
            var requestContinuation = "5";
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top), new SkipCount(skip), new XRequestContinuation(requestContinuation));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", long.Parse(requestContinuation) },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_RequestContinuation不正()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var requestContinuation = "hoge";
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new SelectCount(top), new XRequestContinuation(requestContinuation));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", 0L },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_TOPなし_TOP任意()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var requestContinuation = "5";
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new XRequestContinuation(requestContinuation));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", long.Parse(requestContinuation) },
                { "@r_fetch", 100L }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_ページングあり_TOPなし_TOP必須()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var requestContinuation = "5";
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, new XRequestContinuation(requestContinuation));

            var builder = CreateBuilder(queryParam, null, null, null, null, true);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_添付ファイルメタ取得()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE FileId = @p_1";
            var parameters = new Dictionary<string, object>() { { "@p_1", "hoge" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var operationInfo = new OperationInfo(OperationInfo.OperationType.AttachFileMeta);
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), nativeQuery, operationInfo);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"AttachFileMeta\" WHERE FileId = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "hoge" }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_OData_添付ファイルメタ検索_キーワードなし()
        {
            var q = new Dictionary<QueryStringKey, QueryStringValue>();
            var qs = new QueryStringVO(q);
            var operationInfo = new OperationInfo(OperationInfo.OperationType.AttachFileMeta, "hoge,fuga");
            var repositoryKey = new RepositoryKey("API~piyo");
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.OData), qs, operationInfo, repositoryKey);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT hoge,fuga FROM AttachFileMeta afm WITH(NOLOCK) WHERE \"_Version\" = @p_1 AND \"_Type\" = @p_2");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", "API~piyo" }
            });
            builder.IsCustomSql.IsFalse();
        }

        #endregion

        #region Query

        [TestMethod]
        public void BuildUp_Query_バージョン情報()
        {
            var query = "SELECT \"versioninfo\", \"_etag\" FROM \"VersionInfo\" WHERE \"id\" = @p_1";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.Query), new OperationInfo(OperationInfo.OperationType.VersionInfo), nativeQuery);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"versioninfo\", \"_etag\" FROM \"VersionInfo\" WHERE \"id\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_レスポンスモデルあり_リソースモデルと同じ()
        {
            var resourceSchema = GetDataSchema();
            var responseSchema = GetDataSchema();
            var queryParam = new QueryParam(null, null, null, resourceSchema, null, null, responseSchema, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_レスポンスモデルあり_リソースモデルと異なる_additionalProperties_false()
        {
            var resourceSchema = GetDataSchema();
            var responseSchema = GetDataSchemaSingleProperty();
            var queryParam = new QueryParam(null, null, null, resourceSchema, null, null, responseSchema, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_レスポンスモデルあり_リソースモデルと異なる_additionalProperties_true()
        {
            var resourceSchema = GetDataSchema();
            var responseSchema = GetDataSchemaAllowAdditionalProperties();
            var queryParam = new QueryParam(null, null, null, resourceSchema, null, null, responseSchema, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_楽観排他()
        {
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, new IsOptimisticConcurrency(true));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\",\"_etag\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_ベンダー依存()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(vendorId, systemId, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, new IsVendor(true), null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1 AND \"_Vendor_Id\" = @p_2 AND \"_System_Id\" = @p_3");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", vendorId.Value },
                { "@p_3", systemId.Value }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_個人依存()
        {
            var openId = new OpenId(Guid.NewGuid().ToString());
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(null, null, openId, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, new IsPerson(true), null, null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1 AND \"_Owner_Id\" = @p_2");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", openId.Value }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_領域越え()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(vendorId, systemId, openId, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, new IsVendor(true), new IsPerson(true), new IsOverPartition(true), null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [Ignore]
        [TestMethod]
        public void BuildUp_Query_データ共有()
        {
            // *****************************************************
            // NIY: データ共有処理追加時に対応
            // *****************************************************
            throw new NotImplementedException();
        }

        [TestMethod]
        public void BuildUp_Query_管理項目取得_依存なし()
        {
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            var dataContainer = new PerRequestDataContainer();
            dataContainer.XgetInternalAllField = true;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\",\"_Version\",\"_Reguser_Id\",\"_Regdate\",\"_Upduser_Id\",\"_Upddate\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_管理項目取得_ベンダー依存()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(vendorId, systemId, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, new IsVendor(true), null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);

            var dataContainer = new PerRequestDataContainer();
            dataContainer.XgetInternalAllField = true;
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\",\"_Version\",\"_Reguser_Id\",\"_Regdate\",\"_Upduser_Id\",\"_Upddate\",\"_Vendor_Id\",\"_System_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1 AND \"_Vendor_Id\" = @p_2 AND \"_System_Id\" = @p_3");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", vendorId.Value },
                { "@p_3", systemId.Value }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_QueryStringあり()
        {
            var controllerSchemaModel = GetDataSchema();
            var stringValue = Guid.NewGuid().ToString();
            var intValue = 123;
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("hoge"), new QueryStringValue(stringValue) },
                { new QueryStringKey("fuga"), new QueryStringValue(intValue.ToString()) },
            });
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, queryString, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1 AND \"hoge\" = @p_2 AND \"fuga\" = @p_3");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", stringValue },
                { "@p_3", (double)intValue }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Query_APIクエリあり()
        {
            var controllerSchemaModel = GetDataSchema();
            var stringValue = Guid.NewGuid().ToString();
            var intValue = 123;
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("hoge"), new QueryStringValue(stringValue) },
                { new QueryStringKey("fuga"), new QueryStringValue(intValue.ToString()) },
            });

            var queryType = new QueryType(QueryTypes.NativeDbQuery);
            var apiQuery = new ApiQuery("SELECT * FROM {TABLE_NAME} WHERE \"hoge\" = {hoge} AND \"fuga\" = {fuga}");
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, apiQuery, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, queryString, queryType, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM (SELECT * FROM \"{_tableName}\" WHERE \"hoge\" = @p_2 AND \"fuga\" = @p_3) src WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", stringValue },
                { "@p_3", (double)intValue }
            });
            builder.IsCustomSql.IsTrue();
        }
        [TestMethod]
        public void BuildUp_Query_APIクエリあり_認可()
        {
            var controllerSchemaModel = GetDataSchema();
            var stringValue = Guid.NewGuid().ToString();
            var intValue = 123;
            List<string> openIds = new List<string>() { Guid.NewGuid().ToString() };
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("hoge"), new QueryStringValue(stringValue) },
                { new QueryStringKey("fuga"), new QueryStringValue(intValue.ToString()) },
            });

            var queryType = new QueryType(QueryTypes.NativeDbQuery);
            var apiQuery = new ApiQuery("SELECT * FROM {TABLE_NAME} WHERE \"hoge\" = {hoge} AND \"fuga\" = {fuga}");
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, apiQuery, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, queryString, queryType, null, null, null,
                null, null, null, null, null, null, null,xUserResourceSharing:new XUserResourceSharing(openIds));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM (SELECT * FROM \"{_tableName}\" WHERE \"hoge\" = @p_3 AND \"fuga\" = @p_4) src WHERE \"_Version\" = @p_1 AND \"_Owner_Id\" IN @p_2");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", openIds.ToArray() },
                { "@p_3", stringValue },
                { "@p_4", (double)intValue }
            });
            builder.IsCustomSql.IsTrue();
        }
        [TestMethod]
        public void BuildUp_Query_APIクエリあり_JOINあり()
        {
            var controllerSchemaModel = GetDataSchema();
            var stringValue = Guid.NewGuid().ToString();
            var intValue = 123;
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("hoge"), new QueryStringValue(stringValue) },
                { new QueryStringKey("fuga"), new QueryStringValue(intValue.ToString()) },
            });

            var controllerId = Guid.NewGuid().ToString();
            var joinControllerId = Guid.NewGuid().ToString();
            var joinPhysicalRepositoryId = Guid.NewGuid().ToString();
            var joinTableName = Guid.NewGuid().ToString();

            var queryType = new QueryType(QueryTypes.NativeDbQuery);
            var apiQuery = new ApiQuery($"SELECT * FROM {{TABLE_NAME}} a LEFT JOIN {{TABLE_NAME:{joinControllerId}}} b ON a.\"hoge\" = b.\"hoge\" WHERE a.\"hoge\" = {{hoge}} AND a.\"fuga\" = {{fuga}}");
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, apiQuery, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, queryString, queryType, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, new ControllerId(controllerId), null, new IsOtherResourceSqlAccess(true));

            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(y => y != null && y.Value == _physicalRepositoryId), It.Is<ControllerId>(y => y != null && y.Value == controllerId), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(y => y != null && y.Value == joinPhysicalRepositoryId), It.Is<ControllerId>(y => y != null && y.Value == joinControllerId), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(joinTableName);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository
                .Setup(x => x.GetPhysicalRepositoryIdByControllerId(It.IsAny<ControllerId>(), It.Is<RepositoryType>(y => y == RepositoryType.SQLServer2)))
                .Returns(new PhysicalRepositoryId(joinPhysicalRepositoryId));

            var builder = CreateBuilder(queryParam, null, null, mockContainerDynamicSeparation.Object, mockDynamicApiRepository.Object);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM (SELECT * FROM \"{_tableName}\" a LEFT JOIN \"{joinTableName}\" b ON a.\"hoge\" = b.\"hoge\" WHERE a.\"hoge\" = @p_2 AND a.\"fuga\" = @p_3) src WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", stringValue },
                { "@p_3", (double)intValue }
            });
            builder.IsCustomSql.IsTrue();

            mockContainerDynamicSeparation
                .Verify(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(y => y != null && y.Value == _physicalRepositoryId), It.Is<ControllerId>(y => y != null && y.Value == controllerId), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()), Times.Exactly(1));
            mockContainerDynamicSeparation
                .Verify(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(y => y != null && y.Value == joinPhysicalRepositoryId), It.Is<ControllerId>(y => y != null && y.Value == joinControllerId), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()), Times.Exactly(1));
            mockDynamicApiRepository.Verify(x => x.GetPhysicalRepositoryIdByControllerId(It.IsAny<ControllerId>(), It.Is<RepositoryType>(y => y == RepositoryType.SQLServer2)), Times.Exactly(1));
        }

        [TestMethod]
        public void BuildUp_Query_APIクエリあり_JOINあり_テーブルなし()
        {
            var controllerSchemaModel = GetDataSchema();
            var stringValue = Guid.NewGuid().ToString();
            var intValue = 123;
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("hoge"), new QueryStringValue(stringValue) },
                { new QueryStringKey("fuga"), new QueryStringValue(intValue.ToString()) },
            });

            var controllerId = Guid.NewGuid().ToString();
            var joinControllerId = Guid.NewGuid().ToString();
            var joinPhysicalRepositoryId = Guid.NewGuid().ToString();

            var queryType = new QueryType(QueryTypes.NativeDbQuery);
            var apiQuery = new ApiQuery($"SELECT * FROM {{TABLE_NAME}} a LEFT JOIN {{TABLE_NAME:{joinControllerId}}} b ON a.\"hoge\" = b.\"hoge\" WHERE a.\"hoge\" = {{hoge}} AND a.\"fuga\" = {{fuga}}");
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, apiQuery, null, new ActionTypeVO(ActionType.Query),
                null, null, null, null, null, null, queryString, queryType, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, new ControllerId(controllerId), null, new IsOtherResourceSqlAccess(true));

            var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
            mockContainerDynamicSeparation
                .Setup(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(y => y != null && y.Value == _physicalRepositoryId), It.Is<ControllerId>(y => y != null && y.Value == controllerId), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                .Returns(_tableName);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository
                .Setup(x => x.GetPhysicalRepositoryIdByControllerId(It.IsAny<ControllerId>(), It.IsAny<RepositoryType>()))
                .Returns((PhysicalRepositoryId)null);

            var builder = CreateBuilder(queryParam, null, null, mockContainerDynamicSeparation.Object, mockDynamicApiRepository.Object);
            var exception = AssertEx.Throws<Rfc7807Exception>(() => builder.BuildUp());
            (exception.Rfc7807 as RFC7807ProblemDetailExtendErrors).ErrorCode.Is("E50411");

            mockContainerDynamicSeparation
                .Verify(x => x.GetOrRegisterContainerName(It.Is<PhysicalRepositoryId>(y => y != null && y.Value == _physicalRepositoryId), It.Is<ControllerId>(y => y != null && y.Value == controllerId), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()), Times.Exactly(1));
            mockDynamicApiRepository.Verify(x => x.GetPhysicalRepositoryIdByControllerId(It.IsAny<ControllerId>(), It.IsAny<RepositoryType>()), Times.Exactly(1));
        }

        #endregion

        #region ODataDelete

        [TestMethod]
        public void BuildUp_ODataDelete()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.ODataDelete), nativeQuery, new SelectCount(top));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"id\" FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", 0L },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_ODataDelete_履歴()
        {
            var query = "SELECT * FROM {TABLE_NAME} WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2";
            var parameters = new Dictionary<string, object>() { { "@p_1", "value1" }, { "@p_2", "value2" } };
            var nativeQuery = new NativeQuery(query, parameters, false);
            var top = 10;
            var queryParam = ValueObjectUtil.Create<QueryParam>(new ActionTypeVO(ActionType.ODataDelete), nativeQuery, new SelectCount(top), new IsDocumentHistory(true));

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT * FROM \"{_tableName}\" WHERE COLUMN1 = @p_1 AND COLUMN2 = @p_2 ORDER BY 1 OFFSET @r_offset ROWS FETCH NEXT @r_fetch ROWS ONLY");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", "value1" },
                { "@p_2", "value2" },
                { "@r_offset", 0L },
                { "@r_fetch", (long)top }
            });
            builder.IsCustomSql.IsFalse();
        }

        #endregion

        #region Delete

        [TestMethod]
        public void BuildUp_Delete_APIクエリなし()
        {
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.DeleteData),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"id\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        [TestMethod]
        public void BuildUp_Delete_APIクエリあり()
        {
            var controllerSchemaModel = GetDataSchema();
            var stringValue = Guid.NewGuid().ToString();
            var intValue = 123;
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("hoge"), new QueryStringValue(stringValue) },
                { new QueryStringKey("fuga"), new QueryStringValue(intValue.ToString()) },
            });
            var queryType = new QueryType(QueryTypes.NativeDbQuery);
            var apiQuery = new ApiQuery("SELECT * FROM {TABLE_NAME} WHERE \"hoge\" = {hoge} AND \"fuga\" = {fuga}");
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, apiQuery, null, new ActionTypeVO(ActionType.DeleteData),
                null, null, null, null, null, null, queryString, queryType, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"id\" FROM (SELECT * FROM \"{_tableName}\" WHERE \"hoge\" = @p_2 AND \"fuga\" = @p_3) src WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },
                { "@p_2", stringValue },
                { "@p_3", (double)intValue }
            });
            builder.IsCustomSql.IsTrue();
        }

        [TestMethod]
        public void BuildUp_Delete_履歴()
        {
            var controllerSchemaModel = GetDataSchema();
            var queryParam = new QueryParam(null, null, null, controllerSchemaModel, null, null, null, null, null, null, new ActionTypeVO(ActionType.DeleteData),
                null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, new IsDocumentHistory(true));
            _perRequestDataContainer.XgetInternalAllField = true;

            var builder = CreateBuilder(queryParam);
            builder.BuildUp();
            builder.Sql.Is($"SELECT \"hoge\",\"fuga\",\"foo\",\"bar\",\"id\",\"_Owner_Id\",\"_Version\",\"_Reguser_Id\",\"_Regdate\",\"_Upduser_Id\",\"_Upddate\" FROM \"{_tableName}\" WITH(NOLOCK) WHERE \"_Version\" = @p_1");
            builder.SqlParameterList.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
            builder.IsCustomSql.IsFalse();
        }

        #endregion

        #endregion


        #region PrepareQueryParameters

        [TestMethod]
        public void PrepareQueryParameters_OData()
        {
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("$filter"), new QueryStringValue("hoge eq 'value'") }
            });
            var queryParam = new QueryParam(null, null, null, null, null, null, null, null, null, null, new ActionTypeVO(ActionType.OData),
                null, null, null, null, null, null, queryString, null, null, null, null,
                null, null, null, null, null, null, null);

            var builder = CreateBuilder(queryParam);
            var parameters = builder.PrepareQueryParameters(queryParam);
            parameters.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 }
            });
        }
        [TestMethod]
        public void PrepareQueryParameters_OData_認可()
        {
            var queryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>()
            {
                { new QueryStringKey("$filter"), new QueryStringValue("hoge eq 'value'") }
            });
            List<string> openIds = new List<string>() { Guid.NewGuid().ToString() };
            var queryParam = new QueryParam(null, null, null, null, null, null, null, null, null, null, new ActionTypeVO(ActionType.OData),
                null, null, null, null, null, null, queryString, null, null, null, null,
                null, null, null, null, null, null, null,xUserResourceSharing:new XUserResourceSharing(openIds));

            var builder = CreateBuilder(queryParam);
            var parameters = builder.PrepareQueryParameters(queryParam);
            parameters.ToDictionary(x => x.Value.SqlParameter, y => y.Value.ParameterValue).IsStructuralEqual(new Dictionary<string, object>()
            {
                { "@p_1", 5 },{ "@p_2", openIds.ToArray() }
            });
        }
        #endregion


        #region 正規表現

        [TestMethod]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1", false)]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\"", true)]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\" DESC, \"zzz\" ASC", true)]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\" DESC, \"zzz\" ASC OFFSET 0 ROWS", true)]
        public void OrderByPattern()
        {
            TestContext.Run((string target, bool expected) =>
            {
                new Regex(SqlServerQuerySqlBuilder.OrderByPattern, RegexOptions.IgnoreCase).IsMatch(target).Is(expected);
            });
        }

        [TestMethod]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\"", true)]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\" DESC, \"zzz\" ASC", true)]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\" DESC, \"zzz\" ASC OFFSET 0 ROWS", false)]
        [TestCase("SELECT * FROM \"xxx\" WHERE \"_Version\" = @p_1 ORDER BY \"yyy\" DESC, \"zzz\" ASC OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY", false)]
        public void WithoutOffsetPattern()
        {
            TestContext.Run((string target, bool expected) =>
            {
                new Regex(SqlServerQuerySqlBuilder.WithoutOffsetPattern, RegexOptions.IgnoreCase).IsMatch(target).Is(expected);
            });
        }

        #endregion


        private IQuerySqlBuilder CreateBuilder(
            QueryParam queryParam,
            RepositoryInfo repositoryInfo = null,
            IResourceVersionRepository resourceVersionRepository = null,
            IContainerDynamicSeparationRepository containerDynamicSeparationRepository = null,
            IDynamicApiRepository dynamicApiRepository = null,
            bool xRequestContinuationNeedsTopCount = false)
        {
            var repositoryName = RepositoryType.SQLServer2.ToCode();
            if (repositoryInfo == null)
            {
                repositoryInfo = new RepositoryInfo(Guid.NewGuid(), RepositoryType.SQLServer2.ToCode(), new Tuple<string, bool, Guid?>(Guid.NewGuid().ToString(), false, Guid.Parse(_physicalRepositoryId)));
            }
            if (resourceVersionRepository == null)
            {
                var mockResourceVersionRepository = new Mock<IResourceVersionRepository>();
                mockResourceVersionRepository
                    .Setup(x => x.GetRegisterVersion(It.IsAny<RepositoryKey>(), It.IsAny<XVersion>()))
                    .Returns(new ResourceVersion(5));
                resourceVersionRepository = mockResourceVersionRepository.Object;
            }
            if (containerDynamicSeparationRepository == null)
            {
                var mockContainerDynamicSeparation = new Mock<IContainerDynamicSeparationRepository>();
                mockContainerDynamicSeparation
                    .Setup(x => x.GetOrRegisterContainerName(It.IsAny<PhysicalRepositoryId>(), It.IsAny<ControllerId>(), It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<OpenId>()))
                    .Returns(_tableName);
                containerDynamicSeparationRepository = mockContainerDynamicSeparation.Object;
            }
            if (dynamicApiRepository == null)
            {
                dynamicApiRepository = new Mock<IDynamicApiRepository>().Object;
            }

            return UnityContainer.Resolve<IQuerySqlBuilder>(
                repositoryName,
                new ParameterOverride("queryParam", new InjectionParameter<QueryParam>(queryParam)),
                new ParameterOverride("repositoryInfo", new InjectionParameter<RepositoryInfo>(repositoryInfo)),
                new ParameterOverride("resourceVersionRepository", new InjectionParameter<IResourceVersionRepository>(resourceVersionRepository)),
                new ParameterOverride("containerDynamicSeparationRepository", new InjectionParameter<IContainerDynamicSeparationRepository>(containerDynamicSeparationRepository)),
                new ParameterOverride("dynamicApiRepository", new InjectionParameter<IDynamicApiRepository>(dynamicApiRepository)),
                new ParameterOverride("xRequestContinuationNeedsTopCount", new InjectionParameter<bool>(xRequestContinuationNeedsTopCount)));
        }


        private DataSchema GetDataSchema()
        {
            return new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""hoge"": {{ ""type"": ""string"" }},
        ""fuga"": {{ ""type"": ""number"" }},
        ""foo"": {{ ""type"": ""object"", ""properties"": {{ ""name"": {{ ""type"": ""string"" }} }} }},
        ""bar"": {{ ""type"": ""array"", ""items"": {{ ""type"": ""number"" }} }}
    }},
    ""additionalProperties"": false
}}");
        }

        private DataSchema GetDataSchemaSingleProperty()
        {
            return new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""hoge"": {{ ""type"": ""string"" }}
    }},
    ""additionalProperties"": false
}}");
        }

        private DataSchema GetDataSchemaAllowAdditionalProperties()
        {
            return new DataSchema($@"
{{
    ""type"": ""object"",
    ""properties"": {{
        ""hoge"": {{ ""type"": ""string"" }}
    }}
}}");
        }
    }
}
