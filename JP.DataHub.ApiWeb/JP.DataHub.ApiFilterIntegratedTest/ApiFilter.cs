using System.Net;
using System.Text;
using JP.DataHub.Aop;

namespace JP.DataHub.ApiFilterIntegratedTest
{
    /// <summary>
    /// AOP用のキャッシュテスト
    /// </summary>
    /// <remarks>
    /// テストの流れはFilterApiTest_AopCacheScenario参照
    /// DLLを跨いだスコープの確認のためApiFilterSampleとは別プロジェクトとしている
    /// </remarks>
    public class AopCache1 : AbstractApiFilter
    {
        private string _key;
        private string _value1;

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            _key = param.QueryStringDic.GetOrDefault("key");
            _value1 = param.QueryStringDic.GetOrDefault("value1");

            CacheHelper.Add(_key, _value1);
            return null;
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            var result = CacheHelper.Get<string>(_key);

            // AopCache3でキャッシュは削除されているので値は取得できない
            if (result == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                return response;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("AopCache1.AfterAction: CacheHelper.Get returns unexpected value.", Encoding.UTF8, "text/plain");
                return response;
            }
        }
    }

    /// <summary>
    /// AOP用のキャッシュテスト
    /// </summary>
    /// <remarks>
    /// テストの流れはFilterApiTest_AopCacheScenario参照
    /// </remarks>
    public class AopCache3 : AbstractApiFilter
    {
        private string _key;
        private string _value1;

        public override HttpResponseMessage BeforeAction(IApiFilterActionParam param)
        {
            _key = param.QueryStringDic.GetOrDefault("key");
            _value1 = param.QueryStringDic.GetOrDefault("value1");

            // AopCache1で登録したキャッシュが取得される
            var result = CacheHelper.Get<string>(_key);
            if (result == _value1)
            {
                return null;
            }
            else
            {
                var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                response.Content = new StringContent("AopCache3.BeforeAction: CacheHelper.Get returns unexpected value.", Encoding.UTF8, "text/plain");
                return response;
            }
        }

        public override HttpResponseMessage AfterAction(HttpResponseMessage msg)
        {
            CacheHelper.Remove(_key);
            return null;
        }
    }
}
