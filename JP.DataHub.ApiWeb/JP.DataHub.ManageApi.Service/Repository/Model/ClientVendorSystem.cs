using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Repository.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    internal class ClientVendorSystem
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string vendor_id { get; set; }
        public string system_id { get; set; }
        public long accesstoken_expiration_timespan { get; set; }
        public bool is_active { get; set; }
    }
}