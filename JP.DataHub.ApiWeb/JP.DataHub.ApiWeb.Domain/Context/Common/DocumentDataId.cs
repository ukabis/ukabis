using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.Common
{
    internal record DocumentDataId : IValueObject
    {
        public string Id { get; }
        public string Path { get; }
        public string LogicalId { get; }

        public DocumentDataId(string id, string path, string logicalId)
        {
            Id = id;
            Path = path;
            LogicalId = logicalId;
        }

        public static bool operator ==(DocumentDataId me, object other) => me?.Equals(other) == true;

        public static bool operator !=(DocumentDataId me, object other) => !me?.Equals(other) == true;
    }
}
