using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record FileId : IValueObject
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

        public FileId(string value)
        {
            this.Value = value;
        }

        public FileId(Guid value)
        {
            this.Value = value.ToString();
        }

        public static bool operator ==(FileId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(FileId me, object other) => !me?.Equals(other) == true;
    }

    internal static class FileIddExtension
    {
        public static FileId ToFileId(this string val) => val == null ? null : new FileId(val);
        public static FileId ToFileId(this Guid? val) => val == null ? null : new FileId(val.Value);
        public static FileId ToFileId(this Guid val) => new FileId(val);
    }
}
