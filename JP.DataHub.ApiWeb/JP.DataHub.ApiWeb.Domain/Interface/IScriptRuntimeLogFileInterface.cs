using JP.DataHub.ApiWeb.Domain.Interface.Model;

namespace JP.DataHub.ApiWeb.Domain.Interface
{
    public interface IScriptRuntimeLogFileInterface
    {
        ScriptRuntimeLogFileModel Get(Guid logId, Guid vendorId);
        bool Delete(Guid logId, Guid vendorId);
    }
}
