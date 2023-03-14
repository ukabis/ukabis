using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
    public class ResourceSharingTest : ApiWebItTestCase
    {
        // 違うベンダーシステムで登録。異なるベンダーシステムで取得できる(やりたいこと)
        // ベンダー依存の場合●。
        // ヘッダーがあってもルールが存在しないときは、自分のパーティションを検索する > NotFoundではない

        #region TestData

        private class ResourceSharingTestData : TestDataBase
        {
            public ResourceSharingModel Data1 = new ResourceSharingModel()
            {
                key1 = "key-1",
                kid = "hogehoge"
            };
            public ResourceSharingModel Data1GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharing2~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-1",
                kid = "hogehoge"
            };

            public ResourceSharingModel Data2 = new ResourceSharingModel()
            {
                key1 = "key-2",
                kid = "hogehoge2"
            };
            public ResourceSharingModel Data2GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharing2~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-2",
                kid = "hogehoge2"
            };

            public ResourceSharingModel Data3 = new ResourceSharingModel()
            {
                key1 = "key-3",
                kid = "hogehoge3"
            };
            public ResourceSharingModel Data3GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharing2~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-3",
                kid = "hogehoge3"
            };

            public ResourceSharingModel Data4 = new ResourceSharingModel()
            {
                key1 = "key-4",
                kid = "hogehoge4"
            };
            public ResourceSharingModel Data4GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharing2~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-4",
                kid = "hogehoge4"
            };

            public ResourceSharingModel Data5 = new ResourceSharingModel()
            {
                key1 = "key-5",
                kid = "hogehoge5"
            };
            public ResourceSharingModel Data5GetExpected = new ResourceSharingModel()
            {
                id = $"API~IntegratedTest~ResourceSharing2~{WILDCARD}",
                _Owner_Id = WILDCARD,
                key1 = "key-5",
                kid = "hogehoge5"
            };

            public RegisterResponseModel DataRegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~ResourceSharing2~{WILDCARD}"
            };

            public ResourceSharingTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void ResourceSharing_VenderDependApi_NormalSenario(Repository repository)
        {
            // ベンダーシステムA,B,C,Dを用意
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientC = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemB") { TargetRepository = repository };
            var clientD = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientD.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを一件登録
            var regA = clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;
            var regB = clientB.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;
            var regC = clientC.GetWebApiResponseResult(api.Regist(testData.Data3)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;
            var regD = clientD.GetWebApiResponseResult(api.Regist(testData.Data4)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;

            // 自分で登録したデータは取得可能
            clientA.GetWebApiResponseResult(api.Get(regA.id)).Assert(GetSuccessExpectStatusCode, testData.Data1GetExpected);
            clientB.GetWebApiResponseResult(api.Get(regB.id)).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);
            clientC.GetWebApiResponseResult(api.Get(regC.id)).Assert(GetSuccessExpectStatusCode, testData.Data3GetExpected);
            clientD.GetWebApiResponseResult(api.Get(regD.id)).Assert(GetSuccessExpectStatusCode, testData.Data4GetExpected);

            // "X-ResourceSharingWith指定して、他ベンダーのデータを取得できるか確認
            // To A From B
            var vendor = clientB.VendorSystemInfo.VendorId;
            var system = clientB.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");

            clientA.GetWebApiResponseResult(api.Get(regB.id)).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);

            // To A From B ルールが許可されていても相手のデータの更新が行えないこと
            clientA.GetWebApiResponseResult(api.Update(regB.id, testData.Data3)).Assert(UpdateErrorExpectStatusCode);

            // To A From B ルールが許可されていても相手のデータの削除が行えないこと
            clientA.GetWebApiResponseResult(api.Delete(regB.id)).Assert(DeleteErrorExpectStatusCode);

            // To A From B ルールが許可されていても相手のデータへの追加が行えないこと
            var regAB = clientA.GetWebApiResponseResult(api.Regist(testData.Data4)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;
            clientA.GetWebApiResponseResult(api.Get(regAB.id)).Assert(GetErrorExpectStatusCode);

            // To A From B(DynamicApiにクエリあり)
            clientA.GetWebApiResponseResult(api.GetQuery()).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);

            // To C From D
            vendor = clientD.VendorSystemInfo.VendorId;
            system = clientD.VendorSystemInfo.SystemId;
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");

            clientC.GetWebApiResponseResult(api.Get(regD.id)).Assert(GetSuccessExpectStatusCode, testData.Data4GetExpected);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_ResourceSharingRules_GetAll(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientC = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemB") { TargetRepository = repository };
            var clientD = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // AからBへのルールはあり
            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientD.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Aがデータを2件登録
            clientA.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // AがAの全取得（2レコード）
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data3GetExpected });

            // Bがデータを3件登録
            clientB.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // BがBの全取得（3レコード）
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // Cがデータを5件登録
            var data = new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            clientC.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // CがCの全取得（5レコード）
            var expected = new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected, testData.Data4GetExpected, testData.Data5GetExpected };
            clientC.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, expected);

            // Dがデータを3件登録
            clientD.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // DがDの全取得（3レコード）
            clientD.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // AがBのデータを取得できる(ヘッダーあり)
            var vendor = clientB.VendorSystemInfo.VendorId;
            var system = clientB.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // CがBのデータを取得できない(ヘッダーなし)
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            clientC.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, expected);

            // AがDのデータを取得できる(ヘッダーあり。部分取得)
            vendor = clientD.VendorSystemInfo.VendorId;
            system = clientD.VendorSystemInfo.SystemId;
            // ヘッダーを掃除し後に設定
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data3GetExpected });
        }

        // TODO: ヘッダーはあるがルールが存在しないケース
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_ResourceSharingRules_NotFound(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Aがデータを2件登録
            clientA.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // AがAの全取得（2レコード）
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data3GetExpected });

            // Bがデータを3件登録
            clientB.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // BがBの全取得（3レコード）
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // BがAのデータを取得できない(ルールなし。自分のデータが返る)
            var vendor = clientA.VendorSystemInfo.VendorId;
            var system = clientA.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });
        }

        // ヘッダーがないと他のベンダーシステムが入れたデータは取得できない
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_IsNotExistHeader_XResourceSharingWith(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを1件登録
            var regA = clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;
            var regB = clientB.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;

            // 自分のデータを取得 ヘッダーなし
            clientA.GetWebApiResponseResult(api.Get(regA.id)).Assert(GetSuccessExpectStatusCode, testData.Data1GetExpected);
            clientB.GetWebApiResponseResult(api.Get(regB.id)).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);

            // 他人のデータを取得 ヘッダーなし
            clientA.GetWebApiResponseResult(api.Get(regB.id)).Assert(NotFoundStatusCode);
            clientB.GetWebApiResponseResult(api.Get(regA.id)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_ODataAction(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを1件登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected);

            // データを5件登録
            clientB.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 })).Assert(RegisterSuccessExpectStatusCode);

            // 自分のデータを取得(select) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$select=key1,kid,_Owner_Id,id")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(filter) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$filter=key1 eq 'key-1'")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(top) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$top=3")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(orderby) ヘッダーなし
            clientA.GetWebApiResponseResult(api.OData("$orderby=kid")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected });
            // 自分のデータを取得(count) ヘッダーなし
            var result = clientA.GetWebApiResponseResult(api.OData("$count=true")).Assert(GetSuccessExpectStatusCode).RawContentString;
            JArray.Parse(result)[0].Value<int>().Is(1);

            // AがBのデータを取得できる(5件, ヘッダーあり)
            var vendor = clientB.VendorSystemInfo.VendorId;
            var system = clientB.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");

            // AがBのデータを取得(select)
            var expected = new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected, testData.Data4GetExpected, testData.Data5GetExpected };
            clientA.GetWebApiResponseResult(api.OData("$select=key1,kid,_Owner_Id,id")).Assert(GetSuccessExpectStatusCode, expected);
            // AがBのデータを取得(filter)
            clientA.GetWebApiResponseResult(api.OData("$filter=key1 eq 'key-2'")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data2GetExpected });
            // AがBのデータを取得(top)
            clientA.GetWebApiResponseResult(api.OData("$top=3")).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });
            // AがBのデータを取得(orderby)
            expected = new List<ResourceSharingModel>() { testData.Data5GetExpected, testData.Data4GetExpected, testData.Data3GetExpected, testData.Data2GetExpected, testData.Data1GetExpected };
            clientA.GetWebApiResponseResult(api.OData("$orderby=kid desc")).Assert(GetSuccessExpectStatusCode, expected);
            // AがBのデータを取得(count)
            result = clientA.GetWebApiResponseResult(api.OData("$count=true")).Assert(GetSuccessExpectStatusCode).RawContentString;
            JArray.Parse(result)[0].Value<int>().Is(5);
        }

        #region データのキャッシュテスト
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_CheckApiDataCache(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Aがデータを3件登録
            clientA.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3 })).Assert(RegisterSuccessExpectStatusCode);

            // Aが自分のデータを全取得（3レコード）
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });

            // Bがデータを5件登録
            var data = new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            clientB.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // Bが自分のデータを全取得（5レコード）
            var expected = new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected, testData.Data4GetExpected, testData.Data5GetExpected };
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode);

            // AがBのデータを取得できる(5件, ヘッダーあり)
            var vendor = clientB.VendorSystemInfo.VendorId;
            var system = clientB.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, expected);

            // ヘッダーを除去すると自分のデータが返る
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data1GetExpected, testData.Data2GetExpected, testData.Data3GetExpected });
        }
        #endregion


        // ルールテスト
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_RulesPattern(Repository repository)
        {
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientC = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemB") { TargetRepository = repository };
            var clientD = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var clientE = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemC") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientD.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientE.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Bがデータを5件登録
            clientB.GetWebApiResponseResult(api.RegistList(new List<ResourceSharingModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 })).Assert(RegisterSuccessExpectStatusCode);

            // From_B_To_C. Where句にWhereを使用((Select * From c Where c.key1 != "key-1")。大文字小文字
            // 複数ルール(Select * From c Where c.key1 != "key-1")
            var vendor = clientB.VendorSystemInfo.VendorId;
            var system = clientB.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            clientC.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<ResourceSharingModel>() { testData.Data2GetExpected, testData.Data3GetExpected, testData.Data4GetExpected, testData.Data5GetExpected });

            // From B to E.ルールがすべて空
            clientE.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // From D To E. ルールがおかしい
            vendor = clientD.VendorSystemInfo.VendorId;
            system = clientD.VendorSystemInfo.SystemId;
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            clientE.GetWebApiResponseResult(api.GetAll()).AssertErrorCode(BadRequestStatusCode, "E50405");

            // ルールがすべて Select *From c => これは正常系で網羅しているからなし
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_HeaderCheck(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // VendorIdがない
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"SystemId\":\"TestSystemId\"}}");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(BadRequestStatusCode);

            // 一度ヘッダーを除去
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);

            // SystemIdがない           
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"TestVendorId\"}}");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(BadRequestStatusCode);

            // 一度ヘッダーを除去
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);

            // 形式が違う
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"[{{\"VendorId\":\"TestVendorId\", \"SystemId\":\"TestSystemId\"}}]");
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(BadRequestStatusCode);
        }

        // ODataDeleteのテスト
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResourceSharing_ResourceSharingRules_ODataDelete(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingApi>();
            var testData = new ResourceSharingTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを一件登録
            var regA = clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;
            var regB = clientB.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.DataRegistExpected).Result;

            // 自分で登録したデータは取得可能
            clientA.GetWebApiResponseResult(api.Get(regA.id)).Assert(GetSuccessExpectStatusCode, testData.Data1GetExpected);
            clientB.GetWebApiResponseResult(api.Get(regB.id)).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);

            // Bのデータを削除
            clientB.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);

            // AがBのデータを参照
            var vendor = clientB.VendorSystemInfo.VendorId;
            var system = clientB.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");

            // NotFoundが返る
            clientA.GetWebApiResponseResult(api.Get(regB.id)).Assert(NotFoundStatusCode);
            // ヘッダー解除
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            // Aのデータは消えていないこと
            clientA.GetWebApiResponseResult(api.Get(regA.id)).Assert(GetSuccessExpectStatusCode, testData.Data1GetExpected);
        }


        #region 
        // TODO: ルールのキャッシュテスト
        // TODO: 非同期のテスト
        #endregion
    }
}
