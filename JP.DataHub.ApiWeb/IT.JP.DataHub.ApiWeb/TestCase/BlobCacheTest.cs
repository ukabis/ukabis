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
    public class BlobCacheTest : ApiWebItTestCase
    {
        #region TestData

        private class BlobCacheTestData : TestDataBase
        {
            public List<AreaUnitModelEx> Data1 = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    TestNum = 1
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    TestNum = 2
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    TestNum = 3
                }
            };
            public List<RegisterResponseModel> Data1RegistExpected = new List<RegisterResponseModel>()
            {
                new RegisterResponseModel()
                {
                    id = $"API~IntegratedTest~BlobCacheApi~1~AA"
                },
                new RegisterResponseModel()
                {
                    id = $"API~IntegratedTest~BlobCacheApi~1~BB"
                },
                new RegisterResponseModel()
                {
                    id = $"API~IntegratedTest~BlobCacheApi~1~CC"
                }
            };
            public AreaUnitModelEx Data1Get = new AreaUnitModelEx()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                TestNum = 2,
                id = "API~IntegratedTest~BlobCacheApi~1~BB",
                _Owner_Id = WILDCARD
            };

            public List<AreaUnitModelEx> Data2Get = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    TestNum = 1,
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    TestNum = 2,
                }
            };

            public List<AreaUnitModelEx> Data3Get = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    TestNum = 2,
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    TestNum = 3,
                }
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void BlobCacheTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IBlobCacheApi>();
            var testData = new BlobCacheTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // データ1を登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 取得(CosmosDBからの取得)
            // AAは以前使ったのでリリース時にキャッシュ消す必要がある
            var context1 = client.GetWebApiResponseResult(api.Get("BB")).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;

            // 取得(Blobからの取得)
            var context2 = client.GetWebApiResponseResult(api.Get("BB")).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;
            context1.IsStructuralEqual(context2);

            // モデルにない項目で検索
            // 取得(CosmosDBからの取得)
            var context3 = client.GetWebApiResponseResult(api.GetByMinMax(1, 2)).Assert(GetSuccessExpectStatusCode, testData.Data2Get).Result;

            // 取得(Blobからの取得)
            var context4 = client.GetWebApiResponseResult(api.GetByMinMax(1, 2)).Assert(GetSuccessExpectStatusCode, testData.Data2Get).Result;
            context3.IsStructuralEqual(context4);

            // モデルにない項目で検索
            // 取得(CosmosDBからの取得)
            var context5 = client.GetWebApiResponseResult(api.GetByMinMax(2, 3)).Assert(GetSuccessExpectStatusCode, testData.Data3Get).Result;

            // 取得(Blobからの取得)
            var context6 = client.GetWebApiResponseResult(api.GetByMinMax(2, 3)).Assert(GetSuccessExpectStatusCode, testData.Data3Get).Result;
            context5.IsStructuralEqual(context6);
        }
    }
}
