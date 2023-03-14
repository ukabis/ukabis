using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record NativeQuery : IValueObject
    {
        public string Sql { get; }

        public IDictionary<string, object> Dic { get; }

        /// <summary>
        /// 条件の追加許可有無。
        /// </summary>
        public bool IsAllowAddCondition { get; }

        public NativeQuery(string sql, IDictionary<string, object> dic = null, bool isAllowAddCondition = true)
        {
            Sql = sql;
            Dic = dic;
            IsAllowAddCondition = isAllowAddCondition;
        }

        public static bool operator ==(NativeQuery me, object other) => me?.Equals(other) == true;

        public static bool operator !=(NativeQuery me, object other) => !me?.Equals(other) == true;
    }
}
