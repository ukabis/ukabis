using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.WebRequest;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.UnitTest.Com;
using System.Net;
using JP.DataHub.UnitTest.Com.Extensions;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class ApplicationManagerFilterTest : ApiWebItTestCase
    {
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
        public void ApplicationManagerFilter_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var client2 = new IntegratedTestClient("test2");
            var api = UnityCore.Resolve<IApplicationApi>();
            
            var TestApplicationId1 = "37713EF6-B086-4419-AE21-AA4090DE4312";
            var TestApplicationId2 = "6D9C947D-F694-4722-9DD0-5DEA32F9BD01";
            var TestOpenIdDummy = "7CEC4DC8-CA26-463E-ABB2-E5F599204C96";
            var TestVendorIdDummy = "14668D41-80E8-4F70-9637-C84545BA9D1E";
            var TestSystemIdDummy = "4D9988DD-16E3-4D8A-BF3D-1FDCF14C73CA";
            var openId = client.GetOpenId();

            // テストデータ登録
            var registerData = new List<ApplicationModel>
            { 
                new() {
                    ApplicationId = TestApplicationId1,
                    ApplicationName = TestApplicationId1,
                    IsEnable = true,
                    Manager = new [] { openId },
                    VendorId = TestVendorIdDummy,
                    SystemId = TestSystemIdDummy,
                },
                new() {
                    ApplicationId = TestApplicationId2,
                    ApplicationName = TestApplicationId2,
                    IsEnable = true,
                    Manager = new [] { TestOpenIdDummy },
                    VendorId = TestVendorIdDummy,
                    SystemId = TestSystemIdDummy,
                }
            };
            client.GetWebApiResponseResult(api.RegisterList(registerData)).Assert(RegisterSuccessExpectStatusCode);

            // テスト対象のFilterにアクセス
            var result = client.GetWebApiResponseResult(api.IsManager()).Assert(HttpStatusCode.OK);
            result.Result.Any(x=>x.ApplicationId == TestApplicationId1).IsTrue();
            result.Result.Any(x=>x.ApplicationId == TestApplicationId2).IsFalse();

            // 取得できないパターン
            client2.GetWebApiResponseResult(api.IsManager()).Assert(NotFoundStatusCode);
        }
    }
}
