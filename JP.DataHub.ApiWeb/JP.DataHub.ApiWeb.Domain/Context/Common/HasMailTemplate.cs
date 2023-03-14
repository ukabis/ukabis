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
    /// メールテンプレートがあるかを示すValueObject
    /// </summary>
    internal record HasMailTemplate : IValueObject
    {
        /// <summary>メールテンプレートがあるか</summary>
        public bool Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">メールテンプレートがあるか</param>
        public HasMailTemplate(bool value)
        {
            Value = value;
        }

        public static bool operator ==(HasMailTemplate me, object other) => me?.Equals(other) == true;

        public static bool operator !=(HasMailTemplate me, object other) => !me?.Equals(other) == true;
    }

    internal static class HasMailTemplateExtension
    {
        public static HasMailTemplate ToHasMailTemplate(this bool? flag) => flag == null ? null : new HasMailTemplate(flag.Value);
        public static HasMailTemplate ToHasMailTemplate(this bool flag) => new HasMailTemplate(flag);
        public static HasMailTemplate ToHasMailTemplate(this string str) => ToHasMailTemplate(str.Convert<bool?>());
    }
}
