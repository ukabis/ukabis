using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    internal class PhysicalRepositoryResultModel
    {
        public Guid PhysicalRepositoryId { get; set; }

        public Guid RepositoryGroupId { get; set; }

        public string ConnectionString { get; set; }

        public bool IsFull { get; set; }

        public bool IsActive { get; set; }
    }
}
