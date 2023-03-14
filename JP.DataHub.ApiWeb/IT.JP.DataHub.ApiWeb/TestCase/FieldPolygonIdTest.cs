using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("GeoJson")]
    public class FieldPolygonIdTest : ApiWebItTestCase
    {
        private const int GeoSearchMaxCount = 99;
        private const int GeoSearchMaxDistanceCount = 62;

        // 全OR条件を網羅すると時間がかかるため通常は1件目のみテストする
        // 全OR条件のテストはAPIクエリの修正時などに必要に応じて手動で実施
        //private const int GeoSearchTestCount = GeoSearchMaxCount;
        //private const int GeoSearchTestDistanceCount = GeoSearchMaxDistanceCount;
        private const int GeoSearchTestCount = 1;
        private const int GeoSearchTestDistanceCount = 1;

        #region TestData

        private class FieldPolygonIdTestData : TestDataBase
        {
            public List<FieldPolygonIdModel> Data = new List<FieldPolygonIdModel>()
            {
                new FieldPolygonIdModel()
                {
                    FieldPolygonId = "TestData-01",
                    MinLongitude = 145.3726736474054m,
                    MaxLongitude = 145.383545497679m,
                    MinLatitude = 43.18406796268436m,
                    MaxLatitude = 43.19258059111899m,
                    Code = "00001",
                    FieldType = "田",
                    Polygons = new List<FieldPolygonIdPolygonItem>()
                    {
                        new FieldPolygonIdPolygonItem()
                        {
                            Coordinates = new List<FieldPolygonIdPolygonItemPoint>()
                            {
                                new FieldPolygonIdPolygonItemPoint()
                                {
                                    Latitude = "43.19258059111899",
                                    Longitude = "145.37917211698479"
                                },
                                new FieldPolygonIdPolygonItemPoint()
                                {
                                    Latitude = "43.191720282984086",
                                    Longitude = "145.37953434160269"
                                },
                                new FieldPolygonIdPolygonItemPoint()
                                {
                                    Latitude = "43.191493843748383",
                                    Longitude = "145.3796111147681"
                                }
                            }
                        }
                    },
                    GeoSearch = new FieldPolygonIdGeoSearch()
                    {
                        GeoSearch_1 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.000m,1.000m }, new List<decimal>() { 1.001m,1.000m }, new List<decimal>() { 1.001m,1.001m }, new List<decimal>() { 1.000m,1.001m }, new List<decimal>() { 1.000m,1.000m } } } },
                        GeoSearch_2 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.001m,1.001m }, new List<decimal>() { 1.002m,1.001m }, new List<decimal>() { 1.002m,1.002m }, new List<decimal>() { 1.001m,1.002m }, new List<decimal>() { 1.001m,1.001m } } } },
                        GeoSearch_3 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.002m,1.002m }, new List<decimal>() { 1.003m,1.002m }, new List<decimal>() { 1.003m,1.003m }, new List<decimal>() { 1.002m,1.003m }, new List<decimal>() { 1.002m,1.002m } } } },
                        GeoSearch_4 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.003m,1.003m }, new List<decimal>() { 1.004m,1.003m }, new List<decimal>() { 1.004m,1.004m }, new List<decimal>() { 1.003m,1.004m }, new List<decimal>() { 1.003m,1.003m } } } },
                        GeoSearch_5 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.004m,1.004m }, new List<decimal>() { 1.005m,1.004m }, new List<decimal>() { 1.005m,1.005m }, new List<decimal>() { 1.004m,1.005m }, new List<decimal>() { 1.004m,1.004m } } } },
                        GeoSearch_6 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.005m,1.005m }, new List<decimal>() { 1.006m,1.005m }, new List<decimal>() { 1.006m,1.006m }, new List<decimal>() { 1.005m,1.006m }, new List<decimal>() { 1.005m,1.005m } } } },
                        GeoSearch_7 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.006m,1.006m }, new List<decimal>() { 1.007m,1.006m }, new List<decimal>() { 1.007m,1.007m }, new List<decimal>() { 1.006m,1.007m }, new List<decimal>() { 1.006m,1.006m } } } },
                        GeoSearch_8 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.007m,1.007m }, new List<decimal>() { 1.008m,1.007m }, new List<decimal>() { 1.008m,1.008m }, new List<decimal>() { 1.007m,1.008m }, new List<decimal>() { 1.007m,1.007m } } } },
                        GeoSearch_9 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.008m,1.008m }, new List<decimal>() { 1.009m,1.008m }, new List<decimal>() { 1.009m,1.009m }, new List<decimal>() { 1.008m,1.009m }, new List<decimal>() { 1.008m,1.008m } } } },
                        GeoSearch_10 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.009m,1.009m }, new List<decimal>() { 1.010m,1.009m }, new List<decimal>() { 1.010m,1.010m }, new List<decimal>() { 1.009m,1.010m }, new List<decimal>() { 1.009m,1.009m } } } },
                        GeoSearch_11 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.010m,1.010m }, new List<decimal>() { 1.011m,1.010m }, new List<decimal>() { 1.011m,1.011m }, new List<decimal>() { 1.010m,1.011m }, new List<decimal>() { 1.010m,1.010m } } } },
                        GeoSearch_12 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.011m,1.011m }, new List<decimal>() { 1.012m,1.011m }, new List<decimal>() { 1.012m,1.012m }, new List<decimal>() { 1.011m,1.012m }, new List<decimal>() { 1.011m,1.011m } } } },
                        GeoSearch_13 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.012m,1.012m }, new List<decimal>() { 1.013m,1.012m }, new List<decimal>() { 1.013m,1.013m }, new List<decimal>() { 1.012m,1.013m }, new List<decimal>() { 1.012m,1.012m } } } },
                        GeoSearch_14 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.013m,1.013m }, new List<decimal>() { 1.014m,1.013m }, new List<decimal>() { 1.014m,1.014m }, new List<decimal>() { 1.013m,1.014m }, new List<decimal>() { 1.013m,1.013m } } } },
                        GeoSearch_15 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.014m,1.014m }, new List<decimal>() { 1.015m,1.014m }, new List<decimal>() { 1.015m,1.015m }, new List<decimal>() { 1.014m,1.015m }, new List<decimal>() { 1.014m,1.014m } } } },
                        GeoSearch_16 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.015m,1.015m }, new List<decimal>() { 1.016m,1.015m }, new List<decimal>() { 1.016m,1.016m }, new List<decimal>() { 1.015m,1.016m }, new List<decimal>() { 1.015m,1.015m } } } },
                        GeoSearch_17 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.016m,1.016m }, new List<decimal>() { 1.017m,1.016m }, new List<decimal>() { 1.017m,1.017m }, new List<decimal>() { 1.016m,1.017m }, new List<decimal>() { 1.016m,1.016m } } } },
                        GeoSearch_18 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.017m,1.017m }, new List<decimal>() { 1.018m,1.017m }, new List<decimal>() { 1.018m,1.018m }, new List<decimal>() { 1.017m,1.018m }, new List<decimal>() { 1.017m,1.017m } } } },
                        GeoSearch_19 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.018m,1.018m }, new List<decimal>() { 1.019m,1.018m }, new List<decimal>() { 1.019m,1.019m }, new List<decimal>() { 1.018m,1.019m }, new List<decimal>() { 1.018m,1.018m } } } },
                        GeoSearch_20 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.019m,1.019m }, new List<decimal>() { 1.020m,1.019m }, new List<decimal>() { 1.020m,1.020m }, new List<decimal>() { 1.019m,1.020m }, new List<decimal>() { 1.019m,1.019m } } } },
                        GeoSearch_21 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.020m,1.020m }, new List<decimal>() { 1.021m,1.020m }, new List<decimal>() { 1.021m,1.021m }, new List<decimal>() { 1.020m,1.021m }, new List<decimal>() { 1.020m,1.020m } } } },
                        GeoSearch_22 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.021m,1.021m }, new List<decimal>() { 1.022m,1.021m }, new List<decimal>() { 1.022m,1.022m }, new List<decimal>() { 1.021m,1.022m }, new List<decimal>() { 1.021m,1.021m } } } },
                        GeoSearch_23 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.022m,1.022m }, new List<decimal>() { 1.023m,1.022m }, new List<decimal>() { 1.023m,1.023m }, new List<decimal>() { 1.022m,1.023m }, new List<decimal>() { 1.022m,1.022m } } } },
                        GeoSearch_24 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.023m,1.023m }, new List<decimal>() { 1.024m,1.023m }, new List<decimal>() { 1.024m,1.024m }, new List<decimal>() { 1.023m,1.024m }, new List<decimal>() { 1.023m,1.023m } } } },
                        GeoSearch_25 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.024m,1.024m }, new List<decimal>() { 1.025m,1.024m }, new List<decimal>() { 1.025m,1.025m }, new List<decimal>() { 1.024m,1.025m }, new List<decimal>() { 1.024m,1.024m } } } },
                        GeoSearch_26 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.025m,1.025m }, new List<decimal>() { 1.026m,1.025m }, new List<decimal>() { 1.026m,1.026m }, new List<decimal>() { 1.025m,1.026m }, new List<decimal>() { 1.025m,1.025m } } } },
                        GeoSearch_27 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.026m,1.026m }, new List<decimal>() { 1.027m,1.026m }, new List<decimal>() { 1.027m,1.027m }, new List<decimal>() { 1.026m,1.027m }, new List<decimal>() { 1.026m,1.026m } } } },
                        GeoSearch_28 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.027m,1.027m }, new List<decimal>() { 1.028m,1.027m }, new List<decimal>() { 1.028m,1.028m }, new List<decimal>() { 1.027m,1.028m }, new List<decimal>() { 1.027m,1.027m } } } },
                        GeoSearch_29 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.028m,1.028m }, new List<decimal>() { 1.029m,1.028m }, new List<decimal>() { 1.029m,1.029m }, new List<decimal>() { 1.028m,1.029m }, new List<decimal>() { 1.028m,1.028m } } } },
                        GeoSearch_30 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.029m,1.029m }, new List<decimal>() { 1.030m,1.029m }, new List<decimal>() { 1.030m,1.030m }, new List<decimal>() { 1.029m,1.030m }, new List<decimal>() { 1.029m,1.029m } } } },
                        GeoSearch_31 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.030m,1.030m }, new List<decimal>() { 1.031m,1.030m }, new List<decimal>() { 1.031m,1.031m }, new List<decimal>() { 1.030m,1.031m }, new List<decimal>() { 1.030m,1.030m } } } },
                        GeoSearch_32 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.031m,1.031m }, new List<decimal>() { 1.032m,1.031m }, new List<decimal>() { 1.032m,1.032m }, new List<decimal>() { 1.031m,1.032m }, new List<decimal>() { 1.031m,1.031m } } } },
                        GeoSearch_33 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.032m,1.032m }, new List<decimal>() { 1.033m,1.032m }, new List<decimal>() { 1.033m,1.033m }, new List<decimal>() { 1.032m,1.033m }, new List<decimal>() { 1.032m,1.032m } } } },
                        GeoSearch_34 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.033m,1.033m }, new List<decimal>() { 1.034m,1.033m }, new List<decimal>() { 1.034m,1.034m }, new List<decimal>() { 1.033m,1.034m }, new List<decimal>() { 1.033m,1.033m } } } },
                        GeoSearch_35 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.034m,1.034m }, new List<decimal>() { 1.035m,1.034m }, new List<decimal>() { 1.035m,1.035m }, new List<decimal>() { 1.034m,1.035m }, new List<decimal>() { 1.034m,1.034m } } } },
                        GeoSearch_36 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.035m,1.035m }, new List<decimal>() { 1.036m,1.035m }, new List<decimal>() { 1.036m,1.036m }, new List<decimal>() { 1.035m,1.036m }, new List<decimal>() { 1.035m,1.035m } } } },
                        GeoSearch_37 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.036m,1.036m }, new List<decimal>() { 1.037m,1.036m }, new List<decimal>() { 1.037m,1.037m }, new List<decimal>() { 1.036m,1.037m }, new List<decimal>() { 1.036m,1.036m } } } },
                        GeoSearch_38 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.037m,1.037m }, new List<decimal>() { 1.038m,1.037m }, new List<decimal>() { 1.038m,1.038m }, new List<decimal>() { 1.037m,1.038m }, new List<decimal>() { 1.037m,1.037m } } } },
                        GeoSearch_39 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.038m,1.038m }, new List<decimal>() { 1.039m,1.038m }, new List<decimal>() { 1.039m,1.039m }, new List<decimal>() { 1.038m,1.039m }, new List<decimal>() { 1.038m,1.038m } } } },
                        GeoSearch_40 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.039m,1.039m }, new List<decimal>() { 1.040m,1.039m }, new List<decimal>() { 1.040m,1.040m }, new List<decimal>() { 1.039m,1.040m }, new List<decimal>() { 1.039m,1.039m } } } },
                        GeoSearch_41 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.040m,1.040m }, new List<decimal>() { 1.041m,1.040m }, new List<decimal>() { 1.041m,1.041m }, new List<decimal>() { 1.040m,1.041m }, new List<decimal>() { 1.040m,1.040m } } } },
                        GeoSearch_42 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.041m,1.041m }, new List<decimal>() { 1.042m,1.041m }, new List<decimal>() { 1.042m,1.042m }, new List<decimal>() { 1.041m,1.042m }, new List<decimal>() { 1.041m,1.041m } } } },
                        GeoSearch_43 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.042m,1.042m }, new List<decimal>() { 1.043m,1.042m }, new List<decimal>() { 1.043m,1.043m }, new List<decimal>() { 1.042m,1.043m }, new List<decimal>() { 1.042m,1.042m } } } },
                        GeoSearch_44 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.043m,1.043m }, new List<decimal>() { 1.044m,1.043m }, new List<decimal>() { 1.044m,1.044m }, new List<decimal>() { 1.043m,1.044m }, new List<decimal>() { 1.043m,1.043m } } } },
                        GeoSearch_45 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.044m,1.044m }, new List<decimal>() { 1.045m,1.044m }, new List<decimal>() { 1.045m,1.045m }, new List<decimal>() { 1.044m,1.045m }, new List<decimal>() { 1.044m,1.044m } } } },
                        GeoSearch_46 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.045m,1.045m }, new List<decimal>() { 1.046m,1.045m }, new List<decimal>() { 1.046m,1.046m }, new List<decimal>() { 1.045m,1.046m }, new List<decimal>() { 1.045m,1.045m } } } },
                        GeoSearch_47 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.046m,1.046m }, new List<decimal>() { 1.047m,1.046m }, new List<decimal>() { 1.047m,1.047m }, new List<decimal>() { 1.046m,1.047m }, new List<decimal>() { 1.046m,1.046m } } } },
                        GeoSearch_48 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.047m,1.047m }, new List<decimal>() { 1.048m,1.047m }, new List<decimal>() { 1.048m,1.048m }, new List<decimal>() { 1.047m,1.048m }, new List<decimal>() { 1.047m,1.047m } } } },
                        GeoSearch_49 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.048m,1.048m }, new List<decimal>() { 1.049m,1.048m }, new List<decimal>() { 1.049m,1.049m }, new List<decimal>() { 1.048m,1.049m }, new List<decimal>() { 1.048m,1.048m } } } },
                        GeoSearch_50 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.049m,1.049m }, new List<decimal>() { 1.050m,1.049m }, new List<decimal>() { 1.050m,1.050m }, new List<decimal>() { 1.049m,1.050m }, new List<decimal>() { 1.049m,1.049m } } } },
                        GeoSearch_51 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.050m,1.050m }, new List<decimal>() { 1.051m,1.050m }, new List<decimal>() { 1.051m,1.051m }, new List<decimal>() { 1.050m,1.051m }, new List<decimal>() { 1.050m,1.050m } } } },
                        GeoSearch_52 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.051m,1.051m }, new List<decimal>() { 1.052m,1.051m }, new List<decimal>() { 1.052m,1.052m }, new List<decimal>() { 1.051m,1.052m }, new List<decimal>() { 1.051m,1.051m } } } },
                        GeoSearch_53 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.052m,1.052m }, new List<decimal>() { 1.053m,1.052m }, new List<decimal>() { 1.053m,1.053m }, new List<decimal>() { 1.052m,1.053m }, new List<decimal>() { 1.052m,1.052m } } } },
                        GeoSearch_54 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.053m,1.053m }, new List<decimal>() { 1.054m,1.053m }, new List<decimal>() { 1.054m,1.054m }, new List<decimal>() { 1.053m,1.054m }, new List<decimal>() { 1.053m,1.053m } } } },
                        GeoSearch_55 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.054m,1.054m }, new List<decimal>() { 1.055m,1.054m }, new List<decimal>() { 1.055m,1.055m }, new List<decimal>() { 1.054m,1.055m }, new List<decimal>() { 1.054m,1.054m } } } },
                        GeoSearch_56 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.055m,1.055m }, new List<decimal>() { 1.056m,1.055m }, new List<decimal>() { 1.056m,1.056m }, new List<decimal>() { 1.055m,1.056m }, new List<decimal>() { 1.055m,1.055m } } } },
                        GeoSearch_57 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.056m,1.056m }, new List<decimal>() { 1.057m,1.056m }, new List<decimal>() { 1.057m,1.057m }, new List<decimal>() { 1.056m,1.057m }, new List<decimal>() { 1.056m,1.056m } } } },
                        GeoSearch_58 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.057m,1.057m }, new List<decimal>() { 1.058m,1.057m }, new List<decimal>() { 1.058m,1.058m }, new List<decimal>() { 1.057m,1.058m }, new List<decimal>() { 1.057m,1.057m } } } },
                        GeoSearch_59 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.058m,1.058m }, new List<decimal>() { 1.059m,1.058m }, new List<decimal>() { 1.059m,1.059m }, new List<decimal>() { 1.058m,1.059m }, new List<decimal>() { 1.058m,1.058m } } } },
                        GeoSearch_60 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.059m,1.059m }, new List<decimal>() { 1.060m,1.059m }, new List<decimal>() { 1.060m,1.060m }, new List<decimal>() { 1.059m,1.060m }, new List<decimal>() { 1.059m,1.059m } } } },
                        GeoSearch_61 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.060m,1.060m }, new List<decimal>() { 1.061m,1.060m }, new List<decimal>() { 1.061m,1.061m }, new List<decimal>() { 1.060m,1.061m }, new List<decimal>() { 1.060m,1.060m } } } },
                        GeoSearch_62 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.061m,1.061m }, new List<decimal>() { 1.062m,1.061m }, new List<decimal>() { 1.062m,1.062m }, new List<decimal>() { 1.061m,1.062m }, new List<decimal>() { 1.061m,1.061m } } } },
                        GeoSearch_63 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.062m,1.062m }, new List<decimal>() { 1.063m,1.062m }, new List<decimal>() { 1.063m,1.063m }, new List<decimal>() { 1.062m,1.063m }, new List<decimal>() { 1.062m,1.062m } } } },
                        GeoSearch_64 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.063m,1.063m }, new List<decimal>() { 1.064m,1.063m }, new List<decimal>() { 1.064m,1.064m }, new List<decimal>() { 1.063m,1.064m }, new List<decimal>() { 1.063m,1.063m } } } },
                        GeoSearch_65 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.064m,1.064m }, new List<decimal>() { 1.065m,1.064m }, new List<decimal>() { 1.065m,1.065m }, new List<decimal>() { 1.064m,1.065m }, new List<decimal>() { 1.064m,1.064m } } } },
                        GeoSearch_66 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.065m,1.065m }, new List<decimal>() { 1.066m,1.065m }, new List<decimal>() { 1.066m,1.066m }, new List<decimal>() { 1.065m,1.066m }, new List<decimal>() { 1.065m,1.065m } } } },
                        GeoSearch_67 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.066m,1.066m }, new List<decimal>() { 1.067m,1.066m }, new List<decimal>() { 1.067m,1.067m }, new List<decimal>() { 1.066m,1.067m }, new List<decimal>() { 1.066m,1.066m } } } },
                        GeoSearch_68 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.067m,1.067m }, new List<decimal>() { 1.068m,1.067m }, new List<decimal>() { 1.068m,1.068m }, new List<decimal>() { 1.067m,1.068m }, new List<decimal>() { 1.067m,1.067m } } } },
                        GeoSearch_69 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.068m,1.068m }, new List<decimal>() { 1.069m,1.068m }, new List<decimal>() { 1.069m,1.069m }, new List<decimal>() { 1.068m,1.069m }, new List<decimal>() { 1.068m,1.068m } } } },
                        GeoSearch_70 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.069m,1.069m }, new List<decimal>() { 1.070m,1.069m }, new List<decimal>() { 1.070m,1.070m }, new List<decimal>() { 1.069m,1.070m }, new List<decimal>() { 1.069m,1.069m } } } },
                        GeoSearch_71 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.070m,1.070m }, new List<decimal>() { 1.071m,1.070m }, new List<decimal>() { 1.071m,1.071m }, new List<decimal>() { 1.070m,1.071m }, new List<decimal>() { 1.070m,1.070m } } } },
                        GeoSearch_72 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.071m,1.071m }, new List<decimal>() { 1.072m,1.071m }, new List<decimal>() { 1.072m,1.072m }, new List<decimal>() { 1.071m,1.072m }, new List<decimal>() { 1.071m,1.071m } } } },
                        GeoSearch_73 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.072m,1.072m }, new List<decimal>() { 1.073m,1.072m }, new List<decimal>() { 1.073m,1.073m }, new List<decimal>() { 1.072m,1.073m }, new List<decimal>() { 1.072m,1.072m } } } },
                        GeoSearch_74 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.073m,1.073m }, new List<decimal>() { 1.074m,1.073m }, new List<decimal>() { 1.074m,1.074m }, new List<decimal>() { 1.073m,1.074m }, new List<decimal>() { 1.073m,1.073m } } } },
                        GeoSearch_75 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.074m,1.074m }, new List<decimal>() { 1.075m,1.074m }, new List<decimal>() { 1.075m,1.075m }, new List<decimal>() { 1.074m,1.075m }, new List<decimal>() { 1.074m,1.074m } } } },
                        GeoSearch_76 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.075m,1.075m }, new List<decimal>() { 1.076m,1.075m }, new List<decimal>() { 1.076m,1.076m }, new List<decimal>() { 1.075m,1.076m }, new List<decimal>() { 1.075m,1.075m } } } },
                        GeoSearch_77 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.076m,1.076m }, new List<decimal>() { 1.077m,1.076m }, new List<decimal>() { 1.077m,1.077m }, new List<decimal>() { 1.076m,1.077m }, new List<decimal>() { 1.076m,1.076m } } } },
                        GeoSearch_78 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.077m,1.077m }, new List<decimal>() { 1.078m,1.077m }, new List<decimal>() { 1.078m,1.078m }, new List<decimal>() { 1.077m,1.078m }, new List<decimal>() { 1.077m,1.077m } } } },
                        GeoSearch_79 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.078m,1.078m }, new List<decimal>() { 1.079m,1.078m }, new List<decimal>() { 1.079m,1.079m }, new List<decimal>() { 1.078m,1.079m }, new List<decimal>() { 1.078m,1.078m } } } },
                        GeoSearch_80 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.079m,1.079m }, new List<decimal>() { 1.080m,1.079m }, new List<decimal>() { 1.080m,1.080m }, new List<decimal>() { 1.079m,1.080m }, new List<decimal>() { 1.079m,1.079m } } } },
                        GeoSearch_81 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.080m,1.080m }, new List<decimal>() { 1.081m,1.080m }, new List<decimal>() { 1.081m,1.081m }, new List<decimal>() { 1.080m,1.081m }, new List<decimal>() { 1.080m,1.080m } } } },
                        GeoSearch_82 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.081m,1.081m }, new List<decimal>() { 1.082m,1.081m }, new List<decimal>() { 1.082m,1.082m }, new List<decimal>() { 1.081m,1.082m }, new List<decimal>() { 1.081m,1.081m } } } },
                        GeoSearch_83 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.082m,1.082m }, new List<decimal>() { 1.083m,1.082m }, new List<decimal>() { 1.083m,1.083m }, new List<decimal>() { 1.082m,1.083m }, new List<decimal>() { 1.082m,1.082m } } } },
                        GeoSearch_84 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.083m,1.083m }, new List<decimal>() { 1.084m,1.083m }, new List<decimal>() { 1.084m,1.084m }, new List<decimal>() { 1.083m,1.084m }, new List<decimal>() { 1.083m,1.083m } } } },
                        GeoSearch_85 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.084m,1.084m }, new List<decimal>() { 1.085m,1.084m }, new List<decimal>() { 1.085m,1.085m }, new List<decimal>() { 1.084m,1.085m }, new List<decimal>() { 1.084m,1.084m } } } },
                        GeoSearch_86 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.085m,1.085m }, new List<decimal>() { 1.086m,1.085m }, new List<decimal>() { 1.086m,1.086m }, new List<decimal>() { 1.085m,1.086m }, new List<decimal>() { 1.085m,1.085m } } } },
                        GeoSearch_87 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.086m,1.086m }, new List<decimal>() { 1.087m,1.086m }, new List<decimal>() { 1.087m,1.087m }, new List<decimal>() { 1.086m,1.087m }, new List<decimal>() { 1.086m,1.086m } } } },
                        GeoSearch_88 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.087m,1.087m }, new List<decimal>() { 1.088m,1.087m }, new List<decimal>() { 1.088m,1.088m }, new List<decimal>() { 1.087m,1.088m }, new List<decimal>() { 1.087m,1.087m } } } },
                        GeoSearch_89 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.088m,1.088m }, new List<decimal>() { 1.089m,1.088m }, new List<decimal>() { 1.089m,1.089m }, new List<decimal>() { 1.088m,1.089m }, new List<decimal>() { 1.088m,1.088m } } } },
                        GeoSearch_90 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.089m,1.089m }, new List<decimal>() { 1.090m,1.089m }, new List<decimal>() { 1.090m,1.090m }, new List<decimal>() { 1.089m,1.090m }, new List<decimal>() { 1.089m,1.089m } } } },
                        GeoSearch_91 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.090m,1.090m }, new List<decimal>() { 1.091m,1.090m }, new List<decimal>() { 1.091m,1.091m }, new List<decimal>() { 1.090m,1.091m }, new List<decimal>() { 1.090m,1.090m } } } },
                        GeoSearch_92 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.091m,1.091m }, new List<decimal>() { 1.092m,1.091m }, new List<decimal>() { 1.092m,1.092m }, new List<decimal>() { 1.091m,1.092m }, new List<decimal>() { 1.091m,1.091m } } } },
                        GeoSearch_93 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.092m,1.092m }, new List<decimal>() { 1.093m,1.092m }, new List<decimal>() { 1.093m,1.093m }, new List<decimal>() { 1.092m,1.093m }, new List<decimal>() { 1.092m,1.092m } } } },
                        GeoSearch_94 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.093m,1.093m }, new List<decimal>() { 1.094m,1.093m }, new List<decimal>() { 1.094m,1.094m }, new List<decimal>() { 1.093m,1.094m }, new List<decimal>() { 1.093m,1.093m } } } },
                        GeoSearch_95 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.094m,1.094m }, new List<decimal>() { 1.095m,1.094m }, new List<decimal>() { 1.095m,1.095m }, new List<decimal>() { 1.094m,1.095m }, new List<decimal>() { 1.094m,1.094m } } } },
                        GeoSearch_96 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.095m,1.095m }, new List<decimal>() { 1.096m,1.095m }, new List<decimal>() { 1.096m,1.096m }, new List<decimal>() { 1.095m,1.096m }, new List<decimal>() { 1.095m,1.095m } } } },
                        GeoSearch_97 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.096m,1.096m }, new List<decimal>() { 1.097m,1.096m }, new List<decimal>() { 1.097m,1.097m }, new List<decimal>() { 1.096m,1.097m }, new List<decimal>() { 1.096m,1.096m } } } },
                        GeoSearch_98 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.097m,1.097m }, new List<decimal>() { 1.098m,1.097m }, new List<decimal>() { 1.098m,1.098m }, new List<decimal>() { 1.097m,1.098m }, new List<decimal>() { 1.097m,1.097m } } } },
                        GeoSearch_99 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.098m,1.098m }, new List<decimal>() { 1.099m,1.098m }, new List<decimal>() { 1.099m,1.099m }, new List<decimal>() { 1.098m,1.099m }, new List<decimal>() { 1.098m,1.098m } } } }
                    }
                },
                new FieldPolygonIdModel()
                {
                    FieldPolygonId = "TestData-02",
                    MinLongitude = 145.3726736474054m,
                    MaxLongitude = 145.383545497679m,
                    MinLatitude = 43.18406796268436m,
                    MaxLatitude = 43.19258059111899m,
                    Code = "00002",
                    FieldType = "畑",
                    Polygons = new List<FieldPolygonIdPolygonItem>()
                    {
                        new FieldPolygonIdPolygonItem()
                        {
                            Coordinates = new List<FieldPolygonIdPolygonItemPoint>()
                            {
                                new FieldPolygonIdPolygonItemPoint()
                                {
                                    Latitude = "43.19258059111899",
                                    Longitude = "145.37917211698479"
                                },
                                new FieldPolygonIdPolygonItemPoint()
                                {
                                    Latitude = "43.191720282984086",
                                    Longitude = "145.37953434160269"
                                },
                                new FieldPolygonIdPolygonItemPoint()
                                {
                                    Latitude = "43.191493843748383",
                                    Longitude = "145.3796111147681"
                                }
                            }
                        }
                    },
                    GeoSearch = new FieldPolygonIdGeoSearch()
                    {
                        GeoSearch_1 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.000m,-1.000m }, new List<decimal>() { 1.000m,-1.001m }, new List<decimal>() { 1.001m,-1.001m }, new List<decimal>() { 1.001m,-1.000m }, new List<decimal>() { 1.000m,-1.000m } } } },
                        GeoSearch_2 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.001m,-1.001m }, new List<decimal>() { 1.001m,-1.002m }, new List<decimal>() { 1.002m,-1.002m }, new List<decimal>() { 1.002m,-1.001m }, new List<decimal>() { 1.001m,-1.001m } } } },
                        GeoSearch_3 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.002m,-1.002m }, new List<decimal>() { 1.002m,-1.003m }, new List<decimal>() { 1.003m,-1.003m }, new List<decimal>() { 1.003m,-1.002m }, new List<decimal>() { 1.002m,-1.002m } } } },
                        GeoSearch_4 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.003m,-1.003m }, new List<decimal>() { 1.003m,-1.004m }, new List<decimal>() { 1.004m,-1.004m }, new List<decimal>() { 1.004m,-1.003m }, new List<decimal>() { 1.003m,-1.003m } } } },
                        GeoSearch_5 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.004m,-1.004m }, new List<decimal>() { 1.004m,-1.005m }, new List<decimal>() { 1.005m,-1.005m }, new List<decimal>() { 1.005m,-1.004m }, new List<decimal>() { 1.004m,-1.004m } } } },
                        GeoSearch_6 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.005m,-1.005m }, new List<decimal>() { 1.005m,-1.006m }, new List<decimal>() { 1.006m,-1.006m }, new List<decimal>() { 1.006m,-1.005m }, new List<decimal>() { 1.005m,-1.005m } } } },
                        GeoSearch_7 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.006m,-1.006m }, new List<decimal>() { 1.006m,-1.007m }, new List<decimal>() { 1.007m,-1.007m }, new List<decimal>() { 1.007m,-1.006m }, new List<decimal>() { 1.006m,-1.006m } } } },
                        GeoSearch_8 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.007m,-1.007m }, new List<decimal>() { 1.007m,-1.008m }, new List<decimal>() { 1.008m,-1.008m }, new List<decimal>() { 1.008m,-1.007m }, new List<decimal>() { 1.007m,-1.007m } } } },
                        GeoSearch_9 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.008m,-1.008m }, new List<decimal>() { 1.008m,-1.009m }, new List<decimal>() { 1.009m,-1.009m }, new List<decimal>() { 1.009m,-1.008m }, new List<decimal>() { 1.008m,-1.008m } } } },
                        GeoSearch_10 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.009m,-1.009m }, new List<decimal>() { 1.009m,-1.010m }, new List<decimal>() { 1.010m,-1.010m }, new List<decimal>() { 1.010m,-1.009m }, new List<decimal>() { 1.009m,-1.009m } } } },
                        GeoSearch_11 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.010m,-1.010m }, new List<decimal>() { 1.010m,-1.011m }, new List<decimal>() { 1.011m,-1.011m }, new List<decimal>() { 1.011m,-1.010m }, new List<decimal>() { 1.010m,-1.010m } } } },
                        GeoSearch_12 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.011m,-1.011m }, new List<decimal>() { 1.011m,-1.012m }, new List<decimal>() { 1.012m,-1.012m }, new List<decimal>() { 1.012m,-1.011m }, new List<decimal>() { 1.011m,-1.011m } } } },
                        GeoSearch_13 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.012m,-1.012m }, new List<decimal>() { 1.012m,-1.013m }, new List<decimal>() { 1.013m,-1.013m }, new List<decimal>() { 1.013m,-1.012m }, new List<decimal>() { 1.012m,-1.012m } } } },
                        GeoSearch_14 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.013m,-1.013m }, new List<decimal>() { 1.013m,-1.014m }, new List<decimal>() { 1.014m,-1.014m }, new List<decimal>() { 1.014m,-1.013m }, new List<decimal>() { 1.013m,-1.013m } } } },
                        GeoSearch_15 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.014m,-1.014m }, new List<decimal>() { 1.014m,-1.015m }, new List<decimal>() { 1.015m,-1.015m }, new List<decimal>() { 1.015m,-1.014m }, new List<decimal>() { 1.014m,-1.014m } } } },
                        GeoSearch_16 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.015m,-1.015m }, new List<decimal>() { 1.015m,-1.016m }, new List<decimal>() { 1.016m,-1.016m }, new List<decimal>() { 1.016m,-1.015m }, new List<decimal>() { 1.015m,-1.015m } } } },
                        GeoSearch_17 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.016m,-1.016m }, new List<decimal>() { 1.016m,-1.017m }, new List<decimal>() { 1.017m,-1.017m }, new List<decimal>() { 1.017m,-1.016m }, new List<decimal>() { 1.016m,-1.016m } } } },
                        GeoSearch_18 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.017m,-1.017m }, new List<decimal>() { 1.017m,-1.018m }, new List<decimal>() { 1.018m,-1.018m }, new List<decimal>() { 1.018m,-1.017m }, new List<decimal>() { 1.017m,-1.017m } } } },
                        GeoSearch_19 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.018m,-1.018m }, new List<decimal>() { 1.018m,-1.019m }, new List<decimal>() { 1.019m,-1.019m }, new List<decimal>() { 1.019m,-1.018m }, new List<decimal>() { 1.018m,-1.018m } } } },
                        GeoSearch_20 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.019m,-1.019m }, new List<decimal>() { 1.019m,-1.020m }, new List<decimal>() { 1.020m,-1.020m }, new List<decimal>() { 1.020m,-1.019m }, new List<decimal>() { 1.019m,-1.019m } } } },
                        GeoSearch_21 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.020m,-1.020m }, new List<decimal>() { 1.020m,-1.021m }, new List<decimal>() { 1.021m,-1.021m }, new List<decimal>() { 1.021m,-1.020m }, new List<decimal>() { 1.020m,-1.020m } } } },
                        GeoSearch_22 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.021m,-1.021m }, new List<decimal>() { 1.021m,-1.022m }, new List<decimal>() { 1.022m,-1.022m }, new List<decimal>() { 1.022m,-1.021m }, new List<decimal>() { 1.021m,-1.021m } } } },
                        GeoSearch_23 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.022m,-1.022m }, new List<decimal>() { 1.022m,-1.023m }, new List<decimal>() { 1.023m,-1.023m }, new List<decimal>() { 1.023m,-1.022m }, new List<decimal>() { 1.022m,-1.022m } } } },
                        GeoSearch_24 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.023m,-1.023m }, new List<decimal>() { 1.023m,-1.024m }, new List<decimal>() { 1.024m,-1.024m }, new List<decimal>() { 1.024m,-1.023m }, new List<decimal>() { 1.023m,-1.023m } } } },
                        GeoSearch_25 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.024m,-1.024m }, new List<decimal>() { 1.024m,-1.025m }, new List<decimal>() { 1.025m,-1.025m }, new List<decimal>() { 1.025m,-1.024m }, new List<decimal>() { 1.024m,-1.024m } } } },
                        GeoSearch_26 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.025m,-1.025m }, new List<decimal>() { 1.025m,-1.026m }, new List<decimal>() { 1.026m,-1.026m }, new List<decimal>() { 1.026m,-1.025m }, new List<decimal>() { 1.025m,-1.025m } } } },
                        GeoSearch_27 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.026m,-1.026m }, new List<decimal>() { 1.026m,-1.027m }, new List<decimal>() { 1.027m,-1.027m }, new List<decimal>() { 1.027m,-1.026m }, new List<decimal>() { 1.026m,-1.026m } } } },
                        GeoSearch_28 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.027m,-1.027m }, new List<decimal>() { 1.027m,-1.028m }, new List<decimal>() { 1.028m,-1.028m }, new List<decimal>() { 1.028m,-1.027m }, new List<decimal>() { 1.027m,-1.027m } } } },
                        GeoSearch_29 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.028m,-1.028m }, new List<decimal>() { 1.028m,-1.029m }, new List<decimal>() { 1.029m,-1.029m }, new List<decimal>() { 1.029m,-1.028m }, new List<decimal>() { 1.028m,-1.028m } } } },
                        GeoSearch_30 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.029m,-1.029m }, new List<decimal>() { 1.029m,-1.030m }, new List<decimal>() { 1.030m,-1.030m }, new List<decimal>() { 1.030m,-1.029m }, new List<decimal>() { 1.029m,-1.029m } } } },
                        GeoSearch_31 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.030m,-1.030m }, new List<decimal>() { 1.030m,-1.031m }, new List<decimal>() { 1.031m,-1.031m }, new List<decimal>() { 1.031m,-1.030m }, new List<decimal>() { 1.030m,-1.030m } } } },
                        GeoSearch_32 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.031m,-1.031m }, new List<decimal>() { 1.031m,-1.032m }, new List<decimal>() { 1.032m,-1.032m }, new List<decimal>() { 1.032m,-1.031m }, new List<decimal>() { 1.031m,-1.031m } } } },
                        GeoSearch_33 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.032m,-1.032m }, new List<decimal>() { 1.032m,-1.033m }, new List<decimal>() { 1.033m,-1.033m }, new List<decimal>() { 1.033m,-1.032m }, new List<decimal>() { 1.032m,-1.032m } } } },
                        GeoSearch_34 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.033m,-1.033m }, new List<decimal>() { 1.033m,-1.034m }, new List<decimal>() { 1.034m,-1.034m }, new List<decimal>() { 1.034m,-1.033m }, new List<decimal>() { 1.033m,-1.033m } } } },
                        GeoSearch_35 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.034m,-1.034m }, new List<decimal>() { 1.034m,-1.035m }, new List<decimal>() { 1.035m,-1.035m }, new List<decimal>() { 1.035m,-1.034m }, new List<decimal>() { 1.034m,-1.034m } } } },
                        GeoSearch_36 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.035m,-1.035m }, new List<decimal>() { 1.035m,-1.036m }, new List<decimal>() { 1.036m,-1.036m }, new List<decimal>() { 1.036m,-1.035m }, new List<decimal>() { 1.035m,-1.035m } } } },
                        GeoSearch_37 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.036m,-1.036m }, new List<decimal>() { 1.036m,-1.037m }, new List<decimal>() { 1.037m,-1.037m }, new List<decimal>() { 1.037m,-1.036m }, new List<decimal>() { 1.036m,-1.036m } } } },
                        GeoSearch_38 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.037m,-1.037m }, new List<decimal>() { 1.037m,-1.038m }, new List<decimal>() { 1.038m,-1.038m }, new List<decimal>() { 1.038m,-1.037m }, new List<decimal>() { 1.037m,-1.037m } } } },
                        GeoSearch_39 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.038m,-1.038m }, new List<decimal>() { 1.038m,-1.039m }, new List<decimal>() { 1.039m,-1.039m }, new List<decimal>() { 1.039m,-1.038m }, new List<decimal>() { 1.038m,-1.038m } } } },
                        GeoSearch_40 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.039m,-1.039m }, new List<decimal>() { 1.039m,-1.040m }, new List<decimal>() { 1.040m,-1.040m }, new List<decimal>() { 1.040m,-1.039m }, new List<decimal>() { 1.039m,-1.039m } } } },
                        GeoSearch_41 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.040m,-1.040m }, new List<decimal>() { 1.040m,-1.041m }, new List<decimal>() { 1.041m,-1.041m }, new List<decimal>() { 1.041m,-1.040m }, new List<decimal>() { 1.040m,-1.040m } } } },
                        GeoSearch_42 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.041m,-1.041m }, new List<decimal>() { 1.041m,-1.042m }, new List<decimal>() { 1.042m,-1.042m }, new List<decimal>() { 1.042m,-1.041m }, new List<decimal>() { 1.041m,-1.041m } } } },
                        GeoSearch_43 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.042m,-1.042m }, new List<decimal>() { 1.042m,-1.043m }, new List<decimal>() { 1.043m,-1.043m }, new List<decimal>() { 1.043m,-1.042m }, new List<decimal>() { 1.042m,-1.042m } } } },
                        GeoSearch_44 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.043m,-1.043m }, new List<decimal>() { 1.043m,-1.044m }, new List<decimal>() { 1.044m,-1.044m }, new List<decimal>() { 1.044m,-1.043m }, new List<decimal>() { 1.043m,-1.043m } } } },
                        GeoSearch_45 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.044m,-1.044m }, new List<decimal>() { 1.044m,-1.045m }, new List<decimal>() { 1.045m,-1.045m }, new List<decimal>() { 1.045m,-1.044m }, new List<decimal>() { 1.044m,-1.044m } } } },
                        GeoSearch_46 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.045m,-1.045m }, new List<decimal>() { 1.045m,-1.046m }, new List<decimal>() { 1.046m,-1.046m }, new List<decimal>() { 1.046m,-1.045m }, new List<decimal>() { 1.045m,-1.045m } } } },
                        GeoSearch_47 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.046m,-1.046m }, new List<decimal>() { 1.046m,-1.047m }, new List<decimal>() { 1.047m,-1.047m }, new List<decimal>() { 1.047m,-1.046m }, new List<decimal>() { 1.046m,-1.046m } } } },
                        GeoSearch_48 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.047m,-1.047m }, new List<decimal>() { 1.047m,-1.048m }, new List<decimal>() { 1.048m,-1.048m }, new List<decimal>() { 1.048m,-1.047m }, new List<decimal>() { 1.047m,-1.047m } } } },
                        GeoSearch_49 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.048m,-1.048m }, new List<decimal>() { 1.048m,-1.049m }, new List<decimal>() { 1.049m,-1.049m }, new List<decimal>() { 1.049m,-1.048m }, new List<decimal>() { 1.048m,-1.048m } } } },
                        GeoSearch_50 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.049m,-1.049m }, new List<decimal>() { 1.049m,-1.050m }, new List<decimal>() { 1.050m,-1.050m }, new List<decimal>() { 1.050m,-1.049m }, new List<decimal>() { 1.049m,-1.049m } } } },
                        GeoSearch_51 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.050m,-1.050m }, new List<decimal>() { 1.050m,-1.051m }, new List<decimal>() { 1.051m,-1.051m }, new List<decimal>() { 1.051m,-1.050m }, new List<decimal>() { 1.050m,-1.050m } } } },
                        GeoSearch_52 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.051m,-1.051m }, new List<decimal>() { 1.051m,-1.052m }, new List<decimal>() { 1.052m,-1.052m }, new List<decimal>() { 1.052m,-1.051m }, new List<decimal>() { 1.051m,-1.051m } } } },
                        GeoSearch_53 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.052m,-1.052m }, new List<decimal>() { 1.052m,-1.053m }, new List<decimal>() { 1.053m,-1.053m }, new List<decimal>() { 1.053m,-1.052m }, new List<decimal>() { 1.052m,-1.052m } } } },
                        GeoSearch_54 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.053m,-1.053m }, new List<decimal>() { 1.053m,-1.054m }, new List<decimal>() { 1.054m,-1.054m }, new List<decimal>() { 1.054m,-1.053m }, new List<decimal>() { 1.053m,-1.053m } } } },
                        GeoSearch_55 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.054m,-1.054m }, new List<decimal>() { 1.054m,-1.055m }, new List<decimal>() { 1.055m,-1.055m }, new List<decimal>() { 1.055m,-1.054m }, new List<decimal>() { 1.054m,-1.054m } } } },
                        GeoSearch_56 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.055m,-1.055m }, new List<decimal>() { 1.055m,-1.056m }, new List<decimal>() { 1.056m,-1.056m }, new List<decimal>() { 1.056m,-1.055m }, new List<decimal>() { 1.055m,-1.055m } } } },
                        GeoSearch_57 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.056m,-1.056m }, new List<decimal>() { 1.056m,-1.057m }, new List<decimal>() { 1.057m,-1.057m }, new List<decimal>() { 1.057m,-1.056m }, new List<decimal>() { 1.056m,-1.056m } } } },
                        GeoSearch_58 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.057m,-1.057m }, new List<decimal>() { 1.057m,-1.058m }, new List<decimal>() { 1.058m,-1.058m }, new List<decimal>() { 1.058m,-1.057m }, new List<decimal>() { 1.057m,-1.057m } } } },
                        GeoSearch_59 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.058m,-1.058m }, new List<decimal>() { 1.058m,-1.059m }, new List<decimal>() { 1.059m,-1.059m }, new List<decimal>() { 1.059m,-1.058m }, new List<decimal>() { 1.058m,-1.058m } } } },
                        GeoSearch_60 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.059m,-1.059m }, new List<decimal>() { 1.059m,-1.060m }, new List<decimal>() { 1.060m,-1.060m }, new List<decimal>() { 1.060m,-1.059m }, new List<decimal>() { 1.059m,-1.059m } } } },
                        GeoSearch_61 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.060m,-1.060m }, new List<decimal>() { 1.060m,-1.061m }, new List<decimal>() { 1.061m,-1.061m }, new List<decimal>() { 1.061m,-1.060m }, new List<decimal>() { 1.060m,-1.060m } } } },
                        GeoSearch_62 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.061m,-1.061m }, new List<decimal>() { 1.061m,-1.062m }, new List<decimal>() { 1.062m,-1.062m }, new List<decimal>() { 1.062m,-1.061m }, new List<decimal>() { 1.061m,-1.061m } } } },
                        GeoSearch_63 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.062m,-1.062m }, new List<decimal>() { 1.062m,-1.063m }, new List<decimal>() { 1.063m,-1.063m }, new List<decimal>() { 1.063m,-1.062m }, new List<decimal>() { 1.062m,-1.062m } } } },
                        GeoSearch_64 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.063m,-1.063m }, new List<decimal>() { 1.063m,-1.064m }, new List<decimal>() { 1.064m,-1.064m }, new List<decimal>() { 1.064m,-1.063m }, new List<decimal>() { 1.063m,-1.063m } } } },
                        GeoSearch_65 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.064m,-1.064m }, new List<decimal>() { 1.064m,-1.065m }, new List<decimal>() { 1.065m,-1.065m }, new List<decimal>() { 1.065m,-1.064m }, new List<decimal>() { 1.064m,-1.064m } } } },
                        GeoSearch_66 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.065m,-1.065m }, new List<decimal>() { 1.065m,-1.066m }, new List<decimal>() { 1.066m,-1.066m }, new List<decimal>() { 1.066m,-1.065m }, new List<decimal>() { 1.065m,-1.065m } } } },
                        GeoSearch_67 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.066m,-1.066m }, new List<decimal>() { 1.066m,-1.067m }, new List<decimal>() { 1.067m,-1.067m }, new List<decimal>() { 1.067m,-1.066m }, new List<decimal>() { 1.066m,-1.066m } } } },
                        GeoSearch_68 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.067m,-1.067m }, new List<decimal>() { 1.067m,-1.068m }, new List<decimal>() { 1.068m,-1.068m }, new List<decimal>() { 1.068m,-1.067m }, new List<decimal>() { 1.067m,-1.067m } } } },
                        GeoSearch_69 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.068m,-1.068m }, new List<decimal>() { 1.068m,-1.069m }, new List<decimal>() { 1.069m,-1.069m }, new List<decimal>() { 1.069m,-1.068m }, new List<decimal>() { 1.068m,-1.068m } } } },
                        GeoSearch_70 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.069m,-1.069m }, new List<decimal>() { 1.069m,-1.070m }, new List<decimal>() { 1.070m,-1.070m }, new List<decimal>() { 1.070m,-1.069m }, new List<decimal>() { 1.069m,-1.069m } } } },
                        GeoSearch_71 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.070m,-1.070m }, new List<decimal>() { 1.070m,-1.071m }, new List<decimal>() { 1.071m,-1.071m }, new List<decimal>() { 1.071m,-1.070m }, new List<decimal>() { 1.070m,-1.070m } } } },
                        GeoSearch_72 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.071m,-1.071m }, new List<decimal>() { 1.071m,-1.072m }, new List<decimal>() { 1.072m,-1.072m }, new List<decimal>() { 1.072m,-1.071m }, new List<decimal>() { 1.071m,-1.071m } } } },
                        GeoSearch_73 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.072m,-1.072m }, new List<decimal>() { 1.072m,-1.073m }, new List<decimal>() { 1.073m,-1.073m }, new List<decimal>() { 1.073m,-1.072m }, new List<decimal>() { 1.072m,-1.072m } } } },
                        GeoSearch_74 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.073m,-1.073m }, new List<decimal>() { 1.073m,-1.074m }, new List<decimal>() { 1.074m,-1.074m }, new List<decimal>() { 1.074m,-1.073m }, new List<decimal>() { 1.073m,-1.073m } } } },
                        GeoSearch_75 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.074m,-1.074m }, new List<decimal>() { 1.074m,-1.075m }, new List<decimal>() { 1.075m,-1.075m }, new List<decimal>() { 1.075m,-1.074m }, new List<decimal>() { 1.074m,-1.074m } } } },
                        GeoSearch_76 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.075m,-1.075m }, new List<decimal>() { 1.075m,-1.076m }, new List<decimal>() { 1.076m,-1.076m }, new List<decimal>() { 1.076m,-1.075m }, new List<decimal>() { 1.075m,-1.075m } } } },
                        GeoSearch_77 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.076m,-1.076m }, new List<decimal>() { 1.076m,-1.077m }, new List<decimal>() { 1.077m,-1.077m }, new List<decimal>() { 1.077m,-1.076m }, new List<decimal>() { 1.076m,-1.076m } } } },
                        GeoSearch_78 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.077m,-1.077m }, new List<decimal>() { 1.077m,-1.078m }, new List<decimal>() { 1.078m,-1.078m }, new List<decimal>() { 1.078m,-1.077m }, new List<decimal>() { 1.077m,-1.077m } } } },
                        GeoSearch_79 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.078m,-1.078m }, new List<decimal>() { 1.078m,-1.079m }, new List<decimal>() { 1.079m,-1.079m }, new List<decimal>() { 1.079m,-1.078m }, new List<decimal>() { 1.078m,-1.078m } } } },
                        GeoSearch_80 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.079m,-1.079m }, new List<decimal>() { 1.079m,-1.080m }, new List<decimal>() { 1.080m,-1.080m }, new List<decimal>() { 1.080m,-1.079m }, new List<decimal>() { 1.079m,-1.079m } } } },
                        GeoSearch_81 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.080m,-1.080m }, new List<decimal>() { 1.080m,-1.081m }, new List<decimal>() { 1.081m,-1.081m }, new List<decimal>() { 1.081m,-1.080m }, new List<decimal>() { 1.080m,-1.080m } } } },
                        GeoSearch_82 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.081m,-1.081m }, new List<decimal>() { 1.081m,-1.082m }, new List<decimal>() { 1.082m,-1.082m }, new List<decimal>() { 1.082m,-1.081m }, new List<decimal>() { 1.081m,-1.081m } } } },
                        GeoSearch_83 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.082m,-1.082m }, new List<decimal>() { 1.082m,-1.083m }, new List<decimal>() { 1.083m,-1.083m }, new List<decimal>() { 1.083m,-1.082m }, new List<decimal>() { 1.082m,-1.082m } } } },
                        GeoSearch_84 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.083m,-1.083m }, new List<decimal>() { 1.083m,-1.084m }, new List<decimal>() { 1.084m,-1.084m }, new List<decimal>() { 1.084m,-1.083m }, new List<decimal>() { 1.083m,-1.083m } } } },
                        GeoSearch_85 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.084m,-1.084m }, new List<decimal>() { 1.084m,-1.085m }, new List<decimal>() { 1.085m,-1.085m }, new List<decimal>() { 1.085m,-1.084m }, new List<decimal>() { 1.084m,-1.084m } } } },
                        GeoSearch_86 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.085m,-1.085m }, new List<decimal>() { 1.085m,-1.086m }, new List<decimal>() { 1.086m,-1.086m }, new List<decimal>() { 1.086m,-1.085m }, new List<decimal>() { 1.085m,-1.085m } } } },
                        GeoSearch_87 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.086m,-1.086m }, new List<decimal>() { 1.086m,-1.087m }, new List<decimal>() { 1.087m,-1.087m }, new List<decimal>() { 1.087m,-1.086m }, new List<decimal>() { 1.086m,-1.086m } } } },
                        GeoSearch_88 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.087m,-1.087m }, new List<decimal>() { 1.087m,-1.088m }, new List<decimal>() { 1.088m,-1.088m }, new List<decimal>() { 1.088m,-1.087m }, new List<decimal>() { 1.087m,-1.087m } } } },
                        GeoSearch_89 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.088m,-1.088m }, new List<decimal>() { 1.088m,-1.089m }, new List<decimal>() { 1.089m,-1.089m }, new List<decimal>() { 1.089m,-1.088m }, new List<decimal>() { 1.088m,-1.088m } } } },
                        GeoSearch_90 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.089m,-1.089m }, new List<decimal>() { 1.089m,-1.090m }, new List<decimal>() { 1.090m,-1.090m }, new List<decimal>() { 1.090m,-1.089m }, new List<decimal>() { 1.089m,-1.089m } } } },
                        GeoSearch_91 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.090m,-1.090m }, new List<decimal>() { 1.090m,-1.091m }, new List<decimal>() { 1.091m,-1.091m }, new List<decimal>() { 1.091m,-1.090m }, new List<decimal>() { 1.090m,-1.090m } } } },
                        GeoSearch_92 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.091m,-1.091m }, new List<decimal>() { 1.091m,-1.092m }, new List<decimal>() { 1.092m,-1.092m }, new List<decimal>() { 1.092m,-1.091m }, new List<decimal>() { 1.091m,-1.091m } } } },
                        GeoSearch_93 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.092m,-1.092m }, new List<decimal>() { 1.092m,-1.093m }, new List<decimal>() { 1.093m,-1.093m }, new List<decimal>() { 1.093m,-1.092m }, new List<decimal>() { 1.092m,-1.092m } } } },
                        GeoSearch_94 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.093m,-1.093m }, new List<decimal>() { 1.093m,-1.094m }, new List<decimal>() { 1.094m,-1.094m }, new List<decimal>() { 1.094m,-1.093m }, new List<decimal>() { 1.093m,-1.093m } } } },
                        GeoSearch_95 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.094m,-1.094m }, new List<decimal>() { 1.094m,-1.095m }, new List<decimal>() { 1.095m,-1.095m }, new List<decimal>() { 1.095m,-1.094m }, new List<decimal>() { 1.094m,-1.094m } } } },
                        GeoSearch_96 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.095m,-1.095m }, new List<decimal>() { 1.095m,-1.096m }, new List<decimal>() { 1.096m,-1.096m }, new List<decimal>() { 1.096m,-1.095m }, new List<decimal>() { 1.095m,-1.095m } } } },
                        GeoSearch_97 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.096m,-1.096m }, new List<decimal>() { 1.096m,-1.097m }, new List<decimal>() { 1.097m,-1.097m }, new List<decimal>() { 1.097m,-1.096m }, new List<decimal>() { 1.096m,-1.096m } } } },
                        GeoSearch_98 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.097m,-1.097m }, new List<decimal>() { 1.097m,-1.098m }, new List<decimal>() { 1.098m,-1.098m }, new List<decimal>() { 1.098m,-1.097m }, new List<decimal>() { 1.097m,-1.097m } } } },
                        GeoSearch_99 = new FieldPolygonIdGeoSearchItem() { type = "Polygon", coordinates = new List<List<List<decimal>>>() { new List<List<decimal>>() { new List<decimal>() { 1.098m,-1.098m }, new List<decimal>() { 1.098m,-1.099m }, new List<decimal>() { 1.099m,-1.099m }, new List<decimal>() { 1.099m,-1.098m }, new List<decimal>() { 1.098m,-1.098m } } } }
                    }
                }
            };

            public FieldPolygonIdModel Expected1 = new FieldPolygonIdModel()
            {
                FieldPolygonId = "TestData-01",
                Code = "00001",
                FieldType = "田",
                Polygons = new List<FieldPolygonIdPolygonItem>()
                {
                    new FieldPolygonIdPolygonItem()
                    {
                        Coordinates = new List<FieldPolygonIdPolygonItemPoint>()
                        {
                            new FieldPolygonIdPolygonItemPoint()
                            {
                                Latitude = "43.19258059111899",
                                Longitude = "145.37917211698479"
                            },
                            new FieldPolygonIdPolygonItemPoint()
                            {
                                Latitude = "43.191720282984086",
                                Longitude = "145.37953434160269"
                            },
                            new FieldPolygonIdPolygonItemPoint()
                            {
                                Latitude = "43.191493843748383",
                                Longitude = "145.3796111147681"
                            }
                        }
                    }
                }
            };

            public FieldPolygonIdModel Expected2 = new FieldPolygonIdModel()
            {
                FieldPolygonId = "TestData-02",
                Code = "00002",
                FieldType = "畑",
                Polygons = new List<FieldPolygonIdPolygonItem>()
                {
                    new FieldPolygonIdPolygonItem()
                    {
                        Coordinates = new List<FieldPolygonIdPolygonItemPoint>()
                        {
                            new FieldPolygonIdPolygonItemPoint()
                            {
                                Latitude = "43.19258059111899",
                                Longitude = "145.37917211698479"
                            },
                            new FieldPolygonIdPolygonItemPoint()
                            {
                                Latitude = "43.191720282984086",
                                Longitude = "145.37953434160269"
                            },
                            new FieldPolygonIdPolygonItemPoint()
                            {
                                Latitude = "43.191493843748383",
                                Longitude = "145.3796111147681"
                            }
                        }
                    }
                }
            };

            public FieldPolygonIdTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void FieldPolygonIdGetScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IFieldPolygonIdApi>();
            var testData = new FieldPolygonIdTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // テストデータを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data)).Assert(RegisterSuccessExpectStatusCode);

            // Hit
            for (var i = 1; i <= GeoSearchTestCount; i++)
            {
                client.GetWebApiResponseResult(api.GetByLatLang($"1.{i:000}", $"1.{i - 1:000}")).Assert(GetSuccessExpectStatusCode, testData.Expected1);
            }

            // Missing
            client.GetWebApiResponseResult(api.GetByLatLang($"1.{GeoSearchMaxCount + 1:000}", $"1.{GeoSearchMaxCount + 1:000}")).Assert(NotFoundStatusCode);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        public void FieldPolygonIdGetAreaScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IFieldPolygonIdApi>();
            var testData = new FieldPolygonIdTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // テストデータを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data)).Assert(RegisterSuccessExpectStatusCode);

            // Hit(1件)
            for (var i = 1; i <= GeoSearchTestCount; i++)
            {
                client.GetWebApiResponseResult(api.GetArea($"1.{i - 1:000}3", $"1.{i - 1:000}3", $"1.{i - 1:000}7", $"1.{i - 1:000}7")).Assert(GetSuccessExpectStatusCode, new List<FieldPolygonIdModel> { testData.Expected1 });
            }

            // Hit(2件)
            var result = client.GetWebApiResponseResult(api.GetArea($"-1.{GeoSearchMaxCount + 1:000}7", $"1.{GeoSearchMaxCount - 1:000}3", $"1.{GeoSearchMaxCount - 1:000}7", $"1.{GeoSearchMaxCount - 1:000}7")).Assert(GetSuccessExpectStatusCode).Result;
            result.OrderBy(x => x.FieldPolygonId).ToList().IsStructuralEqual(new List<FieldPolygonIdModel>() { testData.Expected1, testData.Expected2 });

            // Missing
            client.GetWebApiResponseResult(api.GetArea($"1.{GeoSearchMaxCount + 1:000}", $"1.{GeoSearchMaxCount + 1:000}", $"1.{GeoSearchMaxCount + 2:000}", $"{GeoSearchMaxCount + 2:000}")).Assert(NotFoundStatusCode);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        public void FieldPolygonIdGetDistance(Repository repository)
        {
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // MongoDBとCosmosDBの距離の計算結果は完全には一致しないためある程度の誤差は許容するDistanceを指定している
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IFieldPolygonIdApi>();
            var testData = new FieldPolygonIdTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // テストデータを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data)).Assert(RegisterSuccessExpectStatusCode);

            // Hit(1件)
            for (var i = 1; i <= GeoSearchTestDistanceCount; i++)
            {
                var content = client.GetWebApiResponseResult(api.GetDistance($"1.{i:000}5", $"1.{i - 1:000}", "56")).Assert(GetSuccessExpectStatusCode).RawContentString;
                JsonConvert.DeserializeObject<FieldPolygonIdModel>(content).IsStructuralEqual(testData.Expected1);
            }

            // Hit(2件)
            var result = client.GetWebApiResponseResult(api.GetDistance($"0", $"0", "157425")).Assert(GetSuccessExpectStatusCode).Result;
            result.OrderBy(x => x.FieldPolygonId).ToList().IsStructuralEqual(new List<FieldPolygonIdModel>() { testData.Expected1, testData.Expected2 });

            // Missing
            client.GetWebApiResponseResult(api.GetDistance($"1.0015", $"1.000", "55")).Assert(NotFoundStatusCode);
        }
    }
}
