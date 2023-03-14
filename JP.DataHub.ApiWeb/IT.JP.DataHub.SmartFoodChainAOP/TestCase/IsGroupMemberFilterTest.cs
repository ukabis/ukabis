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
    public class IsGroupMemberFilterTest : ItTestCaseBase
    {
        private string TestGroupId = "__IntegratedTestGroupId";

        private string TestGroupAllId = "__IntegratedTestGroupId_All";

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

        /// <summary>
        /// IsGroupMemberFilterのテスト。
        /// AOP自体は存在するが、現状API(/API/Global/Private/Groups/IsGroupMember)を定義していないため動作しない。
        /// </summary>
        [TestMethod]
        [Ignore]
        public void IsGroupMemberFilter_NormalScenario()
        {
            // 代表者でIsGroupMemberをリクエストしてtrueになるか
            var representative = this.CreateApiClient("test1");
            var response = representative.client.Request(representative.api.IsGroupMember(TestGroupId, "SmartFoodChain"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(true);

            // グループメンバーでIsGroupMemberをリクエストしてtrueになるか
            var member = this.CreateApiClient("test2");
            response = member.client.Request(member.api.IsGroupMember(TestGroupId, "SmartFoodChain"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(true);

            // scopeがAllのグループ+個別scope指定でIsGroupMemberをリクエストしてtrueになるか
            var other = this.CreateApiClient("test3");
            response = other.client.Request(other.api.IsGroupMember(TestGroupAllId, "SmartFoodChain"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(true);

            // グループ未所属ユーザーでIsGroupMemberをリクエストしてfalseになるか
            response = other.client.Request(other.api.IsGroupMember(TestGroupId, "SmartFoodChain"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(false);

            // 存在しないグループを指定してIsGroupMemberをリクエストしてfalseになるか
            response = member.client.Request(member.api.IsGroupMember(TestGroupId + "hoge", "SmartFoodChain"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(false);

            // scope違いを指定してIsGroupMemberをリクエストしてfalseになるか
            response = member.client.Request(member.api.IsGroupMember(TestGroupId, "Sensor"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(false);

            // scopeがAllのグループ+存在しないscope指定でIsGroupMemberをリクエストしてfalseになるか
            response = other.client.Request(other.api.IsGroupMember(TestGroupAllId, "hoge"));
            response.StatusCode.Is(HttpStatusCode.OK);
            response.ToWebApiResponseResult<IsGroupMemberModel>().Result.isGroupMember.Is(false);
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
