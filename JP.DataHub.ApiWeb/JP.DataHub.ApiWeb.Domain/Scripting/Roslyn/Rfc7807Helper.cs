using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// RFC7807に準拠したクラス
    /// </summary>
    [JsonObject]
    public class Rfc7807
    {
        /// <summary>
        /// エラーコード
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string error_code { get; set; }
        /// <summary>
        /// HTTPステータスコード
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int status { get; set; }
        /// <summary>
        /// エラーのタイトル
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }
        /// <summary>
        /// エラーの詳細
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string detail { get; set; }
        /// <summary>
        /// エラーの発生場所
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string instance { get; set; }
        /// <summary>
        /// 内包されるエラー情報
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, IEnumerable<string>> errors { get; set; }
    }

    /// <summary>
    /// RFC7807エラーのためのヘルパークラス
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public static class Rfc7807Helper
    {
        /// <summary>
        /// RFC7807クラスへの変換
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <param name="httpStatusCode">HTTP ステータスコード</param>
        /// <param name="Title">エラーのタイトル</param>
        /// <param name="Detail">エラーの詳細</param>
        /// <param name="Instance">問題の発生場所</param>
        /// <param name="errors">内包されるエラー情報</param>
        /// <returns>RFC7807形式のクラスを返す</returns>
        public static Rfc7807 ToRfc7807(string errorCode, HttpStatusCode httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => ToRfc7807(errorCode, (int)httpStatusCode, Title, Detail, Instance, errors);

        /// <summary>
        /// RFC7807クラスへの変換
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <param name="httpStatusCode">HTTP ステータスコード</param>
        /// <param name="Title">エラーのタイトル</param>
        /// <param name="Detail">エラーの詳細</param>
        /// <param name="Instance">問題の発生場所</param>
        /// <param name="errors">内包されるエラー情報</param>
        /// <returns>RFC7807形式のクラスを返す</returns>
        public static Rfc7807 ToRfc7807(string errorCode, int httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
        {
            if (Instance == null)
            {
                var httpContextAccessor = UnityCore.Resolve<IHttpContextAccessor>();
                Instance = httpContextAccessor?.HttpContext?.Request?.Path.Value;
            }
            return new Rfc7807() { error_code = errorCode, status = httpStatusCode, title = Title, detail = Detail, instance = Instance, errors = errors };
        }

        /// <summary>
        /// RFC7807形式(json)への変換
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <param name="httpStatusCode">HTTP ステータスコード</param>
        /// <param name="Title">エラーのタイトル</param>
        /// <param name="Detail">エラーの詳細</param>
        /// <param name="Instance">問題の発生場所</param>
        /// <param name="errors">内包されるエラー情報</param>
        /// <returns>RFC7807のjson形式の文字列を返す</returns>
        public static string ToString(string errorCode, HttpStatusCode httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => ToString(errorCode, (int)httpStatusCode, Title, Detail, Instance, errors);

        /// <summary>
        /// RFC7807形式(json)への変換
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <param name="httpStatusCode">HTTP ステータスコード</param>
        /// <param name="Title">エラーのタイトル</param>
        /// <param name="Detail">エラーの詳細</param>
        /// <param name="Instance">問題の発生場所</param>
        /// <param name="errors">内包されるエラー情報</param>
        /// <returns>RFC7807のjson形式の文字列を返す</returns>
        public static string ToString(string errorCode, int httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
        {
            return JsonHelper.ToJsonString(ToRfc7807(errorCode, httpStatusCode, Title, Detail, Instance, errors));
        }

        /// <summary>
        /// RFC7807形式に準拠したResponseMessageへの変換
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <param name="httpStatusCode">HTTP ステータスコード</param>
        /// <param name="Title">エラーのタイトル</param>
        /// <param name="Detail">エラーの詳細</param>
        /// <param name="Instance">問題の発生場所</param>
        /// <param name="errors">内包されるエラー情報</param>
        /// <returns>RFC7807に準拠したHttpResponseMessageを返す</returns>
        public static HttpResponseMessage ToResponseMessage(string errorCode, HttpStatusCode httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => ToResponseMessage(errorCode, (int)httpStatusCode, Title, Detail, Instance, errors);

        /// <summary>
        /// RFC7807形式に準拠したResponseMessageへの変換
        /// </summary>
        /// <param name="errorCode">エラーコード</param>
        /// <param name="httpStatusCode">HTTP ステータスコード</param>
        /// <param name="Title">エラーのタイトル</param>
        /// <param name="Detail">エラーの詳細</param>
        /// <param name="Instance">問題の発生場所</param>
        /// <param name="errors">内包されるエラー情報</param>
        /// <returns>RFC7807に準拠したHttpResponseMessageを返す</returns>
        public static HttpResponseMessage ToResponseMessage(string errorCode, int httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
        {
            return HttpResponseHelper.CreateJsonResponse(ToRfc7807(errorCode, httpStatusCode, Title, Detail, Instance, errors), (HttpStatusCode)httpStatusCode);
        }
    }
}
