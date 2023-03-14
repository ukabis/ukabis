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
    public class GroupRegisterFilterTest : ItTestCaseBase
    {
        /// <summary>
        /// テストグループ名(登録用)。
        /// </summary>
        private string TestGroupNameRegisterScenario = "GroupRegisterFilterTest.GroupRegisterFilter_RegisterScenario";

        /// <summary>
        /// テストグループ名(更新用)。
        /// </summary>
        private string TestGroupNameUpdateNewGroupScenario = "__IntegratedTest_GroupRegisterFilter_UpdateScenario";

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

        // 予約枠アカウントを消費するため、Ignoreにしておく
        [Ignore]
        [TestMethod]
        public void GroupRegisterFilter_Register()
        {
            // データ削除
            var test1 = this.CreateApiClient("test1");
            var group = test1.client.Request(test1.api.GetGroupList())
                                    .ToWebApiResponseResult<List<GroupsModel>>()
                                    .Result
                                    ?.SingleOrDefault(g => g.groupName == this.TestGroupNameRegisterScenario);
            if (group != null)
            {
                test1.client.Request(test1.api.DeleteEx(group.groupId)).StatusCode.Is(HttpStatusCode.NoContent);
            }

            // 各ユーザのOpenIdを取得
            var openIds = this.GetOpenIds();

            // test1ユーザが代表者のグループを作成する
            group = new GroupsModel()
            {
                groupId = Guid.NewGuid().ToString(),
                groupName = "GroupRegisterFilterTest.GroupRegisterFilter_RegisterScenario",
                representativeMember = new GroupsMemberModel()
                {
                    openId = openIds["test1"],
                    mailAddress = "dev",
                    accessControl = new List<string>() { "ReadWrite" }
                },
                member = new List<GroupsMemberModel>()
                {
                    new GroupsMemberModel()
                    {
                        openId = openIds["test2"],
                        mailAddress = "devops",
                        accessControl = new List<string>() { "ReadWrite" }
                    }
                },
                scope = new List<string>()
                {
                    "All"
                }
            };

            // 管理者なしでグループ作成
            var response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.BadRequest);

            group.manager = new List<string>() { };
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.BadRequest);

            // メンバーに含まれない管理者でグループ作成
            group.manager = new List<string>() { openIds["test1"], openIds["test2"], Guid.NewGuid().ToString() };
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.BadRequest);

            // test1ユーザでグループ作成
            group.manager = new List<string>() { openIds["test1"], openIds["test2"] };
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Created, response.Content.ReadAsStringAsync().Result);

            // test1ユーザでグループ取得
            response = test1.client.Request(test1.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK);
            var resultGroup = JArray.Parse(response.Content.ReadAsStringAsync().Result).FirstOrDefault(x => x["groupName"].Value<string>() == this.TestGroupNameRegisterScenario);
            resultGroup.IsNotNull();

            // 代表者が予約枠アカウントになっており予約枠アカウントの領域に登録されていること
            var representativeOpenId = resultGroup["representativeMember"]["openId"].Value<string>();
            representativeOpenId.IsNot(group.representativeMember.openId);
            representativeOpenId.Is(resultGroup["_Owner_Id"].Value<string>());
            // リクエストで指定した代表者がメンバーとして追加されているか
            resultGroup["member"].Count().Is(2);
            resultGroup["member"][0]["openId"].Value<string>().Is(group.member[0].openId);
            resultGroup["member"][1]["openId"].Value<string>().Is(group.representativeMember.openId);

            // グループ削除
            test1.client.Request(test1.api.DeleteEx(resultGroup["groupId"].Value<string>())).StatusCode.Is(HttpStatusCode.NoContent);
        }

        [TestMethod]
        public void GroupRegisterFilter_UpdateGroupName_OK()
        {
            var test1 = this.CreateApiClient("test1");

            // 予め投入したテストグループ(管理者はtest1ユーザ)を取得
            var response = test1.client.Request(test1.api.GetList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario));
            group.IsNotNull();

            // グループ名を更新(OKが返ってくるか)
            var orgName = group.groupName;
            group.groupName += " updated";
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Created, response.Content.ReadAsStringAsync().Result);

            // グループが更新されているか
            response = test1.client.Request(test1.api.GetList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            response.ToWebApiResponseResult<List<GroupsModel>>()
                    .Result
                    .Single(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario))
                    .groupName
                    .EndsWith("updated");

            // 更新内容を戻す
            group.groupName = orgName;
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Created, response.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void GroupRegisterFilter_UpdateGroupMember_OK()
        {
            var test1 = this.CreateApiClient("test1");

            // 予め投入したテストグループ(管理者はtest1ユーザ)を取得
            var response = test1.client.Request(test1.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario));
            group.IsNotNull();

            // グループメンバーでメンバーを更新(OKが返ってくるか)
            var orgMember = new List<GroupsMemberModel>(group.member);
            group.member.Add(new GroupsMemberModel()
            {
                openId = "foo",
                mailAddress = "foo",
                accessControl = new List<string>() { "Read" }
            });
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Created, response.Content.ReadAsStringAsync().Result);

            // グループが更新されているか
            response = test1.client.Request(test1.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            response.ToWebApiResponseResult<List<GroupsModel>>()
                    .Result
                    .Single(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario))
                    .member
                    .Any(member => member.openId == "foo")
                    .IsTrue($"グループ{group.groupId} のメンバーが更新されていません。");

            // 更新内容を戻す
            group.member = orgMember;
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Created, response.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void GroupRegisterFilter_UpdateGroupByNonManager_BadRequest()
        {
            var test3 = this.CreateApiClient("test3");

            // 予め投入したテストグループ(管理者はtest1ユーザ)を取得
            var response = test3.client.Request(test3.api.GetGroupList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario));
            group.IsNotNull();

            // 管理者以外のメンバーで更新(Forbiddenが返ってくるか)
            response = test3.client.Request(test3.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.Forbidden, response.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void GroupRegisterFilter_UpdateGroupRepresentativeMember_BadRequest()
        {
            var test1 = this.CreateApiClient("test1");

            // 予め投入したテストグループ(管理者はtest1ユーザ)を取得
            var response = test1.client.Request(test1.api.GetList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario));
            group.IsNotNull();

            // 代表者を更新(BadRequestが返ってくるか)
            var representativeMember = group.representativeMember;
            var orgOpenId = representativeMember.openId;
            representativeMember.openId = "hoge";
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.BadRequest, response.Content.ReadAsStringAsync().Result);
        }

        [TestMethod]
        public void GroupRegisterFilter_UpdateManager_BadRequest()
        {
            var test1 = this.CreateApiClient("test1");

            // 予め投入したテストグループ(管理者はtest1ユーザ)を取得
            var response = test1.client.Request(test1.api.GetList());
            response.StatusCode.Is(HttpStatusCode.OK, response.Content.ReadAsStringAsync().Result);
            var group = response.ToWebApiResponseResult<List<GroupsModel>>()
                                .Result
                                ?.SingleOrDefault(g => g.groupName.Contains(this.TestGroupNameUpdateNewGroupScenario));
            group.IsNotNull();

            // 管理者にメンバー以外のOpenIDを追加して更新(BadRequestが返ってくるか)
            group.manager.Add(Guid.NewGuid().ToString());
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.BadRequest, response.Content.ReadAsStringAsync().Result);

            // 管理者を空にして更新(BadRequestが返ってくるか)
            group.manager = null;
            response = test1.client.Request(test1.api.Register(group));
            response.StatusCode.Is(HttpStatusCode.BadRequest, response.Content.ReadAsStringAsync().Result);
        }


        private (IDynamicApiClient client, IGroupsApi api) CreateApiClient(string authId)
        {
            var env = UnityCore.Resolve<IServerEnvironment>();
            var auth = AuthenticationInfoFactory.Create(env).Merge(this.AccountManager.Find(authId));

            return (UnityCore.Resolve<IDynamicApiClient>(env.ToCI(), auth.ToCI()), UnityCore.Resolve<IGroupsApi>(env.ToCI()));
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
