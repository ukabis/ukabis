using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("AOP")]
    public class ODataOverPartitionFilterTest : ApiWebItTestCase
    {
        #region TestData

        private class ODataOverPartitionFilterTestData : TestDataBase
        {
            public List<AreaUnitModel> DataUserA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataExpectUserA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                }
            };

            public List<AreaUnitModel> DataUserB = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "DDD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "EEE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "FFF",
                    AreaUnitName = "fff",
                    ConversionSquareMeters = 6
                }
            };
            public List<AreaUnitModel> DataExpectUserB = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "DDD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "EEE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "FFF",
                    AreaUnitName = "fff",
                    ConversionSquareMeters = 6,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                }
            };

            public List<AreaUnitModel> DataExpectOverPartitionAll = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "DDD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "EEE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "FFF",
                    AreaUnitName = "fff",
                    ConversionSquareMeters = 6,
                    id = WILDCARD,
                    _Owner_Id = WILDCARD
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
        public void ODataOverPartitionFilterTest_NormalScenario()
        {
            var clientA = new IntegratedTestClient("test1");
            var clientB = new IntegratedTestClient("test2");
            var api = UnityCore.Resolve<IODataOverPartitionApi>();
            var testData = new ODataOverPartitionFilterTestData();

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            clientA.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
            clientB.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // UserAでデータを3件登録
            clientA.GetWebApiResponseResult(api.RegisterList(testData.DataUserA)).Assert(RegisterSuccessExpectStatusCode);

            // UserBでデータを3件登録
            clientB.GetWebApiResponseResult(api.RegisterList(testData.DataUserB)).Assert(RegisterSuccessExpectStatusCode);

            // UserAでOData実行
            clientA.GetWebApiResponseResult(api.OData("$orderby=ConversionSquareMeters")).Assert(GetSuccessExpectStatusCode, testData.DataExpectUserA);

            // UserBでOData実行
            clientB.GetWebApiResponseResult(api.OData("$orderby=ConversionSquareMeters")).Assert(GetSuccessExpectStatusCode, testData.DataExpectUserB);

            // UserAで領域越えOData実行
            clientA.GetWebApiResponseResult(api.ODataOverPartition("$orderby=ConversionSquareMeters")).Assert(GetSuccessExpectStatusCode); //, testData.DataExpectOverPartitionAll);

            // UserBで領域越えOData実行
            clientB.GetWebApiResponseResult(api.ODataOverPartition("$orderby=ConversionSquareMeters")).Assert(GetSuccessExpectStatusCode, testData.DataExpectOverPartitionAll);

            // UserABでそれぞれ絞り込み検索できるかどうか
            clientA.GetWebApiResponseResult(api.ODataOverPartition($"$orderby=ConversionSquareMeters&$filter=_Owner_Id eq '{clientA.GetOpenId()}'")).Assert(GetSuccessExpectStatusCode, testData.DataExpectUserA);
            clientB.GetWebApiResponseResult(api.ODataOverPartition($"$orderby=ConversionSquareMeters&$filter=_Owner_Id eq '{clientB.GetOpenId()}'")).Assert(GetSuccessExpectStatusCode, testData.DataExpectUserB);
        }
    }
}
