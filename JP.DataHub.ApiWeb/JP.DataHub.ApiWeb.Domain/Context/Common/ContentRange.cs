using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ContentRange : IValueObject
    {
        public ContentRangeHeaderValue Value { get; }

        public string SourceValue { get; }

        public ContentRange(string value)
        {
            SourceValue = value;
            ContentRangeHeaderValue range = null;
            if (!string.IsNullOrEmpty(value) && ContentRangeHeaderValue.TryParse(value, out range))
            {
                Value = range;
            }
        }

        public static bool operator ==(ContentRange me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ContentRange me, object other) => !me?.Equals(other) == true;
    }

    internal static class ContentRangeExtension
    {
        public static ContentRange ToContentRange(this string str) => str == null ? null : new ContentRange(str);
    }
}
