using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DataLakeStoreRequestId : IValueObject
    {
        public string Value { get; }


        public DataLakeStoreRequestId(string dataLakeStoreRequestId)
        {
            Value = dataLakeStoreRequestId;
        }

        public static bool operator ==(DataLakeStoreRequestId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DataLakeStoreRequestId me, object other) => !me?.Equals(other) == true;
    }

    internal static class DataLakeStoreRequestIdExtension
    {
        public static DataLakeStoreRequestId ToDataLakeStoreRequestId(this string val) => val == null ? null : new DataLakeStoreRequestId(val);
    }
}
