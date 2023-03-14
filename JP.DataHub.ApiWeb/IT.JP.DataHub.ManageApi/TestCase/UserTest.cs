using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.User;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class UserTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        /// <summary>
        /// B2Cユーザー登録の正常系テスト
        /// </summary>
        [TestMethod]
        public void Users_NormalScenario()
        {
            var client = new ManageApiIntegratedTestClient("test1", "DynamicApi", true);
            var api = UnityCore.Resolve<IUsersApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var result = client.GetWebApiResponseResult(api.Get(UserId)).Assert(GetExpectStatusCodes).Result;
            if (result?.UserId != null)
            {
                client.GetWebApiResponseResult(api.Delete(UserId)).Assert(DeleteSuccessStatusCode);
            }

            // ユーザー新規登録
            client.GetWebApiResponseResult(api.Post(RegisterUserData1(userId))).Assert(RegisterSuccessExpectStatusCode);

            // ユーザー1件取得
            var data = client.GetWebApiResponseResult(api.Get(UserId)).Assert(GetSuccessExpectStatusCode).Result;
            data.ToJson().Is(GetData1.ToJson());

            // ユーザー全件取得
            var getDataResult = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result;
            var getData = getDataResult.FirstOrDefault(x => x.UserId == UserId);
            getData.ToJson().Is(GetData1.ToJson());

            // 別のベンダーシステムから取得
            var client2 = new ManageApiIntegratedTestClient("testStaff", "DynamicApi2", true);

            client2.GetWebApiResponseResult(api.GetFullAccess(UserId2)).Assert(GetSuccessExpectStatusCode);
            // GetUserだと取得できない
            client2.GetWebApiResponseResult(api.Get(UserId2)).Assert(ForbiddenExpectStatusCode);

            // 削除
            client.GetWebApiResponseResult(api.Delete(UserId)).Assert(DeleteSuccessStatusCode);

            // 削除後NotFound
            client.GetWebApiResponseResult(api.Get(UserId)).Assert(NotFoundStatusCode);
        }

        #region Data

        private static string userId = "";
        public string UserId = userId;
        public static string WILDCARD = "{{*}}";
        private static string userId2 = "";
        public string UserId2 = userId2;

        /// <summary>
        /// 新規登録正常系データ1
        /// </summary>
        public OpenIdUserRequestModel RegisterUserData1(string userId)
        {
            return new OpenIdUserRequestModel() {
                UserId = userId,
                UserName = "IntegratedTestユーザー1",
                Password = ""
            };
        }

        public string RegistReturnData1 = $@"
{{
    'UserId' : '{userId}',
    'UserName' : 'IntegratedTestユーザー1',
    'OpenId':'{WILDCARD}'
}}";

        public string GetData1 = $@"
{{
    'UserId' : '{userId}',
    'UserName' : 'IntegratedTestユーザー1',
    'CreatedDateTime' :'{WILDCARD}',
    'OpenId':'{WILDCARD}'
}}";

        #endregion

    }
}
