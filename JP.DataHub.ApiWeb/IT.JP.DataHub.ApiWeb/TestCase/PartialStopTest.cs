using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
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
    /// 一部機能停止時のApiの実行可否をテストする。
    /// </summary>
    [TestClass]
    [TestCategory("ManageAPI")]
    public class PartialStopTest : ApiWebItTestCase
    {
        private static HttpStatusCode disabledStatusCode = HttpStatusCode.NotImplemented;

        #region TestData

        private class PartialStopTestData : TestDataBase
        {
            public AcceptDataModel Data1 = new AcceptDataModel()
            {
                Code = "CD01",
                Name = "CD01_Name"
            };

            public AcceptDataModel Data2 = new AcceptDataModel()
            {
                Code = "CD02",
                Name = "CD02_Name"
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        #region リポジトリグループ

        /// <summary>
        /// リポジトリグループが有効/無効のAPIの実行
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_RepositoryGroup()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopTestApi>();
            var testData = new PartialStopTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.GetRepositorySuccess()).Assert(GetSuccessExpectStatusCode).Result.Code.Is(testData.Data1.Code);

            // 上記と同じフィジカルリポジトリが設定された無効なリポジトリグループのAPIを呼んでエラーになること
            client.GetWebApiResponseResult(api.GetRepositoryError()).Assert(disabledStatusCode);

            // APIは無効だが存在はしていること
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            clientM.GetWebApiResponseResult(manageApi.GetApiResourceFromUrl("/API/IntegratedTest/PartialStopTest")).Result.MethodList
                .Any(x => x.MethodUrl.StartsWith(nameof(api.GetRepositoryError))).IsTrue();
        }

        /// <summary>
        /// セカンダリリポジトリグループが有効/無効のAPIの実行
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_SecondaryRepositoryGroup()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopTestApi>();
            var testData = new PartialStopTestData();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode);

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.GetSecondaryRepositorySuccess()).Assert(GetSuccessExpectStatusCode).Result.Code.Is(testData.Data2.Code);

            // 上記と同じフィジカルリポジトリが設定された無効なセカンダリリポジトリグループのAPIを呼んでエラーになること
            client.GetWebApiResponseResult(api.GetSecondaryRepositoryError()).Assert(disabledStatusCode);

            // APIは無効だが存在はしていること
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            clientM.GetWebApiResponseResult(manageApi.GetApiResourceFromUrl("/API/IntegratedTest/PartialStopTest")).Result.MethodList
                .Any(x => x.MethodUrl.StartsWith(nameof(api.GetSecondaryRepositoryError))).IsTrue();
        }

        #endregion

        #region HTTPメソッドタイプ

        /// <summary>
        /// HTTPメソッドタイプがGETのAPIのテスト
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_HttpMethodType_Get()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopHttpMethodTypeApi>();

            // 有効なAPIを実行
            client.GetWebApiResponseResult(api.GetSuccess()).Assert(GetErrorExpectStatusCode);

            // 無効なAPIを実行
            client.GetWebApiResponseResult(api.GetDisabled()).Assert(disabledStatusCode);

            // 存在しないAPIを実行
            client.GetWebApiResponseResult(api.GetError()).Assert(NotImplementedExpectStatusCode);
        }

        /// <summary>
        /// HTTPメソッドタイプがPOSTのAPIのテスト
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_HttpMethodType_Post()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopHttpMethodTypeApi>();

            // 有効なAPIを実行
            client.GetWebApiResponseResult(api.PostSuccess()).Assert(GetErrorExpectStatusCode);

            // 無効なAPIを実行
            client.GetWebApiResponseResult(api.PostDisabled()).Assert(disabledStatusCode);

            // 存在しないAPIを実行
            client.GetWebApiResponseResult(api.PostError()).Assert(NotImplementedExpectStatusCode);
        }

        /// <summary>
        /// HTTPメソッドタイプがPUTのAPIのテスト
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_HttpMethodType_Put()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopHttpMethodTypeApi>();

            // 有効なAPIを実行
            client.GetWebApiResponseResult(api.PutSuccess()).Assert(GetErrorExpectStatusCode);

            // 無効なAPIを実行
            client.GetWebApiResponseResult(api.PutDisabled()).Assert(disabledStatusCode);

            // 存在しないAPIを実行
            client.GetWebApiResponseResult(api.PutError()).Assert(NotImplementedExpectStatusCode);
        }

        /// <summary>
        /// HTTPメソッドタイプがDELETEのAPIのテスト
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_HttpMethodType_Delete()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopHttpMethodTypeApi>();

            // 有効なAPIを実行
            client.GetWebApiResponseResult(api.DeleteSuccess()).Assert(GetErrorExpectStatusCode);

            // 無効なAPIを実行
            client.GetWebApiResponseResult(api.DeleteDisabled()).Assert(disabledStatusCode);

            // 存在しないAPIを実行
            client.GetWebApiResponseResult(api.DeleteError()).Assert(NotImplementedExpectStatusCode);
        }

        /// <summary>
        /// HTTPメソッドタイプがPATCHのAPIのテスト
        /// </summary>
        [TestMethod]
        public void PartialStopTest_NormalSenario_HttpMethodType_Patch()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IPartialStopHttpMethodTypeApi>();

            // 有効なAPIを実行
            client.GetWebApiResponseResult(api.PatchSuccess()).Assert(RegisterErrorExpectStatusCode);

            // 無効なAPIを実行
            client.GetWebApiResponseResult(api.PatchDisabled()).Assert(disabledStatusCode);

            // 存在しないAPIを実行
            client.GetWebApiResponseResult(api.PatchError()).Assert(NotImplementedExpectStatusCode);
        }
        #endregion
    }
}