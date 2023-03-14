using System.Net;
using Newtonsoft.Json.Linq;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class ApiHelper
    {
        #region ExecuteApi関連

        public HttpResponseMessage ExecuteApi(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecuteApiAsync(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam, Dictionary<string, List<string>> headers = null)
            => null;

        public T ExecuteApiToObject<T>(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) where T : class
            => null;

        public HttpResponseMessage ExecuteGetApi(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecuteGetApiAsync(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public T ExecuteGetApiToObject<T>(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null) where T : class
            => null;


        public JToken ExecuteGetApiToJToken(string url, string contents = null, string querystring = null)
            => null;


        public Tuple<HttpStatusCode, JToken> ExecuteGetApiToJTokenAndHttpStatusCode(string url, string contents = null, string querystring = null)
            => null;

        public HttpResponseMessage ExecutePostApi(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        internal HttpResponseMessage ExecutePostApi(string url, Stream contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePostApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        internal Task<HttpResponseMessage> ExecutePostApiAsync(string url, Stream contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecutePutApi(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePutApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecuteDeleteApi(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecuteDeleteApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecutePatchApi(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePatchApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
            => null;
        #endregion

        #region モデル取得関係

        public string GetJsonSchemaByResourceUrl(string url)
            => null;

        public string GetJsonSchemaByModelName(string name)
            => null;
        #endregion

        #region Jsonバリデーション関連

        public IEnumerable<Rfc7807> ValidateWithRequestModel(string input)
            => null;

        public HttpResponseMessage CreateHttpResponseFromValidateWithRequestModel(string input)
            => null;

        public IEnumerable<Rfc7807> ValidateWithResponseModel(string input)
            => null;

        public HttpResponseMessage CreateHttpResponseFromValidateWithResponseModel(string input)
            => null;

        public IEnumerable<Rfc7807> ValidateWithModelByResourceUrl(string url, string input)
            => null;

        public HttpResponseMessage CreateHttpResponseFromValidateWithModelByResourceUrl(string url, string input)
            => null;

        public IEnumerable<Rfc7807> ValidateWithModelByModelName(string name, string input)
            => null;

        public HttpResponseMessage CreateHttpResponseFromValidateWithModelByModelName(string name, string input)
            => null;
        #endregion
    }
}
