using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ODataQuery : IValueObject
    {
        public string Value { get; }

        public bool HasValue { get => !string.IsNullOrEmpty(Value); }

        public bool HasAnyQuery { get => HasValue ? Regex.IsMatch(HttpUtility.UrlDecode(Value), @"\$filter.*/any\(", RegexOptions.IgnoreCase) : false; }
        public bool HasCountQuery { get => HasValue ? Regex.IsMatch(HttpUtility.UrlDecode(Value), @"\$count=true", RegexOptions.IgnoreCase) : false; }

        public ODataQuery(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(ODataQuery me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ODataQuery me, object other) => !me?.Equals(other) == true;
    }

    internal static class ODataQueryExtension
    {
        public static ODataQuery ToODataQuery(this string value) => new ODataQuery(value);
    }
}
