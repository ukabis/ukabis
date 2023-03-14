using System.Net;
using AutoMapper;
using IT.JP.DataHub.ODataOverPartition.Config;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.ODataOverPartition.WebApi;
using IT.JP.DataHub.ODataOverPartition.WebApi.Models;
using IT.JP.DataHub.SmartFoodChainAOP;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.UnitTest.Com.Extensions;

namespace IT.JP.DataHub.ODataOverPartition.TestCase
{
    [TestClass]
    public class ODataOverPartitionFilterTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        /// <summary>
        /// ODataOverPartitionのテスト
        /// /API/IntegratedTest/ODataOverPartitionを使用してのテスト
        /// （ODataOverPartitionメソッドに対してFilterが適用されてるようなのでテストは1回でいいはず）
        /// </summary>
        [TestMethod]
        public void ODataOverPartitionFilterTest_ODataOverPartitionScenario()
        {
            var client1 = new IntegratedTestClient(AppConfig.Account);
            var client2 = new IntegratedTestClient("test3");
            var api = UnityCore.Resolve<IODataOverPartitionApi>();

            var code = "__IntegratedTest";
            
            // テストデータ登録
            var registerData = new List<IntegratedTestSimpleDataModel>
            {
                new()
                {
                    AreaUnitCode = code,
                    AreaUnitName = code,
                    ConversionSquareMeters = 10
                }
            };
            client1.GetWebApiResponseResult(api.RegisterList(registerData)).Assert(RegisterSuccessExpectStatusCode);

            // 別のユーザーがODataOverPartitionで取得
            client2.GetWebApiResponseResult(api.ODataOverPartition(code)).Assert(HttpStatusCode.OK, registerData);

            // ODataでは取得できないことを確認
            client2.GetWebApiResponseResult(api.OData(code)).Assert(NotFoundStatusCode);

            // テストデータ削除
            client1.GetWebApiResponseResult(api.ODataDelete(code)).Assert(DeleteSuccessStatusCode);
        }
        
        /// <summary>
        /// ODataFullAccessのテスト
        /// /API/SmartFoodChain/V2/Private/PartyProductを使用してのテスト
        /// （ODataFullAccessメソッドに対してFilterが適用されてるようなのでテストは1回でいいはず）
        /// </summary>
        [TestMethod]
        public void ODataOverPartitionFilterTest_ODataFullAccessScenario()
        {
            var client1 = new IntegratedTestClient("test1");
            var client2 = new IntegratedTestClient("test3");
            var api = UnityCore.Resolve<IPartyProductApi>();
            var code = "__ODataTest";

            // データ登録
            var registerData = new PartyProductModel()
            {
                GtinCode = code,
                CodeType = "GS1-128",
                PartyId = "",
                IsOrganic = false,
                Profile = new PartyProductProfileModel()
                {
                    CropCode = "01010010"
                }
            };
            client1.GetWebApiResponseResult(api.Register(registerData)).Assert(RegisterSuccessExpectStatusCode);
            
            // 別のユーザーがODataOverPartitionで取得
            var result = client2.GetWebApiResponseResult(api.ODataFullAccess(code)).Assert(HttpStatusCode.OK, new List<PartyProductModel>{registerData});

            // ODataでは取得できないことを確認
            client2.GetWebApiResponseResult(api.OData(($"$filter=GtinCode eq '{code}'"))).Assert(NotFoundStatusCode);

            // テストデータ削除
            client1.GetWebApiResponseResult(api.Delete(code)).Assert(DeleteSuccessStatusCode);
        }
        

    }
}
