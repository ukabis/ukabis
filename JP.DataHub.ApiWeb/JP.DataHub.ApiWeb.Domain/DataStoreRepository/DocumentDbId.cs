using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;

namespace JP.DataHub.ApiWeb.Domain.DataStoreRepository
{
    internal class DocumentDbId
    {
        private const string SEPARATOR = "~";
        private const string REPLACECHAR = "^";

        public string Path { get; }

        public string PhysicalId { get; }

        public string LogicalId { get; }

        public DocumentDbId(string physicalId, string path, string logicalId)
        {
            PhysicalId = physicalId;
            Path = path;
            LogicalId = logicalId;
        }

        public static string CreatePath(RepositoryKey key, IsVendor isVendor, VendorId vendorId, SystemId systemId, IsPerson isPerson, OpenId openId, ResourceVersion version)
        {
            var path = new StringBuilder(key.Type);

            if (isPerson.Value && !string.IsNullOrEmpty(openId.Value))
            {
                path.Append($"{SEPARATOR}{openId.Value}");
            }
            if (isVendor.Value)
            {
                if (!string.IsNullOrEmpty(vendorId.Value))
                {
                    path.Append($"{SEPARATOR}{vendorId.Value}");
                }
                if (!string.IsNullOrEmpty(systemId.Value))
                {
                    path.Append($"{SEPARATOR}{systemId.Value}");
                }
            }
            path.Append($"{SEPARATOR}{version.Value}");
            return path.ToString();
        }

        public static string CreateKey(RepositoryKey key, IsAutomaticId isAutomaticId, JToken json)
        {
            var logicalId = new StringBuilder();
            bool first = true;
            foreach (var logicalKey in key.LogicalKeys)
            {
                var temp = json[logicalKey];
                if (temp == null && (isAutomaticId.Value == false || (isAutomaticId.Value == true && key.LogicalKeys.Count() > 1)))
                {
                    throw new Exception($"{logicalKey} propety not found");
                }

                if (temp != null)
                {
                    object obj = temp.Value<object>();
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        logicalId.Append(SEPARATOR);
                    }

                    logicalId.Append($"{obj?.ToString().Replace("/", REPLACECHAR) ?? "{null}"}");
                }
            }

            if (!isAutomaticId.Value)
            {
                return logicalId.ToString();
            }

            //自動採番する場合
            //自動採番ONのAPIでkeyの値が指定された場合、指定されたkeyがGuidならそれをkeyとして採用(自動採番しない)
            if (key.IsLogicalKeyOnce && Guid.TryParse(logicalId.ToString(), out var _))
            {
                return logicalId.ToString();
            }

            //それ以外の場合なら自動採番する
            if (!first)
            {
                logicalId.Append(SEPARATOR);
            }
            logicalId.Append($"{Guid.NewGuid()}");
            return logicalId.ToString();
        }

        public static DocumentDbId Create(RepositoryKey key, IsVendor isVendor, VendorId vendorId, SystemId systemId, IsPerson isPerson, OpenId openId, IsAutomaticId isAutomaticId, ResourceVersion version, JToken json)
        {
            var path = CreatePath(key, isVendor, vendorId, systemId, isPerson, openId, version);
            var id = CreateKey(key, isAutomaticId, json);
            return new DocumentDbId($"{path}{SEPARATOR}{id}", path, id);
        }
    }
}
