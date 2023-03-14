using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 有効期限のValueObject
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class ExpirationDate : IValueObject
    {
        /// <summary>有効期限(UTC)</summary>
        [Key(0)]
        public DateTime Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">有効期限</param>
        public ExpirationDate(DateTime value)
        {
            Value = value;
        }

        public static bool operator ==(ExpirationDate me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ExpirationDate me, object other) => !me?.Equals(other) == true;
    }
}
