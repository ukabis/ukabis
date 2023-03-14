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
    public class SystemAdminModel
    {
        public string SystemId { get; set; }
        public string AdminSecret { get; set; }
        public bool IsActive { get; set; }
    }
}
