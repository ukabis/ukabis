using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 平文データのValueObject
    /// </summary>
    public class PlainData : IValueObject
    {
        /// <summary>平文データ</summary>
        public byte[] Content { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="content">平文データ</param>
        public PlainData(byte[] content)
        {
            Content = content;
        }

        public static bool operator ==(PlainData me, object other) => me?.Equals(other) == true;

        public static bool operator !=(PlainData me, object other) => !me?.Equals(other) == true;
    }
}
