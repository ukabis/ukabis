using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsVisibleAgreement : IValueObject
    {
        public bool Value { get; }

        public IsVisibleAgreement(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsVisibleAgreement me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsVisibleAgreement me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsVisibleAgreementExtension
    {
        public static IsVisibleAgreement ToIsVisibleAgreement(this bool? val) => val == null ? null : new IsVisibleAgreement(val.Value);
        public static IsVisibleAgreement ToIsVisibleAgreement(this bool val) => new IsVisibleAgreement(val);
        public static IsVisibleAgreement ToIsVisibleAgreement(this string val) => ToIsVisibleAgreement(val.Convert<bool?>());
    }
}
