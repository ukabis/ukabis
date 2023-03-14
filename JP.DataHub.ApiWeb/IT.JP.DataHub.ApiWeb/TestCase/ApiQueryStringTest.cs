using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ApiQueryStringTest : ApiWebItTestCase
    {
        #region TestData
        
        public (int code, string param1, string param2) Data1Props = (1, "value1", "value2");
        public (int code, string param1, string param2) Data2Props = (2, "value1", "value2-2");
        public (int code, string param1, string param2) Data3Props = (3, "value1-3", "value2");

        public AcceptDataModel Data1 = new AcceptDataModel()
        {
            Code = "AA",
            Name = "aaa"
        };

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void ApiQueryStringTest_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IQueryStringApi>();

            // 必須パラメータのみのAPIでデータ取得
            var response = client.GetWebApiResponseResult(api.GetByParameter($"?param1={Data1Props.param1}")).Assert(GetExpectStatusCodes).RawContentString;
            var jtoken = JToken.Parse(response);
            jtoken.Count().Is(2);
            jtoken[0]["code"].Is(Data1Props.code);
            jtoken[1]["code"].Is(Data2Props.code);

            // 任意パラメータのみのAPIでデータ取得(パラメータ未指定)
            response = client.GetWebApiResponseResult(api.GetByOptionParameter(null)).Assert(GetExpectStatusCodes).RawContentString;
            jtoken = JToken.Parse(response);
            jtoken.Count().Is(3);
            jtoken[0]["code"].Is(Data1Props.code);
            jtoken[1]["code"].Is(Data2Props.code);
            jtoken[2]["code"].Is(Data3Props.code);

            // 任意パラメータのみのAPIでデータ取得(パラメータ指定)
            response = client.GetWebApiResponseResult(api.GetByOptionParameter($"?param1={Data1Props.param1}")).Assert(GetExpectStatusCodes).RawContentString;
            jtoken = JToken.Parse(response);
            jtoken.Count().Is(2);
            jtoken[0]["code"].Is(Data1Props.code);
            jtoken[1]["code"].Is(Data2Props.code);

            // 必須・任意パラメータどちらも含むAPIでデータ取得(任意パラメータ未指定)
            response = client.GetWebApiResponseResult(api.GetByParameters($"?param1={Data1Props.param1}")).Assert(GetExpectStatusCodes).RawContentString;
            jtoken = JToken.Parse(response);
            jtoken.Count().Is(2);
            jtoken[0]["code"].Is(Data1Props.code);
            jtoken[1]["code"].Is(Data2Props.code);

            // 必須・任意パラメータどちらも含むAPIでデータ取得(任意パラメータ指定)
            response = client.GetWebApiResponseResult(api.GetByParameters($"?param1={Data1Props.param1}&param2={Data1Props.param2}")).Assert(GetExpectStatusCodes).RawContentString;
            jtoken = JToken.Parse(response);
            jtoken.Count().Is(1);
            jtoken[0]["code"].Is(Data1Props.code);
        }

        [TestMethod]
        public void ApiQueryStringTest_NotImplementedSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IQueryStringApi>();

            // 必須パラメータのみのAPIでデータ取得(必須パラメータ未指定でNotImplemented)
            client.GetWebApiResponseResult(api.GetByParameter(null)).Assert(NotImplementedExpectStatusCode);

            // 任意パラメータのみのAPIでデータ取得(未定義パラメータ指定でNotImplemented)
            client.GetWebApiResponseResult(api.GetByOptionParameter("?param3=value3")).Assert(NotImplementedExpectStatusCode);

            // 必須・任意パラメータどちらも含むAPIでデータ取得(必須パラメータ未指定でNotImplemented)
            client.GetWebApiResponseResult(api.GetByParameters("?param2=value2")).Assert(NotImplementedExpectStatusCode);
        }
    }
}
