namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class HtmlHelper
    {
        public HtmlHelper(HttpClient client)
        {
        }

        public IEnumerable<string> SelectHtmlNodesById(string url, string id, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
            => null;

        public IEnumerable<string> SelectHtmlNodesByClassName(string url, string ClassName, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
            => null;

        public IEnumerable<string> SelectHtmlNodesByQuery(string url, string xpath, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
            => null;
    }
}
