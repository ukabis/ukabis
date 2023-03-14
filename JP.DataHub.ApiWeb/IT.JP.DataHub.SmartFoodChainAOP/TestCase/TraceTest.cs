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
using IT.JP.DataHub.SmartFoodChainAOP.TestCase;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class TraceTest : ItTestCaseBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void TraceRegister_NormalTest()
        {
            //Registerのシナリオ
            //01ITGtin21Product1というProductCodeにはパスワードが掛かっているので、そのままRegisterするとBadRequestになる
            //パスワード間違いでも、BadRequestになる
            //正しいパスワードをクエリストリングに設定するとRegisterは成功する
            //01ITGtin21Product2というProductCodeにはパスワードが掛かってないので、そのままRegisterで成功する
            //ただし、パスワードが掛かってないProductCodeに対して、クエリストリングでPasswordを設定して、
            //空値以外の値だった場合はBadRequestになる

            //データ準備
            var client = new IntegratedTestClient("test1");
            var client1 = new IntegratedTestClient("test2");
            var apiTraceMan = UnityCore.Resolve<ITraceManageApi>();
            var api = UnityCore.Resolve<ITraceApi>();

            //削除
            client.GetWebApiResponseResult(apiTraceMan.DeleteAll());
            client.GetWebApiResponseResult(api.DeleteAll());
            client1.GetWebApiResponseResult(apiTraceMan.DeleteAll());
            client1.GetWebApiResponseResult(api.DeleteAll());

            client1.GetWebApiResponseResult(apiTraceMan.Register(new TraceManageModel
            {
                ProductCode = "01ITGtin21Product1",
                Password = "AAA"
            })).Assert(RegisterSuccessExpectStatusCode);

            var trace = new TraceModel()
            {
                TraceDetailId = Guid.NewGuid().ToString(),
                ArrivalId = "hoge",
                Date = DateTime.UtcNow,
                OfficeId = "hogeOffice",
                OpenId = "hogeOpenId",
                TraceClass = TraceClassEnum.trc.ToString(),
                TraceTitle = "hogehoge",
                TraceMemo = "fugafuga",
                ProductCode = "01ITGtin21Product1",
                CompanyId = "hogeCompany"
            };

            //パスワードが設定されているのでBadRequest
            client.GetWebApiResponseResult(api.Register(trace)).AssertErrorCode(BadRequestStatusCode, "E104404");

            //パスワード間違いでもBadRequest
            client.GetWebApiResponseResult(api.Register(trace, "password=Hoge")).AssertErrorCode(BadRequestStatusCode, "E104404");

            //正しいパスワードをリクエスト <- 成功
            client.GetWebApiResponseResult(api.Register(trace, "password=AAA")).Assert(RegisterSuccessExpectStatusCode);

            //ProductCode 01ITGtin21Product2 を登録
            trace.ProductCode = "01ITGtin21Product2";
            trace.TraceDetailId = Guid.NewGuid().ToString();
            //パスワード掛かってないので成功する
            client.GetWebApiResponseResult(api.Register(trace)).Assert(RegisterSuccessExpectStatusCode);
            //パスワード空値でも成功する
            trace.TraceDetailId = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(api.Register(trace, "password=")).Assert(RegisterSuccessExpectStatusCode);
            //パスワードに値があると失敗する
            trace.TraceDetailId = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(api.Register(trace, "password=hoge")).AssertErrorCode(BadRequestStatusCode, "E104404");


            //RegisterListは、そのまま登録できないことだけ確認
            var traceList = new List<TraceModel>() { new TraceModel ()
            {
                TraceDetailId = Guid.NewGuid().ToString(),
                ArrivalId = "hoge",
                Date = DateTime.UtcNow,
                OfficeId = "hogeOffice",
                OpenId = "hogeOpenId",
                TraceClass = TraceClassEnum.trc.ToString(),
                TraceTitle = "hogehoge",
                TraceMemo = "fugafuga",
                ProductCode = "01ITGtin21Product1",
                CompanyId = "hogeCompany"
            }};

            //パスワードが設定されているのでBadRequest
            client.GetWebApiResponseResult(api.RegisterList(traceList)).AssertErrorCode(BadRequestStatusCode, "E104404");

        }

        [TestMethod]
        public void GetTraceByProductCodeTest()
        {
            //GetTraceByProductCodeのシナリオ
            //GetTraceByProductCodeは領域越え設定してあるので、Aというユーザーが01ITGtin21Product1 というProductCodeのデータを登録
            //BというユーザーがGetTraceByProductCodeで、データを取れることを確認する
            //複数件あっても全部返す仕様なので、そこも確認する

            var client = new IntegratedTestClient("test1"); 
            var client1 = new IntegratedTestClient("test2");
            var apiTraceMan = UnityCore.Resolve<ITraceManageApi>();
            var api = UnityCore.Resolve<ITraceApi>();

            //削除
            client.GetWebApiResponseResult(apiTraceMan.DeleteAll());
            client.GetWebApiResponseResult(api.DeleteAll());
            client1.GetWebApiResponseResult(apiTraceMan.DeleteAll());
            client1.GetWebApiResponseResult(api.DeleteAll());

            var trace = new TraceModel()
            {
                TraceDetailId = "Trace1",
                ArrivalId = "hoge",
                Date = DateTime.UtcNow,
                OfficeId = "hogeOffice",
                OpenId = "hogeOpenId",
                TraceClass = TraceClassEnum.trc.ToString(),
                TraceTitle = "hogehoge",
                TraceMemo = "fugafuga",
                ProductCode = "01ITGtin21Product1",
                CompanyId = "hogeCompany"
            };

            client.GetWebApiResponseResult(api.Register(trace)).Assert(RegisterSuccessExpectStatusCode);
            trace.TraceDetailId = "Trace3";
            client.GetWebApiResponseResult(api.Register(trace)).Assert(RegisterSuccessExpectStatusCode);
            trace.TraceDetailId = "Trace2";
            client.GetWebApiResponseResult(api.Register(trace)).Assert(RegisterSuccessExpectStatusCode);

            var result = client1.GetWebApiResponseResult<List<TraceModel>>(api.GetTraceByProductCode("01ITGtin21Product1")).Assert(GetSuccessExpectStatusCode);
            //全部取れていること
            result.Result.Count.Is(3);
            //念のためPKでソート
            var res = result.Result.OrderBy(x => x.TraceDetailId).ToList();
            //Trace1,2,3 が取得できていること
            res[0].TraceDetailId.Contains("Trace1").IsTrue();
            res[1].TraceDetailId.Contains("Trace2").IsTrue();
            res[2].TraceDetailId.Contains("Trace3").IsTrue();

        }
    }
}
