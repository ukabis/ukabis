using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XRequestContinuation : IValueObject
    {
        public string ContinuationString { get; }

        public XRequestContinuation(string continuationString)
        {
            this.ContinuationString = continuationString;
        }

        public static bool operator ==(XRequestContinuation me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XRequestContinuation me, object other) => !me?.Equals(other) == true;
    }

    internal static class XRequestContinuationExtension
    {
        public static XRequestContinuation ToXRequestContinuation(this string val) => new XRequestContinuation(val);
    }
}