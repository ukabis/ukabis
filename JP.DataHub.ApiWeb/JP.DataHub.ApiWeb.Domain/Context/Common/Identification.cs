using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record Identification : IValueObject
    {
        public string Value { get; }

        public Identification(string identification)
        {
            if (string.IsNullOrEmpty(identification))
            {
                throw new ArgumentNullException("Identificationは文字列です");
            }
            Value = identification;
        }

        public static bool operator ==(Identification me, object other) => me?.Equals(other) == true;

        public static bool operator !=(Identification me, object other) => !me?.Equals(other) == true;
    }

    internal static class IdentificationExtension
    {
        public static Identification ToIdentification(this string val) => val == null ? null : new Identification(val);
    }
}
