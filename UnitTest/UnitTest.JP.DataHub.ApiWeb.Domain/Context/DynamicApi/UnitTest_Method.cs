using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.Api.Core.Resources;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.ApiFilter;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;
using JP.DataHub.Com.Extensions;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi
{
    [TestClass]
    public class UnitTest_Method : UnitTestBase
    {
        private PerRequestDataContainer _perRequestDataContainer;

        // テストデータ
        private Guid _vendorId = Guid.NewGuid();
        private Guid _systemId1 = Guid.NewGuid();
        private Guid _systemId2 = Guid.NewGuid();
        private string _providerVendorId = Guid.NewGuid().ToString();
        private string _approvedUserVendorId = Guid.NewGuid().ToString();
        private string _rejectedUserVendorId = Guid.NewGuid().ToString();

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            // モックの作成
            var loginResultMock = new Mock<User>(null, null, null);
            var mock = new Mock<IAuthenticationRepository>();
            mock.Setup(s => s.Login(It.IsAny<VendorId>(), It.Is<SystemId>(x => x.Value == _systemId1.ToString()), null)).Returns(loginResultMock.Object);
            UnityContainer.RegisterInstance(mock.Object);

            // PerRequestDataContainerの作成
            _perRequestDataContainer = new PerRequestDataContainer();
            _perRequestDataContainer.VendorId = _vendorId.ToString();
            _perRequestDataContainer.SystemId = _systemId1.ToString();
            _perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            _perRequestDataContainer.VendorSystemAuthenticated = true;
            UnityContainer.RegisterInstance<IDataContainer>(_perRequestDataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(_perRequestDataContainer);

            // DynamicApiRepositoryのモック登録
            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(s => s.HasMailTemplate(It.IsAny<ControllerId>(), It.IsAny<VendorId>())).Returns(true);
            mockDynamicApiRepository.Setup(s => s.HasWebhook(It.IsAny<ControllerId>(), It.IsAny<VendorId>())).Returns(true);
            mockDynamicApiRepository.Setup(s => s.IsApprovedAgreement(It.Is<VendorId>(x => x.Value == _approvedUserVendorId), It.IsAny<ControllerId>())).Returns(true);
            mockDynamicApiRepository.Setup(s => s.IsApprovedAgreement(It.Is<VendorId>(x => x.Value == _rejectedUserVendorId), It.IsAny<ControllerId>())).Returns(false);
            UnityContainer.RegisterInstance(mockDynamicApiRepository.Object);

            // IFilterMAnagerのモック登録
            var mockFilterManager = new Mock<IFilterManager>();
            UnityContainer.RegisterInstance(mockFilterManager.Object);

            // HttpContextAccessor
            UnityContainer.RegisterType<IHttpContextAccessor, HttpContextAccessor>();
        }

        [TestMethod]
        public void TestRequest_有効なIdを設定()
        {
            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction())
                .Returns(expectResult);

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // 結果をチェック
            result.IsSameReferenceAs(expectResult);
        }

        [TestMethod]
        public void TestRequest_認証なし()
        {
            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction())
                .Returns(expectResult);

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(true);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // 結果をチェック
            result.IsSameReferenceAs(expectResult);
        }

        [TestMethod]
        public void TestRequest_認証なし_PerRequestDataContainerから指定()
        {
            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction())
                .Returns(expectResult);

            // PerRequestDataContainerの設定
            _perRequestDataContainer.XNotAuthenticationRequest = true;

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // 結果をチェック
            result.IsSameReferenceAs(expectResult);
        }

        [TestMethod]
        public void TestRequest_不正なIdを設定()
        {
            // PerRequestDataContainerの設定
            _perRequestDataContainer.SystemId = _systemId2.ToString();

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            // パラメータ
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(null, null, auth, new Contents(""), null);
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E02403.ToString(), actualMessage["error_code"]);
            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void TestRequest_HasMailTemplate設定()
        {
            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction())
                .Returns(expectResult);

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // 結果をチェック
            result.IsSameReferenceAs(expectResult);
            method.HasMailTemplate.Value.IsTrue();
        }

        [TestMethod]
        public void TestRequest_HasWebhook設定()
        {
            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction()).Returns(expectResult);

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // 結果をチェック
            result.IsSameReferenceAs(expectResult);
            method.HasWebhook.Value.IsTrue();
        }

        [TestMethod]
        public void TestAuthenticate_有効なIdを設定()
        {
            // テスト対象のインスタンス作成
            var method = CreateMethod();

            // テスト対象のメソッド実行
            var result = method.Authenticate();

            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void TestAuthenticate_不正なIdを設定()
        {
            // PerRequestDataContainerの設定
            _perRequestDataContainer.SystemId = _systemId2.ToString();

            // テスト対象のインスタンス作成
            var method = CreateMethod();

            // テスト対象のメソッド実行
            var result = method.Authenticate();

            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var expect = CreateResponseMessage(DynamicApiMessages.NotAllowedForSystem, DynamicApiMessages.NotAllowedForSystem_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02403.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);
        }

        [TestMethod]
        public void IsVendorDependencyValid_正常系_VendorSystemあり()
        {
            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsVendor = new IsVendor(true);
            method.ActionType = new ActionTypeVO(ActionType.Query);
            method.ActionTypeVersion = new ActionTypeVersion(1);

            // テスト対象のメソッド実行
            var result = (bool)InvokePrivateMethod(method, "IsVendorDependencyValid", new object[] { });
            result.Is(true);
        }

        [TestMethod]
        public void IsVendorDependencyValid_正常系_Vendorなし()
        {
            // PerRequestDataContainerの設定
            _perRequestDataContainer.VendorId = null;

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsVendor = new IsVendor(true);
            method.ActionType = new ActionTypeVO(ActionType.Query);
            method.ActionTypeVersion = new ActionTypeVersion(1);

            // テスト対象のメソッド実行
            var result = (bool)InvokePrivateMethod(method, "IsVendorDependencyValid", new object[] { });
            result.Is(false);
        }

        [TestMethod]
        public void IsVendorDependencyValid_正常系_Systemなし()
        {
            // PerRequestDataContainerの設定
            _perRequestDataContainer.SystemId = null;

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsVendor = new IsVendor(true);
            method.ActionType = new ActionTypeVO(ActionType.Query);
            method.ActionTypeVersion = new ActionTypeVersion(1);

            // テスト対象のメソッド実行
            var result = (bool)InvokePrivateMethod(method, "IsVendorDependencyValid", new object[] { });
            result.Is(false);
        }

        [TestMethod]
        public void IsVendorDependencyValid_正常系_Vendor依存なし()
        {
            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsVendor = new IsVendor(false);
            method.ActionType = new ActionTypeVO(ActionType.Query);
            method.ActionTypeVersion = new ActionTypeVersion(1);

            // テスト対象のメソッド実行
            var result = (bool)InvokePrivateMethod(method, "IsVendorDependencyValid", new object[] { });
            result.Is(true);
        }

        [TestMethod]
        public void Authenticate_正常系_XAdmin_認証無し()
        {
            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Authenticate_正常系_XAdmin_認証有り_成功()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);

            // PerRequestDataContainerの設定
            _perRequestDataContainer.Xadmin = Guid.NewGuid().ToString();

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(true);
            mockIAuthenticationRepository.Setup(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()))
                .Returns<AdminKeyword, SystemId>((adminKeyword, systemId) =>
                {
                    adminKeyword.Value.Is(_perRequestDataContainer.Xadmin);
                    systemId.Value.Is(_perRequestDataContainer.SystemId);
                    return new AdminAuthResult(true);
                });
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);

            mockIAuthenticationRepository.Verify(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void Authenticate_正常系_XAdmin_認証有り_失敗()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            mockIAuthenticationRepository.Setup(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()))
                .Returns(new AdminAuthResult(false));
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);

            // PerRequestDataContainerの設定
            _perRequestDataContainer.Xadmin = Guid.NewGuid().ToString();

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(true);
            mockIAuthenticationRepository.Setup(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()))
                .Returns<AdminKeyword, SystemId>((adminKeyword, systemId) =>
                {
                    adminKeyword.Value.Is(_perRequestDataContainer.Xadmin);
                    systemId.Value.Is(_perRequestDataContainer.SystemId);
                    return new AdminAuthResult(false);
                });
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E02404.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var data = result.Content.ReadAsStringAsync().Result;
            var expect = CreateResponseMessage(DynamicApiMessages.AdminAuthFailed, DynamicApiMessages.AdminAuthFailed_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02404.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);

            mockIAuthenticationRepository.Verify(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()),
                Times.Exactly(1));
        }

        [TestMethod]
        public void Authenticate_Exception_IsAdminで発生_そのまま通知()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            mockIAuthenticationRepository.Setup(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()))
                .Returns(new AdminAuthResult(false));
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);

            // PerRequestDataContainerの設定
            _perRequestDataContainer.Xadmin = Guid.NewGuid().ToString();

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(true);
            var expectException = new Exception();
            mockIAuthenticationRepository.Setup(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()))
                .Callback<AdminKeyword, SystemId>((adminKeyword, systemId) =>
                {
                    adminKeyword.Value.Is(_perRequestDataContainer.Xadmin);
                    systemId.Value.Is(_perRequestDataContainer.SystemId);
                })
                .Throws(expectException);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            AssertEx.Catch<Exception>(() =>
            {
                method.Authenticate();
            }).IsSameReferenceAs(expectException);

            mockIAuthenticationRepository.Verify(x => x.IsAdmin(It.IsAny<AdminKeyword>(), It.IsAny<SystemId>()), Times.Exactly(1));
        }

        [TestMethod]
        public void Authenticate_正常系_ベンダーシステム認証省略を許可()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);
            UnityContainer.RegisterInstance<bool>("HeaderAuthentication", false);

            var vendorId = Guid.NewGuid().ToString().ToUpper();
            var systemId = Guid.NewGuid().ToString().ToUpper();
            UnityContainer.RegisterInstance("VendorSystemAuthenticationDefaultVendorId", vendorId);
            UnityContainer.RegisterInstance("VendorSystemAuthenticationDefaultSystemId", systemId);

            _perRequestDataContainer.VendorSystemAuthenticated = false;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVendorSystemAuthenticationAllowNull = new IsVendorSystemAuthenticationAllowNull(true);

            User user = new User(new Vendor(),  null, null);
            mockIAuthenticationRepository.Setup(x => x.Login(It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<UserId>())).Returns(user);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
            _perRequestDataContainer.VendorId.Is(vendorId.ToLower());
            _perRequestDataContainer.SystemId.Is(systemId.ToLower());
        }

        [TestMethod]
        public void Authenticate_正常系_ベンダーシステム認証省略を不許可()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);
            UnityContainer.RegisterInstance<bool>("HeaderAuthentication", false);

            _perRequestDataContainer.VendorSystemAuthenticated = false;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVendorSystemAuthenticationAllowNull = new IsVendorSystemAuthenticationAllowNull(false);

            User user = new User(new Vendor(), null, null);
            mockIAuthenticationRepository.Setup(x => x.Login(It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<UserId>())).Returns(user);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E02402.ToString(), actualMessage["error_code"]);
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var expect = CreateResponseMessage(DynamicApiMessages.VendorSystemAuthRequired, DynamicApiMessages.VendorSystemAuthRequired_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02402.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);
        }

        [TestMethod]
        public void Authenticate_OpenId_Unauthorized()
        {
            // PerRequestDataContainerの設定
            _perRequestDataContainer.AuthorizationError = "{ \"Message\": \"test\" }";

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsOpenIdAuthentication = new IsOpenIdAuthentication(true);

            // テスト対象のメソッド実行
            var result = method.Authenticate();

            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.Unauthorized);
            result.Content.ReadAsStringAsync().Result.Is(_perRequestDataContainer.AuthorizationError);
        }

        [TestMethod]
        public void Authenticate_OpenId_Forbidden()
        {
            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsOpenIdAuthentication = new IsOpenIdAuthentication(true);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E01401.ToString(), actualMessage["error_code"]);

            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var expect = CreateResponseMessage(DynamicApiMessages.OpenIdAuthRequired, DynamicApiMessages.OpenIdAuthRequired_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E01401.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);
        }

        [TestMethod]
        public void Authenticate_OpenId_NotAllowedCA()
        {
            // PerRequestDataContainerの設定
            _perRequestDataContainer.OpenId = Guid.NewGuid().ToString();
            _perRequestDataContainer.Claims = new Dictionary<string, string>();

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsOpenIdAuthentication = new IsOpenIdAuthentication(true);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E01402.ToString(), actualMessage["error_code"]);

            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var expect = CreateResponseMessage(DynamicApiMessages.OpenIdNotAllowedCA, DynamicApiMessages.OpenIdNotAllowedCA_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E01402.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);
        }

        [TestMethod]
        public void Authenticate_AccessControl_Failed()
        {
            // テスト対象のインスタンス作成
            var method = CreateMethod();
            method.IsAccesskey = new IsAccesskey(true);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            var actualMessage = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            Assert.AreEqual(ErrorCodeMessage.Code.E02401.ToString(), actualMessage["error_code"]);

            // 結果をチェック
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var expect = CreateResponseMessage(DynamicApiMessages.AccessControlAuthFailed, DynamicApiMessages.AccessControlAuthFailed_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02401.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);
        }

        [TestMethod]
        public void Authenticate_正常系_同意と承認_自ベンダー()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);

            // PerRequestDataContainerの設定
            _perRequestDataContainer.VendorId = _providerVendorId;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(true);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Authenticate_正常系_同意と承認_他ベンダー_同意あり()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);

            // PerRequestDataContainerの設定
            _perRequestDataContainer.VendorId = _approvedUserVendorId;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(true);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
        }



        [TestMethod]
        public void Authenticate_正常系_同意と承認_他ベンダー_同意なし()
        {
            var mockIAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(mockIAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.VendorId = _rejectedUserVendorId;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(true);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            var expect = CreateResponseMessage(DynamicApiMessages.NotAgreedAndApproved, DynamicApiMessages.NotAgreedAndApproved_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E10413.ToString()).ToJson();
            result.Content.ReadAsStringAsync().Result.ToJson().Is(expect);
        }

        /* クライアント証明書認証は未対応
        [TestMethod]
        public void Authenticate_ClientCertificationAuth_正常系_クライアント証明書使用設定()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", true);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = _vendorId.ToString();
            perRequestDataContainer.SystemId = _systemId1.ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = true;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Authenticate_ClientCertificationAuth_正常系_ベンダー認証とクライアント証明書認証がどっちも有効な場合はベンダー認証を優先()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", true);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = _vendorId.ToString();
            perRequestDataContainer.SystemId = _systemId1.ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = true;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(true);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.OK);
        }


        [TestMethod]
        public void Authenticate_ClientCertificationAuth_異常系_クライアント証明書_authenticatedがfalse()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", true);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = _vendorId.ToString();
            perRequestDataContainer.SystemId = _systemId1.ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = false;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            result.Content.ReadAsStringAsync().Result.Is(CreateResponseMessage(DynamicApiMessages.ClientCertificateAuthenticationRequired, DynamicApiMessages.ClientCertificateAuthenticationRequired_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02411.ToString()));
        }

        [TestMethod]
        public void Authenticate_ClientCertificationAuth_正常系_クライアント証明書が使用できるように設定されていない_且つクライアント証明書が検証されていない()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", false);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = _vendorId.ToString();
            perRequestDataContainer.SystemId = _systemId1.ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = false;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Authenticate_ClientCertificationAuth_正常系_クライアント証明書が使用できるように設定されていないがクライアント証明書がOK()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", false);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = _vendorId.ToString();
            perRequestDataContainer.SystemId = _systemId1.ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = true;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void Authenticate_ClientCertificationAuth_異常系_クライアント証明書が使用できるように設定されているがクライアント証明書が検証されていない()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", true);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = _vendorId.ToString();
            perRequestDataContainer.SystemId = _systemId1.ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = false;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            result.Content.ReadAsStringAsync().Result.Is(CreateResponseMessage(DynamicApiMessages.ClientCertificateAuthenticationRequired, DynamicApiMessages.ClientCertificateAuthenticationRequired_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02411.ToString()));
        }

        [TestMethod]
        public void Authenticate_ClientCertificationAuth_異常系_クライアント証明書OKだがFunc検証失敗()
        {
            UnityContainer.RegisterInstance<bool>("apiweb:UseClientCertificateAuth", true);

            //ClientCertAuthentication
            var mockIClientCertAuthenticationRepository = new Mock<IClientCertificateRepository>();
            UnityContainer.RegisterInstance(mockIClientCertAuthenticationRepository.Object);

            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            perRequestDataContainer.VendorId = "hoge".ToString();
            perRequestDataContainer.SystemId = "hoge".ToString();
            perRequestDataContainer.ClientIpAddress = "127.0.0.1";
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            // PerRequestDataContainerの設定
            perRequestDataContainer.XVendorSystemCertificateAuthenticated = true;

            // テスト対象のインスタンス作成
            var method = CreateMethod(
                UnityContainer.Resolve<Method>()
            );
            method.IsHeaderAuthentication = new IsHeaderAuthentication(false);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsVisibleAgreement = new IsVisibleAgreement(false);
            method.IsClientCertAuthentication = new IsClientCertAuthentication(true);

            // PrivateObjectを作成
            var privateObj = new PrivateObject(method);

            // テスト対象のメソッド実行
            var result = (HttpResponseMessage)privateObj.Invoke("Authenticate");
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            result.Content.ReadAsStringAsync().Result.Is(CreateResponseMessage(DynamicApiMessages.NotAllowedForSystem, DynamicApiMessages.NotAllowedForSystem_Detail, (int)HttpStatusCode.Forbidden, ErrorCodeMessage.Code.E02403.ToString()));
        }
        */

        /// <summary>
        /// 独自認証の設定を無効にした時に、それを呼び出すシーケンス（Authentication.Login）を呼び出していないことを確認する
        /// </summary>
        [TestMethod]
        public void Authenticate_OriginalAuthentication_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableOriginalAuthentication", false);

            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction())
                .Returns(expectResult);

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            var mockAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(typeof(IAuthenticationRepository), mockAuthenticationRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // mockAuthenticationRepository.Loginが呼び出されていないことを確認
            mockAuthenticationRepository.Verify(x => x.Login(It.IsAny<VendorId>(), It.IsAny<SystemId>(), It.IsAny<UserId>()), Times.Never);
        }

        /// <summary>
        /// EnableWebHookAndMailTemplateがfalseの場合に、SELECT文の取得を行わないようにする
        /// </summary>
        [TestMethod]
        public void Authenticate_WebHookAndMailTemplate_Disable()
        {
            UnityContainer.RegisterInstance<bool>("EnableOriginalAuthentication", false);
            UnityContainer.RegisterInstance<bool>("EnableWebHookAndMailTemplate", false);

            var expectResult = new HttpResponseMessage();

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.Setup(x => x.ExecuteAction())
                .Returns(expectResult);

            var mockDataStoreRepository = new Mock<IDynamicApiDataStoreRepositoryFactory>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiDataStoreRepositoryFactory), mockDataStoreRepository.Object);

            var mockAuthenticationRepository = new Mock<IAuthenticationRepository>();
            UnityContainer.RegisterInstance(typeof(IAuthenticationRepository), mockAuthenticationRepository.Object);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            UnityContainer.RegisterInstance(typeof(IDynamicApiRepository), mockDynamicApiRepository.Object);

            // テスト対象のインスタンス作成
            var method = CreateMethod();
            UnityContainer.RegisterInstance(method.ActionType.Value.GetRegistorName(method.ActionTypeVersion.Value), mockIDynamicApiAction.Object);

            // パラメータ
            var requestMediaType = new MediaType("application/json");
            var actionId = new ActionId(null);
            var auth = new NotAuthentication(false);

            // テスト対象のメソッド実行
            var result = method.Request(actionId, requestMediaType, auth, new Contents(""), null);

            // HasMailTemplateとHasWebhookが呼び出されていないことを確認する
            mockDynamicApiRepository.Verify(x => x.HasMailTemplate(It.IsAny<ControllerId>(), It.IsAny<VendorId>()), Times.Never);
            mockDynamicApiRepository.Verify(x => x.HasWebhook(It.IsAny<ControllerId>(), It.IsAny<VendorId>()), Times.Never);
        }

        /// <summary>
        /// 自ベンダーでDataOfferのチェックでOK
        /// </summary>
        [TestMethod]
        public void Authenticate_正常系_DataOfferとDataUse_自ベンダー_OK()
        {
            _perRequestDataContainer.VendorId = _providerVendorId;

            var vendorVO = new VendorVO(Guid.NewGuid(), "hoge", true, false, true);
            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.GetVendor(It.IsAny<VendorId>())).Returns(vendorVO);
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApiRepository.Object);

            // テスト対象のメソッド実行
            var method = CreateMethod();
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
            mockDynamicApiRepository.Verify(x => x.GetVendor(It.IsAny<VendorId>()), Times.Once);
        }

        /// <summary>
        /// 自ベンダーでDataOfferのチェックでForbidden
        /// </summary>
        [TestMethod]
        public void Authenticate_正常系_DataOfferとDataUse_自ベンダー_Forbidden()
        {
            _perRequestDataContainer.VendorId = _providerVendorId;

            var vendorVO = new VendorVO(Guid.NewGuid(), "hoge", false, true, true);
            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.GetVendor(It.IsAny<VendorId>())).Returns(vendorVO);
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApiRepository.Object);

            // テスト対象のメソッド実行
            var method = CreateMethod();
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            mockDynamicApiRepository.Verify(x => x.GetVendor(It.IsAny<VendorId>()), Times.Once);
        }

        /// <summary>
        /// 他ベンダーでDataUseのチェックでOK
        /// </summary>
        [TestMethod]
        public void Authenticate_正常系_DataOfferとDataUse_他ベンダー_OK()
        {
            var vendorVO = new VendorVO(Guid.NewGuid(), "hoge", false, true, true);
            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.GetVendor(It.IsAny<VendorId>())).Returns(vendorVO);
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApiRepository.Object);

            // テスト対象のメソッド実行
            var method = CreateMethod();
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
            mockDynamicApiRepository.Verify(x => x.GetVendor(It.IsAny<VendorId>()), Times.Once);
        }

        /// <summary>
        /// 他ベンダーでDataUseのチェックでForbidden
        /// </summary>
        [TestMethod]
        public void Authenticate_正常系_DataOfferとDataUse_他ベンダー_Forbidden()
        {
            var vendorVO = new VendorVO(Guid.NewGuid(), "hoge", true, false, true);
            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockDynamicApiRepository.Setup(x => x.GetVendor(It.IsAny<VendorId>())).Returns(vendorVO);
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApiRepository.Object);

            // テスト対象のメソッド実行
            var method = CreateMethod();
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.Forbidden);
            mockDynamicApiRepository.Verify(x => x.GetVendor(It.IsAny<VendorId>()), Times.Once);
        }

        /// <summary>
        /// Openベンダーの場合は、データ提供・利用ベンダーの判定を行わないようにする
        /// </summary>
        [TestMethod]
        public void Authenticate_正常系_DataOfferとDataUse_Openベンダー()
        {
            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApiRepository.Object);

            var vendorId = Guid.NewGuid().ToString().ToUpper();
            UnityContainer.RegisterInstance("DefaultVendorId", vendorId);
            _perRequestDataContainer.VendorId = vendorId;

            // テスト対象のメソッド実行
            var method = CreateMethod();
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
            mockDynamicApiRepository.Verify(x => x.GetVendor(It.IsAny<VendorId>()), Times.Never);
        }

        /// <summary>
        /// EnableVendorDataUseAndOfferがfalseの場合に、データ提供・利用ベンダーの判定を行わないようにする
        /// </summary>
        [TestMethod]
        public void Authenticate_正常系_DataOfferとDataUse_無効()
        {
            UnityContainer.RegisterInstance<bool>("EnableVendorDataUseAndOffer", false);

            var mockDynamicApiRepository = new Mock<IDynamicApiRepository>();
            UnityContainer.RegisterInstance<IDynamicApiRepository>(mockDynamicApiRepository.Object);

            // テスト対象のメソッド実行
            var method = CreateMethod();
            var result = method.Authenticate();
            result.StatusCode.Is(HttpStatusCode.OK);
            mockDynamicApiRepository.Verify(x => x.GetVendor(It.IsAny<VendorId>()), Times.Never);
        }

        private Method CreateMethod(Method inputMethod = null)
        {
            var method = inputMethod ?? new Method();
            method.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            method.ApiId = new ApiId(Guid.NewGuid().ToString());
            method.IsHeaderAuthentication = new IsHeaderAuthentication(true);
            method.IsAdminAuthentication = new IsAdminAuthentication(false);
            method.IsOpenIdAuthentication = new IsOpenIdAuthentication(false);
            method.IsAccesskey = new IsAccesskey(false);
            method.ApiUri = new ApiUri("API/Private/Test");
            method.VendorId = new VendorId(_providerVendorId);

            method.IsVendor = new IsVendor(false);
            method.ActionType = new ActionTypeVO(ActionType.Query);
            method.ActionTypeVersion = new ActionTypeVersion(1);
            method.RepositoryInfo = new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo>());
            method.Script = new Script(null);
            method.PostDataType = new PostDataType("array");

            method.IsVendorSystemAuthenticationAllowNull = new IsVendorSystemAuthenticationAllowNull(false);

            return method;
        }
        private string CreateResponseMessage(string title, string detail, int status, string errorCode)
            => JsonConvert.SerializeObject(new { error_code = errorCode, title = title, status = status, detail = detail });

        private object InvokePrivateMethod(object target, string methodName, object[] parameters)
            => target.GetType().InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, target, parameters);
    }
}
