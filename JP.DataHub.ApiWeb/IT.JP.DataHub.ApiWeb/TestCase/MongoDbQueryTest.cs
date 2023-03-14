using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("Mongo系限定")]
    [TestClass]
    public class MongoDbQueryTest : ApiWebItTestCase
    {
        #region TestData

        private class MongoDbQueryTestData : TestDataBase
        {
            public List<MongoDbQueryModel> Data1 = new List<MongoDbQueryModel>()
            {
                new MongoDbQueryModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new MongoDbQueryModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 1
                },
                new MongoDbQueryModel()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 1
                },
                new MongoDbQueryModel()
                {
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 0.0003m
                },
                new MongoDbQueryModel()
                {
                    AreaUnitCode = "EE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 0.000299999999999999m
                }
            };

            public MongoDbQueryTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [DataTestMethod]
        [DataRow(Repository.MongoDb)]
        [DataRow(Repository.MongoDbCds)]
        public void MongoDbQuery_NormalScenario(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IMongoDbQueryApi>();
            var testData = new MongoDbQueryTestData(repository, api.ResourceUrl);

            // クリーンアップ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            var idsA = clientA.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterExpectStatusCodes).RawContentString;
            var idsB = clientB.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterExpectStatusCodes).RawContentString;

            // 検証: Aggregateでベンダー依存を無視してデータが検索されないか
            var result = clientA.GetWebApiResponseResult(api.GetByAggregate()).Assert(GetSuccessExpectStatusCode).Result;
            var ids = JArray.Parse(idsA);
            result.Count.Is(2);
            result.Single(x => x.AreaUnitCode == "BB").id.Is(ids.Single(x => x["id"].Value<string>().EndsWith("BB"))["id"].Value<string>());
            result.Single(x => x.AreaUnitCode == "CC").id.Is(ids.Single(x => x["id"].Value<string>().EndsWith("CC"))["id"].Value<string>());

            result = clientB.GetWebApiResponseResult(api.GetByAggregate()).Assert(GetSuccessExpectStatusCode).Result;
            ids = JArray.Parse(idsB);
            result.Count.Is(2);
            result.Single(x => x.AreaUnitCode == "BB").id.Is(ids.Single(x => x["id"].Value<string>().EndsWith("BB"))["id"].Value<string>());
            result.Single(x => x.AreaUnitCode == "CC").id.Is(ids.Single(x => x["id"].Value<string>().EndsWith("CC"))["id"].Value<string>());

            // 検証: 実数値での検索(OData)
            result = clientA.GetWebApiResponseResult(api.OData("$filter=ConversionSquareMeters eq 0.0003")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count.Is(1);
            result.Single().AreaUnitCode.Is("DD");

            result = clientA.GetWebApiResponseResult(api.OData("$filter=ConversionSquareMeters eq 0.000299999999999999")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count.Is(1);
            result.Single().AreaUnitCode.Is("EE");

            // 検証: 実数値での検索(ApiQuery)
            result = clientA.GetWebApiResponseResult(api.SearchDecimalByApiQuery("0.0003")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count.Is(1);
            result.Single().AreaUnitCode.Is("DD");

            result = clientA.GetWebApiResponseResult(api.SearchDecimalByApiQuery("0.000299999999999999")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count.Is(1);
            result.Single().AreaUnitCode.Is("EE");

            // 検証: 実数値での検索(Aggregate)
            result = clientA.GetWebApiResponseResult(api.SearchDecimalByAggregate("0.0003")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count.Is(1);
            result.Single().AreaUnitCode.Is("DD");

            result = clientA.GetWebApiResponseResult(api.SearchDecimalByAggregate("0.000299999999999999")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count.Is(1);
            result.Single().AreaUnitCode.Is("EE");
        }
    }
}
