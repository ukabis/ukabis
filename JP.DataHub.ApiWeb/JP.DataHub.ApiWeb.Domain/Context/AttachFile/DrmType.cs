using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record DrmType : IValueObject
    {
        public string Value { get; }

        public DrmType(string value)
        {
            Value = value;
        }

        public static bool operator ==(DrmType me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DrmType me, object other) => !me?.Equals(other) == true;
    }
}
