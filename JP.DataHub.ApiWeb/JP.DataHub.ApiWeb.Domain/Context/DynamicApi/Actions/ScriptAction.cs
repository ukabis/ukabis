using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Domain.Scripting;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    // .NET6
    internal class ScriptAction : AbstractDynamicApiAction, IScriptAction
    {
        public Script Script { get; set; }

        public ScriptTypeVO ScriptType { get; set; }

        [Dependency]
        public IScriptRuntimeLogMetaDataRepository ScriptRuntimeLogMetaDataRepository { get; set; }

        private const string logMediaType = "text/plain";
        private const string logReponseHeader = "X-ScriptRuntimeLog-Id";


        public override HttpResponseMessage ExecuteAction()
        {
            var queryParams = Query?.Dic.ToDictionary(x => x.Key.Value, x => x.Value.Value) ?? new Dictionary<string, string>();
            var urlParams = KeyValue?.Dic.ToDictionary(x => x.Key.Value, x => x.Value.Value) ?? new Dictionary<string, string>();

            foreach (var p in urlParams)
            {
                if (!queryParams.ContainsKey(p.Key)) queryParams.Add(p.Key, p.Value);
            }
            var args = new ScriptArgumentParameters
            {
                SystemId = SystemId.Value,
                VendorId = VendorId.Value,
                OpenId = OpenId?.Value,
                Contents = Contents?.ReadToString() ?? "",
                QueryString = queryParams,
                KeyValue = urlParams,
                ScriptHelper = new Scripting.Roslyn.ScriptHelper(ProviderVendorId.ToGuid)
            };
            var scriptExecuter = UnityCore.Resolve<IScriptingExecuter>(ScriptType.Code);
            bool isEnableCatchException = PerRequestDataContainer.XScriptRuntimeLogException;
            var compileResult = scriptExecuter.Compile<HttpResponseMessage>(Script.Value, args, isEnableCatchException);
            if (!string.IsNullOrEmpty(compileResult)) throw new Exception($"CompileError message={compileResult}");

            var startDate = DateTime.UtcNow;
            RoslynScriptRuntimeException ScriptRuntimeException = null;
            AggregateException AgException = null;
            HttpResponseMessage res = null;
            try
            {
                res = scriptExecuter.ExecuteScript<HttpResponseMessage>(args);
            }
            catch (AggregateException e) when (e.InnerException is RoslynScriptRuntimeException)
            {//isEnableCatchExceptionがtrueでスクリプト実行時例外が発生した場合
                args.ScriptHelper.PrintfException(e.InnerException.InnerException);
                ScriptRuntimeException = (RoslynScriptRuntimeException)e.InnerException;
                if (VendorId.Value == ProviderVendorId.Value)
                {
                    ScriptRuntimeException.ScriptRuntimeLogId = args.ScriptHelper.ScriptRuntimeLogId.ToString();
                }
            }
            catch (AggregateException ae)
            {
                AgException = ae;
            }

            //メタデータの作成
            if (args.ScriptHelper.IsExecutedPrintf)
            {
                ScriptRuntimeLogMetaDataRepository.Create(
                    new ScriptRuntimeLogMetaData(args.ScriptHelper.ScriptRuntimeLogId,
                    ApiId.ToGuid,
                    startDate,
                    Convert.ToInt32((DateTime.UtcNow - startDate).TotalMilliseconds),
                    ScriptRuntimeException != null || AgException != null,
                    DateTime.UtcNow,
                    Guid.Parse(OpenId.Value)));

                //同一ベンダーにのみログIDを通知する
                if (VendorId.Value == ProviderVendorId.Value)
                {
                    res?.Headers.Add(logReponseHeader, args.ScriptHelper.ScriptRuntimeLogId.ToString());
                }
            }
            if (ScriptRuntimeException != null || AgException != null) { throw ScriptRuntimeException ?? throw AgException; }
            return res;
        }
    }
}
