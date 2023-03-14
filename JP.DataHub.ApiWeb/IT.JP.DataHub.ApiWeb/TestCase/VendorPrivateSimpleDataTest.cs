using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class VendorPrivateSimpleDataTest : ApiWebItTestCase
    {
        #region TestData

        private class VendorPrivateSimpleDataTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public VendorPrivateSimpleDataTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl, true) { }
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
        public void VendorPrivate_CheckDependency(Repository repository)
        {
            // 同一ユーザーが異なるベンダーでアクセスをしている。
            // VendorPrivateSimpleDataApiは、ベンダー境界（依存）ため、同一人物が異なるベンダーを通してアクセスしても、同じデータが見られない。
            // また、人が違うとデータは取得できない。
            // それを確認する

            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IVendorPrivateSimpleDataApi>();
            var testData = new VendorPrivateSimpleDataTestData(repository, api.ResourceUrl);

            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void VendorPrivate_CheckPartitionKey(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IVendorPrivateSimpleDataApi>();
            var testData = new VendorPrivateSimpleDataTestData(repository, api.ResourceUrl);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            if (!IsIgnoreGetInternalAllField)
            {
                // ベンダー依存のデータが、_Typeと_partitionKeyが正しいか確認（_Typeは依存/非依存では変化しないけど便宜上）
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                var json = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
                var vendorid = json["_Vendor_Id"].ToString();
                var systemid = json["_System_Id"].ToString();
                // SQLServerは_Type,_partitionkeyなしのため割愛
                if (repository != Repository.SqlServer)
                {
                    json["_Type"].ToString().Is($"{testData.PartitionKeyRoot}~VendorPrivate~SimpleData");
                    json["_partitionkey"].ToString().Is($"{testData.PartitionKeyRoot}~VendorPrivate~SimpleData~{vendorid}~{systemid}~1");
                }

                // レスポンスモデルがadditionalProperties=falseの場合
                json = client.GetWebApiResponseResult(api.GetWithAdditionalPropertiesFalse(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
                vendorid = json["_Vendor_Id"].ToString();
                systemid = json["_System_Id"].ToString();
                // SQLServerは_Type,_partitionkeyなしのため割愛
                if (repository != Repository.SqlServer)
                {
                    json["_Type"].ToString().Is($"{testData.PartitionKeyRoot}~VendorPrivate~SimpleData");
                    json["_partitionkey"].ToString().Is($"{testData.PartitionKeyRoot}~VendorPrivate~SimpleData~{vendorid}~{systemid}~1");
                }
            }
        }

        // ODataDeleteテスト
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void VendorPrivate_CheckDependency_ODataDelete(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IVendorPrivateSimpleDataApi>();
            var testData = new VendorPrivateSimpleDataTestData(repository, api.ResourceUrl);

            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // それぞれにデータ登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // ベンダーAの方のデータ削除
            clientA.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);

            // ベンダーBの方のデータは存在することを確認
            clientB.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode);
        }
    }
}
