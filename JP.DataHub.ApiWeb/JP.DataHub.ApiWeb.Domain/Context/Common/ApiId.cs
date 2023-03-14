using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ApiId : IValueObject
    {
        public string Value { get; }

        public Guid? ToGuid
        {
            get
            {
                Guid tmp;
                if (Guid.TryParse(Value, out tmp))
                {
                    return tmp;
                }
                else
                {
                    return null;
                }
            }
        }

        public ApiId(string value)
        {
            this.Value = value;
        }

        public static bool operator ==(ApiId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ApiId me, object other) => !me?.Equals(other) == true;
    }

    internal static class ApiIdExtension
    {
        public static ApiId ToApiId(this string val) => val == null ? null : new ApiId(val);
        public static ApiId ToApiId(this Guid? val) => val == null ? null : new ApiId(val.ToString());
        public static ApiId ToApiId(this Guid val) => new ApiId(val.ToString());
    }
}
