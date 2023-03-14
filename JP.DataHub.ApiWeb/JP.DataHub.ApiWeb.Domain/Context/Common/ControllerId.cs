using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ControllerId : IValueObject
    {
        public string Value { get; }

        public Guid? ToGuid
        {
            get
            {
                Guid tmp;
                if (Guid.TryParse(Value, out tmp))
                {
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
        }

        public ControllerId(string value)
        {
            Value = value;
        }

        public static bool operator ==(ControllerId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ControllerId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ControllerIdExtension
    {
        public static ControllerId ToControllerId(this string? val) => val == null ? null : new ControllerId(val);
        public static ControllerId ToControllerId(this Guid? val) => val == null ? null : new ControllerId(val?.ToString());
    }
}