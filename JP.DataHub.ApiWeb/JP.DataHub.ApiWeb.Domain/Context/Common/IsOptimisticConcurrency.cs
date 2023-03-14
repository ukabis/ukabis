using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// 楽観排他を使用するかどうかを示すValueObject
    /// </summary>
    [MessagePackObject]
    internal record IsOptimisticConcurrency : IValueObject
    {
        /// <summary>楽観排他を使用するかどうか</summary>
        [Key(0)]
        public bool Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">楽観排他を使用するかどうか</param>
        public IsOptimisticConcurrency(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsOptimisticConcurrency me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsOptimisticConcurrency me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsOptimisticConcurrencyExtension
    {
        public static IsOptimisticConcurrency ToIsOptimisticConcurrency(this bool? val) => val == null ? null : new IsOptimisticConcurrency(val.Value);
        public static IsOptimisticConcurrency ToIsOptimisticConcurrency(this bool val) => new IsOptimisticConcurrency(val);
    }
}
