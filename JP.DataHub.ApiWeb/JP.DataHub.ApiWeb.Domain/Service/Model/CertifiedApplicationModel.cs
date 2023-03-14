using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Domain.Service.Model
{
    [Serializable]
    [MessagePackObject(keyAsPropertyName: true)]
    internal class CertifiedApplicationModel
    {
        public string CertifiedApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string VendorId { get; set; }
        public string SystemId { get; set; }
    }
}
