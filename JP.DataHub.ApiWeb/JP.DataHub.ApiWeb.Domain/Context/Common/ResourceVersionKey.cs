using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record ResourceVersionKey : IValueObject
    {
        private const string SEPARATOR = "~";
        private const string VERSION_VALUE = "version";

        public string PartitionKey { get; }

        public string Type { get; }

        public string Id { get; }

        public ResourceVersionKey(RepositoryKey repositoryKey)
        {
            Type = $"{repositoryKey.Type}{SEPARATOR}{VERSION_VALUE}";
            PartitionKey = VERSION_VALUE;
            Id = $"{repositoryKey.Type}{SEPARATOR}{VERSION_VALUE}";
        }

        public ResourceVersionKey(string partitionKey, string type, string id)
        {
            PartitionKey = partitionKey;
            Type = type;
            Id = id;
        }

        public Dictionary<string, object> Dictionary { get => ToDictionary(); }

        private Dictionary<string, object> dic;

        private Dictionary<string, object> ToDictionary()
        {
            if (dic == null)
            {
                dic = new Dictionary<string, object>();
                dic.Add("id", Id);
                dic.Add("_partitionkey", PartitionKey);
                dic.Add("_Type", Type);
            }
            return dic;
        }

        public static bool operator ==(ResourceVersionKey me, object other) => me?.Equals(other) == true;

        public static bool operator !=(ResourceVersionKey me, object other) => !me?.Equals(other) == true;
    }
}