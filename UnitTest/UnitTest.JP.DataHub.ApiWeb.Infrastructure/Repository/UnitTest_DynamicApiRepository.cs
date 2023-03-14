using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using EasyCaching.Core;
using EasyCaching.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_DynamicApiRepository : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            var cachingProvider = new DefaultInMemoryCachingProvider("DynamicApi", new[] { new InMemoryCaching("DynamicApi", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>();

            UnityContainer.RegisterType<IMethod, Method>();
        }


        private Mock<IPerRequestDataContainer> CreatePerRequestDataContainerMock()
        {
            var dataContainerMock = new Mock<IPerRequestDataContainer>();
            dataContainerMock.SetupProperty(x => x.SystemId, Guid.NewGuid().ToString());
            dataContainerMock.SetupProperty(x => x.VendorId, Guid.NewGuid().ToString());
            dataContainerMock.Setup(x => x.GetDateTimeUtil()).Returns(new DateTimeUtil("yyyy/MM/dd", "yyyy/MM/dd hh:mm:ss tt", "yyyy/M/d"));
            return dataContainerMock;
        }


        [TestMethod]
        public void GetIpFilter_ip返却あり()
        {
            UnityContainer container = new UnityContainer();
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            UnityContainer.RegisterInstance<ICache>(new NoCache());
            UnityContainer.RegisterInstance<ICache>("DynamicApi", new NoCache());
            UnityContainer.RegisterInstance<bool>("EnableIpFilter", true);

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            string ipAddress = "1.1.1.1/32";
            mock.Setup(x => x.Query<string>(It.IsAny<string>(), It.IsAny<object>())).Returns(new[] { ipAddress });
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = new ApiId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.GetIpFilter(vendorId, systemId, controllerId, apiId);

            // 結果をチェック
            result.First().Value.Is(ipAddress.ToString());
        }

        [TestMethod]
        public void GetIpFilter_ipnull()
        {
            UnityContainer container = new UnityContainer();
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            UnityContainer.RegisterInstance<ICache>(new NoCache());
            UnityContainer.RegisterInstance<ICache>("DynamicApi", new NoCache());
            UnityContainer.RegisterInstance<bool>("EnableIpFilter", true);

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            string ipAddress = null;
            mock.Setup(x => x.Query<string>(It.IsAny<string>(), It.IsAny<object>())).Returns(new[] { ipAddress });
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = new ApiId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.GetIpFilter(vendorId, systemId, controllerId, apiId);

            // 結果をチェック
            result.First().Value.IsNull();
        }

        [TestMethod]
        public void GetIpFilter_disable()
        {
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            UnityContainer.RegisterInstance<ICache>(new NoCache());
            UnityContainer.RegisterInstance<ICache>("DynamicApi", new NoCache());
            UnityContainer.RegisterInstance<bool>("EnableIpFilter", false);

            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            string ipAddress = "1.1.1.1/32";
            mock.Setup(x => x.Query<string>(It.IsAny<string>(), It.IsAny<object>())).Returns(new[] { ipAddress });
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = new ApiId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.GetIpFilter(vendorId, systemId, controllerId, apiId);

            // 結果をチェック
            result.Any().IsFalse();
        }

        [TestMethod]
        public void ApiEntryToMethod_XgetInternal_指定なし_APIキャッシュ無()
        {
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            var containerMock = GetPerRequestDataContainerMock();

            var dynamicApiRepository = ApiEntryToMethod_GetIDynamicApiRepository();
            var result = (IMethod)dynamicApiRepository.GetType().InvokeMember("ApiEntryToMethod", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, dynamicApiRepository, new object[]
            {
                new AllApiEntity()
                {
                    all_repository_model_list = new List<AllApiRepositoryModel>
                    {
                        new AllApiRepositoryModel
                        {
                            repository_type_cd = "ddb",
                            physical_repository_list = new List<AllApiPhysicalRepositoryModel>(){ new AllApiPhysicalRepositoryModel() { repository_connection_string="hoge",is_full = false } }
                        }
                    },
                    action_type_cd = "quy",
                    is_cache = false
                },
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                "",
                "",
                new string[] { }
            });

            result.CacheInfo.Is(x => x.IsCache == false);
        }

        [TestMethod]
        public void ApiEntryToMethod_XgetInternal_指定なし_APIキャッシュ有()
        {
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            var containerMock = GetPerRequestDataContainerMock();

            var dynamicApiRepository = ApiEntryToMethod_GetIDynamicApiRepository();
            var result = (IMethod)dynamicApiRepository.GetType().InvokeMember("ApiEntryToMethod", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, dynamicApiRepository, new object[] {
                new AllApiEntity()
                {
                    all_repository_model_list = new List<AllApiRepositoryModel>
                    {
                        new AllApiRepositoryModel
                        {
                            repository_type_cd = "ddb",
                            physical_repository_list = new List<AllApiPhysicalRepositoryModel>(){ new AllApiPhysicalRepositoryModel() { repository_connection_string="hoge",is_full = false } }
                        }
                    },
                    action_type_cd = "quy",
                    is_cache = true
                },
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                "",
                "",
                new string[] { }
            });

            result.CacheInfo.Is(x => x.IsCache == true);
        }

        [TestMethod]
        public void ApiEntryToMethod_XgetInternal_All指定_APIキャッシュ無()
        {
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            var containerMock = GetPerRequestDataContainerMock();
            containerMock.SetupProperty(x => x.XgetInternalAllField, true);

            var dynamicApiRepository = ApiEntryToMethod_GetIDynamicApiRepository();
            var result = (IMethod)dynamicApiRepository.GetType().InvokeMember("ApiEntryToMethod", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, dynamicApiRepository, new object[] {
                new AllApiEntity()
                {
                    all_repository_model_list = new List<AllApiRepositoryModel>
                    {
                        new AllApiRepositoryModel
                        {
                            repository_type_cd = "ddb",
                            physical_repository_list = new List<AllApiPhysicalRepositoryModel>(){ new AllApiPhysicalRepositoryModel() { repository_connection_string="hoge",is_full = false } }
                        }
                    },
                    action_type_cd = "quy",
                    is_cache = false
                },
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                "",
                "",
                new string[] { }
            });

            result.CacheInfo.Is(x => x.IsCache == false);
        }

        [TestMethod]
        public void ApiEntryToMethod_XgetInternal_All指定_APIキャッシュ有()
        {
            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            var containerMock = GetPerRequestDataContainerMock();
            containerMock.SetupProperty(x => x.XgetInternalAllField, true);

            var dynamicApiRepository = ApiEntryToMethod_GetIDynamicApiRepository();
            var result = (IMethod)dynamicApiRepository.GetType().InvokeMember("ApiEntryToMethod", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, dynamicApiRepository, new object[] {
                new AllApiEntity()
                {
                    all_repository_model_list = new List<AllApiRepositoryModel>
                    {
                        new AllApiRepositoryModel
                        {
                            repository_type_cd = "ddb",
                            physical_repository_list = new List<AllApiPhysicalRepositoryModel>(){ new AllApiPhysicalRepositoryModel() { repository_connection_string="hoge",is_full = false } }
                        }
                    },
                    action_type_cd = "quy",
                    is_cache = true
                },
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                "",
                "",
                new string[] { }
            });

            result.CacheInfo.Is(x => x.IsCache == true);
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_戻り値あり()
        {
            var mockPerRequest = GetPerRequestDataContainerMock();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(mockPerRequest.Object);

            #region MockのReturns設定
            var rgId = Guid.NewGuid();
            var apiId = Guid.NewGuid();
            List<AllApiEntity> returnValue = new List<AllApiEntity>()
            {
                new AllApiEntity()
                {
                    vendor_id = Guid.NewGuid(),
                    system_id = Guid.NewGuid(),
                    controller_id = Guid.NewGuid(),
                    controller_description = "hoge_controller_description",
                    controller_relative_url = "hoge/controller_relative_url",
                    is_vendor = true,
                    is_enable_controller = true,

                    api_id = apiId,
                    api_description = "hoge_api_description",
                    method_name = "hoge_method_name",
                    method_type = HttpMethodType.MethodTypeEnum.POST.ToString().ToLower(),
                    is_admin_authentication = true,
                    is_header_authentication = true,
                    is_vendor_system_authentication_allow_null = false,
                    is_openid_authentication = true,
                    post_data_type = "hoge_post_data_type",
                    query = "hoge_query",
                    is_enable_api = true,
                    gateway_url = "hoge_gateway_url",
                    gateway_credential_username = "hoge_gateway_credential_username",
                    gateway_credential_password = "hoge_gateway_credential_password",
                    gateway_relay_header = "hoge_gateway_relay_header",
                    is_over_partition = true,
                    repository_group_id = Guid.NewGuid(),

                    script = "hoge_script",
                    action_type_cd = "quy",
                    script_type_cd = "rss",
                    actiontype_version = 1,
                    repository_type_cd = RepositoryType.SQLServer2.ToCode(),
                    is_hidden = true,
                    is_cache = true,
                    cache_minute = 2,
                    cache_key = "hoge_cache_key",
                    is_accesskey = true,
                    is_automatic_id = true,
                    partition_key = "hoge_partition_key",

                    request_schema_id = Guid.NewGuid(),
                    request_schema = "hoge_request_schema",
                    request_schema_name = "hoge_request_schema_name",
                    request_vendor_id = Guid.NewGuid(),
                    request_schema_description = "hoge_request_schema_description",

                    response_schema_id = Guid.NewGuid(),
                    response_schema = "hoge_response_schema",
                    response_schema_name = "hoge_response_schema_name",
                    response_vendor_id = Guid.NewGuid(),
                    response_schema_description = "hoge_respons_schema_description",

                    url_schema_id = Guid.NewGuid(),
                    url_schema = "hoge_url_schema",
                    url_schema_name = "hoge_url_schema_name",
                    url_vendor_id = Guid.NewGuid(),
                    url_schema_description = "hoge_url_schema_description",

                    alias_method_name = "hoge_alias_method_name",
                    is_nomatch_querystring = true,
                    ActionInjector = null,

                    controller_schema_id = Guid.NewGuid(),
                    controller_schema = "hoge_controller_schema",

                    controller_repository_key = "hoge_controller_repository_key",

                    category_id = Guid.NewGuid(),
                    category_name = "hoge_category_name",
                    is_optimistic_concurrency = false,
                    is_enable_blockchain = false,
                    is_container_dynamic_separation = true,
                    is_otherresource_sqlaccess = true,
                    is_enable_resource_version = true,
                    is_require_consent = true,
                    terms_group_code = "terms_group_code",
                    resource_group_id = Guid.NewGuid().ToString()
                }
            };

            #endregion

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            )).Returns(returnValue);

            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(returnValue);

            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mock.Object);

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri("hoge/controller_relative_url/hoge_method_name");
            GetQuery getQuery = new GetQuery("hoge_query");

            ICache cache = new InMemoryCache();
            cache.Clear();

            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = testClass.FindApi(httpMethodType, requestRelativeUri, getQuery);

            cache.Clear();

            // 検証
            #region 検証用データ作成

            IMethod verifyResult = new Method();
            verifyResult.VendorId = new VendorId(returnValue[0].vendor_id.ToString());
            verifyResult.SystemId = new SystemId(returnValue[0].system_id.ToString());
            verifyResult.ApiId = new ApiId(returnValue[0].api_id.ToString());
            verifyResult.ControllerId = new ControllerId(returnValue[0].controller_id.ToString());
            verifyResult.ControllerRelativeUrl = new ControllerUrl(returnValue[0].controller_relative_url);
            verifyResult.MethodType = httpMethodType;
            verifyResult.RequestSchema = new DataSchema(returnValue[0].request_schema);
            verifyResult.ControllerSchema = new DataSchema(returnValue[0].controller_schema);
            verifyResult.UriSchema = new DataSchema(returnValue[0].url_schema);
            verifyResult.ResponseSchema = new DataSchema(returnValue[0].response_schema);
            verifyResult.IsHeaderAuthentication = new IsHeaderAuthentication(returnValue[0].is_header_authentication);
            verifyResult.IsVendorSystemAuthenticationAllowNull = new IsVendorSystemAuthenticationAllowNull(returnValue[0].is_vendor_system_authentication_allow_null);
            verifyResult.IsOpenIdAuthentication = new IsOpenIdAuthentication(returnValue[0].is_openid_authentication);
            verifyResult.IsAdminAuthentication = new IsAdminAuthentication(returnValue[0].is_admin_authentication);
            verifyResult.RepositoryKey = new RepositoryKey(returnValue[0].controller_repository_key);
            verifyResult.ControllerRepositoryKey = new RepositoryKey(returnValue[0].controller_repository_key);
            verifyResult.KeyValue = new UrlParameter(new Dictionary<string, string>().Select(x => new { Key = new UrlParameterKey(x.Key), Value = new UrlParameterValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value));
            verifyResult.Query = null;
            verifyResult.PostDataType = new PostDataType(returnValue[0].post_data_type);
            verifyResult.RelativeUri = new RelativeUri(requestRelativeUri.Value);
            verifyResult.ApiUri = new ApiUri(returnValue[0].method_name);
            verifyResult.ApiQuery = new ApiQuery(returnValue[0].query);
            verifyResult.IsVendor = new IsVendor(returnValue[0].is_vendor);
            verifyResult.GatewayInfo = new GatewayInfo(returnValue[0].gateway_url, returnValue[0].gateway_credential_username, returnValue[0].gateway_credential_password, returnValue[0].gateway_relay_header);
            verifyResult.IsOverPartition = new IsOverPartition(returnValue[0].is_over_partition);
            verifyResult.Script = new Script(returnValue[0].script);
            verifyResult.ScriptType = returnValue[0].script_type_cd.ToScriptTypeVO();
            verifyResult.ActionType = returnValue[0].action_type_cd.ToActionTypeVO();
            verifyResult.CacheInfo = new CacheInfo(returnValue[0].is_cache, returnValue[0].cache_minute, returnValue[0].cache_key);
            verifyResult.IsAccesskey = new IsAccesskey(returnValue[0].is_accesskey);
            verifyResult.IsAutomaticId = new IsAutomaticId(returnValue[0].is_automatic_id);
            verifyResult.ActionTypeVersion = new ActionTypeVersion(returnValue[0].actiontype_version);
            verifyResult.ActionInjectorHandler = null;
            verifyResult.PartitionKey = new PartitionKey(returnValue[0].partition_key);
            verifyResult.IsPerson = new IsPerson(false);
            verifyResult.IsEnableAttachFile = new IsEnableAttachFile(returnValue[0].is_enable_attachfile);
            verifyResult.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>());
            verifyResult.InternalOnly = new InternalOnly(returnValue[0].is_internal_call_only, returnValue[0].internal_call_keyword);
            verifyResult.IsSkipJsonSchemaValidation = new IsSkipJsonSchemaValidation(returnValue[0].is_skip_jsonschema_validation);
            verifyResult.IsOpenidAuthenticationAllowNull = new IsOpenidAuthenticationAllowNull(returnValue[0].is_openid_authentication_allow_null);
            verifyResult.IsOptimisticConcurrency = new IsOptimisticConcurrency(returnValue[0].is_optimistic_concurrency);
            verifyResult.IsEnableBlockchain = new IsEnableBlockchain(false);
            verifyResult.IsUseBlobCache = new IsUseBlobCache(false);
            verifyResult.IsDocumentHistory = new IsDocumentHistory(false);
            verifyResult.IsVisibleAgreement = new IsVisibleAgreement(false);
            verifyResult.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            verifyResult.IsTransparentApi = new IsTransparentApi(false);
            verifyResult.IsClientCertAuthentication = new IsClientCertAuthentication(false);
            verifyResult.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(true);
            verifyResult.IsEnableResourceVersion = new IsEnableResourceVersion(true);
            verifyResult.IsRequireConsent = new IsRequireConsent(true);
            verifyResult.TermsGroupCode = new TermsGroupCode("terms_group_code");
            var dtu = mockPerRequest.Object.GetDateTimeUtil();
            verifyResult.PublicDate = new PublicDate(dtu.ParseDateTimeNull(returnValue[0].public_start_datetime), dtu.ParseDateTimeNull(returnValue[0].public_end_datetime));
            verifyResult.ResourceGroupId = new ResourceGroupId(returnValue[0].resource_group_id);
            #endregion

            result.IsStructuralEqual(verifyResult);
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_戻り値NULL()
        {
            var mockPerRequest = GetPerRequestDataContainerMock();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(mockPerRequest.Object);

            List<AllApiEntity> returnValue = new List<AllApiEntity>();

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AllApiEntity>
                            (
                                It.IsAny<string>()
                            )).Returns(returnValue);

            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mock.Object);

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri("hoge_Url");
            GetQuery getQuery = new GetQuery("hoge_query");

            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = testClass.FindApi(httpMethodType, requestRelativeUri, getQuery);

            // 検証
            result.IsStructuralEqual(null);
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_正常系_Regist()
        {
            TestInitialize();
            var mock = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);
            UnityContainer.RegisterType<IMethod, Method>();

            var requestControllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = Guid.NewGuid();
            var requestApiId = new ApiId(apiId.ToString());
            var requestDataId = new DataId(Guid.NewGuid().ToString());
            var requestField = Guid.NewGuid().ToString();
            var requestValue = Guid.NewGuid().ToString();
            var requestContents = new Contents(new JObject
            {
                [requestField] = requestValue
            }.ToString());
            var requestQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            var requestKeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>());

            var mockResult = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            mockResult[0].api_id = apiId;
            mockResult[0].action_type_cd = "reg";

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns(mockResult);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            var expect = GetVerifyResult(
                mockResult[0],
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                new RequestRelativeUri(""),
                mockResult[0].vendor_id.ToString(),
                mockResult[0].system_id.ToString(),
                dataContainerMock.Object.GetDateTimeUtil()
            );
            expect.ActionType = new ActionTypeVO(ActionType.Query);
            expect.KeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                [new UrlParameterKey("id")] = new UrlParameterValue(requestDataId.Value)
            });
            expect.Script = new Script("");
            expect.ScriptType = null;
            expect.ControllerSchema = new DataSchema(@"
{
  'properties': {
    id: {
      'type': 'string',
    },
  },
  'type': 'object'
}
");
            expect.IsOptimisticConcurrency = new IsOptimisticConcurrency(false);
            expect.IsEnableBlockchain = new IsEnableBlockchain(false);
            expect.IsUseBlobCache = new IsUseBlobCache(false);
            expect.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            expect.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(true);
            expect.IsEnableResourceVersion = new IsEnableResourceVersion(true);
            expect.IsRequireConsent = new IsRequireConsent(true);
            expect.TermsGroupCode = new TermsGroupCode("terms_group_code");
            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindApiForGetExecuteApiInfo(
                requestControllerId,
                requestApiId,
                requestDataId,
                requestContents,
                requestQueryString,
                requestKeyValue
            );

            result.IsStructuralEqual(expect);
            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_正常系_Regist_データがPartitionKeyに含まれる()
        {
            TestInitialize();
            var mock = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);
            UnityContainer.RegisterType<IMethod, Method>();

            var requestControllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = Guid.NewGuid();
            var requestApiId = new ApiId(apiId.ToString());
            var requestDataId = new DataId(Guid.NewGuid().ToString());
            var requestField = Guid.NewGuid().ToString();
            var requestValue = Guid.NewGuid().ToString();
            var requestContents = new Contents(new JObject
            {
                [requestField] = requestValue
            }.ToString());
            var requestQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            var requestKeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>());

            var mockResult = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            mockResult[0].api_id = apiId;
            mockResult[0].action_type_cd = "reg";
            mockResult[0].partition_key = mockResult[0].partition_key + $"/{{{requestField}}}";

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns(mockResult);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            var expect = GetVerifyResult(
                mockResult[0],
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                new RequestRelativeUri(""),
                mockResult[0].vendor_id.ToString(),
                mockResult[0].system_id.ToString(),
                dataContainerMock.Object.GetDateTimeUtil()
            );
            expect.ActionType = new ActionTypeVO(ActionType.Query);
            expect.KeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>()
            {
                [new UrlParameterKey(requestField)] = new UrlParameterValue(requestValue),
                [new UrlParameterKey("id")] = new UrlParameterValue(requestDataId.Value)
            });
            expect.Script = new Script("");
            expect.ScriptType = null;
            expect.ControllerSchema = new DataSchema(@"
{
  'properties': {
    id: {
      'type': 'string',
    },
  },
  'type': 'object'
}
");
            expect.IsOptimisticConcurrency = new IsOptimisticConcurrency(false);
            expect.IsEnableBlockchain = new IsEnableBlockchain(false);
            expect.IsUseBlobCache = new IsUseBlobCache(false);
            expect.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            expect.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(true);
            expect.IsEnableResourceVersion = new IsEnableResourceVersion(true);
            expect.IsRequireConsent = new IsRequireConsent(true);
            expect.TermsGroupCode = new TermsGroupCode("terms_group_code");

            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindApiForGetExecuteApiInfo(
                requestControllerId,
                requestApiId,
                requestDataId,
                requestContents,
                requestQueryString,
                requestKeyValue
            );

            result.IsStructuralEqual(expect);
            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_正常系_Update_Delete()
        {
            TestInitialize();
            var mock = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);
            UnityContainer.RegisterType<IMethod, Method>();

            var requestControllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = Guid.NewGuid();
            var requestApiId = new ApiId(apiId.ToString());
            var requestDataId = new DataId(Guid.NewGuid().ToString());
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            var requestKeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>());

            var mockResult = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            mockResult[0].api_id = apiId;
            mockResult[0].action_type_cd = "upd";

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns(mockResult);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            var expect = GetVerifyResult(
                mockResult[0],
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                new RequestRelativeUri(""),
                mockResult[0].vendor_id.ToString(),
                mockResult[0].system_id.ToString(),
                dataContainerMock.Object.GetDateTimeUtil()
            );
            expect.ActionType = new ActionTypeVO(ActionType.Query);
            expect.Query = requestQueryString;
            expect.KeyValue = requestKeyValue;
            expect.Script = new Script("");
            expect.ScriptType = null;
            expect.IsOptimisticConcurrency = new IsOptimisticConcurrency(false);
            expect.IsEnableBlockchain = new IsEnableBlockchain(false);
            expect.IsUseBlobCache = new IsUseBlobCache(false);
            expect.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            expect.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(true);
            expect.IsEnableResourceVersion = new IsEnableResourceVersion(true);
            expect.IsRequireConsent = new IsRequireConsent(true);
            expect.TermsGroupCode = new TermsGroupCode("terms_group_code");

            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindApiForGetExecuteApiInfo(
                requestControllerId,
                requestApiId,
                requestDataId,
                requestContents,
                requestQueryString,
                requestKeyValue
            );

            result.IsStructuralEqual(expect);
            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_エラー_一致無し()
        {
            TestInitialize();
            UnityContainer.RegisterType<IMethod, Method>();
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", new Mock<IJPDataHubDbConnection>().Object);

            var requestControllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = Guid.NewGuid();
            var requestApiId = new ApiId(apiId.ToString());
            var requestDataId = new DataId(Guid.NewGuid().ToString());
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            var requestKeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>());

            var mockResult = new List<AllApiEntity>();

            var dataContainerMock = new Mock<IPerRequestDataContainer>();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns(mockResult);

            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);

            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindApiForGetExecuteApiInfo(
                requestControllerId,
                requestApiId,
                requestDataId,
                requestContents,
                requestQueryString,
                requestKeyValue
            );
            result.IsSameReferenceAs(null);

            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_Exception_FindApiForGetExecuteApiInfoでException()
        {
            TestInitialize();
            UnityContainer.RegisterType<IMethod, Method>();
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", new Mock<IJPDataHubDbConnection>().Object);

            var requestControllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = Guid.NewGuid();
            var requestApiId = new ApiId(apiId.ToString());
            var requestDataId = new DataId(Guid.NewGuid().ToString());
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestQueryString = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            var requestKeyValue = new UrlParameter(new Dictionary<UrlParameterKey, UrlParameterValue>());

            var expectException = new Exception();

            var dataContainerMock = new Mock<IPerRequestDataContainer>();
            UnityContainer.RegisterInstance(dataContainerMock.Object);
            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Throws(expectException);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);
            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            AssertEx.Catch<Exception>(() => target.FindApiForGetExecuteApiInfo(
                requestControllerId,
                requestApiId,
                requestDataId,
                requestContents,
                requestQueryString,
                requestKeyValue
            )).IsSameReferenceAs(expectException);

            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindEnumerableApi_正常系()
        {
            TestInitialize();
            UnityContainer.RegisterType<IMethod, Method>();

            var requestMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var requestQuery = new GetQuery(Guid.NewGuid().ToString());

            var mockResult = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            mockResult[0].action_type_cd = "quy";
            mockResult[0].method_type = "GET";

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var requestRelativeUri = new RequestRelativeUri($"{mockResult[0].controller_relative_url}/{mockResult[0].method_name}");

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            )).Returns(mockResult);

            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(mockResult);

            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, time, action) =>
                {
                    return (List<AllApiEntity>)action();
                });
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<ActionObject>()))
                .Returns<string, ActionObject>((key, action) =>
                {
                    return (List<AllApiEntity>)action();
                });
            mockICache.Setup(x => x.Get<ApiTreeNode>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, time, action) =>
                {
                    return (ApiTreeNode)action();
                });
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            var expect = GetVerifyResult(
                mockResult[0],
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                requestRelativeUri,
                mockResult[0].vendor_id.ToString(),
                mockResult[0].system_id.ToString(),
                dataContainerMock.Object.GetDateTimeUtil()
            );
            expect.ActionType = new ActionTypeVO(ActionType.EnumerableQuery);
            expect.IsOptimisticConcurrency = new IsOptimisticConcurrency(false);
            expect.IsEnableBlockchain = new IsEnableBlockchain(false);
            expect.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>());
            expect.IsUseBlobCache = new IsUseBlobCache(false);
            expect.IsContainerDynamicSeparation = new IsContainerDynamicSeparation(true);
            expect.IsOtherResourceSqlAccess = new IsOtherResourceSqlAccess(true);
            expect.IsEnableResourceVersion = new IsEnableResourceVersion(true);
            expect.IsRequireConsent = new IsRequireConsent(true);
            expect.TermsGroupCode = new TermsGroupCode("terms_group_code");

            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindEnumerableApi(
                requestMethodType,
                requestRelativeUri,
                requestQuery
            );

            result.IsStructuralEqual(expect);
            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindEnumerableApi_エラー_一致無し()
        {
            TestInitialize();
            UnityContainer.RegisterType<IMethod, Method>();

            var requestMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var requestQuery = new GetQuery(Guid.NewGuid().ToString());

            var mockResult = new List<AllApiEntity>
            {
            };

            var dataContainerMock = new Mock<IPerRequestDataContainer>();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            )).Returns(mockResult);

            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(mockResult);

            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, time, action) =>
                {
                    return (List<AllApiEntity>)action();
                });
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<ActionObject>()))
                .Returns<string, ActionObject>((key, action) =>
                {
                    return (List<AllApiEntity>)action();
                });
            mockICache.Setup(x => x.Get<ApiTreeNode>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, time, action) =>
                {
                    return (ApiTreeNode)action();
                });
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindEnumerableApi(
                requestMethodType,
                requestRelativeUri,
                requestQuery
            );

            result.IsSameReferenceAs(null);
            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(0));
        }

        [TestMethod]
        public void DynamicApiRepository_FindEnumerableApi_エラー_ActionTypeの一致無し()
        {
            TestInitialize();
            UnityContainer.RegisterType<IMethod, Method>();

            var requestMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            var requestQuery = new GetQuery(Guid.NewGuid().ToString());

            var mockResult = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            mockResult[0].action_type_cd = "upd";
            mockResult[0].method_type = "GET";

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var requestRelativeUri = new RequestRelativeUri($"{mockResult[0].controller_relative_url}/{mockResult[0].method_name}");

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            )).Returns(mockResult);

            mock.Setup(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(mockResult);

            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, time, action) =>
                {
                    return (List<AllApiEntity>)action();
                });
            mockICache.Setup(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<ActionObject>()))
                .Returns<string, ActionObject>((key, action) =>
                {
                    return (List<AllApiEntity>)action();
                });
            mockICache.Setup(x => x.Get<ApiTreeNode>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, time, action) =>
                {
                    return (ApiTreeNode)action();
                });
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            var target = UnityContainer.Resolve<IDynamicApiRepository>();
            var result = target.FindEnumerableApi(
                requestMethodType,
                requestRelativeUri,
                requestQuery
            );

            result.IsSameReferenceAs(null);
            mockICache.Verify(x => x.Get<List<AllApiEntity>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()), Times.Exactly(1));
        }

        private IDynamicApiRepository ApiEntryToMethod_GetIDynamicApiRepository()
        {
            var mock = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);
            var mockICache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<IMethod>(new Method());

            return UnityContainer.Resolve<IDynamicApiRepository>();
        }


        private IDynamicApiRepository GetContorollerIpFilter_GetIDynamicApiRepository()
        {
            var mock = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);
            var mockICache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>("DynamicApi", mockICache.Object);

            UnityContainer.RegisterInstance<IMethod>(new Method());

            return UnityContainer.Resolve<IDynamicApiRepository>();
        }

        private Mock<IPerRequestDataContainer> GetPerRequestDataContainerMock()
        {
            //perRequestDataContainerをモックを使って上書き
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, "aaaaaa");
            perRequestDataContainer.SetupProperty(x => x.VendorId, "123456789");
            perRequestDataContainer.Setup(x => x.GetDateTimeUtil()).Returns(new DateTimeUtil("yyyy/MM/dd", "yyyy/MM/dd hh:mm:ss tt", "yyyy/M/d"));
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);
            return perRequestDataContainer;
        }

        private AllApiEntity GetAllApiEntity()
        {
            return new AllApiEntity()
            {
                vendor_id = Guid.NewGuid(),
                system_id = Guid.NewGuid(),
                controller_id = Guid.NewGuid(),
                controller_description = "hoge_controller_description",
                controller_relative_url = $"API/{Guid.NewGuid().ToString()}",
                is_vendor = true,
                is_enable_controller = true,

                api_id = Guid.NewGuid(),
                api_description = "hoge_api_description",
                method_name = Guid.NewGuid().ToString(),
                method_type = HttpMethodType.MethodTypeEnum.POST.ToString().ToLower(),
                is_admin_authentication = true,
                is_header_authentication = true,
                is_openid_authentication = true,
                post_data_type = "hoge_post_data_type",
                query = "hoge_query",
                is_enable_api = true,
                gateway_url = "hoge_gateway_url",
                gateway_credential_username = "hoge_gateway_credential_username",
                gateway_credential_password = "hoge_gateway_credential_password",
                gateway_relay_header = "hoge_gateway_relay_header",
                is_over_partition = true,
                repository_group_id = Guid.NewGuid(),

                script = "hoge_script",
                action_type_cd = "quy",
                script_type_cd = "rss",
                actiontype_version = 1,
                repository_type_cd = RepositoryType.SQLServer2.ToCode(),
                all_repository_model_list = new List<AllApiRepositoryModel>
                {
                    new AllApiRepositoryModel()
                    {
                        repository_type_cd = RepositoryType.SQLServer2.ToCode(),
                        physical_repository_list = new List<AllApiPhysicalRepositoryModel>() {
                            new AllApiPhysicalRepositoryModel() {
                                repository_connection_string = "Server=tcp:,1433;Initial Catalog=;Persist Security Info=False;User ID=;Password=;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", is_full = false } }
                    }
                },
                is_hidden = true,
                is_cache = true,
                cache_minute = 2,
                cache_key = "hoge_cache_key",
                is_accesskey = true,
                is_automatic_id = true,
                partition_key = "hoge_partition_key",

                request_schema_id = Guid.NewGuid(),
                request_schema = "hoge_request_schema",
                request_schema_name = "hoge_request_schema_name",
                request_vendor_id = Guid.NewGuid(),
                request_schema_description = "hoge_request_schema_description",

                response_schema_id = Guid.NewGuid(),
                response_schema = "hoge_response_schema",
                response_schema_name = "hoge_response_schema_name",
                response_vendor_id = Guid.NewGuid(),
                response_schema_description = "hoge_response_schema_description",

                url_schema_id = Guid.NewGuid(),
                url_schema = "hoge_url_schema",
                url_schema_name = "hoge_url_schema_name",
                url_vendor_id = Guid.NewGuid(),
                url_schema_description = "hoge_url_schema_description",

                alias_method_name = "hoge_alias_method_name",
                is_nomatch_querystring = true,
                ActionInjector = null,

                controller_schema_id = Guid.NewGuid(),
                controller_schema = "hoge_controller_schema",

                controller_repository_key = "hoge_controller_repository_key",

                category_id = Guid.NewGuid(),
                category_name = "hoge_category_name",
                is_enable_attachfile = false,
                is_enable_blockchain = false,
                is_container_dynamic_separation = true,
                is_otherresource_sqlaccess = true,
                is_enable_resource_version = true,
                is_require_consent = true,
                terms_group_code = "terms_group_code",
                resource_group_id = Guid.NewGuid().ToString()
            };
        }

        private IMethod GetVerifyResult(
            AllApiEntity entity,
            HttpMethodType httpMethodType,
            RequestRelativeUri requestRelativeUri,
            string vendorId,
            string systemId,
            DateTimeUtil dtu,
            bool? isAdminAuthentification = null,
            bool? isCache = null,
            bool? isAccessKeyValue = null,
            PostDataType postDataType = null,
            string methodName = null,
            string getQuery = null,
            ActionTypeVO actionType = null
        )
        {
            IMethod verifyResult = new Method();
            verifyResult.VendorId = new VendorId(entity.vendor_id.ToString());
            verifyResult.SystemId = new SystemId(entity.system_id.ToString());
            verifyResult.ApiId = new ApiId(entity.api_id.ToString());
            verifyResult.ControllerId = new ControllerId(entity.controller_id.ToString());
            verifyResult.ControllerRelativeUrl = new ControllerUrl(entity.controller_relative_url);
            verifyResult.MethodType = httpMethodType;
            verifyResult.RequestSchema = new DataSchema(entity.request_schema);
            verifyResult.ControllerSchema = new DataSchema(entity.controller_schema);
            verifyResult.UriSchema = new DataSchema(entity.url_schema);
            verifyResult.ResponseSchema = new DataSchema(entity.response_schema);
            verifyResult.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(
                new List<RepositoryInfo> { new RepositoryInfo(
                    entity.repository_type_cd, new Dictionary<string, bool>(){ { "Server=tcp:,1433;Initial Catalog=;Persist Security Info=False;User ID=;Password=;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;", false } }) });
            verifyResult.IsHeaderAuthentication = new IsHeaderAuthentication(entity.is_header_authentication);
            verifyResult.IsVendorSystemAuthenticationAllowNull = new IsVendorSystemAuthenticationAllowNull(entity.is_vendor_system_authentication_allow_null);
            verifyResult.IsOpenIdAuthentication = new IsOpenIdAuthentication(entity.is_openid_authentication);
            verifyResult.IsAdminAuthentication = new IsAdminAuthentication(isAdminAuthentification ?? entity.is_admin_authentication);
            verifyResult.RepositoryKey = new RepositoryKey(entity.controller_repository_key);
            verifyResult.ControllerRepositoryKey = new RepositoryKey(entity.controller_repository_key);
            verifyResult.KeyValue = new UrlParameter(new Dictionary<string, string>().Select(x => new { Key = new UrlParameterKey(x.Key), Value = new UrlParameterValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value));
            verifyResult.Query = null;
            verifyResult.PostDataType = postDataType ?? new PostDataType(entity.post_data_type);
            verifyResult.RelativeUri = new RelativeUri(requestRelativeUri.Value);
            verifyResult.ApiUri = new ApiUri(methodName ?? entity.method_name);
            verifyResult.ApiQuery = new ApiQuery(getQuery ?? entity.query);
            verifyResult.IsVendor = new IsVendor(entity.is_vendor);
            verifyResult.GatewayInfo = new GatewayInfo(entity.gateway_url, entity.gateway_credential_username, entity.gateway_credential_password, entity.gateway_relay_header);
            verifyResult.IsOverPartition = new IsOverPartition(entity.is_over_partition);
            verifyResult.Script = new Script(entity.script);
            verifyResult.ScriptType = entity.script_type_cd.ToScriptTypeVO();
            verifyResult.ActionType = actionType ?? entity.action_type_cd.ToActionTypeVO();
            verifyResult.CacheInfo = new CacheInfo(isCache ?? entity.is_cache, entity.cache_minute, entity.cache_key);
            verifyResult.IsAccesskey = new IsAccesskey(isAccessKeyValue ?? entity.is_accesskey);
            verifyResult.IsAutomaticId = new IsAutomaticId(entity.is_automatic_id);
            verifyResult.ActionTypeVersion = new ActionTypeVersion(entity.actiontype_version);
            verifyResult.ActionInjectorHandler = null;
            verifyResult.PartitionKey = new PartitionKey(entity.partition_key);
            verifyResult.IsPerson = new IsPerson(false);
            verifyResult.IsEnableAttachFile = new IsEnableAttachFile(entity.is_enable_attachfile);
            verifyResult.InternalOnly = new InternalOnly(entity.is_internal_call_only, entity.internal_call_keyword);
            verifyResult.IsSkipJsonSchemaValidation = new IsSkipJsonSchemaValidation(entity.is_skip_jsonschema_validation);
            verifyResult.IsOpenidAuthenticationAllowNull = new IsOpenidAuthenticationAllowNull(entity.is_openid_authentication_allow_null);
            verifyResult.PublicDate = new PublicDate(dtu.ParseDateTimeNull(entity.public_start_datetime), dtu.ParseDateTimeNull(entity.public_end_datetime));
            verifyResult.IsDocumentHistory = new IsDocumentHistory(false);
            verifyResult.IsVisibleAgreement = new IsVisibleAgreement(false);
            verifyResult.IsTransparentApi = new IsTransparentApi(false);
            verifyResult.IsClientCertAuthentication = new IsClientCertAuthentication(false);
            verifyResult.IsEnableResourceVersion = new IsEnableResourceVersion(true);
            verifyResult.ResourceGroupId = new ResourceGroupId(entity.resource_group_id);
            return verifyResult;
        }
    }
}