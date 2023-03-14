using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record SkipCount : IValueObject
    {
        public int Value { get; }

        public SkipCount(int value)
        {
            Value = value;
        }

        public static bool operator ==(SkipCount me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SkipCount me, object other) => !me?.Equals(other) == true;
    }

    internal static class SkipCountExtension
    {
        public static SkipCount ToSkipCount(this int val) => new SkipCount(val);
    }
}
