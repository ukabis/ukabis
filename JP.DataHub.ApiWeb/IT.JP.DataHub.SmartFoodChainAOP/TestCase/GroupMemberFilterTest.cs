using System.Net;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class GroupMemberFilterTest : ItTestCaseBase
    {
        /// <summary>
        /// テストグループ名(代表者にReadWrite権限有り)。
        /// </summary>
        private string TestGroupName = "__IntegratedTest_GroupMemberFilter_UpdateScenario";


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void GroupMemberFilter_UpdateByManager()
        {
            var test2 = this.CreateApiClient("test2");
            var openIds = this.GetOpenIds();
            // 予め投入したテストグループ(test2ユーザは管理者)を取得
            var response = test2.client.Request(test2.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupName));
            group.IsNotNull();

            // グループ名を更新(NoContentが返ってくるか)
            var orgName = group.groupName;
            group.groupName += " updated";
            response = test2.client.Request(test2.api.Update(group.groupId, group));
            response.StatusCode.Is(HttpStatusCode.NoContent, response.Content.ReadAsStringAsync().Result);

            // グループが更新されているか
            response = test2.client.Request(test2.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            response.ToWebApiResponseResult<List<GroupsModel>>()
                    .Result
                    .Single(g => g.groupName.Contains(this.TestGroupName))
                    .groupName
                    .EndsWith("updated")
                    .IsTrue($"グループ{group.groupId} のメンバーが更新されていません。");

            // 更新内容を戻す
            group.groupName = orgName;
            response = test2.client.Request(test2.api.Update(group.groupId, group));
            response.StatusCode.Is(HttpStatusCode.NoContent, response.Content.ReadAsStringAsync().Result);
        }

        // テストグループ登録で予約枠アカウントを消費するため、Ignoreにしておく
        [Ignore]
        [TestMethod]
        public void GroupMemberFilter_DeleteByManager()
        {
            // 各ユーザのOpenIdを取得
            var openIds = this.GetOpenIds();

            // test2ユーザを管理者としてグループを作成する
            var group = new GroupsModel()
            {
                groupName = "GroupMemberFilter_DeleteByWritableMember_NoContent",
                representativeMember = null,
                member = new List<GroupsMemberModel>()
                {
                    new GroupsMemberModel()
                    {
                        openId = openIds["test1"],
                        mailAddress = "devops",
                        accessControl = new List<string>() { "ReadWrite" }
                    },
                    new GroupsMemberModel()
                    {
                        openId = openIds["test2"],
                        mailAddress = "test2",
                        accessControl = new List<string>() { "ReadWrite" }
                    }
                },
                manager = new List<string>() { openIds["test2"] },
                scope = new List<string>()
                {
                    "All"
                }
            };

            var test1 = this.CreateApiClient("test1");
            var response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Created, response.Content.ReadAsStringAsync().Result);

            // 作成したグループを取得
            var test2 = this.CreateApiClient("test2");
            response = test2.client.Request(test2.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var groupId = response.ToWebApiResponseResult<List<GroupsModel>>()
                                  .Result
                                  .Single(g => g.groupName.Contains(group.groupName))
                                  .groupId;

            // 管理者でグループを削除できるか
            test2.client.Request(test2.api.Delete(groupId)).StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void GroupMemberFilter_UpdateByMember()
        {
            var test3 = this.CreateApiClient("test3");

            // 予め投入したテストグループ(test3ユーザはメンバー)を取得
            var response = test3.client.Request(test3.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupName));
            group.IsNotNull();

            // メンバーで更新(Forbiddenが返ってくるか)
            response = test3.client.Request(test3.api.Update(group.groupId, group));
            response.StatusCode.Is(HttpStatusCode.Forbidden, response.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void GroupMemberFilter_DeleteByMember()
        {
            var test3 = this.CreateApiClient("test3");

            // 予め投入したテストグループ(test3ユーザはメンバー)を取得
            var response = test3.client.Request(test3.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupName));
            group.IsNotNull();

            // メンバーで削除(Forbiddenが返ってくるか)
            response = test3.client.Request(test3.api.Delete(group.groupId));
            response.StatusCode.Is(HttpStatusCode.Forbidden, response.Content.ReadAsStringAsync().Result);
        }

        private (IDynamicApiClient client, IGroupsApi api) CreateApiClient(string authId)
        {
            var env = UnityCore.Resolve<IServerEnvironment>();
            var auth = this.CreateAuth(env, authId);
            return (UnityCore.Resolve<IDynamicApiClient>(env.ToCI(), auth.ToCI()), UnityCore.Resolve<IGroupsApi>(env.ToCI()));
        }

        private IAuthenticationInfo CreateAuth(IServerEnvironment env, string authId)
        {
            return AuthenticationInfoFactory.Create(env).Merge(this.AccountManager.Find(authId));
        }

        private Dictionary<string, string> GetOpenIds()
        {
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var api = UnityCore.Resolve<IOpenIdUserApi>();
            var users = new List<string>() { "test1", "test2", "test3" };

            var openIds = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var account = this.AccountManager.Find(user);
                var address = ((AuthenticationInfo)account).OpenId.Account;
                var response = client.Request(api.GetFullAccess(address)).ToWebApiResponseResult<OpenIdUserModel>();
                response.StatusCode.Is(HttpStatusCode.OK);
                openIds.Add(user, response.Result.OpenId);
            }

            return openIds;
        }
    }
}
