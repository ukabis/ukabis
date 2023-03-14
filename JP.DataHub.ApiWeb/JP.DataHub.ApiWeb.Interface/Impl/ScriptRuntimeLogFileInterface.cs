using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Interface.Model;
using JP.DataHub.ApiWeb.Domain.ApplicationService;

namespace JP.DataHub.ApiWeb.Interface.Impl
{
    public class ScriptRuntimeLogFileInterface : IScriptRuntimeLogFileInterface
    {
        private IScriptRuntimeLogApplicationService _scriptRuntimeLog = UnityCore.Resolve<IScriptRuntimeLogApplicationService>();

        public ScriptRuntimeLogFileModel Get(Guid logId, Guid vendorId) => _scriptRuntimeLog.Get(logId, vendorId);
        public bool Delete(Guid logId, Guid vendorId) => _scriptRuntimeLog.Delete(logId, vendorId);
    }
}
