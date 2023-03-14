using JP.DataHub.Com.DDD;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    class ScriptRuntimeLogWriteEventData : IValueObject, IDomainEventData
    {
        public ScriptRuntimeLogWriteEventData(DateTime occurredDateTime, Guid? vendorId, Guid scriptRuntimeLogId, string name, string content, string contentType)
        {
            OccurredDateTime = occurredDateTime;
            AppendFile = new ScriptRuntimeLogAppendFile(
            name,
            vendorId ?? throw new ArgumentNullException("VendorId"),
            scriptRuntimeLogId,
            content,
            contentType);
        }

        public Type GetEventDataType()
        {
            return this.GetType();
        }
        public DateTime OccurredDateTime { get; }

        public ScriptRuntimeLogAppendFile AppendFile { get; }
    }
}
