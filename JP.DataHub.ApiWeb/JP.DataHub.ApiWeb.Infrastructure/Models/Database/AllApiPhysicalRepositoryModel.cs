using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Database
{
    [MessagePackObject]
    public class AllApiPhysicalRepositoryModel
    {
        [Key(0)]
        public string repository_connection_string { get; set; }
        [Key(1)]
        public bool is_full { get; set; }
        [Key(2)]
        public Guid? PhysicalRepositoryId { get; set; }
    }
}
