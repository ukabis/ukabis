using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
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
    [TestCategory("DocumentHistory")]
    public class DocumentVersionTest : ApiWebItTestCase
    {
        private const string HIGH_PERFORMANCE = "HighPerformance";
        private const string LOW_PERFORMANCE = "LowPerformance";
        private const string DELETE = "Delete";

        #region TestData

        private class DocumentVersionTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~DocumentVersion";

            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~DocumentVersion~1~AA"
            };
            public AreaUnitModel Data1Expected = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = "API~IntegratedTest~DocumentVersion~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1Patch1 = new AreaUnitModel()
            {
                ConversionSquareMeters = 99
            };
            public AreaUnitModel Data1Patched1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 99,
                id = "API~IntegratedTest~DocumentVersion~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1Patch2 = new AreaUnitModel()
            {
                ConversionSquareMeters = 999
            };
            public AreaUnitModel Data1Patched2 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 999,
                id = "API~IntegratedTest~DocumentVersion~1~AA",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1ExpectedFull = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~DocumentVersion~1~{WILDCARD}",
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~DocumentVersion",
                _Type = $"API~IntegratedTest~DocumentVersion",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data1_2 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "name2",
                ConversionSquareMeters = 555
            };
            public RegisterResponseModel Data1_2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~DocumentVersion~1~AA"
            };
            public AreaUnitModel Data1_2Expected = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "name2",
                ConversionSquareMeters = 555,
                id = "API~IntegratedTest~DocumentVersion~1~AA",
                _Owner_Id = WILDCARD
            };

            public List<AreaUnitModel> DataArrayAllSameDocument = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 2,
                }
            };

            public List<AreaUnitModel> DataArray = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                }
            };


            public DocumentVersionTestData(Repository repository, string resourceUrl, bool isVendor = false) : 
                base(repository, resourceUrl, isVendor) { }
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
        public void DocumentVersionTest_Public(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);
            var openId = client.GetOpenId();

            DeleteHistory(client, api);

            var documentkey = testData.Data1RegistExpected.id;
            var location = testData.ResourceUrl.ToLower();

            // 最初に何もないときがあるので、まずはデータを入れて消した時を初期として始める
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);

            var baseversion = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var base_version_no = baseversion.Max(x => x.VersionNo);

            // Registerとそのバージョンが正しいか？
            var ret = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).ContentString;
            var version1 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version1 = version1.Where(x => x.VersionNo == base_version_no + 1).FirstOrDefault();
            register_version1.LocationType.Is(HIGH_PERFORMANCE);
            version1.Where(x => x.VersionKey == register_version1.VersionKey).Count().Is(1);
            register_version1.Location.Is(documentkey);
            register_version1.OpenId.Is(openId);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // versionkey指定なしでGetDocumentHistoryをした場合、最新バージョンがもらえるか？
            var id = string.IsNullOrEmpty(ret) ? null : ret.ToJson()["id"].ToString();
            client.GetWebApiResponseResult(api.GetDocumentHistory(id)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // Patchのそのバージョンが正しいか？
            client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1Patch1)).Assert(UpdateSuccessExpectStatusCode);
            var version2 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version2 = version2.Where(x => x.VersionNo == base_version_no + 1).FirstOrDefault();
            var update_version2 = version2.Where(x => x.VersionNo == base_version_no + 2).FirstOrDefault();
            register_version2.VersionKey.Is(register_version1.VersionKey);
            register_version2.LocationType.Is(LOW_PERFORMANCE);
            update_version2.LocationType.Is(HIGH_PERFORMANCE);
            version2.Where(x => x.VersionKey == update_version2.VersionKey).Count().Is(1);
            AssertEx.Is(register_version2.Location, $"publicdata{location}/{AssertEx.WILDCARD}");
            update_version2.Location.Is(documentkey);
            register_version2.OpenId.Is(openId);
            update_version2.OpenId.Is(openId);
            register_version2.RepositoryGroupId.IsNot(update_version2.RepositoryGroupId);
            register_version2.PhysicalRepositoryId.IsNot(update_version2.PhysicalRepositoryId);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Patched1);

            // Patch 2回目のそのバージョンが正しいか？
            client.GetWebApiResponseResult(api.Update(testData.Data1.AreaUnitCode, testData.Data1Patch2)).Assert(UpdateSuccessExpectStatusCode);
            var version3 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version3 = version3.Where(x => x.VersionNo == base_version_no + 1).FirstOrDefault();
            var update_version3 = version3.Where(x => x.VersionNo == base_version_no + 2).FirstOrDefault();
            var update2_version3 = version3.Where(x => x.VersionNo == base_version_no + 3).FirstOrDefault();
            update_version3.VersionKey.Is(update_version2.VersionKey);
            update_version3.LocationType.Is(LOW_PERFORMANCE);
            update2_version3.LocationType.Is(HIGH_PERFORMANCE);
            version3.Where(x => x.VersionKey == update2_version3.VersionKey).Count().Is(1);
            AssertEx.Is(update_version3.Location, $"publicdata{location}/{AssertEx.WILDCARD}");
            update2_version3.Location.Is(documentkey);
            update_version3.OpenId.Is(openId);
            update2_version3.OpenId.Is(openId);
            register_version3.RepositoryGroupId.Is(update_version3.RepositoryGroupId);
            register_version3.PhysicalRepositoryId.Is(update_version3.PhysicalRepositoryId);
            update2_version3.RepositoryGroupId.Is(update_version2.RepositoryGroupId);
            update2_version3.PhysicalRepositoryId.Is(update_version2.PhysicalRepositoryId);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Patched2);

            // DELETEしてその記録がされるか？
            client.GetWebApiResponseResult(api.Delete(testData.Data1.AreaUnitCode)).Assert(DeleteSuccessStatusCode);
            var version4 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version4 = version4.Where(x => x.VersionNo == base_version_no + 1).FirstOrDefault();
            var update_version4 = version4.Where(x => x.VersionNo == base_version_no + 2).FirstOrDefault();
            var update2_version4 = version4.Where(x => x.VersionNo == base_version_no + 3).FirstOrDefault();
            var delete_version4 = version4.Where(x => x.VersionNo == base_version_no + 4).FirstOrDefault();
            update2_version4.LocationType.Is(LOW_PERFORMANCE);
            delete_version4.LocationType.Is(DELETE);
            AssertEx.Is(update2_version4.Location, $"publicdata{location}/{AssertEx.WILDCARD}");
            delete_version4.OpenId.Is(openId);
            update2_version4.RepositoryGroupId.Is(update_version3.RepositoryGroupId);
            update2_version4.PhysicalRepositoryId.Is(update_version3.PhysicalRepositoryId);
            delete_version4.RepositoryGroupId.IsNull();
            delete_version4.PhysicalRepositoryId.IsNull();
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(NotFoundStatusCode);

            // もう一度DELETEしても記録されない
            client.GetWebApiResponseResult(api.Delete(testData.Data1.AreaUnitCode)).Assert(NotFoundStatusCode);
            var version5 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            version4.Count().Is(version5.Count());
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(NotFoundStatusCode);

            // Registerすると、記録される
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            var version6 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version6 = version6.Where(x => x.VersionNo == base_version_no + 1).FirstOrDefault();
            var update_version6 = version6.Where(x => x.VersionNo == base_version_no + 2).FirstOrDefault();
            var update2_version6 = version6.Where(x => x.VersionNo == base_version_no + 3).FirstOrDefault();
            var delete_version6 = version6.Where(x => x.VersionNo == base_version_no + 4).FirstOrDefault();
            var register2_version6 = version6.Where(x => x.VersionNo == base_version_no + 5).FirstOrDefault();
            var delcount = version6.Where(x => x.LocationType == DELETE).Count();
            version6.Where(x => x.LocationType == LOW_PERFORMANCE).Count().Is(version6.Count() - delcount - 1);
            version6.Where(x => x.LocationType == HIGH_PERFORMANCE).Count().Is(1);
            register2_version6.LocationType.Is(HIGH_PERFORMANCE);
            register2_version6.Location.Is(documentkey);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // 全削除
            client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteExpectStatusCodes);
            var version7 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var odatadelete_version7 = version7.Where(x => x.VersionNo == base_version_no + 6).FirstOrDefault();

            // Registerすると、記録される
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            var version8 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register3_version8 = version8.Where(x => x.VersionNo == base_version_no + 7).FirstOrDefault();

            // データの確認
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register_version1.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, update_version2.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Patched1);
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, update2_version3.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Patched2);
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, delete_version4.VersionNo)).AssertErrorCode(NotFoundStatusCode, "E30407");
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register2_version6.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, odatadelete_version7.VersionNo)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register3_version8.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var driveoutData = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectedFull).Result ;
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // 履歴に退避
            client.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(HttpStatusCode.NoContent);

            var versionDod = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var registerDod_version1 = versionDod.OrderByDescending(x => x.VersionNo).FirstOrDefault();
            registerDod_version1.LocationType.Is(LOW_PERFORMANCE);
            registerDod_version1.VersionNo.Is(base_version_no + 7);
            versionDod.Where(x => x.VersionKey == register_version1.VersionKey).Count().Is(1);
            AssertEx.Is(registerDod_version1.Location, $"publicdata{location}/{AssertEx.WILDCARD}");
            registerDod_version1.OpenId.Is(openId);
            // データは取れない
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(NotFoundStatusCode);
            // 履歴の取得
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, registerDod_version1.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // 履歴から復帰
            client.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            versionDod = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            registerDod_version1 = versionDod.OrderByDescending(x => x.VersionNo).FirstOrDefault();
            registerDod_version1.LocationType.Is(HIGH_PERFORMANCE);
            registerDod_version1.VersionNo.Is(base_version_no + 7);
            versionDod.Where(x => x.VersionKey == register_version1.VersionKey).Count().Is(1);
            AssertEx.Is(registerDod_version1.Location, documentkey);
            registerDod_version1.OpenId.Is(openId);
            // データは復帰している
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // 管理項目も復帰していることを確認する(DriveOut前に取得したデータと比較して同じになること)
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, driveoutData);
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // データが存在しているときのReturnDocumentはBadRequest
            client.GetWebApiResponseResult(api.ReturnDocument(documentkey)).AssertErrorCode(HttpStatusCode.BadRequest, "E30408");

            client.GetWebApiResponseResult(api.DriveOutDocument(null)).Assert(HttpStatusCode.BadRequest);
            client.GetWebApiResponseResult(api.ReturnDocument(null)).AssertErrorCode(HttpStatusCode.BadRequest, "E30402");

            // データが存在しないID
            client.GetWebApiResponseResult(api.DriveOutDocument("fugafuga")).Assert(HttpStatusCode.NotFound);
            client.GetWebApiResponseResult(api.ReturnDocument("fugafuga")).AssertErrorCode(HttpStatusCode.NotFound, "E30404");

            // 復帰後にデータを登録
            client.GetWebApiResponseResult(api.Register(testData.Data1_2)).Assert(RegisterSuccessExpectStatusCode, testData.Data1_2RegistExpected);

            var version1_2 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version1_2 = version1_2.OrderByDescending(x => x.VersionNo).FirstOrDefault();
            register_version1_2.LocationType.Is(HIGH_PERFORMANCE);
            version1_2.Where(x => x.VersionKey == register_version1_2.VersionKey).Count().Is(1);
            register_version1_2.VersionNo.Is(base_version_no + 8);
            register_version1_2.Location.Is(documentkey);
            register_version1_2.OpenId.Is(openId);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1_2Expected);

            // 履歴に退避2回目
            client.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            // 退避後にデータを登録
            client.GetWebApiResponseResult(api.Register(testData.Data1_2)).Assert(RegisterSuccessExpectStatusCode, testData.Data1_2RegistExpected);
            var version1_3 = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version1_3 = version1_3.OrderByDescending(x => x.VersionNo).FirstOrDefault();
            register_version1_3.LocationType.Is(HIGH_PERFORMANCE);
            version1_3.Where(x => x.VersionKey == register_version1_3.VersionKey).Count().Is(1);
            register_version1_3.VersionNo.Is(base_version_no + 9);
            register_version1_3.Location.Is(documentkey);
            register_version1_3.OpenId.Is(openId);
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1_2Expected);

            var register_version1_3_2 = version1_3.Where(x => x.VersionNo == base_version_no + 8).FirstOrDefault();
            register_version1_3_2.LocationType.Is(LOW_PERFORMANCE);
            AssertEx.Is(register_version1_3_2.Location, $"publicdata{location}/{AssertEx.WILDCARD}");

            // こちらのAPIはベンダー依存無しなので、ベンダーBでも操作できることを確認
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            // GetDocumentVersion
            clientB.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode);
            // GetDocumentHistory
            clientB.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register_version1.VersionNo)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
            // DriveOutDocument
            clientB.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            // ReturnDocument
            clientB.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(HttpStatusCode.NoContent);

            // データを削除後にDriveout
            client.GetWebApiResponseResult(api.Delete(testData.Data1.AreaUnitCode)).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryCheck_Get(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // 一旦全削除
            client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteExpectStatusCodes);

            // Registerする
            var regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            // docKey と verKey 取得
            var arr = JArray.Parse(regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First());
            var docKey = arr[0]["documents"][0]["documentKey"];
            var verKey = arr[0]["documents"][0]["versionKey"];

            // Getする
            var getResponse = client.GetWebApiResponseResult(api.Get("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // ヘッダチェック
            getResponse.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = getResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // 履歴ヘッダは配列
            checkheader.Type.Is(JTokenType.Array);
            arr = checkheader as JArray;
            // API１つの更新なので、ArrayCountは1
            arr.Count().Is(1);

            // 0番目のresourecePath は、自身のAPIのコントローラ名
            arr[0]["resourcePath"].ToString().Is(testData.ResourceUrl);
            arr[0]["isSelfHistory"].Is(true);
            var documentsExpect = @"
{
    'documentKey':'{*}',
    'documentVersion':'{*}'
}".ToJson();
            // 0番目のdocumentsは、Registした数なので3件
            arr[0]["documents"].Count().Is(1);

            foreach (var ds in arr[0]["documents"])
            {
                ds.IsStructuralEqual(documentsExpect);
            }

            //最新版(↑で登録した)のdocKeyとverKeyのはず
            arr[0]["documents"][0]["documentKey"].Is(docKey);
            arr[0]["documents"][0]["versionKey"].Is(verKey);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryCheck_OData(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // 一旦全削除
            client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteExpectStatusCodes);

            // Registerする
            var regResponse = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            // docKey と verKey 取得
            var arr = JArray.Parse(regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First());
            var docKey = arr[0]["documents"][0]["documentKey"];
            var verKey = arr[0]["documents"][0]["versionKey"];

            // ODataでGetする
            var getResponse = client.GetWebApiResponseResult(api.OData($"$select=AreaUnitCode&$filter=id eq '{docKey}'")).Assert(GetSuccessExpectStatusCode);

            // ヘッダチェック
            getResponse.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = getResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // 履歴ヘッダは配列
            checkheader.Type.Is(JTokenType.Array);
            arr = checkheader as JArray;
            // API１つの更新なので、ArrayCountは1
            arr.Count().Is(1);

            // 0番目のresourecePath は、自身のAPIのコントローラ名
            arr[0]["resourcePath"].ToString().Is(testData.ResourceUrl);
            arr[0]["isSelfHistory"].Is(true);
            var documentsExpect = @"
{
    'documentKey':'{*}',
    'documentVersion':'{*}'
}".ToJson();
            // 0番目のdocumentsは、Registした数なので3件
            arr[0]["documents"].Count().Is(1);

            foreach (var ds in arr[0]["documents"])
            {
                ds.IsStructuralEqual(documentsExpect);
            }

            //最新版(↑で登録した)のdocKeyとverKeyのはず
            arr[0]["documents"][0]["documentKey"].Is(docKey);
            arr[0]["documents"][0]["versionKey"].Is(verKey);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryCheck_Delete(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // Registerする
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 全削除
            var delResponse = client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
            var arr = JArray.Parse(delResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First());
            
            // ヘッダチェック
            delResponse.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = delResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            //履歴ヘッダは配列
            checkheader.Type.Is(JTokenType.Array);
            arr = checkheader as JArray;
            // ArrayCountは1
            arr.Count().Is(1);

            // 0番目のresourecePath は、自身のAPIのコントローラ名
            arr[0]["resourcePath"].ToString().Is(testData.ResourceUrl);
            arr[0]["isSelfHistory"].Is(true);
            var documentsExpect = @"
{
    'documentKey':'{*}',
    'documentVersion':'{*}'
}".ToJson();
            // 0番目のdocumentsは、Registした数なので3件
            arr[0]["documents"].Count().Is(1);

            foreach (var ds in arr[0]["documents"])
            {
                ds.IsStructuralEqual(documentsExpect);
            }

            // 履歴を見に行き、Deleteで返却された履歴がDelete履歴であることを確認する
            foreach (var ds in arr[0]["documents"])
            {
                var hist = client.GetWebApiResponseResult(api.GetDocumentVersion(ds["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode).Result;
                // 該当のバージョンキーを探して、delete履歴であること
                var target = hist.Where(x => x.VersionKey == ds["versionKey"].ToString()).ToList();
                target.Count.Is(1);
                target[0].LocationType.Is("Delete");
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryCheck_ODataDelete(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // Registerする
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 全削除
            var delResponse = client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);
            var arr = JArray.Parse(delResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First());
            // ヘッダチェック
            delResponse.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = delResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // 履歴ヘッダは配列
            checkheader.Type.Is(JTokenType.Array);
            arr = checkheader as JArray;
            // ArrayCountは1
            arr.Count().Is(1);

            //0番目のresourecePath は、自身のAPIのコントローラ名
            arr[0]["resourcePath"].ToString().Is(testData.ResourceUrl);
            arr[0]["isSelfHistory"].Is(true);
            var documentsExpect = @"
{
    'documentKey':'{*}',
    'documentVersion':'{*}'
}".ToJson();
            // 0番目のdocumentsは、Registした数なので3件
            arr[0]["documents"].Count().Is(1);

            foreach (var ds in arr[0]["documents"])
            {
                ds.IsStructuralEqual(documentsExpect);
            }

            // 履歴を見に行き、Deleteで返却された履歴がDelete履歴であることを確認する
            foreach (var ds in arr[0]["documents"])
            {
                var hist = client.GetWebApiResponseResult(api.GetDocumentVersion(ds["documentKey"].ToString())).Assert(GetSuccessExpectStatusCode).Result;
                // 該当のバージョンキーを探して、delete履歴であること
                var target = hist.Where(x => x.VersionKey == ds["versionKey"].ToString()).ToList();
                target.Count.Is(1);
                target[0].LocationType.Is("Delete");
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryCheck_Array_AllSameDocument(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 3件登録
            // 同一ドキュメントは、Listでは登録できない
            client.GetWebApiResponseResult(api.RegisterList(testData.DataArrayAllSameDocument)).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryThrowAway(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // 最初に何もないときがあるので、まずはデータを入れて消した時を初期として始める
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            var documentkey = testData.Data1RegistExpected.id;
            var version = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            if (version.Count <= 0)
            {
                client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
                client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
                client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
                client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            }

            var max = version.Select(x => x.VersionNo).Max();
            client.GetWebApiResponseResult(api.HistoryThrowAway(documentkey)).Assert(GetSuccessExpectStatusCode);
            version = client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            version.Count.Is(1);
            version[0].VersionNo.Is(max);

            // 認証の確認
            // Forbidden が返って来ること
            client.DisableAdminAuthentication();
            client.GetWebApiResponseResult(api.HistoryThrowAway(documentkey)).Assert(ForbiddenExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryThrowAway_ErrorSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();

            DeleteHistory(client, api);

            client.GetWebApiResponseResult(api.HistoryThrowAway(string.Empty)).AssertErrorCode(BadRequestStatusCode, "E30402");
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryHeaderCheck(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 3件登録
            var regResponse = client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(RegisterSuccessExpectStatusCode);

            // 履歴が作成されていないか確認
            // ヘッダチェック
            regResponse.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = regResponse.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            // 履歴ヘッダは配列
            checkheader.Type.Is(JTokenType.Array);
            var arr = checkheader as JArray;
            // API１つの更新なので、ArrayCountは1
            arr.Count().Is(1);

            // 0番目のresourecePath は、自身のAPIのコントローラ名
            arr[0]["resourcePath"].ToString().Is(testData.ResourceUrl);
            arr[0]["isSelfHistory"].Is(true);
            var documentsExpect = @"
{
    'documentKey':'{*}',
    'documentVersion':'{*}'
}".ToJson();
            // 0番目のdocumentsは、Registした数なので3件
            arr[0]["documents"].Count().Is(3);

            foreach (var ds in arr[0]["documents"])
            {
                ds.IsStructuralEqual(documentsExpect);
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_NoHistory_WhenConflictOccured(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionWithOptimisticConcurrencyApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl, true);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 1件登録
            client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            // etag をわざと全然違うものにして再度登録⇒コンフリクト発生
            testData.Data1._etag = "hoge";
            var response = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(HttpStatusCode.Conflict);

            // 履歴が作成されていないか確認(履歴ヘッダが返って来ていないこと)
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(0);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_NoHistory_WhenConflictOccured_Array_First(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionWithOptimisticConcurrencyApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl, true);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 3件登録
            client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(RegisterSuccessExpectStatusCode);
            // データGet
            var getdata = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 1つ目以外にetag設定
            testData.DataArray[0]._etag = "hoge";
            testData.DataArray[1]._etag = getdata[1]._etag;
            testData.DataArray[2]._etag = getdata[2]._etag;

            // コンフリクト発生
            var response = client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(HttpStatusCode.Conflict);

            // 履歴が作成されていないか確認
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();

            // 1つ目の履歴(documentKeyの末尾が~AA)が存在しないこと
            foreach (var h in checkheader[0]["documents"])
            {
                h["documentKey"].ToString().EndsWith("~AA").Is(false);
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_NoHistory_WhenConflictOccured_Array_Second(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionWithOptimisticConcurrencyApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl, true);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 3件登録
            client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(RegisterSuccessExpectStatusCode);
            // データGet
            var getdata = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 2つ目以外にetag設定
            testData.DataArray[0]._etag = getdata[0]._etag;
            testData.DataArray[1]._etag = "hoge";
            testData.DataArray[2]._etag = getdata[2]._etag;

            // コンフリクト発生
            var response = client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(HttpStatusCode.Conflict);

            // 履歴が作成されていないか確認
            // ヘッダチェック
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();

            // 2つ目の履歴(documentKeyの末尾が~BB)が存在しないこと
            foreach (var h in checkheader[0]["documents"])
            {
                h["documentKey"].ToString().EndsWith("~BB").Is(false);
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_NoHistory_WhenConflictOccured_Array_Third(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionWithOptimisticConcurrencyApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl, true);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 3件登録
            client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(RegisterSuccessExpectStatusCode);
            // データGet
            var getdata = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 3つ目以外にetag設定
            testData.DataArray[0]._etag = getdata[0]._etag;
            testData.DataArray[1]._etag = getdata[1]._etag;
            testData.DataArray[2]._etag = "hoge";

            // コンフリクト発生
            var response = client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(HttpStatusCode.Conflict);

            // 履歴が作成されていないか確認
            // ヘッダチェック
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson().ToList();

            // 3つ目の履歴(documentKeyの末尾が~CC)が存在しないこと
            foreach (var h in checkheader[0]["documents"])
            {
                h["documentKey"].ToString().EndsWith("~CC").Is(false);
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_NoHistory_WhenConflictOccured_Array_AllConflict(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionWithOptimisticConcurrencyApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl, true);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 3件登録
            client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(RegisterSuccessExpectStatusCode);
            // データGet
            var getdata = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 全てに不正なetag設定
            testData.DataArray[0]._etag = "hoge";
            testData.DataArray[1]._etag = "hoge";
            testData.DataArray[2]._etag = "hoge";

            // コンフリクト発生
            var response = client.GetWebApiResponseResult(api.RegisterList(testData.DataArray)).Assert(HttpStatusCode.Conflict);

            // 履歴が作成されていないか確認
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(0);
        }

        [TestMethod]
        public void DocumentVersionTest_NoHistoryResourceCallTransparentAPI()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();

            var documentkey = $"API~IntegratedTest~AutoKeySimpleData~1~AA";
            client.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(NotImplementedExpectStatusCode);
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, 1)).Assert(NotImplementedExpectStatusCode);
            client.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(NotImplementedExpectStatusCode);
            client.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(NotImplementedExpectStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryReferenceCheck_ReferenceFalse(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 1件登録
            var response = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 履歴が作成されていないか確認
            // ヘッダチェック
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var arr = checkheader as JArray;
            // docKey, verKey 取得
            var docKey = arr[0]["documents"][0]["documentKey"].ToString();
            var verKey = arr[0]["documents"][0]["versionKey"].ToString();

            // Data1登録(上書き)
            testData.Data1.ConversionSquareMeters = 100000;
            response = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            var reqHeader = "{'reference':false}";
            api.AddHeaders.Add(HeaderConst.X_ReferenceHistory, reqHeader);

            // GetDocumentHistory
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionGuid(docKey, Guid.Parse(verKey))).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_HistoryReferenceCheck_ReferenceTrue(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl);

            DeleteHistory(client, api);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 1件登録
            var response = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 履歴が作成されていないか確認
            // ヘッダチェック
            response.Headers.Count(x => x.Key == HeaderConst.X_DocumentHistory).Is(1);
            var checkheader = response.Headers.GetValues(HeaderConst.X_DocumentHistory).First().ToJson();

            var arr = checkheader as JArray;
            // docKey, verKey 取得
            var docKey = arr[0]["documents"][0]["documentKey"].ToString();
            var verKey = arr[0]["documents"][0]["versionKey"].ToString();

            // Data1登録(上書き)
            testData.Data1.ConversionSquareMeters = 100000;
            response = client.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            var reqHeader = "{'reference':true}";
            api.AddHeaders.Add(HeaderConst.X_ReferenceHistory, reqHeader);

            // GetDocumentHistory reference true にしても、snapshot が無いので、通常通り
            client.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionGuid(docKey, Guid.Parse(verKey))).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void DocumentVersionTest_Public_NotFoundByOtherVendorTest(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IDocumentVersionVendorDependencyApi>();
            var testData = new DocumentVersionTestData(repository, api.ResourceUrl, true);

            DeleteHistory(clientA, api);
            DeleteHistory(clientB, api);

            var retData1 = clientA.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).ContentString;
            var documentkey = retData1.ToJson()["id"].ToString();

            clientA.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);
            var baseversion = clientA.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var base_version_no = baseversion.Max(x => x.VersionNo);

            clientA.GetWebApiResponseResult(api.Register(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            var version1 = clientA.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode).Result;
            var register_version1 = version1.Where(x => x.VersionNo == base_version_no + 1).FirstOrDefault();

            clientA.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register_version1.VersionNo)).Assert(GetSuccessExpectStatusCode);

            // 履歴に退避
            clientA.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(HttpStatusCode.NoContent);

            // 履歴から復帰
            clientA.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            // データは復帰している
            clientA.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Expected);

            // ベンダーBでは、NotFoundになること
            // GetDocumentVersion
            clientB.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(NotFoundStatusCode);
            // GetDocumentHistory
            clientB.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register_version1.VersionNo)).Assert(NotFoundStatusCode);
            // ReturnDocument
            clientB.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(NotFoundStatusCode);
            // Get
            clientB.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(NotFoundStatusCode);

            // 念のため、ベンダーAで再確認
            // GetDocumentVersion
            clientA.GetWebApiResponseResult(api.GetDocumentVersion(documentkey)).Assert(GetSuccessExpectStatusCode);
            // GetDocumentHistory
            clientA.GetWebApiResponseResult(api.GetDocumentHistoryWithVersionNo(documentkey, register_version1.VersionNo)).Assert(GetSuccessExpectStatusCode);
            // DriveOutDocument
            clientA.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            // ReturnDocument
            clientA.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(HttpStatusCode.NoContent);

            DeleteHistory(clientA, api);
        }


        /// <summary>
        /// 履歴の削除を実施する
        /// </summary>
        private void DeleteHistory<T>(IntegratedTestClient client, ICommonResource<T> api)
        {
            var response = client.GetWebApiResponseResult(api.OData("$select=id")).Assert(GetExpectStatusCodes);
            if (response.StatusCode != HttpStatusCode.NotFound)
            {
                var data = response.RawContentString.ToJson();
                foreach (var json in data)
                {
                    var id = json["id"].ToString();
                    client.GetWebApiResponseResult(api.HistoryThrowAway(id)).Assert(GetExpectStatusCodes);
                }
            }
        }
    }
}
