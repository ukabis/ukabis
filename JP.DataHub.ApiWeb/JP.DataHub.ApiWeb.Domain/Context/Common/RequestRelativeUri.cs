using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record RequestRelativeUri : IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestRelativeUri" /> class.
        /// </summary>
        public RequestRelativeUri(string requestRelativeUri)
        {
            Value = requestRelativeUri;
        }

        public string Value { get; }

        public static bool operator ==(RequestRelativeUri me, object other) => me?.Equals(other) == true;

        public static bool operator !=(RequestRelativeUri me, object other) => !me?.Equals(other) == true;
    }

    internal static class RequestRelativeUriExtension
    {
        public static RequestRelativeUri ToRequestRelativeUri(this string val) => val == null ? null : new RequestRelativeUri(val);
    }
}