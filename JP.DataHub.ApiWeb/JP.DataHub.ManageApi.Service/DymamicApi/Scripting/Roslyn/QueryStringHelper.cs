namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public static class QueryStringHelper
    {
        public static KeyValueResult<string> Find(this Dictionary<string, string> dic, string key)
            => null;

        public static KeyValueResult<T> Find<T>(this Dictionary<string, string> dic, string key)
            => null;
    }
}