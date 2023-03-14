using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using JP.DataHub.Com.Unity;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
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
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public static class Rfc7807Helper
    {
        public static Rfc7807 ToRfc7807(string errorCode, HttpStatusCode httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => null;

        public static Rfc7807 ToRfc7807(string errorCode, int httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => null;

        public static string ToString(string errorCode, HttpStatusCode httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => null;

        public static string ToString(string errorCode, int httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => null;

        public static HttpResponseMessage ToResponseMessage(string errorCode, HttpStatusCode httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => null;

        public static HttpResponseMessage ToResponseMessage(string errorCode, int httpStatusCode, string Title, string Detail, string Instance = null, Dictionary<string, IEnumerable<string>> errors = null)
            => null;
    }
}
