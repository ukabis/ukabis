using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [Serializable]
    public record AccessTokenExpirationTimeSpan : IValueObject
    {
        public TimeSpan Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">有効期限</param>
        public AccessTokenExpirationTimeSpan(TimeSpan value)
        {
            Value = value;
        }

        public static bool operator ==(AccessTokenExpirationTimeSpan me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AccessTokenExpirationTimeSpan me, object other) => !me?.Equals(other) == true;
    }
}
