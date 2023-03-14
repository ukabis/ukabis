using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ResourceSharingRuleId : IValueObject
    {
        public Guid Value { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="id">id</param>
        public ResourceSharingRuleId(Guid id)
        {
            Value = id;
        }

        public static bool operator ==(ResourceSharingRuleId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResourceSharingRuleId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ResourceSharingRuleIdExtension
    {
        public static ResourceSharingRuleId ToResourceSharingRuleId(this Guid value) => value == null ? null : new ResourceSharingRuleId(value);
    }
}
