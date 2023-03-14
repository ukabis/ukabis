using System.Net;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.UnitTest.Com.Extensions;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class GroupFilterTest : ItTestCaseBase
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

        [TestMethod]
        public void GroupFilter_NormalScenario()
        {
            // データ削除
            var representative = this.CreateApiClient("test1");
            var response = representative.client.Request(representative.api.DeleteAllByGroup(TestGroupId));
            this.CheckStatusCode(response.StatusCode, new List<HttpStatusCode>() { HttpStatusCode.NoContent, HttpStatusCode.NotFound });
            var other = this.CreateApiClient("test3");
            response = other.client.Request(other.api.DeleteAll());
            var msg = response.Content.ReadAsStringAsync().Result;
            this.CheckStatusCode(response.StatusCode, new List<HttpStatusCode>() { HttpStatusCode.NoContent, HttpStatusCode.NotFound });

            // グループ代表者でデータ登録
            var groupData0 = new GroupFilterModel() { code = "0" };
            representative.client.Request(representative.api.Register(groupData0, TestGroupId)).StatusCode.Is(HttpStatusCode.Created);

            // グループ代表者でデータ取得
            representative.client.Request(representative.api.Get(groupData0.code, TestGroupId))
                .ToWebApiResponseResult<GroupFilterModel>().Result.code.Is(groupData0.code);

            // グループIDを指定せずにグループ代表者でデータ取得(APIは個人依存でデータ所有者は代表者なので取得可能)
            representative.client.Request(representative.api.Get(groupData0.code))
                .ToWebApiResponseResult<GroupFilterModel>().Result.code.Is(groupData0.code);

            // グループIDを指定してグループメンバーでデータ取得(代表者としてAPIが実行されるため取得可能)
            var member = this.CreateApiClient("test2");
            var result = member.client.Request(member.api.Get(groupData0.code, TestGroupId))
                .ToWebApiResponseResult<GroupFilterModel>().Result;
            result.code.Is(groupData0.code);

            // グループIDを指定せずにグループメンバーでデータ取得(グループメンバー個人のデータを取得するのでNotFound)
            member.client.Request(member.api.Get(groupData0.code)).StatusCode.Is(HttpStatusCode.NotFound);

            // グループメンバーで登録したデータの_Owner_Idがグループ代表者になっていることを確認する
            var groupData1 = new GroupFilterModel() { code = "1" };
            member.client.Request(member.api.Register(groupData1, TestGroupId)).StatusCode.Is(HttpStatusCode.Created);
            var api = member.api.Get(groupData1.code, TestGroupId);
            member.client.Request(api).ToWebApiResponseResult<GroupFilterModel>().Result._Owner_Id.Is(result._Owner_Id);

            // グループメンバー以外でデータ登録
            var otherData = new GroupFilterModel() { code = "2" };
            other.client.Request(other.api.Register(otherData)).StatusCode.Is(HttpStatusCode.Created);

            // グループメンバー以外でグループ代表者のデータを取得(データを取得できないことを確認)
            other.client.Request(other.api.Get(groupData0.code)).StatusCode.Is(HttpStatusCode.NotFound);

            // グループメンバー以外で自身のデータを取得
            other.client.Request(other.api.Get(otherData.code))
                .ToWebApiResponseResult<GroupFilterModel>().Result.code.Is(otherData.code);

            // データ削除
            member.client.Request(member.api.DeleteAllByGroup(TestGroupId)).StatusCode.Is(HttpStatusCode.NoContent);
            other.client.Request(other.api.DeleteAll()).StatusCode.Is(HttpStatusCode.NoContent);

            // 所属していないグループを指定してデータを取得(BadRequest)
            other.client.Request(other.api.Get(groupData0.code, TestGroupId)).StatusCode.Is(HttpStatusCode.BadRequest);

            // 存在しないグループを指定してデータを取得(BadRequest)
            other.client.Request(other.api.Get(groupData0.code, "test")).StatusCode.Is(HttpStatusCode.BadRequest);
        }

        private (IDynamicApiClient client, IGroupFilterApi api) CreateApiClient(string authId)
        {
            var env = UnityCore.Resolve<IServerEnvironment>();
            var auth = this.CreateAuth(env, authId);
            return (UnityCore.Resolve<IDynamicApiClient>(env.ToCI(), auth.ToCI()), UnityCore.Resolve<IGroupFilterApi>(env.ToCI()));
        }

        private IAuthenticationInfo CreateAuth(IServerEnvironment env, string authId)
        {
            return AuthenticationInfoFactory.Create(env).Merge(this.AccountManager.Find(authId));
        }

        private void CheckStatusCode(HttpStatusCode actual, List<HttpStatusCode> expected)
        {
            expected.Contains(actual).IsTrue($"Expected Codes: [{string.Join(',', expected.Select(code => code.ToString()))}]. Actual Code: {actual.ToString()}.");
        }
    }
}
