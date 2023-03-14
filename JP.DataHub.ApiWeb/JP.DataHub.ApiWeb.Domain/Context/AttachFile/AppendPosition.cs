using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record AppendPosition : IValueObject
    {
        public long Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppendPosition" /> class.
        /// </summary>
        public AppendPosition(long value)
        {
            Value = value;
        }

        public static bool operator ==(AppendPosition me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AppendPosition me, object other) => !me?.Equals(other) == true;
    }

    internal static class AppendPositionExtension
    {
        public static AppendPosition ToAppendPosition(this long val) => new AppendPosition(val);
    }
}
