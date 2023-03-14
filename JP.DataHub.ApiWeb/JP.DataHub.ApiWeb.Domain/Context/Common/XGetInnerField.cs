using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XGetInnerField : IValueObject
    {
        public bool Value { get; }

        public XGetInnerField(bool value)
        {
            Value = value;
        }

        public static bool operator ==(XGetInnerField me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XGetInnerField me, object other) => !me?.Equals(other) == true;
    }

    internal static class XGetInnerFieldExtension
    {
        public static XGetInnerField ToXGetInnerField(this bool? val) => val == null ? null : new XGetInnerField(val.Value);
        public static XGetInnerField ToXGetInnerField(this bool val) => new XGetInnerField(val);
        public static XGetInnerField ToXGetInnerField(this string val) => ToXGetInnerField(val.Convert<bool?>());
    }
}
