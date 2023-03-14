using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.Log;
using JP.DataHub.MVC.Unity;

namespace JP.DataHub.ApiWeb.Infrastructure.WebRequest
{
    // .NET6
    internal class WebRequestManager : IWebRequestManager
    {
        private IHttpClientFactory _httpClientFactory { get; }
        public WebRequestManager(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(WebRequestManager));

        /// <summary>
        /// 指定された URI に GET 要求を非同期操作として送信します。
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="timeout">timeout</param>
        /// <returns>
        /// リクエスト結果
        /// </returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUri, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);

            return await client.GetAsync(requestUri);
        }

        public async Task<Stream> GetStreamAsync(string requestUri, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);

            return await client.GetStreamAsync(requestUri);
        }

        public HttpResponseMessage Get(string requestUri, string userName, string password, Dictionary<string, string> header = null, HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead, TimeSpan? timeout = null)
        {
            logger.Info($"WebRequestManager.Get:url={requestUri}");
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            logger.Info($"WebRequestManager Created HttpClient");
            var result = client.GetAsync(requestUri, httpCompletionOption).Result;
            logger.Info($"WebRequestManager Requested :url={requestUri}:StatusCode={result.StatusCode}");
            return result;
        }

        /// <summary>
        /// 指定された URI に GET 要求を非同期操作として送信します。
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="userName">Basic認証用ユーザ</param>
        /// <param name="password">Basic認証用パスワード</param>
        /// <param name="timeout">timeout</param>
        /// <returns>
        /// リクエスト結果
        /// </returns>
        public async Task<HttpResponseMessage> GetAsync(string requestUri, string userName, string password, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, timeout: timeout);
            return await client.GetAsync(requestUri);
        }

        public Task<Stream> GetStreamAsync(string requestUri, string userName, string password, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, timeout: timeout);

            return client.GetStreamAsync(requestUri);
        }

        /// <summary>
        /// 指定された URI に GET 要求を非同期操作として送信します。(Jon形式)
        /// </summary>
        /// <param name="requestUri">要求の送信先URL</param>
        /// <param name="timeout">timeout</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> JsonGetAsync(string requestUri, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetAsync(requestUri);
        }

        public HttpResponseMessage Post(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.PostAsync(requestUri, CreateContents(content)).Result;
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            return await client.PostAsync(requestUri, CreateContents(content));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, Stream content, string userName, string password, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, timeout: timeout);
            return await client.PostAsync(requestUri, CreateContents(content));
        }

        /// <summary>
        /// 指定された URI に POST 要求を非同期操作として送信します。
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="content">サーバーに送信される HTTP 要求の内容</param>
        /// <param name="timeout">timeout</param>
        /// <returns>
        /// リクエスト結果
        /// </returns>
        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            return await client.PostAsync(requestUri, content);
        }


        /// <summary>
        /// 指定された URI に POST 要求を非同期操作として送信します。(Json形式)
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="content">サーバーに送信される HTTP 要求の内容</param>
        /// <param name="timeout">timeout</param>
        /// <returns>
        /// リクエスト結果
        /// </returns>
        public async Task<HttpResponseMessage> JsonPostAsync(string requestUri, HttpContent content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.PostAsync(requestUri, content);
        }

        public HttpResponseMessage Put(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.PutAsync(requestUri, CreateContents(content)).Result;
        }

        public HttpResponseMessage Delete(string requestUri, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.DeleteAsync(requestUri).Result;
        }

        public HttpResponseMessage Patch(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, requestUri)
            {
                Content = CreateContents(content)
            };
            return client.SendAsync(request).Result;
        }

        #region
        private HttpClient CreateHttpClientInstance(string userName = null, string password = null, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            //var client = new HttpClient();
            var client = _httpClientFactory.CreateClient();
            if (timeout != null)
            {
                client.Timeout = (TimeSpan)timeout;
            }
            //ヘッダーを追加
            if (header != null)
            {
                foreach (var h in header)
                {
                    if (h.Key != "Content-Type")
                    {
                        //AcceptヘッダーなどはAdd()だと不正な値だったときにバリデーションで弾かれるので
                        if (!client.DefaultRequestHeaders.TryAddWithoutValidation(h.Key, h.Value))
                        {
                            logger.Warn($"Failure Add RelayHeader HeaderKey:{h.Key}");
                        }
                    }
                }
            }

            //DynamicApiで設定されている場合はそちらを優先する
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var byteArray = Encoding.ASCII.GetBytes($"{userName}:{password}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }

            return client;
        }

        private StreamContent CreateContents(Stream content)
        {
            var streamContents = new StreamContent(content);
            streamContents.Headers.Remove("Content-Type");
            streamContents.Headers.TryAddWithoutValidation("Content-Type", HttpContextAccessor.Current.Request.Headers["Content-Type"].ToList());
            return streamContents;
        }

        #endregion
    }
}
