using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class QueryODataTest : ApiWebItTestCase
    {
        #region TestData

        private class QueryODataTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~QueryODataTest";

            public List<AreaUnitModelEx> Data1 = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "EE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5
                }
            };

            public List<AreaUnitModelEx> Data1_NormalQuery_GetExpected = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    id = "API~IntegratedTest~QueryODataTest~1~AA",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModelEx()
                {
                    id = "API~IntegratedTest~QueryODataTest~1~BB",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModelEx()
                {
                    id = "API~IntegratedTest~QueryODataTest~1~CC",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModelEx()
                {
                    id = "API~IntegratedTest~QueryODataTest~1~DD",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                }
            };

            public AreaUnitModelEx Data1_WithRepositoryKeyQuery_Expected = new AreaUnitModelEx()
            {
                id = "API~IntegratedTest~QueryODataTest~1~AA",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModelEx Data2 = new AreaUnitModelEx()
            {
                id = "API~IntegratedTest~QueryODataTest~1~BB",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 2
            };

            public AreaUnitModelEx Data3 = new AreaUnitModelEx()
            {
                id = "API~IntegratedTest~QueryODataTest~1~CC",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "CC",
                AreaUnitName = "ccc",
                ConversionSquareMeters = 3
            };

            public AreaUnitModelEx Data4 = new AreaUnitModelEx()
            {
                AreaUnitCode = "FunctionTest",
                AreaUnitCodeLower = "functiontest",
                AreaUnitCodeUpper = "FUNCTIONTEST",
                AreaUnitName = "   FunctionTest   ",
                IntValue = 12m,
                DoubleValue1 = 0.4m,
                DoubleValue2 = 0.6m,
                GeoSearch = new GeoJsonPointGeometry()
                {
                    type = "Point",
                    coordinates = new List<decimal>() { 1.000m, 1.000m }
                }
            };

            public QueryODataTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, true, client: client) { }
        }

        private class QueryODataEscapeTestData : TestDataBase
        {
            public List<AreaUnitModel> Data1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "D1",
                    AreaUnitName = "d\'\'",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "D2",
                    AreaUnitName = "d\'",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "D3",
                    AreaUnitName = "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~",
                    ConversionSquareMeters = 3
                }
            };

            public List<AreaUnitModel> Data1_GetExpected1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~QueryODataEscapeTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D1",
                    AreaUnitName = "d\'\'",
                    ConversionSquareMeters = 1
                }
            };

            public List<AreaUnitModel> Data1_GetExpected2 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~QueryODataEscapeTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D2",
                    AreaUnitName = "d\'",
                    ConversionSquareMeters = 2
                }
            };

            public List<AreaUnitModel> Data1_ExpectedAfterDelete1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~QueryODataEscapeTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D1",
                    AreaUnitName = "d\'\'",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~QueryODataEscapeTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D3",
                    AreaUnitName = "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~",
                    ConversionSquareMeters = 3
                }
            };

            public List<AreaUnitModel> Data1_GetExpected3 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~QueryODataEscapeTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D3",
                    AreaUnitName = "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~",
                    ConversionSquareMeters = 3
                }
            };

            public QueryODataEscapeTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        private class ODataFilterPatternTestData : TestDataBase
        {
            public ODataPatchModel Data1_1 = new ODataPatchModel()
            {
                STR_VALUE = "AA",
                STR_NULL = "AA",
                INT_VALUE = 1,
                DBL_VALUE = 1.1m,
                OBJ_VALUE = new ODataPatchObject() { key1 = "value" },
                ARY_VALUE = new List<string>() { "value1", "value2" },
                BOL_VALUE = true,
                DAT_VALUE = "2021-01-02T12:13:14"
            };

            public ODataPatchModel Data1_2 = new ODataPatchModel()
            {
                STR_VALUE = "BB",
                INT_VALUE = 2,
                DBL_VALUE = 2.2m,
                OBJ_VALUE = new ODataPatchObject() { key1 = "value" },
                ARY_VALUE = new List<string>() { "value1", "value2" },
                BOL_VALUE = false,
                DAT_VALUE = "2021-01-02T12:13:14"
            };

            public ODataPatchModel Data1_3 = new ODataPatchModel()
            {
                STR_VALUE = "CC",
                STR_NULL = "CC",
                INT_VALUE = 3,
                DBL_VALUE = 3.3m,
                OBJ_VALUE = new ODataPatchObject() { key1 = "value" },
                ARY_VALUE = new List<string>() { "value1", "value2" },
                BOL_VALUE = true,
                DAT_VALUE = "2021-01-02T12:13:14"
            };

            public ODataFilterPatternTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }


        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void QueryODataTest_NormalScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataApi>();
            var testData = new QueryODataTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // データを5件登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // キー無しクエリで、データをGET(メソッドのクエリ設定に、$top=4を指定している)
            client.GetWebApiResponseResult(api.QueryODataGetNormal()).Assert(GetSuccessExpectStatusCode, testData.Data1_NormalQuery_GetExpected);

            // キーありクエリで、データをGET(メソッドのクエリ設定に、$top=4を指定している)
            client.GetWebApiResponseResult(api.QueryODataGetWithRepositoryKey("AA", "aaa")).Assert(GetSuccessExpectStatusCode, testData.Data1_WithRepositoryKeyQuery_Expected);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void QueryODataTest_ODataFunctionSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = (repository == Repository.SqlServer ? UnityCore.Resolve<IQueryODataFunctionApi>() : UnityCore.Resolve<IQueryODataApi>());
            var testData = new QueryODataTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録
            client.GetWebApiResponseResult(api.RegistList(new List<AreaUnitModelEx>() { testData.Data4 })).Assert(RegisterSuccessExpectStatusCode);

            /* contains */
            // true
            var result = client.GetWebApiResponseResult(api.OData("$filter=contains(AreaUnitCode, 'tion')")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=contains(AreaUnitCode, 'xxx')")).Assert(NotFoundStatusCode);

            // 引数にプロパティ
            // MongoDBは第二引数定数のみ
            if (repository == Repository.CosmosDb)
            {
                result = client.GetWebApiResponseResult(api.OData("$filter=contains(AreaUnitName, AreaUnitCode)")).Assert(GetSuccessExpectStatusCode).Result;
                result.Count().Is(1);
            }

            /* endswith */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=endswith(AreaUnitCode, 'est')")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=endswith(AreaUnitCode, 'Tes')")).Assert(NotFoundStatusCode);

            // 引数にプロパティ
            // MongoDBは第二引数定数のみ
            if (repository == Repository.CosmosDb)
            {
                client.GetWebApiResponseResult(api.OData("$filter=endswith(AreaUnitName, AreaUnitCode)")).Assert(NotFoundStatusCode);
            }

            /* startswith */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=startswith(AreaUnitCode, 'Fu')")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=startswith(AreaUnitCode, 'un')")).Assert(NotFoundStatusCode);

            // 引数にプロパティ
            // MongoDBは第二引数定数のみ
            if (repository == Repository.CosmosDb)
            {
                client.GetWebApiResponseResult(api.OData("$filter=startswith(AreaUnitName, AreaUnitCode)")).Assert(NotFoundStatusCode);
            }

            /* length */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=length(AreaUnitCode) eq 12")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=length(AreaUnitCode) eq 11")).Assert(NotFoundStatusCode);

            // プロパティ比較
            result = client.GetWebApiResponseResult(api.OData("$filter=length(AreaUnitCode) eq IntValue")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=IntValue eq length(AreaUnitCode)")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            /* indexof */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=indexof(AreaUnitCode, 'un') eq 1")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=indexof(AreaUnitCode, 'un') eq 2")).Assert(NotFoundStatusCode);

            // 引数にプロパティ
            result = client.GetWebApiResponseResult(api.OData("$filter=indexof(AreaUnitName, AreaUnitCode) eq 3")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // プロパティ比較
            client.GetWebApiResponseResult(api.OData("$filter=indexof(AreaUnitCode, 'un') eq IntValue")).Assert(NotFoundStatusCode);

            client.GetWebApiResponseResult(api.OData("$filter=IntValue eq indexof(AreaUnitCode, 'un')")).Assert(NotFoundStatusCode);

            /* substring */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=substring(AreaUnitCode, 1) eq 'unctionTest'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=substring(AreaUnitCode, 1, 10) eq 'unctionTes'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=substring(AreaUnitCode, 1) eq 'unctionTes'")).Assert(NotFoundStatusCode);

            client.GetWebApiResponseResult(api.OData("$filter=substring(AreaUnitCode, 1, 10) eq 'unctionTest'")).Assert(NotFoundStatusCode);

            // 引数にプロパティ
            result = client.GetWebApiResponseResult(api.OData("$filter=substring(AreaUnitName, IntValue, 3) eq 'est'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // プロパティ比較
            client.GetWebApiResponseResult(api.OData("$filter=substring(AreaUnitCode, 1, 10) eq AreaUnitCode")).Assert(NotFoundStatusCode);

            client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCode eq substring(AreaUnitCode, 1, 10)")).Assert(NotFoundStatusCode);

            /* tolower */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=tolower(AreaUnitCode) eq 'functiontest'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            // SQLServerは大文字小文字が区別されないためスキップ
            if (repository != Repository.SqlServer)
            {
                client.GetWebApiResponseResult(api.OData("$filter=tolower(AreaUnitCode) eq 'FunctionTest'")).Assert(NotFoundStatusCode);
            }

            // プロパティ比較
            result = client.GetWebApiResponseResult(api.OData("$filter=tolower(AreaUnitCode) eq AreaUnitCodeLower")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCodeLower eq tolower(AreaUnitCode)")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            /* toupper */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=toupper(AreaUnitCode) eq 'FUNCTIONTEST'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            // SQLServerは大文字小文字が区別されないためスキップ
            if (repository != Repository.SqlServer)
            {
                client.GetWebApiResponseResult(api.OData("$filter=toupper(AreaUnitCode) eq 'FunctionTest'")).Assert(NotFoundStatusCode);
            }

            // プロパティ比較
            result = client.GetWebApiResponseResult(api.OData("$filter=toupper(AreaUnitCode) eq AreaUnitCodeUpper")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCodeUpper eq toupper(AreaUnitCode)")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            /* trim */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=trim(AreaUnitName) eq 'FunctionTest'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            // SQLServerは末尾の空白が無視されるためスキップ
            if (repository != Repository.SqlServer)
            {
                client.GetWebApiResponseResult(api.OData("$filter=trim(AreaUnitName) eq 'FunctionTest '")).Assert(NotFoundStatusCode);
            }

            // プロパティ比較
            result = client.GetWebApiResponseResult(api.OData("$filter=trim(AreaUnitName) eq AreaUnitCode")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCode eq trim(AreaUnitName)")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            /* concat */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=concat(AreaUnitCode, '2') eq 'FunctionTest2'")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=concat('2', AreaUnitCode) eq 'FunctionTest2'")).Assert(NotFoundStatusCode);

            // プロパティ比較
            result = client.GetWebApiResponseResult(api.OData("$filter=concat('Function', 'Test') eq AreaUnitCode")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCode eq concat('Function', 'Test')")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            /* round */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=round(DoubleValue1) eq 0")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            result = client.GetWebApiResponseResult(api.OData("$filter=round(DoubleValue2) eq 1")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=round(DoubleValue1) eq 1")).Assert(NotFoundStatusCode);

            // プロパティ比較
            client.GetWebApiResponseResult(api.OData("$filter=round(DoubleValue1) eq IntValue")).Assert(NotFoundStatusCode);

            client.GetWebApiResponseResult(api.OData("$filter=IntValue eq round(DoubleValue1)")).Assert(NotFoundStatusCode);

            /* floor */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=floor(DoubleValue2) eq 0")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=floor(DoubleValue2) eq 1")).Assert(NotFoundStatusCode);

            // プロパティ比較
            client.GetWebApiResponseResult(api.OData("$filter=floor(DoubleValue2) eq IntValue")).Assert(NotFoundStatusCode);

            client.GetWebApiResponseResult(api.OData("$filter=IntValue eq floor(DoubleValue2)")).Assert(NotFoundStatusCode);

            /* ceiling */
            // true
            result = client.GetWebApiResponseResult(api.OData("$filter=ceiling(DoubleValue1) eq 1")).Assert(GetSuccessExpectStatusCode).Result;
            result.Count().Is(1);

            // false
            client.GetWebApiResponseResult(api.OData("$filter=ceiling(DoubleValue1) eq 0")).Assert(NotFoundStatusCode);

            // プロパティ比較
            client.GetWebApiResponseResult(api.OData("$filter=ceiling(DoubleValue1) eq IntValue")).Assert(NotFoundStatusCode);

            client.GetWebApiResponseResult(api.OData("$filter=IntValue eq ceiling(DoubleValue1)")).Assert(NotFoundStatusCode);

            /* geo.distance */
            // MongoDBは2dsphereindexがないと動作しないためスキップ
            if (repository == Repository.CosmosDb)
            {
                // true
                result = client.GetWebApiResponseResult(api.OData("$filter=geo.distance(GeoSearch, geography'POINT(1 1)') eq 0")).Assert(GetSuccessExpectStatusCode).Result;
                result.Count().Is(1);

                // false
                client.GetWebApiResponseResult(api.OData("$filter=geo.distance(GeoSearch, geography'POINT(1 1)') eq 100")).Assert(NotFoundStatusCode);
            }

            /* geo.intersects */
            // SQLServerは地理情報検索非対応のためスキップ
            if (repository != Repository.SqlServer)
            {
                // true
                result = client.GetWebApiResponseResult(api.OData("$filter=geo.intersects(GeoSearch, geography'POLYGON((1 1, 10 1, 10 10, 1 10, 1 1))')")).Assert(GetSuccessExpectStatusCode).Result;
                result.Count().Is(1);

                // false
                client.GetWebApiResponseResult(api.OData("$filter=geo.intersects(GeoSearch, geography'POLYGON((2 2, 10 2, 10 10, 2 10, 2 2))')")).Assert(NotFoundStatusCode);
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void QueryODataTest_ODataEscapeSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataEscapeApi>();
            var testData = new QueryODataEscapeTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データを3件登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // エスケープありで検索(シングルクォート)
            client.GetWebApiResponseResult(api.OData("$filter=AreaUnitName eq 'd''' and concat(AreaUnitName, '''') eq 'd'''''")).Assert(GetSuccessExpectStatusCode, testData.Data1_GetExpected2);

            // エスケープありで削除(シングルクォート)
            client.GetWebApiResponseResult(api.ODataDelete("$filter=AreaUnitName eq 'd''' and concat(AreaUnitName, '''') eq 'd'''''")).Assert(DeleteSuccessStatusCode);

            // 削除結果を確認
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data1_ExpectedAfterDelete1);

            // 記号全部入りで検索
            client.GetWebApiResponseResult(api.OData($"$filter=AreaUnitName eq '{HttpUtility.UrlEncode("! \"#$%&''()*+,-./:;<=>?@[\\]^_`{|}~")}'")).Assert(GetSuccessExpectStatusCode, testData.Data1_GetExpected3);

            // 記号全部入りで検索(ODataクエリ)
            client.GetWebApiResponseResult(api.GetByODataQuery()).Assert(GetSuccessExpectStatusCode, testData.Data1_GetExpected3);

            // 記号全部入りで削除
            client.GetWebApiResponseResult(api.ODataDelete($"$filter=AreaUnitName eq '{HttpUtility.UrlEncode("! \"#$%&''()*+,-./:;<=>?@[\\]^_`{|}~")}'")).Assert(DeleteSuccessStatusCode);

            // 削除結果を確認
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data1_GetExpected1);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public async Task QueryODataTest_ODataFilterPatternSenario(Repository repository)
        {
            var clientA = new IntegratedTestClient("test1") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2") { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataFilterPatternApi>();
            var testData = new ODataFilterPatternTestData(repository, api.ResourceUrl);

            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            const int waitTime = 2000;
            string strTime;
            string result;

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1_1)).Assert(RegisterSuccessExpectStatusCode);

            await Task.Delay(waitTime);
            var time1 = DateTime.UtcNow;

            // データを登録
            clientB.GetWebApiResponseResult(api.Regist(testData.Data1_2)).Assert(RegisterSuccessExpectStatusCode);

            await Task.Delay(waitTime);
            var time2 = DateTime.UtcNow;

            // データを登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1_3)).Assert(RegisterSuccessExpectStatusCode);

            List<ODataPatchModel> array;
            if (!IsIgnoreGetInternalAllField)
            {
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

                // 管理項目による検索(_Regdate:YYYY-MM-DD HH:MM:SS形式)
                strTime = time1.ToString("yyyy-MM-dd HH:mm:ss");
                array = clientA.GetWebApiResponseResult(api.OData($"$filter=_Regdate lt '{strTime}'")).Assert(GetSuccessExpectStatusCode).Result;
                array.Count.Is(1);
                var userId = array[0]._Reguser_Id;

                // 管理項目による検索(_Reguser_Id)
                array = clientA.GetWebApiResponseResult(api.OData($"$filter=_Reguser_Id eq '{userId}'")).Assert(GetSuccessExpectStatusCode).Result;
                array.Count.Is(2);

                // 管理項目による検索(_Upddate:YYYY-MM-DDTHH:MM:SS形式)
                strTime = time2.ToString("yyyy-MM-ddTHH:mm:ss");
                array = clientA.GetWebApiResponseResult(api.OData($"$filter=_Upddate lt '{strTime}'&$orderby=_Upddate")).Assert(GetSuccessExpectStatusCode).Result;
                array.Count.Is(2);
                userId = array[1]._Upduser_Id;

                // 管理項目による検索(_Upduser_Id)
                array = clientA.GetWebApiResponseResult(api.OData($"$filter=_Upduser_Id eq '{userId}'")).Assert(GetSuccessExpectStatusCode).Result;
                array.Count.Is(1);
            }

            // nullでの検索
            array = clientA.GetWebApiResponseResult(api.OData($"$filter=STR_NULL eq null")).Assert(GetSuccessExpectStatusCode).Result;
            array.Count.Is(1);
            array[0].STR_VALUE.Is("BB");

            // boolでの検索
            array = clientA.GetWebApiResponseResult(api.OData($"$filter=BOL_VALUE eq false")).Assert(GetSuccessExpectStatusCode).Result;
            array.Count.Is(1);
            array[0].STR_VALUE.Is("BB");

            // notでの検索
            array = clientA.GetWebApiResponseResult(api.OData($"$filter=not BOL_VALUE")).Assert(GetSuccessExpectStatusCode).Result;
            array.Count.Is(1);
            array[0].STR_VALUE.Is("BB");
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void QueryODataTest_ODataFilterValidationErrorSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataFilterPatternApi>();

            // AdditionalProperties
            client.GetWebApiResponseResult(api.OData($"$filter=Hoge lt 'Fuga'")).AssertErrorCode(GetErrorExpectStatusCode, "E10422");

            // StringにNumber
            var detail = client.GetWebApiResponseResult(api.OData($"$filter=STR_VALUE eq 123")).AssertErrorCode(BadRequestStatusCode, "E10426").RawContentString.ToJson();
            detail["errors"].FindProperty("STR_VALUE").IsNotNull();

            // DateTimeにString
            detail = client.GetWebApiResponseResult(api.OData($"$filter=_Regdate lt 'Hoge'")).AssertErrorCode(BadRequestStatusCode, "E10426").RawContentString.ToJson();
            detail["errors"].FindProperty("_Regdate").IsNotNull();

            // NumberにString
            detail = client.GetWebApiResponseResult(api.OData($"$filter=INT_VALUE eq 'Hoge'")).AssertErrorCode(BadRequestStatusCode, "E10426").RawContentString.ToJson();
            detail["errors"].FindProperty("INT_VALUE").IsNotNull();
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void QueryODataTest_ODataContinuationSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataApi>();
            var testData = new QueryODataTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを5件登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // ページングを有効にする
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, " ");

            // 2件ずつ取得 Orderby なし　順不同なので件数のみチェック
            var result = client.GetWebApiResponseResult(api.OData("$top=2")).Assert(GetSuccessExpectStatusCode);
            result.Result.Count.Is(2);

            api.AddHeaders.Remove(HeaderConst.X_RequestContinuation);
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First());
            result = client.GetWebApiResponseResult(api.OData("$top=2")).Assert(GetSuccessExpectStatusCode);
            result.Result.Count().Is(2);

            api.AddHeaders.Remove(HeaderConst.X_RequestContinuation);
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First());
            result = client.GetWebApiResponseResult(api.OData("$top=2")).Assert(GetSuccessExpectStatusCode);
            result.Result.Count().Is(1);
            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");

            // 2件ずつ取得 絞り込みあり
            api.AddHeaders.Remove(HeaderConst.X_RequestContinuation);
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, " ");
            result = client.GetWebApiResponseResult(api.OData("$top=1&$orderby=AreaUnitCode&$filter=ConversionSquareMeters ge 2 and ConversionSquareMeters le 3")).Assert(GetSuccessExpectStatusCode);
            result.Result.Count().Is(1);
            result.Result[0].IsStructuralEqual(testData.Data2);

            api.AddHeaders.Remove(HeaderConst.X_RequestContinuation);
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First());
            result = client.GetWebApiResponseResult(api.OData("$top=1&$orderby=AreaUnitCode&$filter=ConversionSquareMeters ge 2 and ConversionSquareMeters le 3")).Assert(GetSuccessExpectStatusCode);
            result.Result.Count().Is(1);
            result.Result[0].IsStructuralEqual(testData.Data3);

            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");
        }

        /**
         * top指定なし、top任意設定の場合、デフォルト100件でページング処理される。
         */
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void QueryODataTest_ODataContinuationSenario_topCount_null_XRequestContinuationNeedsTopCount_false(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataApi>();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを101件登録(デフォルト100件でページング)
            client.GetWebApiResponseResult(api.RegistList(Data101())).Assert(RegisterSuccessExpectStatusCode);

            //ページングを有効にする
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, " ");

            // 取得 Orderby なし　top なし　順不同なので件数のみチェック
            var result = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode);
            result.Result.Count.Is(100); // top指定なしでページング処理される（デフォルト100件）

            api.AddHeaders.Remove(HeaderConst.X_RequestContinuation);
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First());
            result = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode);
            result.Result.Count().Is(1); // top指定なしでページング処理される（デフォルト100件）

            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            result.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");
        }

        /**
         * top指定無し、top必須設定の場合、ページング処理されない。
         */
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        [Ignore]
        public void QueryODataTest_ODataContinuationSenario_topCount_null_XRequestContinuationNeedsTopCount_true(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IQueryODataApi>();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを101件登録
            client.GetWebApiResponseResult(api.RegistList(Data101())).Assert(RegisterSuccessExpectStatusCode);

            //ページングを有効にする
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, " ");

            // 取得 Orderby なし　top なし　順不同なので件数のみチェック
            var result = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode);
            result.Result.Count.Is(101); // top指定なしでページング処理されない
            // ページング処理されないのでX-ResponseContinuationは無い
            result.Headers.TryGetValues(HeaderConst.X_ResponseContinuation, out _).IsFalse();
        }

        public List<AreaUnitModelEx> Data101()
        {
            var data = new List<AreaUnitModelEx>();
            for (int i = 0; i < 101; i++)
            {
                data.Add(new AreaUnitModelEx()
                {
                    AreaUnitCode = $"{i}",
                    AreaUnitName = $"{i}",
                    ConversionSquareMeters = i
                });
            }
            return data;
        }
    }
}