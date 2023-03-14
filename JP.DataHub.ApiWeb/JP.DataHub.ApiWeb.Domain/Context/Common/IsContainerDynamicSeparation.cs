using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// 動的Container分離を使用するかどうかを示すValueObject
    /// </summary>
    [MessagePackObject]
    internal record IsContainerDynamicSeparation : IValueObject
    {
        /// <summary>動的Container分離を使用するかどうか</summary>
        [Key(0)]
        public bool Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">動的Container分離を使用するかどうか</param>
        public IsContainerDynamicSeparation(bool value)
        {
            this.Value = value;
        }

        public static bool operator ==(IsContainerDynamicSeparation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsContainerDynamicSeparation me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsContainerDynamicSeparationExtension
    {
        public static IsContainerDynamicSeparation ToIsContainerDynamicSeparation(this bool val) => new IsContainerDynamicSeparation(val);
    }
}