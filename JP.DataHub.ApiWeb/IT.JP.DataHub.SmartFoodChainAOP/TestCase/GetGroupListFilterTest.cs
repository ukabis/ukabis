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
    public class GetGroupListFilterTest : ItTestCaseBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }

        [TestMethod]
        public void GetGroupListFilter_NormalScenario()
        {
            // 代表者で所属グループを取得
            var test1 = this.CreateApiClient("test1");
            var response = test1.client.Request(test1.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>();
            response.StatusCode.Is(HttpStatusCode.OK);
            ValidateGroup(response.Result.Single(group => group.groupId.StartsWith("__")));

            // 管理者でグループを取得
            var test4 = this.CreateApiClient("test4");
            response = test4.client.Request(test4.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>();
            response.StatusCode.Is(HttpStatusCode.OK);
            ValidateGroup(response.Result.Single(group => group.groupId.StartsWith("__")));

            // メンバーで所属グループを取得
            var test2 = this.CreateApiClient("test2");
            response = test2.client.Request(test2.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>();
            response.StatusCode.Is(HttpStatusCode.OK);
            // テスト用グループが正しく取得できることを確認する
            response.Result.Single(group => group.groupId.StartsWith("__")).groupId.Is("__IntegratedTestGroupId");

            // 複数グループ所属ユーザでグループを取得
            var test3 = this.CreateApiClient("test3");
            response = test3.client.Request(test3.api.GetGroupList()).ToWebApiResponseResult<List<GroupsModel>>();
            response.StatusCode.Is(HttpStatusCode.OK);
            // テスト用グループが正しく取得できることを確認する
            var groupIds = response.Result.Where(group => group.groupId.StartsWith("__"))
                                          .Select(group => group.groupId)
                                          .OrderBy(id => id)
                                          .ToList();
            groupIds.Count.Is(2);
            groupIds[0].Is("__IntegratedTestGroupId_All");
            groupIds[1].Is("__IntegratedTestGroupId_Multiple");
        }

        [TestMethod]
        public void GetGroupListFilter_WithScope()
        {
            var test1 = this.CreateApiClient("test1");
            var scope = "SmartFoodChain";
            var response = test1.client.Request(test1.api.GetGroupListWithScope(scope)).ToWebApiResponseResult<List<GroupsModel>>();
            response.StatusCode.Is(HttpStatusCode.OK);

            // テスト用グループだけ取得
            var groups = response.Result.Where(group => group.groupId.StartsWith("__")).ToList();

            // 指定したscope及びAll以外のグループが含まれていないかどうか
            var unexpectedGroupIds = response.Result.Where(group => !(group.scope.Contains("All") || group.scope.Contains(scope)))
                                                  .Select(group => group.groupId)
                                                  .ToList();
            unexpectedGroupIds.Count.Is(0, $"scope: {{{scope}}} を持たないグループが含まれています。想定外グループのgroupId: {string.Join(", ", unexpectedGroupIds)}");
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

        private void ValidateGroup(GroupsModel group)
        {
            group.groupId.Is("__IntegratedTestGroupId");
            group.groupName.Is("Test Group");
            group.scope.Single().Is("SmartFoodChain");
            group.CompanyId.Is("__IntegratedTestCompanyId");
            var representativeMember = group.representativeMember;
            representativeMember.mailAddress.Is("dev");
            representativeMember.accessControl.Single().Is("ReadWrite");
            group.member.Count.Is(2);
            group.member[0].mailAddress.Is("devops");
            group.member[0].accessControl.Single().Is("Read");
            group.member[1].mailAddress.Is("test4");
            group.member[1].accessControl.Single().Is("Read");
        }
    }
}
