using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 暗号化データのValueObject
    /// </summary>
    public class EncryptData : IValueObject
    {
        /// <summary>暗号化データ</summary>
        public byte[] Content { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="content">暗号化データ</param>
        public EncryptData(byte[] content)
        {
            Content = content;
        }
    }
}
