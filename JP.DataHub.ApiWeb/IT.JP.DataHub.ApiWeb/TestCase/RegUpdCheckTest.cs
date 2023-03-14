using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class RegUpdCheckTest : ApiWebItTestCase
    {
        #region TestData

        private class RegUpdCheckTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~RegUpdCheck~1~AA"
            };

            public AreaUnitModel Data1Patch = new AreaUnitModel()
            {
                ConversionSquareMeters = 2
            };

            public RegUpdCheckTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void RegUpdCheck(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegUpdCheckApi>();
            var testData = new RegUpdCheckTestData(repository, api.ResourceUrl);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var reg1 = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).Result;
            reg1._Regdate.Is(reg1._Upddate);
            reg1._Reguser_Id.Is(reg1._Upduser_Id);

            System.Threading.Thread.Sleep(900);

            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            var reg2 = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).Result;
            reg1._Regdate.Is(reg2._Regdate);
            reg1._Reguser_Id.Is(reg2._Reguser_Id);
            reg2._Reguser_Id.Is(reg2._Upduser_Id);
            reg2._Regdate.IsNot(reg2._Upddate);
            Assert.IsTrue(reg2._Regdate.ToDateTime() < reg2._Upddate.ToDateTime());

            System.Threading.Thread.Sleep(900);

            client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);
            var upd = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).Result;
            reg2._Upddate.IsNot(upd._Upddate);
            Assert.IsTrue(reg2._Upddate.ToDateTime() < upd._Upddate.ToDateTime());
        }
    }
}
