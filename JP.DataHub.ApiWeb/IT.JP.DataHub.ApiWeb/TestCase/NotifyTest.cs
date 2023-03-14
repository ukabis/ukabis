using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    [TestCategory("DocumentHistory")]
    public class NotifyTest : ApiWebItTestCase
    {
        private IntegratedTestClient client;

        private IReferenceNotifyApi reference;
        private IReferenceNotifySourceApi src;
        private IReferenceNotifyFirstApi first;

        private IReferenceNotifyODataApi referenceodata;
        private IReferenceNotifyODataFirstApi firstodata;
        private IReferenceNotifySourceHistCheckApi referenceNotifySourceHistCheckApi;

        private IReferenceNotifyODataNestSettingApi referenceodataNestsetting;
        private IReferenceNotifyODataNestSettingFirstApi firstodataNestsetting;

        public string ecpect_SourceShallowPathOData = $@"[
{{
  'AreaUnitCode': 'AA',
  '_Owner_Id': '{WILDCARD}',
  'id': 'API~IntegratedTest~ReferenceNotifySource~1~AA',
  'AreaUnitName': 'aa',
  'ConversionSquareMeters': 2000,
  'Obj': {{
    'prop1': 'b',
    'prop2': 'bb',
    'prop3': 'bbb'
  }},
  'Array': [
    'z',
    'zz',
    'zzz'
  ],'ArrayData': [
    'z',
    'zz',
    'zzz'
  ],'ArraObject':null,
  'ArrayObject': [
    {{
      'p1': 'pa',
      'p2': 'paa'
    }}
  ],
}},
{{
  'AreaUnitCode': 'AA',
  '_Owner_Id': '{WILDCARD}',
  'id': 'API~IntegratedTest~ReferenceNotifySource~1~hogeId2',
  'AreaUnitName': 'aa',
  'ConversionSquareMeters': 5000,
  'Obj': {{
    'prop1': 'b',
    'prop2': 'bb',
    'prop3': 'bbb'
  }},
  'Array': [
    'z',
    'zz',
    'zzz'
  ],
  'ArrayData': [
    'z',
    'zz',
    'zzz'
  ],'ArraObject':null,
  'ArrayObject': [
    {{
      'p1': 'pa',
      'p2': 'paa'
    }}
  ],
}}]";

        #region TestData

        private class ReferenceNotifyFirstTestData : TestDataBase
        {
            public ReferenceNotifyFirstModel Data1 = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject",
                Ref11 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,BooleanNullProp",
                Ref12 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberNullProp",
                Ref13 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,StringNullProp",
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/xx",
                        ObservationItemCode = "x",
                        ObservationValue = "xx"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/02",
                        ObservationItemCode = "2",
                        ObservationValue = "22"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "3",
                    prop2 = "yy",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "zzz"
                    }
                }
            };

            public ReferenceNotifyFirstModel Data2 = new ReferenceNotifyFirstModel()
            {
                Code = "02",
                Name = "002",
                temp = 100,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Array[5]",
                Ref5 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberArray[4]",
                Ref6 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0].ABC",
                Ref7 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Object.prop3.prop34",
                Ref8 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject",
                Ref11 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,BooleanNullProp",
                Ref12 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberNullProp",
                Ref13 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,StringNullProp",
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/xx",
                        ObservationItemCode = "x",
                        ObservationValue = "xx"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/02",
                        ObservationItemCode = "2",
                        ObservationValue = "22"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "3",
                    prop2 = "yy",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "zzz"
                    }
                }
            };

            public ReferenceNotifyFirstModel Data99 = new ReferenceNotifyFirstModel()
            {
                Code = "99",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject",
                Ref11 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,BooleanNullProp",
                Ref12 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberNullProp",
                Ref13 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,StringNullProp",
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/xx",
                        ObservationItemCode = "x",
                        ObservationValue = "xx"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/02",
                        ObservationItemCode = "2",
                        ObservationValue = "22"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "3",
                    prop2 = "yy",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "zzz"
                    }
                }
            };

            public ReferenceNotifyFirstModel Data1_nulltest = new ReferenceNotifyFirstModel()
            {
                Code = "10",
                Name = "111",
                temp = 10,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Obj",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,abc",
                Ref9 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,ArrayObject",
                Ref11 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,BooleanNullProp",
                Ref12 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberNullProp",
                Ref13 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,StringNullProp",
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/xx",
                        ObservationItemCode = "x",
                        ObservationValue = "xx"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/02",
                        ObservationItemCode = "2",
                        ObservationValue = "22"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "3",
                    prop2 = "yy",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "zzz"
                    }
                }
            };

            public ReferenceNotifyFirstModel Data1_toHistSettingApi = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/ReferenceNotifySourceHistoryCheck/Get/FL,ArrayObject",
                Ref11 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,BooleanNullProp",
                Ref12 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,NumberNullProp",
                Ref13 = "$Reference /API/IntegratedTest/ReferenceNotifySource/Get/FL,StringNullProp",
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/xx",
                        ObservationItemCode = "x",
                        ObservationValue = "xx"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/02",
                        ObservationItemCode = "2",
                        ObservationValue = "22"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "3",
                    prop2 = "yy",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "zzz"
                    }
                }
            };
        }

        private class ReferenceNotifySourceTestData : TestDataBase
        {
            public List<ReferenceNotifySourceModel> Data = new List<ReferenceNotifySourceModel>()
            {
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aa",
                    ConversionSquareMeters = 1000m,
                    Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                    Array = new List<string>() { "z", "zz", "zzz" },
                    ArrayData = new List<string>() { "z", "zz", "zzz" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pa", p2 = "paa" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "HA",
                    AreaUnitName = "ha",
                    ConversionSquareMeters = 10000m,
                    Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                    Array = new List<string>() { "y", "yy", "yyy" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pb", p2 = "pbb" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "M2",
                    AreaUnitName = "㎡",
                    ConversionSquareMeters = 1m,
                    Obj = new ReferenceNotifyObject() { prop1 = "c", prop2 = "cc", prop3 = "ccc" },
                    Array = new List<string>() { "x", "xx", "xxx" },
                    ArrayData = new List<string>() { "x", "xx", "xxx" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pc", p2 = "pcc" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "TB",
                    AreaUnitName = "坪",
                    ConversionSquareMeters = 3.305785m,
                    Obj = new ReferenceNotifyObject() { prop1 = "d", prop2 = "dd", prop3 = "ddd" },
                    Array = new List<string>() { "w", "ww", "www" },
                    ArrayData = new List<string>() { "w", "ww", "www" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pd", p2 = "pdd" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "UN",
                    AreaUnitName = "畝",
                    ConversionSquareMeters = 99736m,
                    Obj = new ReferenceNotifyObject() { prop1 = "e", prop2 = "ee", prop3 = "eee" },
                    Array = new List<string>() { "v", "vv", "vvv" },
                    ArrayData = new List<string>() { "v", "vv", "vvv" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pe", p2 = "pee" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "TN",
                    AreaUnitName = "反",
                    ConversionSquareMeters = 990m,
                    Obj = new ReferenceNotifyObject() { prop1 = "f", prop2 = "ff", prop3 = "fff" },
                    Array = new List<string>() { "u", "uu", "uuu" },
                    ArrayData = new List<string>() { "u", "uu", "uuu" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pf", p2 = "pff" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "CH",
                    AreaUnitName = "町（町歩）",
                    ConversionSquareMeters = 666.667m,
                    Obj = new ReferenceNotifyObject() { prop1 = "g", prop2 = "gg", prop3 = "ggg" },
                    Array = new List<string>() { "t", "tt", "ttt" },
                    ArrayData = new List<string>() { "t", "tt", "ttt" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pg", p2 = "pgg" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "XX",
                    AreaUnitName = "xx",
                    ConversionSquareMeters = 1000m,
                    Obj = new ReferenceNotifyObject() { prop1 = "h", prop2 = "hh", prop3 = "hhh" },
                    Array = new List<string>() { "s", "ss", "sss" },
                    ArrayData = new List<string>() { "s", "ss", "sss" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "ph", p2 = "phh" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "FL",
                    AreaUnitName = "FullData",
                    ConversionSquareMeters = 1m,
                    Array = new List<string>() { "s1", "s2", "s3" },
                    ArrayData = new List<string>() { "s1", "s2", "s3" },
                    NumberArray = new List<decimal?>() { 123, 456, 789 },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>()
                    {
                        new ReferenceNotifySourceArrayObjectItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new ReferenceNotifySourceArrayObjectItem() { Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    },
                    Object = new PropObject() { prop1 = "3", prop2 = "33", prop3 = new PropObjectChild() { prop31 = "333" } }
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "aa",
                    ConversionSquareMeters = 2000m,
                    Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                    Array = new List<string>() { "z", "zz", "zzz" },
                    ArrayData = new List<string>() { "z", "zz", "zzz" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pa", p2 = "paa" } }
                }
            };

            public ReferenceNotifySourceModel Data1 = new ReferenceNotifySourceModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1m
            };

            public ReferenceNotifySourceModel DataEcpectedShallowPath = new ReferenceNotifySourceModel()
            {
                id = "API~IntegratedTest~ReferenceNotifySource~1~AA",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aa",
                ConversionSquareMeters = 123m,
                ConversionSquareMeters2 = 9999m,
                Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Array = new List<string>() { "z", "zz", "zzz" },
                ArrayData = new List<string>() { "z", "zz", "zzz" },
                ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pa", p2 = "paa" } }
            };

            public ReferenceNotifySourceModel DataGetExpectedFL1 = new ReferenceNotifySourceModel()
            {
                id = "API~IntegratedTest~ReferenceNotifySource~1~FL",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "FL",
                AreaUnitName = "FullData",
                ConversionSquareMeters = 1m,
                Array = new List<string>() { "s1", "s2", "s3" },
                ArrayData = new List<string>() { "s1", "s2", "s3" },
                NumberArray = new List<decimal?>() { 123, 456, 789 },
                ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>()
                {
                    new ReferenceNotifySourceArrayObjectItem() { Date = "2020/12/12", ObservationItemCode = "7777", ObservationValue = ";6666" },
                    null,
                    new ReferenceNotifySourceArrayObjectItem() { Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                },
                Object = new PropObject() { prop1 = "3", prop2 = "33", prop3 = new PropObjectChild() { prop31 = "333" } }
            };

            public ReferenceNotifySourceModel DataGetExpectedFL2 = new ReferenceNotifySourceModel()
            {
                id = "API~IntegratedTest~ReferenceNotifySource~1~FL",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "FL",
                AreaUnitName = "FullData",
                ConversionSquareMeters = 1m,
                Array = new List<string>() { "s1", "s2", "s3" },
                ArrayData = new List<string>() { "s1", "s2", "s3" },
                NumberArray = new List<decimal?>() { 123, 456, 789 },
                ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>()
                {
                    new ReferenceNotifySourceArrayObjectItem() { Date = "2020/12/01", ObservationItemCode = "99;9", ObservationValue = "8;88" },
                },
                Object = new PropObject() { prop1 = "3", prop2 = "33", prop3 = new PropObjectChild() { prop31 = "333" } }
            };

            public ReferenceNotifySourceModel DataGetExpectedFL3 = new ReferenceNotifySourceModel()
            {
                id = "API~IntegratedTest~ReferenceNotifySource~1~FL",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "FL",
                AreaUnitName = "FullData",
                ConversionSquareMeters = 1m,
                Array = new List<string>() { "s1", "s2", "xyz" },
                ArrayData = new List<string>() { "s1", "s2", "s3" },
                NumberArray = new List<decimal?>() { 123, 10000, 789 },
                ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>()
                {
                    new ReferenceNotifySourceArrayObjectItem() { Date = "2099/12/31", ObservationItemCode = "1", ObservationValue = "11" },
                    null,
                    new ReferenceNotifySourceArrayObjectItem() { Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                },
                Object = new PropObject() { prop1 = "3", prop2 = "33", prop3 = new PropObjectChild() { prop31 = "****" } }
            };

            public List<ReferenceNotifySourceModelEx2> DataAllPropertyValueNull = new List<ReferenceNotifySourceModelEx2>()
            {
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "tst1",
                    ConversionSquareMeters = 100m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "HA",
                    AreaUnitName = "tst2",
                    ConversionSquareMeters = 200m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "M2",
                    AreaUnitName = "tst3",
                    ConversionSquareMeters = 300m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "TB",
                    AreaUnitName = "tst4",
                    ConversionSquareMeters = 400m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "UN",
                    AreaUnitName = "tst5",
                    ConversionSquareMeters = 500m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "TN",
                    AreaUnitName = "tst6",
                    ConversionSquareMeters = 600m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "CH",
                    AreaUnitName = "tst7",
                    ConversionSquareMeters = 700m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "XX",
                    AreaUnitName = "tst8",
                    ConversionSquareMeters = 800m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "FL",
                    AreaUnitName = "tst9",
                    ConversionSquareMeters = 900m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "tst10",
                    ConversionSquareMeters = 1000m,
                    Obj = new ReferenceNotifyObjectEx() {  prop1 = null, prop2 = null, prop3 = null },
                    Array = new List<string>() { null, null, null },
                    ArrayData = new List<string>() { null, null, null },
                    NumberArray = new List<decimal?>() { null, null, null },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItemEx>()
                    {
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null },
                        null,
                        new ReferenceNotifySourceArrayObjectItemEx() { Date = null, ObservationItemCode = null, ObservationValue = null }
                    },
                    Object = new PropObjectEx() { prop1 = null, prop2 = null, prop3 = new PropObjectChildEx() { prop31 = null } }
                }
            };

            public List<ReferenceNotifySourceModelEx2> DataAllNull = new List<ReferenceNotifySourceModelEx2>()
            {
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "tst1",
                    ConversionSquareMeters = 100m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "HA",
                    AreaUnitName = "tst2",
                    ConversionSquareMeters = 200m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "M2",
                    AreaUnitName = "tst3",
                    ConversionSquareMeters = 300m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "TB",
                    AreaUnitName = "tst4",
                    ConversionSquareMeters = 400m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "UN",
                    AreaUnitName = "tst5",
                    ConversionSquareMeters = 500m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "TN",
                    AreaUnitName = "tst6",
                    ConversionSquareMeters = 600m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "CH",
                    AreaUnitName = "tst7",
                    ConversionSquareMeters = 700m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "XX",
                    AreaUnitName = "tst8",
                    ConversionSquareMeters = 800m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "FL",
                    AreaUnitName = "tst9",
                    ConversionSquareMeters = 900m
                },
                new ReferenceNotifySourceModelEx2()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "tst10",
                    ConversionSquareMeters = 1000m
                }
            };

            public List<ReferenceNotifySourceModel> Data_NotifyPropertyMissing = new List<ReferenceNotifySourceModel>()
            {
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "tst1",
                    ConversionSquareMeters = 100m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "HA",
                    AreaUnitName = "tst2",
                    ConversionSquareMeters = 200m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "M2",
                    AreaUnitName = "tst3",
                    ConversionSquareMeters = 300m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "TB",
                    AreaUnitName = "tst4",
                    ConversionSquareMeters = 400m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "UN",
                    AreaUnitName = "tst5",
                    ConversionSquareMeters = 500m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "TN",
                    AreaUnitName = "tst6",
                    ConversionSquareMeters = 600m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "CH",
                    AreaUnitName = "tst7",
                    ConversionSquareMeters = 700m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "XX",
                    AreaUnitName = "tst8",
                    ConversionSquareMeters = 800m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "FL",
                    AreaUnitName = "tst9",
                    ConversionSquareMeters = 900m
                },
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "tst10",
                    ConversionSquareMeters = 1000m
                }
            };
        }

        private class ReferenceNotifyODataFirstTestData : TestDataBase
        {
            public ReferenceNotifyFirstModel Data1 = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifySource/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };
            public ReferenceNotifyFirstModel Data1_histCheck = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifySourceHistoryCheck/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };

            public ReferenceNotifyFirstModel Data2 = new ReferenceNotifyFirstModel()
            {
                Code = "02",
                Name = "002",
                temp = 100,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifySource/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };
            public ReferenceNotifyFirstModel Data2_histCheck = new ReferenceNotifyFirstModel()
            {
                Code = "02",
                Name = "002",
                temp = 100,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifySourceHistoryCheck/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };

            public ReferenceNotifyFirstModel Data99 = new ReferenceNotifyFirstModel()
            {
                Code = "99",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifySource/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };
            public ReferenceNotifyFirstModel Data99_histCheck = new ReferenceNotifyFirstModel()
            {
                Code = "99",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifySourceHistoryCheck/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };
        }

        private class ReferenceNotifyODataTestData
        {
            public ReferenceNotifyODataModelEx DataBefore = new ReferenceNotifyODataModelEx()
            {
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModelEx>()
                {
                    new ReferenceNotifySourceModelEx()
                    {
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bef", prop3 = "before" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1000m,
                        AreaUnitCode = "AA"
                    },
                }
            };

            public ReferenceNotifyODataModelEx DataAfter = new ReferenceNotifyODataModelEx()
            {
                Code = "01",
                Name = "002Aft",
                temp = 999,
                Ref1 = new List<ReferenceNotifySourceModelEx>()
                {
                    new ReferenceNotifySourceModelEx()
                    {
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA"
                    },
                }
            };
        }

        private class ReferenceNotifyODataNestSettingFirstTestData : TestDataBase
        {
            public ReferenceNotifyFirstModel Data1_histCheck = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifyOData/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };

            public ReferenceNotifyFirstModel Data2_histCheck = new ReferenceNotifyFirstModel()
            {
                Code = "02",
                Name = "002",
                temp = 100,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifyOData/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };

            public ReferenceNotifyFirstModel Data99_histCheck = new ReferenceNotifyFirstModel()
            {
                Code = "99",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference \"/API/IntegratedTest/ReferenceNotifyOData/OData?$select=id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData&$filter=AreaUnitName eq 'aa'\",id,AreaUnitCode,ConversionSquareMeters,AreaUnitName,Obj,ArrayData"
            };
        }

        private class ReferenceNotifyODataNestSettingTestData
        {
            public ReferenceNotifyODataNestModel Data1 = new ReferenceNotifyODataNestModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = new List<ReferenceNotifyODataModelEx>()
                {
                    new ReferenceNotifyODataModelEx()
                    {
                        Code = "01",
                        Name = "001",
                        temp = 99,
                        Ref1 = new List<ReferenceNotifySourceModelEx>()
                        {
                            new ReferenceNotifySourceModelEx()
                            {
                                ArrayData = null,
                                Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                                AreaUnitName = "aa",
                                ConversionSquareMeters = 1000m,
                                AreaUnitCode = "AA"
                            },
                            new ReferenceNotifySourceModelEx()
                            {
                                ArrayData = null,
                                Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                                AreaUnitName = "aa",
                                ConversionSquareMeters = 2000m,
                                AreaUnitCode = "BB"
                            }

                        }
                    }
                }
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);

            client = new IntegratedTestClient(AppConfig.Account);

            reference = UnityCore.Resolve<IReferenceNotifyApi>();
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            first = UnityCore.Resolve<IReferenceNotifyFirstApi>();
            var firstData = new ReferenceNotifyFirstTestData();
            client.GetWebApiResponseResult(first.Regist(firstData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data99)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data1_nulltest)).Assert(RegisterSuccessExpectStatusCode);

            src = UnityCore.Resolve<IReferenceNotifySourceApi>();
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(src.RegistList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
        }

        /// <summary>
        /// 履歴の削除を実施する
        /// </summary>
        private void DeleteHistory()
        {
            var reference = UnityCore.Resolve<IReferenceNotifyODataApi>();
            DeleteHistory(client, reference);

            var referenceHistCheck = UnityCore.Resolve<IReferenceNotifySourceHistCheckApi>();
            DeleteHistory(client, referenceHistCheck);

            var referenceOdataNest = UnityCore.Resolve<IReferenceNotifyODataNestSettingApi>();
            DeleteHistory(client, referenceOdataNest);
        }
        private void DeleteHistory<T>(IntegratedTestClient client, ICommonResource<T> api)
        {
            var response = client.GetWebApiResponseResult(api.OData("$select=id")).Assert(GetExpectStatusCodes);
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                var data = response.RawContentString.ToJson();
                foreach (var json in data)
                {
                    var id = json["id"].ToString();
                    client.GetWebApiResponseResult(api.HistoryThrowAway(id)).Assert(GetExpectStatusCodes);
                }
            }
        }

        [TestMethod]
        public void NotifyRegist_ShallowPath()
        {
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" }
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.Get("AA")).Assert(GetSuccessExpectStatusCode, srcData.DataEcpectedShallowPath);

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_ObjectArrayPart()
        {
            // 以下のObservationItemCode/ObservationValueの「;」について -->基盤文字列処理において、文字列中の「;(セミコロン)」でsplit処理されないことを確認する目的で付与している
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Ref9 = new PropArrayItem() { Date = "2020/12/12", ObservationItemCode = "7777", ObservationValue = ";6666" }
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode, srcData.DataGetExpectedFL1);

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_ObjectArrayFull()
        {
            // 以下のObservationItemCode/ObservationValueの「;」について -->基盤文字列処理において、文字列中の「;(セミコロン)」でsplit処理されないことを確認する目的で付与している
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Ref10 = new List<PropArrayItem>() { new PropArrayItem() { Date = "2020/12/01", ObservationItemCode = "99;9", ObservationValue = "8;88" } }
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode, srcData.DataGetExpectedFL2);

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_ShallowPath()
        {
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" }
            };
            client.GetWebApiResponseResult(reference.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);

            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.Get("AA")).Assert(GetSuccessExpectStatusCode, srcData.DataEcpectedShallowPath);

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_DeepPath()
        {
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****"
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode, srcData.DataGetExpectedFL3);

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_BadRequestTest()
        {
            // 更新失敗
            var data = new ReferenceNotifyModel()
            {
                Code = "99",
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref8 = "XYZ"
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_Rollback()
        {
            // 参照先の元データ
            var srcData = new ReferenceNotifySourceTestData();
            var org = new Dictionary<string, ReferenceNotifySourceModel>();
            var keysSource = srcData.Data.Select(x => x.AreaUnitCode).ToList();
            foreach (string key in keysSource)
            {
                org.Add(key, client.GetWebApiResponseResult(src.Get(key)).Assert(GetSuccessExpectStatusCode).Result);
            }

            // 変更前のデータを取得
            var org01 = client.GetWebApiResponseResult(reference.Get("99")).Assert(GetSuccessExpectStatusCode).Result;
            // 更新失敗
            var data = new ReferenceNotifyModelEx()
            {
                Code = "99",
                Ref1 = null,
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****"
            };
            client.GetWebApiResponseResult(reference.RegistEx(data)).Assert(BadRequestStatusCode);
            // データが変わっていないことを確認
            var ref01 = client.GetWebApiResponseResult(reference.Get("99")).Assert(GetSuccessExpectStatusCode).Result;
            ref01.IsStructuralEqual(org01);

            // 参照先が変わっていないことを確認
            foreach (var item in org)
            {
                client.GetWebApiResponseResult(src.Get(item.Key)).Assert(GetSuccessExpectStatusCode).Result.IsStructuralEqual(item.Value);
            }

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_Rollback()
        {
            // 参照先の元データ
            var srcData = new ReferenceNotifySourceTestData();
            var org = new Dictionary<string, ReferenceNotifySourceModel>();
            var keysSource = srcData.Data.Select(x => x.AreaUnitCode).ToList();
            foreach (string key in keysSource)
            {
                org.Add(key, client.GetWebApiResponseResult(src.Get(key)).Assert(GetSuccessExpectStatusCode).Result);
            }

            // 変更前のデータを取得
            var org01 = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).Result;
            // 更新失敗
            var data = new ReferenceNotifyModelEx()
            {
                Code = "01",
                Ref1 = null,
                Ref2 = 1000m,
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****"
            };
            client.GetWebApiResponseResult(reference.UpdateEx("01", data)).Assert(BadRequestStatusCode);
            // データが変わっていないことを確認
            var ref01 = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).Result;
            ref01.IsStructuralEqual(org01);

            // 参照先が変わっていないことを確認
            foreach (var item in org)
            {
                client.GetWebApiResponseResult(src.Get(item.Key)).Assert(GetSuccessExpectStatusCode).Result.IsStructuralEqual(item.Value);
            }

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_RollbackArray()
        {
            // 参照先の元データ
            var srcData = new ReferenceNotifySourceTestData();
            var org = new Dictionary<string, ReferenceNotifySourceModel>();
            var keysSource = srcData.Data.Select(x => x.AreaUnitCode).ToList();
            foreach (string key in keysSource)
            {
                org.Add(key, client.GetWebApiResponseResult(src.Get(key)).Assert(GetSuccessExpectStatusCode).Result);
            }

            // 変更前のデータを取得
            var org01 = client.GetWebApiResponseResult(reference.Get("99")).Assert(GetSuccessExpectStatusCode).Result;
            // 更新失敗
            var data = new List<ReferenceNotifyModelEx>()
            {
                new ReferenceNotifyModelEx()
                {
                    Code = "01",
                    Ref4 = "SmartFoodChain"
                },
                new ReferenceNotifyModelEx()
                {
                    Code = "99",
                    Ref1 = null,
                    Ref2 = 1000m,
                    Ref4 = "xyz",
                    Ref5 = 10000m,
                    Ref6 = "2099/12/31",
                    Ref7 = "****",
                    Ref8 = "XYZ"
                }
            };
            client.GetWebApiResponseResult(reference.RegistListEx(data)).Assert(BadRequestStatusCode);
            // データが変わっていないことを確認
            var ref01 = client.GetWebApiResponseResult(reference.Get("99")).Assert(GetSuccessExpectStatusCode).Result;
            ref01.IsStructuralEqual(org01);

            // 参照先が変わっていないことを確認
            foreach (var item in org)
            {
                client.GetWebApiResponseResult(src.Get(item.Key)).Assert(GetSuccessExpectStatusCode).Result.IsStructuralEqual(item.Value);
            }

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_OData_CheckHistoryHeader()
        {
            ODataTestInit(true);

            // RefSourceからデータ取得
            var response = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode);
            var data = response.Result;

            // Reference先の履歴ヘッダ Get系で返却されるので、チェック
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // isSelfチェック
            var self = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First();
            var refSource = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First();

            // 自身（Notify元) は、true
            self["isSelfHistory"].Is(true);
            // 以外は（Notify先) は、false
            refSource["isSelfHistory"].Is(false);

            var refSourceHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();
            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            // Reference先と元で、2件
            historyHeader.Count().Is(2);
            // Reference先は履歴2件
            refSourceHist.Count.Is(2);
            // Reference元は履歴1件
            refSelfHist.Count.Is(1);

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            //id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            //id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            // 履歴ヘッダ
            var regResponse = client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            historyHeader = regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // isSelfチェック
            self = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First();
            refSource = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First();

            // 自身（Notify元) は、true
            self["isSelfHistory"].Is(true);
            // 以外は（Notify先) は、false
            refSource["isSelfHistory"].Is(false);

            var refSourceHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();
            var refSelfHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            // Notify先と元で、2件
            historyHeader.Count().Is(2);
            // Notify先は履歴4件
            refSourceHist_after.Count.Is(4);
            // Notify元は履歴1件
            refSelfHist_after.Count.Is(1);
        }

        [TestMethod]
        public void NotifyRegist_OData_CheckHitorySnapshot()
        {
            ODataTestInit(true);

            // RefSourceからデータ取得
            var response = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode);
            var data = response.Result;
            var refdata = data.Ref1;

            // Ref1の1番目のデータは削除(=Delete)
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            // 履歴ヘッダ
            var regResponse = client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            var historyHeader = regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // 履歴がそれぞれあること
            notifySidx.IsNot(-1);
            notifyidx.IsNot(-1);

            // Notify元のAPIの履歴ヘッダ取得
            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();
            // Notify元は１回だけの更新なので、documentsの件数は1
            refSelfHist.Count.Is(1);

            // GetDocumentVersionを実行し、Latestのデータにsnapshot プロパティがあることを確認する
            var getResponse = client.GetWebApiResponseResult(referenceodata.GetDocumentVersion(refSelfHist[0]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnullでないこと
            var ver = getResponse.RawContentString.ToJson().ToList();
            var latest = ver[ver.Count - 1];
            latest["Snapshot"].IsNotNull();

            // Notify先のAPIの履歴ヘッダ取得
            var refSourceHist = historyHeader[notifySidx]["documents"].ToList();
            // Notify先は、documentsの件数は4
            refSourceHist.Count.Is(4);

            // １番目のNotify
            getResponse = client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.GetDocumentVersion(refSourceHist[0]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnull であること
            ver = getResponse.RawContentString.ToJson().ToList();
            latest = ver[ver.Count - 1];
            latest["Snapshot"].Type.Is(JTokenType.Null);

            // ２番目のNotify
            getResponse = client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.GetDocumentVersion(refSourceHist[1]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            //履歴の最新の「snapshot」がnullであること
            ver = getResponse.RawContentString.ToJson().ToList();
            latest = ver[ver.Count - 1];
            latest["Snapshot"].Type.Is(JTokenType.Null);

            // ３番目のNotify
            getResponse = client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.GetDocumentVersion(refSourceHist[2]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnullであること
            ver = getResponse.RawContentString.ToJson().ToList();
            latest = ver[ver.Count - 1];
            latest["Snapshot"].Type.Is(JTokenType.Null);
        }

        [TestMethod]
        public void NotifyRegist_Array_OData_CheckHitoryHeader()
        {
            ODataTestInit(true);

            // RefSourceからデータ取得
            var response = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode);
            var data = response.Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            var editdatas = new List<ReferenceNotifyODataModel>();
            var refId1 = $"API~IntegratedTest~ReferenceNotifyOData~{vendor}~{system}~1~01";
            var refId2 = $"API~IntegratedTest~ReferenceNotifyOData~{vendor}~{system}~1~02";
            data.id = refId1;
            editdatas.Add(JsonConvert.DeserializeObject<ReferenceNotifyODataModel>(JsonConvert.SerializeObject(data)));
            data.id = refId2;
            data.Code = "02";
            editdatas.Add(JsonConvert.DeserializeObject<ReferenceNotifyODataModel>(JsonConvert.SerializeObject(data)));

            var regResponse = client.GetWebApiResponseResult(referenceodata.RegistList(editdatas)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴ヘッダ
            var historyHeader = regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // 履歴がそれぞれあること
            notifySidx.IsNot(-1);
            notifyidx.IsNot(-1);

            // Notify元のAPIの履歴ヘッダ取得
            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();
            // Notify元は２回だけの更新なので、documentsの件数は2
            refSelfHist.Count.Is(2);

            // Notify先は、7件のはず
            var refSourceHist = historyHeader[notifySidx]["documents"].ToList();
            refSourceHist.Count.Is(7);
        }

        [TestMethod]
        public void NotifyUpdate_OData_CheckHistoryHeader()
        {
            ODataTestInit(true);

            // RefSourceからデータ取得
            var response = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode);
            var data = response.Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            // 履歴ヘッダ
            var updResponse = client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);
            var historyHeader = updResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // isSelfチェック
            var self = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First();
            var refSource = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First();

            // 自身（Notify元) は、true
            self["isSelfHistory"].Is(true);
            // 以外は（Notify先) は、false
            refSource["isSelfHistory"].Is(false);

            var refSourceHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"];
            var refSourceHistCount = refSourceHist.Count(x => (x["documentKey"].ToString() == id1 || x["documentKey"].ToString() == id2 || x["documentKey"].ToString() == id3));
            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"];
            var refSelfHistCount = refSelfHist.Count(x => (x["documentKey"].ToString().StartsWith("API~IntegratedTest~ReferenceNotifyOData")));

            // Notify先と元で、2件
            historyHeader.Count().Is(2);
            // Notify先は履歴3件
            refSourceHistCount.Is(3);
            // Notify元は履歴1件
            refSelfHistCount.Is(1);
        }

        [TestMethod]
        public void NotifyUpdate_OData_CheckHitorySnapshot()
        {
            ODataTestInit(true);

            // RefSourceからデータ取得
            var response = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode);
            var data = response.Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            // 履歴ヘッダ
            var updResponse = client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);
            var historyHeader = updResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // 履歴がそれぞれあること
            notifySidx.IsNot(-1);
            notifyidx.IsNot(-1);

            // Notify元のAPIの履歴ヘッダ取得
            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();
            // Notify元は１回だけの更新なので、documentsの件数は1
            refSelfHist.Count.Is(1);

            // GetDocumentVersionを実行し、Latestのデータにsnapshot プロパティがあることを確認する
            var getResponse = client.GetWebApiResponseResult(referenceodata.GetDocumentVersion(refSelfHist[0]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnullでないこと
            var ver = getResponse.RawContentString.ToJson().ToList();
            var latest = ver[ver.Count - 1];
            latest["Snapshot"].IsNotNull();

            // Notify先のAPIの履歴ヘッダ取得
            var refSourceHist = historyHeader[notifySidx]["documents"].ToList();
            // Notify先は、documentsの件数は4
            refSourceHist.Count.Is(4);

            // １番目のNotify
            getResponse = client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.GetDocumentVersion(refSourceHist[0]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnull であること
            ver = getResponse.RawContentString.ToJson().ToList();
            latest = ver[ver.Count - 1];
            latest["Snapshot"].Type.Is(JTokenType.Null);

            // ２番目のNotify
            getResponse = client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.GetDocumentVersion(refSourceHist[1]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnullであること
            ver = getResponse.RawContentString.ToJson().ToList();
            latest = ver[ver.Count - 1];
            latest["Snapshot"].Type.Is(JTokenType.Null);

            //３番目のNotify
            getResponse = client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.GetDocumentVersion(refSourceHist[2]["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode);

            // 履歴の最新の「snapshot」がnullであること
            ver = getResponse.RawContentString.ToJson().ToList();
            latest = ver[ver.Count - 1];
            latest["Snapshot"].Type.Is(JTokenType.Null);
        }

        [TestMethod]
        public void NotifyRegist_OData_CompareById()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var id2 = "API~IntegratedTest~ReferenceNotifySource~1~hogeId2";
            var id3 = "API~IntegratedTest~ReferenceNotifySource~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Patch 1件、Regist2件、Delete 1件で、差し引き beforeからの件数差分は　+1件のはず
            updated.Count().Is(beforeSourceData.Count() + 1);

            var ref1upd = updated.Where(x => x.id == id1);
            var updatedData = JToken.FromObject(ref1upd);
            // 更新データのチェック
            updatedData[0]["id"].Is(id1);
            updatedData[0]["ConversionSquareMeters"].Is(2000d);

            var ref2upd = updated.Where(x => x.id == id2);
            updatedData = JToken.FromObject(ref2upd);
            updatedData[0]["id"].Is(id2);
            updatedData[0]["ConversionSquareMeters"].Is(5000d);
            updatedData[0]["ArrayData"].Is(@"[""y"",""yy"",""yyy""]".ToJson());

            var ref3upd = updated.Where(x => x.id == id3);
            updatedData = JToken.FromObject(ref3upd);
            updatedData[0]["id"].Is(id3);
            updatedData[0]["ConversionSquareMeters"].Is(7000d);
            updatedData[0]["ArrayData"].Is(@"[""b"",""bb"",""bbb""]".ToJson());
        }

        [TestMethod]
        public void NotifyRegist_OData_CompareByRequestNull()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Notifyプロパティのデータをnull にする
            var overrideData = new ReferenceNotifyODataModelEx()
            {
                Code = data.Code,
                Name = data.Name,
                temp = data.temp,
                Ref1 = null
            };

            // 履歴ヘッダ
            var regResponse = client.GetWebApiResponseResult(referenceodata.RegistEx(overrideData)).Assert(RegisterSuccessExpectStatusCode);
            var historyHeader = regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // Notify元の履歴のみであること
            notifySidx.Is(-1);
            notifyidx.IsNot(-1);

            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();

            // Delete動作となるため、NotifySourceの履歴ヘッダがなく、Notify元の履歴ヘッダが1件のみであること
            historyHeader.Count().Is(2);
            refSelfHist.Count.Is(1);

            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Delete 2件走るはずなので、処理後は、処理前より2件減っていること（全件削除動作ではないこと)
            updated.Count().IsNot(0);
            updated.Count().Is(beforeSourceData.Count() - 2);
        }

        [TestMethod]
        public void NotifyRegist_OData_CompareByRequestEmptyArray()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Notifyプロパティのデータを空配列 にする
            data.Ref1 = new List<ReferenceNotifySourceModel>();

            // 履歴ヘッダ
            var regResponse = client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            var historyHeader = regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // Notify元の履歴のみであること
            notifySidx.Is(-1);
            notifyidx.IsNot(-1);

            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();

            // Delete動作となるため、NotifySourceの履歴ヘッダがなく、Notify元の履歴ヘッダが1件のみであること
            historyHeader.Count().Is(2);
            refSelfHist.Count.Is(1);

            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            //Delete 2件走るはずなので、処理後は、処理前より2件減っていること（全件削除動作ではないこと)
            updated.Count().IsNot(0);
            updated.Count().Is(beforeSourceData.Count() - 2);
        }

        [TestMethod]
        public void NotifyRegist_ErrorIdAndRepoKeyNotFound()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            var refdata = data.Ref1;
            // Ref1の1番目のデータは削除(=Delete)
            refdata.RemoveAt(1);

            // id削除
            refdata[0].id = null;
            // RepositoryKey削除
            refdata[0].AreaUnitCode = null;

            client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_OData_CompareByRepositoryKey()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            var id1 = refdata[0].id;
            var id2 = "API~IntegratedTest~ReferenceNotifySource~1~H2";
            var id3 = "API~IntegratedTest~ReferenceNotifySource~1~H3";

            // repoKeyで比較したいので、id削除
            refdata[0].id = null;

            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            updData.ConversionSquareMeters = 2000m;
            updData.AreaUnitCode = "AA";

            // id2 の要素追加(=Regist)
            regData1.AreaUnitCode = "H2";
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.AreaUnitCode = "H3";
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Patch 1件、Regist2件、Delete 1件で、差し引き beforeからの件数差分は　+1件のはず
            updated.Count().Is(beforeSourceData.Count() + 1);

            var updatedData = updated.Where(x => x.id == id1).ToList();
            // 更新データのチェック
            updatedData[0].id.Is(id1);
            updatedData[0].ConversionSquareMeters.Is(2000m);

            updatedData = updated.Where(x => x.id == id2).ToList();
            updatedData[0].id.Is(id2);
            updatedData[0].ConversionSquareMeters.Is(5000m);
            updatedData[0].ArrayData.Is(new List<string>() { "y", "yy", "yyy" });

            updatedData = updated.Where(x => x.id == id3).ToList();
            updatedData[0].id.Is(id3);
            updatedData[0].ConversionSquareMeters.Is(7000m);
            updatedData[0].ArrayData.Is(new List<string>() { "b", "bb", "bbb" });
        }

        [TestMethod]
        public void NotifyUpdate_OData_CompareById()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var id2 = "API~IntegratedTest~ReferenceNotifySource~1~hogeId2";
            var id3 = "API~IntegratedTest~ReferenceNotifySource~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);
            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Patch 1件、Regist1件、Delete 0件で、差し引き beforeからの件数差分は　+1件のはず
            updated.Count().Is(beforeSourceData.Count() + 1);

            var updatedData = updated.Where(x => x.id == id1).ToList();
            // 更新データのチェック
            updatedData[0].id.Is(id1);
            updatedData[0].ConversionSquareMeters.Is(2000m);

            updatedData = updated.Where(x => x.id == id2).ToList();
            updatedData[0].id.Is(id2);
            updatedData[0].ConversionSquareMeters.Is(5000m);
            updatedData[0].ArrayData.Is(new List<string>() { "y", "yy", "yyy" });

            updatedData = updated.Where(x => x.id == id3).ToList();
            updatedData[0].id.Is(id3);
            updatedData[0].ConversionSquareMeters.Is(7000m);
            updatedData[0].ArrayData.Is(new List<string>() { "b", "bb", "bbb" });

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_OData_CompareByRepositoryKey()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            var refdata = data.Ref1;
            // Ref1の1番目のデータは削除(=Delete)
            refdata.RemoveAt(1);

            var id1 = refdata[0].id;
            var id2 = "API~IntegratedTest~ReferenceNotifySource~1~H2";
            var id3 = "API~IntegratedTest~ReferenceNotifySource~1~H3";

            // repoKeyで比較したいので、id削除
            refdata[0].id = null;

            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            updData.ConversionSquareMeters = 2000m;
            updData.AreaUnitCode = "AA";

            // id2 の要素追加(=Regist)
            regData1.AreaUnitCode = "H2";
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.AreaUnitCode = "H3";
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);
            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Patch 1件、Regist2件、Delete 1件で、差し引き beforeからの件数差分は　+1件のはず
            updated.Count().Is(beforeSourceData.Count() + 1);

            var updatedData = updated.Where(x => x.id == id1).ToList();
            // 更新データのチェック
            updatedData[0].id.Is(id1);
            updatedData[0].ConversionSquareMeters.Is(2000m);

            updatedData = updated.Where(x => x.id == id2).ToList();
            updatedData[0].id.Is(id2);
            updatedData[0].ConversionSquareMeters.Is(5000m);
            updatedData[0].ArrayData.Is(new List<string>() { "y", "yy", "yyy" });

            updatedData = updated.Where(x => x.id == id3).ToList();
            updatedData[0].id.Is(id3);
            updatedData[0].ConversionSquareMeters.Is(7000m);
            updatedData[0].ArrayData.Is(new List<string>() { "b", "bb", "bbb" });
        }

        [TestMethod]
        public void NotifyUpdate_OData_CompareByRequestNull()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Notifyプロパティのデータをnull にする
            var newData = new ReferenceNotifyODataModelEx()
            {
                Code = data.Code,
                Name = data.Name,
                temp = data.temp,
                Ref1 = null
            };
            var updResponse = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", newData)).Assert(UpdateSuccessExpectStatusCode);

            // 履歴ヘッダ
            var historyHeader = updResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // Notify元の履歴のみであること
            notifySidx.Is(-1);
            notifyidx.IsNot(-1);

            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();

            // Delete動作となるため、NotifySourceの履歴ヘッダがなく、Notify元の履歴ヘッダが1件のみであること
            historyHeader.Count().Is(2);
            refSelfHist.Count.Is(1);

            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Delete 2件走るはずなので、処理後は、処理前より2件減っていること（全件削除動作ではないこと)
            updated.Count().IsNot(0);
            updated.Count().Is(beforeSourceData.Count() - 2);
        }

        [TestMethod]
        public void NotifyUpdate_OData_CompareByRequestEmptyArray()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            var beforeSourceData = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Notifyプロパティのデータを空配列 にする
            data.Ref1 = new List<ReferenceNotifySourceModel>();

            var updResponse = client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);

            // 履歴ヘッダ
            var historyHeader = updResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();
            // Notify元先のIndex
            var notifySidx = -1;
            var notifyidx = -1;
            for (int i = 0; i < historyHeader.Count; i++)
            {
                if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData")
                {
                    notifyidx = i;
                }
                else if (historyHeader[i]["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck")
                {
                    notifySidx = i;
                }
            }
            // Notify元の履歴のみであること
            notifySidx.Is(-1);
            notifyidx.IsNot(-1);

            var refSelfHist = historyHeader[notifyidx]["documents"].ToList();

            // Delete動作となるため、NotifySourceの履歴ヘッダがなく、Notify元の履歴ヘッダが1件のみであること
            historyHeader.Count().Is(2);
            refSelfHist.Count.Is(1);

            var updated = client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Delete 2件走るはずなので、処理後は、処理前より2件減っていること（全件削除動作ではないこと)
            updated.Count().IsNot(0);
            updated.Count().Is(beforeSourceData.Count() - 2);
        }

        [TestMethod]
        public void NotifyUpdate_OData_ErrorIdAndRepoKeyNotFound()
        {
            ODataTestInit();

            // RefSourceからデータ取得
            client.GetWebApiResponseResult(src.OData()).Assert(GetSuccessExpectStatusCode);
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            var refdata = data.Ref1;
            // Ref1の1番目のデータは削除(=Delete)
            refdata.RemoveAt(1);

            // ID削除
            refdata[0].id = null;
            // RepositoryKeyも削除
            refdata[0].AreaUnitCode = null;

            client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_OData_Rollback()
        {
            ODataTestInit();

            // 参照先の元データ
            var srcData = new ReferenceNotifySourceTestData();
            var org = new Dictionary<string, ReferenceNotifySourceModel>();
            var keysSource = srcData.Data.Select(x => x.AreaUnitCode).ToList();
            foreach (string key in keysSource)
            {
                org.Add(key, client.GetWebApiResponseResult(src.Get(key)).Assert(GetSuccessExpectStatusCode).Result);
            }

            // 変更前のデータを取得
            var org01 = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // データ取得
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1を取得し、編集してNotifyする
            var ref1 = data.Ref1;
            var updData = ref1[0];
            var regData = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(ref1[0]));

            // 必須プロパティを抜く
            regData.AreaUnitName = null;

            var id1 = ref1[0].id;
            var id2 = "API~IntegratedTest~ReferenceNotifySource~1~hogeId2";

            updData.ConversionSquareMeters = 2000m;

            regData.id = id2;
            regData.ConversionSquareMeters = 5000m;
            regData.ArrayData = new List<string>() { "y", "yy", "yyy" };
            ref1.Add(regData);
            data.Ref1 = ref1;

            // 更新失敗
            var response = client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(BadRequestStatusCode);

            // RefSourceのエラー内容が入っていること
            var resMsg = response.RawContentString;
            resMsg.Contains(@"{""AreaUnitName"":[""Required properties are missing from object: AreaUnitName.(code:14)""]}").Is(true);
            var resList = resMsg.ToJson();

            resList[0]["error_code"].Value<string>().Is("E10401");
            resList[1]["error_code"].Value<string>().Is("E10403");

            // データが変わっていないことを確認
            var ref01 = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;
            ref01.IsStructuralEqual(org01);

            // 参照先が変わっていないことを確認
            foreach (var item in org)
            {
                client.GetWebApiResponseResult(src.Get(item.Key)).Assert(GetSuccessExpectStatusCode, item.Value);
            }

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdateOData_Rollback()
        {
            ODataTestInit();

            // 参照先の元データ
            var srcData = new ReferenceNotifySourceTestData();
            var org = new Dictionary<string, ReferenceNotifySourceModel>();
            var keysSource = srcData.Data.Select(x => x.AreaUnitCode).ToList();
            foreach (string key in keysSource)
            {
                org.Add(key, client.GetWebApiResponseResult(src.Get(key)).Assert(GetSuccessExpectStatusCode).Result);
            }

            // 変更前のデータを取得
            var org01 = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // データ取得
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1を取得し、編集してNotifyする
            var ref1 = data.Ref1;
            var updData = ref1[0];
            var regData = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(ref1[0]));

            // 必須プロパティを抜く
            regData.AreaUnitName = null;

            var id1 = ref1[0].id;
            var id2 = "API~IntegratedTest~ReferenceNotifySource~1~hogeId2";

            updData.ConversionSquareMeters = 2000m;

            regData.id = id2;
            regData.ConversionSquareMeters = 5000m;
            regData.ArrayData = new List<string>() { "y", "yy", "yyy" };
            ref1.Add(regData);
            data.Ref1 = ref1;

            // 更新失敗
            var response = client.GetWebApiResponseResult(referenceodata.Update("01", data)).AssertErrorCode(BadRequestStatusCode, "E10401");

            // RefSourceのエラー内容が入っていること
            var resMsg = response.ContentString;
            resMsg.Contains(@"{""AreaUnitName"":[""Required properties are missing from object: AreaUnitName.(code:14)""]}").Is(true);

            // データが変わっていないことを確認
            var ref01 = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;
            ref01.IsStructuralEqual(org01);

            // 参照先が変わっていないことを確認
            foreach (var item in org)
            {
                client.GetWebApiResponseResult(src.Get(item.Key)).Assert(GetSuccessExpectStatusCode, item.Value);
            }

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_OData_CheckHistoryHeader_NestSetting()
        {
            ODataTestInit(true);

            // nestsettingを初期化
            var firstodataNestsettingData = new ReferenceNotifyODataNestSettingFirstTestData();
            client.GetWebApiResponseResult(firstodataNestsetting.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(firstodataNestsetting.Register(firstodataNestsettingData.Data1_histCheck)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodataNestsetting.Register(firstodataNestsettingData.Data2_histCheck)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodataNestsetting.Register(firstodataNestsettingData.Data99_histCheck)).Assert(RegisterSuccessExpectStatusCode);

            // nestデータ登録
            var testData = new ReferenceNotifyODataNestSettingTestData();
            var response = client.GetWebApiResponseResult(referenceodataNestsetting.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴ヘッダチェック
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelf = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyODataNestSetting").First()["documents"].ToList();
            var refMiddle = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();
            var refEnd = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();

            // 自身は１回更新
            refSelf.Count.Is(1);
            // 中間のAPIも、Regist１件なので、１件
            refMiddle.Count.Is(1);
            // 最後のは、ネストされた配列２件分の更新なので、２件
            refEnd.Count.Is(2);
        }

        [TestMethod]
        public void NotifyUpdate_OData_CheckHistoryHeader_NestSetting()
        {
            ODataTestInit(true);

            // nestsettingを初期化
            var firstodataNestsettingData = new ReferenceNotifyODataNestSettingFirstTestData();
            client.GetWebApiResponseResult(firstodataNestsetting.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(firstodataNestsetting.Register(firstodataNestsettingData.Data1_histCheck)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodataNestsetting.Register(firstodataNestsettingData.Data2_histCheck)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodataNestsetting.Register(firstodataNestsettingData.Data99_histCheck)).Assert(RegisterSuccessExpectStatusCode);

            // nestデータ登録
            var testData = new ReferenceNotifyODataNestSettingTestData();
            var response = client.GetWebApiResponseResult(referenceodataNestsetting.Update("01", testData.Data1)).Assert(UpdateSuccessExpectStatusCode);

            // 履歴ヘッダチェック
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelf = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyODataNestSetting").First()["documents"].ToList();
            var refMiddle = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();
            var refEnd = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();

            // 自身は１回更新
            refSelf.Count.Is(1);
            // 中間のAPIも、Regist１件なので、１件
            refMiddle.Count.Is(1);
            // 最後のは、ネストされた配列２件分の更新なので、２件
            refEnd.Count.Is(2);
        }

        [TestMethod]
        public void NotifyRegist_OData_CheckHistoryHeader_HistSettingToNoHistSetting()
        {
            // historyのfirst初期化をスキップ
            ODataTestInit(true, true);

            // 履歴設定の無い方に、Notifyを向ける
            var firstodataData = new ReferenceNotifyODataFirstTestData();
            client.GetWebApiResponseResult(firstodata.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data99)).Assert(RegisterSuccessExpectStatusCode);

            // RefSourceからデータ取得
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            //id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            //id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            // 履歴ヘッダ
            var response = client.GetWebApiResponseResult(referenceodata.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSourceHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySource").First()["documents"];
            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            // Notify先は履歴0件(null)
            refSourceHist.Type.Is(JTokenType.Null);
            // Notify元は履歴1件
            refSelfHist.Count.Is(1);
        }

        [TestMethod]
        public void NotifyRegist_NoHistSettingToHistSetting()
        {
            ODataTestInit(true);

            // first履歴設定ありAPIに向ける
            var firstData = new ReferenceNotifyFirstTestData();
            client.GetWebApiResponseResult(first.Regist(firstData.Data1_toHistSettingApi)).Assert(RegisterSuccessExpectStatusCode);

            // regist
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" }
            };
            var response = client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴ヘッダ
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSourceHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();
            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotify").First()["documents"];

            // Notify先は履歴0件(null)
            refSelfHist.Type.Is(JTokenType.Null);
            // Notify元は履歴1件
            refSourceHist.Count.Is(1);

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_OData_CheckHistoryHeader_HistSettingToNoHistSetting()
        {
            // historyのfirst初期化をスキップ
            ODataTestInit(true, true);

            // 履歴設定の無い方に、Notifyを向ける
            var firstodataData = new ReferenceNotifyODataFirstTestData();
            client.GetWebApiResponseResult(firstodata.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data99)).Assert(RegisterSuccessExpectStatusCode);

            // RefSourceからデータ取得
            var data = client.GetWebApiResponseResult(referenceodata.Get("01")).Assert(GetSuccessExpectStatusCode).Result;

            // Ref1の1番目のデータは削除(=Delete)
            var refdata = data.Ref1;
            refdata.RemoveAt(1);

            // Ref1の0番目を編集(=Patch)
            var updData = refdata[0];
            var regData1 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));
            var regData2 = JsonConvert.DeserializeObject<ReferenceNotifySourceModel>(JsonConvert.SerializeObject(refdata[0]));

            var id1 = refdata[0].id;
            var vendor = client.VendorSystemInfo.VendorId;
            var system = client.VendorSystemInfo.SystemId;
            var id2 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId2";
            var id3 = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{vendor}~{system}~1~hogeId3";

            updData.ConversionSquareMeters = 2000m;

            // id2 の要素追加(=Regist)
            regData1.id = id2;
            regData1.ConversionSquareMeters = 5000m;
            regData1.ArrayData = new List<string>() { "y", "yy", "yyy" };
            refdata.Add(regData1);

            // id3 の要素追加(=Regist)
            regData2.id = id3;
            regData2.ConversionSquareMeters = 7000m;
            regData2.ArrayData = new List<string>() { "b", "bb", "bbb" };
            refdata.Add(regData2);

            data.Ref1 = refdata;

            //履歴ヘッダ
            var response = client.GetWebApiResponseResult(referenceodata.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSourceHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySource").First()["documents"];
            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            // Notify先は履歴0件(null)
            refSourceHist.Type.Is(JTokenType.Null);
            // Notify元は履歴1件
            refSelfHist.Count.Is(1);
        }

        [TestMethod]
        public void NotifyUpdate_NoHistSettingToHistSetting()
        {
            ODataTestInit(true);

            // first履歴設定ありAPIに向ける
            var firstData = new ReferenceNotifyFirstTestData();
            client.GetWebApiResponseResult(first.Regist(firstData.Data1_toHistSettingApi)).Assert(RegisterSuccessExpectStatusCode);

            // regist
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" }
            };
            var response = client.GetWebApiResponseResult(reference.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);

            // 履歴ヘッダ
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSourceHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();
            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotify").First()["documents"];

            // Notify先は履歴0件(null)
            refSelfHist.Type.Is(JTokenType.Null);
            // Notify元は履歴1件
            refSourceHist.Count.Is(1);

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_OData_ReferenceHistory()
        {
            ODataTestInit(true);

            // データ登録
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataBefore)).Assert(RegisterSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ登録(上書き)
            response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataAfter)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴ヘッダ
            historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();
            var refSourceHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"];

            JArray docs = new JArray();
            // 履歴ヘッダ分ループ
            foreach (var h1 in refSourceHist_after)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.First().AddAfterSelf(new JProperty("versionKey", h1["versionKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }
            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // Selfは過去版、RefSourceは最新版となるはず
            var expectData = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}",
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA"
                    }
                }
            };
            histRefData.IsStructuralEqual(expectData);
        }

        [TestMethod]
        public void NotifyRegist_OData_ReferenceHistory_UseSnapshot()
        {
            ODataTestInit(true);

            // データ登録
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataBefore)).Assert(RegisterSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ登録(上書き)
            response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataAfter)).Assert(RegisterSuccessExpectStatusCode);

            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true
}}";

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // $Referenceが解決されていること
            var expectData = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}",
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bef", prop3 = "before" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1000m,
                        AreaUnitCode = "AA"
                    }
                }
            };
            histRefData.IsStructuralEqual(expectData);
        }

        [TestMethod]
        public void NotifyRegist_OData_ReferenceHistory_ReferenceFalse()
        {
            ODataTestInit(true);

            // データ登録
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataBefore)).Assert(RegisterSuccessExpectStatusCode);
            var data = response.Result;

            //　あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ登録(上書き)
            response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataAfter)).Assert(RegisterSuccessExpectStatusCode);

            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':false
}}";

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();

            // $Referenceが解決されていないこと
            var expectData = @"{
    'id':'{{*}}',
    '_Owner_Id':'{{*}}',
    'Code': '01',
    'Name': '001Bef',
    'temp': 99,
    'Ref1': '$Reference{{*}}'
  }".ToJson();
            histRefData.Is(expectData);
        }

        [TestMethod]
        public void NotifyUpdate_OData_ReferenceHistory()
        {
            ODataTestInit(true);

            // データ更新
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataBefore)).Assert(UpdateSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ更新(上書き)
            response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataAfter)).Assert(UpdateSuccessExpectStatusCode);

            // 履歴ヘッダ
            historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();
            var refSourceHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"];

            JArray docs = new JArray();
            // 履歴ヘッダ分ループ
            foreach (var h1 in refSourceHist_after)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.First().AddAfterSelf(new JProperty("versionKey", h1["versionKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }
            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // Selfは過去版、RefSourceは最新版となるはず
            var expectData = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}",
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA"
                    }
                }
            };
            histRefData.IsStructuralEqual(expectData);
        }

        [TestMethod]
        public void NotifyUpdate_OData_ReferenceHistory_UseSnapshot()
        {
            ODataTestInit(true);

            // データ更新
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataBefore)).Assert(UpdateSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ更新(上書き)
            response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataAfter)).Assert(UpdateSuccessExpectStatusCode);

            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true
}}";

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // $Referenceが解決されていること
            var expectData = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}",
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bef", prop3 = "before" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1000m,
                        AreaUnitCode = "AA"
                    }
                }
            };
            histRefData.IsStructuralEqual(expectData);
        }

        [TestMethod]
        public void NotifyUpdate_OData_ReferenceHistory_ReferenceFalse()
        {
            ODataTestInit(true);

            // データ更新
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataBefore)).Assert(UpdateSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ更新(上書き)
            response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataAfter)).Assert(UpdateSuccessExpectStatusCode);

            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':false
}}";

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();

            // $Referenceが解決されていないこと
            var expectData = @"{
    'id':'{{*}}',
    '_Owner_Id':'{{*}}',
    'Code': '01',
    'Name': '001Bef',
    'temp': 99,
    'Ref1': '$Reference{{*}}'
  }".ToJson();
            histRefData.Is(expectData);
        }

        [TestMethod]
        public void NotifyRegist_ReferenceHistory_ReferenceSourceIsEmpty()
        {
            ODataTestInit(true);

            // データ登録
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataBefore)).Assert(RegisterSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ登録(上書き)
            response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataAfter)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴ヘッダ
            historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();
            var refSourceHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"];

            JArray docs = new JArray();
            // 履歴ヘッダ分ループ
            foreach (var h1 in refSourceHist_after)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.First().AddAfterSelf(new JProperty("versionKey", h1["versionKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }

            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            // Reference先をクリアする
            referenceNotifySourceHistCheckApi = UnityCore.Resolve<IReferenceNotifySourceHistCheckApi>();
            client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.DeleteAll()).Assert(DeleteSuccessStatusCode);

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // Referece先のデータが無いので、null になること
            var expectData = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}",
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA"
                    }
                }
            };
            histRefData.IsStructuralEqual(expectData);
        }

        [TestMethod]
        public void NotifyUpdate_ReferenceHistory_ReferenceSourceIsEmpty()
        {
            ODataTestInit(true);

            // データ更新
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataBefore)).Assert(UpdateSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHist[0]["documentKey"].ToString();
            var selfVerKey = refSelfHist[0]["versionKey"].ToString();

            // データ登録(上書き)
            response = client.GetWebApiResponseResult(referenceodata.UpdateEx("01", testData.DataAfter)).Assert(UpdateSuccessExpectStatusCode);

            // 履歴ヘッダ
            historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();
            var refSourceHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"];

            JArray docs = new JArray();
            // 履歴ヘッダ分ループ
            foreach (var h1 in refSourceHist_after)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.First().AddAfterSelf(new JProperty("versionKey", h1["versionKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }

            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            // Reference先をクリアする
            referenceNotifySourceHistCheckApi = UnityCore.Resolve<IReferenceNotifySourceHistCheckApi>();
            client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.DeleteAll()).Assert(DeleteSuccessStatusCode);

            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // Referece先のデータが無くても、履歴データが引けること
            var expectData = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}",
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA"
                    }
                }
            };
            histRefData.IsStructuralEqual(expectData);
        }

        [TestMethod]
        public void NotifyRegist_NullAllowedPropertyTest()
        {
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Ref11 = false,
                Ref12 = 1m,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'ArrayData' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {{
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      }},
      null,
      {{
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      }},
    ],
    'Object' : {{
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {{
        'prop31' : '333'
      }},
    }},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_NullAllowedPropertyTest_nullNotify()
        {
            var data = new ReferenceNotifyModelEx2()
            {
                Code = "01",
                Ref11 = null,
                Ref12 = null,
                Ref13 = null
            };
            client.GetWebApiResponseResult(reference.RegistEx2(data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'ArrayData' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {{
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      }},
      null,
      {{
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      }},
    ],
    'Object' : {{
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {{
        'prop31' : '333'
      }},
    }},
    'BooleanNullProp':null,
    'NumberNullProp':null,
    'StringNullProp':null,
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_NullAllowedPropertyTest_nullNotify_sourcePropertyValuesAreNull()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListEx2(srcData.DataAllPropertyValueNull)).Assert(RegisterSuccessExpectStatusCode);

            var data = new ReferenceNotifyModel()
            {
                Code = "10",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref11 = false,
                Ref12 = 1m,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : [null,null,'xyz'],
    'ArrayData' : [null,null,null],
    'NumberArray' : [null,10000,null],
    'ArrayObject' :[
                    {{ 'Date': '2099/12/31','ObservationItemCode': null,'ObservationValue': null}},
                    null,
                    {{'Date': null,'ObservationItemCode': null,'ObservationValue': null}}
                   ],
    'Object' :{{ 'prop1': null,'prop2': null,'prop3': {{
        'prop31': '****'
      }}}},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_NullAllowedPropertyTest_nullNotify_sourceNull()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListEx2(srcData.DataAllNull)).Assert(RegisterSuccessExpectStatusCode);

            var data = new ReferenceNotifyModel()
            {
                Code = "10",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref11 = false,
                Ref12 = 1m,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : ['xyz'],
    'ArrayData' : null,
    'NumberArray' : [10000],
    'ArrayObject' :[{{'Date': '2099/12/31'}}],
    'Object' :{{'prop3': {{'prop31':'****'}}}},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_NullAllowedPropertyTest_nullNotify_NotifyPropertyMissing()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistList(srcData.Data_NotifyPropertyMissing)).Assert(RegisterSuccessExpectStatusCode);

            var data = new ReferenceNotifyModel()
            {
                Code = "10",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref11 = false,
                Ref12 = 1m,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Regist(data)).Assert(RegisterSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : ['xyz'],
    'NumberArray' : [10000],
    'ArrayObject' :[{{'Date': '2099/12/31'}}],
    'Object' :{{'prop3': {{'prop31':'****'}}}},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_NullAllowedPropertyTest_nullNotify_NotifyPropertyMissing()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistList(srcData.Data_NotifyPropertyMissing)).Assert(RegisterSuccessExpectStatusCode);

            var data = new ReferenceNotifyModel()
            {
                Code = "10",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref11 = false,
                Ref12 = 1m,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Update("10", data)).Assert(UpdateSuccessExpectStatusCode);

            string expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : ['xyz'],
    'NumberArray' : [10000],
    'ArrayObject' :[{{'Date': '2099/12/31'}}],
    'Object' :{{'prop3': {{'prop31':'****'}}}},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyRegist_NullAllowedPropertyTest_RollbackTest()
        {
            var data = new ReferenceNotifyModelEx3()
            {
                Code = "10",
                Ref11 = "NotBoolean",
                Ref12 = "NotNumber",
                Ref13 = null
            };
            client.GetWebApiResponseResult(reference.RegistEx3(data)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            string expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'ArrayData' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {{
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      }},
      null,
      {{
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      }},
    ],
    'Object' : {{
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {{
        'prop31' : '333'
      }},
    }},
    'BooleanNullProp':null,
    'NumberNullProp':null,
    'StringNullProp':null,
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_NullAllowedPropertyTest()
        {
            var data = new ReferenceNotifyModel()
            {
                Code = "01",
                Ref2 = 1,
                Ref11 = false,
                Ref12 = 1,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Update("01", data)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            string expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'ArrayData' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {{
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      }},
      null,
      {{
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      }},
    ],
    'Object' : {{
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {{
        'prop31' : '333'
      }},
    }},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_NullAllowedPropertyTest_nullNotify()
        {
            var data = new ReferenceNotifyModelEx2()
            {
                Code = "01",
                Ref2 = 1,
                Ref11 = null,
                Ref12 = null,
                Ref13 = null
            };
            client.GetWebApiResponseResult(reference.UpdateEx2("01", data)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            string expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'ArrayData' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {{
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      }},
      null,
      {{
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      }},
    ],
    'Object' : {{
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {{
        'prop31' : '333'
      }},
    }},
    'BooleanNullProp':null,
    'NumberNullProp':null,
    'StringNullProp':null,
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_NullAllowedPropertyTest_nullNotify_sourcePropertyValuesAreNull()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListEx2(srcData.DataAllPropertyValueNull)).Assert(RegisterSuccessExpectStatusCode);

            var data = new ReferenceNotifyModel()
            {
                Code = "10",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref11 = false,
                Ref12 = 1,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Update("10", data)).Assert(UpdateSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : [null,null,'xyz'],
    'ArrayData' : [null,null,null],
    'NumberArray' : [null,10000,null],
    'ArrayObject' :[
                    {{ 'Date': '2099/12/31','ObservationItemCode': null,'ObservationValue': null}},
                    null,
                    {{'Date': null,'ObservationItemCode': null,'ObservationValue': null}}
                   ],
    'Object' :{{ 'prop1': null,'prop2': null,'prop3': {{
        'prop31': '****'
      }}}},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_NullAllowedPropertyTest_nullNotify_sourceNull()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListEx2(srcData.DataAllNull)).Assert(RegisterSuccessExpectStatusCode);

            var data = new ReferenceNotifyModel()
            {
                Code = "10",
                Ref1 = 123,
                Ref2 = 9999,
                Ref3 = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bb", prop3 = "bbb" },
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****",
                Ref11 = false,
                Ref12 = 1,
                Ref13 = "hoge"
            };
            client.GetWebApiResponseResult(reference.Update("10", data)).Assert(UpdateSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : ['xyz'],
    'ArrayData' : null,
    'NumberArray' : [10000],
    'ArrayObject' :[{{'Date': '2099/12/31'}}],
    'Object' :{{'prop3': {{'prop31':'****'}}}},
    'BooleanNullProp':false,
    'NumberNullProp':1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyUpdate_NullAllowedPropertyTest_RollbackTest()
        {
            var data = new ReferenceNotifyModelEx3()
            {
                Code = "01",
                Ref11 = "NotBoolean",
                Ref12 = "NotNumber",
                Ref13 = null
            };
            client.GetWebApiResponseResult(reference.UpdateEx3("01", data)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode);

            var expect_FL = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'ArrayData' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {{
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      }},
      null,
      {{
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      }},
    ],
    'Object' : {{
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {{
        'prop31' : '333'
      }},
    }},
    'BooleanNullProp':null,
    'NumberNullProp':null,
    'StringNullProp':null,
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL.ToJson());

            // reset init
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        public void NotifyReferenceHistory_CheckSelfisLatestReferenceSourceIsHisotry_ReferenceSourceIsEmpty_DesignatedNoVersionKey()
        {
            ODataTestInit(true);

            // データ登録
            var testData = new ReferenceNotifyODataTestData();
            var response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataBefore)).Assert(RegisterSuccessExpectStatusCode);
            var data = response.Result;

            // あとで使うので、自分の履歴を取っておく
            var historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var refSelfHist_before = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();
            var selfDocKey_bef = refSelfHist_before[0]["documentKey"].ToString();
            var selfVerKey_bef = refSelfHist_before[0]["versionKey"].ToString();

            var refSourceHist_before = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"].ToList();

            // データ登録(上書き)
            response = client.GetWebApiResponseResult(referenceodata.RegistEx(testData.DataAfter)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴ヘッダ
            historyHeader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();
            var refSourceHist_after = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck").First()["documents"];

            var refSelfHistAft = historyHeader.Where(x => x["resourcePath"].ToString() == "/API/IntegratedTest/ReferenceNotifyOData").First()["documents"].ToList();

            var selfDocKey = refSelfHistAft[0]["documentKey"].ToString();
            var selfVerKey = refSelfHistAft[0]["versionKey"].ToString();

            JArray docs = new JArray();
            // 履歴ヘッダ分ループ(versionKeyは指定しない)
            foreach (var h1 in refSourceHist_before)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.First().AddAfterSelf(new JProperty("versionKey", h1["versionKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }
            // refenceヘッダ
            var refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            // Reference元は最新履歴、Reference先を過去履歴で取得できるか
            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            var histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // VersionKey無しの場合、Referece先のデータは、aft(最新版)で解決されること
            var expected = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "002Aft",
                temp = 999,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bef", prop3 = "before" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1000m,
                        AreaUnitCode = "AA",
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}"
                    }
                }
            };
            histRefData.IsStructuralEqual(expected);

            docs = new JArray();
            // 履歴ヘッダ分ループ(versionKeyは指定しない)
            foreach (var h1 in refSourceHist_before)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }
            // refenceヘッダ
            refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            // Reference先のVersionKey無しは、最新履歴が返って来るか（Reference元は、過去履歴）
            referenceodata.AddHeaders.Remove(HeaderConst.X_ReferenceHistory);
            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey_bef, Guid.Parse(selfVerKey_bef))).Assert(GetSuccessExpectStatusCode).Result;

            // VersionKey無しの場合、Referece先のデータは、aft(最新版)で解決されること（Reference元は、過去履歴）
            expected = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA",
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}"
                    }
                }
            };
            histRefData.IsStructuralEqual(expected);

            docs = new JArray();
            // 履歴ヘッダ分ループ(versionKeyは指定しない)
            foreach (var h1 in refSourceHist_before)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.First().AddAfterSelf(new JProperty("versionKey", h1["versionKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }
            // 履歴ヘッダ分ループ(versionKeyは指定しない)
            foreach (var h1 in refSourceHist_after)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }

            refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";

            // Reference先の履歴を２つ指定したら、２つ返って来る
            referenceodata.AddHeaders.Remove(HeaderConst.X_ReferenceHistory);
            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey_bef, Guid.Parse(selfVerKey_bef))).Assert(GetSuccessExpectStatusCode).Result;

            // VersionKey無しの場合、Referece先のデータは、aft(最新版)で解決されること（Reference元は、過去履歴）
            expected = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "001Bef",
                temp = 99,
                Ref1 = new List<ReferenceNotifySourceModel>()
                {
                    new ReferenceNotifySourceModel()
                    {
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "b", prop2 = "bef", prop3 = "before" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1000m,
                        AreaUnitCode = "AA",
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}"
                    },
                    new ReferenceNotifySourceModel()
                    {
                        ArrayData = null,
                        Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aft", prop3 = "after" },
                        AreaUnitName = "aa",
                        ConversionSquareMeters = 1001m,
                        AreaUnitCode = "AA",
                        id = $"API~IntegratedTest~ReferenceNotifySourceHistoryCheck~{WILDCARD}"
                    }
                }
            };
            histRefData.IsStructuralEqual(expected);

            // Reference先をクリアする
            referenceNotifySourceHistCheckApi = UnityCore.Resolve<IReferenceNotifySourceHistCheckApi>();
            client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.DeleteAll()).Assert(DeleteSuccessStatusCode);

            docs = new JArray();
            // 履歴ヘッダ分ループ(versionKeyは指定しない)
            foreach (var h1 in refSourceHist_after)
            {
                var token = "{'__dummy__':null}".ToJson();
                token.First().AddAfterSelf(new JProperty("resourcePath", "/API/IntegratedTest/ReferenceNotifySourceHistoryCheck"));
                token.First().AddAfterSelf(new JProperty("documentKey", h1["documentKey"].ToString()));
                token.RemoveField("__dummy__");
                docs.Add(token);
            }

            refHeader = $@"
{{
    'reference':true,
    'refhistinfo':{docs.ToString()}
}}";
            referenceodata.AddHeaders.Remove(HeaderConst.X_ReferenceHistory);
            referenceodata.AddHeaders.Add(HeaderConst.X_ReferenceHistory, refHeader.Replace("\r", "").Replace("\n", ""));
            histRefData = client.GetWebApiResponseResult(referenceodata.GetDocumentHistoryWithVersionGuid(selfDocKey, Guid.Parse(selfVerKey))).Assert(GetSuccessExpectStatusCode).Result;

            // VersionKey無しの場合、Referece先のデータは、null(最新版:削除済みなので)で解決されること
            expected = new ReferenceNotifyODataModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "01",
                Name = "002Aft",
                temp = 999,
                Ref1 = null
            };
            histRefData.IsStructuralEqual(expected);
        }

        [TestMethod]
        public void Notify_NumericTest()
        {
            var srcData = new ReferenceNotifySourceTestData();
            client.GetWebApiResponseResult(src.RegistListEx2(srcData.DataAllPropertyValueNull)).Assert(RegisterSuccessExpectStatusCode);

            //--- Registパターン
            //.0 チェック
            var response = client.GetWebApiResponseResult(reference.RegistAsString("{ 'Code' : '10', 'Ref1':123.0, 'Ref2' : 9999.0, 'Ref3': { 'prop1': 'b', 'prop2': 'bb', 'prop3': 'bbb' },'Ref4':'xyz', 'Ref5':10000.0, 'Ref6':'2099/12/31', 'Ref7':'****', 'Ref11':false, 'Ref12':1.0, 'Ref13':'hoge'}")).Assert(RegisterSuccessExpectStatusCode);
            var expect_FL_0 = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123.0,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : [null,null,'xyz'],
    'ArrayData' : [null,null,null],
    'NumberArray' : [null,10000.0,null],
    'ArrayObject' :[
                    {{ 'Date': '2099/12/31','ObservationItemCode': null,'ObservationValue': null}},
                    null,
                    {{'Date': null,'ObservationItemCode': null,'ObservationValue': null}}
                   ],
    'Object' :{{ 'prop1': null,'prop2': null,'prop3': {{
        'prop31': '****'
      }}}},
    'BooleanNullProp':false,
    'NumberNullProp':1.0,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999.0
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL_0.ToJson());

            //.1 チェック
            response = client.GetWebApiResponseResult(reference.RegistAsString("{ 'Code' : '10', 'Ref1':123.1, 'Ref2' : 9999.1, 'Ref3': { 'prop1': 'b', 'prop2': 'bb', 'prop3': 'bbb' },'Ref4':'xyz', 'Ref5':10000.1, 'Ref6':'2099/12/31', 'Ref7':'****', 'Ref11':false, 'Ref12':1.1, 'Ref13':'hoge'}")).Assert(RegisterSuccessExpectStatusCode);
            var expect_FL_1 = $@"
  {{
    '_Owner_Id': '{WILDCARD}',
    'id': 'API~IntegratedTest~ReferenceNotifySource~1~FL',
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 123.1,
    'Obj':{{'prop1': 'b','prop2': 'bb','prop3': 'bbb'}},
    'Array' : [null,null,'xyz'],
    'ArrayData' : [null,null,null],
    'NumberArray' : [null,10000.1,null],
    'ArrayObject' :[
                    {{ 'Date': '2099/12/31','ObservationItemCode': null,'ObservationValue': null}},
                    null,
                    {{'Date': null,'ObservationItemCode': null,'ObservationValue': null}}
                   ],
    'Object' :{{ 'prop1': null,'prop2': null,'prop3': {{
        'prop31': '****'
      }}}},
    'BooleanNullProp':false,
    'NumberNullProp':1.1,
    'StringNullProp':'hoge',
    'ConversionSquareMeters2': 9999.1
  }}";
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL_1.ToJson());


            //--Updateパターン
            //.0
            var updResponse = client.GetWebApiResponseResult(reference.UpdateAsString("10", "{ 'Ref1':123.0, 'Ref2' : 9999.0, 'Ref3': { 'prop1': 'b', 'prop2': 'bb', 'prop3': 'bbb' },'Ref4':'xyz', 'Ref5':10000.0, 'Ref6':'2099/12/31', 'Ref7':'****', 'Ref11':false, 'Ref12':1.0, 'Ref13':'hoge'}")).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL_0.ToJson());

            //.1
            updResponse = client.GetWebApiResponseResult(reference.UpdateAsString("10", "{ 'Ref1':123.1, 'Ref2' : 9999.1, 'Ref3': { 'prop1': 'b', 'prop2': 'bb', 'prop3': 'bbb' },'Ref4':'xyz', 'Ref5':10000.1, 'Ref6':'2099/12/31', 'Ref7':'****', 'Ref11':false, 'Ref12':1.1, 'Ref13':'hoge'}")).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(src.Get("FL")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(expect_FL_1.ToJson());

            // reset init
            client.GetWebApiResponseResult(src.RegistListSingle(srcData.Data1)).Assert(RegisterSuccessExpectStatusCode);
        }

        private void ODataTestInit(bool isHeadcheckTest = false, bool isSkipFirstInit = false)
        {
            // 履歴使用リソースの履歴削除
            DeleteHistory();

            referenceodata = UnityCore.Resolve<IReferenceNotifyODataApi>();
            client.GetWebApiResponseResult(referenceodata.DeleteAll()).Assert(DeleteExpectStatusCodes);

            var firstodataData = new ReferenceNotifyODataFirstTestData();
            firstodata = UnityCore.Resolve<IReferenceNotifyODataFirstApi>();
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data99)).Assert(RegisterSuccessExpectStatusCode);

            if (isHeadcheckTest)
            {
                referenceodataNestsetting = UnityCore.Resolve<IReferenceNotifyODataNestSettingApi>();
                firstodataNestsetting = UnityCore.Resolve<IReferenceNotifyODataNestSettingFirstApi>();

                var srcData = new ReferenceNotifySourceTestData();
                referenceNotifySourceHistCheckApi = UnityCore.Resolve<IReferenceNotifySourceHistCheckApi>();
                client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.DeleteAll()).Assert(DeleteExpectStatusCodes);
                client.GetWebApiResponseResult(referenceNotifySourceHistCheckApi.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);

                firstodata = UnityCore.Resolve<IReferenceNotifyODataFirstApi>();
                if (isSkipFirstInit) return;

                client.GetWebApiResponseResult(firstodata.DeleteAll()).Assert(DeleteExpectStatusCodes);
                client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data1_histCheck)).Assert(RegisterSuccessExpectStatusCode);
                client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data2_histCheck)).Assert(RegisterSuccessExpectStatusCode);
                client.GetWebApiResponseResult(firstodata.Regist(firstodataData.Data99_histCheck)).Assert(RegisterSuccessExpectStatusCode);
            }
        }
    }
}
