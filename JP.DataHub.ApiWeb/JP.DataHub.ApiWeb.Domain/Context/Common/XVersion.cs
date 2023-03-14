using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XVersion : IValueObject
    {
        public int Value { get; }

        public XVersion(int value)
        {
            Value = value;
        }

        public static bool operator ==(XVersion me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XVersion me, object other) => !me?.Equals(other) == true;
    }

    internal static class XVersionExtension
    {
        public static XVersion ToXVersion(this int? val) => val == null ? null : new XVersion(val.Value);
        public static XVersion ToXVersion(this int val) => new XVersion(val);
    }
}
