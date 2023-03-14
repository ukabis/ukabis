using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;

namespace JP.DataHub.ApiWeb.Infrastructure.Models.Database
{
    [MessagePackObject]
    public class DB_ApiAccessVendorSelect
    {
        [Key(0)]
        public Guid vendor_id { get; set; }

        [Key(1)]
        public Guid access_key { get; set; }

        [Key(2)]
        public Guid system_id { get; set; }
    }
}
