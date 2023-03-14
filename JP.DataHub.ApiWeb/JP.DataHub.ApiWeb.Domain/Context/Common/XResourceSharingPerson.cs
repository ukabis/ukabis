using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record XResourceSharingPerson : IValueObject
    {
        public string Value { get; }

        public XResourceSharingPerson(string value)
        {
            Value = value;
        }

        public static bool operator ==(XResourceSharingPerson me, object other) => me?.Equals(other) == true;

        public static bool operator !=(XResourceSharingPerson me, object other) => !me?.Equals(other) == true;
    }

    internal static class XResourceSharingPersonExtension
    {
        public static XResourceSharingPerson ToXResourceSharingPerson(this string val) => new XResourceSharingPerson(val);
    }
}
