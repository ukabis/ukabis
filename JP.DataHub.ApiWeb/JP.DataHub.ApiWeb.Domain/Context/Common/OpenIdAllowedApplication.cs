using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record OpenIdAllowedApplication : IValueObject
    {
        /// <summary>アプリケーションID</summary>
        public string Value { get; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="value">アプリケーションID</param>
        public OpenIdAllowedApplication(string value)
        {
            Value = value;
        }

        public static bool operator ==(OpenIdAllowedApplication me, object other) => me?.Equals(other) == true;

        public static bool operator !=(OpenIdAllowedApplication me, object other) => !me?.Equals(other) == true;
    }

    internal static class OpenIdAllowedApplicationExtension
    {
        public static OpenIdAllowedApplication ToOpenIdAllowedApplication(this string val) => val == null ? null : new OpenIdAllowedApplication(val);
    }
}