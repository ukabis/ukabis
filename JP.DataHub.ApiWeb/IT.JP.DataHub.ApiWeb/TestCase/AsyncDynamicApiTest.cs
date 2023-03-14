using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Async")]
    public class AsyncDynamicApiTest : ApiWebItTestCase
    {
        #region TestData

        private class AsyncDynamicApiTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AsyncDynamicApi~1~{WILDCARD}"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~AsyncDynamicApi~1~{WILDCARD}",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public AreaUnitModel Data1Patch = new AreaUnitModel()
            {
                ConversionSquareMeters = 2
            };
            public AreaUnitModel Data1Patched = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~AsyncDynamicApi~1~{WILDCARD}",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10
            };

            public AreaUnitModel Data9 = new AreaUnitModel()
            {
                AreaUnitCode = "ZZ"
            };

            public List<AreaUnitModel> Data7Units = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 100
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "HA",
                    AreaUnitName = "ha",
                    ConversionSquareMeters = 10000
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "M2",
                    AreaUnitName = "㎡",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "TN",
                    AreaUnitName = "反",
                    ConversionSquareMeters = 990
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "sa",
                    AreaUnitName = "sample",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "GG",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 100
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "GA",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 100
                }
            };

            public List<AreaUnitModel> DataTop2_1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 100
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "HA",
                    AreaUnitName = "ha",
                    ConversionSquareMeters = 10000
                }
            };
            public List<AreaUnitModel> DataTop2_2 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "M2",
                    AreaUnitName = "㎡",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "TN",
                    AreaUnitName = "反",
                    ConversionSquareMeters = 990
                }
            };
            public List<AreaUnitModel> DataTop2_3 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "sa",
                    AreaUnitName = "sample",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "GG",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 100
                }
            };
            public List<AreaUnitModel> DataTop2_4 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "GA",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 100
                }
            };
            
            public string DataXml7Units = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com.net/XMLSchema-instance/"" dh:anonymous_array=""true"">
    <item dh:array=""true"">
        <AreaUnitCode>AA</AreaUnitCode>
        <AreaUnitName>a</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">100</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>HA</AreaUnitCode>
        <AreaUnitName>ha</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">10000</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>M2</AreaUnitCode>
        <AreaUnitName>㎡</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">1</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>TN</AreaUnitCode>
        <AreaUnitName>反</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">990</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>sa</AreaUnitCode>
        <AreaUnitName>sample</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">1</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>GG</AreaUnitCode>
        <AreaUnitName>a</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">100</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>GA</AreaUnitCode>
        <AreaUnitName>a</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">100</ConversionSquareMeters>
    </item>
</root>";

            public string DataXmlTop2_1 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com.net/XMLSchema-instance/"" dh:anonymous_array=""true"">
    <item dh:array=""true"">
        <AreaUnitCode>AA</AreaUnitCode>
        <AreaUnitName>a</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">100</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>HA</AreaUnitCode>
        <AreaUnitName>ha</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">10000</ConversionSquareMeters>
    </item>
</root>";

            public string DataXmlTop2_2 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com.net/XMLSchema-instance/"" dh:anonymous_array=""true"">
    <item dh:array=""true"">
        <AreaUnitCode>M2</AreaUnitCode>
        <AreaUnitName>㎡</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">1</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>TN</AreaUnitCode>
        <AreaUnitName>反</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">990</ConversionSquareMeters>
    </item>
</root>";

            public string DataXmlTop2_3 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com.net/XMLSchema-instance/"" dh:anonymous_array=""true"">
    <item dh:array=""true"">
        <AreaUnitCode>sa</AreaUnitCode>
        <AreaUnitName>sample</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">1</ConversionSquareMeters>
    </item>
    <item dh:array=""true"">
        <AreaUnitCode>GG</AreaUnitCode>
        <AreaUnitName>a</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">100</ConversionSquareMeters>
    </item>
</root>";

            public string DataXmlTop2_4 = @"<?xml version=""1.0"" encoding=""utf-8""?>
<root xmlns:dh=""http://example.com.net/XMLSchema-instance/"" dh:anonymous_array=""true"">
    <item dh:array=""true"">
        <AreaUnitCode>GA</AreaUnitCode>
        <AreaUnitName>a</AreaUnitName>
        <ConversionSquareMeters dh:type=""number"">100</ConversionSquareMeters>
    </item>
