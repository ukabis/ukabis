using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record PostDataType : IValueObject
    {
        public string Value { get; }

        public bool IsArray { get => Value == "array";  }

        public IsArray ToIsArray { get => new IsArray(IsArray); }

        public PostDataType(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(PostDataType me, object other) => me?.Equals(other) == true;

        public static bool operator !=(PostDataType me, object other) => !me?.Equals(other) == true;
    }

    internal static class PostDataTypeExtension
    {
        public static PostDataType ToPostDataType(this string val) => val == null ? null : new PostDataType(val);
    }
}