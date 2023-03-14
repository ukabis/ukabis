using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record TermsGroupCode : IValueObject
    {
        public string Value { get; }

        public TermsGroupCode(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(TermsGroupCode me, object other) => me?.Equals(other) == true;

        public static bool operator !=(TermsGroupCode me, object other) => !me?.Equals(other) == true;
    }

    internal static class TermsGroupCodeExtension
    {
        public static TermsGroupCode ToTermsGroupCode(this string val) => val == null ? null : new TermsGroupCode(val);
    }
}