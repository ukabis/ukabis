using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record DrmKey : IValueObject
    {
        public string Value { get; }

        public DrmKey(string value)
        {
            Value = value;
        }

        public static bool operator ==(DrmKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DrmKey me, object other) => !me?.Equals(other) == true;
    }
}
