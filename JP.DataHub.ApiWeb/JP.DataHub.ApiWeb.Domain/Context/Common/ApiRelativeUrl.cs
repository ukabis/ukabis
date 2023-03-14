using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record ApiRelativeUrl : IValueObject
    {
        public string Value { get; }

        public ApiRelativeUrl(string url)
        {
            this.Value = url;
        }

        private Lazy<List<string>> _split => new Lazy<List<string>>(() => new List<string>(Value.Split("/")));

        public List<string> Split { get => _split.Value; }

        public static bool operator ==(ApiRelativeUrl me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiRelativeUrl me, object other) => !me?.Equals(other) == true;
    }

    internal static class ApiRelativeUrlExtension
    {
        public static ApiRelativeUrl ToApiRelativeUrl(this string val) => val == null ? null : new ApiRelativeUrl(val);
    }
}
