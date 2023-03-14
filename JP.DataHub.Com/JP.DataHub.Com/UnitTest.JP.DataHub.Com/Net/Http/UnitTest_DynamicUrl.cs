using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Attributes;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.Com.WebApi;

namespace UnitTest.JP.DataHub.Com.Net.Http
{
    [TestClass]
    public class UnitTest_DynamicUrl : ComUnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        internal class TestResource : Resource
        {
            public TestResource()
                : base()
            {
            }
            public TestResource(string serverUrl)
                : base(serverUrl)
            {
            }
            public TestResource(IServerEnvironment serverEnvironment)
                : base(serverEnvironment)
            {
            }

            [WebApiPost]
            [AutoGenerateReturnModel]
            public WebApiRequestModel<VoidModel> AdaptResourceSchema() => MakeApiRequestModel<WebApiRequestModel<VoidModel>>();
        }

        [TestMethod]
        public void DynamicUrl_ResourceUrl()
        {
            var client = new WebApiClient(Environment, AuthenticationResult);
            var api = new TestResource();
            api.ResourceUrl = "/API/Public/Hoge";
            api.AddHeaders.Add("X-Cache", "1");
            var requestModel = api.AdaptResourceSchema();
            requestModel.ResourceUrl.Is("/API/Public/Hoge");
            requestModel.Action.Is("AdaptResourceSchema");
            requestModel.Header.Count.Is(1);
            requestModel.Header["X-Cache"].Is(new string[] { "1" });
        }

        [TestMethod]
        public void DynamicUrl_ActionUrl()
        {
            var client = new WebApiClient(Environment, AuthenticationResult);
            var api = new Resource();
            api.ResourceUrl = "/API/Public/Hoge";
            api.ActionUrl = "ActionMethod";
            api.AddHeaders.Add("X-Hoge", "2");
            var requestModel = api.MakeApiRequestModel<WebApiRequestModel<VoidModel>>();
            requestModel.ResourceUrl.Is("/API/Public/Hoge");
            requestModel.Action.Is("ActionMethod");
            requestModel.Header.Count.Is(1);
            requestModel.Header["X-Hoge"].Is(new string[] { "2" });
        }
    }
}
