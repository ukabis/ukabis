using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsPerson : IValueObject
    {
        public bool Value { get; }

        public IsPerson(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsPerson me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsPerson me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsPersonExtension
    {
        public static IsPerson ToIsPerson(this bool? flag) => flag == null ? null : new IsPerson(flag.Value);
        public static IsPerson ToIsPerson(this bool flag) => new IsPerson(flag);
        public static IsPerson ToIsPerson(this string str) => ToIsPerson(str.Convert<bool?>());
    }
}
