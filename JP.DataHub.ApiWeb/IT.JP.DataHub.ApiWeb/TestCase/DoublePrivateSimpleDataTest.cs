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
    public class DoublePrivateSimpleDataTest : ApiWebItTestCase
    {
        #region TestData

        private class DoublePrivateSimpleDataTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~DoublePrivate~SimpleData~1~{WILDCARD}"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~DoublePrivate~SimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };


            public DoublePrivateSimpleDataTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl, true, true) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void DoublePrivate_CheckDependency()
        {
            //// 同一ユーザーが異なるベンダーでアクセスをしている。
            //// VendorPrivateSimpleDataApiは、ベンダー境界（依存）ため、同一人物が異なるベンダーを通してアクセスしても、同じデータが見られない。
            //// また、人が違うとデータは取得できない。
            //// それを確認する

            //var contexta = new webapicontext(testcontext, $"{server}.smartfoodchainadmintoken", "masas");
            //var contextb = new webapicontext(testcontext, $"{server}.smartfoodchainportaltoken", "masas");
            //var contextc = new webapicontext(testcontext, $"{server}.smartfoodchainadmintoken", "hoge");
            //var contextd = new webapicontext(testcontext, $"{server}.smartfoodchainportaltoken", "hoge");

            //var resourcea = new vendorprivatesimpledataapi(contexta);
            //var resourceb = new vendorprivatesimpledataapi(contextb);
            //var resourcec = new vendorprivatesimpledataapi(contextc);
            //var resourced = new vendorprivatesimpledataapi(contextd);

            //var api = resourcea.deleteall();
            //contexta.actionandassert(api.request, resourcea.deleteexpectstatuscodes);
            //api = resourceb.deleteall();
            //contextb.actionandassert(api.request, resourceb.deleteexpectstatuscodes);
            //api = resourceb.deleteall();
            //contextc.actionandassert(api.request, resourcec.deleteexpectstatuscodes);
            //api = resourceb.deleteall();
            //contextd.actionandassert(api.request, resourced.deleteexpectstatuscodes);

            //api = resourcea.regist(resourcea.data1);
            //var rega = context.actionandassert(api.request, resourcea.registsuccessexpectstatuscode);

            //var hogedata = resourcec.data1.tojson();
            //hogedata.removefield("conversionsquaremeters");
            //hogedata.addfield("conversionsquaremeters", 10);
            //api = resourcec.regist(hogedata);
            //var regc = context.actionandassert(api.request, resourcec.registsuccessexpectstatuscode);

            //api = resourcea.odata();
            //var all = context.actionandassert(api.request, resourcea.getsuccessexpectstatuscode).tojson();

            //api = resourcea.odata();
            //var resulta = context.actionandassert(api.request, resourcea.getsuccessexpectstatuscode).tojson();
            //resulta.first["conversionsquaremeters"].is (1);
            //api = resourceb.odata();
            //context.actionandassert(api.request, resourceb.geterrorexpectstatuscode);
            //api = resourcec.odata();
            //var resultc = context.actionandassert(api.request, resourcec.getsuccessexpectstatuscode).tojson();
            //resultc.first["conversionsquaremeters"].is (10);
            //api = resourced.odata();
            //context.actionandassert(api.request, resourced.geterrorexpectstatuscode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DoublePrivate_CheckPartitionKey(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDoublePrivateSimpleDataApi>();
            var testData = new DoublePrivateSimpleDataTestData(repository, api.ResourceUrl);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            if (!IsIgnoreGetInternalAllField)
            {
                // ベンダー依存のデータが、_Typeと_partitionKeyが正しいか確認（_Typeは依存/非依存では変化しないけど便宜上）
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                var result = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).Result;
                var ownerid = result._Owner_Id;
                var vendorid = result._Vendor_Id;
                var systemid = result._System_Id;

                result.id.Is($"{testData.PartitionKeyRoot}~DoublePrivate~SimpleData~{ownerid}~{vendorid}~{systemid}~1~AA");

                // SQLServerは_Type,_partitionkeyなしのため割愛
                if (repository != Repository.SqlServer)
                {
                    result._Type.Is($"{testData.PartitionKeyRoot}~DoublePrivate~SimpleData");
                    result._partitionkey.Is($"{testData.PartitionKeyRoot}~DoublePrivate~SimpleData~{vendorid}~{systemid}~{ownerid}~1");
                }
            }
        }
    }
}
