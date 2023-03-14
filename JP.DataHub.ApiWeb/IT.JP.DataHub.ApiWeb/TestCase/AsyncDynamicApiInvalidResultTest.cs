using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore] // これを実行するとStagingにアラートが飛ぶのでIgnore
    [TestClass]
    [TestCategory("Async")]
    public class AsyncDynamicApiInvalidResultTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod] // Stagingでしか動かない想定
        public void NoHeaderResultScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();

            var requestId = "6a26721f-cc56-473b-84ad-9b27d1ad5059";
            var errMessageJson = $@"
{{
    ""Message"": ""RequestId={requestId} Result File Not Found""
}}";

            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(InternalServerErrorStatusCode).ContentString.ToJson().Is(JToken.Parse(errMessageJson));
        }

        [TestMethod] // Stagingでしか動かない想定
        public void NoBodyResultScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();

            var requestId = "a8f0428c-ee49-4a39-9cd4-e0e949384586";
            var errMessageJson = $@"
{{
    ""Message"": ""RequestId={requestId} Result File Not Found""
}}";

            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(InternalServerErrorStatusCode).ContentString.ToJson().Is(JToken.Parse(errMessageJson));
        }
    }
}

