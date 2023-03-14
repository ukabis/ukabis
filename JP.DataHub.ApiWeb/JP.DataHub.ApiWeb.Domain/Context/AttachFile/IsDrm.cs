using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record IsDrm : IValueObject
    {
        public bool Value { get; }

        public IsDrm(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsDrm me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsDrm me, object other) => !me?.Equals(other) == true;
    }
}
