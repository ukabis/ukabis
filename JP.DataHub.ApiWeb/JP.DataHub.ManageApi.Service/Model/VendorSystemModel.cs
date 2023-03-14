using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ManageApi.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    public class VendorSystemModel
    {
        public string SystemId { get; set; }
        public string SystemName { get; set; }
        public string ApplicationId { get; set; }
        public DateTime SystemUpdDate { get; set; }
        public bool IsEnableSystem { get; set; }
    }
}
