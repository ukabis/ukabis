using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record Script : IValueObject
    {
        public string Value { get; }

        public Script(string value)
        {
            Value = value;
        }

        public static bool operator ==(Script me, object other) => me?.Equals(other) == true;

        public static bool operator !=(Script me, object other) => !me?.Equals(other) == true;
    }

    internal static class ScriptExtension
    {
        public static Script ToScript(this string val) => val == null ? null : new Script(val);
    }
}
