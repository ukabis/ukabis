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
    internal record EmailAddress : IValueObject
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
        /// Initializes a new instance of the <see cref="EmailAddress"/> class.
        /// </summary>
        /// <param name="emailAddress">The account.</param>
        public EmailAddress(string emailAddress)
        {
            this.Value = emailAddress;
        }

        public static bool operator ==(EmailAddress me, object other) => me?.Equals(other) == true;

        public static bool operator !=(EmailAddress me, object other) => !me?.Equals(other) == true;
    }

    internal static class EmailAddressExtension
    {
        public static EmailAddress ToEmailAddress(this string val) => val == null ? null  : new EmailAddress(val);
    }
}
