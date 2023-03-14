using JP.DataHub.Aop;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    internal class ApiHelperBypass : IApiHelper
    {
        //public HttpResponseMessage ExecuteApi(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) => throw new NotSupportedException(); //Helper.ExecuteApi(contents, queryString, urlParam);

        //public T ExecuteApiToObject<T>(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) where T : class => throw new NotSupportedException(); //Helper.ExecuteApiToObject<T>(contents, queryString, urlParam);

        public HttpResponseMessage ExecuteGetApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecuteGetApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public T ExecuteGetApiToObject<T>(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) where T : class
            => null;

        public HttpResponseMessage ExecutePostApi(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePostApiAsync(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecutePostApi(string url, Stream contents, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePostApiAsync(string url, Stream contents, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecutePutApi(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePutApiAsync(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecuteDeleteApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecuteDeleteApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public HttpResponseMessage ExecutePatchApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;

        public Task<HttpResponseMessage> ExecutePatchApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null)
            => null;
    }
}