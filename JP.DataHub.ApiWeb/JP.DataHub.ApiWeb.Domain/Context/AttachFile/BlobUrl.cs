using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record BlobUrl : IValueObject
    {
        public string Value { get; }

        public BlobUrl(string value)
        {
            Value = value;
        }

        public static bool operator ==(BlobUrl me, object other) => me?.Equals(other) == true;

        public static bool operator !=(BlobUrl me, object other) => !me?.Equals(other) == true;
    }
}
