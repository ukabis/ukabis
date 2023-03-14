using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class CacheDocumentHistoryTest : ApiWebItTestCase
    {
        #region TestData

        private class CacheDocumentHistoryTestData : TestDataBase
        {
            public AreaUnitModelEx Data1 = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA",
                AreaUnitName = "aaa"
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~CacheDocumentHistory~1~DataA"
            };
            public AreaUnitModelEx Data1Get = new AreaUnitModelEx()
            {
                PK = "DataA",
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = "API~IntegratedTest~CacheDocumentHistory~1~DataA",
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
                id = $"API~IntegratedTest~CacheDocumentHistory~1~DataB"
            };
            public AreaUnitModelEx Data2Get = new AreaUnitModelEx()
            {
                PK = "DataB",
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                id = "API~IntegratedTest~CacheDocumentHistory~1~DataB",
                _Owner_Id = WILDCARD,
                Timestamp = WILDCARD
            };


            public CacheDocumentHistoryTestData(string resourceUrl) : base(Repository.Default, resourceUrl, true) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void CacheDocumentHistoryTest_DocumentHistorySenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<ICacheDocumentHistoryApi>();
            var testData = new CacheDocumentHistoryTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            var documentkey = client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result.id;

            // 全取得
            var first = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;
            var second = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data1Get }).Result;

            // キャッシュから取得しているため一致
            first.IsStructuralEqual(second);
            // もう１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.Data2RegistExpected);

            // 履歴に退避
            client.GetWebApiResponseResult(api.DriveOutDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            // データは取れない キャッシュが削除されていることの確認
            client.GetWebApiResponseResult(api.Get(testData.Data1.PK)).Assert(NotFoundStatusCode);
            // GetAllのキャッシュもクリアされている
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get });

            // 履歴から復帰
            client.GetWebApiResponseResult(api.ReturnDocument(documentkey)).Assert(HttpStatusCode.NoContent);
            // データは取得できる 
            client.GetWebApiResponseResult(api.Get(testData.Data1.PK)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            // GetAllのキャッシュもクリアされている
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModelEx>() { testData.Data2Get, testData.Data1Get });
        }
    }
}
