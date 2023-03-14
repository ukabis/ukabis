using System.Text;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class ExternalSourceHelper
    {
        public ExternalSourceHelper(HttpClient client)
        {
        }

        public class ParseConfiguration
        {
            public bool HasHeaderRecord { get; set; } = true;
            public Encoding Encoding { get; set; } = Encoding.UTF8;
            public bool IgnoreBlankLines { get; set; } = true;
            public bool DetectColumnCountChanges { get; set; } = true;
            public char Comment { get; set; } = '#';
        }

        public IEnumerable<IDictionary<string, object>> ParseCsvFromUrlToDictionary(string url, ParseConfiguration config = null, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
            => null;

        public IEnumerable<T> ParseCsvFromUrlToObject<T>(string url, Func<IDictionary<string, object>, T> mapper = null, ParseConfiguration config = null, Func<IDictionary<string, object>, Boolean> validator = null, string BasicAuthenticationId = null, string BasicAuthenticationPassword = null)
            => null;
    }
}
