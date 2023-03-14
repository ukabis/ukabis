using System.Text;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Infrastructure.Data.EventHub
{
    internal class EventHubPartitionKey
    {

        private const string SEPARATOR = "~";

        public string Value { get; }

        public EventHubPartitionKey(string value)
        {

            Value = value;
        }
        public static EventHubPartitionKey CreateRegisterPartition(PartitionKey partitionKey, IsVendor isVendor, VendorId vendorId, SystemId systemId, JToken json)
        {
            var partitionKeyBase = CreateBase(partitionKey, isVendor, vendorId, systemId);

            IEnumerable<string> logicalkeys = null;
            var logicalKey = new StringBuilder();
            if (partitionKey != null && !string.IsNullOrEmpty(partitionKey.Value))
            {
                logicalkeys = partitionKey.LogicalKeys;
            }
            else
            {
                return null;

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
                return new EventHubPartitionKey($"{partitionKeyBase}{SEPARATOR}{logicalKey.ToString()}");
            }
            return new EventHubPartitionKey(partitionKeyBase);
        }


        private static string CreateBase(PartitionKey partitionKey, IsVendor isVendor, VendorId vendorId, SystemId systemId)
        {
            var partitionKeyBase = new StringBuilder();
            if (partitionKey != null && !string.IsNullOrEmpty(partitionKey.Value))
            {
                partitionKeyBase.Append(partitionKey.BaseString);
            }
            else
            {
                return null;
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
            return partitionKeyBase.ToString();
        }
    }
}
