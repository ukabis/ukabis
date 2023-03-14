using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class CacheTest : ApiWebItTestCase
    {
        #region TestData

        private class CacheTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~Cache";

            public AreaUnitModelEx Data1 = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA",
                AreaUnitName = "aaa"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Cache~1~DataA"
            };
            public AreaUnitModelEx Data1Get = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = "API~IntegratedTest~Cache~1~DataA",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD
            };

            public AreaUnitModelEx Data2 = new AreaUnitModelEx()
            {
                PK = "DataB",
                AreaUnitCode = "BB",
                AreaUnitName = "bbb"
            };
            public RegisterResponseModel Data2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Cache~1~DataB"
            };
            public AreaUnitModelEx Data2Get = new AreaUnitModelEx()
            {
                PK = "DataB",
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                id = "API~IntegratedTest~Cache~1~DataB",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD
            };
            public AreaUnitModelEx Data2GetFull = new AreaUnitModelEx()
            {
                PK = "DataB",
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                id = "API~IntegratedTest~Cache~1~DataB",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Version = 1,
                _partitionkey = WILDCARD,
                _Type = WILDCARD,
            };

            public AreaUnitModelEx Data3Update = new AreaUnitModelEx()
            {
                AreaUnitCode = "CC",
                AreaUnitName = "bbb"
            };
            public AreaUnitModelEx Data3Get = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "CC",
                AreaUnitName = "bbb",
                id = "API~IntegratedTest~Cache~1~DataA",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD
            };

            public AreaUnitModelEx Data1_2 = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA_2",
                AreaUnitName = "aaa_2"
            };
            public RegisterResponseModel Data1_2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Cache~1~DataA"
            };
            public AreaUnitModelEx Data1_2Get = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA_2",
                AreaUnitName = "aaa_2",
                id = "API~IntegratedTest~Cache~1~DataA",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD
            };

            public AreaUnitModelEx DataODataDel_Data1 = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA",
                AreaUnitName = "aaa"
            };
            public AreaUnitModelEx DataODataDel_Data1Get = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = "API~IntegratedTest~Cache~1~DataA",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD
            };


            public CacheTestData(string resourceUrl, bool isVendor = false, bool isPerson = false) : base(Repository.Default, resourceUrl, isVendor, isPerson) { }
        }

        #endregion


        // n秒おきに9回までリトライ
        private RetryPolicy<HttpResponseMessage> retryPolicy(int intervalsec) => Policy
            .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(intervalsec));

        private RetryPolicy<HttpResponseMessage> retryArrayPolicy(int intervalsec, int arraySize = 2) => Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }
                var result = JToken.Parse(r.Content.ReadAsStringAsync().Result);
                return result.Count() != arraySize;

            })
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(20));


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void CacheTest_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ICacheApi>();
            var testData = new CacheTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            // 全取得
            var firstArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            var secondArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            // キャッシュから取得しているため一致
            firstArray.IsStructuralEqual(secondArray);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.Data2RegistExpected);
            // キャッシュはリフレッシュされる
            firstArray = client.GetWebApiResponseResult(api.GetAll(), retryArrayPolicy(5)).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get, testData.Data2Get }).Result;
            firstArray.IsNotStructuralEqual(secondArray);
            secondArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get, testData.Data2Get }).Result;
            firstArray.IsStructuralEqual(secondArray);

            // 1件取得
            var firstSingle = client.GetWebApiResponseResult(api.Get(testData.Data1.PK)).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;
            var secondSingle = client.GetWebApiResponseResult(api.Get(testData.Data1.PK)).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;
            firstSingle.IsStructuralEqual(secondSingle);
            // 別データ指定
            firstSingle = client.GetWebApiResponseResult(api.Get(testData.Data2.PK)).Assert(GetSuccessExpectStatusCode, testData.Data2Get).Result;
            secondSingle = client.GetWebApiResponseResult(api.Get(testData.Data2.PK)).Assert(GetSuccessExpectStatusCode, testData.Data2Get).Result;
            firstSingle.IsStructuralEqual(secondSingle);

            // クエリストリングが項目名と一致しない場合
            firstSingle = client.GetWebApiResponseResult(api.GetByItem(testData.Data1.PK)).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;
            secondSingle = client.GetWebApiResponseResult(api.GetByItem(testData.Data1.PK)).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;
            firstSingle.IsStructuralEqual(secondSingle);
            // 別データ指定
            firstSingle = client.GetWebApiResponseResult(api.GetByItem(testData.Data2.PK)).Assert(GetSuccessExpectStatusCode, testData.Data2Get).Result;
            secondSingle = client.GetWebApiResponseResult(api.GetByItem(testData.Data2.PK)).Assert(GetSuccessExpectStatusCode, testData.Data2Get).Result;
            firstSingle.IsStructuralEqual(secondSingle);

            // データを更新
            client.GetWebApiResponseResult(api.Update(testData.Data1.PK, testData.Data3Update)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data3Get, testData.Data2Get });

            //データ削除
            client.GetWebApiResponseResult(api.Delete(testData.Data1.PK)).Assert(DeleteSuccessStatusCode);
            firstArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get }).Result;

            if (!IsIgnoreGetInternalAllField)
            {
                // GetInternalAllField ON
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                var firstAll = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2GetFull }).Result;
                var secondAll = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2GetFull }).Result;
                firstAll.IsStructuralEqual(secondAll);
                firstArray.IsNotStructuralEqual(secondAll);
                // GetInternalAllField削除
                api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);
            }

            secondArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get }).Result;
            firstArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get }).Result;
            firstArray.IsStructuralEqual(secondArray);

            /* 定義更新によるキャッシュクリアは行われないため割愛(FW版も同様だがバリデーションの問題で通っていた)
            // 定義更新
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();

            var apiDef = clientM.GetWebApiResponseResult(manageApi.GetApiResourceFromUrl(api.ResourceUrl, false)).Assert(GetSuccessExpectStatusCode).Result;
            var methodId = apiDef.MethodList.Where(x => x.MethodUrl == "DeleteAll").First().MethodId;
            var targetMethod = clientM.GetWebApiResponseResult(manageApi.GetApiMethod(methodId)).Assert(GetSuccessExpectStatusCode).Result;
            var methodUpdate = new RegisterMethodModel()
            {
                ApiId = targetMethod.ApiId,
                Url = "DeleteAll",
                HttpMethodTypeCd = "DELETE",
                ActionTypeCd = "del",
                IsHeaderAuthentication = true,
                IsOpenIdAuthentication = true,
                RepositoryGroupId = targetMethod.RepositoryGroupId,
                IsEnable = true,
                IsHidden = true,
                QueryType = "cdb"
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterMethod(methodUpdate)).Assert(RegisterSuccessExpectStatusCode);
            firstArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get }).Result;
            firstArray.IsNotStructuralEqual(secondArray);
            */

            // キャッシュ無効
            api.AddHeaders.Add(HeaderConst.X_Cache, "true");
            firstArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get }).Result;
            secondArray = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get }).Result;
            firstArray.IsNotStructuralEqual(secondArray);
        }

        [TestMethod]
        public void CacheTest_VendorSenario()
        {
            var api = UnityCore.Resolve<ICacheVendorApi>();
            var testData = new CacheTestData(api.ResourceUrl, true);

            var clientA = new IntegratedTestClient("test1");
            var clientB = new IntegratedTestClient("test2", "SmartFoodChainPortal");

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            clientB.GetWebApiResponseResult(api.Regist(testData.Data1_2)).Assert(RegisterSuccessExpectStatusCode, testData.Data1_2RegistExpected);

            // 全取得
            var firstA = clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            var secondA = clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            // キャッシュから取得しているため一致
            firstA.IsStructuralEqual(secondA);

            var firstB = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            var secondB = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            // キャッシュから取得しているため一致
            firstB.IsStructuralEqual(secondB);

            // データを追加
            clientA.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.Data2RegistExpected);
            // キャッシュはクリアされる
            secondB = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            firstB.IsNotStructuralEqual(secondB);
        }

        [TestMethod]
        public void CacheTest_PersonSenario()
        {
            var api = UnityCore.Resolve<ICachePersonApi>();
            var testData = new CacheTestData(api.ResourceUrl, false, true);

            var clientA = new IntegratedTestClient("test1");
            var clientB = new IntegratedTestClient("test2");

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            clientA.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);
            clientB.GetWebApiResponseResult(api.Regist(testData.Data1_2)).Assert(RegisterSuccessExpectStatusCode, testData.Data1_2RegistExpected);

            // 全取得
            var firstA = clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            var secondA = clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            // キャッシュから取得しているため一致
            firstA.IsStructuralEqual(secondA);

            var firstB = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            var secondB = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            // キャッシュから取得しているため一致
            firstB.IsStructuralEqual(secondB);

            // データを追加
            clientA.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.Data2RegistExpected);
            // キャッシュはクリアされる
            secondB = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            firstB.IsNotStructuralEqual(secondB);
        }

        [TestMethod]
        public void CacheTest_ResourceSharingSenario()
        {
            var api = UnityCore.Resolve<ICacheVendorApi>();
            var testData = new CacheTestData(api.ResourceUrl, true);

            var clientFrom = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin");
            var clientTo = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem");

            // 最初に全データの消去
            clientFrom.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientTo.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            clientFrom.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 全取得
            var first = clientFrom.GetWebApiResponseResult(api.GetAll(), retryPolicy(5)).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            var second = clientFrom.GetWebApiResponseResult(api.GetAll(), retryPolicy(5)).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            // キャッシュから取得しているため一致
            first.IsStructuralEqual(second);

            // データは取得できない
            clientTo.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // 共有設定
            var vendor = clientFrom.VendorSystemInfo.VendorId;
            var system = clientFrom.VendorSystemInfo.SystemId;
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingWith, $"{{\"VendorId\":\"{vendor}\", \"SystemId\":\"{system}\"}}");
            // データの取得は可能
            first = clientTo.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            second = clientTo.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            // キャッシュから取得しているため一致
            first.IsStructuralEqual(second);
            // 共有設定を外すとデータはとれない
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingWith);
            clientTo.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データ1を１件登録
            clientTo.GetWebApiResponseResult(api.Regist(testData.Data1_2)).Assert(RegisterSuccessExpectStatusCode, testData.Data1_2RegistExpected);
            first = clientTo.GetWebApiResponseResult(api.GetAll(), retryPolicy(5)).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            second = clientTo.GetWebApiResponseResult(api.GetAll(), retryPolicy(5)).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1_2Get }).Result;
            // キャッシュから取得しているため一致
            first.IsStructuralEqual(second);
        }

        //ODataDeleteTest
        [TestMethod]
        public void ODataDeleteSenario()
        {
            var api = UnityCore.Resolve<ICacheApi>();
            var testData = new CacheTestData(api.ResourceUrl);
            var client = new IntegratedTestClient(AppConfig.Account);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.DataODataDel_Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 全取得(キャッシュ)
            var first = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.DataODataDel_Data1Get }).Result;
            var second = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.DataODataDel_Data1Get }).Result;
            first.IsStructuralEqual(second);

            // データ削除
            client.GetWebApiResponseResult(api.ODataDelete()).Assert(DeleteSuccessStatusCode);
            // キャッシュも消えて、データ存在しないことを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        public void CacheSizeOverSenario()
        {
            // サイズ確認用API（GetCacheSizeTest）はクエリで文字列を付与しており1件当たり約100KBのサイズ（MassagePackで圧縮時のサイズ）になる
            // キャッシュ最大サイズが1MBで指定されているため10件以上登録をしサイズ確認用API（GetCacheSizeTest）で全件取得すればキャッシュ最大サイズを超えてキャッシュされない
            var api = UnityCore.Resolve<ICacheApi>();
            var testData = new CacheTestData(api.ResourceUrl);
            var client = new IntegratedTestClient(AppConfig.Account);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // サイズ確認用APIで全取得
            var first = client.GetWebApiResponseResult(api.GetCacheSizeTest()).Assert(GetSuccessExpectStatusCode).Result;
            var second = client.GetWebApiResponseResult(api.GetCacheSizeTest()).Assert(GetSuccessExpectStatusCode).Result;
            // キャッシュから取得しているため一致
            first.IsStructuralEqual(second);

            // データを10件登録
            for (int i = 0; i < 10; i++)
            {
                testData.Data1.PK = i.ToString();
                client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            }

            // サイズ確認用APIで全取得
            first = client.GetWebApiResponseResult(api.GetCacheSizeTest()).Assert(GetSuccessExpectStatusCode).Result;
            second = client.GetWebApiResponseResult(api.GetCacheSizeTest()).Assert(GetSuccessExpectStatusCode).Result;
            // キャッシュされていないため不一致
            first.IsNotStructuralEqual(second);
        }


        //TODO API取得APIのIFが他のものに準拠せずDBの物理名になっているのでとりあえずの暫定対応
        private string CreateRegisterJsonString(string apiid, string repositoryGroupId)
        {
            {
                string json = $@"{{
  'ApiId': '{apiid}',
  'Url': 'DeleteAll',
  'HttpMethodTypeCd': 'DELETE',
  'ActionTypeCd': 'del',
  'MethodDescriptiveText': null,
  'Query': null,
  'RequestModelId': null,
  'ResponseModelId': null,
  'UrlModelId': null,
  'IsPostDataTypeArray': false,
  'GatewayUrl': null,
  'GatewayCredentialUserName': null,
  'GatewayCredentialPassword': null,
  'GatewayRelayHeader': null,
  'IsHeaderAuthentication': true,
  'IsVendorSystemAuthenticationAllowNull': false,
  'IsOpenIdAuthentication': true,
  'IsAdminAuthentication': false,
  'IsOverPartition': false,
  'RepositoryGroupId': '{repositoryGroupId}',
  'IsEnable': true,
  'IsAutomaticId': false,
  'IsHidden': true,
  'IsIncludeYourOwnVendor': true,
  'IsCache': false,
  'CacheMinute': '0',
  'CacheKey': null,
  'SecondaryRepositoryGroupIds': null,
  'ApiLinkList': null,
  'Script': null,
  'ScriptType': null,
  'QueryType': 'cdb'
}}";
                return json;
            }
        }
    }
}
