using System;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("AOP")]
    public class FilterApiTest : ApiWebItTestCase
    {
        private class MyParam
        {
            public string Action { get; set; }
            public string VendorId { get; set; }
            public string SystemId { get; set; }
            public string OpenId { get; set; }
            public string ResourceUrl { get; set; }
            public string ApiUrl { get; set; }
            public string MediaType { get; set; }
            public string QueryString { get; set; }
            public string Contents { get; set; }
            public string Accept { get; set; }
            public string ContentRange { get; set; }
            string HttpMethodType { get; set; }
        }


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void FilterApiTest_NormalScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IFilterApi>();

            var response = client.GetWebApiResponseResult(api.OData()).Assert(GetExpectStatusCodes);
            response.Headers.Single(x => x.Key == "X-Hook" && x.Value.Count() == 1 && x.Value.First() == "1").IsNotNull();
            response.Headers.Single(x => 
            {
                if (x.Key != "X-BeforeAction")
                {
                    return false;
                }

                var param = JsonConvert.DeserializeObject<MyParam>(x.Value.FirstOrDefault());
                param.OpenId.Is(client.GetOpenId());
                param.VendorId.Is(client.VendorSystemInfo.VendorId);
                param.SystemId.Is(client.VendorSystemInfo.SystemId);

                return true;
            }).IsNotNull();
        }

        [TestMethod]
        public void FilterApiTest_OrderedFilterScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IOrderedFilterApi>();

            var response = client.GetWebApiResponseResult(api.OData()).Assert(GetExpectStatusCodes);
            response.Headers.Single(x => x.Key == "X-Hook" && x.Value.Count() == 1 && x.Value.First() == "2").IsNotNull();
            response.Headers.Single(x => x.Key == "X-AfterAction" && x.Value.Count() == 1 && x.Value.First() == "OrderedApiFilterB").IsNotNull();
            response.Headers.Single(x => x.Key == "X-RequestUrl" && x.Value.Count() == 1 && x.Value.First() == "?$select=AreaUnitCode&$top=1").IsNotNull();
        }

        /// <summary>
        /// AOP用キャッシュテスト
        /// </summary>
        /// <remarks>
        /// DLLを跨いだスコープの確認のため2つのプロジェクトを使用
        /// 1.ApiFilterIntegratedTest.AopCache1.BeforeAction => AopCacheHelper.Add     : キャッシュを追加
        /// 2.ApiFilterSample.AopCache2.BeforeAction         => AopCacheHelper.GetOrAdd: DLLが違うため1の結果は取得されない
        /// 3.ApiFilterIntegratedTest.AopCache3.BeforeAction => AopCacheHelper.Get     : 1で追加したキャッシュが取得される
        /// 4.ApiFilterIntegratedTest.AopCache3.AfterAction  => AopCacheHelper.Remove  : 1で追加したキャッシュを削除
        /// 5.ApiFilterSample.AopCache2.AfterAction          => AopCacheHelper.GetOrAdd: 2で追加したキャッシュが取得される
        /// 6.ApiFilterIntegratedTest.AopCache1.AfterAction  => AopCacheHelper.Get     : キャッシュは削除済のため取得されない
        /// </remarks>
        [TestMethod]
        public void FilterApiTest_AopCacheScenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAopCacheApi>();

            client.GetWebApiResponseResult(api.GetEx(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString())).Assert(GetSuccessExpectStatusCode);
        }
    }
}
