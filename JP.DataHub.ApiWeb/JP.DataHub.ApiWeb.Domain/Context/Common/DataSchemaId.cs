using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DataSchemaId : IValueObject
    {
        public string Value { get; }

        public DataSchemaId(string value)
        {
            Value = value;
        }

        public static bool operator ==(DataSchemaId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DataSchemaId me, object other) => !me?.Equals(other) == true;
    }
}