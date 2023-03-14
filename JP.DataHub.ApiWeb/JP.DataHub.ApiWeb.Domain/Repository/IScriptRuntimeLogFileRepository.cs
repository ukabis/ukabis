using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;

namespace JP.DataHub.ApiWeb.Domain.Repository
{
    interface IScriptRuntimeLogFileRepository
    {
        Task<Uri> AppendAsync(ScriptRuntimeLogAppendFile file);
        ScriptRuntimeLogGetFile Get(Guid logId, Guid vendorId);
        bool Delete(Guid logId, Guid vendorId);
    }
}
