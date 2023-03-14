using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// Webhookがあるかを示すValueObject
    /// </summary>
    internal record HasWebhook : IValueObject
    {
        /// <summary>メールテンプレートがあるか</summary>
        public bool Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">Webhookがあるか</param>
        public HasWebhook(bool value)
        {
            Value = value;
        }

        public static bool operator ==(HasWebhook me, object other) => me?.Equals(other) == true;

        public static bool operator !=(HasWebhook me, object other) => !me?.Equals(other) == true;
    }

    internal static class HasWebhookExtension
    {
        public static HasWebhook ToHasWebhook(this bool? flag) => flag == null ? null : new HasWebhook(flag.Value);
        public static HasWebhook ToHasWebhook(this bool flag) => new HasWebhook(flag);
        public static HasWebhook ToHasWebhook(this string str) => ToHasWebhook(str.Convert<bool?>());
    }
}
