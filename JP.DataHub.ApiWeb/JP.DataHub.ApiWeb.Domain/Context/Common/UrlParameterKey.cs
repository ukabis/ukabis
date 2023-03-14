using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record UrlParameterKey : IValueObject
    {
        public string Value { get; }

        public UrlParameterKey(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(UrlParameterKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(UrlParameterKey me, object other) => !me?.Equals(other) == true;
    }

    internal static class UrlParameterKeyExtension
    {
        public static UrlParameterKey ToUrlParameterKey(this string str) => str == null ? null : new UrlParameterKey(str);
    }
}
