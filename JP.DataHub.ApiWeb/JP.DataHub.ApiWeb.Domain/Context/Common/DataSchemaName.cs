using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DataSchemaName : IValueObject
    {
        public string Value { get; }

        public DataSchemaName(string value)
        {
            Value = value;
        }

        public static bool operator ==(DataSchemaName me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DataSchemaName me, object other) => !me?.Equals(other) == true;
    }

    internal static class DataSchemaNameExtension
    {
        public static DataSchemaName ToDataSchemaName(this string val) => val == null ? null : new DataSchemaName(val);
    }
}