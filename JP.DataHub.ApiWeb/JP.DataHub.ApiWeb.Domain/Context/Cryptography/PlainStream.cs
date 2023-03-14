using System;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 平文データストリームValueObject
    /// </summary>
    public class PlainStream : IValueObject
    {
        /// <summary>復号データストリーム</summary>
        public Stream Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">平文データストリーム</param>
        public PlainStream(Stream value)
        {
            Value = value;
        }
    }
}
