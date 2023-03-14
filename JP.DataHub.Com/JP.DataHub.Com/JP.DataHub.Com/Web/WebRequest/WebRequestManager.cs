using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Unity;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Log;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.Com.Web.WebRequest
{
    public class WebRequestManager : IWebRequestManager
    {
        private static readonly JPDataHubLogger logger = new JPDataHubLogger(typeof(WebRequestManager));

        public string ClientName { get; set; } = null;
        public IHttpClientFactory ClientFactory { get; set; } = null;

        public WebRequestManager(string clientName = null, HttpMessageHandler httpMessageHandler = null)
        {
            ClientName = clientName ?? string.Empty;
            ClientFactory = HttpClientFactory.CreateInstance(httpMessageHandler);
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

        public HttpResponseMessage Send(HttpRequestMessage request, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = CreateHttpClientInstance(timeout: timeout, header: header);
            return client.SendAsync(request).Result;
        }

        public HttpResponseMessage Post(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.PostAsync(requestUri, CreateContents(content)).Result;
        }

        public HttpResponseMessage Put(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.PutAsync(requestUri, CreateContents(content)).Result;
        }

        public HttpResponseMessage Patch(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.PatchAsync(requestUri, CreateContents(content)).Result;
        }

        public HttpResponseMessage Delete(string requestUri, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return client.DeleteAsync(requestUri).Result;
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout, header: header);

            return await client.GetAsync(requestUri);
        }

        public async Task<HttpResponseMessage> GetAsync(string requestUri, string userName, string password, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, timeout: timeout);
            return await client.GetAsync(requestUri);
        }

        public async Task<Stream> GetStreamAsync(string requestUri, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);

            return await client.GetStreamAsync(requestUri);
        }

        public Task<Stream> GetStreamAsync(string requestUri, string userName, string password, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, timeout: timeout);

            return client.GetStreamAsync(requestUri);
        }

        public async Task<HttpResponseMessage> JsonGetAsync(string requestUri, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout, header: header);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.GetAsync(requestUri);
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

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            return await client.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> JsonPostAsync(string requestUri, HttpContent content, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout, header: header);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, Stream content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            return await client.PatchAsync(requestUri, CreateContents(content));
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, Stream content, string userName, string password, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, timeout: timeout);
            return await client.PatchAsync(requestUri, CreateContents(content));
        }

        public async Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            return await client.PatchAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> JsonPatchAsync(string requestUri, HttpContent content, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(timeout: timeout);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return await client.PostAsync(requestUri, content);
        }

        public async Task<HttpResponseMessage> DeleteAsync(string requestUri, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = this.CreateHttpClientInstance(userName, password, header, timeout);
            return await client.DeleteAsync(requestUri);
        }

        #region
        private HttpClient CreateHttpClientInstance(string userName = null, string password = null, Dictionary<string, string> header = null, TimeSpan? timeout = null)
        {
            var client = ClientFactory.CreateClient(ClientName);
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
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);
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
            return streamContents;
        }

        #endregion
    }
}
