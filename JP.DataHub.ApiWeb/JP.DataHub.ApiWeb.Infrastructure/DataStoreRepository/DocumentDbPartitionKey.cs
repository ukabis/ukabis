using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    internal class DocumentDbPartitionKey
    {
        private const string SEPARATOR = "~";

        public string Value { get; }

        public DocumentDbPartitionKey(string value)
        {

            Value = value;
        }


        public static bool CreateQueryPartition(PartitionKey partitionKey, RepositoryKey repositoryKey, IsVendor isVendor, VendorId vendorId, SystemId systemId, IsPerson isPerson, OpenId openId, ResourceVersion version, QueryStringVO query, UrlParameter keyValue, IsOverPartition isOverPartition, out DocumentDbPartitionKey documentDbPartitionKey)
        {
            documentDbPartitionKey = null;
            if (partitionKey == null && repositoryKey == null && isVendor == null && vendorId == null && systemId == null && isPerson == null && openId == null && version == null && query == null && keyValue == null && isOverPartition == null)
            {
                return false;
            }

            var partitionKeyBase = CreateBase(partitionKey, repositoryKey, isVendor, vendorId, systemId, isPerson, openId);
            IEnumerable<string> logicalkeys = null;
            var logicalKey = new StringBuilder();
            if (isOverPartition.Value)
            {
                documentDbPartitionKey = null;
                return false;
            }
            if (partitionKey != null && !string.IsNullOrEmpty(partitionKey.Value))
            {
                logicalkeys = partitionKey.LogicalKeys;
            }
            else
            {
                logicalkeys = null;
                documentDbPartitionKey = new DocumentDbPartitionKey($"{partitionKeyBase}{SEPARATOR}{version.Value}");
                return true;

            }
            bool first = true;
            Dictionary<string, string> concatParams = new Dictionary<string, string>();
            if (query != null)
            {
                foreach (var queryKeyValue in query.Dic)
                {
                    if (!concatParams.ContainsKey(queryKeyValue.Key.Value))
                    {
                        concatParams.Add(queryKeyValue.Key.Value, queryKeyValue.Value.Value);
                    }
                }
            }
            if (keyValue != null)
            {
                foreach (var keyValueKeyValue in keyValue.Dic)
                {
                    if (!concatParams.ContainsKey(keyValueKeyValue.Key.Value))
                    {
                        concatParams.Add(keyValueKeyValue.Key.Value, keyValueKeyValue.Value.Value);
                    }

                }
            }
            if (logicalkeys != null && logicalkeys.Any())
            {
                foreach (var key in logicalkeys)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        logicalKey.Append(SEPARATOR);
                    }
                    if (concatParams.ContainsKey(key))
                    {
                        logicalKey.Append(concatParams[key]);
                    }
                    else
                    {
                        documentDbPartitionKey = null;
                        return false;
                    }
                }
                documentDbPartitionKey = new DocumentDbPartitionKey($"{partitionKeyBase}{SEPARATOR}{version.Value}{SEPARATOR}{logicalKey.ToString()}");
                return true;
            }
            documentDbPartitionKey = new DocumentDbPartitionKey(partitionKeyBase);
            return true;
        }

        public static DocumentDbPartitionKey CreateRegisterPartition(PartitionKey partitionKey, RepositoryKey repositoryKey, IsVendor isVendor, VendorId vendorId, SystemId systemId, IsPerson isPerson, OpenId openId, ResourceVersion version, JToken json)
        {
            var partitionKeyBase = CreateBase(partitionKey, repositoryKey, isVendor, vendorId, systemId, isPerson, openId);
            IEnumerable<string> logicalkeys = null;
            var logicalKey = new StringBuilder();
            if (partitionKey != null && !string.IsNullOrEmpty(partitionKey.Value))
            {
                logicalkeys = partitionKey.LogicalKeys;
            }
            else
            {
                logicalkeys = null;
                return new DocumentDbPartitionKey($"{partitionKeyBase}{SEPARATOR}{version.Value}");

            }
            bool first = true;
            if (logicalkeys != null && logicalkeys.Any())
            {
                foreach (var key in logicalkeys)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        logicalKey.Append(SEPARATOR);
                    }
                    if (json[key] != null)
                    {
                        logicalKey.Append(json[key]);
                    }
                }
                return new DocumentDbPartitionKey($"{partitionKeyBase}{SEPARATOR}{version.Value}{SEPARATOR}{logicalKey.ToString()}");
            }
            return new DocumentDbPartitionKey(partitionKeyBase);
        }


        private static string CreateBase(PartitionKey partitionKey, RepositoryKey repositoryKey, IsVendor isVendor, VendorId vendorId, SystemId systemId, IsPerson isPerson, OpenId openId)
        {
            var partitionKeyBase = new StringBuilder();
            if (partitionKey != null && !string.IsNullOrEmpty(partitionKey.Value))
            {
                partitionKeyBase.Append(partitionKey.BaseString);
            }
            else
            {
                partitionKeyBase.Append(repositoryKey.Type);
            }
            if (isVendor.Value)
            {
                if (!string.IsNullOrEmpty(vendorId.Value))
                {
                    partitionKeyBase.Append($"{SEPARATOR}{vendorId.Value}");
                }
                if (!string.IsNullOrEmpty(systemId.Value))
                {
                    partitionKeyBase.Append($"{SEPARATOR}{systemId.Value}");
                }
            }
            if (isPerson.Value)
            {
                if (!string.IsNullOrEmpty(openId.Value))
                {
                    partitionKeyBase.Append($"{SEPARATOR}{openId.Value}");
                }
            }
            return partitionKeyBase.ToString();
        }
    }
}
