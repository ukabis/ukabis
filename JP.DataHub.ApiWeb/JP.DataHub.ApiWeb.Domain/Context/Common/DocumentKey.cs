using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DocumentKey : IValueObject
    {
        private const string SEPARATOR = "~";
        private const string VERSION_VALUE = "documentversion";

        public string PartitionKey { get; }

        public string Type { get; }

        public string Id { get; }

        public Dictionary<string, object> Dictionary { get => ToDictionary(); }

        private Dictionary<string, object> _dic;

        public DocumentKey(RepositoryKey repositoryKey, string id)
        {
            Type = $"{repositoryKey.Type}{SEPARATOR}{VERSION_VALUE}";
            PartitionKey = VERSION_VALUE;
            Id = id;
        }

        private Dictionary<string, object> ToDictionary()
        {
            if (_dic == null)
            {
                _dic = new Dictionary<string, object>();
                _dic.Add("id", Id);
                _dic.Add("_partitionkey", PartitionKey);
                _dic.Add("_Type", Type);
            }
            return _dic;
        }

        public static bool operator ==(DocumentKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentKey me, object other) => !me?.Equals(other) == true;
    }
}
