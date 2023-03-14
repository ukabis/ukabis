using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record InputStream : IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InputStream" /> class.
        /// </summary>
        public InputStream(Stream value)
        {
            Value = value;
        }

        public Stream Value { get; }

        public static bool operator ==(InputStream me, object other) => me?.Equals(other) == true;

        public static bool operator !=(InputStream me, object other) => !me?.Equals(other) == true;
    }
}
