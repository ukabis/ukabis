using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure
{
    public class FakeHttpClientHandler : HttpClientHandler
    {
        private readonly Dictionary<string, HttpResponseMessage> responseDic = new Dictionary<string, HttpResponseMessage>();

        /// <summary>
        /// 指定したHttpメソッド、Urlに対するレスポンスを設定します。
        /// </summary>
        /// <param name="method">Httpメソッド</param>
        /// <param name="url">Url</param>
        /// <param name="status">レスポンスのステータスコード</param>
        /// <param name="content">レスポンスの内容</param>
        public void SetResponse(HttpMethod method, string url, HttpStatusCode status, string content)
        {
            var response = new HttpResponseMessage(status)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };

            responseDic[method.Method + " " + url] = response;
        }

        /// <summary>
        /// リクエスト対応するレスポンスを返します。
        /// </summary>
        /// <param name="request">リクエスト</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>レスポンス</returns>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string key = request.Method.Method + " " + request.RequestUri;

            var response = responseDic.ContainsKey(key) ? responseDic[key] :
                new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(string.Empty),
                    RequestMessage = request
                };

            return Task.FromResult(response);
        }
    }
}
