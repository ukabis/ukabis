using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ODataPatchTest : ApiWebItTestCase
    {
        #region TestData

        private class ODataPatchTestData : TestDataBase
        {
            public List<ODataPatchModel> Data1 = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge1",
                    STR_NULL = null,
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge2",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge3",
                    STR_NULL = null,
                    INT_VALUE= 333m,
                    DBL_VALUE= 333.333m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key3 = "value3" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-03-13T03:30:33",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge4",
                    STR_NULL = null,
                    INT_VALUE= 444m,
                    DBL_VALUE= 444.444m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key4 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value4-1", "value4-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-04-14T04:40:44",
                    DAT_NULL = null
                }
            };

            public List<ODataPatchModel> Data1Expected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge1",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge1",
                    STR_NULL = null,
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge2",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge3",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge3",
                    STR_NULL = null,
                    INT_VALUE= 333m,
                    DBL_VALUE= 333.333m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key3 = "value3" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-03-13T03:30:33",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge4",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge4",
                    STR_NULL = null,
                    INT_VALUE= 444m,
                    DBL_VALUE= 444.444m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key4 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value4-1", "value4-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-04-14T04:40:44",
                    DAT_NULL = null
                }
            };

            public ODataPatchModelForUpd Data2 = new ODataPatchModelForUpd()
            {
                STR_NULL = "hogehoge"
            };

            public List<ODataPatchModel> Data2Expected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge1",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge1",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge2",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge3",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge3",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 333m,
                    DBL_VALUE= 333.333m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key3 = "value3" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-03-13T03:30:33",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge4",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge4",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 444m,
                    DBL_VALUE= 444.444m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key4 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value4-1", "value4-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-04-14T04:40:44",
                    DAT_NULL = null
                }
            };

            public ODataPatchModelForUpdEx Data3 = new ODataPatchModelForUpdEx()
            {
                STR_NULL = null,
                INT_VALUE = 222m,
                DBL_VALUE = 555.555m,
                NUM_NULL = 555,
                OBJ_VALUE = new ODataPatchObject { key5_1 = "value5-1" },
                OBJ_NULL = new ODataPatchObject { key5_2 = "value5-2" },
                ARY_VALUE = new List<string> { "value5-1-1", "value5-1-2" },
                ARY_NULL = new List<string> { "value5-2-1", "value5-2-2" },
                BOL_NULL = true,
                DAT_VALUE = "2021-05-15T05:50:55",
                DAT_NULL = "2022-05-15T05:50:55"
            };

            public List<ODataPatchModel> Data3Expected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge1",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge1",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge2",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge3",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge3",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 555.555m,
                    NUM_NULL = 555m,
                    OBJ_VALUE= new ODataPatchObject { key5_1 = "value5-1" },
                    OBJ_NULL = new ODataPatchObject { key5_2 = "value5-2" },
                    ARY_VALUE= new List<string> { "value5-1-1", "value5-1-2" },
                    ARY_NULL = new List<string> { "value5-2-1", "value5-2-2" },
                    BOL_VALUE= false,
                    BOL_NULL = true,
                    DAT_VALUE= "2021-05-15T05:50:55",
                    DAT_NULL = "2022-05-15T05:50:55"
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge4",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge4",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 555.555m,
                    NUM_NULL = 555m,
                    OBJ_VALUE= new ODataPatchObject { key5_1 = "value5-1" },
                    OBJ_NULL = new ODataPatchObject { key5_2 = "value5-2" },
                    ARY_VALUE= new List<string> { "value5-1-1", "value5-1-2" },
                    ARY_NULL = new List<string> { "value5-2-1", "value5-2-2" },
                    BOL_VALUE= false,
                    BOL_NULL = true,
                    DAT_VALUE= "2021-05-15T05:50:55",
                    DAT_NULL = "2022-05-15T05:50:55"
                }
            };

            public ODataPatchModelForUpdEx Data4 = new ODataPatchModelForUpdEx()
            {
                STR_NULL = "fugafuga",
                DBL_VALUE = 666.666m,
                NUM_NULL = null,
                OBJ_VALUE = new ODataPatchObject { key6_1 = "value6-1" },
                OBJ_NULL = null,
                ARY_VALUE = new List<string> { "value6-1-1", "value6-1-2" },
                ARY_NULL = null,
                BOL_VALUE = false,
                BOL_NULL = null,
                DAT_VALUE = "2021-06-16T06:06:06",
                DAT_NULL = null,
                _Where = new ODataPatchWhere()
                {
                    ColumnName = "STR_VALUE",
                    Operator = "IN",
                    Object = new List<string> { "hoge1", "hoge2", "hoge3" }
                }
            };

            public List<ODataPatchModel> Data4Expected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge1",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge1",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge2",
                    STR_NULL = "fugafuga",
                    INT_VALUE= 222m,
                    DBL_VALUE= 666.666m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key6_1 = "value6-1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value6-1-1", "value6-1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-06-16T06:06:06",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge3",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge3",
                    STR_NULL = "fugafuga",
                    INT_VALUE= 222m,
                    DBL_VALUE= 666.666m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key6_1 = "value6-1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value6-1-1", "value6-1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-06-16T06:06:06",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchTest~1~hoge4",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge4",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 555.555m,
                    NUM_NULL = 555m,
                    OBJ_VALUE= new ODataPatchObject { key5_1 = "value5-1" },
                    OBJ_NULL = new ODataPatchObject { key5_2 = "value5-2" },
                    ARY_VALUE= new List<string> { "value5-1-1", "value5-1-2" },
                    ARY_NULL = new List<string> { "value5-2-1", "value5-2-2" },
                    BOL_VALUE= false,
                    BOL_NULL = true,
                    DAT_VALUE= "2021-05-15T05:50:55",
                    DAT_NULL = "2022-05-15T05:50:55"
                }
            };

            public ODataPatchModelForUpd Data5 = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyopiyo",
                _Where = new ODataPatchWhereEx()
                {
                    ColumnName = "STR_VALUE",
                    Operator = "IN",
                    Object = new List<string> { "hoge1" }
                }
            };

            public ODataPatchModelForUpd Data6 = new ODataPatchModelForUpd()
            {
                id = "piyopiyo"
            };

            public ODataPatchModelForUpd Data7 = new ODataPatchModelForUpd()
            {
                _Vendor_Id = "hogehoge",
                _System_Id = "fugafuga"
            };

            public ODataPatchModelForUpd Data8 = new ODataPatchModelForUpd()
            {
                HOGE = "hogehoge",
            };

            public ODataPatchModelForUpd Data9 = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyopiyo",
                _Where = new ODataPatchWhereEx()
                {
                    HOGE = "HOGE",
                    FUGA = "FUGA",
                    PIYO = new List<string>() { "PIYO" }
                }
            };

            public ODataPatchModelForUpd Data10 = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyopiyo",
                _Where = new ODataPatchWhereEx()
                {
                    ColumnName = "HOGE",
                    Operator = "IN",
                    Object = new List<string>() { "hoge1" }
                }
            };

            public ODataPatchModelForUpd Data11 = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyopiyo",
                _Where = new ODataPatchWhereEx()
                {
                    ColumnName = "_Version",
                    Operator = "IN",
                    Object = new List<string>() { "hoge1" }
                }
            };

            public ODataPatchModelForUpd Data12 = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyopiyo",
                _Where = new ODataPatchWhereEx()
                {
                    ColumnName = "STR_VALUE",
                    Operator = "EQ",
                    Object = new List<string>() { "hoge1" }
                }
            };

            public ODataPatchModelForUpd Data13 = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyopiyo",
                _Where = new ODataPatchWhereEx()
                {
                    ColumnName = "STR_VALUE",
                    Operator = "EQ",
                    Object = new List<string>() { }
                }
            };

            public ODataPatchModelForUpd Data14 = new ODataPatchModelForUpd()
            {
                STR_NULL = "$Base64(44G744G744G744G744G744G744G744G744G744G744G744G744G744GS44G744GS44G744GS44G144GM44G144GM44G044KI44G044Kj)"
            };

            public ODataPatchModelForUpdEx2 Data15 = new ODataPatchModelForUpdEx2()
            {
                INT_VALUE = "hogehoge"
            };

            public ODataPatchModelForUpd Data16 = new ODataPatchModelForUpd()
            {
                STR_NULL = "foobar"
            };

            public ODataPatchTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, true, client: client) { }
        }

        private class ODataPatchEscapeTestData : TestDataBase
        {
            public List<AreaUnitModel> Data1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "D1",
                    AreaUnitName = "hogehoge",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "D2",
                    AreaUnitName = "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~",
                    ConversionSquareMeters = 2
                }
            };
            public List<AreaUnitModel> Data1Expected = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~ODataPatchEscapeTest~1~D1",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D1",
                    AreaUnitName = "hogehoge",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~ODataPatchEscapeTest~1~D2",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D2",
                    AreaUnitName = "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~",
                    ConversionSquareMeters = 2
                }
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                ConversionSquareMeters = 3,
                _Where = new ODataPatchWhere()
                {
                    ColumnName = "AreaUnitName",
                    Operator = "IN",
                    Object = new List<string>() { "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~" }
                }
            };
            public List<AreaUnitModel> Data2Expected = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~ODataPatchEscapeTest~1~D1",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D1",
                    AreaUnitName = "hogehoge",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~ODataPatchEscapeTest~1~D2",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "D2",
                    AreaUnitName = "! \"#$%&\'()*+,-./:;<=>?@[\\]^_`{|}~",
                    ConversionSquareMeters = 3
                }
            };

            public ODataPatchEscapeTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, client: client) { }
        }

        private class ODataPatchPersonPrivateTestData : TestDataBase
        {
            public List<ODataPatchModel> Data1 = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge1",
                    STR_NULL = null,
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge2",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge3",
                    STR_NULL = null,
                    INT_VALUE= 333m,
                    DBL_VALUE= 333.333m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key3 = "value3" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-03-13T03:30:33",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE= "hoge4",
                    STR_NULL = null,
                    INT_VALUE= 444m,
                    DBL_VALUE= 444.444m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key4 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value4-1", "value4-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-04-14T04:40:44",
                    DAT_NULL = null
                }
            };

            public List<ODataPatchModel> Data1Expected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge1",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge1",
                    STR_NULL = null,
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge2",
                    STR_NULL = null,
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge3",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge3",
                    STR_NULL = null,
                    INT_VALUE= 333m,
                    DBL_VALUE= 333.333m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key3 = "value3" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-03-13T03:30:33",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge4",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge4",
                    STR_NULL = null,
                    INT_VALUE= 444m,
                    DBL_VALUE= 444.444m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key4 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value4-1", "value4-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-04-14T04:40:44",
                    DAT_NULL = null
                }
            };

            public ODataPatchModelForUpd Data2 = new ODataPatchModelForUpd()
            {
                STR_NULL = "hogehoge"
            };

            public List<ODataPatchModel> Data2Expected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge1",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge1",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 111m,
                    DBL_VALUE= 111.111m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-01-11T01:10:11",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge2",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 222m,
                    DBL_VALUE= 222.222m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE= true,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-02-12T02:20:22",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge3",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge3",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 333m,
                    DBL_VALUE= 333.333m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key3 = "value3" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-03-13T03:30:33",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~ODataPatchPersonPrivateTest~1~hoge4",
                    _Owner_Id = WILDCARD,
                    STR_VALUE= "hoge4",
                    STR_NULL = "hogehoge",
                    INT_VALUE= 444m,
                    DBL_VALUE= 444.444m,
                    NUM_NULL = null,
                    OBJ_VALUE= new ODataPatchObject { key4 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE= new List<string> { "value4-1", "value4-2" },
                    ARY_NULL = null,
                    BOL_VALUE= false,
                    BOL_NULL = null,
                    DAT_VALUE= "2021-04-14T04:40:44",
                    DAT_NULL = null
                }
            };

            public ODataPatchPersonPrivateTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, false, true, client: client) { }
        }

        private class ODataPatchOptimisticConcurrencyTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitName = "zzz"
            };

            public ODataPatchOptimisticConcurrencyTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, client: client) { }
        }

        private class ODataPatchMultiRepositoryTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public AreaUnitModel Data1Expected = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~ODataPatchMultiRepositoryTest~1~AA",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitName = "zzz"
            };
            public AreaUnitModel Data2Expected = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~ODataPatchMultiRepositoryTest~1~AA",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "zzz",
                ConversionSquareMeters = 1
            };

            public ODataPatchMultiRepositoryTestData(string resourceUrl, IntegratedTestClient client) : base(Repository.Default, resourceUrl, true, client: client) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void ODataPatchTest_RdbmsSingleRepository_NormalScenario(Repository repository)
        {
            var api = UnityCore.Resolve<IODataPatchApi>();
            var clientA = new IntegratedTestClient("test1", "SmartFoodChainAdmin") { TargetRepository = repository };
            var testDataA = new ODataPatchTestData(repository, api.ResourceUrl, clientA);
            // 他ベンダー影響確認用
            var clientB = new IntegratedTestClient("test1", "SmartFoodChainPortal") { TargetRepository = repository };
            var testDataB = new ODataPatchTestData(repository, api.ResourceUrl, clientB);
            // UPDUSERID確認用
            var clientC = new IntegratedTestClient("test2", "SmartFoodChainAdmin") { TargetRepository = repository };


            // 最初に全データの消去(両ベンダー)
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録(両ベンダー)
            clientA.GetWebApiResponseResult(api.RegisterList(testDataA.Data1)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(testDataB.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 登録結果確認(両ベンダー)
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data1Expected);
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataB.Data1Expected);

            // 全件更新(単一項目更新)
            clientA.GetWebApiResponseResult(api.ODataPatchAll(null, testDataA.Data2)).Assert(UpdateSuccessExpectStatusCode);

            // 更新結果確認
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data2Expected);

            // 条件付きで更新(複数項目更新、$top/$select/$orderbyは無視される)
            clientA.GetWebApiResponseResult(api.ODataPatchEx("$top=1&$select=STR_VALUE,STR_NULL&$filter=BOL_VALUE eq false&$orderby=STR_VALUE,STR_NULL", testDataA.Data3)).Assert(UpdateSuccessExpectStatusCode);

            // 更新結果確認
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data3Expected);

            // 追加条件ありで更新
            clientA.GetWebApiResponseResult(api.ODataPatchEx("$filter=INT_VALUE eq 222", testDataA.Data4)).Assert(UpdateSuccessExpectStatusCode);

            // 更新結果確認
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data4Expected);

            // 該当データなしの条件で更新
            clientA.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testDataA.Data5)).AssertErrorCode(NotFoundStatusCode, "I10402");

            // 更新結果確認(変わっていないこと)
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data4Expected);


            // UPDUSERID、UPDDATEが更新されていること
            // 更新前
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");

            var before = clientC.GetWebApiResponseResult(api.OData("$filter=STR_VALUE eq 'hoge1'")).Assert(GetSuccessExpectStatusCode).Result;

            // 更新
            clientC.GetWebApiResponseResult(api.ODataPatchAll(null, testDataA.Data5)).Assert(UpdateSuccessExpectStatusCode);

            // 更新後
            var after = clientC.GetWebApiResponseResult(api.OData("$filter=STR_VALUE eq 'hoge1'")).Assert(GetSuccessExpectStatusCode).Result;

            before[0]._Upduser_Id.IsNot(after[0]._Upduser_Id);
            before[0]._Upddate.IsNot(after[0]._Upddate);

            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // もう一方のベンダーのデータに影響がないこと
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataB.Data1Expected);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void ODataPatchTest_RdbmsSingleRepository_EscapeScenario(Repository repository)
        {
            var api = UnityCore.Resolve<IODataPatchEscapeApi>();
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var testData = new ODataPatchEscapeTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 登録結果確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // 条件付きで更新(OData、追加条件とも記号全部入り)
            client.GetWebApiResponseResult(api.ODataPatch($"$filter=AreaUnitName eq '{HttpUtility.UrlEncode("! \"#$%&''()*+,-./:;<=>?@[\\]^_`{|}~")}'", testData.Data2)).Assert(UpdateSuccessExpectStatusCode);

            // 更新結果確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.Data2Expected);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void ODataPatchTest_RdbmsSingleRepository_PersonPrivateScenario(Repository repository)
        {
            var api = UnityCore.Resolve<IODataPatchPersonPrivateApi>();
            var clientA = new IntegratedTestClient("test1") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2") { TargetRepository = repository };
            var testDataA = new ODataPatchPersonPrivateTestData(repository, api.ResourceUrl, clientA);
            var testDataB = new ODataPatchPersonPrivateTestData(repository, api.ResourceUrl, clientB);

            // 最初に全データの消去(両ユーザー)
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録(両ユーザー)
            clientA.GetWebApiResponseResult(api.RegisterList(testDataA.Data1)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(testDataB.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 登録結果確認(両ユーザー)
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data1Expected);
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataB.Data1Expected);

            // 全件更新(単一項目更新)
            clientA.GetWebApiResponseResult(api.ODataPatchAll(null, testDataA.Data2)).Assert(UpdateSuccessExpectStatusCode);

            // 更新結果確認
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataA.Data2Expected);

            // もう一方のユーザーのデータに影響がないこと
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testDataB.Data1Expected);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void ODataPatchTest_RdbmsSingleRepository_BadRequestScenario(Repository repository)
        {
            var api = UnityCore.Resolve<IODataPatchApi>();
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var testData = new ODataPatchTestData(repository, api.ResourceUrl, client);

            // OData不正
            // 構文エラー(ODataException)
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1", testData.Data2)).AssertErrorCode(BadRequestStatusCode, "E50406");

            // 値の型がスキーマと異なる
            client.GetWebApiResponseResult(api.ODataPatch("$filter=INT_VALUE eq 'piyo1'", testData.Data2)).AssertErrorCode(BadRequestStatusCode, "E10426");

            // 存在しないプロパティ
            client.GetWebApiResponseResult(api.ODataPatch("$filter=HOGE eq 'piyo1'", testData.Data2)).AssertErrorCode(NotFoundStatusCode, "E10422");

            // PatchData不正
            // JSONでない
            client.GetWebApiResponseResult(api.ODataPatchAsString("$filter=STR_VALUE eq 'piyo1'", Guid.NewGuid().ToString())).AssertErrorCode(BadRequestStatusCode, "E10438");

            // 配列
            client.GetWebApiResponseResult(api.ODataPatchAsString("$filter=STR_VALUE eq 'piyo1'", "[]")).AssertErrorCode(BadRequestStatusCode, "E10438");

            // 更新対象なし
            client.GetWebApiResponseResult(api.ODataPatchAsString("$filter=STR_VALUE eq 'piyo1'", "{}")).AssertErrorCode(BadRequestStatusCode, "E10438");

            // 更新対象が管理項目(id)
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data6)).AssertErrorCode(BadRequestStatusCode, "E10439");

            // 更新対象が管理項目(_XXX)
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data7)).AssertErrorCode(BadRequestStatusCode, "E10439");

            // 更新対象が存在しないプロパティ
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data8)).AssertErrorCode(BadRequestStatusCode, "E10439");

            // 追加条件の形式不正
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data9)).AssertErrorCode(BadRequestStatusCode, "E10440");

            // 追加条件の列名が存在しないプロパティ
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data10)).AssertErrorCode(BadRequestStatusCode, "E10440");

            // 追加条件の列名が禁止項目
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data11)).AssertErrorCode(BadRequestStatusCode, "E10440");

            // 追加条件のオペレータが存在しないプロパティ
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data12)).AssertErrorCode(BadRequestStatusCode, "E10440");

            // 追加条件のオブジェクトが空
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data13)).AssertErrorCode(BadRequestStatusCode, "E10440");

            // 更新値がBase64
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data14)).AssertErrorCode(BadRequestStatusCode, "E10437");

            // 更新値のJSONバリデーションエラー
            client.GetWebApiResponseResult(api.ODataPatchEx2("$filter=STR_VALUE eq 'piyo1'", testData.Data15)).AssertErrorCode(BadRequestStatusCode, "E10402");

            // 更新値のJSONバリデーションエラー(ForeignKey)
            client.GetWebApiResponseResult(api.ODataPatch("$filter=STR_VALUE eq 'piyo1'", testData.Data16)).AssertErrorCode(BadRequestStatusCode, "E10402");
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void ODataPatchTest_RdbmsSingleRepository_OptimisticConcurrency(Repository repository)
        {
            var api = UnityCore.Resolve<IODataPatchOptimisticConcurrencyApi>();
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var testData = new ODataPatchOptimisticConcurrencyTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを登録
            client.GetWebApiResponseResult(api.RegisterList(new List<AreaUnitModel>() { testData.Data1 })).Assert(RegisterSuccessExpectStatusCode);

            // ヘッダ指定なしで更新(BadRequest)
            client.GetWebApiResponseResult(api.ODataPatchAll(null, testData.Data2)).AssertErrorCode(BadRequestStatusCode, "E10436");

            // ヘッダ指定ありで更新(NoContent)
            api.AddHeaders.Add(HeaderConst.X_NoOptimistic, "true");
            client.GetWebApiResponseResult(api.ODataPatchAll(null, testData.Data2)).Assert(UpdateSuccessExpectStatusCode);
        }

        [TestMethod]
        public void ODataPatchTest_SupportedRepositoryNotExistsScenario()
        {
            var api = UnityCore.Resolve<IODataPatchNotSupportedRepositoryApi>();
            var client = new IntegratedTestClient(AppConfig.Account);

            var data = new AreaUnitModel()
            {
                AreaUnitName = "zzz"
            };
            client.GetWebApiResponseResult(api.ODataPatchAll(null, data)).AssertErrorCode(BadRequestStatusCode, "E10435");
        }

        [TestMethod]
        public void ODataPatchTest_MultiRepositoryScenario()
        {
            var api = UnityCore.Resolve<IODataPatchMultiRepositoryApi>();
            var client = new IntegratedTestClient(AppConfig.Account);
            var testData = new ODataPatchMultiRepositoryTestData(api.ResourceUrl, client);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(api.RegisterList(new List<AreaUnitModel>() { testData.Data1 })).Assert(RegisterSuccessExpectStatusCode);

            // 登録結果確認(両リポジトリ)
            client.GetWebApiResponseResult(api.GetAllFromCosmosDB()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Expected });
            client.GetWebApiResponseResult(api.GetAllFromSqlServer()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Expected });

            // 更新
            client.GetWebApiResponseResult(api.ODataPatchAll(null, testData.Data2)).Assert(UpdateSuccessExpectStatusCode);

            // 更新結果確認(SQLServerのみ更新されていること)
            client.GetWebApiResponseResult(api.GetAllFromCosmosDB()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Expected });
            client.GetWebApiResponseResult(api.GetAllFromSqlServer()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data2Expected });
        }
    }
}