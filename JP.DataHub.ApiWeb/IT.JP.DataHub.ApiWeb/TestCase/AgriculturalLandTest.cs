using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("GeoJson")]
    public class AgriculturalLandTest : ApiWebItTestCase
    {
        #region TestData

        private class AgriculturalLandTestData : TestDataBase
        {
            public List<AgriculturalLandModel> TestData = new List<AgriculturalLandModel>
            {
                new AgriculturalLandModel()
                {
                    CityCode = "TestData-01",
                    Latitude = 1.000m,
                    Longitude = 1.000m,
                    GeoSearch = new AgriculturalLandGeoSearch()
                    {
                        type = "Point",
                        coordinates = new List<decimal>() { 1.000m, 1.000m }
                    }
                },
                new AgriculturalLandModel()
                {
                    CityCode = "TestData-02",
                    Latitude = 1.001m,
                    Longitude = 1.000m,
                    GeoSearch = new AgriculturalLandGeoSearch()
                    {
                        type = "Point",
                        coordinates = new List<decimal>() { 1.000m, 1.001m }
                    }
                },
                new AgriculturalLandModel()
                {
                    CityCode = "TestData-03",
                    Latitude = 1.001m,
                    Longitude = 1.001m,
                    GeoSearch = new AgriculturalLandGeoSearch()
                    {
                        type = "Point",
                        coordinates = new List<decimal>() { 1.001m, 1.001m }
                    }
                },
                new AgriculturalLandModel()
                {
                    CityCode = "TestData-04",
                    Latitude = 1.000m,
                    Longitude = 1.001m,
                    GeoSearch = new AgriculturalLandGeoSearch()
                    {
                        type = "Point",
                        coordinates = new List<decimal>() { 1.001m, 1.000m }
                    }
                }
            };

            public AgriculturalLandModel Expected1 = new AgriculturalLandModel()
            {
                CityCode = "TestData-02",
                Latitude = 1.001m,
                Longitude = 1.000m
            };

            public AgriculturalLandModel Expected2 = new AgriculturalLandModel()
            {
                CityCode = "TestData-03",
                Latitude = 1.001m,
                Longitude = 1.001m
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        //[DataRow(Repository.MongoDb)]
        public void AgriculturalLandTest_SearchByDistance(Repository repository)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // MongoDBとCosmosDBの距離の計算結果は完全には一致しないためある程度の誤差は許容するDistanceを指定している
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAgriculturalLandApi>();
            var testData = new AgriculturalLandTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // テストデータを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.TestData)).Assert(RegisterSuccessExpectStatusCode);

            // Hit(1件)
            client.GetWebApiResponseResult(api.SearchByDistanceSingle("1.0015", "1.000", "56")).Assert(GetSuccessExpectStatusCode, testData.Expected1);

            // Hit(2件)
            var response = client.GetWebApiResponseResult(api.SearchByDistance("1.0015", "1.000", "125")).Assert(GetSuccessExpectStatusCode);
            response.Result.OrderBy(x => x.CityCode).ToList().IsStructuralEqual(new List<AgriculturalLandModel>() { testData.Expected1, testData.Expected2 });

            // Missing
            client.GetWebApiResponseResult(api.SearchByDistance("1.0015", "1.000", "55")).Assert(NotFoundStatusCode);
        }
    }
}
