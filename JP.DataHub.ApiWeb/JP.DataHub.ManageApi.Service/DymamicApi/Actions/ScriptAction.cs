using JP.DataHub.Com.Unity;
using JP.DataHub.ManageApi.Service.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions
{
    internal class ScriptAction //: AbstractDynamicApiActionBase, IScriptAction
    {
        //public string Script { get; set; }

        //public ScriptType ScriptType { get; set; }

        //private IScriptRuntimeLogMetaDataRepository ScriptRuntimeLogMetaDataRepository { get => _lazyScriptRuntimeLogMetaDataRepository.Value; }
        //private Lazy<IScriptRuntimeLogMetaDataRepository> _lazyScriptRuntimeLogMetaDataRepository = new(() => UnityCore.Resolve<IScriptRuntimeLogMetaDataRepository>("DynamicApi"));

        //private const string logMediaType = "text/plain";
        //private const string logReponseHeader = "X-ScriptRuntimeLog-Id";

        //public override HttpResponseMessage ExecuteAction()
        //{
        //    var queryParams = Query?.ToDictionary(x => x.Key.Value, x => x.Value.Value) ?? new Dictionary<string, string>();
        //    var urlParams = KeyValue?.ToDictionary(x => x.Key.Value, x => x.Value.Value) ?? new Dictionary<string, string>();

        //    foreach (var p in urlParams)
        //    {
        //        if (!queryParams.ContainsKey(p.Key)) queryParams.Add(p.Key, p.Value);
        //    }
        //    var args = new ScriptArgumentParameters
        //    {
        //        SystemId = SystemId.Value,
        //        VendorId = VendorId.Value,
        //        OpenId = OpenId?.Value,
        //        Contents = Contents?.ReadToString() ?? "",
        //        QueryString = queryParams,
        //        KeyValue = urlParams,
        //        ScriptHelper = new ETL.Scripting.Roslyn.ScriptHelper(ProviderVendorId.ToGuid)
        //    };
        //    var scriptExecuter = DomainUnityContainer.Resolve<IScriptingExecuter>(ScriptType.Value.GetCode());
        //    bool isEnableCatchException = PerRequestDataContainer.XScriptRuntimeLogException;
        //    var compileResult = scriptExecuter.Compile<HttpResponseMessage>(Script.Value, args, isEnableCatchException);
        //    if (!string.IsNullOrEmpty(compileResult)) throw new Exception($"CompileError message={compileResult}");

        //    var startDate = DateTime.UtcNow;
        //    RoslynScriptRuntimeException ScriptRuntimeException = null;
        //    AggregateException AgException = null;
        //    HttpResponseMessage res = null;
        //    try
        //    {
        //        res = scriptExecuter.ExecuteScript<HttpResponseMessage>(args);
        //    }
        //    catch (AggregateException e) when (e.InnerException is RoslynScriptRuntimeException)
        //    {//isEnableCatchExceptionがtrueでスクリプト実行時例外が発生した場合
        //        args.ScriptHelper.PrintfException(e.InnerException.InnerException);
        //        ScriptRuntimeException = (RoslynScriptRuntimeException)e.InnerException;
        //        if (VendorId.Value == ProviderVendorId.Value)
        //        {
        //            ScriptRuntimeException.ScriptRuntimeLogId = args.ScriptHelper.ScriptRuntimeLogId.ToString();
        //        }
        //    }
        //    catch (AggregateException ae)
        //    {
        //        AgException = ae;
        //    }

        //    //メタデータの作成
        //    if (args.ScriptHelper.IsExecutedPrintf)
        //    {
        //        ScriptRuntimeLogMetaDataRepository.Create(
        //            new ScriptRuntimeLogMetaData(args.ScriptHelper.ScriptRuntimeLogId,
        //            ApiId.ToGuid,
        //            startDate,
        //            Convert.ToInt32((DateTime.UtcNow - startDate).TotalMilliseconds),
        //            ScriptRuntimeException != null || AgException != null,
        //            DateTime.UtcNow,
        //            Guid.Parse(OpenId.Value)));

        //        //同一ベンダーにのみログIDを通知する
        //        if (VendorId.Value == ProviderVendorId.Value)
        //        {
        //            res?.Headers.Add(logReponseHeader, args.ScriptHelper.ScriptRuntimeLogId.ToString());
        //        }
        //    }
        //    if (ScriptRuntimeException != null || AgException != null) { throw ScriptRuntimeException ?? throw AgException; }
        //    return res;
        //}
    }
}
