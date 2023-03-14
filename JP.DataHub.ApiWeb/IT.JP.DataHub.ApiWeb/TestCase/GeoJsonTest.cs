using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("GeoJson")]
    public class GeoJsonTest : ApiWebItTestCase
    {
        #region TestData

        private class GeoJsonPointTestData : TestDataBase
        {
            public List<GeoJsonPointDataModel> Data1 = new List<GeoJsonPointDataModel>()
            {
                new GeoJsonPointDataModel()
                {
                    CityCode = "472085",
                    Longitude = 127.735523872m,
                    Latitude = 26.2496550830001m
                },
                new GeoJsonPointDataModel()
                {
                    CityCode = "472086",
                    Longitude = 127.735825747m,
                    Latitude = 26.2498129090001m
                }
            };

            public GeoJsonPointDataModel Data1Expected = new GeoJsonPointDataModel()
            {
                CityCode = "472085",
                Longitude = 127.735523872m,
                Latitude = 26.2496550830001m,
                id = "API~IntegratedTest~GeoJsonPointTest~1~472085",
                _Owner_Id = WILDCARD
            };

            public GeoJsonPointModel Data1ExpectedGeoJson = new GeoJsonPointModel()
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
                            coordinates = new List<decimal>() { 127.735523872m, 26.2496550830001m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "472085",
                            id = "API~IntegratedTest~GeoJsonPointTest~1~472085",
                            _Owner_Id = WILDCARD
                        }
                    }
                }
            };

            public GeoJsonPointModel Data1ExpectedAllGeoJson = new GeoJsonPointModel()
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
                            coordinates = new List<decimal>() { 127.735523872m, 26.2496550830001m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "472085",
                            id = "API~IntegratedTest~GeoJsonPointTest~1~472085",
                            _Owner_Id = WILDCARD
                        }
                    },
                    new GeoJsonPointFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPointGeometry()
                        {
                            type = "Point",
                            coordinates = new List<decimal>() { 127.735825747m, 26.2498129090001m }
                        },
                        properties = new GeoJsonPointProperty()
                        {
                            CityCode = "472086",
                            id = "API~IntegratedTest~GeoJsonPointTest~1~472086",
                            _Owner_Id = WILDCARD
                        }
                    }
                }
            };

            public List<GeoJsonPointDataModel> Data1NonGeoJsonExpected = new List<GeoJsonPointDataModel>()
            {
                new GeoJsonPointDataModel()
                {
                    CityCode = "472085"
                },
                new GeoJsonPointDataModel()
                {
                    CityCode = "472086"
                }
            };

            public GeoJsonPointTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        private class GeoJsonPolygonTestData : TestDataBase
        {
            public List<GeoJsonPolygonDataModel> Data1 = new List<GeoJsonPolygonDataModel>()
            {
                new GeoJsonPolygonDataModel()
                {
                    CityCode = "472085",
                    Polygons = new List<GeoJsonPolygonDataPolygon>()
                    {
                        new GeoJsonPolygonDataPolygon()
                        {
                            Coordinates = new List<GeoJsonPolygonDataPoint>()
                            {
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.77126159107729",
                                    Longitude = "140.4738777820126"
                                },
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.77110744438427",
                                    Longitude = "140.4738889808593"
                                },
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.77101585083464",
                                    Longitude = "140.4735389089906"
                                },
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.77126159107729",
                                    Longitude = "140.4738777820126"
                                }
                            }
                        }
                    }
                },
                new GeoJsonPolygonDataModel()
                {
                    CityCode = "472086",
                    Polygons = new List<GeoJsonPolygonDataPolygon>()
                    {
                        new GeoJsonPolygonDataPolygon()
                        {
                            Coordinates = new List<GeoJsonPolygonDataPoint>()
                            {
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.68885523353125",
                                    Longitude = "141.1373064725661"
                                },
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.68874563682443",
                                    Longitude = "141.1365028218124"
                                },
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.68920533371121",
                                    Longitude = "141.1364130480752"
                                },
                                new GeoJsonPolygonDataPoint()
                                {
                                    Latitude = "40.68885523353125",
                                    Longitude = "141.1373064725661"
                                }
                            }
                        }
                    }
                }
            };

            public GeoJsonPolygonDataModel Data1Expected = new GeoJsonPolygonDataModel()
            {
                CityCode = "472085",
                Polygons = new List<GeoJsonPolygonDataPolygon>()
                {
                    new GeoJsonPolygonDataPolygon()
                    {
                        Coordinates = new List<GeoJsonPolygonDataPoint>()
                        {
                            new GeoJsonPolygonDataPoint()
                            {
                                Latitude = "40.77126159107729",
                                Longitude = "140.4738777820126"
                            },
                            new GeoJsonPolygonDataPoint()
                            {
                                Latitude = "40.77110744438427",
                                Longitude = "140.4738889808593"
                            },
                            new GeoJsonPolygonDataPoint()
                            {
                                Latitude = "40.77101585083464",
                                Longitude = "140.4735389089906"
                            },
                            new GeoJsonPolygonDataPoint()
                            {
                                Latitude = "40.77126159107729",
                                Longitude = "140.4738777820126"
                            }
                        }
                    }
                },
                id = "API~IntegratedTest~GeoJsonPolygonTest~1~472085",
                _Owner_Id = WILDCARD
            };

            public GeoJsonPolygonModel Data1ExpectedGeoJson = new GeoJsonPolygonModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPolygonFeature>()
                {
                    new GeoJsonPolygonFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPolygonGeometry()
                        {
                            type = "Polygon",
                            coordinates = new List<List<List<decimal>>>()
                            {
                                new List<List<decimal>>()
                                {
                                    new List<decimal>() { 140.4738777820126m, 40.77126159107729m },
                                    new List<decimal>() { 140.4738889808593m, 40.77110744438427m },
                                    new List<decimal>() { 140.4735389089906m, 40.77101585083464m },
                                    new List<decimal>() { 140.4738777820126m, 40.77126159107729m }
                                }
                            }
                        },
                        properties = new GeoJsonPolygonProperty()
                        {
                            CityCode = "472085",
                            id = "API~IntegratedTest~GeoJsonPolygonTest~1~472085",
                            _Owner_Id = WILDCARD
                        }
                    }
                }
            };

            public GeoJsonPolygonModel Data1ExpectedAllGeoJson = new GeoJsonPolygonModel()
            {
                type = "FeatureCollection",
                features = new List<GeoJsonPolygonFeature>()
                {
                    new GeoJsonPolygonFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPolygonGeometry()
                        {
                            type = "Polygon",
                            coordinates = new List<List<List<decimal>>>()
                            {
                                new List<List<decimal>>()
                                {
                                    new List<decimal>() { 140.4738777820126m, 40.77126159107729m },
                                    new List<decimal>() { 140.4738889808593m, 40.77110744438427m },
                                    new List<decimal>() { 140.4735389089906m, 40.77101585083464m },
                                    new List<decimal>() { 140.4738777820126m, 40.77126159107729m }
                                }
                            }
                        },
                        properties = new GeoJsonPolygonProperty()
                        {
                            CityCode = "472085",
                            id = "API~IntegratedTest~GeoJsonPolygonTest~1~472085",
                            _Owner_Id = WILDCARD
                        }
                    },
                    new GeoJsonPolygonFeature()
                    {
                        type = "Feature",
                        geometry = new GeoJsonPolygonGeometry()
                        {
                            type = "Polygon",
                            coordinates = new List<List<List<decimal>>>()
                            {
                                new List<List<decimal>>()
                                {
                                    new List<decimal>() { 141.1373064725661m, 40.68885523353125m },
                                    new List<decimal>() { 141.1365028218124m, 40.68874563682443m },
                                    new List<decimal>() { 141.1364130480752m, 40.68920533371121m },
                                    new List<decimal>() { 141.1373064725661m, 40.68885523353125m }
                                }
                            }
                        },
                        properties = new GeoJsonPolygonProperty()
                        {
                            CityCode = "472086",
                            id = "API~IntegratedTest~GeoJsonPolygonTest~1~472086",
                            _Owner_Id = WILDCARD
                        }
                    }
                }
            };

            public List<GeoJsonPolygonDataModel> Data1NonGeoJsonExpected = new List<GeoJsonPolygonDataModel>()
            {
                new GeoJsonPolygonDataModel()
                {
                    CityCode = "472085"
                },
                new GeoJsonPolygonDataModel()
                {
                    CityCode = "472086"
                }
            };

            public GeoJsonPolygonTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void GeoJson_PointSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IGeoJsonPointApi>();
            var testData = new GeoJsonPointTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // Query取得(通常)
            var expectedMediaType = "application/json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            var response = client.GetWebApiResponseResult(api.Get("472085")).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
            response.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // Query取得
            expectedMediaType = "application/geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            var responseGeoJson = client.GetWebApiResponseResult(api.GetAsGeoJson("472085")).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // Query(配列)取得
            expectedMediaType = "application/vnd.geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            responseGeoJson = client.GetWebApiResponseResult(api.GetAllAsGeoJson()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedAllGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // OData取得
            expectedMediaType = "application/geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            responseGeoJson = client.GetWebApiResponseResult(api.ODataAsGeoJson()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedAllGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            expectedMediaType = "application/vnd.geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            responseGeoJson = client.GetWebApiResponseResult(api.ODataAsGeoJson()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedAllGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // 非GeoJson
            api.AddHeaders.Remove(HeaderConst.Accept);
            var responseList = client.GetWebApiResponseResult(api.OData("$select=CityCode")).Assert(GetSuccessExpectStatusCode, testData.Data1NonGeoJsonExpected);
            responseList.RawContent.Headers.ContentType.MediaType.Is("application/json");
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void GeoJson_PolygonSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IGeoJsonPolygonApi>();
            var testData = new GeoJsonPolygonTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // Query取得(通常)
            var expectedMediaType = "application/json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            var response = client.GetWebApiResponseResult(api.Get("472085")).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
            response.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // Query取得
            expectedMediaType = "application/geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            var responseGeoJson = client.GetWebApiResponseResult(api.GetAsGeoJson("472085")).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // Query(配列)取得
            expectedMediaType = "application/vnd.geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            responseGeoJson = client.GetWebApiResponseResult(api.GetAllAsGeoJson()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedAllGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // OData取得
            expectedMediaType = "application/geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            responseGeoJson = client.GetWebApiResponseResult(api.ODataAsGeoJson()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedAllGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            expectedMediaType = "application/vnd.geo+json";
            api.AddHeaders[HeaderConst.Accept] = new string[] { expectedMediaType };
            responseGeoJson = client.GetWebApiResponseResult(api.ODataAsGeoJson()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedAllGeoJson);
            responseGeoJson.RawContent.Headers.ContentType.MediaType.Is(expectedMediaType);

            // 非GeoJson
            api.AddHeaders.Remove(HeaderConst.Accept);
            var responseList = client.GetWebApiResponseResult(api.OData("$select=CityCode")).Assert(GetSuccessExpectStatusCode, testData.Data1NonGeoJsonExpected);
            responseList.RawContent.Headers.ContentType.MediaType.Is("application/json");
        }
    }
}