using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XResponseContinuation : IValueObject
    {
        public string ContinuationString { get; }

        public XResponseContinuation(string continuationString)
        {
            // 値が空文字またはnullのHTTPヘッダはクライアントに送信されないためスペースを入れる。
            // (クライアント側では値が空文字のヘッダとして受信される。)
            ContinuationString = string.IsNullOrEmpty(continuationString) ? " " : continuationString;
        }

        public static bool operator ==(XResponseContinuation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XResponseContinuation me, object other) => !me?.Equals(other) == true;
    }

    internal static class XResponseContinuationExtension
    {
        public static XResponseContinuation ToXResponseContinuation(this string val) => val == null ? null : new XResponseContinuation(val);
    }
}