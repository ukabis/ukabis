using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Validations;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ContentType : IValueObject
    {
        public string Value { get; }

        public ContentType(string value)
        {
            Value = value;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(ContentType me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ContentType me, object other) => !me?.Equals(other) == true;
    }

    internal static class ContentTypeExtension
    {
        public static ContentType ToContentType(this string str) => str == null ? null : new ContentType(str);
    }
}
