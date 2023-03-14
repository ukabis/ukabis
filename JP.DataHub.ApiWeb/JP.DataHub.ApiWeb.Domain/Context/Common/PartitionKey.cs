using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record PartitionKey : IValueObject
    {
        public string Value { get; }

        public string BaseString { get { return _baseString; } }

        public IEnumerable<string> LogicalKeys
        {
            get { return _logicalKeys; }
        }

        public bool IsExsitsLogicalKey
        {
            get { return _logicalKeys.Any(); }
        }

        private string _baseString;

        private IEnumerable<string> _logicalKeys;


        public PartitionKey(string value)
        {
            this.Value = value;
            this._logicalKeys = EditLogicalKeys().ToList();
            this._baseString = EditBaseString();
        }

        private IEnumerable<string> EditLogicalKeys()
        {

            if (string.IsNullOrEmpty(Value) == true)
            {
                yield break;
            }
            foreach (var key in Value.Split("/".ToCharArray()).ToList().Where(x => string.IsNullOrEmpty(x) == false))
            {
                if (key.StartsWith("{") == true && key.EndsWith("}") == true)
                {
                    yield return key.Substring(1, key.Length - 2);

                }
            }
            yield break;
        }

        private string EditBaseString()
        {
            if (string.IsNullOrEmpty(Value))
            {
                return "";
            }
            return string.Join("~", Value.Split('/').Where(x => string.IsNullOrEmpty(x) == false && !(x.StartsWith("{") == true && x.EndsWith("}") == true)));
        }

        public static bool operator ==(PartitionKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(PartitionKey me, object other) => !me?.Equals(other) == true;
    }
}
