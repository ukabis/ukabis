using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record IsSuccess : IValueObject
    {
        public bool Value { get; }

        public IsSuccess(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsSuccess me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsSuccess me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsSuccessExtension
    {
        public static IsSuccess ToIsSuccess(this bool val) => new IsSuccess(val);
    }
}
