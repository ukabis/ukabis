using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    internal record AsyncRequestId : IValueObject
    {
        [Key(0)]
        public string Value { get; }


        public AsyncRequestId(string vaule)
        {
            Value = vaule;
        }

        public static bool operator ==(AsyncRequestId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AsyncRequestId me, object other) => !me?.Equals(other) == true;
    }

    internal static class AsyncRequestIdExtension
    {
        public static AsyncRequestId ToAsyncRequestId(this string val) => val == null ? null : new AsyncRequestId(val);
        public static AsyncRequestId ToAsyncRequestId(this Guid val) => val == null ? null : new AsyncRequestId(val.ToString());
    }
}
