using System.Collections.Generic;
using System.Net;
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
    public class KeyAssignedSimpleDataTest : ApiWebItTestCase
    {
        public readonly string REGDATE = "_Regdate";

        #region TestData

        private class KeyAssignedSimpleDataTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~KeyAssignedSimpleData";

            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1GetFull = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA",
                _Owner_Id = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD,
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~KeyAssignedSimpleData~1",
                _Type = $"API~IntegratedTest~KeyAssignedSimpleData"
            };
            public AreaUnitModel Data1SystemPropertiesInclude = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = "hogehoge_id",
                _Owner_Id = "hogehoge_owner",
                _Reguser_Id = "hogehoge_regu",
                _Regdate = "hogehoge_regd",
                _Upduser_Id = "hogehoge_regd",
                _Upddate = "hogehoge_regd",
                _Version = 199,
                _partitionkey = "hogehoge_pa",
                _Type = "hogehoge_type"
            };
            public AreaUnitModel Data1WithMissingProperty = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "xxx"
            };
            public AreaUnitModel Data1WithMissingPropertyGet = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "xxx",
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1WithMissingPropertyPatch = new AreaUnitModel()
            {
                AreaUnitName = "aaa"
            };
            public AreaUnitModel Data1WithMissingPropertyPatched = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1Patch = new AreaUnitModel()
            {
                id = "hogehoge2",
                ConversionSquareMeters = 2
            };
            public AreaUnitModel Data1Patched = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA",
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
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~BB"
            };
            public AreaUnitModel Data2Get = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~BB",
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
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~CC"
            };
            public AreaUnitModel Data3Get = new AreaUnitModel()
            {
                AreaUnitCode = "CC",
                AreaUnitName = "ccc",
                ConversionSquareMeters = 100,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~CC",
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
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~DD"
            };
            public AreaUnitModel Data4Get = new AreaUnitModel()
            {
                AreaUnitCode = "DD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 1000,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~DD",
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
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~EE"
            };
            public AreaUnitModel Data5Get = new AreaUnitModel()
            {
                AreaUnitCode = "EE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 10000,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~EE",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data6 = new AreaUnitModel()
            {
                AreaUnitCode = "gg",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public AreaUnitModel Data6Get = new AreaUnitModel()
            {
                AreaUnitCode = WILDCARD,
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public string Data7Key16 = "AA 0x20";
            public AreaUnitModel Data7 = new AreaUnitModel()
            {
                AreaUnitCode = "AA  ",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data7RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA 0x20"
            };
            public AreaUnitModel Data7Get = new AreaUnitModel()
            {
                AreaUnitCode = "AA  ",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA 0x20",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data7Patch = new AreaUnitModel()
            {
                AreaUnitCode = "AA  ",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2
            };
            public AreaUnitModel Data7Patched = new AreaUnitModel()
            {
                AreaUnitCode = "AA  ",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2,
                id = $"API~IntegratedTest~KeyAssignedSimpleData~1~AA 0x20",
                _Owner_Id = WILDCARD
            };

            public KeyAssignedSimpleDataTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        public void KeyAssignedSimpleDataTest_NormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).AssertErrorCode(NotFoundStatusCode, "I10401");
            // 消えていることを確認(GetAll)
            client.GetWebApiResponseResult(api.GetAll()).AssertErrorCode(NotFoundStatusCode, "I10403");

            // データ1を１件登録
            var reg1 = client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result;

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // 登録していないキーのデータを取得（ヒットしない）
            client.GetWebApiResponseResult(api.Get(testData.Data2.AreaUnitCode)).AssertErrorCode(NotFoundStatusCode, "I10403");

            // 全取得（1レコードのはず）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });

            // データ2を１件登録
            var reg2 = client.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.Data2RegistExpected).Result;

            // １つ目のデータを取得
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            // ２つ目のデータを取得
            client.GetWebApiResponseResult(api.Get(testData.Data2.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // KEYでQueryStringで取得
            client.GetWebApiResponseResult(api.GetByQuery(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            client.GetWebApiResponseResult(api.GetByQuery(testData.Data2.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // idでQueryStringで取得
            client.GetWebApiResponseResult(api.GetById(reg1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            client.GetWebApiResponseResult(api.GetById(reg2.id)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // 全取得（２レコード）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get });

            // 全データを登録
            var allRegData = new List<AreaUnitModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            var allRegistExpected = new List<RegisterResponseModel>() { testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected };
            client.GetWebApiResponseResult(api.RegistList(allRegData)).Assert(RegisterSuccessExpectStatusCode, allRegistExpected);

            // 全部のデータが入ったことを確認
            var allGetExpected = new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get };
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, allGetExpected);

            // X-GetInternalAllField指定して、システムプロパティを返すか確認
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            if (IsIgnoreGetInternalAllField)
            {
                // X-GetInternalAllField指定しても、システムプロパティが返却されないことを確認
                client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            }
            else
            {
                client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1GetFull);

                // システム依存プロパティを指定してデータを登録、システム依存プロパティが正しくセットされていることを確認(ユーザーから指定したシステムプロパティは無視される）
                if (repository != Repository.SqlServer)
                {
                    client.GetWebApiResponseResult(api.Regist(testData.Data1SystemPropertiesInclude)).Assert(RegisterSuccessExpectStatusCode);
                    client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1GetFull);
                }
            }
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // null非許容の任意項目を項目なしで登録して更新(AGRI_ICT-5874再現)
            client.GetWebApiResponseResult(api.Regist(testData.Data1WithMissingProperty)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1WithMissingPropertyGet);

            client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1WithMissingPropertyPatch)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1WithMissingPropertyPatched);

            // "AA"データをPATCHする
            client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);

            // "AA"を取得して正しく変更されているか確認
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Patched);

            // ID自動割り振りの確認
            var reg6 = client.GetWebApiResponseResult(api.AutoKeyRegist(testData.Data6)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.GetById(reg6.id)).Assert(GetSuccessExpectStatusCode, testData.Data6Get);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void KeyAssignedSimpleDataTest_DeleteAllSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // １件登録（ダミー用）
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            // 全件削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
            // 再度全件削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(NotFoundStatusCode);
            // 取得する(０件）
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void KeyAssignedSimpleDataTest_OtherVendorAccess_Public(Repository repository)
        {
            // ベンダー依存しないAPIなので、↑のベンダーとは異なるベンダーでアクセスして同一のデータが取得できることを確認する
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var clientC = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // ベンダーA,B,Cで全データを削除
            clientA.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(DeleteExpectStatusCodes, (HttpStatusCode.NotFound, "E10421"));
            clientB.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(DeleteExpectStatusCodes, (HttpStatusCode.NotFound, "E10421"));
            clientC.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(DeleteExpectStatusCodes, (HttpStatusCode.NotFound, "E10421"));

            // ベンダーAで全データを登録
            // 全データを登録
            var allRegData = new List<AreaUnitModel>() { testData.Data1, testData.Data2, testData.Data3, testData.Data4, testData.Data5 };
            var allRegistExpected = new List<RegisterResponseModel>() { testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected };
            clientA.GetWebApiResponseResult(api.RegistList(allRegData)).Assert(RegisterSuccessExpectStatusCode, allRegistExpected);

            // ベンダーA,B,Cで全データを取得（ベンダーB,Cでも全件が取得できること=ベンダーAで登録し、ベンダー境界がないため）
            var allGetExpected = new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get };
            clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, allGetExpected);
            clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, allGetExpected);
            clientC.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, allGetExpected);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void KeyAssignedSimpleDataTest_NotSetAccessTokenAuth(Repository repository)
        {
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();

            // 認証情報があるのでアクセスできる
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetExpectStatusCodes);

            // OPENIDの認証情報を設定していないためアクセスできない
            client = new IntegratedTestClient(null, "SmartFoodChainAdmin") { TargetRepository = repository };
            // TODO:現在はOpenID認証を必須にしていないため
            //client.GetWebApiResponseResult(api.GetAll()).Assert(ForbiddenExpectStatusCode);


            // OPENIDの認証情報を設定していないためアクセスできない
            client = new IntegratedTestClient(null, null) { TargetRepository = repository };
            client.GetWebApiResponseResult(api.GetAll()).Assert(ForbiddenExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void JsonSchemaNoValidationTest_RegisterSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // AreaUnitNameが必須だがそれがないデータは登録に失敗する
            var fail = new AreaUnitModel()
            {
                AreaUnitCode = "AA"
            };
            var response = client.GetWebApiResponseResult(api.Regist(fail)).Assert(RegisterErrorExpectStatusCode);
            response.RawContentString.IndexOf("AreaUnitName").IsNot(-1);

            // ↑でエラーになったものが、バリデーションチェックしないAPIの場合は正常に登録される
            client.GetWebApiResponseResult(api.RegistNoValidation(fail)).Assert(RegisterSuccessExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void JsonSchemaNoValidationTest_PatchSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // アップデートの正常
            var response = client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);
            response.ContentString.Is(string.Empty);
            // アップデート処理で、文字数オーバー
            var update = new AreaUnitModel()
            {
                AreaUnitName = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
            };
            response = client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, update)).Assert(UpdateValidationErrorExpectStatusCode);
            response.ContentString.IndexOf("maximum length of").IsNot(-1);
            // アップデートのAPIで、バリデーションしないを使って、文字数オーバーでエラーが出ないこと
            response = client.GetWebApiResponseResult(api.UpdateNoValidation(testData.Data1.AreaUnitCode, update)).Assert(UpdateSuccessExpectStatusCode);
            response.ContentString.Is(string.Empty);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void KeyAssignedSimpleDataTest_KeySpaceSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(GetAll)
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データ7を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data7)).Assert(RegisterSuccessExpectStatusCode, testData.Data7RegistExpected);

            // OData
            client.GetWebApiResponseResult(api.OData($"$filter=AreaUnitCode eq '{testData.Data7.AreaUnitCode}'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data7Get });

            // 16進数
            client.GetWebApiResponseResult(api.GetByHex(testData.Data7Key16)).Assert(GetSuccessExpectStatusCode, testData.Data7Get);

            // GetAll
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data7Get });

            // アップデート
            var response = client.GetWebApiResponseResult(api.UpdateByHex(testData.Data7Key16, testData.Data7Patch)).Assert(UpdateSuccessExpectStatusCode);
            response.ContentString.Is(string.Empty);

            // データ取得
            // OData
            client.GetWebApiResponseResult(api.OData($"$filter=AreaUnitCode eq '{testData.Data7.AreaUnitCode}'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data7Patched });

            // 16進数
            client.GetWebApiResponseResult(api.GetByHex(testData.Data7Key16)).Assert(GetSuccessExpectStatusCode, testData.Data7Patched);

            // 削除
            client.GetWebApiResponseResult(api.DeleteByHex(testData.Data7Key16)).Assert(DeleteSuccessStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void KeyAssignedSimpleDataTest_KeySpaceSenario_Sql(Repository repository)
        {
            // SQLはGET/{key}が使えないため別でテストを作る

            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IKeyAssignedSimpleDataApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(GetAll)
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データ7を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data7)).Assert(RegisterSuccessExpectStatusCode, testData.Data7RegistExpected);

            // OData
            client.GetWebApiResponseResult(api.OData($"$filter=AreaUnitCode eq '{testData.Data7.AreaUnitCode}'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data7Get });

            // 全データの消去
            client.GetWebApiResponseResult(api.ODataDelete($"$filter=AreaUnitCode eq '{testData.Data7.AreaUnitCode}'")).Assert(DeleteSuccessStatusCode);
        }

        // AGRI_ICT-5678 再現テスト
        // 以下の条件を満たす場合に別リソースのデータが取得されないことを確認
        // ・リソースにパーティションキーが指定されていること
        // ・リクエストにX-RequestContinuationが指定されていること
        // ・APIがGetAllなどパーティションキーの構成要素をパラメータに含まないこと
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void PartitionKeyAndPagingCombinationTest(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IPartitionKeyAndPagingCombinationApi>();
            var testData = new KeyAssignedSimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを１件登録
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 消えていることを確認(GetAll)
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, " ");
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });
        }
    }
}
