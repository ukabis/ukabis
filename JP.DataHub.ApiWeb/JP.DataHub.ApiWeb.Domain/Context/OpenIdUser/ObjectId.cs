using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.OpenIdUser
{
    /// <summary>
    /// オブジェクトID
    /// </summary>
    internal record ObjectId : IValueObject
    {
        /// <summary>オブジェクトID</summary>
        public string Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">オブジェクトID</param>
        public ObjectId(string value)
        {
            Value = value;
        }

        public static bool operator ==(ObjectId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ObjectId me, object other) => !me?.Equals(other) == true;
    }
}
