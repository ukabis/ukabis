using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 暗号化データストリームValueObject
    /// </summary>
    public class EncryptStream : IValueObject
    {
        /// <summary>暗号化データストリーム</summary>
        public Stream Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">暗号化データストリーム</param>
        public EncryptStream(Stream value)
        {
            Value = value;
        }
    }
}
