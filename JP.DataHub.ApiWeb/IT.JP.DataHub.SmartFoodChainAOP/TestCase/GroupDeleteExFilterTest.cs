using System.Net;
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
    public class GroupDeleteExFilterTest : ItTestCaseBase
    {
        private string TestGroupId = "__IntegratedTestGroupId";

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
        public void GroupDeleteExFilter_NormalScenario()
        {
            // テスト用グループを取得
            var representative = this.CreateApiClient("test1");
            var group = representative.client.Request(representative.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>()
                .Result.Single(group => group.groupId == TestGroupId);

            // グループ未所属ユーザでグループを登録
            // (環境ごとのOpenIdの差異を吸収するため、テスト用グループをコピーして登録する)
            var other = this.CreateApiClient("test3");
            var groupName = $"Test Group_{Guid.NewGuid()}";
            group.groupId = null;
            group.groupName = groupName;
            other.client.Request(other.api.Register(group)).StatusCode.Is(HttpStatusCode.Created);

            // 登録したグループを再取得
            group = representative.client.Request(representative.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>()
                .Result.Single(group => group.groupName == groupName);
            var groupId = group.groupId;

            // グループ未所属ユーザでグループを削除(NotFound)
            other.client.Request(other.api.DeleteEx(groupId)).StatusCode.Is(HttpStatusCode.NotFound);

            // グループメンバーで所属グループを削除(Forbidden)
            var member = this.CreateApiClient("test2");
            member.client.Request(member.api.DeleteEx(groupId)).StatusCode.Is(HttpStatusCode.Forbidden);

            // 代表者で所属グループを削除(Forbidden)
            representative.client.Request(representative.api.DeleteEx(groupId)).StatusCode.Is(HttpStatusCode.Forbidden);

            // 管理者で所属グループを削除
            var manager = this.CreateApiClient("test4");
            manager.client.Request(representative.api.DeleteEx(groupId)).StatusCode.Is(HttpStatusCode.NoContent);

            // 削除されていることを確認
            var groups = representative.client.Request(representative.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>().Result;
            groups.Any(g => g.groupId == groupId).Is(false);
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
    }
}
