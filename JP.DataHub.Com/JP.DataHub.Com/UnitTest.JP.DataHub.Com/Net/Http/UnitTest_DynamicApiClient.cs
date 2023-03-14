using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.Com.WebApi;

namespace UnitTest.JP.DataHub.Com.Net.Http
{
    [TestClass]
    public class UnitTest_DynamicApiClient : ComUnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestCleanup]
        public override void TestCleanup()
        {
            base.TestCleanup();
        }

        /// <summary>
        /// DIを利用する
        /// appsettings.jsonで指定されているサーバーと環境、およびアカウントを使ってサーバーにアクセスする
        /// </summary>
        [TestMethod]
        public void DependencyInjectionバージョン_デフォルトの設定でWebAPIを呼び出す()
        {
            var client = UnityCore.Resolve<IDynamicApiClient>();
            var brandname = UnityCore.Resolve<IBrandNameApi>();
            var x = client.Request(brandname.GetList())
                .ToWebApiResponseResult<List<BrandNameModel>>();
            x.StatusCode.IsNot(System.Net.HttpStatusCode.Forbidden);
            //x.Result.Count.Is(8);

            // 楽天市場のランキング情報を文字列で取得
            var ranking = UnityCore.Resolve<IRakutenIchibaItemRanking>();
            var y = client.Request(ranking.GetRanking())
                .ToWebApiResponseResult();
        }

        /// <summary>
        /// DIを利用する
        /// サーバーや環境は「test1」の「AzureDotnet6開発」を指定する
        /// アカウントは「test2」を使う
        /// </summary>
        [TestMethod]
        public void DependencyInjectionバージョン_環境やアカウントを指定()
        {
            var env = ServerMan.Find("test1", "AzureDotnet6開発");
            var account = env.GetAuthenticationInfo().Merge(AccountManager.Find("test2"));
            var client = UnityCore.ResolveCI<IDynamicApiClient>(env, account);
            var brandname = UnityCore.ResolveCI<IBrandNameApi>(env);
            var x = client.Request(brandname.GetList());
            x.StatusCode.IsNot(System.Net.HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void CreateInstance_Version()
        {
            var client = new DynamicApiClient(Environment, AuthenticationInfo);
            var brandname = new BrandNameApi_Inheritence(Environment);
            WebApiClient.AdditionalHeader.Add("hoge", "hogehoge");                  // 任意なヘッダーを追加している
            var x = client.Request(brandname.GetList())
                .ToWebApiResponseResult<List<BrandNameModel>>();
            x.StatusCode.IsNot(System.Net.HttpStatusCode.Forbidden);
        }
    }
}
