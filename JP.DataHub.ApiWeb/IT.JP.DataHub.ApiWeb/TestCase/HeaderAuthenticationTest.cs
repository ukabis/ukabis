using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("廃止：旧ヘッダー認証")]
    [TestClass]
    public class HeaderAuthenticationTest : ApiWebItTestCase
    {
#if false
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void HeaderAuthenticationTest_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account, "");
            var api = UnityCore.Resolve<IHeaderAuthenticationApi>();
            var testData = new GeoJsonPointTestData(repository, api.ResourceUrl);

            ContextHeaderAuth = new WebApiContext(TestContext, $"{Server}.SmartFoodChain2TestSystemToken", "masas", null, true);
            ContextTokenAuth = new WebApiContext(TestContext);
            ContextOtherVendor = new WebApiContext(TestContext, $"{Server}.SmartFoodChain2TestSystemToken", "masas");

            ResourceHeaderAuth = new HeaderAuthenticationApi(ContextHeaderAuth);
            ResourceTokenAuth = new HeaderAuthenticationApi(ContextTokenAuth);
            ResourceOtherVendor = new HeaderAuthenticationApi(ContextOtherVendor);


            // クリーンアップ
            ContextTokenAuth.ActionAndAssert(ResourceTokenAuth.ODataDelete().Request, ResourceTokenAuth.DeleteExpectStatusCodes);
            ContextOtherVendor.ActionAndAssert(ResourceOtherVendor.ODataDelete().Request, ResourceOtherVendor.DeleteExpectStatusCodes);

            // 許可設定なしのAPIはForbidden
            ContextTokenAuth.ActionAndAssert(ResourceHeaderAuth.GetAll().Request, ResourceHeaderAuth.ForbiddenExpectStatusCode);

            // 許可設定ありのAPIは成功
            ContextTokenAuth.ActionAndAssert(ResourceHeaderAuth.Register("{ \"key\": \"TEST\" }").Request, ResourceHeaderAuth.RegistSuccessExpectStatusCode);

            // ベンダー依存APIで別ベンダーを指定して登録したためNotFound
            ContextTokenAuth.ActionAndAssert(ResourceTokenAuth.Get("TEST").Request, ResourceTokenAuth.NotFoundStatusCode);

            // 別ベンダーからは取得可能
            ContextOtherVendor.ActionAndAssert(ResourceOtherVendor.Get("TEST").Request, ResourceOtherVendor.GetSuccessExpectStatusCode);
        }
#endif
    }
}
