using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("Mongo系限定")]
    [TestClass]
    public class ContainerDynamicSeparationTest : ApiWebItTestCase
    {
        #region TestData

        private class ContainerDynamicSeparationTestData : TestDataBase
        {
            public List<AreaUnitModel> DataA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "A1",
                    AreaUnitName = "2A1",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "A2",
                    AreaUnitName = "2A2",
                    ConversionSquareMeters = 2
                }
            };
            public AreaUnitModel DataGetA1 = new AreaUnitModel()
            {
                AreaUnitCode = "A1",
                AreaUnitName = "2A1",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~ContainerDynamicSeparation~{WILDCARD}~A1",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel DataGetA2 = new AreaUnitModel()
            {
                AreaUnitCode = "A2",
                AreaUnitName = "2A2",
                ConversionSquareMeters = 2,
                id = $"API~IntegratedTest~ContainerDynamicSeparation~{WILDCARD}~A2",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel DataSelectedA1 = new AreaUnitModel()
            {
                AreaUnitCode = "A1"
            };
            public AreaUnitModel DataSelectedA2 = new AreaUnitModel()
            {
                AreaUnitCode = "A2"
            };

            public List<AreaUnitModel> DataB = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "B1",
                    AreaUnitName = "3B1",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "B2",
                    AreaUnitName = "3B2",
                    ConversionSquareMeters = 2
                }
            };
            public AreaUnitModel DataGetB1 = new AreaUnitModel()
            {
                AreaUnitCode = "B1",
                AreaUnitName = "3B1",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~ContainerDynamicSeparation~{WILDCARD}~B1",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel DataGetB2 = new AreaUnitModel()
            {
                AreaUnitCode = "B2",
                AreaUnitName = "3B2",
                ConversionSquareMeters = 2,
                id = $"API~IntegratedTest~ContainerDynamicSeparation~{WILDCARD}~B2",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel DataSelectedB1 = new AreaUnitModel()
            {
                AreaUnitCode = "B1"
            };
            public AreaUnitModel DataSelectedB2 = new AreaUnitModel()
            {
                AreaUnitCode = "B2"
            };

            public List<AreaUnitModel> DataC = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "C1",
                    AreaUnitName = "1C1",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "C2",
                    AreaUnitName = "1C2",
                    ConversionSquareMeters = 2
                }
            };
            public AreaUnitModel DataGetC1 = new AreaUnitModel()
            {
                AreaUnitCode = "C1",
                AreaUnitName = "1C1",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~ContainerDynamicSeparation~{WILDCARD}~C1",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel DataGetC2 = new AreaUnitModel()
            {
                AreaUnitCode = "C2",
                AreaUnitName = "1C2",
                ConversionSquareMeters = 2,
                id = $"API~IntegratedTest~ContainerDynamicSeparation~{WILDCARD}~C2",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel DataSelectedC1 = new AreaUnitModel()
            {
                AreaUnitCode = "C1"
            };
            public AreaUnitModel DataSelectedC2 = new AreaUnitModel()
            {
                AreaUnitCode = "C2"
            };


            public ContainerDynamicSeparationTestData(string resourceUrl) : base(Repository.MongoDbCds, resourceUrl, true) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void ContainerDynamicSeparationTest_VendorPrivateScenario()
        {
            var api = UnityCore.Resolve<IContainerDynamicSeparationApi>();
            var testData = new ContainerDynamicSeparationTestData(api.ResourceUrl);

            var clientA = new IntegratedTestClient("test1", "SmartFoodChain2TestSystem") { TargetRepository = Repository.MongoDbCds };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChainPortal") { TargetRepository = Repository.MongoDbCds };
            var clientC = new IntegratedTestClient("test1", "SmartFoodChainPortal") { TargetRepository = Repository.MongoDbCds };

            // クリーンアップ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 別ベンダーで同じIDのデータを登録しても別々のコンテナに保管される
            // Register
            clientA.GetWebApiResponseResult(api.RegistList(testData.DataA)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegistList(testData.DataB)).Assert(RegisterSuccessExpectStatusCode);
            clientC.GetWebApiResponseResult(api.RegistList(testData.DataC)).Assert(RegisterSuccessExpectStatusCode);

            // Query
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataGetA1, testData.DataGetA2 });
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataGetB1, testData.DataGetB2, testData.DataGetC1, testData.DataGetC2 });
            clientC.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataGetB1, testData.DataGetB2, testData.DataGetC1, testData.DataGetC2 });

            // OData
            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataGetA1, testData.DataGetA2 });
            clientB.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataGetB1, testData.DataGetB2, testData.DataGetC1, testData.DataGetC2 });
            clientC.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataGetB1, testData.DataGetB2, testData.DataGetC1, testData.DataGetC2 });

            // 領域越え
            // 全件
            var expected = new List<AreaUnitModel>() { testData.DataGetA1, testData.DataGetA2, testData.DataGetB1, testData.DataGetB2, testData.DataGetC1, testData.DataGetC2 };
            var array = clientA.GetWebApiResponseResult(api.GetAllOverPartition()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(expected);
            array = clientB.GetWebApiResponseResult(api.GetAllOverPartition()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(expected);
            array = clientC.GetWebApiResponseResult(api.GetAllOverPartition()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(expected);

            // 別コンテナ1件
            clientA.GetWebApiResponseResult(api.GetOverPartition("B2")).Assert(GetSuccessExpectStatusCode, testData.DataGetB2);
            clientB.GetWebApiResponseResult(api.GetOverPartition("A1")).Assert(GetSuccessExpectStatusCode, testData.DataGetA1);
            clientC.GetWebApiResponseResult(api.GetOverPartition("A2")).Assert(GetSuccessExpectStatusCode, testData.DataGetA2);

            // ODataクエリ: $filter=ConversionSquareMeters eq 2&$orderby=AreaUnitName&$top=2&$select=AreaUnitCode
            expected = new List<AreaUnitModel>() { testData.DataSelectedC2, testData.DataSelectedA2 };
            clientA.GetWebApiResponseResult(api.GetByODataQueryOverPartition()).Assert(GetSuccessExpectStatusCode, expected);
            clientB.GetWebApiResponseResult(api.GetByODataQueryOverPartition()).Assert(GetSuccessExpectStatusCode, expected);
            clientC.GetWebApiResponseResult(api.GetByODataQueryOverPartition()).Assert(GetSuccessExpectStatusCode, expected);

            // APIクエリ(内容はODataクエリと同等)
            clientA.GetWebApiResponseResult(api.GetByApiQueryOverPartition()).Assert(GetSuccessExpectStatusCode, expected);
            clientB.GetWebApiResponseResult(api.GetByApiQueryOverPartition()).Assert(GetSuccessExpectStatusCode, expected);
            clientC.GetWebApiResponseResult(api.GetByApiQueryOverPartition()).Assert(GetSuccessExpectStatusCode, expected);

            // APIクエリ(Aggregate、$sort/$limitもコンテナ単位になるため結果が異なる)
            expected = new List<AreaUnitModel>() { testData.DataSelectedA2, testData.DataSelectedC2 };
            array = clientA.GetWebApiResponseResult(api.GetByAggregateOverPartition()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(expected);
            array = clientB.GetWebApiResponseResult(api.GetByAggregateOverPartition()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(expected);
            array = clientC.GetWebApiResponseResult(api.GetByAggregateOverPartition()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(expected);

            // バージョンは共通コンテナのため全体に影響する
            // 現在バージョン比較
            var versionA = clientA.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            var prevVersion = versionA.CurrentVersion;
            var versionB = clientB.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            versionA.IsStructuralEqual(versionB);
            var versionC = clientC.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            versionA.IsStructuralEqual(versionC);

            // 片方でバージョンアップ後にバージョン比較
            clientA.GetWebApiResponseResult(api.SetNewVersion()).Assert(RegisterSuccessExpectStatusCode);
            versionA = clientA.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            versionA.CurrentVersion.Is(prevVersion + 1);
            versionB = clientB.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            versionA.IsStructuralEqual(versionB);
            versionC = clientC.GetWebApiResponseResult(api.GetCurrentVersion()).Assert(GetSuccessExpectStatusCode).Result;
            versionA.IsStructuralEqual(versionC);
        }
    }
}
