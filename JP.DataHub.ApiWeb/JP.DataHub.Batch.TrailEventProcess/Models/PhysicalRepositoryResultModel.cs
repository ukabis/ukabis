using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Batch.TrailEventProcess.Models
{
    public class PhysicalRepositoryResultModel
    {
        public Guid PhysicalRepositoryId { get; set; }

        public Guid RepositoryGroupId { get; set; }

        public string ConnectionString { get; set; }

        public bool IsFull { get; set; }

        public bool IsActive { get; set; }
    }
}
