using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Database
{
    // .NET6
    [MessagePackObject]
    internal class AllApiRepositoryIncludePhysicalRepositoryModel
    {
        [Key(0)]
        public Guid api_id { get; set; }
        [Key(1)]
        public Guid? repository_group_id { get; set; }
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
        public Guid? physical_repository_id { get; set; }
        [Key(9)]
        public Guid? controller_id { get; set; }
    }
}
