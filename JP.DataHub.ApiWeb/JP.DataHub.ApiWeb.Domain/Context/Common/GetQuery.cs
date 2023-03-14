using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record GetQuery : IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetQuery" /> class.
        /// </summary>
        public GetQuery(string query)
        {
            Value = query;
        }

        public string Value { get; }

        public static bool operator ==(GetQuery me, object other) => me?.Equals(other) == true;

        public static bool operator !=(GetQuery me, object other) => !me?.Equals(other) == true;
    }

    internal static class GetQueryExtension
    {
        public static GetQuery ToGetQuery(this string str) => str == null ? null : new GetQuery(str);
    }
}
