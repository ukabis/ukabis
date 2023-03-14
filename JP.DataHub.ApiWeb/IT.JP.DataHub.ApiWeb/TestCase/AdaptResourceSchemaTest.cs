using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using Polly.Retry;
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
    [TestCategory("ManageAPI")]
    public class AdaptResourceSchemaTest : ApiWebItTestCase
    {
        #region TestData

        private class AdaptResourceSchemaTestData : TestDataBase
        {
            #region Data

            public string Data1 = $@"
[
    {{
        'STR_VALUE': 'data1',
        'STR_NULL': null,
        'INT_VALUE': 1234,
        'DBL_VALUE': 1234.5678,
        'NUM_NULL': null,
        'OBJ_VALUE': {{ 'key1': 'value1' }},
        'OBJ_NULL': null,
        'ARY_VALUE': [ 'value1-1', 'value1-2' ],
        'ARY_NULL': null,
        'BOL_VALUE': true,
        'BOL_NULL': null,
        'DAT_VALUE': '2018-11-13T20:20:39',
        'DAT_NULL': null,
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': '魑魅魍魎檸檬憂鬱',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""!#$%&'()*+,-./:;<=>?@[\\]^_`{{|}}~\""""
    }}
]";

            public string Data1GetExpected = $@"
[
    {{
        'id': '{WILDCARD}',
        'STR_VALUE': 'data1',
        'STR_NULL': null,
        'INT_VALUE': 1234.0,
        'DBL_VALUE': 1234.5678,
        'NUM_NULL': null,
        'OBJ_VALUE': {{ 'key1': 'value1' }},
        'OBJ_NULL': null,
        'ARY_VALUE': [ 'value1-1', 'value1-2' ],
        'ARY_NULL': null,
        'BOL_VALUE': true,
        'BOL_NULL': null,
        'DAT_VALUE': '2018-11-13T20:20:39',
        'DAT_NULL': null,
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': '魑魅魍魎檸檬憂鬱',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""!#$%&'()*+,-./:;<=>?@[\\]^_`{{|}}~\"""",
        '_Owner_Id': '{WILDCARD}'
    }}
]";


            public string Data1GetExpectedAfterAlterTable = $@"
[
    {{
        'id': '{WILDCARD}',
        'STR_VALUE': 'data1',
        'STR_NULL': null,
        'INT_VALUE': 1234.0,
        'DBL_VALUE': 1234.5678,
        'NUM_NULL': null,
        'OBJ_VALUE': {{ 'key1': 'value1' }},
        'OBJ_NULL': null,
        'ARY_VALUE': [ 'value1-1', 'value1-2' ],
        'ARY_NULL': null,
        'BOL_VALUE': true,
        'BOL_NULL': null,
        'DAT_VALUE': '2018-11-13T20:20:39',
        'DAT_NULL': null,
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': '魑魅魍魎檸檬憂鬱',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""!#$%&'()*+,-./:;<=>?@[\\]^_`{{|}}~\"""",
        'ADD_STR2': null,
        'ADD_NUM2': null,
        'ADD_OBJ2': null,
        'ADD_ARY2': null,
        'ADD_BOL2': null,
        'ADD_DAT2': null,
        '_Owner_Id': '{WILDCARD}'
    }}
]";

            public string Data2 = $@"
[
    {{
        'ADD_STR1': 'hoge',
        'STR_VALUE': 'data1',
        'STR_NULL': 'fuga',
        'ADD_STR2': 'piyo',
        'ADD_NUM1': 1111,
        'INT_VALUE': 2222,
        'DBL_VALUE': 3333.333,
        'NUM_NULL': 4444,
        'ADD_NUM2': 5555.555,
        'ADD_OBJ1': {{ 'key1-1': 'value1-1' }},
        'OBJ_VALUE': {{ 'key1-2': 'value1-2' }},
        'OBJ_NULL': {{ 'key1-3': 'value1-3' }},
        'ADD_OBJ2': {{ 'key1-4': 'value1-4' }},
        'ADD_ARY1': [ 'value1-1-1', 'value1-1-2' ],
        'ARY_VALUE': [ 'value1-2-1', 'value1-2-2' ],
        'ARY_NULL': [ 'value1-3-1', 'value1-3-2' ],
        'ADD_ARY2': [ 'value1-4-1', 'value1-4-2' ],
        'ADD_BOL1': true,
        'BOL_VALUE': true,
        'BOL_NULL': false,
        'ADD_BOL2': false,
        'ADD_DAT1': '2018-11-13T20:20:39+09:00',
        'DAT_VALUE': '2018-11-13T20:20:39',
        'DAT_NULL': '2019-11-13T20:20:39+09:00',
        'ADD_DAT2': '2019-11-13T20:20:39',
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': 'ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿ',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""^	　 $""
    }},
    {{
        'ADD_STR1': 'foo',
        'STR_VALUE': 'data2',
        'STR_NULL': 'bar',
        'ADD_STR2': null,
        'ADD_NUM1': 6666,
        'INT_VALUE': 9999,
        'DBL_VALUE': 8888.888,
        'NUM_NULL': 7777.777,
        'ADD_NUM2': null,
        'ADD_OBJ1': {{ 'key2-1': 'value2-1' }},
        'OBJ_VALUE': {{ 'key2-2': 'value2-2' }},
        'OBJ_NULL': {{ 'key2-3': 'value2-3' }},
        'ADD_OBJ2': null,
        'ADD_ARY1': [ 'value2-1-1', 'value2-1-2' ],
        'ARY_VALUE': [ 'value2-2-1', 'value2-2-2' ],
        'ARY_NULL': [ 'value2-3-1', 'value2-3-2' ],
        'ADD_ARY2': null,
        'ADD_BOL1': false,
        'BOL_VALUE': false,
        'BOL_NULL': true,
        'ADD_BOL2': null,
        'ADD_DAT1': '2020-11-13T20:20:39',
        'DAT_VALUE': '2020-11-13T20:20:39+09:00',
        'DAT_NULL': '2021-11-13T20:20:39',
        'ADD_DAT2': null,
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': '魑魅魍魎檸檬憂鬱',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""!#$%&'()*+,-./:;<=>?@[\\]^_`{{{{|}}}}~\""""
    }}
]";

            public string Data2GetExpected = $@"
[
    {{
        'id': '{WILDCARD}',
        'STR_VALUE': 'data1',
        'STR_NULL': 'fuga',
        'INT_VALUE': 2222,
        'DBL_VALUE': 3333.333,
        'NUM_NULL': 4444,
        'OBJ_VALUE': {{ 'key1-2': 'value1-2' }},
        'OBJ_NULL': {{ 'key1-3': 'value1-3' }},
        'ARY_VALUE': [ 'value1-2-1', 'value1-2-2' ],
        'ARY_NULL': [ 'value1-3-1', 'value1-3-2' ],
        'BOL_VALUE': true,
        'BOL_NULL': false,
        'DAT_VALUE': '2018-11-13T20:20:39',
        'DAT_NULL': '2019-11-13T11:20:39',
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': 'ｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿ',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""^	　 $"",
        'ADD_STR1': 'hoge',
        'ADD_STR2': 'piyo',
        'ADD_NUM1': 1111,
        'ADD_NUM2': 5555.555,
        'ADD_OBJ1': {{ 'key1-1': 'value1-1' }},
        'ADD_OBJ2': {{ 'key1-4': 'value1-4' }},
        'ADD_ARY1': [ 'value1-1-1', 'value1-1-2' ],
        'ADD_ARY2': [ 'value1-4-1', 'value1-4-2' ],
        'ADD_BOL1': true,
        'ADD_BOL2': false,
        'ADD_DAT1': '2018-11-13T11:20:39',
        'ADD_DAT2': '2019-11-13T20:20:39',
        '_Owner_Id': '{WILDCARD}'
    }},
    {{
        'id': '{WILDCARD}',
        'STR_VALUE': 'data2',
        'STR_NULL': 'bar',
        'INT_VALUE': 9999,
        'DBL_VALUE': 8888.888,
        'NUM_NULL': 7777.777,
        'OBJ_VALUE': {{ 'key2-2': 'value2-2' }},
        'OBJ_NULL': {{ 'key2-3': 'value2-3' }},
        'ARY_VALUE': [ 'value2-2-1', 'value2-2-2' ],
        'ARY_NULL': [ 'value2-3-1', 'value2-3-2' ],
        'BOL_VALUE': false,
        'BOL_NULL': true,
        'DAT_VALUE': '2020-11-13T11:20:39',
        'DAT_NULL': '2021-11-13T20:20:39',
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': '魑魅魍魎檸檬憂鬱',
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": ""!#$%&'()*+,-./:;<=>?@[\\]^_`{{{{|}}}}~\"""",
        'ADD_STR1': 'foo',
        'ADD_STR2': null,
        'ADD_NUM1': 6666,
        'ADD_NUM2': null,
        'ADD_OBJ1': {{ 'key2-1': 'value2-1' }},
        'ADD_OBJ2': null,
        'ADD_ARY1': [ 'value2-1-1', 'value2-1-2' ],
        'ADD_ARY2': null,
        'ADD_BOL1': false,
        'ADD_BOL2': null,
        'ADD_DAT1': '2020-11-13T20:20:39',
        'ADD_DAT2': null,
        '_Owner_Id': '{WILDCARD}'
    }}
]";

            #endregion


            #region JsonSchemas

            public string JsonSchemaBase = $@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}";

            public string JsonSchemaAllowAdditionalProperties = $@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": {{ 'type': 'string' }}
    }}
}}";

            public string JsonSchemaDeleteColumn = $@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}";

            public string JsonSchemaModifyColumn = $@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'string' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}";

            public string JsonSchemaInvalidColumnName = $@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~@"": {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}";

            public string JsonSchemaInvalidColumnLength = $@"
{{
    'type': 'object',
    'properties': {{
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$x': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}";

            public string JsonSchemaAddColumn = $@"
{{
    'type': 'object',
    'properties': {{
        'ADD_STR1':     {{ 'type': 'string' }},
        'STR_VALUE':    {{ 'type': 'string' }},
        'STR_NULL':     {{ 'type': ['string', 'null'] }},
        'ADD_STR2':     {{ 'type': ['string', 'null'] }},
        'ADD_NUM1':     {{ 'type': 'number' }},
        'INT_VALUE':    {{ 'type': 'number' }},
        'DBL_VALUE':    {{ 'type': 'number' }},
        'NUM_NULL':     {{ 'type': ['number', 'null'] }},
        'ADD_NUM2':     {{ 'type': ['number', 'null'] }},
        'ADD_OBJ1':     {{ 'type': 'object' }},
        'OBJ_VALUE':    {{ 'type': 'object' }},
        'OBJ_NULL':     {{ 'type': ['object', 'null'] }},
        'ADD_OBJ2':     {{ 'type': ['object', 'null'] }},
        'ADD_ARY1':     {{ 'type': 'array' }},
        'ARY_VALUE':    {{ 'type': 'array' }},
        'ARY_NULL':     {{ 'type': ['array', 'null'] }},
        'ADD_ARY2':     {{ 'type': ['array', 'null'] }},
        'ADD_BOL1':     {{ 'type': 'boolean' }},
        'BOL_VALUE':    {{ 'type': 'boolean' }},
        'BOL_NULL':     {{ 'type': ['boolean', 'null'] }},
        'ADD_BOL2':     {{ 'type': ['boolean', 'null'] }},
        'ADD_DAT1':     {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_VALUE':    {{ 'type': 'string', 'format': 'date-time' }},
        'DAT_NULL':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        'ADD_DAT2':     {{ 'type': ['string', 'null'], 'format': 'date-time' }},
        '列名最大長 88バイト　　　　　　　　　　　　　　　　　　　　$': {{ 'type': 'string' }},
        ""記号 !#$%&'()*+,-./:;<=>?[\\]^_`{{|}}~"": {{ 'type': 'string' }},
        '_etag':        {{ 'type': 'string' }}
    }},
    'additionalProperties': false
}}";

            #endregion

            public AdaptResourceSchemaTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        // 実行毎にリソースとテーブルを作成するため一部環境でのみ実行
        [TestMethod]
        public void AdaptResourceSchemaTest_NormalSenario()
        {
            var repository = Repository.SqlServer;
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAdaptResourceSchemaApi>();
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new AdaptResourceSchemaTestData(repository, api.ResourceUrl);

            var vendorId = client.VendorSystemInfo.VendorId;
            var systemId = client.VendorSystemInfo.SystemId;
            var repositoryGroupId = AppConfig.SqlServerRepositoryGroupId.ToString();

            // モデル初期化
            var resouceSchema = clientM.GetWebApiResponseResult(manageApi.GetSchemas()).Assert(GetSuccessExpectStatusCode).Result.First(x => x.SchemaName == "/API/IntegratedTest/AdaptResourceSchemaTest/");
            resouceSchema.JsonSchema = testData.JsonSchemaBase;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            // 既存リソースを削除
            clientM.GetWebApiResponseResult(manageApi.DeleteApiFromUrl(testData.ResourceUrl)).Assert(new HttpStatusCode[] { HttpStatusCode.NotFound, HttpStatusCode.NoContent });

            // 新規リソース/APIを作成
            var apiDef = new ApiModel()
            {
                VendorId = vendorId,
                SystemId = systemId,
                ApiName = testData.ResourceUrl,
                RelativeUrl = testData.ResourceUrl,
                RepositoryKey = testData.ResourceUrl + "/{STR_VALUE}",
                PartitionKey = testData.ResourceUrl,
                IsEnable = true,
            };
            var apiId = clientM.GetWebApiResponseResult(manageApi.RegisterApi(apiDef)).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;

            var methodDef = new RegisterMethodModel()
            {
                ApiId = apiId,
                HttpMethodTypeCd = "POST",
                Url = "RegisterList",
                IsPostDataTypeArray = true,
                ActionTypeCd = "reg",
                RepositoryGroupId = repositoryGroupId
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterMethod(methodDef)).Assert(RegisterSuccessExpectStatusCode);


            // API作成直後はNotImplementedになる場合があるためX-Cache＆リトライ
            api.AddHeaders.Add(HeaderConst.X_Cache, "on");
            var retryPolicy = Policy.HandleResult<HttpResponseMessage>(r =>
            {
                return r.StatusCode == HttpStatusCode.NotImplemented;
            }).WaitAndRetry(9, i => TimeSpan.FromMilliseconds(2000));


            ////////////////////////////////////////////
            // NG: リソースモデルなし
            ////////////////////////////////////////////
            client.GetWebApiResponseResult(api.AdaptResourceSchema(), retryPolicy).Assert(BadRequestStatusCode).ContentString.Contains("E50409").IsTrue();

            // リソースモデル設定
            var modelId = resouceSchema.SchemaId;
            apiDef.ModelId = modelId;
            clientM.GetWebApiResponseResult(manageApi.RegisterApi(apiDef)).Assert(RegisterSuccessExpectStatusCode);


            ////////////////////////////////////////////
            // OK: リソースモデル適用(テーブル作成)
            ////////////////////////////////////////////
            client.GetWebApiResponseResult(api.AdaptResourceSchema()).Assert(HttpStatusCode.OK);

            // データ登録
            client.GetWebApiResponseResult(api.RegisterListAsString(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            // データ取得
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(testData.Data1GetExpected.ToJson());


            ////////////////////////////////////////////
            // OK: リソースモデル適用(変更なし)
            ////////////////////////////////////////////
            client.GetWebApiResponseResult(api.AdaptResourceSchema()).Assert(HttpStatusCode.OK);

            // データ取得
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(testData.Data1GetExpected.ToJson());


            ////////////////////////////////////////////
            // NG: AllowAdditionalProperties
            ////////////////////////////////////////////
            resouceSchema.JsonSchema = testData.JsonSchemaAllowAdditionalProperties;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(api.AdaptResourceSchema(), retryPolicy).Assert(BadRequestStatusCode).ContentString.Contains("E50410").IsTrue();


            ////////////////////////////////////////////
            // NG: 列削除
            ////////////////////////////////////////////
            resouceSchema.JsonSchema = testData.JsonSchemaDeleteColumn;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(api.AdaptResourceSchema(), retryPolicy).Assert(BadRequestStatusCode).ContentString.Contains("E50410").IsTrue();


            ////////////////////////////////////////////
            // NG: 列型変更
            ////////////////////////////////////////////
            resouceSchema.JsonSchema = testData.JsonSchemaModifyColumn;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(api.AdaptResourceSchema(), retryPolicy).Assert(BadRequestStatusCode).ContentString.Contains("E50410").IsTrue();


            ////////////////////////////////////////////
            // NG: 列名禁止文字
            ////////////////////////////////////////////
            resouceSchema.JsonSchema = testData.JsonSchemaInvalidColumnName;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(api.AdaptResourceSchema(), retryPolicy).Assert(BadRequestStatusCode).ContentString.Contains("E50410").IsTrue();


            ////////////////////////////////////////////
            // NG: 列名最大長超過
            ////////////////////////////////////////////
            resouceSchema.JsonSchema = testData.JsonSchemaInvalidColumnLength;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(api.AdaptResourceSchema(), retryPolicy).Assert(BadRequestStatusCode).ContentString.Contains("E50410").IsTrue();


            ////////////////////////////////////////////
            // OK: 列追加
            ////////////////////////////////////////////
            resouceSchema.JsonSchema = testData.JsonSchemaAddColumn;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(resouceSchema)).Assert(RegisterSuccessExpectStatusCode);

            client.GetWebApiResponseResult(api.AdaptResourceSchema()).Assert(HttpStatusCode.OK);

            // データ取得
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(testData.Data1GetExpectedAfterAlterTable.ToJson());

            // データ登録(追加項目あり)
            client.GetWebApiResponseResult(api.RegisterListAsString(testData.Data2)).Assert(RegisterSuccessExpectStatusCode);
            // データ取得(追加項目あり)
            client.GetWebApiResponseResult(api.OData("$orderby=STR_VALUE")).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson().Is(testData.Data2GetExpected.ToJson());
        }
    }
}
