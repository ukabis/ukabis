using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class UpdateTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void UpdateTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var updateApi = UnityCore.Resolve<IUpdateApi>();

            // 最初に全データの消去
            client.GetWebApiResponseResult(updateApi.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(updateApi.GetAll()).Assert(GetErrorExpectStatusCode);

            // データ1を１件登録
            var data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            var expected1 = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~UpdateApi~1~AA"
            };
            client.GetWebApiResponseResult(updateApi.Regist(data1)).Assert(RegisterSuccessExpectStatusCode, expected1);

            // 登録した１件を取得
            var expected2 = new AreaUnitModel()
            {
                id = expected1.id,
                AreaUnitCode = data1.AreaUnitCode,
                AreaUnitName = data1.AreaUnitName,
                ConversionSquareMeters = data1.ConversionSquareMeters,
                _Owner_Id = WILDCARD
            };
            client.GetWebApiResponseResult(updateApi.Get(data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, expected2);

            // 配列(1件)でUpdate
            var data2 = new List<AreaUnitModel>()
            {
                new ()
                {
                    AreaUnitCode = data1.AreaUnitCode,
                    AreaUnitName = "a2",
                    ConversionSquareMeters = 2
                }
            };
            client.GetWebApiResponseResult(updateApi.Update(data1.AreaUnitCode, data2)).AssertErrorCode(BadRequestStatusCode, "E10409");

            //配列(複数件)でUpdate
            var data3 = new List<AreaUnitModel>()
            {
                new ()
                {
                    AreaUnitCode = data1.AreaUnitCode,
                    AreaUnitName = "a2",
                    ConversionSquareMeters = 2
                },
                new ()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                }
            };
            client.GetWebApiResponseResult(updateApi.Update(data1.AreaUnitCode, data3)).Assert(BadRequestStatusCode);

            // 1件Update
            var data4 = new AreaUnitModel()
            {
                AreaUnitCode = data1.AreaUnitCode,
                AreaUnitName = "a2",
                ConversionSquareMeters = 2
            };
            client.GetWebApiResponseResult(updateApi.Update(data1.AreaUnitCode, data4)).Assert(UpdateSuccessExpectStatusCode);

            // Updateしたデータを取得
            var expected3 = new AreaUnitModel()
            {
                id = expected1.id,
                AreaUnitCode = data4.AreaUnitCode,
                AreaUnitName = data4.AreaUnitName,
                ConversionSquareMeters = data4.ConversionSquareMeters,
                _Owner_Id = WILDCARD
            };
            client.GetWebApiResponseResult(updateApi.Get(data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, expected3);
        }
    }
}
