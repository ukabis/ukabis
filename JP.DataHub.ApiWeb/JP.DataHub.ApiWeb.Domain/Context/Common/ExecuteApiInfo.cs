using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ExecuteApiInfo : IValueObject
    {
        public string ControllerId { get; }
        public string ApiId { get; }

        public string DataId { get; }
        public string Contents { get; }

        public string QueryString { get; }
        public string KeyValue { get; }

        public ExecuteApiInfo(string controllerId, string apiId, string dataId, string contents, string queryString, string keyValue)
        {
            ControllerId = controllerId;
            ApiId = apiId;
            DataId = dataId;
            Contents = contents;
            QueryString = queryString;
            KeyValue = keyValue;
        }

        public static bool operator ==(ExecuteApiInfo me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ExecuteApiInfo me, object other) => !me?.Equals(other) == true;
    }
}
