using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.WebRequest
{
    public interface IWebRequestManager
    {
        HttpResponseMessage Get(string requestUri, string userName, string password, Dictionary<string, string> header = null, HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseContentRead, TimeSpan? timeout = null);

        HttpResponseMessage Send(HttpRequestMessage request, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        HttpResponseMessage Post(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        HttpResponseMessage Put(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        HttpResponseMessage Patch(string requestUri, Stream content, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        HttpResponseMessage Delete(string requestUri, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        /// <summary>
        /// 指定された URI に GET 要求を非同期操作として送信します。
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="timeout">タイムアウト値</param>
        /// <returns>
        /// リクエスト結果
        /// </returns>
        Task<HttpResponseMessage> GetAsync(string requestUri, Dictionary<string, string> header = null, TimeSpan? timeout = null);

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
        Task<HttpResponseMessage> GetAsync(string requestUri, string userName, string password, TimeSpan? timeout = null);

        Task<Stream> GetStreamAsync(string requestUri, TimeSpan? timeout = null);

        Task<Stream> GetStreamAsync(string requestUri, string userName, string password, TimeSpan? timeout = null);

        /// <summary>
        /// 指定された URI に GET 要求を非同期操作として送信します。(Json形式)
        /// </summary>
        /// <param name="requestUri">要求の送信先URL</param>
        /// <param name="timeout">timeout</param>
        /// <returns></returns>
        Task<HttpResponseMessage> JsonGetAsync(string requestUri, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        Task<HttpResponseMessage> PostAsync(string requestUri, Stream content, TimeSpan? timeout = null);
        
        Task<HttpResponseMessage> PostAsync(string requestUri, Stream content, string userName, string password, TimeSpan? timeout = null);

        /// <summary>
        /// 指定された URI に POST 要求を非同期操作として送信します。
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="content">サーバーに送信される HTTP 要求の内容</param>
        /// <param name="timeout">timeout</param>
        /// <returns>リクエスト結果</returns>
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, TimeSpan? timeout = null);

        /// <summary>
        /// 指定された URI に POST 要求を非同期操作として送信します。(Json形式)
        /// </summary>
        /// <param name="requestUri">要求の送信先 URI</param>
        /// <param name="content">サーバーに送信される HTTP 要求の内容</param>
        /// <param name="timeout">timeout</param>
        /// <returns>リクエスト結果</returns>
        Task<HttpResponseMessage> JsonPostAsync(string requestUri, HttpContent content, Dictionary<string, string> header = null, TimeSpan? timeout = null);

        Task<HttpResponseMessage> PatchAsync(string requestUri, Stream content, TimeSpan? timeout = null);

        Task<HttpResponseMessage> PatchAsync(string requestUri, Stream content, string userName, string password, TimeSpan? timeout = null);

        Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent content, TimeSpan? timeout = null);

        Task<HttpResponseMessage> JsonPatchAsync(string requestUri, HttpContent content, TimeSpan? timeout = null);

        Task<HttpResponseMessage> DeleteAsync(string requestUri, string userName, string password, Dictionary<string, string> header = null, TimeSpan? timeout = null);
    }
}
