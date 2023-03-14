using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record IsAppendStream : IValueObject
    {
        public bool Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IsAppendStream" /> class.
        /// </summary>
        public IsAppendStream(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsAppendStream me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsAppendStream me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsAppendStreamExtension
    {
        public static IsAppendStream ToIsAppendStream(this bool? val) => val == null ? null : new IsAppendStream(val.Value);
        public static IsAppendStream ToIsAppendStream(this bool val) => new IsAppendStream(val);
        public static IsAppendStream ToIsAppendStream(this string val) => ToIsAppendStream(val.Convert<bool?>());
    }
}
