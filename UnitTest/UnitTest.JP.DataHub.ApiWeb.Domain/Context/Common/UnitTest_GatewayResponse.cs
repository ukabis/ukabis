using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_GatewayResponse : UnitTestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            var container = new UnityContainer();
            UnityCore.UnityContainer = container;
            container.RegisterInstance("MaxSaveApiResponseCacheSize", 1000);
        }

        [TestMethod]
        public void GatewayResponse_Noncache()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent("aaaaaa");
            var testData = new GatewayResponse(response, false);
            testData.Base64SeralizedHttpResponseMessage.IsNull();
        }

        [TestMethod]
        public void GatewayResponse_Serialize()
        {
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StringContent("aaaaaa");
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            response.Headers.Add("hoge", "fuga");
            var testData = new GatewayResponse(response, true);
            var target = MessagePackSerializer.Serialize(testData);
            var result = MessagePackSerializer.Deserialize<GatewayResponse>(target);
            result.Message.IsStructuralEqual(testData.Message);
            result.StatusCode.Is(testData.StatusCode);
            result.Base64SeralizedHttpResponseMessage.Is(testData.Base64SeralizedHttpResponseMessage);
            result.ContentLength.Is(testData.ContentLength);
            result.Headers.Count().Is(testData.Headers.Count());
            foreach (var resultHeader in result.Headers)
            {
                var testDataHeader = testData.Headers.First(x => x.Key == resultHeader.Key);
                resultHeader.Value.Count().Is(testDataHeader.Value.Count());
                resultHeader.Value.ToArray().IsStructuralEqual(testDataHeader.Value.ToArray());
            }
        }

        [TestMethod]
        public void GatewayResponse_SerializeValueNull()
        {
            HttpResponseMessage response = null;
            var testData = new GatewayResponse(response);
            var target = MessagePackSerializer.Serialize(testData);
            MessagePackSerializer.Deserialize<GatewayResponse>(target).IsStructuralEqual(testData);
        }

        [TestMethod]
        public void GatewayResponse_SerializeNull()
        {
            GatewayResponse testData = null;
            var target = MessagePackSerializer.Serialize(testData);
            MessagePackSerializer.Deserialize<GatewayResponse>(target).IsStructuralEqual(testData);
        }
    }
}
