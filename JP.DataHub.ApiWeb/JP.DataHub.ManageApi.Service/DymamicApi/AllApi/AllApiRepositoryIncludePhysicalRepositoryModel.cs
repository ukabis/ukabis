using MessagePack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.DymamicApi.Actions.AllApi
{
    [MessagePackObject]
    internal class AllApiRepositoryIncludePhysicalRepositoryModel
    {
        [Key(0)]
        public string api_id { get; set; }
        [Key(1)]
        public string? repository_group_id { get; set; }
        [Key(2)]
        public string repository_type_cd { get; set; }
        [Key(3)]
        public bool is_primary { get; set; }
        [Key(4)]
        public bool is_enable { get; set; }
        [Key(5)]
        public string repository_connection_string { get; set; }
        [Key(6)]
        public bool is_full { get; set; }
        [Key(7)]
        public bool is_secondary_primary { get; set; }
        [Key(8)]
        public string? physical_repository_id { get; set; }
        [Key(9)]
        public string? controller_id { get; set; }
    }
}
