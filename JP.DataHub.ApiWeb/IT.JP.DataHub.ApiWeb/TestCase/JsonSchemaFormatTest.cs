using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class JsonSchemaFormatTest : ApiWebItTestCase
    {
        #region TestData

        private class JsonSchemaFormatForeignKeyData : TestDataBase
        {
            public JsonSchemaFormatModel Data1 = new JsonSchemaFormatModel()
            {
                Date = "2020-01-01"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~JsonSchemaFormatForeignKey~1~{WILDCARD}"
            };
            public JsonSchemaFormatModel Data1Get = new JsonSchemaFormatModel()
            {
                Date = "2020-01-01",
                id = "API~IntegratedTest~JsonSchemaFormatForeignKey~1~2020-01-01",
                _Owner_Id = WILDCARD
            };

            public JsonSchemaFormatModel Data2 = new JsonSchemaFormatModel()
            {
                Date = "2020-01-32"
            };
            public RegisterResponseModel Data2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~JsonSchemaFormatForeignKey~1~{WILDCARD}"
            };
            public JsonSchemaFormatModel Data2Get = new JsonSchemaFormatModel()
            {
                Date = "2020-01-32",
                id = "API~IntegratedTest~JsonSchemaFormatForeignKey~2~2020-01-32",
                _Owner_Id = WILDCARD
            };

            public JsonSchemaFormatModel Data3 = new JsonSchemaFormatModel()
            {
                Date = "2020-01-01a"
            };
            public RegisterResponseModel Data3RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~JsonSchemaFormatForeignKey~1~{WILDCARD}"
            };
            public JsonSchemaFormatModel Data3Get = new JsonSchemaFormatModel()
            {
                Date = "2020-01-01a",
                id = "API~IntegratedTest~JsonSchemaFormatForeignKey~2~2020-01-01a",
                _Owner_Id = WILDCARD
            };

            public JsonSchemaFormatForeignKeyData(string resourceUrl) : base(Repository.Default, resourceUrl, true) { }
        }

        private class JsonSchemaFormatTestData : TestDataBase
        {
            public JsonSchemaFormatModel Data1 = new JsonSchemaFormatModel()
            {
                TestId = "1234567",
                Date = "2020-01-01"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~JsonSchemaFormat~1~{WILDCARD}"
            };
            public JsonSchemaFormatModel Data1Get = new JsonSchemaFormatModel()
            {
                TestId = "1234567",
                Date = "2020-01-01",
                id = $"API~IntegratedTest~JsonSchemaFormat~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public JsonSchemaFormatModel Data2 = new JsonSchemaFormatModel()
            {
                TestId = "ABCDEFG",
                Date = "2020-01-32"
            };

            public JsonSchemaFormatModel Data3 = new JsonSchemaFormatModel()
            {
                TestId = "HIJKLMN",
                Date = "2021-01-01"
            };

            public JsonSchemaFormatModel Data4 = new JsonSchemaFormatModel()
            {
                TestId = "OPQRSTU",
                Date = "2020-01-01a"
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /*
以下、テスト用のAPI定義

/API/IntegratedTest/JsonSchemaFormatForeignKey
{code}
{
    'description':'IntegratedTestJsonSchemaFormatForeignKey',
    'properties': {
        'Date': {
            'title': '日付',
            'type': 'string',
            'required':true
        }
    },
    'type': 'object'
}
{/code}

/API/IntegratedTest/JsonSchemaFormat
{code}
{
    'description':'IntegretedTestJsonSchemaFormat',
    'properties': {
        'TestId': {
            'title': 'テストID',
            'type': 'string',
            'required':true
        },
        'Date': {
            'title': 'Date',
            'type': 'string',
            'required':true,
            'minLength': 0,
            'maxLength': 10,
            'format':'date;ForeignKey /API/IntegratedTest/JsonSchemaFormatForeignKey/Exists/{value};xxx;yyy'
        }
    },
    'type': 'object',
    'additionalProperties' : false
}
{code}
         */

        /**
         * JsonSchemaのformatに、JsonSchema規定要素と基盤独自要素の双方を含めてもValidationが効くことを確認する。
         * →「'format':'date;ForeignKey /API/IntegratedTest/DateFormat/Exists/{value}'」のような指定。
         */
        [TestMethod]
        public void JsonSchemaFormatTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);

            // ForeignKey用のデータ
            var fkApi = UnityCore.Resolve<IJsonSchemaFormatForeignKeyApi>();
            var fkData = new JsonSchemaFormatForeignKeyData(fkApi.ResourceUrl);
            client.GetWebApiResponseResult(fkApi.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(fkApi.Register(fkData.Data1)).Assert(RegisterSuccessExpectStatusCode, fkData.Data1RegistExpected);
            client.GetWebApiResponseResult(fkApi.Register(fkData.Data2)).Assert(RegisterSuccessExpectStatusCode, fkData.Data2RegistExpected);
            client.GetWebApiResponseResult(fkApi.Register(fkData.Data3)).Assert(RegisterSuccessExpectStatusCode, fkData.Data3RegistExpected);
            client.GetWebApiResponseResult(fkApi.Exists(fkData.Data1.Date)).Assert(ExistsSuccessExpectStatusCode);
            client.GetWebApiResponseResult(fkApi.Exists(fkData.Data2.Date)).Assert(ExistsSuccessExpectStatusCode);
            client.GetWebApiResponseResult(fkApi.Exists(fkData.Data3.Date)).Assert(ExistsSuccessExpectStatusCode);
            client.GetWebApiResponseResult(fkApi.Exists(fkData.Data1.Date + "ABC")).Assert(ExistsErrorExpectStatusCode);


            var api = UnityCore.Resolve<IJsonSchemaFormatApi>();
            var testData = new JsonSchemaFormatTestData();
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            // 正常登録
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            // dateでエラー
            client.GetWebApiResponseResult(api.Register(testData.Data2)).Assert(RegisterErrorExpectStatusCode);
            // ForeignKeyでエラー
            var response3 = client.GetWebApiResponseResult(api.Register(testData.Data3)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402").ContentString;
            response3.Contains("There are no results from").Is(true);
            // maxLengthでエラー
            client.GetWebApiResponseResult(api.Register(testData.Data4)).Assert(RegisterErrorExpectStatusCode);

            client.GetWebApiResponseResult(fkApi.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // ForeignKey側のデータを削除したため、ForeignKeyエラー
            var responseEnd = client.GetWebApiResponseResult(api.Register(testData.Data1)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402").ContentString;
            responseEnd.Contains("There are no results from").Is(true);
        }
    }
}