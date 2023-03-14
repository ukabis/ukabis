using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 公開鍵IDのValueObject
    /// </summary>
    internal class PublicKeyId : IValueObject
    {
        /// <summary>公開鍵ID</summary>
        public string Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">システムID</param>
        public PublicKeyId(string value)
        {
            Value = value;
        }
    }
}
