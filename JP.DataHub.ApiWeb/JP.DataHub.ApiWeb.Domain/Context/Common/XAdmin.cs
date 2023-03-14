using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal class XAdmin : IValueObject
    {
        public string Value { get; }

        public XAdmin(string value)
        {
            Value = value;
        }

        public static bool operator ==(XAdmin me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XAdmin me, object other) => !me?.Equals(other) == true;
    }

    internal static class XAdminExtension
    {
        public static XAdmin ToXAdmin(this string str) => str == null ? null : new XAdmin(str);
    }
}