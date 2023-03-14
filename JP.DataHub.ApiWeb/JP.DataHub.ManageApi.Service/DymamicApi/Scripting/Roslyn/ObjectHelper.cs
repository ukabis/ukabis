namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public static class ObjectHelper
    {
        public static T To<T>(this object obj)
            => default(T);

        public static T Convert<T>(this object obj)
            => default(T);

        public static object Convert(this object obj, Type type)
            => null;

        public static bool IsValid<T>(this object obj)
            => true;
    }
}