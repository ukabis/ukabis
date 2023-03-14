using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DataId : IValueObject
    {
        public string Value { get; }


        public DataId(string dataId)
        {
            Value = dataId;
        }

        public static bool operator ==(DataId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DataId me, object other) => !me?.Equals(other) == true;
    }

    internal static class DataIdExtension
    {
        public static DataId ToDataId(this string val) => val == null ? null : new DataId(val);
        public static DataId ToDataId(this Guid? val) => val == null ? null : new DataId(val?.ToString());
    }
}