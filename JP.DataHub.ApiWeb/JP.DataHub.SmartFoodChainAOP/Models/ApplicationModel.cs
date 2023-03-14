using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.SmartFoodChainAOP.Models
{
    [MessagePackObject(true)]
    [Serializable]
    public class ApplicationModel
    {
        public string ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
        public List<string> Manager { get; set; }
        public bool IsEnable { get; set; }
    }
}
