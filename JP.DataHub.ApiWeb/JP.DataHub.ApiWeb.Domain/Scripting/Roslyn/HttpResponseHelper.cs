using System.Text;
using System.Net;
using Newtonsoft.Json;
using JP.DataHub.ApiWeb.Domain.Scripting.Attributes;

namespace JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    /// <summary>
    /// クライアントへ返却するHttpResponseMessageを作成するためのヘルパークラス
    /// 構文チェック用のダミークラスが[JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn]に存在します。
    /// パブリックメソッドを追加・削除・変更する場合はダミークラスも同様にしてください。
    /// </summary>
    [RoslynScriptHelp]
    public class HttpResponseHelper
    {
        private static string MEDIATYPE_APPLICATIONJSON = "application/json";

        /// <summary>
        /// <ja>Contents(文字列)をUtf-8エンコードでHttpResponseMessageを作成します。</ja>
        /// <en>Creates the Content of the HttpResponseMessage with UTF-8 encoding.</en>
        /// </summary>
        /// <param name="content">
        /// <ja>コンテンツ (JSONやXMLの変換は呼び元で行うこと)</ja>
        /// <en>Content (Conversion to JSON/XML is done on the caller's end)</en>
        /// </param>
        /// <param name="code">
        /// <ja>HttpStatusコード</ja>
        /// <en>Http status code</en>
        /// </param>
        /// <param name="mediaType">
        /// <ja>メディアタイプ</ja>
        /// <en>Media type</en>
        /// </param>
        /// <returns>
        /// <ja>HttpResponseMessage</ja>
        /// <en>HttpResponseMessage</en>
        /// </returns>
        public static HttpResponseMessage Create(string content, HttpStatusCode code, string mediaType = null)
        {
            return new HttpResponseMessage(code) { Content = new StringContent(content, Encoding.UTF8, mediaType ?? MEDIATYPE_APPLICATIONJSON) };
        }

        /// <summary>
        /// <ja>オブジェクトをJsonに変換してHttpResponseMessageを作成します。</ja>
        /// <en>Converts an object to JSON and creates an HttpResponseMessage.</en>
        /// </summary>
        /// <param name="data">
        /// <ja>Jsonに変換するオブジェクト</ja>
        /// <en>Object to convert to JSON</en>
        /// </param>
        /// <param name="code">
        /// <ja>HttpStatusコード</ja>
        /// <en>Http status code</en>
        /// </param>
        /// <returns>
        /// <ja>HttpResponseMessage</ja>
        /// <en>HttpResponseMessage</en>
        /// </returns>
        public static HttpResponseMessage CreateJsonResponse(object data, HttpStatusCode code)
        {
            var retContent = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, MEDIATYPE_APPLICATIONJSON);
            return new HttpResponseMessage(code) { Content = retContent };
        }
    }
}
