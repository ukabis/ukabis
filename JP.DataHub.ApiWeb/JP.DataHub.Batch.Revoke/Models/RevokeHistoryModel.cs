using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.Revoke.Models
{
    public class RevokeHistoryModel
    {
        public string RevokeHistoryId { get; set; }
        public string ControllerId { get; set; }
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
    }
}
