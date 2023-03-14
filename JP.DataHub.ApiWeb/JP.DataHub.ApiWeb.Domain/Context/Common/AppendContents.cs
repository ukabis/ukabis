using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    public record AppendContents : IValueObject
    {
        public string Value { get; }

        public AppendContents(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(AppendContents me, object other) => me?.Equals(other) == true;

        public static bool operator !=(AppendContents me, object other) => !me?.Equals(other) == true;
    }

    internal static class AppendContentsNameExtension
    {
        public static AppendContents ToAppendContents(this string val) => val == null ? null : new AppendContents(val);
    }
}