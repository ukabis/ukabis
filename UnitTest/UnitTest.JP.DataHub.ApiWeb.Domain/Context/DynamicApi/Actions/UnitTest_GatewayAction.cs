using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_GatewayAction : UnitTestBase
    {
        public void SetupContainer()
        {
            base.TestInitialize(true);
            UnityContainer.RegisterInstance<IDataContainer>(new PerRequestDataContainer());
        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_GET_正常系()
        {
            SetupContainer();
            var mockRepository = new Mock<IDynamicGatewayRepository>();
            mockRepository.Setup(x => x.Get(It.IsAny<RequestGatewayUrl>(), It.IsAny<GatewayInfo>(), It.IsAny<Contents>(), It.IsAny<bool>())).Returns(new GatewayResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance<IDynamicGatewayRepository>(mockRepository.Object);

            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/1";

            GatewayInfo gatewayInfo = new GatewayInfo(gatewayUrl, "user", "pass", "");
            Contents contents = new Contents("");
            var action = CreateGatewayAction(HttpMethodType.MethodTypeEnum.GET, gatewayInfo, requestUrl, gatewayUrl, controllerUrl, apiUrl);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.OK);
            mockRepository.Verify(x => x.Get(It.Is<RequestGatewayUrl>(y => y.Value.Equals(accessUrl)), gatewayInfo, action.Contents, false), Times.Once);

        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_POST_正常系()
        {
            SetupContainer();
            var mockRepository = new Mock<IDynamicGatewayRepository>();
            mockRepository.Setup(x => x.Post(It.IsAny<RequestGatewayUrl>(), It.IsAny<GatewayInfo>(), It.IsAny<Contents>(), It.IsAny<bool>())).Returns(new GatewayResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance<IDynamicGatewayRepository>(mockRepository.Object);

            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/1";
            GatewayInfo gatewayInfo = new GatewayInfo(gatewayUrl, "user", "pass", "");
            Contents contents = new Contents("");

            var action = CreateGatewayAction(HttpMethodType.MethodTypeEnum.POST, gatewayInfo, requestUrl, gatewayUrl, controllerUrl, apiUrl);
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.OK);
            mockRepository.Verify(x => x.Post(It.Is<RequestGatewayUrl>(y => y.Value.Equals(accessUrl)), gatewayInfo, action.Contents, false), Times.Once);

        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_PUT_正常系()
        {
            SetupContainer();
            var mockRepository = new Mock<IDynamicGatewayRepository>();
            mockRepository.Setup(x => x.Put(It.IsAny<RequestGatewayUrl>(), It.IsAny<GatewayInfo>(), It.IsAny<Contents>(), It.IsAny<bool>())).Returns(new GatewayResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance<IDynamicGatewayRepository>(mockRepository.Object);

            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/1";
            GatewayInfo gatewayInfo = new GatewayInfo(gatewayUrl, "user", "pass", "");
            Contents contents = new Contents("");

            var action = CreateGatewayAction(HttpMethodType.MethodTypeEnum.PUT, gatewayInfo, requestUrl, gatewayUrl, controllerUrl, apiUrl);
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.OK);
            mockRepository.Verify(x => x.Put(It.Is<RequestGatewayUrl>(y => y.Value.Equals(accessUrl)), gatewayInfo, action.Contents, false), Times.Once);

        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_DELETE_正常系()
        {
            SetupContainer();
            var mockRepository = new Mock<IDynamicGatewayRepository>();
            mockRepository.Setup(x => x.Delete(It.IsAny<RequestGatewayUrl>(), It.IsAny<GatewayInfo>(), It.IsAny<Contents>(), It.IsAny<bool>())).Returns(new GatewayResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance<IDynamicGatewayRepository>(mockRepository.Object);
            
            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/1";
            GatewayInfo gatewayInfo = new GatewayInfo(gatewayUrl, "user", "pass", "");
            Contents contents = new Contents("");
            var action = CreateGatewayAction(HttpMethodType.MethodTypeEnum.DELETE, gatewayInfo, requestUrl, gatewayUrl, controllerUrl, apiUrl);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.OK);
            mockRepository.Verify(x => x.Delete(It.Is<RequestGatewayUrl>(y => y.Value.Equals(accessUrl)), gatewayInfo, action.Contents, false), Times.Once);
        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_PATHC_正常系()
        {
            SetupContainer();
            var mockRepository = new Mock<IDynamicGatewayRepository>();
            mockRepository.Setup(x => x.Patch(It.IsAny<RequestGatewayUrl>(), It.IsAny<GatewayInfo>(), It.IsAny<Contents>(), It.IsAny<bool>())).Returns(new GatewayResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance<IDynamicGatewayRepository>(mockRepository.Object);
            
            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/1";
            GatewayInfo gatewayInfo = new GatewayInfo(gatewayUrl, "user", "pass", "");
            Contents contents = new Contents("");
            var action = CreateGatewayAction(HttpMethodType.MethodTypeEnum.PATCH, gatewayInfo, requestUrl, gatewayUrl, controllerUrl, apiUrl);

            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.OK);
            mockRepository.Verify(x => x.Patch(It.Is<RequestGatewayUrl>(y => y.Value.Equals(accessUrl)), gatewayInfo, action.Contents, false), Times.Once);
        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_その他メソッド()
        {
            SetupContainer();
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.UserId = new Guid();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            GatewayInfo gatewayInfo = new GatewayInfo(gatewayUrl, "user", "pass", "");
            Contents contents = new Contents("");
            var action = CreateGatewayAction(HttpMethodType.MethodTypeEnum.TRACE, gatewayInfo, requestUrl, gatewayUrl, controllerUrl, apiUrl);
            var result = action.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.NotImplemented);

        }

        [TestMethod]
        public void GatewayAction_ExecuteAction_GET_キャッシュヒット()
        {
            var response = new GatewayResponse(new HttpResponseMessage()
            {
                Content = new StringContent("{\"aaaa\":\"bbbb\"}"),
            });

            SetupContainer();
            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Contains(It.IsAny<string>())).Returns(true);
            var outValue = false;
            mockCache.Setup(x => x.Get<GatewayResponse>(It.IsAny<string>(), out outValue, true)).Returns(response);
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var mockRepository = new Mock<IDynamicGatewayRepository>();
            mockRepository.Setup(x => x.CreateCacheKey(It.IsAny<IGatewayAction>())).Returns("key");

            UnityContainer.RegisterInstance(mockRepository.Object);
            GatewayInfo gatewayInfo = new GatewayInfo("http://hoge.fuga/api/{par3}/{par1}/?honya={par2}&honya2={par4}", "user", "pass", "");

            var testClass = UnityContainer.Resolve<GatewayAction>();
            testClass.SystemId = new SystemId(Guid.NewGuid().ToString());
            testClass.VendorId = new VendorId(Guid.NewGuid().ToString());
            testClass.ProviderSystemId = new SystemId(Guid.NewGuid().ToString());
            testClass.ProviderVendorId = new VendorId(Guid.NewGuid().ToString());
            testClass.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            testClass.GatewayInfo = gatewayInfo;
            testClass.CacheInfo = new CacheInfo(true, 0, "");
            testClass.ActionType = new ActionTypeVO(ActionType.Gateway);
            testClass.ApiId = new ApiId(Guid.NewGuid().ToString());

            var result = testClass.ExecuteAction();
            result.StatusCode.IsStructuralEqual(HttpStatusCode.OK);

            result.Content.IsStructuralEqual(response.Message.Content);
            result.Headers.GetValues("X-Cache").First().Is($"HIT key:DynamicApiAction.{testClass.ProviderVendorId.Value}.{testClass.ProviderSystemId.Value}.{testClass.ControllerId.Value}.{testClass.ApiId.Value}.key");
        }

        private GatewayAction CreateGatewayAction(string requestUrl, string gatewayUrl, string controllerUrl, string apiUrl)
        {
            string[] splitUrl = UriUtil.SplitRelativeUrl(requestUrl.Split('?')[0]);
            GatewayAction action = new GatewayAction();
            action.ApiUri = new ApiUri(controllerUrl + apiUrl);
            action.GatewayInfo = new GatewayInfo(gatewayUrl, "", "", "");

            Dictionary<string, string> keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitUrl, controllerUrl, apiUrl);
            if (keyValue == null)
            {
                keyValue = UriUtil.IsMatchUrlWithoutQueryString(splitUrl, controllerUrl, apiUrl);
            }
            if (keyValue == null)
            {
                keyValue = new Dictionary<string, string>();
            }
            action.KeyValue = new UrlParameter(keyValue.Select(x => new { Key = new UrlParameterKey(x.Key), Value = new UrlParameterValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value));

            var sp = requestUrl.Split('?');
            string query = "";
            if (sp.Length == 2)
            {
                query = "?" + sp[1];
            }

            Dictionary<QueryStringKey, QueryStringValue> q = new Dictionary<QueryStringKey, QueryStringValue>();
            if (query != null && string.IsNullOrEmpty(query) == false)
            {
                q = UriUtil.ParseQueryString(query).Select(x => new { Key = new QueryStringKey(x.Key), Value = new QueryStringValue(x.Value) }).ToDictionary(key => key.Key, value => value.Value);
            }
            if (keyValue != null && keyValue.Count > 0)
            {
                foreach (var k in keyValue)
                {
                    if (q.Where(x => x.Key.Value == k.Key).Count() == 0)
                    {
                        q.Add(new QueryStringKey(k.Key), new QueryStringValue(k.Value));
                    }
                }
            }
            action.Query = q.Count() == 0 ? null : new QueryStringVO(q);

            return action;

        }

        private GatewayAction CreateGatewayAction(
            HttpMethodType.MethodTypeEnum methodType,
            GatewayInfo gatewayInfo, string requestUrl, string gatewayUrl, string controllerUrl, string apiUrl)
        {
            GatewayAction action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            action.SystemId = new SystemId(Guid.NewGuid().ToString());
            action.VendorId = new VendorId(Guid.NewGuid().ToString());
            action.ApiId = new ApiId(Guid.NewGuid().ToString());
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.MethodType = new HttpMethodType(methodType);
            action.RepositoryKey = new RepositoryKey("/API/Private/GatewayTest/{Id}");
            action.ActionType = new ActionTypeVO(ActionType.Gateway);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            action.GatewayInfo = gatewayInfo;
            action.Contents = new Contents("'hogehoge':'fugafuga'");
            action.CacheInfo = new CacheInfo(false, 0, "");
            return action;
        }
    }
}
