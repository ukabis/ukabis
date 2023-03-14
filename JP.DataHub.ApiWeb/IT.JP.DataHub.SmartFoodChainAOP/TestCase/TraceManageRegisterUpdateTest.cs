using System.Net;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using static System.Net.Mime.MediaTypeNames;
using JP.DataHub.UnitTest.Com.Extensions;
using Azure;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class TraceManageRegisterUpdateTest : ItTestCaseBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void TraceManageRegister_NormalTest()
        {
            //Registerのシナリオ
            //Aという人がAAというProductCodeにパスワードを設定
            //Bと言う人は、AAというProductCodeは新規に登録できない（ProductCodeはリソースでUQ） <- BadRequest
            // BBというProductCodeがリソースに未存在なら、登録できる
            //Aという人は、AAというProductCodeのパスワード変更のために、Registerは使えない(Updateを使う) <- BadRequest
            //あと細かな仕様で、パスワードはハッシュ化されるため、ODataで取ってもハッシュ値の返却となる
            //Passwordが必須項目でないため、Null,空値もOK。この場合Null/空もハッシュ化されて保存される

            var client1 = new IntegratedTestClient("test1"); //<- Aという人
            var client2 = new IntegratedTestClient("test3"); //<- Bという人
            var api = UnityCore.Resolve<ITraceManageApi>();

            //削除
            client1.GetWebApiResponseResult(api.DeleteAll());
            client2.GetWebApiResponseResult(api.DeleteAll());

            //AがAAを登録
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "AA",
                Password = "AAA"
            })).Assert(RegisterSuccessExpectStatusCode);

            //BがAAを登録 <- BadRequest
            client2.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "AA",
                Password = "AAA"
            })).AssertErrorCode(BadRequestStatusCode, "E104405");

            //BがBBを登録 <- OK
            client2.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "BB",
                Password = "BBB"
            })).Assert(RegisterSuccessExpectStatusCode);

            //AがAAを、パスワードCCCで登録 <- BadRequest
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "AA",
                Password = "CCC"
            })).AssertErrorCode(BadRequestStatusCode, "E104405");

            //AがBBを登録 <- BadRequest
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "BB",
                Password = "DDD"
            })).AssertErrorCode(BadRequestStatusCode, "E104405");

            //ODataで取って、パスワードがハッシュ値(というか、AAAでない)になってることを確認
            var response = client1.GetWebApiResponseResult(api.OData("$filter=ProductCode eq 'AA'")).Result;
            response[0].Password.IsNot("AAA");

            var passwordAAA = response[0].Password;

            //念のため確認。AがBBをODataで取っても取れない(Register時の処理ではODataOtherAccessible使うので見付かるが、通常は取れない)
            client1.GetWebApiResponseResult(api.OData("$filter=ProductCode eq 'BB'")).Assert(NotFoundStatusCode);

            //AがCC1/2/3を、パスワードnullで登録、ついでにRegisterListのチェック
            client1.GetWebApiResponseResult(api.RegisterList(new List<TraceManageModel>() {
                new TraceManageModel () { ProductCode = "CC1" },
                new TraceManageModel () { ProductCode = "CC2", Password = null },
                new TraceManageModel () { ProductCode = "CC3", Password = string.Empty }
            })).Assert(RegisterSuccessExpectStatusCode);

            //それぞれ取得し、パスワードがハッシュ値になっていること(null/空じゃないこと)を確認
            response = client1.GetWebApiResponseResult(api.OData("$filter=ProductCode eq 'CC1' or ProductCode eq 'CC2' or ProductCode eq 'CC3'")).Result;
            response.Where(x => string.IsNullOrEmpty(x.Password)).Count().Is(0);
            //Null/空は同じハッシュ値になるようなので、一致確認
            response.Where(x => x.Password == "cf83e1357eefb8bdf1542850d66d8007d620e4050b5715dc83f4a921d36ce9ce47d0d13c5d85f2b0ff8318d2877eec2f63b931bd47417a81a538327af927da3e").Count().Is(3);
        }

        [TestMethod]
        public void TraceManageRegisterUpdate_NormalTest()
        {
            //Updateのシナリオ
            //Aという人がAAというProductCodeにパスワードを設定
            //AAはRegisterだと変更できない(変更できないのはRegisterTestでやる)から、Updateする。
            //X-Passwordで現行の正しいパスワードを設定してUpdate <- 成功
            //X-Password未指定、X-Passwordのパスワードアンマッチ <- BadRequest
            //Bという人が、AAの正しいX-Passwordを指定 <- BadRequest
            //細かいところで、Passwordが必須項目でないため、Null,空値もOKなので、Null/空値での照合テストする

            var client1 = new IntegratedTestClient("test1"); //<- Aという人
            var client2 = new IntegratedTestClient("test3"); //<- Bという人
            var api = UnityCore.Resolve<ITraceManageApi>();

            //削除
            client1.GetWebApiResponseResult(api.DeleteAll());
            client2.GetWebApiResponseResult(api.DeleteAll());

            //AがAAを登録
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "AA",
                Password = "AAA"
            })).Assert(RegisterSuccessExpectStatusCode);

            //現パスワードのハッシュ値取得
            var pwd = client1.GetWebApiResponseResult(api.OData("$filter=ProductCode eq 'AA'")).Result.FirstOrDefault()?.Password;

            //AがAAのパスワードを変更 X-Password正しい <- 成功
            api.AddHeaders.Add("X-Password", "AAA");
            client1.GetWebApiResponseResult(api.Update("AA", new TraceManagePasswordModel { Password = "BBB"}))
                .Assert(UpdateSuccessExpectStatusCode);

            var newPwd = client1.GetWebApiResponseResult(api.OData("$filter=ProductCode eq 'AA'")).Result.FirstOrDefault()?.Password;

            //パスワードが変わってること、ハッシュ値なのでBBBじゃないこと
            newPwd.IsNot(pwd);
            newPwd.IsNot("BBB");

            //AがAAのパスワードを変更(X-Password間違い) <- BadRequest
            api.AddHeaders.Remove("X-Password");
            api.AddHeaders.Add("X-Password", "hoge");
            client1.GetWebApiResponseResult(api.Update("AA", new TraceManagePasswordModel { Password = "BBB" }))
                .AssertErrorCode(BadRequestStatusCode, "E104404");

            //ProductCode指定が無い <- BadRequest
            client2.GetWebApiResponseResult(api.Update(null, new TraceManagePasswordModel { Password = "BBB" }))
                .AssertErrorCode(BadRequestStatusCode, "E104406");

            //X-Password未指定 <- BadRequest
            api.AddHeaders.Remove("X-Password");
            client1.GetWebApiResponseResult(api.Update("AA", new TraceManagePasswordModel { Password = "BBB" }))
                .AssertErrorCode(BadRequestStatusCode, "E104407");

            //BがAAの正しいパスワードを使って更新 <- BadRequest
            api.AddHeaders.Remove("X-Password");
            api.AddHeaders.Add("X-Password", "BBB");
            client2.GetWebApiResponseResult(api.Update("AA", new TraceManagePasswordModel { Password = "BBB" }))
                .AssertErrorCode(BadRequestStatusCode, "E104408");

            //そもそもProductCodeが存在しない <- NotFound
            client2.GetWebApiResponseResult(api.Update("hogehoge", new TraceManagePasswordModel { Password = "BBB" }))
                .AssertErrorCode(NotFoundStatusCode, "E104409");

            //Null/空値の確認
            //AがCC1/2を、パスワードnullで登録
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "CC1"
            })).Assert(RegisterSuccessExpectStatusCode);
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "CC2",
                Password = null
            })).Assert(RegisterSuccessExpectStatusCode);

            //AがCC3を、パスワード空値で登録
            client1.GetWebApiResponseResult(api.Register(new TraceManageModel
            {
                ProductCode = "CC3",
                Password = string.Empty
            })).Assert(RegisterSuccessExpectStatusCode);

            //空値で照合(nullはAddHeaderでエラーになるのでやらない)
            api.AddHeaders.Remove("X-Password");
            api.AddHeaders.Add("X-Password", string.Empty);
            client1.GetWebApiResponseResult(api.Update("CC1", new TraceManagePasswordModel { Password = "BBB" }))
                .Assert(UpdateSuccessExpectStatusCode);

            client1.GetWebApiResponseResult(api.Update("CC2", new TraceManagePasswordModel { Password = "BBB" }))
                .Assert(UpdateSuccessExpectStatusCode);

            client1.GetWebApiResponseResult(api.Update("CC3", new TraceManagePasswordModel { Password = "BBB" }))
                .Assert(UpdateSuccessExpectStatusCode);

            var response = client1.GetWebApiResponseResult(api.OData("$filter=ProductCode eq 'CC1' or ProductCode eq 'CC2' or ProductCode eq 'CC3'")).Result;

            //事前に取ったパスワード BBBのハッシュ値と同じこと
            response.Where(x => x.Password == newPwd).Count().Is(3);
        }
    }
}
