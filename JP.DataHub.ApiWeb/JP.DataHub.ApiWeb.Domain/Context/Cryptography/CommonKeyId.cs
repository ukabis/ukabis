using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Cryptography
{
    /// <summary>
    /// 共通鍵IDのValueObject
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class CommonKeyId : IValueObject
    {
        /// <summary>共通鍵ID</summary>
        [Key(0)]
        public string Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">システムID</param>
        public CommonKeyId(string value = null)
        {
            Value = string.IsNullOrEmpty(value) ? Guid.NewGuid().ToString() : value;
        }

        public static bool operator ==(CommonKeyId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(CommonKeyId me, object other) => !me?.Equals(other) == true;
    }
}
