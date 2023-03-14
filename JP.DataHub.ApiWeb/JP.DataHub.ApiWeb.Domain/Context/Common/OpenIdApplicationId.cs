using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    // .NET6
    [MessagePackObject]
    internal class OpenIdApplicationId : IValueObject
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
        /// Initializes a new instance of the <see cref="OpenIdApplicationId" /> class.
        /// </summary>
        /// <param name="openApplicationId">The application identifier.</param>
        public OpenIdApplicationId(string openApplicationId)
        {
            this.Value = openApplicationId;
        }

        public static bool operator ==(OpenIdApplicationId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(OpenIdApplicationId me, object other) => !me?.Equals(other) == true;
    }

    internal static class OpenIdApplicationIdCacheExtension
    {
        public static OpenIdApplicationId ToOpenIdApplicationId(this string val) => val == null ? null : new OpenIdApplicationId(val);
    }
}
