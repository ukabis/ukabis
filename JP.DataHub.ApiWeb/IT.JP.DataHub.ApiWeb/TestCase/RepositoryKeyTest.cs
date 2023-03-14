using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class RepositoryKeyTest : ApiWebItTestCase
    {
        #region TestData

        private class RepositoryKey1TestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public RegisterResponseModel DataRegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~RepositoryKey1~{WILDCARD}"
            };

            public List<AreaUnitModel> Data3 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RepositoryKey1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = WILDCARD,
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RepositoryKey1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = WILDCARD,
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                }
            };
        }

        private class RepositoryKey2TestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data3 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public RegisterResponseModel DataRegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~RepositoryKey2~{WILDCARD}"
            };

            public List<AreaUnitModel> Data4 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RepositoryKey2~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = WILDCARD,
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                }
            };
        }

        private class RepositoryKeyNoSchemaTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };

            public RegisterResponseModel DataRegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~RepositoryKeyNoSchema~{WILDCARD}"
            };

            public List<AreaUnitModel> Data3 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RepositoryKeyNoSchema~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = WILDCARD,
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RepositoryKeyNoSchema~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = WILDCARD,
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                }
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// リポジトリキー項目が1つ
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void RepositoryKeyTest_Key1Scenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRepositoryKey1Api>();
            var testData = new RepositoryKey1TestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // Key項目がないデータは登録できない
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(BadRequestStatusCode);

            // Key項目があるデータは登録できる
            client.GetWebApiResponseResult(api.Register(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected);

            // ID自動割り振りのAPIを使用すると、Key項目がないデータも登録できる
            client.GetWebApiResponseResult(api.RegisterAutoKey(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected);

            // 正常に登録できてることを確認
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data3);
        }

        /// <summary>
        /// リポジトリキー項目が2つ
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void RepositoryKeyTest_Key2Scenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRepositoryKey2Api>();
            var testData = new RepositoryKey2TestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // Key項目がないデータは登録できない
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(BadRequestStatusCode);

            // Key項目が複数の場合、1つが足りないと登録できない
            client.GetWebApiResponseResult(api.Register(testData.Data2)).Assert(BadRequestStatusCode);

            // Key項目があるデータは登録できる
            client.GetWebApiResponseResult(api.Register(testData.Data3)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected);

            // 正常に登録できてることを確認
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data4);
        }

        /// <summary>
        /// スキーマなし
        /// （SQLServerはスキーマがないと作れないのでテスト対象外）
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RepositoryKeyTest_NoSchemaScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRepositoryKeyNoSchemaApi>();
            var testData = new RepositoryKeyNoSchemaTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // Key項目がないデータは登録できない
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(BadRequestStatusCode);

            // Key項目があるデータは登録できる
            client.GetWebApiResponseResult(api.Register(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected);

            // ID自動割り振りのAPIを使用すると、Key項目がないデータも登録できる
            client.GetWebApiResponseResult(api.RegisterAutoKey(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected);

            // 正常に登録できてることを確認
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data3);
        }
    }
}