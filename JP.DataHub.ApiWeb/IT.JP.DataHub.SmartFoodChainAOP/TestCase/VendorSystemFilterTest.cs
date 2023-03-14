using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.UnitTest.Com;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    // TODO ApiのInterfaceの書き方を他と合わせるとテストが通らなくなる
    [TestClass]
    public class VendorSystemFilterTest : ItTestCaseBase
    {
        private string TestApplicationId = "0DC4747C-E27A-4E68-A376-B3061D9758C4";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void VendorSystemFilter__NormalScenario()
        {
            // ベンダー依存のリソースから、applicationId未指定でデータを取得
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var api = UnityCore.Resolve<IVendorSystemFilterApi>();
            var code = "0";
            var request = api.Get(code);
            request.Header = new Dictionary<string, string[]>()
            {
                { HeaderConst.X_GetInternalAllField, new string[] { "true" } }
            };
            var response = client.Request(request).ToWebApiResponseResult<VendorSystemFilterModel>();
            response.StatusCode.Is(HttpStatusCode.OK);
            response.Result.code.Is(code);
            var vendorId = response.Result._Vendor_Id;
            var systemId = response.Result._System_Id;

            // ベンダー依存のリソースから、applicationIdを指定してデータを取得
            request = api.Get(code, TestApplicationId);
            request.Header = new Dictionary<string, string[]>()
            {
                { HeaderConst.X_GetInternalAllField, new string[] { "true" } }
            };
            response = client.Request(request).ToWebApiResponseResult<VendorSystemFilterModel>();
            response.StatusCode.Is(HttpStatusCode.OK);
            response.Result.code.Is(code);

            // 異なるベンダー・システムのデータを取得できているか
            response.Result._Vendor_Id.IsNot(vendorId);
            response.Result._System_Id.IsNot(systemId);

            // 存在しないアプリケーションを取得してでデータを取得(BadRequest)
            client.Request(api.Get(code, "test")).StatusCode.Is(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// 対象のAPIにAOPが適用されていることを確認する。
        /// </summary>
        [TestMethod]
        public void VendorSystemFilter__TargetScenario()
        {
            // Functionを登録
            var functionModel = new FunctionModel()
            {
                ApplicationId = TestApplicationId,
                FunctionId = Guid.NewGuid().ToString(),
                FunctionName = "__IntegratedTest_Function"
            };
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var functionApi = UnityCore.Resolve<IFunctionApi>();
            var registerResponse = client.Request(functionApi.Register(functionModel, TestApplicationId)).ToWebApiResponseResult<RegisterResultModel>();
            registerResponse.StatusCode.Is(HttpStatusCode.Created);

            // Functionを取得(applicationId指定でOK、未指定だとNotFound)
            client.Request(functionApi.Get(functionModel.FunctionId, TestApplicationId)).StatusCode.Is(HttpStatusCode.OK);
            client.Request(functionApi.Get(functionModel.FunctionId)).StatusCode.Is(HttpStatusCode.NotFound);

            // Roleを登録
            var roleModel = new RoleModel()
            {
                ApplicationId = TestApplicationId,
                PrivateRoleId = Guid.NewGuid().ToString(),
                RoleName = "__IntegratedTest_Role",
            };
            var roleApi = UnityCore.Resolve<IRoleApi>();
            registerResponse = client.Request(roleApi.Register(roleModel, TestApplicationId)).ToWebApiResponseResult<RegisterResultModel>();
            registerResponse.StatusCode.Is(HttpStatusCode.Created);

            // Roleを取得(applicationId指定でOK、未指定だとNotFound)
            client.Request(roleApi.Get(roleModel.PrivateRoleId, TestApplicationId)).StatusCode.Is(HttpStatusCode.OK);
            client.Request(roleApi.Get(roleModel.PrivateRoleId)).StatusCode.Is(HttpStatusCode.NotFound);

            // AuthorizeUserを登録(applicationId指定)
            var authorizeUserModel = new AuthorizeUserModel()
            {
                ApplicationId = TestApplicationId,
                AuthorizeUserId = Guid.NewGuid().ToString(),
                OpenId = "test"
            };
            var authorizeUserApi = UnityCore.Resolve<IAuthorizeUserApi>();
            registerResponse = client.Request(authorizeUserApi.Register(authorizeUserModel, TestApplicationId)).ToWebApiResponseResult<RegisterResultModel>();
            registerResponse.StatusCode.Is(HttpStatusCode.Created);

            // AuthorizeUserを取得(applicationId指定でOK、未指定だとNotFound)
            client.Request(authorizeUserApi.Get(authorizeUserModel.AuthorizeUserId, TestApplicationId)).StatusCode.Is(HttpStatusCode.OK);
            client.Request(authorizeUserApi.Get(authorizeUserModel.AuthorizeUserId)).StatusCode.Is(HttpStatusCode.NotFound);

            // AuthorizeUserを削除
            client.Request(authorizeUserApi.Delete(authorizeUserModel.AuthorizeUserId, TestApplicationId)).StatusCode.Is(HttpStatusCode.NoContent);
            // Roleを削除
            client.Request(roleApi.Delete(roleModel.PrivateRoleId, TestApplicationId)).StatusCode.Is(HttpStatusCode.NoContent);
            // Functionを削除
            client.Request(functionApi.Delete(functionModel.FunctionId, TestApplicationId)).StatusCode.Is(HttpStatusCode.NoContent);
        }
    }
}
