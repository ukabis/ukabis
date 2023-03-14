using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record AdminKeyword : IValueObject
    {
        public string Value { get; }

        public AdminKeyword(string keyword)
        {
            this.Value = keyword;
        }

        public static bool operator ==(AdminKeyword me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AdminKeyword me, object other) => !me?.Equals(other) == true;
    }

    internal static class AdminKeywordExtension
    {
        public static AdminKeyword ToAdminKeyword(this string val) => val == null ? null : new AdminKeyword(val);
    }
}
