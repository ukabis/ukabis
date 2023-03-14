using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    [MessagePackObject]
    internal class OpenIdClientSecret : IValueObject
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [Key(0)]
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenIdClientSecret" /> class.
        /// </summary>
        /// <param name="openIdClientSecret">The application identifier.</param>
        public OpenIdClientSecret(string openIdClientSecret)
        {
            this.Value = openIdClientSecret;
        }

        public static bool operator ==(OpenIdClientSecret me, object other) => me?.Equals(other) == true;

        public static bool operator !=(OpenIdClientSecret me, object other) => !me?.Equals(other) == true;
    }

    internal static class OpenIdClientSecretCacheExtension
    {
        public static OpenIdClientSecret ToOpenIdClientSecret(this string val) => val == null ? null : new OpenIdClientSecret(val);
    }
}
