using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record FileLength : IValueObject
    {
        public long Value { get; }

        public FileLength(long value)
        {
            Value = value;
        }

        public static bool operator ==(FileLength me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FileLength me, object other) => !me?.Equals(other) == true;
    }
}
