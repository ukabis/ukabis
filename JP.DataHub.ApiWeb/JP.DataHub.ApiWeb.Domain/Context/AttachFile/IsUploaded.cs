using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record IsUploaded : IValueObject
    {
        public bool Value { get; }

        public IsUploaded(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsUploaded me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsUploaded me, object other) => !me?.Equals(other) == true;
    }
}
