using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JP.DataHub.Com.Json.Schema;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.QueryCompiler;
using JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.JsonPropertyFormatExtensions
{
    [TestClass]
    public class UnitTest_ReferenceProperty : UnitTestBase
    {
        private static string protectSchema = @"{
  'description':'IntegratedTestProtectSchema',
  'properties': {
    'Code': {
      'title': 'コード',
      'type': 'string',
      'required':true,
    },
    'Name': {
      'maxLength': 20,
      'title': '名称',
      'type': 'string',
      'required':true,
    },
    'temp': {
      'title': 'テンポラリー',
      'type': 'number',
    },
    'Ref': {
      'title': '参照',
      'type': 'string',
      'format':'Reference(/API/Reference/ABC,xyz)',
    },
    'RefWrite': {
      'title': '参照',
      'type': 'string',
      'format':'Reference(/API/Reference/ABC,xyz)',
    },
    'RefProtect': {
      'title': '参照',
      'type': 'string',
      'format':'Reference(/API/Reference/ABC,xyz);Protect;Notify',
    },
    'RefArray': {
      'title': '参照_配列返却',
      'type': 'array',
      'format':'Reference(/API/Reference/ABC,xyz)',
    },
    'RefProtectArray': {
      'title': '参照_配列返却',
      'type': 'array',
      'format':'Reference(/API/Reference/ABC,xyz);Protect;Notify',
    },
    'RefProtectArrayOData': {
      'title': '参照_配列返却_OData',
      'type': 'array',
      'format':'Reference(/API/Reference/OData?$select=xyz,xyz);Protect;Notify',
    },
     'RefProtectArrayOData_multiproperty': {
      'title': '参照_配列返却_OData',
      'type': 'array',
      'format':'Reference(\""/API/Reference/OData?$select=tuw,xyz\"",uvw,xyz);Protect;Notify',
    },
     'RefProtect_ArrayNull_OData_multiproperty': {
      'title': '参照_配列返却_OData_ArrayNull',
      'type': ['array', 'null'],
      'format':'Reference(\""/API/Reference/OData?$select=tuw,xyz\"",uvw,xyz);Protect',
    },
     'RefProtect_NullArray_OData_multiproperty': {
      'title': '参照_配列返却_OData_ArrayNull',
      'type': ['null', 'array'],
      'format':'Reference(\""/API/Reference/OData?$select=tuw,xyz\"",uvw,xyz);Protect',
    },
    'RefNotify1': {
      'title': '参照',
      'type': 'string',
      'format':'Reference(/API/Reference/ABC,qwe);Protect;Notify',
    },
    'RefNotify2': {
      'title': '参照',
      'type': 'string',
      'format':'Reference(/API/Reference/ABC,zxc);Protect;Notify',
    },
    'RefNotify3': {
      'title': '参照',
      'type': 'string',
      'format':'Reference(/API/Reference/XYZ,xyz);Protect;Notify',
    },
    'ap': {
      'title': '配列',
      'type': 'array',
      'item': [ { 'type' : 'string' } ],
    },
    'ap2': {
      'title': '配列',
      'type': 'array',
      'item': [ { 'type' : 'string', } ],
    },
    'PropArray': {
      'title': '配列',
      'type': 'array',
      'items': {
        'properties': {
          'Date': {
            'title': '日付',
            'type': 'string',
          },
          'ObservationItemCode': {
            'title': '観察項目コード',
            'type': 'string',
          },
          'ObservationValue': {
            'title': '観察値',
            'type': 'string',
          },
        },
      },
    },
    'PropObject': {
      'title': 'オブジェクト',
      'type': 'object',
      'properties': {
        'prop1': {
          'title': 'prop1',
          'type': 'string',
        },
        'prop2': {
          'title': 'prop2',
          'type': 'string',
        },
        'prop3': {
          'title': 'prop3',
          'type': 'object',
          'properties': {
            'prop31': {
              'title': 'prop31',
              'type': 'string',
            },
          },
        },
      },
    },
    'ArrayObjectProtect': {
      'title': '配列',
      'type': 'array',
      'items': {
        'properties': {
          'Date': {
            'title': '日付',
            'type': 'string',
          },
        },
      },
    },
   'additionalProperties' : true
  },
  'type': 'object'
}";
        private static string protectOriginalJson1 = @"
{
    'Code' : '99',
    'Name' : '999',
    'temp' : 9,
    'Ref' : '$Reference(/API/Reference/ABC,def)',
    'RefProtect' : '$Reference(/API/Reference/ABC,xyz)',
    'RefWrite' : '$Reference(/API/Reference/ABC,abc);$Value(""4567890"")',
    'RefNotify1' : '$Reference(/API/Reference/ABC,qwe);$Value(""4567890"")',
    'RefNotify2' : '$Reference(/API/Reference/ABC,zxc);$Value(""456"")',
    'RefNotify3' : '$Reference(/API/Reference/XYZ,xyz);$Value(""987"")',
    'ap' : [ '1', '2', '3' ],
    'ap2' : [ '11', '22', '33' ],
    'PropArray' : [
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
    'PropObject' : {
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {
        'prop31' : '333',
      }
    },
    'ArrayObjectProtect' : [
      {
        'Date' : '2020/01/01',
      },
      {
        'Date' : '2020/02/02',
      },
    ],
}
";
        private static string protectUpdateAllInvalidJson = @"
{
    'Code' : '99',
    'Name' : '999',
    'temp' : 9,
    'Ref' : '12345',
    'RefProtect' : '45678',
    'RefWrite' : '32454545',
    'RefNotify1' : '32454545',
    'RefNotify1' : '32454545',
    'ap' : [ '1', '2', '3' ],
    'ap2' : [ '11', '22', '33' ],
    'PropArray' : [
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
    'PropObject' : {
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {
        'prop31' : '333',
      }
    },
    'ArrayObjectProtect' : [
      {
        'Date' : '2020/01/01',
      },
      {
        'Date' : '2020/02/02',
      },
    ],
}
";

        private static string protectUpdateJson = @"
{
    'Code' : '99',
    'Name' : '999',
    'temp' : 9,
    'Ref' : '$Value(12345)',
    'RefProtect' : '$Value(45678)',
    'RefWrite' : '$Value(32454545)',
    'RefNotify1' : '$Value(0987654321)',
    'RefNotify2' : '$Value(ABC)',
    'RefNotify3' : '$Value(あいうえお)',
    'ap' : [ '1', '2', '3' ],
    'ap2' : [ '11', '22', '33' ],
    'PropArray' : [
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
    'PropObject' : {
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {
        'prop31' : '333',
      }
    },
    'ArrayObjectProtect' : [
      {
        'Date' : '2020/01/01',
      },
      {
        'Date' : '2020/02/02',
      },
    ],
}
";
        private static string protectUpdateRefreshJson = @"
{
    'Code' : '99',
    'Name' : '999',
    'temp' : 9,
    'Ref' : '$Null',
    'RefProtect' : '$Null',
    'RefWrite' : '$Null',
    'ap' : [ '1', '2', '3' ],
    'ap2' : [ '11', '22', '33' ],
    'PropArray' : [
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
    'PropObject' : {
      'prop1' : '3',
      'prop2' : '33',
      'prop3' : {
        'prop31' : '333',
      }
    },
    'ArrayObjectProtect' : [
      {
        'Date' : '2020/01/01',
      },
      {
        'Date' : '2020/02/02',
      },
    ],
}
";
        private static string protectUpdateODataJson = @"
{
    'Code' : '99',
    'Name' : '999',
    'temp' : 9,
    'Ref' : '$Null',
    'RefProtect' : '123452',
    'RefWrite' : '$Null',
    'RefArray' : '$Reference(/API/Reference/ABC,def)',
    'RefProtectArrayOData_multiproperty' : [
       {'id' : 'hogeUpdateId1', 'uvw': 'hoge', 'xyz': 'moge' },
       {'id' : 'hogeRegistId3', 'uvw': 'hoge3', 'xyz': 'moge3' },
    ]
}
";

        private static string otherJson = @"
[{
    'id':  'hogeId1',
    'abc' : '123',
    'def' : '456',
    'xyz' : 789,
}]
";

        private static string referenceArraySingleTestJson = @"
{
    'Code' : '99',
    'Name' : '999',
    'temp' : 9,
    'Ref' : '$Reference(/API/Reference/ABC,def)',
    'RefProtect' : '$Reference(/API/Reference/ABC,xyz)',
    'RefWrite' : '$Reference(/API/Reference/ABC,abc);$Value(""4567890"")',
    'RefArray' : '$Reference(/API/Reference/ABC,def)',
    'RefProtectArray' : '$Reference(/API/Reference/ABC,xyz)',
    'RefProtectArrayOData' : '$Reference(/API/Reference/OData?$select=xyz,xyz)',
    'RefProtectArrayOData_multiproperty' : '$Reference(\""/API/Reference/OData?$select=uvw,xyz\"",uvw,xyz)',
    'RefProtect_ArrayNull_OData_multiproperty' : '$Reference(\""/API/Reference/OData?$select=uvw,xyz\"",uvw,xyz)',
    'RefProtect_NullArray_OData_multiproperty' : '$Reference(\""/API/Reference/OData?$select=uvw,xyz\"",uvw,xyz)',
}
";
        private static string otherJsonArray_index_1 = @"
[
    {
        'abc' : '123',
        'def' : '456',
        'xyz' : 789,
        'uvw' : 111

    }
]
";
        private static string otherJsonArray_index_2 = @"
[
    {
        'abc' : '123',
        'def' : '456',
        'uvw' : 123,
        'xyz' : 789,

    },
    {
        'abc' : 'hoge',
        'def' : 'fuga',
        'uvw' : 111,
        'xyz' : 000,
    }
]
";
        private static string otherJsonArray_index_3 = @"
[
    {
        'id'  : 'hogeUpdateId1',
        'abc' : '123',
        'def' : '456',
        'uvw' : 123,
        'xyz' : 789,

    },
    {
        'id'  : 'hogeDeleteId2',
        'abc' : 'hoge',
        'def' : 'fuga',
        'uvw' : 111,
        'xyz' : 000,
    }
]
";


        //異常系：int とstring 混合

        private static string otherJsonArrayAbnormalPattern_intstring_mixed = @"
[
    {
        'abc' : 123,
    },
    {
        'abc' : 'hoge',
    }
]
";
        //異常系：二重配列
        private static string otherJsonArrayAbnormalPattern_doubleArray = @"
[
    {
        'abc' : ['def':'hogehoge','xyz':'fugafuga'],
    },
    {
        'abc' : 'hoge',
    }
]
";


        private JSchema schema = ToJSchema(protectSchema);

        private static JSchema? ToJSchema(string str)
        {
            JSchemaReaderSettings settings = new JSchemaReaderSettings
            {
                Validators = new List<IJsonValidator> { new JsonFormatCustomValidator() }
            };
            return str == null ? null : JSchema.Parse(str, settings);
        }


        [TestInitialize]
        public override void TestInitialize()
        {
            UnityCore.UnityContainer = null;
            var unityContainer = new UnityContainer();

            unityContainer.RegisterInstance(Configuration);

            UnityCore.UnityContainer = unityContainer;
        }

        [TestMethod]
        public void RefProtect_Reference()
        {
            // 参照(=Reference)が設定されているのはオリジナルなものか？
            // 参照定義されているが、$Valueとして値を更新しているようなデータは、更新された値を返しているか？チェックする
            var json = protectOriginalJson1.ToJson();
            _otherData = otherJson.ToJson();
            var fmt = new JsonPropertyFormatProtect(false) { Schema = schema, Update = json, GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'456'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'789'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'\"4567890\"'"));
            Same(json, protectOriginalJson1.ToJson(), "Ref", "RefProtect", "RefWrite", "RefNotify1", "RefNotify2", "RefNotify3");
        }

        [TestMethod]
        public void RefProtect_Update_AllInvalid()
        {
            // 更新処理だが、$Valueでもないのですべて戻す
            // Protectの場合は$Valueすらも受け付けない
            var json = protectUpdateAllInvalidJson.ToJson();
            _otherData = otherJson.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => protectOriginalJson1.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def);$Value(12345)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc);$Value(32454545)'"));
            Same(json, protectOriginalJson1.ToJson(), "Ref", "RefProtect", "RefWrite", "RefNotify1", "RefNotify2", "RefNotify3");
        }

        [TestMethod]
        public void RefProtect_Update()
        {
            // 更新処理
            // Ref：Protectではないので書き込みしに来た値も保持する
            // RefProtect : Protectなので書き込みに来た値は無視し、オリジナルの値になる
            // RefWrite : Protectではないので書き込みしに来た値も保持する
            var json = protectUpdateJson.ToJson();
            _otherData = otherJson.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => protectOriginalJson1.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def);$Value($Value(12345))'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc);$Value($Value(32454545))'"));
            Same(json, protectOriginalJson1.ToJson(), "Ref", "RefProtect", "RefWrite");

            // $Nullでもとに戻す
            json = protectUpdateRefreshJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => protectOriginalJson1.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
        }

        [TestMethod]
        public void RefProtect_Notify()
        {
            // 更新処理
            // RefNotify : UpdateOtherResourceに通知される
            var json = protectUpdateJson.ToJson();
            _otherData = otherJson.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => protectOriginalJson1.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def);$Value($Value(12345))'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc);$Value($Value(32454545))'"));
            Same(json, protectOriginalJson1.ToJson(), "Ref", "RefProtect", "RefWrite");
            fmt.UpdateOtherResource.Count.Is(4);
            fmt.UpdateOtherResource[0].Url.Is("/API/Reference/UpdateById/hogeId1");
            fmt.UpdateOtherResource[0].Property.Is("xyz");
            fmt.UpdateOtherResource[0].Value.Is("45678");
            fmt.UpdateOtherResource[1].Url.Is("/API/Reference/UpdateById/hogeId1");
            fmt.UpdateOtherResource[1].Property.Is("qwe");
            fmt.UpdateOtherResource[1].Value.Is("0987654321");
            fmt.UpdateOtherResource[2].Url.Is("/API/Reference/UpdateById/hogeId1");
            fmt.UpdateOtherResource[2].Property.Is("zxc");
            fmt.UpdateOtherResource[2].Value.Is("ABC");
            fmt.UpdateOtherResource[3].Url.Is("/API/Reference/UpdateById/hogeId1");
            fmt.UpdateOtherResource[3].Property.Is("xyz");
            fmt.UpdateOtherResource[3].Value.Is("あいうえお");

            // $Nullでもとに戻す
            json = protectUpdateRefreshJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => protectOriginalJson1.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
        }

        [TestMethod]
        public void RefProtect_Notify_IncludeOData()
        {
            // 更新処理
            var json = protectUpdateODataJson.ToJson();
            _otherData = otherJsonArray_index_3.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
            Same(json, referenceArraySingleTestJson.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray",
                "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
            fmt.UpdateOtherResource.Count.Is(4);
            var patchMethod = new HttpMethod("Patch");
            fmt.UpdateOtherResource[0].Url.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].Property.Is("xyz");
            fmt.UpdateOtherResource[0].Value.Is("123452");
            fmt.UpdateOtherResource[0].RollbackUrl.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].RollbackValue.Is("789");
            fmt.UpdateOtherResource[0].TargetHttpMethod.Is(patchMethod);
            fmt.UpdateOtherResource[0].RollbackHttpMethod.Is(patchMethod);

            fmt.UpdateOtherResource[1].Url.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[1].TargetHttpMethod.Is(patchMethod);
            fmt.UpdateOtherResource[1].RollbackHttpMethod.Is(patchMethod);
            fmt.UpdateOtherResource[1].Value.ToJson().Is("{'id': 'hogeUpdateId1','uvw':'hoge','xyz':'moge'}".ToJson());
            fmt.UpdateOtherResource[1].RollbackUrl.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[1].RollbackValue.ToJson().Is("{'id': 'hogeUpdateId1','abc': '123','def': '456', 'uvw':123,'xyz':789}".ToJson());
            fmt.UpdateOtherResource[2].Url.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[2].TargetHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[2].Value.ToJson().Is("{'id':'hogeRegistId3','uvw':'hoge3','xyz':'moge3'}".ToJson());
            fmt.UpdateOtherResource[2].RollbackUrl.IsNull();
            fmt.UpdateOtherResource[2].RollbackHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[3].Url.Is("/API/Reference/DeleteById/hogeDeleteId2");
            fmt.UpdateOtherResource[3].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[3].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[3].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[3].RollbackValue.ToJson().Is("{'id':'hogeDeleteId2','abc':'hoge','def':'fuga','uvw':111,'xyz':0}".ToJson());

            // $Nullでもとに戻す
            json = protectUpdateODataJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true)
            {
                Schema = schema,
                Update = json,
                GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(),
                GetOtherResource = (string url, bool flg)
                    => new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(HttpStatusCode.OK, otherJsonArray_index_3.ToJson(), false, _controllerUrl, _keyProperties)
            };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
        }

        [TestMethod]
        public void RefProtect_Notify_IncludeOData_NotifyPropertyNullRequest()
        {
            // 更新処理
            var json = protectUpdateODataJson.ToJson();
            json["RefProtectArrayOData_multiproperty"] = null;

            _otherData = otherJsonArray_index_3.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
            Same(json, referenceArraySingleTestJson.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray",
                "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
            fmt.UpdateOtherResource.Count.Is(3);
            fmt.UpdateOtherResource[0].TargetHttpMethod.Is(new HttpMethod("Patch"));
            fmt.UpdateOtherResource[0].Url.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].Property.Is("xyz");
            fmt.UpdateOtherResource[0].Value.Is("123452");
            fmt.UpdateOtherResource[0].RollbackUrl.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].RollbackValue.Is("789");
            fmt.UpdateOtherResource[0].RollbackHttpMethod.Is(new HttpMethod("Patch"));


            //ODataNotifyのデータは、全部Deleteのはず
            fmt.UpdateOtherResource[1].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[1].Url.Is("/API/Reference/DeleteById/hogeUpdateId1");
            fmt.UpdateOtherResource[1].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[1].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[1].RollbackValue.ToJson().Is("{'id': 'hogeUpdateId1','abc': '123','def': '456', 'uvw':123,'xyz':789}".ToJson());
            fmt.UpdateOtherResource[2].Url.Is("/API/Reference/DeleteById/hogeDeleteId2");
            fmt.UpdateOtherResource[2].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[2].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[2].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[2].RollbackValue.ToJson().Is("{'id':'hogeDeleteId2','abc':'hoge','def':'fuga','uvw':111,'xyz':0}".ToJson());

            // $Nullでもとに戻す
            json = protectUpdateODataJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true)
            {
                Schema = schema,
                Update = json,
                GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(),
                GetOtherResource = (string url, bool flg)
                    => new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(HttpStatusCode.OK, otherJsonArray_index_3.ToJson(), false, _controllerUrl, _keyProperties)
            };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
        }

        [TestMethod]
        public void RefProtect_Notify_IncludeOData_NotifyPropertyEmptyArrayRequest()
        {
            // 更新処理
            var json = protectUpdateODataJson.ToJson();
            json["RefProtectArrayOData_multiproperty"] = "[]".ToJson();

            _otherData = otherJsonArray_index_3.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
            Same(json, referenceArraySingleTestJson.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray",
                "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
            fmt.UpdateOtherResource.Count.Is(3);
            fmt.UpdateOtherResource[0].TargetHttpMethod.Is(new HttpMethod("Patch"));
            fmt.UpdateOtherResource[0].Url.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].Property.Is("xyz");
            fmt.UpdateOtherResource[0].Value.Is("123452");
            fmt.UpdateOtherResource[0].RollbackUrl.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].RollbackValue.Is("789");
            fmt.UpdateOtherResource[0].RollbackHttpMethod.Is(new HttpMethod("Patch"));

            //ODataNotifyのデータは、全部Deleteのはず
            fmt.UpdateOtherResource[1].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[1].Url.Is("/API/Reference/DeleteById/hogeUpdateId1");
            fmt.UpdateOtherResource[1].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[1].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[1].RollbackValue.ToJson().Is("{'id': 'hogeUpdateId1','abc': '123','def': '456', 'uvw':123,'xyz':789}".ToJson());
            fmt.UpdateOtherResource[2].Url.Is("/API/Reference/DeleteById/hogeDeleteId2");
            fmt.UpdateOtherResource[2].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[2].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[2].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[2].RollbackValue.ToJson().Is("{'id':'hogeDeleteId2','abc':'hoge','def':'fuga','uvw':111,'xyz':0}".ToJson());

            // $Nullでもとに戻す
            json = protectUpdateODataJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true)
            {
                Schema = schema,
                Update = json,
                GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(),
                GetOtherResource = (string url, bool flg)
                    => new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(HttpStatusCode.OK, otherJsonArray_index_3.ToJson(), false, _controllerUrl, _keyProperties)
            };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
        }

        [TestMethod]
        public void RefProtect_Notify_IncludeOData_NotifyPropertyEmptyJsonRequest()
        {
            // 更新処理
            var json = protectUpdateODataJson.ToJson();
            json["RefProtectArrayOData_multiproperty"] = "{}".ToJson();

            _otherData = otherJsonArray_index_3.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
            Same(json, referenceArraySingleTestJson.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray",
                "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
            fmt.UpdateOtherResource.Count.Is(3);
            fmt.UpdateOtherResource[0].TargetHttpMethod.Is(new HttpMethod("Patch"));
            fmt.UpdateOtherResource[0].Url.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].Property.Is("xyz");
            fmt.UpdateOtherResource[0].Value.Is("123452");
            fmt.UpdateOtherResource[0].RollbackUrl.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].RollbackValue.Is("789");
            fmt.UpdateOtherResource[0].RollbackHttpMethod.Is(new HttpMethod("Patch"));

            //ODataNotifyのデータは、全部Deleteのはず
            fmt.UpdateOtherResource[1].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[1].Url.Is("/API/Reference/DeleteById/hogeUpdateId1");
            fmt.UpdateOtherResource[1].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[1].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[1].RollbackValue.ToJson().Is("{'id': 'hogeUpdateId1','abc': '123','def': '456', 'uvw':123,'xyz':789}".ToJson());
            fmt.UpdateOtherResource[2].Url.Is("/API/Reference/DeleteById/hogeDeleteId2");
            fmt.UpdateOtherResource[2].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[2].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[2].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[2].RollbackValue.ToJson().Is("{'id':'hogeDeleteId2','abc':'hoge','def':'fuga','uvw':111,'xyz':0}".ToJson());

            // $Nullでもとに戻す
            json = protectUpdateODataJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true)
            {
                Schema = schema,
                Update = json,
                GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(),
                GetOtherResource = (string url, bool flg)
                    => new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(HttpStatusCode.OK, otherJsonArray_index_3.ToJson(), false, _controllerUrl, _keyProperties)
            };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
        }

        [TestMethod]
        public void RefProtect_Notify_IncludeOData_NotifyPropertyEmptyStringRequest()
        {
            // 更新処理
            var json = protectUpdateODataJson.ToJson();
            json["RefProtectArrayOData_multiproperty"] = "";

            _otherData = otherJsonArray_index_3.ToJson();
            var fmt = new JsonPropertyFormatProtect(true) { Schema = schema, Update = json, GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(), GetOtherResource = GetOther };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
            Same(json, referenceArraySingleTestJson.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray",
                "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
            fmt.UpdateOtherResource.Count.Is(3);
            fmt.UpdateOtherResource[0].TargetHttpMethod.Is(new HttpMethod("Patch"));
            fmt.UpdateOtherResource[0].Url.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].Property.Is("xyz");
            fmt.UpdateOtherResource[0].Value.Is("123452");
            fmt.UpdateOtherResource[0].RollbackUrl.Is("/API/Reference/UpdateById/hogeUpdateId1");
            fmt.UpdateOtherResource[0].RollbackValue.Is("789");
            fmt.UpdateOtherResource[0].RollbackHttpMethod.Is(new HttpMethod("Patch"));

            //ODataNotifyのデータは、全部Deleteのはず
            fmt.UpdateOtherResource[1].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[1].Url.Is("/API/Reference/DeleteById/hogeUpdateId1");
            fmt.UpdateOtherResource[1].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[1].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[1].RollbackValue.ToJson().Is("{'id': 'hogeUpdateId1','abc': '123','def': '456', 'uvw':123,'xyz':789}".ToJson());
            fmt.UpdateOtherResource[2].Url.Is("/API/Reference/DeleteById/hogeDeleteId2");
            fmt.UpdateOtherResource[2].TargetHttpMethod.Is(HttpMethod.Delete);
            fmt.UpdateOtherResource[2].RollbackUrl.Is("/API/Reference/Register");
            fmt.UpdateOtherResource[2].RollbackHttpMethod.Is(HttpMethod.Post);
            fmt.UpdateOtherResource[2].RollbackValue.ToJson().Is("{'id':'hogeDeleteId2','abc':'hoge','def':'fuga','uvw':111,'xyz':0}".ToJson());

            // $Nullでもとに戻す
            json = protectUpdateODataJson.ToJson();
            fmt = new JsonPropertyFormatProtect(true)
            {
                Schema = schema,
                Update = json,
                GetOriginalData = (JToken token, bool flg) => referenceArraySingleTestJson.ToJson(),
                GetOtherResource = (string url, bool flg)
                    => new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(HttpStatusCode.OK, otherJsonArray_index_3.ToJson(), false, _controllerUrl, _keyProperties)
            };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'$Reference(/API/Reference/ABC,def)'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'$Reference(/API/Reference/ABC,xyz)'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'$Reference(/API/Reference/ABC,abc)'"));
            json.GetPropertyValue("RefProtectArrayOData_multiproperty").Is(JValue.Parse("'$Reference(\"/API/Reference/OData?$select=uvw,xyz\",uvw,xyz)'"));
        }

        /// <summary>
        /// 参照先からの返却値が、Arrayだけど単数
        /// </summary>
        [TestMethod]
        public void ArrayValueSingleValue_ReturnTest_ArrayIndex1()
        {
            var json = referenceArraySingleTestJson.ToJson();
            _otherData = otherJsonArray_index_1.ToJson();
            var fmt = new JsonPropertyFormatProtect(false)
            {
                Schema = schema,
                Update = json,
                GetOtherResource = GetOther
            };
            fmt.Recovery();
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'\"4567890\"'"));
            json.GetPropertyValue("RefArray").Is("{'def':'456'}".ToJArray());
            json.GetPropertyValue("RefProtectArray").Is("{'xyz':789}".ToJArray());

            var jarr = json.GetPropertyValue("RefProtectArrayOData");
            jarr[0].Is(JValue.Parse("{'xyz':789}"));
            jarr = json.GetPropertyValue("RefProtectArrayOData_multiproperty");
            jarr[0].Is(JValue.Parse("{'xyz':789,'uvw':111}"));

            jarr = json.GetPropertyValue("RefProtect_ArrayNull_OData_multiproperty");
            jarr[0].Is(JValue.Parse("{'xyz':789,'uvw':111}"));
            jarr = json.GetPropertyValue("RefProtect_NullArray_OData_multiproperty");
            jarr[0].Is(JValue.Parse("{'xyz':789,'uvw':111}"));

            Same(json, protectOriginalJson1.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray", "RefProtectArray",
                "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
        }

        /// <summary>
        /// Reference先からの返却値が、Arrayで複数
        /// </summary>
        [TestMethod]
        public void ArrayValueSingleValue_ReturnTest_ArrayIndex_2()
        {
            var json = referenceArraySingleTestJson.ToJson();
            _otherData = otherJsonArray_index_2.ToJson();
            var fmt = new JsonPropertyFormatProtect(false)
            {
                Schema = schema,
                Update = json,
                GetOtherResource = GetOther
            };
            fmt.Recovery();
            json.GetPropertyValue("Ref").Is(JValue.Parse("'456'"));
            json.GetPropertyValue("RefProtect").Is(JValue.Parse("'789'"));
            json.GetPropertyValue("RefWrite").Is(JValue.Parse("'\"4567890\"'"));
            json.GetPropertyValue("RefArray").Is(JArray.Parse("[{'def':'456'},{'def':'fuga'}]"));
            json.GetPropertyValue("RefProtectArray").Is("{'xyz':789},{'xyz':0}".ToJArray());

            var jarr = json.GetPropertyValue("RefProtectArrayOData");
            jarr[0].Is(JValue.Parse("{'xyz':789}"));
            jarr[1].Is(JValue.Parse("{'xyz':0}"));

            jarr = json.GetPropertyValue("RefProtectArrayOData_multiproperty");
            jarr[0].Is(JValue.Parse("{'xyz':789,'uvw':123}"));
            jarr[1].Is(JValue.Parse("{'xyz':0,'uvw':111}"));

            jarr = json.GetPropertyValue("RefProtect_ArrayNull_OData_multiproperty");
            jarr[0].Is(JValue.Parse("{'xyz':789,'uvw':123}"));
            jarr[1].Is(JValue.Parse("{'xyz':0,'uvw':111}"));

            jarr = json.GetPropertyValue("RefProtect_NullArray_OData_multiproperty");
            jarr[0].Is(JValue.Parse("{'xyz':789,'uvw':123}"));
            jarr[1].Is(JValue.Parse("{'xyz':0,'uvw':111}"));

            Same(json, protectOriginalJson1.ToJson(), "Ref", "RefProtect", "RefWrite", "RefArray", "RefProtectArray", "RefProtectArrayOData", "RefProtectArrayOData_multiproperty", "RefProtect_ArrayNull_OData_multiproperty", "RefProtect_NullArray_OData_multiproperty");
        }

        [TestMethod]
        public void ArrayValueSingleValue_Reference結果_intとstringの配列()
        {
            var json = referenceArraySingleTestJson.ToJson();
            _otherData = otherJsonArrayAbnormalPattern_intstring_mixed.ToJson();
            var fmt = new JsonPropertyFormatProtect(false)
            {
                Schema = schema,
                Update = json,
                GetOtherResource = GetOther
            };

            fmt.Recovery();
        }

        [TestMethod]
        public void ArrayValueSingleValue_Reference結果異常テスト_二重配列()
        {
            var json = referenceArraySingleTestJson.ToJson();
            var fmt = new JsonPropertyFormatProtect(false)
            {
                Schema = schema,
                Update = json,
                GetOtherResource = (string url, bool flg) => new Tuple<HttpStatusCode, JToken, bool, string, List<string>>
                    (HttpStatusCode.OK, otherJsonArrayAbnormalPattern_doubleArray.ToJson(), false, null, null)
            };
            try
            {
                fmt.Recovery();
            }
            catch
            {
                //異常終了すること
                return;
            }
            throw new AssertFailedException();
        }


        private string _controllerUrl = "/API/Reference";
        private List<string> _keyProperties = new List<string>();
        private JToken _otherData = "[{'id':'hogeId1', 'hogepro1':'hogehoge11', 'hogepro2':'hogehoge12', 'ConversionSquareMeters2':100}, {'id':'hogeId3', 'hogepro1':'hogehoge31', 'hogepro2':'hogehoge32', 'ConversionSquareMeters2':300}]".ToJson();

        public Tuple<HttpStatusCode, JToken, bool, string, List<string>> GetOther(string url, bool flg)
        {
            return new Tuple<HttpStatusCode, JToken, bool, string, List<string>>(HttpStatusCode.OK, _otherData, url.Contains("/OData") ? true : false, _controllerUrl, _keyProperties);
        }

        private void Same(JToken json1, JToken json2, params string[] skipPath)
        {
            List<string> path = json1.Children().ToList().Select(x => x.Path).ToList();
            if (path != null)
            {
                foreach (var x in skipPath)
                {
                    path.Remove(x);
                }
            }
            path.ForEach(x =>
            {
                Console.WriteLine(x);
                json1.GetPropertyValue(x).Is(json2.GetPropertyValue(x));
            });
        }
    }
}
