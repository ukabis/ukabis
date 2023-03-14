using System.Collections.ObjectModel;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal class Meta : ReadOnlyDictionary<MetaKey, MetaValue>, IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Meta" /> class.
        /// </summary>
        public Meta(Dictionary<MetaKey, MetaValue> value) : base(value)
        {
        }
    }

    internal class MetaKey : IValueObject
    {
        public string Value { get; }

        public MetaKey(string value)
        {
            Value = value;
        }

        public static bool operator ==(MetaKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(MetaKey me, object other) => !me?.Equals(other) == true;
    }

    internal class MetaValue : IValueObject
    {
        public string Value { get; }

        public MetaValue(string value)
        {
            Value = value;
        }

        public static bool operator ==(MetaValue me, object other) => me?.Equals(other) == true;

        public static bool operator !=(MetaValue me, object other) => !me?.Equals(other) == true;
    }
}
