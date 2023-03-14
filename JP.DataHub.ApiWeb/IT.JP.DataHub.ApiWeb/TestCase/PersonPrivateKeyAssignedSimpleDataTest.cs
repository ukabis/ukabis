using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// 個人領域（個人依存）でのテストケース
    /// </summary>
    [TestClass]
    public class PersonPrivateKeyAssignedSimpleDataTest : ApiWebItTestCase
    {
        #region TestData

        private class PersonPrivateKeyAssignedSimpleDataTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~PersonPrivate~KeyAssignedSimpleData~1~AA"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                id = "API~IntegratedTest~PersonPrivate~KeyAssignedSimpleData~1~AA",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public PersonPrivateKeyAssignedSimpleDataTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, false, true, client) { }
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
        public void PersonPrivate_CheckDependency(Repository repository)
        {
            // ２人のユーザーが異なるベンダーでアクセスをしている。
            // PersonPrivateKeyAssignedSimpleDataApiは、個人リソース（ベンダーリソースではない）ため、ベンダーを跨いでも個人が同一ならデータが取得できる
            // また、人が違うとデータは取得できない。
            // それを確認する

            var clientA = new IntegratedTestClient("test1", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientC = new IntegratedTestClient("test1", "SmartFoodChainPortal") { TargetRepository = repository };

            var api = UnityCore.Resolve<IPersonPrivateKeyAssignedSimpleDataApi>();
            var testData = new PersonPrivateKeyAssignedSimpleDataTestData(repository, api.ResourceUrl, clientA);

            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });
            clientB.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
            clientC.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void PersonPrivate_CheckPartitionKey(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IPersonPrivateKeyAssignedSimpleDataApi>();
            var testData = new PersonPrivateKeyAssignedSimpleDataTestData(repository, api.ResourceUrl, client);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            if (!IsIgnoreGetInternalAllField)
            {
                // 個人依存のデータが、_Typeと_partitionKeyが正しいか確認（_Typeは依存/非依存では変化しないけど便宜上）
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                var data = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).Result;
                var ownerid = data._Owner_Id;
                // SQLServerは_Type,_partitionkeyなしのため割愛
                if (repository != Repository.SqlServer)
                {
                    data._Type.Is($"{testData.PartitionKeyRoot}~PersonPrivate~KeyAssignedSimpleData");
                    data._partitionkey.Is($"{testData.PartitionKeyRoot}~PersonPrivate~KeyAssignedSimpleData~{ownerid}~1");
                }

                // レスポンスモデルがadditionalProperties=falseの場合
                data = client.GetWebApiResponseResult(api.GetWithAdditionalPropertiesFalse(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode).Result;
                ownerid = data._Owner_Id;
                // SQLServerは_Type,_partitionkeyなしのため割愛
                if (repository != Repository.SqlServer)
                {
                    data._Type.Is($"{testData.PartitionKeyRoot}~PersonPrivate~KeyAssignedSimpleData");
                    data._partitionkey.Is($"{testData.PartitionKeyRoot}~PersonPrivate~KeyAssignedSimpleData~{ownerid}~1");
                }
                data._Vendor_Id.IsNull();
                data._System_Id.IsNull();
            }
        }

        //ODataDeleteテスト
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void PersonPrivate_CheckDependency_ODataDelete(Repository repository)
        {
            var api = UnityCore.Resolve<IPersonPrivateKeyAssignedSimpleDataApi>();
            var clientA = new IntegratedTestClient("test1") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2") { TargetRepository = repository };
            var testDataA = new PersonPrivateKeyAssignedSimpleDataTestData(repository, api.ResourceUrl, clientA);
            var testDataB = new PersonPrivateKeyAssignedSimpleDataTestData(repository, api.ResourceUrl, clientB);

            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // ユーザーAとユーザーBに、それぞれデータ登録
            clientA.GetWebApiResponseResult(api.Regist(testDataA.Data1)).Assert(RegisterSuccessExpectStatusCode, testDataA.Data1RegistExpected);
            clientB.GetWebApiResponseResult(api.Regist(testDataB.Data1)).Assert(RegisterSuccessExpectStatusCode, testDataB.Data1RegistExpected);

            // ユーザーAのデータ削除
            clientA.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);
            // ユーザーBのデータは消えていないこと
            clientB.GetWebApiResponseResult(api.Get(testDataB.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testDataB.Data1Get);
        }

    }
}