</root>";

            public string Data7UnitCsv = @"AreaUnitCode,AreaUnitName,ConversionSquareMeters
AA,a,100
HA,ha,10000
M2,㎡,1
TN,反,990
sa,sample,1
GG,a,100
GA,a,100
";

            public string DataCsvTop2_1 = @"AreaUnitCode,AreaUnitName,ConversionSquareMeters
AA,a,100
HA,ha,10000
";
            public string DataCsvTop2_2 = @"AreaUnitCode,AreaUnitName,ConversionSquareMeters
M2,㎡,1
TN,反,990
";
            public string DataCsvTop2_3 = @"AreaUnitCode,AreaUnitName,ConversionSquareMeters
sa,sample,1
GG,a,100
";
            public string DataCsvTop2_4 = @"AreaUnitCode,AreaUnitName,ConversionSquareMeters
GA,a,100
";

            public List<AgriculturalLandModel> Data7Points = new List<AgriculturalLandModel>()
            {
                new AgriculturalLandModel()
                {
                    CityCode = "000001",
                    Longitude = 127.000000001m,
                    Latitude = 26.0000000000001m
                },
                new AgriculturalLandModel()
                {
                    CityCode = "000002",
                    Longitude = 127.000000002m,
                    Latitude = 26.0000000000002m
                },
                new AgriculturalLandModel()
                {
                    CityCode = "000003",
                    Longitude = 127.000000003m,
                    Latitude = 26.0000000000003m
                },
                new AgriculturalLandModel()
                {
                    CityCode = "000004",
                    Longitude = 127.000000004m,
                    Latitude = 26.0000000000004m
                },
                new AgriculturalLandModel()
                {
                    CityCode = "000005",
                    Longitude = 127.000000005m,
                    Latitude = 26.0000000000005m
                },
                new AgriculturalLandModel()
                {
                    CityCode = "000006",
                    Longitude = 127.000000006m,
                    Latitude = 26.0000000000006m
                },
                new AgriculturalLandModel()
                {
                    CityCode = "000007",
                    Longitude = 127.000000007m,
                    Latitude = 26.0000000000007m
                }
            };

            public GeoJsonPointModel DataGeoJson7Points = new GeoJsonPointModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPointFeature>()
                {
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000001m, 26.0000000000001m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000001"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000002m, 26.0000000000002m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000002"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000003m, 26.0000000000003m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000003"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000004m, 26.0000000000004m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000004"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000005m, 26.0000000000005m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000005"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000006m, 26.0000000000006m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000006"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000007m, 26.0000000000007m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000007"
                        }
                    }
                }
            };

            public GeoJsonPointModel DataGeoJsonTop2_1 = new GeoJsonPointModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPointFeature>()
                {
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000001m, 26.0000000000001m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000001"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000002m, 26.0000000000002m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000002"
                        }
                    }
                }
            };
            public GeoJsonPointModel DataGeoJsonTop2_2 = new GeoJsonPointModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPointFeature>()
                {
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000003m, 26.0000000000003m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000003"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000004m, 26.0000000000004m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000004"
                        }
                    }
                }
            };
            public GeoJsonPointModel DataGeoJsonTop2_3 = new GeoJsonPointModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPointFeature>()
                {
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000005m, 26.0000000000005m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000005"
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000006m, 26.0000000000006m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000006"
                        }
                    }
                }
            };
            public GeoJsonPointModel DataGeoJsonTop2_4 = new GeoJsonPointModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPointFeature>()
                {
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.000000007m, 26.0000000000007m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "000007"
                        }
                    }
                }
            };

            public AsyncDynamicApiTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void AsyncDynamicApiTest_NormalScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // 念のためデータを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データをAsyncで登録
            var regRequest = api.Regist(testData.Data1);
            var requestId = client.ExecAsyncApiJson(regRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 登録したデータ1件をAsyncでページングなしでGet
            var getRequest = api.Get("AA");
            var requestId2 = client.ExecAsyncApiJson(getRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId2)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // 結果をAsyncで変更
            var updRequest = api.Update("AA", testData.Data1Patch);
            var requestId3 = client.ExecAsyncApiJson(updRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId3)).Assert(UpdateSuccessExpectStatusCode);

            // 正しく変更されたか確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Patched);

            // データをAsyncで削除
            var delRequest = api.Delete("AA");
            var requestId4 = client.ExecAsyncApiJson(delRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId4)).Assert(DeleteSuccessStatusCode);

            // 削除されたか確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_AsyncWithVersionScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncWithVersionApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // エラー時のリトライに備えて登録バージョンをリセット
            client.GetWebApiResponseResult(api.SetNewVersion()).Assert(RegisterSuccessExpectStatusCode);

            // 【非同期】現在のバージョン番号取得
            var currentVersionRequest = api.GetCurrentVersion();
            var requestId = client.ExecAsyncApiJson(currentVersionRequest);
            var first_version = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<CurrentVersionResponseModel>(GetSuccessExpectStatusCode);

            var versionInfoRequest = api.GetVersionInfo();
            requestId = client.ExecAsyncApiJson(versionInfoRequest);
            var versionInfo = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<VersionInfoResponseModel>(GetSuccessExpectStatusCode);
            versionInfo.currentversion.Is(first_version.CurrentVersion);

            // 【非同期】レコード削除
            var delRequest = api.ODataDelete();
            requestId = client.ExecAsyncApiJson(delRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(DeleteExpectStatusCodes);

            // 【非同期】レコードを１件登録
            var regRequest = api.Register(testData.Data1);
            requestId = client.ExecAsyncApiJson(regRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(RegisterSuccessExpectStatusCode);
            var first_count = 1;

            // 【非同期】次のバージョン作成
            var newVersionRequest = api.SetNewVersion();
            requestId = client.ExecAsyncApiJson(newVersionRequest);
            var nextversion = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<CurrentVersionResponseModel>(RegisterSuccessExpectStatusCode);
            nextversion.CurrentVersion.Is(first_version.CurrentVersion + 1);

            // 【非同期】次のバージョンに、レコードを2件登録
            regRequest = api.Register(testData.Data1);
            requestId = client.ExecAsyncApiJson(regRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(RegisterSuccessExpectStatusCode);
            regRequest = api.Register(testData.Data2);
            requestId = client.ExecAsyncApiJson(regRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(RegisterSuccessExpectStatusCode);
            var second_count = 2;

            // 【非同期】最初のバージョンを指定して件数を取得
            api.AddHeaders.Add(HeaderConst.X_Version, first_version.CurrentVersion.ToString());
            var countRequest = api.GetCount();
            requestId = client.ExecAsyncApiJson(countRequest);
            var count = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<GetCountResponseModel>(GetSuccessExpectStatusCode);
            count.Count.Is(first_count);
            api.AddHeaders.Remove(HeaderConst.X_Version);

            // 【非同期】カレントバージョンを指定して件数を取得
            api.AddHeaders.Add(HeaderConst.X_Version, nextversion.CurrentVersion.ToString());
            countRequest = api.GetCount();
            requestId = client.ExecAsyncApiJson(countRequest);
            count = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<GetCountResponseModel>(GetSuccessExpectStatusCode);
            count.Count.Is(second_count);
            api.AddHeaders.Remove(HeaderConst.X_Version);

            // 【非同期】登録バージョンを作成
            var regVersionRequest = api.CreateRegisterVersion();
            requestId = client.ExecAsyncApiJson(regVersionRequest);
            var regVersion = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<RegisterVersionResponseModel>(RegisterSuccessExpectStatusCode);
            regVersion.RegisterVersion.Is(first_version.CurrentVersion + 2);

            var getRegVersionRequest = api.GetRegisterVersion();
            requestId = client.ExecAsyncApiJson(getRegVersionRequest);
            regVersion = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<RegisterVersionResponseModel>(GetSuccessExpectStatusCode);
            regVersion.RegisterVersion.Is(first_version.CurrentVersion + 2);

            // 【非同期】登録バージョンに、レコードを1件登録
            api.AddHeaders.Add(HeaderConst.X_Version, regVersion.RegisterVersion.ToString());
            regRequest = api.Register(testData.Data1);
            requestId = client.ExecAsyncApiJson(regRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(RegisterSuccessExpectStatusCode);

            // 【非同期】登録バージョンを指定して件数を取得
            countRequest = api.GetCount();
            requestId = client.ExecAsyncApiJson(countRequest);
            count = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<GetCountResponseModel>(GetSuccessExpectStatusCode);
            count.Count.Is(1);
            api.AddHeaders.Remove(HeaderConst.X_Version);

            // 【非同期】登録バージョンをFIX
            var compRegVersionRequest = api.CompleteRegisterVersion();
            requestId = client.ExecAsyncApiJson(compRegVersionRequest);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode);

            // 【非同期】バージョン指定なし(=カレントバージョン)を指定して件数を取得
            countRequest = api.GetCount();
            requestId = client.ExecAsyncApiJson(countRequest);
            count = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertAndResult<GetCountResponseModel>(GetSuccessExpectStatusCode);
            count.Count.Is(1);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_ContentTypeJsonScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // 念のためデータを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 取得に使うデータをまとめて登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data7Units)).Assert(RegisterSuccessExpectStatusCode);

            // それぞれの形式でページングなしで取得
            // TOP2
            var request = api.OData("$top=2&$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            var requestId = client.ExecAsyncApiJson(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode, testData.DataTop2_1);

            // 全件
            request = api.OData("$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            requestId = client.ExecAsyncApiJson(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode, testData.Data7Units);

            // それぞれの形式でページングありで取得
            request = api.OData("$top=2&$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            request.Header.Add(HeaderConst.X_RequestContinuation, " ");
            requestId = client.ExecAsyncApiJson(request);

            var continuation = client.GetResultPaging(requestId, " ", Accept.Json, testData.DataTop2_1);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Json, testData.DataTop2_2);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Json, testData.DataTop2_3);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Json, testData.DataTop2_4, true);

            // 後処理データを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_ContentTypeXmlScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // 念のためデータを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 取得に使うデータをまとめて登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data7Units)).Assert(RegisterSuccessExpectStatusCode);

            // それぞれの形式でページングなしで取得
            // TOP2
            var request = api.OData("$top=2&$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            var requestId = client.ExecAsyncApiXml(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode).ContentString.StringToXml().Is(testData.DataXmlTop2_1.StringToXml());

            // 全件
            request = api.OData("$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            requestId = client.ExecAsyncApiXml(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode).ContentString.StringToXml().Is(testData.DataXml7Units.StringToXml());

            // それぞれの形式でページングありで取得
            request = api.OData("$top=2&$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            request.Header.Add(HeaderConst.X_RequestContinuation, " ");
            requestId = client.ExecAsyncApiXml(request);

            var continuation = client.GetResultPaging(requestId, " ", Accept.Xml, testData.DataXmlTop2_1);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Xml, testData.DataXmlTop2_2);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Xml, testData.DataXmlTop2_3);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Xml, testData.DataXmlTop2_4, true);

            // 後処理データを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_ContentTypeCsvScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // 念のためデータを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 取得に使うデータをまとめて登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data7Units)).Assert(RegisterSuccessExpectStatusCode);

            // それぞれの形式でページングなしで取得
            // TOP2
            var request = api.OData("$top=2&$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            var requestId = client.ExecAsyncApiCsv(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode).ContentString.Is(testData.DataCsvTop2_1);

            // 全件
            request = api.OData("$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            requestId = client.ExecAsyncApiCsv(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode).ContentString.Is(testData.Data7UnitCsv);

            // それぞれの形式でページングありで取得
            request = api.OData("$top=2&$select=AreaUnitCode,AreaUnitName,ConversionSquareMeters");
            request.Header.Add(HeaderConst.X_RequestContinuation, " ");
            requestId = client.ExecAsyncApiCsv(request);

            var continuation = client.GetResultPaging(requestId, " ", Accept.Csv, testData.DataCsvTop2_1);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Csv, testData.DataCsvTop2_2);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Csv, testData.DataCsvTop2_3);
            continuation = client.GetResultPaging(requestId, continuation, Accept.Csv, testData.DataCsvTop2_4, true);

            // 後処理データを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_ContentTypeGeoJsonScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApiGeoJson>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // 念のためデータを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 取得に使うデータをまとめて登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data7Points)).Assert(RegisterSuccessExpectStatusCode);

            // それぞれの形式でページングなしで取得
            // TOP2
            var request = api.OData("$top=2&$select=CityCode,Longitude,Latitude");
            var requestId = client.ExecAsyncApiGeoJson(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode, testData.DataGeoJsonTop2_1);

            // 全件
            request = api.OData("$select=CityCode,Longitude,Latitude");
            requestId = client.ExecAsyncApiGeoJson(request);
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode, testData.DataGeoJson7Points);

            // それぞれの形式でページングありで取得
            request = api.OData("$top=2&$select=CityCode,Longitude,Latitude");
            request.Header.Add(HeaderConst.X_RequestContinuation, " ");
            requestId = client.ExecAsyncApiGeoJson(request);

            var continuation = client.GetResultPaging(requestId, " ", Accept.GeoJson, testData.DataGeoJsonTop2_1);
            continuation = client.GetResultPaging(requestId, continuation, Accept.GeoJson, testData.DataGeoJsonTop2_2);
            continuation = client.GetResultPaging(requestId, continuation, Accept.GeoJson, testData.DataGeoJsonTop2_3);
            continuation = client.GetResultPaging(requestId, continuation, Accept.GeoJson, testData.DataGeoJsonTop2_4, true);

            // 後処理データを全消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
        }

        [TestMethod]
        public void AsyncDynamicApiTest_AsyncGatewayScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(Repository.Default, null);

            // Gatewayの確認。Image、Jsonがそれぞれ受け取れるか
            // パラメータが正しく引き渡せるか、中継ヘッダーは正しく中継できるか

            // 先にリクエストを2つ投げておく
            // ①Image返却
            var gatewayApiImageCacheOff = UnityCore.Resolve<IGatewayImagesCacheOffApi>();
            var request = gatewayApiImageCacheOff.GetAsync("1");
            var headers = new Dictionary<string, string[]>()
            { 
                { HeaderConst.X_IsAsync, "true" }, 
                { "X-RelayTest1", "test1" },
                { "X-RelayTest2", "test2" },
                { "X-RelayTest3", "test3" }
            };
            var asyncReg1 = client.GetWebApiResponseResult(request, headers).Assert(HttpStatusCode.Accepted).Result;

            // ②Json返却
            var gatewayApiValueCacheOff = UnityCore.Resolve<IGatewayValuesCacheOffApi>();
            request = gatewayApiValueCacheOff.GetByQueryStringAsync("id2=hoge&id1=fuga");
            var asyncReg2 = client.GetWebApiResponseResult(request, headers).Assert(HttpStatusCode.Accepted).Result;

            // StatusがEndになるまでStatusをGet
            client.WaitForAsyncEnd(asyncReg1.RequestId);

            // 取得結果を取得
            var response = client.GetWebApiResponseResult(async.GetResult(asyncReg1.RequestId)).Assert(GetSuccessExpectStatusCode);
            var stream = response.RawContent.ReadAsStream();
            stream.Length.Is(GatewayTest.TestImage1Size);

            // StatusがEndになるまでStatusをGet
            client.WaitForAsyncEnd(asyncReg2.RequestId);

            // 登録結果を取得
            var response2 = client.GetWebApiResponseResult(async.GetResult(asyncReg2.RequestId)).Assert(GetSuccessExpectStatusCode);
            var resultJson1 = JsonConvert.DeserializeObject<GatewayTest.GatewayValueResult>(response2.ContentString);

            //中継ヘッダーが正しく中継できていることを確認 API側で1,3だけ中継する設定にしている
            resultJson1.IsContainsHeaderKey("X-RelayTest1").IsTrue();
            resultJson1.IsContainsHeaderKey("X-RelayTest2").IsFalse();
            resultJson1.IsContainsHeaderKey("X-RelayTest3").IsTrue();

            //パラメータが正しくセットされていることの確認
            resultJson1.IsContainsQueryString("id2", "hoge").IsTrue();
            resultJson1.IsContainsQueryString("id1", "fuga").IsTrue();
        }

        // ODataDeleteテスト
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_NormalScenario_ODataDelete(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // ODataDeleteをAsyncで実行
            var request = api.ODataDelete();
            var requestId = client.ExecAsyncApiJson(request);

            // 削除結果を取得
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(DeleteSuccessStatusCode);

            // 削除されたか確認
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AsyncDynamicApiTest_BadRequestScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(repository, api.ResourceUrl);

            // データ登録をAsyncで実行
            var request = api.Regist(testData.Data9);
            var requestId = client.ExecAsyncApiJson(request);

            // 登録結果を取得
            var response = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertErrorCode(BadRequestStatusCode, "E10402");
            JToken.Parse(response.ContentString)["status"].Value<int>().Is(400);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb, true)]
        [DataRow(Repository.CosmosDb, false)]
        public void AsyncDynamicApiTest_NotFoundScenario(Repository repository, bool withContinuation)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ取得(ページングあり)をAsyncで実行
            var request = api.OData();
            if (withContinuation)
            {
                request.Header.Add(HeaderConst.X_RequestContinuation, " ");
            }
            var requestId = client.ExecAsyncApiJson(request);

            // 登録結果を取得
            client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(NotFoundStatusCode);
        }

        [Ignore] // アラートが発生するため通常はIgnore
        [TestMethod]
        [DataRow(Repository.CosmosDb, true)]
        [DataRow(Repository.CosmosDb, false)]
        public void AsyncDynamicApiTest_InternalServerErrorScenario(Repository repository, bool withContinuation)
        {
            var retryStatusErrorPolicy = Policy
                .HandleResult<HttpResponseMessage>(r =>
                {
                    if (!r.IsSuccessStatusCode)
                    {
                        return true;
                    }

                    var status = JsonConvert.DeserializeObject<GetStatusResponseModel>(r.Content.ReadAsStringAsync().Result);
                    status.Status.IsNot("End");
                    return status.Status != "Error";
                })
                .WaitAndRetry(9, i => TimeSpan.FromSeconds(20));

            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();

            // データ取得をAsyncで実行
            var request = api.GetAll500();
            if (withContinuation)
            {
                request.Header.Add(HeaderConst.X_RequestContinuation, " ");
            }
            var requestId = client.ExecAsyncApiJson(request, retryStatusErrorPolicy);

            // 登録結果を取得
            var response = client.GetWebApiResponseResult(async.GetResult(requestId)).AssertErrorCode(InternalServerErrorStatusCode, "E99999");
            JToken.Parse(response.ContentString)["status"].Value<int>().Is(500);
        }

        // データが大きすぎるのでLocalでのみ実施すること
        [Ignore] // 時間がかかりすぎるため通常はIgnore
        [TestMethod]
        public void AsyncDynamicApiTest_BigSizeResponseScenario()
        {
            var retryLongtimeStatusPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }

                var status = JsonConvert.DeserializeObject<GetStatusResponseModel>(r.Content.ReadAsStringAsync().Result);
                status.Status.IsNot("Error");
                return status.Status != "End";
            })
            .WaitAndRetry(60, i => TimeSpan.FromSeconds(60));

            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAsyncDynamicApi>();
            var async = UnityCore.Resolve<IAsyncApi>();
            var testData = new AsyncDynamicApiTestData(Repository.Default, null);

            // データ取得をAsyncで実行
            var request = api.GatewayGet2GB();
            var headers = new Dictionary<string, string[]>() { { HeaderConst.X_IsAsync, "true" } };
            var response = client.GetWebApiResponseResult(request, headers).Assert(HttpStatusCode.Accepted);
            var requestId = JsonConvert.DeserializeObject<AsyncRequestResponseModel>(response.RawContentString).RequestId;

            Task.Delay(TimeSpan.FromSeconds(6000));

            // StatusがEndになるまでStatusをGet
            client.WaitForAsyncEnd(requestId, retryLongtimeStatusPolicy);

            var tempFileName = Guid.NewGuid().ToString();

            // 登録結果を取得
            var result = client.GetWebApiResponseResult(async.GetResult(requestId)).Assert(GetSuccessExpectStatusCode);
            using (var fileStream = File.Create(Path.Combine(Path.GetTempPath(), tempFileName)))
            {
                var st = result.RawContent.ReadAsStream();
                st.CopyTo(fileStream);
            }
            var fileInfo = new FileInfo(Path.Combine(Path.GetTempPath(), tempFileName));
            fileInfo.Length.Is(2461215039);
        }
    }
}

