using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_RequestGatewayUrl : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }


        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_パラメータなし()
        {
            string requestUrl = "http://api.hoge/api/private/apiname";
            string gatewayUrl = "http://gateway.hoge";
            string controllerUrl = "/api/private/";
            string apiUrl = "apiname";
            string accessUrl = "http://gateway.hoge/";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリングパラメータあり()
        {
            string requestUrl = "http://api.hoge/api/private/apiname?par=1";
            string gatewayUrl = "http://gateway.hoge?par={p1}";
            string controllerUrl = "/api/private/";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/?par=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_Gatewayパラメータなし()
        {
            string requestUrl = "http://api.hoge/api/private/apiname?par=1";
            string gatewayUrl = "http://gateway.hoge";
            string controllerUrl = "/api/private/";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/?par=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_DyanamiAPIGatewayパラメータなし()
        {
            string requestUrl = "http://api.hoge/api/private/apiname?par=1";
            string gatewayUrl = "http://gateway.hoge";
            string controllerUrl = "/api/private/";
            string apiUrl = "apiname";
            string accessUrl = "http://gateway.hoge/?par=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ()
        {
            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }
        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータGatewayなし()
        {
            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}";
            string accessUrl = "http://gateway.hoge/";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }
        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数()
        {
            string requestUrl = "/api/private/apiname/1/2";
            string gatewayUrl = "http://gateway.hoge/{p1}/{p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}/{p2}";
            string accessUrl = "http://gateway.hoge/1/2";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_定義と不一致()
        {
            string requestUrl = "/api/private/apiname/1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname";
            string accessUrl = "http://gateway.hoge/";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_パラメータ名が違うもの()
        {
            string requestUrl = "/api/private/apiname?par=1";
            string gatewayUrl = "http://gateway.hoge/?AAAA={p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/?AAAA=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリングURLパラメータ複合()
        {
            string requestUrl = "/api/private/apiname/1?par=2";
            string gatewayUrl = "http://gateway.hoge/{p1}?par={p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}?par={p2}";
            string accessUrl = "http://gateway.hoge/1?par=2";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリングURLパラメータ複合入れ替え()
        {
            string requestUrl = "/api/private/apiname/1?par=2";
            string gatewayUrl = "http://gateway.hoge/{p2}?par={p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}?par={p2}";
            string accessUrl = "http://gateway.hoge/2?par=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリング複数()
        {
            string requestUrl = "/api/private/apiname?par1=1&par2=2";
            string gatewayUrl = "http://gateway.hoge?par1={p1}&par2={p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par1={p1}&par2={p2}";
            string accessUrl = "http://gateway.hoge/?par1=1&par2=2";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリング複数入れ替え()
        {
            string requestUrl = "/api/private/apiname?par1=1&par2=2";
            string gatewayUrl = "http://gateway.hoge?par1={p2}&par2={p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par1={p1}&par2={p2}";
            string accessUrl = "http://gateway.hoge/?par1=2&par2=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリング複数リクエストパラメータなし()
        {
            string requestUrl = "/api/private/apiname?par1=1";
            string gatewayUrl = "http://gateway.hoge?par1={p1}&par2={p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par1={p1}&par2={p2}";
            string accessUrl = "http://gateway.hoge/?par1=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリングからURLパラメータに入れ替え()
        {
            string requestUrl = "/api/private/apiname?par=1";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/1?par=1";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリングからURLパラメータに入れ替えパラメータなし()
        {
            string requestUrl = "/api/private/apiname";
            string gatewayUrl = "http://gateway.hoge/{p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_クエリストリングからURLパラメータに入れ替えパラメータなし2階層()
        {
            string requestUrl = "/api/private/apiname";
            string gatewayUrl = "http://gateway.hoge/{p1}/{p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par={p1}&par2={p2}";
            string accessUrl = "http://gateway.hoge/";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_固定URL()
        {
            string requestUrl = "/api/private/apiname?par=1";
            string gatewayUrl = "http://gateway.hoge?par={p1}&aaa=bbb";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/?par=1&aaa=bbb";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_Valueなし()
        {
            string requestUrl = "/api/private/apiname?par=";
            string gatewayUrl = "http://gateway.hoge?par={p1}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par={p1}";
            string accessUrl = "http://gateway.hoge/?par=";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_Keyのみ()
        {
            string requestUrl = "/api/private/apiname?par";
            string gatewayUrl = "http://gateway.hoge?par";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par";
            string accessUrl = "http://gateway.hoge/?par";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_エラー()
        {
            string requestUrl = "/api/private/apiname?par2=2";
            string gatewayUrl = "http://gateway.hoge/{p1}/{p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname?par1={p1}&par2={p2}";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            try
            {
                RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);
            }
            catch (Exception ex)
            {
                ex.Message.IsStructuralEqual("Parameter is not set : {p1}");
            }
        }
        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_エラー_2()
        {
            string requestUrl = "/api/private/apiname/1/?par1=&par2=4";
            string gatewayUrl = "http://gateway.hoge/{p1}/{p2}?par1={p3}&par2={p4}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p3}/{p4}?par1={p1}&par2={p2}";
            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            try
            {
                RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);
            }
            catch (Exception ex)
            {
                ex.Message.IsStructuralEqual("Parameter is not set : {p1}");
            }
        }
        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_2()
        {
            string requestUrl = "/api/private/apiname/1/2?par1=3&par2=4";
            string gatewayUrl = "http://gateway.hoge/{p1}/{p2}?par1={p3}&par2={p4}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}/{p2}?par1={p3}&par2={p4}";
            string accessUrl = "http://gateway.hoge/1/2?par1=3&par2=4";

            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);

        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_3()
        {
            string requestUrl = "/api/private/apiname/1/2?par1=3&par2=4";
            string gatewayUrl = "http://gateway.hoge/{p3}/{p4}?par1={p1}&par2={p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}/{p2}?par1={p3}&par2={p4}";
            string accessUrl = "http://gateway.hoge/3/4?par1=1&par2=2";

            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_4()
        {
            string requestUrl = "/api/private/apiname/1/2?par1=3";
            string gatewayUrl = "http://gateway.hoge/{p3}/{p4}?par1={p1}&par2={p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}/{p2}?par1={p3}&par2={p4}";
            string accessUrl = "http://gateway.hoge/3?par1=1&par2=2";

            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_5()
        {
            string requestUrl = "/api/private/apiname/1/2?par1=3&par2=";
            string gatewayUrl = "http://gateway.hoge/{p3}/{p4}?par1={p1}&par2={p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}/{p2}?par1={p3}&par2={p4}";
            string accessUrl = "http://gateway.hoge/3?par1=1&par2=2";

            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
        }

        [TestMethod]
        public void RequestGatewayUrl_CreateGatewayUrl_URLパラメータ複数_6()
        {
            string requestUrl = "/api/private/apiname/1/2?par1=3&par2=";
            string gatewayUrl = "http://gateway.hoge/{p1}/{p2}";
            string controllerUrl = "/api/private";
            string apiUrl = "apiname/{p1}/{p2}?par1={p3}&par2={p4}";
            string accessUrl = "http://gateway.hoge/1/2?par1=3&par2";

            var action = CreateGatewayAction(requestUrl, gatewayUrl, controllerUrl, apiUrl);
            RequestGatewayUrl requestGatewayUrl = new RequestGatewayUrl(new GatewayUri(gatewayUrl), action.Query, action.KeyValue, action.ApiUri);

            requestGatewayUrl.Value.IsStructuralEqual(accessUrl);
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
