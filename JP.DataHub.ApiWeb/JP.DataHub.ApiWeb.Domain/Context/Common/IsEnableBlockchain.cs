using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// ブロックチェーンを使用するかどうかを示すValueObject
    /// </summary>
    [MessagePackObject]
    internal class IsEnableBlockchain : IValueObject
    {
        /// <summary>ブロックチェーンを使用するかどうか</summary>
        [Key(0)]
        public bool Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">ブロックチェーンを使用するかどうか</param>
        public IsEnableBlockchain(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsEnableBlockchain me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsEnableBlockchain me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsEnableBlockchainExtension
    {
        public static IsEnableBlockchain ToIsEnableBlockchain(this bool? val) => val == null ? null : new IsEnableBlockchain(val.Value);
        public static IsEnableBlockchain ToIsEnableBlockchain(this bool val) => new IsEnableBlockchain(val);
        public static IsEnableBlockchain ToIsEnableBlockchain(this string val) => ToIsEnableBlockchain(val.Convert<bool?>());
    }
}