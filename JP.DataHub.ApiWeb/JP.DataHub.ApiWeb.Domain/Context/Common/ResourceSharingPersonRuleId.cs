using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// 個人リソースシェアリングルールIDのValueObject
    /// </summary>
    internal record ResourceSharingPersonRuleId : IValueObject
    {
        /// <summary>アプリケーションID</summary>
        public string Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">個人リソースシェアリングルールID</param>
        public ResourceSharingPersonRuleId(string value)
        {
            Value = value;
        }

        public static bool operator ==(ResourceSharingPersonRuleId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResourceSharingPersonRuleId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ResourceSharingPersonRuleIdExtension
    {
        public static ResourceSharingPersonRuleId ToResourceSharingPersonRuleId(this string val) => val == null ? null : new ResourceSharingPersonRuleId(val);
    }
}
