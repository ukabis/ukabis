using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record IsEndStream : IValueObject
    {
        public bool Value { get; }

        public IsEndStream(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsEndStream me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsEndStream me, object other) => !me?.Equals(other) == true;
    }
}
