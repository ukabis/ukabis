using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi
{
    [MessagePackObject]
    internal class AllApiPhysicalRepositoryModel
    {
        [Key(0)]
        public string repository_connection_string { get; set; }
        [Key(1)]
        public bool is_full { get; set; }
        [Key(2)]
        public string? PhysicalRepositoryId { get; set; }
    }
}
