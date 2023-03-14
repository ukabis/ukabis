using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record SelectCount : IValueObject
    {
        public int Value { get; }

        public SelectCount(int value)
        {
            Value = value;
        }

        public static bool operator ==(SelectCount me, object other) => me?.Equals(other) == true;

        public static bool operator !=(SelectCount me, object other) => !me?.Equals(other) == true;
    }

    internal static class SelectCountExtension
    {
        public static SelectCount ToSelectCount(this int val) => new SelectCount(val);
    }
}