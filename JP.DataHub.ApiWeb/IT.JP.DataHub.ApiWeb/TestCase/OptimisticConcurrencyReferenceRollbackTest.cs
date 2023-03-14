using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public class OptimisticConcurrencyReferenceRollbackTest : ApiWebItTestCase
    {
        #region TestData

        private class OptimisticConcurrencyReferenceNotifySourceTestData : TestDataBase
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
        }

        private class OptimisticConcurrencyAndReferenceNotifyTestData : TestDataBase
        {
            public ReferenceNotifyFirstModel DataFirst1 = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject",
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

            public ReferenceNotifyFirstModel DataFirst2 = new ReferenceNotifyFirstModel()
            {
                Code = "02",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject",
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

            public ReferenceNotifyFirstModel DataFirst3 = new ReferenceNotifyFirstModel()
            {
                Code = "03",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,ConversionSquareMeters2",
                Ref3 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/AA,Obj",
                Ref4 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,Array[2]",
                Ref5 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,NumberArray[1]",
                Ref6 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject[0].Date",
                Ref7 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,Object.prop3.prop31",
                Ref8 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/XYZ,abc",
                Ref9 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject[0]",
                Ref10 = "$Reference /API/IntegratedTest/OptimisticConcurrencyReferenceNotifySource/Get/FL,ArrayObject",
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

            public List<ReferenceNotifyModel> DataMiddleErrorExpected = new List<ReferenceNotifyModel>()
            {
                new ReferenceNotifyModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    Code = "01",
                    Name = "1111",
                    temp = 1111,
                    Ref1 = 789m,
                    Ref2 = null,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "c", prop2 = "cc", prop3 = "ccc" },
                    Ref4 = "s3",
                    Ref5 = 456m,
                    Ref6 = "2020/01/01",
                    Ref7 = "333",
                    Ref8 = null,
                    Ref9 = new PropArrayItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                    Ref10 = new List<PropArrayItem>()
                    {
                        new PropArrayItem() {  Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new PropArrayItem() {  Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    }
                },
                new ReferenceNotifyModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    Code = "02",
                    Name = "001",
                    temp = 99,
                    Ref1 = 789m,
                    Ref2 = null,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "c", prop2 = "cc", prop3 = "ccc" },
                    Ref4 = "s3",
                    Ref5 = 456m,
                    Ref6 = "2020/01/01",
                    Ref7 = "333",
                    Ref8 = null,
                    Ref9 = new PropArrayItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                    Ref10 = new List<PropArrayItem>()
                    {
                        new PropArrayItem() {  Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new PropArrayItem() {  Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    },
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
                },
                new ReferenceNotifyModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    Code = "03",
                    Name = "003",
                    temp = 33,
                    Ref1 = 789m,
                    Ref2 = null,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "c", prop2 = "cc", prop3 = "ccc" },
                    Ref4 = "s3",
                    Ref5 = 456m,
                    Ref6 = "2020/01/01",
                    Ref7 = "333",
                    Ref8 = null,
                    Ref9 = new PropArrayItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                    Ref10 = new List<PropArrayItem>()
                    {
                        new PropArrayItem() {  Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new PropArrayItem() {  Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    }
                }
            };

            public List<ReferenceNotifyModel> DataConflictStopExpected = new List<ReferenceNotifyModel>()
            {
                new ReferenceNotifyModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    Code = "01",
                    Name = "1111",
                    temp = 1111,
                    Ref1 = 789m,
                    Ref2 = null,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                    Ref4 = "s3",
                    Ref5 = 456m,
                    Ref6 = "2020/01/01",
                    Ref7 = "333",
                    Ref8 = null,
                    Ref9 = new PropArrayItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                    Ref10 = new List<PropArrayItem>()
                    {
                        new PropArrayItem() {  Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new PropArrayItem() {  Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    }
                },
                new ReferenceNotifyModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    Code = "02",
                    Name = "001",
                    temp = 99,
                    Ref1 = 789m,
                    Ref2 = null,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                    Ref4 = "s3",
                    Ref5 = 456m,
                    Ref6 = "2020/01/01",
                    Ref7 = "333",
                    Ref8 = null,
                    Ref9 = new PropArrayItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                    Ref10 = new List<PropArrayItem>()
                    {
                        new PropArrayItem() {  Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new PropArrayItem() {  Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    },
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
                },
                new ReferenceNotifyModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    Code = "03",
                    Name = "001",
                    temp = 99,
                    Ref1 = 789m,
                    Ref2 = null,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                    Ref4 = "s3",
                    Ref5 = 456m,
                    Ref6 = "2020/01/01",
                    Ref7 = "333",
                    Ref8 = null,
                    Ref9 = new PropArrayItem() { Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                    Ref10 = new List<PropArrayItem>()
                    {
                        new PropArrayItem() {  Date = "2020/01/01", ObservationItemCode = "1", ObservationValue = "11" },
                        null,
                        new PropArrayItem() {  Date = "2020/02/02", ObservationItemCode = "2", ObservationValue = "22" }
                    },
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
        public void ConflictOccurred_NotifyRollbackTest_Regist()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var src = UnityCore.Resolve<IOptimisticConcurrencyReferenceNotifySourceApi>();
            var srcData = new OptimisticConcurrencyReferenceNotifySourceTestData();
            var reference = UnityCore.Resolve<IOptimisticConcurrencyAndReferenceNotifyApi>();
            var refData = new OptimisticConcurrencyAndReferenceNotifyTestData();

            // Notifyの設定
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(src.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst1)).Assert(RegisterSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var etags = data.Select(x => x._etag).ToList();

            // 違うetagを付け、データを変えて登録 ⇒ コンフリクト発生
            var regData = new ReferenceNotifyModel()
            {
                _etag = "hoge",
                Code = "01",
                Name = "1111",
                temp = 1111,
                Ref1 = 789m,
                Ref2 = 123m,
                Ref3 = new ReferenceNotifyObject() { prop1 = "z", prop2 = "zz", prop3 = "zzz" }
            };
            client.GetWebApiResponseResult(reference.Register(regData)).Assert(ConflictExpectStatusCode);

            // データ取得
            var updated_aft = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Reference先もロールバックされている（更新前のデータと同じ）こと
            updated_aft.IsStructuralEqual(data);
        }

        [TestMethod]
        public void ConflictOccurred_NotifyRollbackTest_Regist_Array_MiddleError()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var src = UnityCore.Resolve<IOptimisticConcurrencyReferenceNotifySourceApi>();
            var srcData = new OptimisticConcurrencyReferenceNotifySourceTestData();
            var reference = UnityCore.Resolve<IOptimisticConcurrencyAndReferenceNotifyApi>();
            var refData = new OptimisticConcurrencyAndReferenceNotifyTestData();

            //Notifyの設定
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(src.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst3)).Assert(RegisterSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var etags = data.Select(x => x._etag).ToList();

            // 違うetagを付け、データを変えて登録 ⇒ コンフリクト発生
            var regDataList = new List<ReferenceNotifyModel>()
            {
                new ReferenceNotifyModel()
                {
                    _etag = etags[0],
                    Code = "01",
                    Name = "1111",
                    temp = 1111,
                    Ref1 = 789m
                },
                new ReferenceNotifyModel()
                {
                    _etag = "hoge",
                    Code = "02",
                    Name = "002",
                    temp = 22,
                    Ref1 = 222m
                },
                new ReferenceNotifyModel()
                {
                    _etag = etags[2],
                    Code = "03",
                    Name = "003",
                    temp = 33,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "c", prop2 = "cc", prop3 = "ccc" }
                }
            };
            client.GetWebApiResponseResult(reference.RegisterList(regDataList)).Assert(ConflictExpectStatusCode);

            // データ取得
            var updated_aft = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Reference先もロールバックされている（更新前のデータと同じ）こと
            updated_aft.IsStructuralEqual(refData.DataMiddleErrorExpected);
        }

        [TestMethod]
        public void ConflictOccurred_NotifyRollbackTest_Regist_Array_MiddleError_ConflictStop()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var src = UnityCore.Resolve<IOptimisticConcurrencyReferenceNotifySourceApi>();
            var srcData = new OptimisticConcurrencyReferenceNotifySourceTestData();
            var reference = UnityCore.Resolve<IOptimisticConcurrencyAndReferenceNotifyApi>();
            var refData = new OptimisticConcurrencyAndReferenceNotifyTestData();

            // Notifyの設定
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(src.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst3)).Assert(RegisterSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var etags = data.Select(x => x._etag).ToList();

            // X-RegisterConflictStop=trueを設定する。
            reference.AddHeaders.Add(HeaderConst.X_RegisterConflictStop, "true");

            // 違うetagを付け、データを変えて登録 ⇒ コンフリクト発生
            var regDataList = new List<ReferenceNotifyModel>()
            {
                new ReferenceNotifyModel()
                {
                    _etag = etags[0],
                    Code = "01",
                    Name = "1111",
                    temp = 1111,
                    Ref1 = 789m
                },
                new ReferenceNotifyModel()
                {
                    _etag = "hoge",
                    Code = "02",
                    Name = "002",
                    temp = 22,
                    Ref1 = 222m
                },
                new ReferenceNotifyModel()
                {
                    _etag = etags[2],
                    Code = "03",
                    Name = "003",
                    temp = 33,
                    Ref3 = new ReferenceNotifyObject() { prop1 = "c", prop2 = "cc", prop3 = "ccc" }
                }
            };
            client.GetWebApiResponseResult(reference.RegisterList(regDataList)).Assert(ConflictExpectStatusCode);

            // データ取得
            var updated_aft = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Reference先もロールバックされている（ただし、添え字0の更新は、戻っていない）こと
            updated_aft.IsStructuralEqual(refData.DataConflictStopExpected);
        }

        [TestMethod]
        public void ConflictOccurred_NotifyRollbackTest_Update()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var src = UnityCore.Resolve<IOptimisticConcurrencyReferenceNotifySourceApi>();
            var srcData = new OptimisticConcurrencyReferenceNotifySourceTestData();
            var reference = UnityCore.Resolve<IOptimisticConcurrencyAndReferenceNotifyApi>();
            var refData = new OptimisticConcurrencyAndReferenceNotifyTestData();

            // Notifyの設定
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(src.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst1)).Assert(RegisterSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var etags = data.Select(x => x._etag).ToList();

            // 違うetagを付け、データを変えて更新 ⇒ コンフリクト発生
            var updData = new ReferenceNotifyModel()
            {
                _etag = "hoge",
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = 789m,
                Ref2 = 123m,
                Ref3 = new ReferenceNotifyObject() { prop1 = "z", prop2 = "zz", prop3 = "zzz" }
            };
            client.GetWebApiResponseResult(reference.Update("01", updData)).Assert(ConflictExpectStatusCode);

            // データ取得
            var updated_aft = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Reference先もロールバックされている（更新前のデータと同じ）こと
            updated_aft.IsStructuralEqual(data);
        }

        [TestMethod]
        public void ConflictOccurred_NotifyRollbackTest_Regist_DeepPath()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var src = UnityCore.Resolve<IOptimisticConcurrencyReferenceNotifySourceApi>();
            var srcData = new OptimisticConcurrencyReferenceNotifySourceTestData();
            var reference = UnityCore.Resolve<IOptimisticConcurrencyAndReferenceNotifyApi>();
            var refData = new OptimisticConcurrencyAndReferenceNotifyTestData();

            // Notifyの設定
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(src.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst1)).Assert(RegisterSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var etags = data.Select(x => x._etag).ToList();

            // 違うetagを付け、データを変えて登録 ⇒ コンフリクト発生
            var regData = new ReferenceNotifyModel()
            {
                _etag = "hoge",
                Code = "01",
                Name = "1111",
                temp = 1111,
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****"
            };
            client.GetWebApiResponseResult(reference.Register(regData)).Assert(ConflictExpectStatusCode);

            // データ取得
            var updated_aft = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Reference先もロールバックされている（更新前のデータと同じ）こと
            updated_aft.IsStructuralEqual(data);
        }

        [TestMethod]
        public void ConflictOccurred_NotifyRollbackTest_Update_DeepPath()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var src = UnityCore.Resolve<IOptimisticConcurrencyReferenceNotifySourceApi>();
            var srcData = new OptimisticConcurrencyReferenceNotifySourceTestData();
            var reference = UnityCore.Resolve<IOptimisticConcurrencyAndReferenceNotifyApi>();
            var refData = new OptimisticConcurrencyAndReferenceNotifyTestData();

            // Notifyの設定
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(src.RegisterList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(reference.RegisterFirst(refData.DataFirst1)).Assert(RegisterSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;
            var etags = data.Select(x => x._etag).ToList();

            // 違うetagを付け、データを変えて更新 ⇒ コンフリクト発生
            var regData = new ReferenceNotifyModel()
            {
                _etag = "hoge",
                Code = "01",
                Name = "1111",
                temp = 1111,
                Ref2 = 123,
                Ref4 = "xyz",
                Ref5 = 10000m,
                Ref6 = "2099/12/31",
                Ref7 = "****"
            };
            client.GetWebApiResponseResult(reference.Register(regData)).Assert(ConflictExpectStatusCode);

            // データ取得
            var updated_aft = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // Reference先もロールバックされている（更新前のデータと同じ）こと
            updated_aft.IsStructuralEqual(data);
        }
    }
}