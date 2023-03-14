using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_DynamicGatewayRepository : UnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        public void CreateCacheKey_中継キー一つ()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("aaa", new List<string>() { "fuga" });
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>() { { "aaa", "fuga" } }, action.GatewayInfo.CredentialPassword, action.GatewayInfo.CredentialUsername, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }

        [TestMethod]
        public void CreateCacheKey_中継キー２つ()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("aaa", new List<string>() { "fuga" });
            perRequestDataContainer.RequestHeaders.Add("bbb", new List<string>() { "hoge" });
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>() { { "aaa", "fuga" }, { "bbb", "hoge" } }, action.GatewayInfo.CredentialPassword, action.GatewayInfo.CredentialUsername, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }

        [TestMethod]
        public void CreateCacheKey_中継キー大文字小文字()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("AAA", new List<string>() { "fuga" });
            perRequestDataContainer.RequestHeaders.Add("bbb", new List<string>() { "hoge" });
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.GatewayInfo = new GatewayInfo("http://hogehoge", "hoge", "hogepass", "aaa,BBB");
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>() { { "AAA", "fuga" }, { "bbb", "hoge" } }, action.GatewayInfo.CredentialPassword, action.GatewayInfo.CredentialUsername, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }

        [TestMethod]
        public void CreateCacheKey_中継キーなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>(), action.GatewayInfo.CredentialPassword, action.GatewayInfo.CredentialUsername, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }

        [TestMethod]
        public void CreateCacheKey_中継キーValueなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("aaa", new List<string>());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>() { { "aaa", null } }, action.GatewayInfo.CredentialPassword, action.GatewayInfo.CredentialUsername, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }

        [TestMethod]
        public void CreateCacheKey_認証なし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("aaa", new List<string>() { "fuga" });
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.GatewayInfo = new GatewayInfo("http://hogehoge", null, null, "aaa,bbb"); ;
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>() { { "aaa", "fuga" } }, null, null, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }

        [TestMethod]
        public void CreateCacheKey_リレー設定なし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("aaa", new List<string>() { "fuga" });
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.GatewayInfo = new GatewayInfo("http://hogehoge", null, null, null);
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>(), null, null, action.RelativeUri.Value, action.Query.OriginalQueryString);
            testResult.Is(result);
        }


        [TestMethod]
        public void CreateCacheKey_Queryなし()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.RequestHeaders = new Dictionary<string, List<string>>();
            perRequestDataContainer.RequestHeaders.Add("aaa", new List<string>() { "fuga" });
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateAction();
            action.GatewayInfo = new GatewayInfo("http://hogehoge", null, null, null);
            action.Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>());
            var testObject = new DynamicGatewayRepository();
            var testResult = testObject.CreateCacheKey(action);
            var result = CreateKey(new Dictionary<string, string>(), null, null, action.RelativeUri.Value, null);
            testResult.Is(result);
        }

        private GatewayAction CreateAction()
        {
            GatewayAction action = new GatewayAction();
            var query = new Dictionary<QueryStringKey, QueryStringValue>();
            action.RelativeUri = new RelativeUri("/API/TEST/");
            action.Query = new QueryStringVO(query, "hoge=fuga");
            action.GatewayInfo = new GatewayInfo("http://hogehoge", "hoge", "hogepass", "aaa,bbb");
            return action;
        }


        private string CreateKey(Dictionary<string, string> headers, string password, string username, string url, string query)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var header in headers)
            {
                sb.Append(header.Key.ToLower());
                sb.Append(":");
                sb.Append(header.Value ?? "");
                sb.Append("~");
            }
            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
            {
                sb.Append(username);
                sb.Append(password);
            }
            var hash = GetHashString(sb.ToString());

            //キーはDnamicApiのURL+QueryString+Hedderと認証情報をハッシュ化したもの
            return $"{url}?{query}~{hash}";
        }

        private static string GetHashString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            StringBuilder result = new StringBuilder();
            using (var algorithm = new SHA256CryptoServiceProvider())
            {
                byte[] bs = algorithm.ComputeHash(data);
                algorithm.Clear();
                foreach (byte b in bs)
                {
                    result.Append(b.ToString("X2"));
                }
            }
            return result.ToString();
        }
    }
}