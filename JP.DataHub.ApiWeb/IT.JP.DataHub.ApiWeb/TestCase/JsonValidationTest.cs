using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class JsonValidationTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void Number6_0()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // バリデーションしないプロパティ：文字列
            client.GetWebApiResponseResult(api.RegistAsString("{ 'Num6dot0' : 123456 }")).Assert(RegisterSuccessExpectStatusCode);

            // バリデーションしないプロパティ：数値
            client.GetWebApiResponseResult(api.RegistAsString("{ 'Num6dot0' : 12345.1 }")).Assert(BadRequestStatusCode);

            // バリデーションしないプロパティ：数値
            client.GetWebApiResponseResult(api.RegistAsString("{ 'Num6dot0' : 123.123 }")).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void Num7_3()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // バリデーションしないプロパティ：文字列
            client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 1234567 }")).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 1234.567 }")).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 123.4567 }")).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void JsonValidationError_Regist_Simple_ResultCheck()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // BadRequest
            var response = client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 'hoge' }")).AssertErrorCode(BadRequestStatusCode, "E10402");
            // ContentType は、application/problem+json
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+json");
            // メッセージチェック
            var msg = response.RawContentString.ToJson();
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["Num7dot3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");
        }

        [TestMethod]
        public void JsonValidationError_Regist_Simple_ResultCheck_XML()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // BadRequest
            api.AddHeaders.Add(HeaderConst.Accept, "application/xml");
            var response = client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 'hoge' }")).Assert(BadRequestStatusCode);
            // ContentType は、application/problem+xml
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+xml");
            // メッセージチェック
            var msg = response.RawContentString.StringToXml();
            msg.Element("error_code").Value.Is("E10402");
            var chk = msg.Element("errors").Elements("Num7dot3").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Invalid type. Expected Number but got String.(code:18)");
        }


        [TestMethod]
        public void JsonValidationError_RegistList_ResultCheck_FirstIsInvalid()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // BadRequest
            var response = client.GetWebApiResponseResult(api.RegisterListAsString("[{ 'Num7dot3' : 'hoge1' }, { 'Num7dot3' : 222 },{ 'Num7dot3' : 333 }]")).AssertErrorCode(BadRequestStatusCode, "E10402");
            // ContentType は、application/problem+json
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+json");
            // メッセージチェック
            var msg = response.RawContentString.ToJson();
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["Num7dot3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");
        }


        [TestMethod]
        public void JsonValidationError_RegistList_ResultCheck_SecondIsInvalid()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // BadRequest
            var response = client.GetWebApiResponseResult(api.RegisterListAsString("[{ 'Num7dot3' : 111 }, { 'Num7dot3' : 'hoge2' },{ 'Num7dot3' : 333 }]")).AssertErrorCode(BadRequestStatusCode, "E10402");
            // ContentType は、application/problem+json
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+json");
            // メッセージチェック
            var msg = response.RawContentString.ToJson();
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["Num7dot3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");
        }


        [TestMethod]
        public void JsonValidationError_RegistList_ResultCheck_ThirdIsInvalid()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // BadRequest
            var response = client.GetWebApiResponseResult(api.RegisterListAsString("[{ 'Num7dot3' : 111 }, { 'Num7dot3' : 222 },{ 'Num7dot3' : 'hoge3' }]")).AssertErrorCode(BadRequestStatusCode, "E10402");
            // ContentType は、application/problem+json
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+json");
            // メッセージチェック
            var msg = response.RawContentString.ToJson();
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["Num7dot3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");
        }

        [TestMethod]
        public void JsonValidationError_RegistList_ResultCheck_AllInvalid()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // BadRequest
            var response = client.GetWebApiResponseResult(api.RegisterListAsString("[{ 'Num7dot3' : 'hoge1' }, { 'Num7dot3' : 'hoge2' },{ 'Num7dot3' : 'hoge3' }]")).Assert(BadRequestStatusCode);
            // ContentType は、application/problem+json
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+json");

            var msg = response.RawContentString.ToJson();
            msg.Type.Is(JTokenType.Array);
            msg.Count().Is(3);
            // それぞれチェック

            msg[0]["error_code"].ToString().Is("E10402");
            // メッセージチェック
            var messages = msg[0]["errors"];
            messages.Count().Is(1);
            messages["Num7dot3"].Children().Count().Is(1);
            messages["Num7dot3"][0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");

            messages = msg[1]["errors"];
            messages.Count().Is(1);
            messages["Num7dot3"].Children().Count().Is(1);
            messages["Num7dot3"][0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");

            messages = msg[2]["errors"];
            messages.Count().Is(1);
            messages["Num7dot3"].Children().Count().Is(1);
            messages["Num7dot3"][0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");
        }

        [TestMethod]
        public void JsonValidationError_Update_ResultCheck()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // 1件登録
            var id = client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 111 }")).Assert(RegisterSuccessExpectStatusCode).Result.id;

            // Updateエラーを発生させる
            var response = client.GetWebApiResponseResult(api.UpdateById(id , "{ 'Num7dot3' : 'hoge1' }")).AssertErrorCode(BadRequestStatusCode, "E10402");
            // ContentType は、application/problem+json
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+json");
            // メッセージチェック
            var msg = response.RawContentString.ToJson();
            var messages = msg["errors"].ToList();
            messages.Count.Is(1);
            var chk = msg["errors"]["Num7dot3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Number but got String.(code:18)");
        }

        [TestMethod]
        public void JsonValidationError_Update_ResultCheck_XML()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IJsonValidationApi>();

            // 1件登録
            var id = client.GetWebApiResponseResult(api.RegistAsString("{ 'Num7dot3' : 111 }")).Assert(RegisterSuccessExpectStatusCode).Result.id;

            // Updateエラーを発生させる
            api.AddHeaders.Add(HeaderConst.Accept, "application/xml");
            var response = client.GetWebApiResponseResult(api.UpdateById(id, "{ 'Num7dot3' : 'hoge1' }")).Assert(BadRequestStatusCode);
            // ContentType は、application/problem+xml
            response.RawContent.Headers.ContentType.MediaType.Is("application/problem+xml");
            var msg = response.RawContentString.StringToXml();
            msg.Element("error_code").Value.Is("E10402");
            var chk = msg.Element("errors").Elements("Num7dot3").ToList();
            chk.Count.Is(1);
            chk[0].Value.Is("Invalid type. Expected Number but got String.(code:18)");
        }
    }
}
