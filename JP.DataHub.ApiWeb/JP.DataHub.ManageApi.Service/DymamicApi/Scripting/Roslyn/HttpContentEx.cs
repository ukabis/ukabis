namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    internal static class HttpContentEx
    {
        public static T DeserializeObject<T>(this HttpContent content) where T : class
            => null;

        public static object DeserializeObject(this HttpContent content)
            => null;
    }
}
