using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("Roslyn")]
    public class RoslynScriptCacheTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void RoslynScriptCacheTest_NormalScenario()
        {
            // APIが2回目呼ばれるときはキャッシュを利用する

            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRoslynScriptCacheApiForPerson>();

            // CacheExpireSec=0 既存のキャッシュを削除する
            var dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            api.AddHeaders.Add(HeaderConst.X_Cache, "on");
            client.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var json = client.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();

            // Requestした時刻を返すことを確認
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);

            // dateTimeNowとdateTimeNow2は異なるはずだが念のため
            System.Threading.Tasks.Task.Delay(10);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            var dateTimeNow2 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = client.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow2, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();

            // キャッシュされたデーターが取得されることを確認
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);
        }

        [TestMethod]
        public void RoslynScriptCacheTest_ForAll_NormalScenario()
        {
            // 他ベンダー、他システムからもキャッシュを利用可能

            var clientA = new IntegratedTestClient("test1", "SmartFoodChainAdmin");
            var clientB = new IntegratedTestClient("test2", "SmartFoodChainAdmin");
            var clientC = new IntegratedTestClient("test3", "SmartFoodChain2TestSystemC");
            var api = UnityCore.Resolve<IRoslynScriptCacheApiForAll>();

            // CacheExpireSec=0 既存のキャッシュを削除する
            var dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            api.AddHeaders.Add(HeaderConst.X_Cache, "on");
            clientA.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);
            clientC.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNowを返すことを確認
            dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var json = clientA.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);

            // dateTimeNowとdateTimeNow2は異なるはずだが念のため
            System.Threading.Tasks.Task.Delay(10);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNowを返すことを確認（キャッシュを利用）
            var dateTimeNow2 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow2, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);

            System.Threading.Tasks.Task.Delay(10);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNowを返すことを確認（キャッシュを利用）
            var dateTimeNow3 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = clientC.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow3, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);
        }

        [TestMethod]
        public void RoslynScriptCacheTest_ForVendor_NormalScenario()
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin");
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystemC");
            var api = UnityCore.Resolve<IRoslynScriptCacheApiForVendor>();

            // CacheExpireSec=0 既存のキャッシュを削除する
            var dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            api.AddHeaders.Add(HeaderConst.X_Cache, "on");
            clientA.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNowを返すことを確認
            dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var json = clientA.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);

            // dateTimeNowとdateTimeNow2は異なるはずだが念のため
            System.Threading.Tasks.Task.Delay(10);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNow2を返すことを確認
            var dateTimeNow2 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow2, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow2);

            // 同じベンダーの場合はキャッシュを共有する
            // dateTimeNow2を返すことを確認
            var dateTimeNow3 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow2, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow2);
        }

        [TestMethod]
        public void RoslynScriptCacheTest_ForPerson_NormalScenario()
        {
            var clientA = new IntegratedTestClient("test1");
            var clientB = new IntegratedTestClient("test2");
            var api = UnityCore.Resolve<IRoslynScriptCacheApiForPerson>();

            // CacheExpireSec=0 既存のキャッシュを削除する
            var dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            api.AddHeaders.Add(HeaderConst.X_Cache, "on");
            clientA.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 0)).Assert(GetSuccessExpectStatusCode);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNowを返すことを確認
            dateTimeNow = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            var json = clientA.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow);

            // dateTimeNowとdateTimeNow2は異なるはずだが念のため
            System.Threading.Tasks.Task.Delay(10);

            // 日付を取得（キャッシュがなければ Requestした時刻を返す）
            // dateTimeNow2を返すことを確認
            var dateTimeNow2 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow2, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow2);

            // 同じ個人であればキャッシュの共有ができる
            // dateTimeNow2を返すことを確認
            var dateTimeNow3 = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            json = clientB.GetWebApiResponseResult(api.GetRoslynCacheDateTime(dateTimeNow3, 60)).Assert(GetSuccessExpectStatusCode).ContentString.ToJson();
            Assert.AreEqual(json["Result"]["CacheValue"].ToString(), dateTimeNow2);
        }
    }
}

