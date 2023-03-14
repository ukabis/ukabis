namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class CacheHelper
    {
        public void Add(string key, object value)
        {
        }

        public void Add(string key, object value, TimeSpan expiration)
        {
        }

        public T Get<T>(string key)
            => default(T);


        public T GetOrAdd<T>(string key, TimeSpan expiration, Func<T> misshitAction)
            => default(T);

        public void Remove(string key)
        {
        }
    }
}