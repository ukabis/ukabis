using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.Com.DDD;
using MessagePack;

namespace JP.DataHub.ApiWeb.Domain.Context.Authentication
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public record class ClientVendorSystem : IEntity
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string vendor_id { get; set; }
        public string system_id { get; set; }
        public long accesstoken_expiration_timespan { get; set; }
        public bool is_active { get; set; }
    }
}
