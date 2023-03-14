using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ApiWeb.Core.Model
{
    // .NET6
    public class AsyncResultJsonModel
    {
        public string Status { get; set; }

        public DateTime? RequestDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ResultPath { get; set; }

        public string RequestId { get; set; }

    }
}
