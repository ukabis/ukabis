namespace JP.DataHub.ManageApi.Service.DymamicApi.Scripting.Roslyn
{
    /// <summary>
    /// RoslynScriptの構文チェックを行うためのダミークラスです。
    /// 本体は[JP.DataHub.ApiWeb.Domain.Scripting.Roslyn]にあります。
    /// メソッドを追加するときは本体と同期をとってください。
    /// </summary>
    public class ScriptHelper
    {
        internal ScriptHelper(Guid? providerVendorId)
        {
        }

        public void Printf(string src, params object[] args)
        {
        }

        public void PrintfException(Exception e)
        {
        }
    }
}
