using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record FilePath : IValueObject
    {
        public string Value { get; }

        public bool IsAbsolutePath { get; }

        public FilePath(string value, bool isAbsolutePath = false)
        {
            Value = value;
            IsAbsolutePath = isAbsolutePath;
        }

        public static bool operator ==(FilePath me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FilePath me, object other) => !me?.Equals(other) == true;
    }
}
