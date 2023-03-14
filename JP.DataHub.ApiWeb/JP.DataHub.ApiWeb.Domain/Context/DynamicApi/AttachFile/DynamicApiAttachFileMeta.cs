using System.Collections.ObjectModel;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.DynamicApi.AttachFile
{
    internal record DynamicApiAttachFileMeta : IValueObject
    {
        public ReadOnlyDictionary<string, string> Dic { get; set; }

        public DynamicApiAttachFileMeta(Dictionary<string, string> value)
        {
            Dic = new ReadOnlyDictionary<string, string>(value);
        }

        public static bool operator ==(DynamicApiAttachFileMeta me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DynamicApiAttachFileMeta me, object other) => !me?.Equals(other) == true;
    }
}
