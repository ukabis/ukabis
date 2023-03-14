using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ProtectSchemaKeyTest : ApiWebItTestCase
    {
        #region TestData

        private class ProtectSchemaKeyTestData : TestDataBase
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

            public List<ReferenceNotifySourceModel> Data1Reregist = new List<ReferenceNotifySourceModel>()
            {
                new ReferenceNotifySourceModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "bb",
                    ConversionSquareMeters = 9999,
                    Obj = new ReferenceNotifyObject() { prop1 = "a", prop2 = "aa", prop3 = "aaa" },
                    Array = new List<string>() { "z", "zz", "zzz" },
                    ArrayObject = new List<ReferenceNotifySourceArrayObjectItem>() { new ReferenceNotifySourceArrayObjectItem() { p1 = "pa", p2 = "paa" } }
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


        /// <summary>
        /// 論理キーありのデータに対して、Protect属性の指定をして、正しく動作するか
        /// </summary>
        [TestMethod]
        public void NormalSenario_Key()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IProtectSchemaKeyApi>();
            var testData = new ProtectSchemaKeyTestData();

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 元データを入れる
            client.GetWebApiResponseResult(api.RegistList(testData.Data)).Assert(RegisterSuccessExpectStatusCode);

            // AAの中身を確認（想定通りのデータが入っている）
            var data = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode).Result;
            data.AreaUnitName.Is("aa");
            data.ConversionSquareMeters.Is(1000);

            // AAのAreaUnitName,ConversionSquareMetersを変更する
            client.GetWebApiResponseResult(api.RegistList(testData.Data1Reregist)).Assert(RegisterSuccessExpectStatusCode);

            // AreaUnitNameはProtectなので、変更されていないことを確認する
            // ConversionSquareMetersはProtectではないので、変更されてよい
            data = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode).Result;
            data.AreaUnitName.Is("aa");
            data.ConversionSquareMeters.Is(9999);
        }
    }
}
