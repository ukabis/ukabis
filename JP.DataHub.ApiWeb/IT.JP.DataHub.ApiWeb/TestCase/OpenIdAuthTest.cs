using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("ManageAPI")]
    public class OpenIdAuthTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);


            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var methodList = clientM.GetWebApiResponseResult(manageApi.GetApiResourceFromUrl("/API/IntegratedTest/OpenIdAuth"))
                .Assert(GetSuccessExpectStatusCode).Result.MethodList;

            foreach (var method in methodList)
            {
                var id = method.MethodId;
                clientM.GetWebApiResponseResult(manageApi.UseOpenIdAuthorizeMethod(id, true)).Assert(HttpStatusCode.OK);
                clientM.GetWebApiResponseResult(manageApi.AddOpenIdAuthorizeMethod(id, clientM.GetOpenId())).Assert(HttpStatusCode.OK);
            }
        }

        [TestMethod]
        public void AuthorizedUser()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IOpenIdAuthApi>();

            client.GetWebApiResponseResult(api.GetAll()).Assert(GetExpectStatusCodes);
        }

        [TestMethod]
        public void RejectUser()
        {
            var client = new IntegratedTestClient("test3");
            var api = UnityCore.Resolve<IOpenIdAuthApi>();

            client.GetWebApiResponseResult(api.GetAll()).AssertErrorCode(ForbiddenExpectStatusCode, "E01403");
        }
    }
}
