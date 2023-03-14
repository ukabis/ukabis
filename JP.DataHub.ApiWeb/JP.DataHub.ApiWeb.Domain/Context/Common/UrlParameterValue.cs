using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record UrlParameterValue : IValueObject
    {
        public string Value { get; }
        public UrlParameterValue(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(UrlParameterValue me, object other) => me?.Equals(other) == true;

        public static bool operator !=(UrlParameterValue me, object other) => !me?.Equals(other) == true;
    }

    internal static class UrlParameterValueExtension
    {
        public static UrlParameterValue ToUrlParameterKey(this string str) => str == null ? null : new UrlParameterValue(str);
    }
}