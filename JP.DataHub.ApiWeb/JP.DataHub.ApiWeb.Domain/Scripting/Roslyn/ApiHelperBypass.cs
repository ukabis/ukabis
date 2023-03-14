using JP.DataHub.Aop;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    internal class ApiHelperBypass : IApiHelper
    {
        private ApiHelper Helper = new ApiHelper();

        //public HttpResponseMessage ExecuteApi(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) => throw new NotSupportedException(); //Helper.ExecuteApi(contents, queryString, urlParam);

        //public T ExecuteApiToObject<T>(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) where T : class => throw new NotSupportedException(); //Helper.ExecuteApiToObject<T>(contents, queryString, urlParam);

        public HttpResponseMessage ExecuteGetApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecuteGetApi(url, contents, querystring, null, headers);

        public Task<HttpResponseMessage> ExecuteGetApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecuteGetApiAsync(url, contents, querystring, null, headers);

        public T ExecuteGetApiToObject<T>(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) where T : class => Helper.ExecuteGetApiToObject<T>(url, contents, querystring, null, headers);

        public HttpResponseMessage ExecutePostApi(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePostApi(url, contents, querystring, null, headers);

        public Task<HttpResponseMessage> ExecutePostApiAsync(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePostApiAsync(url, contents, querystring, null, headers);
        public HttpResponseMessage ExecutePostApi(string url, Stream contents, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePostApi(url, contents, querystring, null, headers);

        public Task<HttpResponseMessage> ExecutePostApiAsync(string url, Stream contents, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePostApiAsync(url, contents, querystring, null, headers);

        public HttpResponseMessage ExecutePutApi(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePutApi(url, contents, querystring, null, headers);

        public Task<HttpResponseMessage> ExecutePutApiAsync(string url, string contents, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePutApiAsync(url, contents, querystring, null, headers);

        public HttpResponseMessage ExecuteDeleteApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecuteDeleteApi(url, contents, querystring, null, headers);

        public Task<HttpResponseMessage> ExecuteDeleteApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecuteDeleteApiAsync(url, contents, querystring, null, headers);

        public HttpResponseMessage ExecutePatchApi(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePatchApi(url, contents, querystring, null, headers);

        public Task<HttpResponseMessage> ExecutePatchApiAsync(string url, string contents = null, string querystring = null, Dictionary<string, List<string>> headers = null) => Helper.ExecutePatchApiAsync(url, contents, querystring, null, headers);
    }
}