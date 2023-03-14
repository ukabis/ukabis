using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XUserResourceSharing : IValueObject
    {
        public List<string> Value { get; }

        public XUserResourceSharing(List<string> value)
        {
            Value = value;
        }

        public static bool operator ==(XUserResourceSharing me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XUserResourceSharing me, object other) => !me?.Equals(other) == true;
    }

    internal static class XUserResourceSharingExtension
    {
        public static XUserResourceSharing ToXUserResourceSharing(this List<string> val) => new XUserResourceSharing(val);
    }
}
