using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ResourceGroupId : IValueObject
    {
        public string Value { get; }

        public ResourceGroupId(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(ResourceGroupId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResourceGroupId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ResourceGroupIdExtension
    {
        public static ResourceGroupId ToResourceGroupId(this string val) => val == null ? null : new ResourceGroupId(val);
    }
}