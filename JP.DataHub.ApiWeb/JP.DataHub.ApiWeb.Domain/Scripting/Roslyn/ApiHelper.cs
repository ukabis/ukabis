using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using AutoMapper;
using JP.DataHub.Com.Async;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;
using JP.DataHub.Com.Extensions;
using JP.DataHub.ApiWeb.Core.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// APIを呼び出すためのヘルパークラス
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public class ApiHelper
    {
        private static Lazy<IMapper> _mapper = new Lazy<IMapper>(() =>
        {
            var mapping = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RFC7807ProblemDetailExtendErrors, Rfc7807>().ReverseMap();
            });
            return mapping.CreateMapper();
        });

        private static IMapper Mapper => _mapper.Value;

        private IDynamicApiAction apiAction = UnityCore.Resolve<IDynamicApiDataContainer>().baseApiAction;

        private int ThresholdJsonSchemaValidaitonParallelize => UnityCore.Resolve<int>("ThresholdJsonSchemaValidaitonParallelize");

        private bool ReturnJsonValidatorErrorDetail { get => _returnJsonValidatorErrorDetail.Value; }
        private Lazy<bool> _returnJsonValidatorErrorDetail = new Lazy<bool>(() => UnityCore.Resolve<bool>("Return.JsonValidator.ErrorDetail"));

        private IPerRequestDataContainer PerRequestDataContainer { get => _perRequestDtaContainer.Value; }
        private Lazy<IPerRequestDataContainer> _perRequestDtaContainer = new Lazy<IPerRequestDataContainer>(() => UnityCore.Resolve<IPerRequestDataContainer>());

        private const string MEDIATYPE_JSON = "application/json";
        private const string MEDIATYPE_ProblemJson = "application/problem+json";

        #region ExecuteApi関連

        /// <summary>
        /// <ja>DynamicApiで定義した自身のAPIを呼び出します。</ja>
        /// <en>Executes the API that you have defined using DynamcAPI.</en>
        /// </summary>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="queryString">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="urlParam">
        /// <ja>URLパラメータ</ja>
        /// <en>URL parameters</en>
        /// </param>
        /// <param name="headers">
        /// <ja>HttpRequestのヘッダー</ja>
        /// <en>HttpRequest Header parameters</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>Response</en>
        /// </returns>
        public HttpResponseMessage ExecuteApi(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                perRequestDataContainer.MergeRequestHeaders(this.HeaderDictionaryDeepCopy(headers));
            }

            apiAction.Initialize();
            EditParam(contents, queryString, urlParam);
            var result = apiAction.ExecuteAction();

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }

            return result;
        }

        /// <summary>
        /// <ja>DynamicApiで定義した自身のAPIを呼び出します。これは非同期です</ja>
        /// <en>Executes the API that you have defined using DynamcAPI. This is asynchronous.</en>
        /// </summary>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="queryString">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="urlParam">
        /// <ja>URLパラメータ</ja>
        /// <en>URL parameters</en>
        /// </param>
        /// <param name="headers">
        /// <ja>HttpRequestのヘッダー</ja>
        /// <en>HttpRequest Header parameters</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>ResponseのTASK</en>
        /// </returns>
        public Task<HttpResponseMessage> ExecuteApiAsync(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam, Dictionary<string, List<string>> headers = null)
        {
            // 実装の観点：普通に非同期で処理すると、HttpContext.Currentがnullになる。よって別スレッド前のHttpContext.Currentを渡してあげる
            // API実行という意味では、本来もともとのAPIを実行するのであるため、HttpContext.Currentが同じもので問題ない（理屈上は）
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteApiAsyncExecutor.NewSync(httpContextAccessor.HttpContext, ExecuteApi);
            return executor.ExecuteAsync(contents, queryString, urlParam, headers);
        }

        /// <summary>
        /// <ja>DynamicApiで定義した自身のAPIを呼び出しオブジェクトに変換します。</ja>
        /// <en>Executes the API that you have defined using DynamcAPI as an object</en>
        /// </summary>
        /// <typeparam name="T">
        /// <ja>変換したいオブジェクトの型</ja>
        /// <en>The type of the object you want to convert</en>
        /// </typeparam>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="queryString">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="urlParam">
        /// <ja>URLパラメータ</ja>
        /// <en>URL parameters</en>
        /// </param>
        /// <returns>
        /// <ja>オブジェクト</ja>
        /// <en>Object</en>
        /// </returns>
        public T ExecuteApiToObject<T>(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam) where T : class
        {
            apiAction.Initialize();
            EditParam(contents, queryString, urlParam);
            var apiResult = apiAction.ExecuteAction();
            if (apiResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return apiResult.Content.DeserializeObject<T>();
            }
            return null;
        }

        /// <summary>
        /// URLにクエリストリングが含まれている場合に分割する
        /// </summary>
        /// <param name="url"></param>
        /// <param name="querystring"></param>
        private void CorrectQueryString(ref string url, ref string querystring)
        {
            if (url.Contains('?'))
            {
                string[] tmp = url.Split('?');
                if (tmp?.Length == 2)
                {
                    url = tmp[0];
                    querystring = string.Format("?{0}", tmp[1]);
                }
            }
            else if (string.IsNullOrEmpty(querystring) == false && querystring.StartsWith("?") == false)
            {
                querystring = $"?{querystring}";
            }
        }


        /// <summary>
        /// <ja>他のURLのAPIを呼び出します。(Getメソッド)</ja>
        /// <en>Executes GET API at other URL.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>Response</en>
        /// </returns>
        public HttpResponseMessage ExecuteGetApi(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var oldFlag = perRequestDataContainer.IsInternalCall;
            var oldKey = perRequestDataContainer.InternalCallKeyword;

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                perRequestDataContainer.MergeRequestHeaders(this.HeaderDictionaryDeepCopy(headers));
            }
            perRequestDataContainer.IsInternalCall = true;
            perRequestDataContainer.InternalCallKeyword = internalCallKeyword;

            CorrectQueryString(ref url, ref querystring);
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            var result = api.Request(new DynamicApiRequestModel { HttpMethod = "GET", Contents = contents, MediaType = MEDIATYPE_JSON, QueryString = querystring, RelativeUri = url });

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }
            perRequestDataContainer.IsInternalCall = oldFlag;
            perRequestDataContainer.InternalCallKeyword = oldKey;

            return result.ToHttpResponseMessage();
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します(Getメソッド)。これは非同期です</ja>
        /// <en>Executes GET API at other URL. This is asynchronous.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果のTASK</ja>
        /// <en>Task of Response</en>
        /// </returns>
        public Task<HttpResponseMessage> ExecuteGetApiAsync(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteXXXApiAsyncExecutor.NewSync(httpContextAccessor.HttpContext, ExecuteGetApi);
            return executor.ExecuteAsync(url, contents, querystring, internalCallKeyword, headers);
        }


        /// <summary>
        /// <ja>他のURLのAPIを呼び出し戻り値をオブジェクトに変換します。(Getメソッド)</ja>
        /// <en>Executes GET API at other URL and converts the data returned into an object.</en>
        /// </summary>
        /// <typeparam name="T">
        /// <ja>変換したいオブジェクトの型</ja>
        /// <en>The type of the object you want to convert</en>
        /// </typeparam>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>オブジェクト</ja>
        /// <en>Object</en>
        /// </returns>
        public T ExecuteGetApiToObject<T>(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null) where T : class
        {
            CorrectQueryString(ref url, ref querystring);
            var apiResult = ExecuteGetApi(url, contents, querystring, internalCallKeyword, headers);
            if (apiResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return apiResult.Content.DeserializeObject<T>();
            }
            return null;
        }


        public JToken ExecuteGetApiToJToken(string url, string contents = null, string querystring = null)
        {
            CorrectQueryString(ref url, ref querystring);
            var apiResult = ExecuteGetApi(url, contents, querystring);
            if (apiResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JToken.FromObject(JsonHelper.ToJson(apiResult.Content.ReadAsStringAsync().Result));
            }
            return null;
        }


        public Tuple<HttpStatusCode, JToken> ExecuteGetApiToJTokenAndHttpStatusCode(string url, string contents = null, string querystring = null)
        {
            CorrectQueryString(ref url, ref querystring);
            var apiResult = ExecuteGetApi(url, contents, querystring);
            JToken data = null;
            if (apiResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                data = JToken.FromObject(JsonHelper.ToJson(apiResult.Content.ReadAsStringAsync().Result));
            }
            return Tuple.Create(apiResult.StatusCode, data);
        }


        /// <summary>
        /// <ja>他のURLのAPIを呼び出します。(POSTメソッド)</ja>
        /// <en>Executes POST API at other URL.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>Response</en>
        /// </returns>
        public HttpResponseMessage ExecutePostApi(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var oldFlag = perRequestDataContainer.IsInternalCall;
            var oldKey = perRequestDataContainer.InternalCallKeyword;
            var mediaType = MEDIATYPE_JSON;

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                var deepCopyHeaders = this.HeaderDictionaryDeepCopy(headers);
                perRequestDataContainer.MergeRequestHeaders(deepCopyHeaders);
                if (deepCopyHeaders.TryGetValue("Content-Type", out var contentType))
                {
                    mediaType = contentType?.First() ?? MEDIATYPE_JSON;
                }
            }
            perRequestDataContainer.IsInternalCall = true;
            perRequestDataContainer.InternalCallKeyword = internalCallKeyword;

            CorrectQueryString(ref url, ref querystring);
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            var result = api.Request(new DynamicApiRequestModel { HttpMethod = "POST", Contents = contents, MediaType = mediaType, QueryString = querystring, RelativeUri = url });

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }
            perRequestDataContainer.IsInternalCall = oldFlag;
            perRequestDataContainer.InternalCallKeyword = oldKey;

            return result.ToHttpResponseMessage();
        }

        /// <summary>
        /// ContentsがStreamの、POSTAPI
        /// （Roslynから呼ばれたくないのでInternal)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contents"></param>
        /// <param name="querystring"></param>
        /// <param name="internalCallKeyword"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal HttpResponseMessage ExecutePostApi(string url, Stream contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var oldFlag = perRequestDataContainer.IsInternalCall;
            var oldKey = perRequestDataContainer.InternalCallKeyword;
            var mediaType = MEDIATYPE_JSON;

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                var deepCopyHeaders = this.HeaderDictionaryDeepCopy(headers);
                perRequestDataContainer.MergeRequestHeaders(deepCopyHeaders);
                if (deepCopyHeaders.TryGetValue("Content-Type", out var contentType))
                {
                    mediaType = contentType?.First() ?? MEDIATYPE_JSON;
                }
            }
            perRequestDataContainer.IsInternalCall = true;
            perRequestDataContainer.InternalCallKeyword = internalCallKeyword;

            CorrectQueryString(ref url, ref querystring);
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            var result = api.Request(new DynamicApiRequestModel { HttpMethod = "POST", ContentsStream = contents, Contents = null, MediaType = mediaType, QueryString = querystring, RelativeUri = url });

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }
            perRequestDataContainer.IsInternalCall = oldFlag;
            perRequestDataContainer.InternalCallKeyword = oldKey;

            return result.ToHttpResponseMessage();
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します(POSTメソッド)。これは非同期です</ja>
        /// <en>Executes POST API at other URL. This is asynchronous.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果のTASK</ja>
        /// <en>Task of Response</en>
        /// </returns>
        public Task<HttpResponseMessage> ExecutePostApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteXXXApiAsyncExecutor.NewSync(httpContextAccessor.HttpContext, ExecutePostApi);
            return executor.ExecuteAsync(url, contents, querystring, internalCallKeyword, headers);
        }

        /// <summary>
        /// 非同期版：ContentsがStreamの、POSTAPI
        /// （Roslynから呼ばれたくないのでInternal)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="contents"></param>
        /// <param name="querystring"></param>
        /// <param name="internalCallKeyword"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        internal Task<HttpResponseMessage> ExecutePostApiAsync(string url, Stream contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteXXXApiAsyncExecutorWithStream.NewSync(httpContextAccessor.HttpContext, ExecutePostApi);
            return executor.ExecuteAsync(url, contents, querystring, internalCallKeyword, headers);
        }


        /// <summary>
        /// <ja>他のURLのAPIを呼び出します。(PUTメソッド)</ja>
        /// <en>Executes PUT API at other URL.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>Response</en>
        /// </returns>
        public HttpResponseMessage ExecutePutApi(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var oldFlag = perRequestDataContainer.IsInternalCall;
            var oldKey = perRequestDataContainer.InternalCallKeyword;

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                perRequestDataContainer.MergeRequestHeaders(this.HeaderDictionaryDeepCopy(headers));
            }
            perRequestDataContainer.IsInternalCall = true;
            perRequestDataContainer.InternalCallKeyword = internalCallKeyword;

            CorrectQueryString(ref url, ref querystring);
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            var result = api.Request(new DynamicApiRequestModel { HttpMethod = "PUT", Contents = contents, MediaType = MEDIATYPE_JSON, QueryString = querystring, RelativeUri = url });

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }
            perRequestDataContainer.IsInternalCall = oldFlag;
            perRequestDataContainer.InternalCallKeyword = oldKey;

            return result.ToHttpResponseMessage();
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します(PUTメソッド)。これは非同期です</ja>
        /// <en>Executes PUT API at other URL. This is asynchronous.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果のTASK</ja>
        /// <en>Task of Response</en>
        /// </returns>
        public Task<HttpResponseMessage> ExecutePutApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteXXXApiAsyncExecutor.NewSync(httpContextAccessor.HttpContext, ExecutePutApi);
            return executor.ExecuteAsync(url, contents, querystring, internalCallKeyword, headers);
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します。(DELETEメソッド)</ja>
        /// <en>Executes DELETE API at other URL.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>Response</en>
        /// </returns>
        public HttpResponseMessage ExecuteDeleteApi(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var oldFlag = perRequestDataContainer.IsInternalCall;
            var oldKey = perRequestDataContainer.InternalCallKeyword;

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                perRequestDataContainer.MergeRequestHeaders(this.HeaderDictionaryDeepCopy(headers));
            }
            perRequestDataContainer.IsInternalCall = true;
            perRequestDataContainer.InternalCallKeyword = internalCallKeyword;

            CorrectQueryString(ref url, ref querystring);
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            var result = api.Request(new DynamicApiRequestModel { HttpMethod = "DELETE", Contents = contents, MediaType = MEDIATYPE_JSON, QueryString = querystring, RelativeUri = url });

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }
            perRequestDataContainer.IsInternalCall = oldFlag;
            perRequestDataContainer.InternalCallKeyword = oldKey;

            return result.ToHttpResponseMessage();
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します(DELETEメソッド)。これは非同期です</ja>
        /// <en>Executes DELETE API at other URL. This is asynchronous.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果のTASK</ja>
        /// <en>Task of Response</en>
        /// </returns>
        public Task<HttpResponseMessage> ExecuteDeleteApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteXXXApiAsyncExecutor.NewSync(httpContextAccessor.HttpContext, ExecuteDeleteApi);
            return executor.ExecuteAsync(url, contents, querystring, internalCallKeyword, headers);
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します(PATCHメソッド)</ja>
        /// <en>Executes PATCH API at other URL.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果</ja>
        /// <en>Response</en>
        /// </returns>
        public HttpResponseMessage ExecutePatchApi(string url, string contents = null, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            var oldFlag = perRequestDataContainer.IsInternalCall;
            var oldKey = perRequestDataContainer.InternalCallKeyword;

            // ヘッダーが指定されている場合はマージ
            Dictionary<string, List<string>> oldHeaders = null;
            if (headers != null)
            {
                oldHeaders = this.HeaderDictionaryDeepCopy(perRequestDataContainer.RequestHeaders);
                perRequestDataContainer.MergeRequestHeaders(this.HeaderDictionaryDeepCopy(headers));
            }
            perRequestDataContainer.IsInternalCall = true;
            perRequestDataContainer.InternalCallKeyword = internalCallKeyword;

            CorrectQueryString(ref url, ref querystring);
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            var result = api.Request(new DynamicApiRequestModel { HttpMethod = "PATCH", Contents = contents, MediaType = MEDIATYPE_JSON, QueryString = querystring, RelativeUri = url });

            // 元のヘッダーを復元
            if (headers != null)
            {
                perRequestDataContainer.RequestHeaders = oldHeaders;
            }
            perRequestDataContainer.IsInternalCall = oldFlag;
            perRequestDataContainer.InternalCallKeyword = oldKey;

            return result.ToHttpResponseMessage();
        }

        /// <summary>
        /// <ja>他のURLのAPIを呼び出します(PATCHメソッド)。これは非同期です</ja>
        /// <en>Executes PATCH API at other URL. This is asynchronous.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>URL</ja>
        /// <en>URL</en>
        /// </param>
        /// <param name="contents">
        /// <ja>Body(JSON形式)</ja>
        /// <en>Body (JSON format)</en>
        /// </param>
        /// <param name="querystring">
        /// <ja>Query文字</ja>
        /// <en>Query string</en>
        /// </param>
        /// <param name="internalCallKeyword">
        /// <ja>内部呼び出しキーワード</ja>
        /// <en>Internal call keyword</en>
        /// </param>
        /// <param name="headers">
        /// <ja>Requestヘッダー</ja>
        /// <en>Request headers</en>
        /// </param>
        /// <returns>
        /// <ja>実行結果のTASK</ja>
        /// <en>Task of Response</en>
        /// </returns>
        public Task<HttpResponseMessage> ExecutePatchApiAsync(string url, string contents, string querystring = null, string internalCallKeyword = null, Dictionary<string, List<string>> headers = null)
        {
            var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
            var executor = ExecuteXXXApiAsyncExecutor.NewSync(httpContextAccessor.HttpContext, ExecutePatchApi);
            return executor.ExecuteAsync(url, contents, querystring, internalCallKeyword, headers);
        }

        private Dictionary<string, string> ConvertOriginalQuery()
        {
            var ret = new Dictionary<string, string>();
            //DynamicApiReposityでなぜかURLのKEYVALUEをQueryStringに入れてしまっている。そこで外す。
            if (apiAction.Query != null)
            {
                foreach (var query in apiAction.Query.Dic)
                {
                    if (!apiAction.KeyValue.Dic.Any(x => x.Key.Value == query.Key.Value))
                    {
                        ret.Add(query.Key.Value, query.Value.Value);
                    }
                }
            }
            return ret;
        }

        private void EditParam(string contents, Dictionary<string, string> queryString, Dictionary<string, string> urlParam)
        {
            var origanlQuery = ConvertOriginalQuery();
            var queryValueObject = new Dictionary<QueryStringKey, QueryStringValue>();
            var urlValueObject = new Dictionary<UrlParameterKey, UrlParameterValue>();
            if (queryString != null)
            {
                foreach (var query in queryString)
                {
                    if (origanlQuery.ContainsKey(query.Key))
                    {
                        queryValueObject.Add(new QueryStringKey(query.Key), new QueryStringValue(query.Value));
                    }
                    else
                    {
                        throw new Exception($"Invalid QueryString Key {query.Key}");
                    }
                }
            }
            if (urlParam != null)
            {
                foreach (var urlkey in urlParam)
                {
                    if (apiAction.KeyValue.Dic.Any(x => x.Key.Value == urlkey.Key))
                    {
                        urlValueObject.Add(new UrlParameterKey(urlkey.Key), new UrlParameterValue(urlkey.Value));
                        queryValueObject.Add(new QueryStringKey(urlkey.Key), new QueryStringValue(urlkey.Value));
                    }
                    else
                    {
                        throw new Exception($"Invalid UrlParam Key {urlkey.Key}");
                    }

                }
            }

            if (queryValueObject.Any())
            {
                apiAction.Query = new QueryStringVO(queryValueObject);
            }
            if (urlValueObject.Any())
            {
                apiAction.KeyValue = new UrlParameter(urlValueObject);
            }
            apiAction.Contents = new Contents(contents);
        }
        #endregion

        #region モデル取得関係
        /// <summary>
        /// リソース相対URLから、リソースのJsonSchemaを取得する
        /// </summary>
        /// <param name="url">リソースURL</param>
        /// <returns>JsonSchema文字列</returns>
        public string GetJsonSchemaByResourceUrl(string url)
        {
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            return api.GetControllerSchemaByUrl(url);
        }

        /// <summary>
        /// モデル名から、リソースのJsonSchemaを取得する
        /// </summary>
        /// <param name="name">モデル名</param>
        /// <returns>JsonSchema文字列</returns>
        public string GetJsonSchemaByModelName(string name)
        {
            var api = UnityCore.Resolve<IDynamicApiInterface>();
            return api.GetSchemaModelByName(name);
        }
        #endregion

        #region Jsonバリデーション関連
        /// <summary>
        /// <ja>ApiのリクエストモデルでJsonSchemaのバリデーションを実行します</ja>
        /// <en>Validation with json schema configured with api.</en>
        /// </summary>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public IEnumerable<Rfc7807> ValidateWithRequestModel(string input) => ValidateJson(input, apiAction?.RequestSchema?.ToJSchema(), () => "There is no request model.");

        /// <summary>
        /// <ja>ValidateWithRequestModel()の結果からHttpResponseMessageを生成します</ja>
        /// <en>Generates HttpResponseMessage from ValidateWithRequestModel() result.</en>
        /// </summary>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public HttpResponseMessage CreateHttpResponseFromValidateWithRequestModel(string input) => CreateHttpResponseFromValidate(ValidateWithRequestModel(input));

        /// <summary>
        /// <ja>ApiのレスポンスモデルでJsonSchemaのバリデーションを実行します</ja>
        /// <en>Validation with json schema configured with api.</en>
        /// </summary>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public IEnumerable<Rfc7807> ValidateWithResponseModel(string input) => ValidateJson(input, apiAction?.ResponseSchema?.ToJSchema(), () => "There is no response model.");

        /// <summary>
        /// <ja>ValidateWithResponseModel()の結果からHttpResponseMessageを生成します</ja>
        /// <en>Generates HttpResponseMessage from ValidateWithResponseModel() result.</en>
        /// </summary>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public HttpResponseMessage CreateHttpResponseFromValidateWithResponseModel(string input) => CreateHttpResponseFromValidate(ValidateWithResponseModel(input));

        /// <summary>
        /// <ja>リソースのモデルでJsonSchemaのバリデーションを実行します</ja>
        /// <en>Validation with json schema configured with resource model.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>リソースのURL</ja>
        /// <en>URL of the resource</en>
        /// </param>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public IEnumerable<Rfc7807> ValidateWithModelByResourceUrl(string url, string input)
            => ValidateJson(input, JSchema.Parse(GetJsonSchemaByResourceUrl(url)), () => "No resource found matching url.");

        /// <summary>
        /// <ja>URL付きValidateWithRequestModel()の結果からHttpResponseMessageを生成します</ja>
        /// <en>Generates HttpResponseMessage from ValidateWithRequestModel(url) result.</en>
        /// </summary>
        /// <param name="url">
        /// <ja>リソースのURL</ja>
        /// <en>URL of the resource</en>
        /// </param>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public HttpResponseMessage CreateHttpResponseFromValidateWithModelByResourceUrl(string url, string input) => CreateHttpResponseFromValidate(ValidateWithModelByResourceUrl(url, input));

        /// <summary>
        /// <ja>リソースのモデルでJsonSchemaのバリデーションを実行します</ja>
        /// <en>Validation with json schema configured with resource model.</en>
        /// </summary>
        /// <param name="name">
        /// <ja>モデル名</ja>
        /// <en>name of model</en>
        /// </param>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public IEnumerable<Rfc7807> ValidateWithModelByModelName(string name, string input)
            => ValidateJson(input, JSchema.Parse(GetJsonSchemaByModelName(name)), () => "No data model found matching name.");

        /// <summary>
        /// <ja>URL付きValidateWithRequestModel()の結果からHttpResponseMessageを生成します</ja>
        /// <en>Generates HttpResponseMessage from ValidateWithRequestModel(url) result.</en>
        /// </summary>
        /// <param name="name">
        /// <ja>モデル名</ja>
        /// <en>name of model</en>
        /// </param>
        /// <param name="input">
        /// <ja>Json文字列</ja>
        /// <en>Json string</en>
        /// </param>
        /// <returns>
        /// <ja>バリデーション結果</ja>
        /// <en>Validation result</en>
        /// </returns>
        public HttpResponseMessage CreateHttpResponseFromValidateWithModelByModelName(string name, string input) => CreateHttpResponseFromValidate(ValidateWithModelByModelName(name, input));

        private IEnumerable<Rfc7807> ValidateJson(string input, JSchema schema, Func<string> noschema_errormessage)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (schema == null)
            {
                throw new ArgumentNullException(noschema_errormessage == null ? nameof(schema) : noschema_errormessage());
            }

            // JSON形式でなければBadRequest
            JToken json;
            try
            {
                json = input.ToJson();
            }
            catch (JsonReaderException ex)
            {
                var error = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10402, apiAction?.RelativeUri?.Value);
                error.Errors = new Dictionary<string, dynamic>() { { "Unparsable", new List<string>() { ex.Message } } };
                return Mapper.Map<List<Rfc7807>>(new List<RFC7807ProblemDetailExtendErrors>() { error });
            }

            // JsonSchemaバリデーション
            var result = new List<RFC7807ProblemDetailExtendErrors>();
            IList<ValidationError> jsonErrors;

            if (json is JArray jArray && apiAction.PostDataType?.IsArray == true)
            {
                if (ThresholdJsonSchemaValidaitonParallelize <= jArray.Count)
                {
                    var httpContext = UnityCore.Resolve<IHttpContextAccessor>().HttpContext;
                    var dataContainer = UnityCore.Resolve<IPerRequestDataContainer>();

                    ParallelExtension.ForEachWithContextAndPartitiner(jArray, i =>
                    {
                        if (!jArray[i].IsValid(schema, out jsonErrors))
                        {
                            EditJsonSchemaErrorMessage(jsonErrors, ref result);
                        }
                    }, httpContext, dataContainer);
                }
                else
                {
                    jArray.ForEach(x =>
                    {
                        if (!x.IsValid(schema, out jsonErrors))
                        {
                            EditJsonSchemaErrorMessage(jsonErrors, ref result);
                        }
                    });
                }

            }
            else
            {
                if (!json.IsValid(schema, out jsonErrors))
                {
                    EditJsonSchemaErrorMessage(jsonErrors, ref result);
                }

            }
            if (!result.Any())
            {
                return null;
            }
            return Mapper.Map<List<Rfc7807>>(result);
        }

        private void EditJsonSchemaErrorMessage(IList<ValidationError> jsonErrors, ref List<RFC7807ProblemDetailExtendErrors> validationErros)
        {
            Dictionary<string, dynamic> problems = new Dictionary<string, dynamic>();
            var vals = problems;
            jsonErrors.ForEach(x =>
            {
                string prop = string.Empty;
                if (string.IsNullOrEmpty(x.Path) && x.Value == null)
                {
                    prop = "RootInvalid";
                }
                else
                {
                    //プロパティ設定のエラー以外の場合（required)
                    //配列で必須エラーの時は配列のインデックスが指定される
                    if (string.IsNullOrEmpty(x.Path) || Regex.IsMatch(x.Path, @"^\[\d+\]$"))
                    {
                        var errProps = (List<string>)x.Value;
                        for (int i = 0; i < errProps.Count; i++)
                        {
                            prop += errProps[i];
                            if (i != (errProps.Count - 1))
                            {
                                prop += ",";
                            }
                        }
                    }
                    //プロパティ設定のエラーの場合
                    else
                    {
                        if (string.IsNullOrWhiteSpace(x.Path))
                        {
                            prop = x.Path;
                        }
                        else
                        {
                            var bracketPos = x.Path.LastIndexOf("].");
                            prop = bracketPos > -1 ? x.Path.Substring(bracketPos + 2, x.Path.Length - bracketPos - 2) : x.Path;
                        }
                    }
                }

                //エラーリストに当該プロパティが存在の場合
                if (vals.ContainsKey(prop))
                {
                    var lst = vals[prop];
                    lst.Add($@"{x.Message}(code:{(int)x.ErrorType})");
                }
                //エラーリストに当該プロパティが非存在の場合
                else
                {
                    vals.Add(prop, new List<string>() { $@"{x.Message}(code:{(int)x.ErrorType})" });
                }
            });

            var error = ErrorCodeMessage.GetRFC7807(ErrorCodeMessage.Code.E10402, apiAction?.RelativeUri?.Value);
            if (ReturnJsonValidatorErrorDetail || PerRequestDataContainer.ReturnNeedsJsonValidatorErrorDetail)
            {
                error.Errors = problems;
            }
            validationErros.Add(error);
        }

        /// <summary>
        /// CreateHttpResponseFromValidateの共通処理　Rfc7807のリストからHttpResponseMessageを作成する
        /// </summary>
        /// <returns></returns>
        private HttpResponseMessage CreateHttpResponseFromValidate(IEnumerable<Rfc7807> rfc7807List)
        {
            if (rfc7807List?.Any() == true)
            {
                var body = rfc7807List.Count() == 1
                    ? JsonConvert.SerializeObject(rfc7807List.Single())
                    : JsonConvert.SerializeObject(rfc7807List);
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(body, Encoding.UTF8, MEDIATYPE_ProblemJson)
                };
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        #endregion

        private Dictionary<string, List<string>> HeaderDictionaryDeepCopy(Dictionary<string, List<string>> headers)
        {
            return headers?.ToDictionary(x => x.Key, x => new List<string>(x.Value));
        }
    }

    internal class ExecuteApiAsyncExecutor : AsyncFuncExecutor<string, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, List<string>>, HttpResponseMessage>
    {
        internal static ExecuteApiAsyncExecutor NewSync(Func<string, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, List<string>>, HttpResponseMessage> action)
        {
            return new ExecuteApiAsyncExecutor(action.ToAsync());
        }

        internal static ExecuteApiAsyncExecutor NewSync(HttpContext httpContext, Func<string, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, List<string>>, HttpResponseMessage> action)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            return new ExecuteApiAsyncExecutor(action.ToAsync(() => httpContext.Switch(perRequestDataContainer)));
        }

        public ExecuteApiAsyncExecutor(Func<string, Dictionary<string, string>, Dictionary<string, string>, Dictionary<string, List<string>>, Task<HttpResponseMessage>> asyncAction)
            : base(asyncAction)
        {
        }

        public new Task<HttpResponseMessage> ExecuteAsync(string arg1, Dictionary<string, string> arg2, Dictionary<string, string> arg3, Dictionary<string, List<string>> arg4)
        {
            ;
            return m_AsyncAction(arg1, arg2, arg3, arg4);
        }
    }

    internal class ExecuteXXXApiAsyncExecutor : AsyncFuncExecutor<string, string, string, string, Dictionary<string, List<string>>, HttpResponseMessage>
    {
        internal static ExecuteXXXApiAsyncExecutor NewSync(Func<string, string, string, string, Dictionary<string, List<string>>, HttpResponseMessage> action)
        {
            return new ExecuteXXXApiAsyncExecutor(action.ToAsync());
        }

        internal static ExecuteXXXApiAsyncExecutor NewSync(HttpContext httpContext, Func<string, string, string, string, Dictionary<string, List<string>>, HttpResponseMessage> action)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            return new ExecuteXXXApiAsyncExecutor(action.ToAsync(() => httpContext.Switch(perRequestDataContainer)));
        }

        public ExecuteXXXApiAsyncExecutor(Func<string, string, string, string, Dictionary<string, List<string>>, Task<HttpResponseMessage>> asyncAction)
            : base(asyncAction)
        {
        }
    }

    internal class ExecuteXXXApiAsyncExecutorWithStream : AsyncFuncExecutor<string, Stream, string, string, Dictionary<string, List<string>>, HttpResponseMessage>
    {
        internal static ExecuteXXXApiAsyncExecutorWithStream NewSync(Func<string, Stream, string, string, Dictionary<string, List<string>>, HttpResponseMessage> action)
        {
            return new ExecuteXXXApiAsyncExecutorWithStream(action.ToAsync());
        }

        internal static ExecuteXXXApiAsyncExecutorWithStream NewSync(HttpContext httpContext, Func<string, Stream, string, string, Dictionary<string, List<string>>, HttpResponseMessage> action)
        {
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            return new ExecuteXXXApiAsyncExecutorWithStream(action.ToAsync(() => httpContext.Switch(perRequestDataContainer)));
        }

        public ExecuteXXXApiAsyncExecutorWithStream(Func<string, Stream, string, string, Dictionary<string, List<string>>, Task<HttpResponseMessage>> asyncAction)
            : base(asyncAction)
        {
        }
    }

    internal static class ApiHelperContextSwitch
    {
        public static void Switch(this HttpContext src, IPerRequestDataContainer original)
        {
            // NIY: .Net6でHttpContextの差し替えは可能なのか？
            //src.Switch();
            //original.DeepCopy(UnityCore.Resolve<IPerRequestDataContainer>());
        }
    }
}
