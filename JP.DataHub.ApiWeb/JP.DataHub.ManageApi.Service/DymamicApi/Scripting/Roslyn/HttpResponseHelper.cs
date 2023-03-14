using System.Net;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class HttpResponseHelper
    {
        public static HttpResponseMessage Create(string content, HttpStatusCode code, string mediaType = null)
            => null;

        public static HttpResponseMessage CreateJsonResponse(object data, HttpStatusCode code)
            => null;
    }
}
