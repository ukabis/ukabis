using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Extensions;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    /// <summary>
    /// DynamicAPIとして管理するデータの履歴を残すかどうかを示すValueObject
    /// </summary>
    [MessagePackObject]
    internal record IsDocumentHistory : IValueObject
    {
        /// <summary>データの履歴を残すかどうか</summary>
        [Key(0)]
        public bool Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">データの履歴を残すかどうか</param>
        public IsDocumentHistory(bool value)
        {
            Value = value;
        }

        public static bool operator ==(IsDocumentHistory me, object other) => me?.Equals(other) == true;

        public static bool operator !=(IsDocumentHistory me, object other) => !me?.Equals(other) == true;
    }

    internal static class IsDocumentHistoryExtension
    {
        public static IsDocumentHistory ToIsDocumentHistory(this bool? val) => val == null ? null : new IsDocumentHistory(val.Value);
        public static IsDocumentHistory ToIsDocumentHistory(this bool val) => new IsDocumentHistory(val);
        public static IsDocumentHistory ToIsDocumentHistory(this string val) => ToIsDocumentHistory(val.Convert<bool?>());
    }
}
