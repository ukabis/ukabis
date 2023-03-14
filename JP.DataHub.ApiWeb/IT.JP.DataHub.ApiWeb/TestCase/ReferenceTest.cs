using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("DocumentHistory")]
    public class ReferenceTest : ApiWebItTestCase
    {
        #region TestData

        private class ReferenceTestFirstData : TestDataBase
        {
            public ReferenceNotifyFirstModel Data1 = new ReferenceNotifyFirstModel()
            {
                Code = "01",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref4_type_stringnull = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref5_type_objectnull = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,Obj",
                Ref6_numbercheck = "$Reference /API/IntegratedTest/ReferenceSource/Get/TB,ConversionSquareMeters",
                Ref7_numbercheck = "$Reference /API/IntegratedTest/ReferenceSource/Get/FL,ConversionSquareMeters",
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
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/HA,Obj",
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

            public ReferenceNotifyFirstModel Data3 = new ReferenceNotifyFirstModel()
            {
                Code = "03",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/UN,Array[1]",
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

            public ReferenceNotifyFirstModel Data4 = new ReferenceNotifyFirstModel()
            {
                Code = "04",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/TB,ArrayObject[0].p1",
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

            public ReferenceNotifyFirstModel Data1R = new ReferenceNotifyFirstModel()
            {
                Code = "1R",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
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

            public ReferenceNotifyFirstModel Data2R = new ReferenceNotifyFirstModel()
            {
                Code = "2R",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
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

            public ReferenceNotifyFirstModel Data3R = new ReferenceNotifyFirstModel()
            {
                Code = "3R",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
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

            public ReferenceNotifyFirstModel Data4R = new ReferenceNotifyFirstModel()
            {
                Code = "4R",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
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

            public ReferenceNotifyFirstModel Data1N = new ReferenceNotifyFirstModel()
            {
                Code = "1N",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters",
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

            public ReferenceNotifyFirstModel Data2N = new ReferenceNotifyFirstModel()
            {
                Code = "2N",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Obj",
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

            public ReferenceNotifyFirstModel Data3N = new ReferenceNotifyFirstModel()
            {
                Code = "3N",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,Array[1]",
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

            public ReferenceNotifyFirstModel Data4N = new ReferenceNotifyFirstModel()
            {
                Code = "4N",
                Name = "001",
                temp = 99,
                Ref1 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
                Ref2 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
                Ref3 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
                Ref4 = "$Reference /API/IntegratedTest/ReferenceSource/Get/XX,ArrayObject[0].p1",
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

        private class ReferenceTestSourceData : TestDataBase
        {
            public string Data = @"
[
  {
    'AreaUnitCode': 'AA',
    'AreaUnitName': 'aa',
    'ConversionSquareMeters': 1000,
    'Obj' :{ 'prop1' : 'a', 'prop2' : 'aa', 'prop3' : 'aaa', },
    'Array' :[ 'z', 'zz', 'zzz',],
    'ArrayObject' : [ { 'p1':'pa', 'p2':'paa' }, ],
  },
  {
    'AreaUnitCode': 'HA',
    'AreaUnitName': 'ha',
    'ConversionSquareMeters': 10000,
    'Obj' :{ 'prop1' : 'b', 'prop2' : 'bb', 'prop3' : 'bbb', },
    'Array' : [ 'y', 'yy', 'yyy',],
    'ArrayObject' : [ { 'p1':'pb', 'p2':'pbb' }, ],
  },
  {
    'AreaUnitCode': 'M2',
    'AreaUnitName': '㎡',
    'ConversionSquareMeters': 1,
    'Obj' :{ 'prop1' : 'c', 'prop2' : 'cc', 'prop3' : 'ccc', },
    'Array' : [ 'x', 'xx', 'xxx',],
    'ArrayObject' : [ { 'p1':'pc', 'p2':'pcc' }, ],
  },
  {
    'AreaUnitCode': 'TB',
    'AreaUnitName': '坪',
    'ConversionSquareMeters': 3.305785,
    'Obj' :{ 'prop1' : 'd', 'prop2' : 'dd', 'prop3' : 'ddd', },
    'Array' : [ 'w', 'ww', 'www',],
    'ArrayObject' : [ { 'p1':'pd', 'p2':'pdd' }, ],
  },
  {
    'AreaUnitCode': 'UN',
    'AreaUnitName': '畝',
    'ConversionSquareMeters': 99.1736,
    'Obj' :{ 'prop1' : 'e', 'prop2' : 'ee', 'prop3' : 'eee', },
    'Array' : [ 'v', 'vv', 'vvv',],
    'ArrayObject' : [ { 'p1':'pe', 'p2':'pee' }, ],
  },
  {
    'AreaUnitCode': 'TN',
    'AreaUnitName': '反',
    'ConversionSquareMeters': 990,
    'Obj' :{ 'prop1' : 'f', 'prop2' : 'ff', 'prop3' : 'fff', },
    'Array' : [ 'u', 'uu', 'uuu',],
    'ArrayObject' : [ { 'p1':'pf', 'p2':'pff' }, ],
  },
  {
    'AreaUnitCode': 'CH',
    'AreaUnitName': '町（町歩）',
    'ConversionSquareMeters': 666.667,
    'Obj' :{ 'prop1' : 'g', 'prop2' : 'gg', 'prop3' : 'ggg', },
    'Array' : [ 't', 'tt', 'ttt',],
    'ArrayObject' : [ { 'p1':'pg', 'p2':'pgg' }, ],
  },
  {
    'AreaUnitCode': 'XX',
    'AreaUnitName': 'xx',
    'ConversionSquareMeters': 1000,
    'Obj' :{ 'prop1' : 'h', 'prop2' : 'hh', 'prop3' : 'hhh', },
    'Array' : [ 's', 'ss', 'sss',],
    'ArrayObject' : [ { 'p1':'ph', 'p2':'phh' }, ],
  },
  {
    'AreaUnitCode': 'FL',
    'AreaUnitName': 'FullData',
    'ConversionSquareMeters': 1,
    'Array' : [ 's1', 's2', 's3', ],
    'NumberArray' : [ 123, 456, 789, ],
    'ArrayObject' : [
      {
        'Date' : '2020/01/01',
        'ObservationItemCode' : '1',
        'ObservationValue' : '11',
      },
      null,
      {
        'Date' : '2020/02/02',
        'ObservationItemCode' : '2',
        'ObservationValue' : '22',
      },
    ],
    'Object' : {
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {
        'prop31' : '333',
      }
    },
  },
  {
    'AreaUnitCode': 'BB',
    'AreaUnitName': 'aa',
    'ConversionSquareMeters': 2000,
    'Obj' :{ 'prop1' : 'a', 'prop2' : 'aa', 'prop3' : 'aaa', },
    'Array' :[ 'z', 'zz', 'zzz',],
    'ArrayObject' : [ { 'p1':'pa', 'p2':'paa' }, ],
  }
]";
        }

        #endregion


        private IntegratedTestClient client;
        private IReferenceApi reference;
        private IReferenceSourceApi src;
        private IReferenceFirstApi first;
        private IReferenceArrayTestApi reference_arraytest;

        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);

            client = new IntegratedTestClient(AppConfig.Account);

            reference = UnityCore.Resolve<IReferenceApi>();
            client.GetWebApiResponseResult(reference.DeleteAll()).Assert(DeleteExpectStatusCodes);

            first = UnityCore.Resolve<IReferenceFirstApi>();
            var firstData = new ReferenceTestFirstData();
            client.GetWebApiResponseResult(first.Regist(firstData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data3)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data4)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data1R)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data2R)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data3R)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data4R)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data1N)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data2N)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data3N)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(first.Regist(firstData.Data4N)).Assert(RegisterSuccessExpectStatusCode);

            src = UnityCore.Resolve<IReferenceSourceApi>();
            var srcData = new ReferenceTestSourceData();
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(src.RegistList(srcData.Data)).Assert(RegisterSuccessExpectStatusCode);

            reference_arraytest = UnityCore.Resolve<IReferenceArrayTestApi>();
            client.GetWebApiResponseResult(reference_arraytest.DeleteAll()).Assert(DeleteExpectStatusCodes);
        }

        [TestMethod]
        public void Reference_null()
        {
            client.GetWebApiResponseResult(src.DeleteAll()).Assert(DeleteExpectStatusCodes);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data.FirstOrDefault().GetPropertyValue("Ref1").Type.Is(JTokenType.Null);

            data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data.GetPropertyValue("Ref1").Type.Is(JTokenType.Null);
        }

        [TestMethod]
        public void Reference_ExistDocument_ButPropertyIsNull()
        {
            client.GetWebApiResponseResult(src.UpdateAsString("AA", "{'Obj':null}")).Assert(UpdateSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data.FirstOrDefault().GetPropertyValue("Ref5_type_objectnull").Type.Is(JTokenType.Null);

            data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data.GetPropertyValue("Ref5_type_objectnull").Type.Is(JTokenType.Null);
        }

        [TestMethod]
        public void Reference_ExistDocument_ButPropertyIsEmptyString()
        {
            client.GetWebApiResponseResult(src.UpdateAsString("AA", "{'Obj':''}")).Assert(UpdateSuccessExpectStatusCode);

            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data.FirstOrDefault().GetPropertyValue("Ref5_type_objectnull").Type.Is(JTokenType.Null);

            data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data.GetPropertyValue("Ref5_type_objectnull").Type.Is(JTokenType.Null);
        }

        [TestMethod]
        public void Reference_value()
        {
            var data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.FirstOrDefault().GetPropertyValue("Ref1") as JValue).Value.Is("1000");

            data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.GetPropertyValue("Ref1") as JValue).Value.Is("1000");

            client.GetWebApiResponseResult(src.UpdateAsString("AA", "{ 'ConversionSquareMeters': 9898 }")).Assert(UpdateSuccessExpectStatusCode);

            data = client.GetWebApiResponseResult(reference.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.FirstOrDefault().GetPropertyValue("Ref1") as JValue).Value.Is("9898");

            data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.GetPropertyValue("Ref1") as JValue).Value.Is("9898");

            client.GetWebApiResponseResult(src.UpdateAsString("AA", "{ 'ConversionSquareMeters': 1000 }")).Assert(UpdateSuccessExpectStatusCode);
        }

        [TestMethod]
        public void Reference_object()
        {
            var data = client.GetWebApiResponseResult(reference.Get("02")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref1"].ToString().Is(@"{
  ""prop1"": ""b"",
  ""prop2"": ""bb"",
  ""prop3"": ""bbb""
}");
        }

        [TestMethod]
        public void Reference_array()
        {
            var data = client.GetWebApiResponseResult(reference.Get("03")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref1"].ToString().Is("vv");
        }

        [TestMethod]
        public void Reference_arrayObject()
        {
            var data = client.GetWebApiResponseResult(reference.Get("04")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref1"].ToString().Is("pd");
        }

        [TestMethod]
        public void Reference_Protect()
        {
            // 参照先の値を確認
            var data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.GetPropertyValue("Ref2") as JValue).Value.Is("1000");

            // 更新処理
            client.GetWebApiResponseResult(reference.UpdateAsString("01", "{ 'Ref2' : '999' }")).Assert(UpdateSuccessExpectStatusCode);

            data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.GetPropertyValue("Ref2") as JValue).Value.Is("1000");
        }

        [TestMethod]
        public void Reference_type_stringnull_numbercheck()
        {
            // type string と、type string null で実行結果が同じであること
            var data = client.GetWebApiResponseResult(reference.Get("01")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            (data.GetPropertyValue("Ref4") as JValue).Value.Is(
                (data.GetPropertyValue("Ref4_type_stringnull") as JValue).Value
                );

            data["Ref6_numbercheck"].ToString().Is("3.305785");
            //1.0ではないこと
            data["Ref7_numbercheck"].ToString().Is("1");
        }

        [TestMethod]
        public void Reference_Write()
        {
            // 参照の値が初期値か？（参照先の値か？）
            var data = client.GetWebApiResponseResult(reference.Get("04")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref1"].ToString().Is("pd");

            // 書き換えができる
            client.GetWebApiResponseResult(reference.UpdateAsString("04", "{ 'Ref1' : '67534787' }")).Assert(UpdateSuccessExpectStatusCode);

            data = client.GetWebApiResponseResult(reference.Get("04")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref1"].ToString().Is("67534787");

            // 書き換えたものを消去
            client.GetWebApiResponseResult(reference.UpdateAsString("04", "{ 'Ref1' : '$Null' }")).Assert(UpdateSuccessExpectStatusCode);

            // もとに戻っている
            data = client.GetWebApiResponseResult(reference.Get("04")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref1"].ToString().Is("pd");
        }

        [TestMethod]
        public void Reference_WriteNotify_Simple()
        {
            string KEY = "1R";
            // 参照の値が初期値か？（参照先の値か？）
            var data = client.GetWebApiResponseResult(reference.Get(KEY)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref3"].ToString().Is("1000");

            // 書き換え成功
            client.GetWebApiResponseResult(reference.UpdateAsString(KEY, "{ 'Ref3' : '987654321' }")).Assert(UpdateSuccessExpectStatusCode);

            data = client.GetWebApiResponseResult(reference.Get(KEY)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref3"].ToString().Is("987654321");

            // 書き換えたものを消去
            client.GetWebApiResponseResult(reference.UpdateAsString(KEY, "{ 'Ref3' : '$Null' }")).Assert(UpdateSuccessExpectStatusCode);

            // もとに戻っている
            data = client.GetWebApiResponseResult(reference.Get(KEY)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref3"].ToString().Is("1000");
        }

        [TestMethod]
        public void Reference_WriteNotify_Protect()
        {
            string KEY = "1N";
            // 参照の値が初期値か？（参照先の値か？）
            var data = client.GetWebApiResponseResult(reference.Get(KEY)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref4"].ToString().Is("1000");

            // 書き換え成功
            client.GetWebApiResponseResult(reference.UpdateAsString(KEY, "{ 'Ref4' : '987654321' }")).Assert(UpdateSuccessExpectStatusCode);

            data = client.GetWebApiResponseResult(reference.Get(KEY)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref4"].ToString().Is("1000");

            // 書き換えたものを消去
            client.GetWebApiResponseResult(reference.UpdateAsString(KEY, "{ 'Ref4' : '$Null' }")).Assert(UpdateSuccessExpectStatusCode);

            // もとに戻っている
            data = client.GetWebApiResponseResult(reference.Get(KEY)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
            data["Ref4"].ToString().Is("1000");
        }

        [TestMethod]
        public void Reference_ResponseIsArrayTest()
        {
            var data1_R_Array = $@"
{{
    ""Code"" : ""data1_R_Array"",
    ""Name"" : ""001"",
    ""Ref1_SingleReference"" : ""$Reference /API/IntegratedTest/ReferenceSource/Get/AA,ConversionSquareMeters"",
    ""Ref2_ODataArrayReference"" : ""$Reference /API/IntegratedTest/ReferenceSource/OData?$select=ConversionSquareMeters&$filter=AreaUnitName eq 'aa',ConversionSquareMeters"",
    ""Ref3_NormalArrayReference"" : ""$Reference /API/IntegratedTest/ReferenceSource/GetConversionSquareMeters/aa,ConversionSquareMeters"",
    ""Ref4_ODataArrayReferenceButReturnSingleProperty"" : ""$Reference /API/IntegratedTest/ReferenceSource/OData?$select=ConversionSquareMeters&$filter=AreaUnitName eq 'aa',ConversionSquareMeters"",
    ""Ref5_ODataSingleReturnPropertyIsArray"" : ""$Reference /API/IntegratedTest/ReferenceSource/OData?$select=ConversionSquareMeters&$filter=ConversionSquareMeters eq 2000,ConversionSquareMeters"",
    ""Ref6_ODataMultiProperty"" : ""$Reference \""/API/IntegratedTest/ReferenceSource/OData?$select=AreaUnitName,ConversionSquareMeters&$filter=ConversionSquareMeters eq 2000\"",AreaUnitName,ConversionSquareMeters"",
    ""Ref7_OData_TypeArrayNull_MultiProperty"" : ""$Reference \""/API/IntegratedTest/ReferenceSource/OData?$select=AreaUnitName,ConversionSquareMeters&$filter=ConversionSquareMeters eq 2000\"",AreaUnitName,ConversionSquareMeters"",
    ""Ref8_ODataArrayReference_OrderBy"" : ""$Reference /API/IntegratedTest/ReferenceSource/OData?$select=ConversionSquareMeters&$orderby=ConversionSquareMeters DESC&$filter=AreaUnitName eq 'aa',ConversionSquareMeters"",
}}";

            // 参照元データ投入
            client.GetWebApiResponseResult(reference_arraytest.RegistAsString(data1_R_Array)).Assert(RegisterSuccessExpectStatusCode);

            // Reference結果取得
            var data = client.GetWebApiResponseResult(reference_arraytest.Get("data1_R_Array")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();

            // ノンデグ確認
            data["Ref1_SingleReference"].Is("1000");

            // ODataで、複数ドキュメントの特定プロパティが返って来ているか確認
            var ref2_expect = "{'ConversionSquareMeters':1000},{'ConversionSquareMeters':2000}".ToJArray();
            var data2 = data["Ref2_ODataArrayReference"];
            data2.Is(ref2_expect);

            // 普通のGetで、複数ドキュメントの特定プロパティが返って来ているか確認
            var ref3_expect = "{'ConversionSquareMeters':1000},{'ConversionSquareMeters':2000}".ToJArray();
            var data3 = data["Ref3_NormalArrayReference"];
            data3.Is(ref3_expect);

            // ref4 は、参照先は複数返って来るが、呼び元のモデルが単一プロパティ(string)なので、
            // １つのみレスポンスされる想定
            var ref4_expect = "1000".ToJValue();
            var data4 = data["Ref4_ODataArrayReferenceButReturnSingleProperty"];
            data4.Is(ref4_expect);

            // ref5 は、配列添え字1 のデータがレスポンスされる想定
            var ref5_expect = "{'ConversionSquareMeters':2000}".ToJson().ToJsonArray();
            var data5 = data["Ref5_ODataSingleReturnPropertyIsArray"];
            data5.Is(ref5_expect);

            // ref6 は、複数項目参照
            var ref6_expect = "{'ConversionSquareMeters':2000, 'AreaUnitName':'aa'}".ToJson().ToJsonArray();
            var data6 = data["Ref6_ODataMultiProperty"];
            data6.Is(ref6_expect);

            // ref7 は、複数項目参照（ただし、PropertyType が[array, null]
            var ref7_expect = "{'ConversionSquareMeters':2000, 'AreaUnitName':'aa'}".ToJson().ToJsonArray();
            var data7 = data["Ref7_OData_TypeArrayNull_MultiProperty"];
            data7.Is(ref7_expect);

            // ODataで、複数ドキュメントの特定プロパティが返って来ているか確認
            var ref8_expect = "{'ConversionSquareMeters':2000},{'ConversionSquareMeters':1000}".ToJArray();
            var data8 = data["Ref8_ODataArrayReference_OrderBy"];
            data8.Is(ref8_expect);
        }
    }
}
