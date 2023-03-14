using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.AttachFile
{
    internal record Key : IValueObject
    {
        public string Value { get; }
        public Key(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(Key me, object other) => me?.Equals(other) == true;

        public static bool operator !=(Key me, object other) => !me?.Equals(other) == true;

        public bool IsKeyMach(Key requestKey)
        {
            if (!string.IsNullOrEmpty(this.Value) && requestKey.Value != this.Value)
            {
                return false;
            }
            return true;
        }
    }
}
