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
    internal record ContentLength : IValueObject
    {
        public long Value { get; }

        public ContentLength(long value)
        {
            Value = value;
            ValidatorEx.ExceptionValidateObject(this);
        }

        public static bool operator ==(ContentLength me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ContentLength me, object other) => !me?.Equals(other) == true;
    }

    internal static class ContentLengthExtension
    {
        public static ContentLength ToContentLength(this long? val) => val == null ? null : new ContentLength(val.Value);
    }
}
