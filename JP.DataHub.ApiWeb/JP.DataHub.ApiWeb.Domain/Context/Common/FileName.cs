using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record FileName : IValueObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileName" /> class.
        /// </summary>
        public FileName(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static bool operator ==(FileName me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FileName me, object other) => !me?.Equals(other) == true;
    }

    internal static class FileNameExtension
    {
        public static FileName ToFileName(this string? val) => val == null ? null : new FileName(val);
    }
}
