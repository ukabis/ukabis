using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.TrailEventProcess.Models
{
    public class StatusManagementModel
    {
        public string RequestId { get; set; }
        public string Status { get; set; }
        public DateTime? EndDate { get; set; }
        public string ResultPath { get; set; }
        public TimeSpan? ExecutionTime { get; set; }

    }
}
