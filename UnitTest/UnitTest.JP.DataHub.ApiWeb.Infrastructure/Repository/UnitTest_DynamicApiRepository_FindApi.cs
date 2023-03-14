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
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_DynamicApiRepository_FindApi : UnitTestBase
    {
        private string _defaultConnectionString = "Server=tcp:,1433;Initial Catalog=;Persist Security Info=False;User ID=;Password=;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            // モックの作成
            var dataContainerMock = new Mock<IPerRequestDataContainer>();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            UnityContainer.RegisterType<IMethod, Method>();

            var cachingProvider = new DefaultInMemoryCachingProvider("DynamicApi", new[] { new InMemoryCaching("DynamicApi", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>();
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
        public void DynamicApiRepository_FindApi_正常系_通常()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(returnValue[0]) };
            var mock = RegisterDbConnectionMock(returnValue, returnValue2);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"{returnValue[0].controller_relative_url}/{returnValue[0].method_name}");

            var expect = GetVerifyResult(
                returnValue[0],
                httpMethodType,
                requestRelativeUri,
                dataContainerMock.Object.VendorId,
                dataContainerMock.Object.SystemId,
                dataContainerMock.Object.GetDateTimeUtil(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                returnValue2.FirstOrDefault()
            );

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                new List<Guid> {Guid.NewGuid()}
            });
            cache.Clear();

            result.IsStructuralEqual(expect);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(2));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_透過API()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(returnValue[0]) };

            var mock = RegisterDbConnectionMock(returnValue, returnValue2);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"{returnValue[0].controller_relative_url}/GetVersionInfo");

            var expect = GetVerifyResult(
                returnValue[0],
                httpMethodType,
                requestRelativeUri,
                dataContainerMock.Object.VendorId,
                dataContainerMock.Object.SystemId,
                dataContainerMock.Object.GetDateTimeUtil(),
                false,
                false,
                false,
                new PostDataType(""),
                "GetVersionInfo",
                null,
                null,
                returnValue2.FirstOrDefault()
            ); ;

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                null
            });
            cache.Clear();

            // 透過APIのApiIdは都度変化する
            expect.ApiId = result.ApiId;
            expect.ActionInjectorHandler = result.ActionInjectorHandler;

            result.IsStructuralEqual(expect);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(2));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_透過API_上書き()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var apiEntity = GetAllApiEntity();
            var getVersionInfoEntity = GetAllApiEntity();
            getVersionInfoEntity.controller_id = apiEntity.controller_id;
            getVersionInfoEntity.controller_relative_url = apiEntity.controller_relative_url;
            getVersionInfoEntity.is_transparent_api = true;
            getVersionInfoEntity.method_type = "GET";
            getVersionInfoEntity.method_name = "GetVersionInfo";

            var returnValue = new List<AllApiEntity>
            {
                apiEntity,
                getVersionInfoEntity
            };
            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(returnValue[0]) };
            var mock = RegisterDbConnectionMock(returnValue, returnValue2);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"{returnValue[0].controller_relative_url}/GetVersionInfo");

            var expect = GetVerifyResult(
                returnValue[1],
                httpMethodType,
                requestRelativeUri,
                dataContainerMock.Object.VendorId,
                dataContainerMock.Object.SystemId,
                dataContainerMock.Object.GetDateTimeUtil(),
                null,
                false,
                null,
                new PostDataType(""),
                "GetVersionInfo",
                null,
                null,
                returnValue2.FirstOrDefault()
            );

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                null
            });
            cache.Clear();

            // 透過APIのApiIdはgetVersionInfoEntityを引き継ぎ
            expect.ApiId = new ApiId(getVersionInfoEntity.api_id.ToString());
            expect.ActionInjectorHandler = result.ActionInjectorHandler;
            expect.IsTransparentApi = new IsTransparentApi(true);

            result.IsStructuralEqual(expect);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(2));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_透過API_RagisterRawData()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(returnValue[0]) };

            var mock = RegisterDbConnectionMock(returnValue, returnValue2);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"{returnValue[0].controller_relative_url}/RegisterRawData");

            var expect = GetVerifyResult(
                returnValue[0],
                httpMethodType,
                requestRelativeUri,
                dataContainerMock.Object.VendorId,
                dataContainerMock.Object.SystemId,
                dataContainerMock.Object.GetDateTimeUtil(),
                true,
                false,
                false,
                new PostDataType(""),
                "RegisterRawData",
                null,
                "rrd".ToActionTypeVO(),
                returnValue2.FirstOrDefault()
            );
            expect.IsOpenIdAuthentication = new IsOpenIdAuthentication(false);
            // RegisterRawDataはScriptがnullになる
            expect.Script = new Script(null);

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                null
            });
            cache.Clear();

            // 透過APIのApiIdは都度変化する
            expect.ApiId = result.ApiId;
            expect.ActionInjectorHandler = result.ActionInjectorHandler;

            result.IsStructuralEqual(expect);
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_コントローラ()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };

            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(returnValue[0]) };

            var mock = RegisterDbConnectionMock(returnValue, returnValue2);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"{returnValue[0].controller_relative_url}");

            var expect = GetVerifyResult(
                returnValue[0],
                httpMethodType,
                requestRelativeUri,
                dataContainerMock.Object.VendorId,
                dataContainerMock.Object.SystemId,
                dataContainerMock.Object.GetDateTimeUtil(),
                false,
                false,
                false,
                new PostDataType("array"),
                "OData",
                null,
                new ActionTypeVO(ActionType.OData),
                returnValue2.FirstOrDefault()
            );

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                null
            });
            cache.Clear();

            // 透過APIのApiIdは都度変化する
            expect.ApiId = result.ApiId;
            expect.ActionInjectorHandler = result.ActionInjectorHandler;

            result.IsStructuralEqual(expect);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(2));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_一致なし_データ無し()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
            };

            var mock = RegisterDbConnectionMock(returnValue);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                null
            });
            cache.Clear();

            result.IsSameReferenceAs(null);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_正常系_一致なし_コントローラ違い()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };

            var mock = RegisterDbConnectionMock(returnValue);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"/API/{Guid.NewGuid().ToString()}/{returnValue[0].method_name}");

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                null
            });
            cache.Clear();

            result.IsSameReferenceAs(null);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApi_一致無し_除外()
        {
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };

            var mock = RegisterDbConnectionMock(returnValue);

            // テスト対象のインスタンスを作成
            DynamicApiRepository testClass = UnityContainer.Resolve<DynamicApiRepository>();

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri($"{returnValue[0].controller_relative_url}/{returnValue[0].method_name}");

            var cache = new InMemoryCache();
            cache.Clear();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApi", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                httpMethodType,
                requestRelativeUri,
                null,
                new List<Guid> {returnValue[0].api_id}
            });
            cache.Clear();

            result.IsSameReferenceAs(null);

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>()
            ), Times.Exactly(0));

            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_正常系()
        {
            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(returnValue[0]) };

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mock = RegisterDbConnectionMock(returnValue, returnValue2);

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri("");

            var expect = GetVerifyResult(
                returnValue[0],
                httpMethodType,
                requestRelativeUri,
                returnValue[0].vendor_id.ToString(),
                returnValue[0].system_id.ToString(),
                dataContainerMock.Object.GetDateTimeUtil(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                returnValue2.FirstOrDefault()
            );

            var cache = new InMemoryCache();
            cache.Clear();
            var testClass = UnityContainer.Resolve<DynamicApiRepository>();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApiForGetExecuteApiInfo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                expect.ControllerId,
                expect.ApiId
            });
            cache.Clear();

            result.IsStructuralEqual(expect);
            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_一致無し_ControllerId不正()
        {
            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mock = RegisterDbConnectionMock(returnValue);

            var cache = new InMemoryCache();
            cache.Clear();
            var testClass = UnityContainer.Resolve<DynamicApiRepository>();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApiForGetExecuteApiInfo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                new ControllerId(""),
                new ApiId(returnValue[0].api_id.ToString())
            });
            cache.Clear();

            result.IsSameReferenceAs(null);
            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(0));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_一致無し_ApiId不正()
        {
            var returnValue = new List<AllApiEntity>
            {
                GetAllApiEntity()
            };
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mock = RegisterDbConnectionMock(returnValue);

            var cache = new InMemoryCache();
            cache.Clear();
            var testClass = UnityContainer.Resolve<DynamicApiRepository>();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApiForGetExecuteApiInfo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                new ControllerId(returnValue[0].api_id.ToString()),
                new ApiId("")
            });
            cache.Clear();

            result.IsSameReferenceAs(null);
            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(0));
        }

        [TestMethod]
        public void DynamicApiRepository_FindApiForGetExecuteApiInfo_一致無し_Api定義なし()
        {
            var returnValue = new List<AllApiEntity>
            {
            };
            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            var mock = RegisterDbConnectionMock(returnValue);

            var cache = new InMemoryCache();
            cache.Clear();
            var testClass = UnityContainer.Resolve<DynamicApiRepository>();
            var result = (IMethod)testClass.GetType().InvokeMember("_FindApiForGetExecuteApiInfo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            {
                new ControllerId(Guid.NewGuid().ToString()),
                new ApiId(Guid.NewGuid().ToString())
            });
            cache.Clear();

            result.IsSameReferenceAs(null);
            mock.Verify(x => x.Query<AllApiEntity>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void DynamicApiRepository_ApiEntryToMethod_パラメータと一致するかチェック()
        {
            RegisterDbConnectionMock();

            var requestAllApiEntity = GetAllApiEntity();
            requestAllApiEntity.all_repository_model_list = new List<AllApiRepositoryModel>()
            {
                new AllApiRepositoryModel
                {
                    physical_repository_list = new List<AllApiPhysicalRepositoryModel>(){ new AllApiPhysicalRepositoryModel() { repository_connection_string= _defaultConnectionString, is_full = false } },
                    repository_type_cd = requestAllApiEntity.repository_type_cd
                }
            };
            var returnValue2 = new List<AllApiRepositoryIncludePhysicalRepositoryModel> { GetAllApiRepositoryIncludePhysicalRepositoryModel(requestAllApiEntity) };

            var dataContainerMock = CreatePerRequestDataContainerMock();
            UnityContainer.RegisterInstance(dataContainerMock.Object);

            HttpMethodType httpMethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            RequestRelativeUri requestRelativeUri = new RequestRelativeUri("");

            var expect = GetVerifyResult(
                requestAllApiEntity,
                httpMethodType,
                requestRelativeUri,
                requestAllApiEntity.vendor_id.ToString(),
                requestAllApiEntity.system_id.ToString(),
                dataContainerMock.Object.GetDateTimeUtil(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null
            );

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<DynamicApiRepository>();
            var result = (IMethod)testClass.GetType().InvokeMember("ApiEntryToMethod", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, testClass, new object[]
            { 
                requestAllApiEntity, 
                httpMethodType, 
                requestRelativeUri.Value, 
                "", 
                new string[] { } 
            });
            result.IsStructuralEqual(expect);
        }

        private Mock<IJPDataHubDbConnection> RegisterDbConnectionMock(IEnumerable<AllApiEntity> returnValue = null)
        {
            var mock = new Mock<IJPDataHubDbConnection>();
            if (returnValue != null)
            {
                mock.Setup(x => x.Query<AllApiEntity>
                (
                    It.IsAny<string>()
                )).Returns(returnValue);

                mock.Setup(x => x.Query<AllApiEntity>
                (
                    It.IsAny<string>(),
                    It.IsAny<object>()
                )).Returns(returnValue);
            }

            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            return mock;
        }

        private Mock<IJPDataHubDbConnection> RegisterDbConnectionMock(IEnumerable<AllApiEntity> returnValue, IEnumerable<AllApiRepositoryIncludePhysicalRepositoryModel> returnValue2)
        {
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AllApiEntity>(It.IsAny<string>())).Returns(returnValue);
            mock.Setup(x => x.Query<AllApiEntity>(It.IsAny<string>(), It.IsAny<object>())).Returns(returnValue);
            mock.Setup(x => x.Query<AllApiRepositoryIncludePhysicalRepositoryModel>(It.IsAny<string>())).Returns(returnValue2);
            mock.Setup(x => x.Query<AllApiRepositoryIncludePhysicalRepositoryModel>(It.IsAny<string>(), It.IsAny<object>())).Returns(returnValue2);
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);
            return mock;
        }


        private AllApiEntity GetAllApiEntity()
        {
            var result = new AllApiEntity()
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
                is_optimistic_concurrency = false,
                is_enable_blockchain = false,
                all_repository_model_list = new List<AllApiRepositoryModel>
                    {
                        new AllApiRepositoryModel
                        {
                            repository_type_cd = "ddb",
                            physical_repository_list = new List<AllApiPhysicalRepositoryModel>(){ new AllApiPhysicalRepositoryModel() { repository_connection_string= _defaultConnectionString, is_full = false, PhysicalRepositoryId = Guid.NewGuid() } },
                        }
                    },
                is_use_blob_cache = false,
                is_container_dynamic_separation = true,
                is_otherresource_sqlaccess = true,
                is_enable_resource_version = true,
                is_require_consent = true,
                terms_group_code = "terms_group_code",
                resource_group_id = Guid.NewGuid().ToString()
            };
            result.all_repository_model_list.ForEach(x => x.repository_group_id = result.repository_group_id);
            return result;
        }

        private AllApiRepositoryIncludePhysicalRepositoryModel GetAllApiRepositoryIncludePhysicalRepositoryModel(AllApiEntity allApiEntity)
        {
            return new AllApiRepositoryIncludePhysicalRepositoryModel()
            {
                api_id = allApiEntity.api_id,
                repository_group_id = allApiEntity.repository_group_id,
                repository_connection_string = _defaultConnectionString,
                repository_type_cd = allApiEntity.repository_type_cd,
                is_enable = true,
                is_full = false,
                is_primary = true,
                is_secondary_primary = false,
                physical_repository_id = allApiEntity.all_repository_model_list.FirstOrDefault()?.physical_repository_list.FirstOrDefault().PhysicalRepositoryId
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
            ActionTypeVO actionType = null,
            AllApiRepositoryIncludePhysicalRepositoryModel physicalRepositoryModel = null
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
            var xxx = new RepositoryInfo(physicalRepositoryModel?.repository_group_id, entity.repository_type_cd, new List<Tuple<string, bool, Guid?>>() { new Tuple<string, bool, Guid?>(_defaultConnectionString, false, physicalRepositoryModel?.physical_repository_id) });
            verifyResult.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo> { xxx });
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
            verifyResult.IsPerson = new IsPerson(false);
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
            verifyResult.IsEnableAttachFile = new IsEnableAttachFile(entity.is_enable_attachfile);
            verifyResult.IsSkipJsonSchemaValidation = new IsSkipJsonSchemaValidation(entity.is_skip_jsonschema_validation);
            verifyResult.InternalOnly = new InternalOnly(entity.is_internal_call_only, entity.internal_call_keyword);
            verifyResult.IsOpenidAuthenticationAllowNull = new IsOpenidAuthenticationAllowNull(entity.is_openid_authentication_allow_null);
            verifyResult.PublicDate = new PublicDate(dtu.ParseDateTimeNull(entity.public_start_datetime), dtu.ParseDateTimeNull(entity.public_end_datetime));
            verifyResult.IsOptimisticConcurrency = new IsOptimisticConcurrency(entity.is_optimistic_concurrency);
            verifyResult.IsEnableBlockchain = new IsEnableBlockchain(entity.is_enable_blockchain);
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
            verifyResult.ResourceGroupId = new ResourceGroupId(entity.resource_group_id);
            return verifyResult;
        }
    }
}
