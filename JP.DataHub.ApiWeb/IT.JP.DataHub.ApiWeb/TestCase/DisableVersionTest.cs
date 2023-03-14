using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class DisableVersionTest : ApiWebItTestCase
    {
        #region TestData

        private class DisableVersionTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~VersionDisable";

            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~1~AA"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = "API~IntegratedTest~VersionDisable~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1Patch = new AreaUnitModel()
            {
                id = "hogehoge2",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2
            };
            public AreaUnitModel Data1Patched = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2,
                id = "API~IntegratedTest~VersionDisable~1~AA",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data2 = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10
            };
            public RegisterResponseModel Data2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~1~BB"
            };
            public AreaUnitModel Data2Get = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10,
                id = "API~IntegratedTest~VersionDisable~1~BB",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data3 = new AreaUnitModel()
            {
                AreaUnitCode = "CC",
                AreaUnitName = "ccc",
                ConversionSquareMeters = 100
            };
            public RegisterResponseModel Data3RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~1~CC"
            };
            public AreaUnitModel Data3Get = new AreaUnitModel()
            {
                AreaUnitCode = "CC",
                AreaUnitName = "ccc",
                ConversionSquareMeters = 100,
                id = "API~IntegratedTest~VersionDisable~1~CC",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data4 = new AreaUnitModel()
            {
                AreaUnitCode = "DD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 1000
            };
            public RegisterResponseModel Data4RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~1~DD"
            };
            public AreaUnitModel Data4Get = new AreaUnitModel()
            {
                AreaUnitCode = "DD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 1000,
                id = "API~IntegratedTest~VersionDisable~1~DD",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data5 = new AreaUnitModel()
            {
                AreaUnitCode = "EE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 10000
            };
            public RegisterResponseModel Data5RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~1~EE"
            };
            public AreaUnitModel Data5Get = new AreaUnitModel()
            {
                AreaUnitCode = "EE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 10000,
                id = "API~IntegratedTest~VersionDisable~1~EE",
                _Owner_Id = WILDCARD
            };


            public DisableVersionTestData(Repository repository, string resourceUrl, bool isVendor = false, bool isPerson = false) : base(repository, resourceUrl, isVendor, isPerson) { }
        }

        private class DisableVersionVendorPrivateTestData : TestDataBase
        {
            public AreaUnitModel DataVendorOriginal = new AreaUnitModel()
            {
                AreaUnitCode = "XX",
                AreaUnitName = "xxx",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel DataVendorOriginalRegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~Vendor~1~XX"
            };

            public DisableVersionVendorPrivateTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl, true) { }
        }

        private class DisableVersionPersonPrivateTestData : TestDataBase
        {
            public AreaUnitModel DataPersonOriginal = new AreaUnitModel()
            {
                AreaUnitCode = "XX",
                AreaUnitName = "xxx",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel DataPersonOriginalRegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~VersionDisable~Person~1~XX"
            };

            public DisableVersionPersonPrivateTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl, false, true) { }
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
        public void DisableVersion_NormalScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDisableVersionApi>();
            var testData = new DisableVersionTestData(repository, api.ResourceUrl);

            // リソースバージョン関連の透過APIはすべてNotImplement
            var transparentApis = new List<WebApiRequestModel>()
            {
                api.GetCurrentVersion(),
                api.GetVersionInfo(),
                api.CompleteRegisterVersion(),
                api.CreateRegisterVersion(),
                api.GetRegisterVersion(),
                api.SetNewVersion()
            };
            transparentApis.ForEach(x => client.GetWebApiResponseResult(x).Assert(NotImplementedExpectStatusCode));

            // クリーンアップ
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // レコードを登録
            var regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // レコードを取得して確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // レコードを上書きで登録
            testData.Data1.AreaUnitName = "aaa_new";
            regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // レコードを取得して確認
            testData.Data1Get.AreaUnitName = "aaa_new";
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // レコードを更新
            client.GetWebApiResponseResult(api.Update("AA", testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);

            // レコードを取得して確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Patched);

            // 全データ登録
            var regData = new List<AreaUnitModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            var regExpected = new List<RegisterResponseModel>() { testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected };
            var regListResponse = client.GetWebApiResponseResult(api.RegisterList(regData)).Assert(RegisterSuccessExpectStatusCode);
            JsonConvert.DeserializeObject<List<RegisterResponseModel>>(regListResponse.RawContentString).OrderBy(x => x.id).ToList().IsStructuralEqual(regExpected);

            // 全件取得して確認
            var getExpected = new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get };
            var array = client.GetWebApiResponseResult(api.GetList()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(getExpected);

            // GetCountの動作確認
            client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(5);

            // ODataで条件指定して確認
            client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCode eq 'AA'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });

            // ODataQueryでデータ取得
            client.GetWebApiResponseResult(api.GetByODataQuery("CC")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data3Get });

            if (repository == Repository.CosmosDb)
            {
                // CosmosQueryでデータ取得
                client.GetWebApiResponseResult(api.GetByQuery("BB")).Assert(GetSuccessExpectStatusCode, testData.Data2Get);
            }

            // ID自動割り振りで登録
            testData.Data1.AreaUnitCode = null;
            var regResult = client.GetWebApiResponseResult(api.AutoRegister(testData.Data1)).Assert(RegisterSuccessExpectStatusCode).Result;
            regResult.Version.Is(1);

            testData.Data1Get.id = regResult.id;
            testData.Data1Get.AreaUnitCode = regResult.RepositoryKey;

            client.GetWebApiResponseResult(api.Get(regResult.RepositoryKey)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DisableVersion_VendorScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientAnotherVendor = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IDisableVersionVendorPrivateApi>();
            var testData = new DisableVersionTestData(repository, api.ResourceUrl, true);
            var testDataAnotherVendor = new DisableVersionVendorPrivateTestData(repository, api.ResourceUrl);

            // リソースバージョン関連の透過APIはすべてNotImplement
            var transparentApis = new List<WebApiRequestModel>()
            {
                api.GetCurrentVersion(),
                api.GetVersionInfo(),
                api.CompleteRegisterVersion(),
                api.CreateRegisterVersion(),
                api.GetRegisterVersion(),
                api.SetNewVersion()
            };
            transparentApis.ForEach(x => client.GetWebApiResponseResult(x).Assert(NotImplementedExpectStatusCode));

            // クリーンアップ
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientAnotherVendor.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // レコードを登録
            var regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // レコードを取得して確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // レコードを上書きで登録
            testData.Data1.AreaUnitName = "aaa_new";
            regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // レコードを取得して確認
            testData.Data1Get.AreaUnitName = "aaa_new";
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // レコードを更新
            client.GetWebApiResponseResult(api.Update("AA", testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);

            // レコードを取得して確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Patched);

            // 全データ登録
            var regData = new List<AreaUnitModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            var regExpected = new List<RegisterResponseModel>() { testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected };
            var regListResponse = client.GetWebApiResponseResult(api.RegisterList(regData)).Assert(RegisterSuccessExpectStatusCode);
            JsonConvert.DeserializeObject<List<RegisterResponseModel>>(regListResponse.RawContentString).OrderBy(x => x.id).ToList().IsStructuralEqual(regExpected);

            // GetCountの動作確認
            client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(5);

            // ODataで条件指定して確認
            client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCode eq 'AA'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });


            // ベンダー依存の確認
            // AnotherVendorからはここまでで登録したデータは見えないこと
            clientAnotherVendor.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(0);
            clientAnotherVendor.GetWebApiResponseResult(api.Get("AA")).Assert(NotFoundStatusCode);

            // AnotherVendorでデータ登録して 無印のベンダーからは取得できないこと
            regResponse = clientAnotherVendor.GetWebApiResponseResult(api.Register(testDataAnotherVendor.DataVendorOriginal)).Assert(RegisterSuccessExpectStatusCode, testDataAnotherVendor.DataVendorOriginalRegistExpected);
            client.GetWebApiResponseResult(api.Get("XX")).Assert(NotFoundStatusCode);
        }

        [DataTestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DisableVersion_PersonScenario(Repository repository)
        {
            var client = new IntegratedTestClient("test1") { TargetRepository = repository };
            var clientAnotherPerson = new IntegratedTestClient("test2") { TargetRepository = repository };
            var api = UnityCore.Resolve<IDisableVersionPersonPrivateApi>();
            var testData = new DisableVersionTestData(repository, api.ResourceUrl, false, true);
            var testDataAnotherPerson = new DisableVersionPersonPrivateTestData(repository, api.ResourceUrl);

            // リソースバージョン関連の透過APIはすべてNotImplement
            var transparentApis = new List<WebApiRequestModel>()
            {
                api.GetCurrentVersion(),
                api.GetVersionInfo(),
                api.CompleteRegisterVersion(),
                api.CreateRegisterVersion(),
                api.GetRegisterVersion(),
                api.SetNewVersion()
            };
            transparentApis.ForEach(x => client.GetWebApiResponseResult(x).Assert(NotImplementedExpectStatusCode));

            // クリーンアップ
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientAnotherPerson.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // レコードを登録
            var regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // レコードを取得して確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // レコードを上書きで登録
            testData.Data1.AreaUnitName = "aaa_new";
            regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // レコードを取得して確認
            testData.Data1Get.AreaUnitName = "aaa_new";
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // レコードを更新
            client.GetWebApiResponseResult(api.Update("AA", testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);

            // レコードを取得して確認
            client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Patched);

            // 全データ登録
            var regData = new List<AreaUnitModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            var regExpected = new List<RegisterResponseModel>() { testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected };
            var regListResponse = client.GetWebApiResponseResult(api.RegisterList(regData)).Assert(RegisterSuccessExpectStatusCode);
            JsonConvert.DeserializeObject<List<RegisterResponseModel>>(regListResponse.RawContentString).OrderBy(x => x.id).ToList().IsStructuralEqual(regExpected);

            // GetCountの動作確認
            client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(5);

            // ODataで条件指定して確認
            client.GetWebApiResponseResult(api.OData("$filter=AreaUnitCode eq 'AA'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });


            // 個人依存の確認
            // AnotherPersonからはここまでで登録したデータは見えないこと
            clientAnotherPerson.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(0);
            clientAnotherPerson.GetWebApiResponseResult(api.Get("AA")).Assert(NotFoundStatusCode);

            // AnotherPersonでデータ登録して 無印のユーザーからは取得できないこと
            regResponse = clientAnotherPerson.GetWebApiResponseResult(api.Register(testDataAnotherPerson.DataPersonOriginal)).Assert(RegisterSuccessExpectStatusCode, testDataAnotherPerson.DataPersonOriginalRegistExpected);
            client.GetWebApiResponseResult(api.Get("XX")).Assert(NotFoundStatusCode);
        }
    }
}
