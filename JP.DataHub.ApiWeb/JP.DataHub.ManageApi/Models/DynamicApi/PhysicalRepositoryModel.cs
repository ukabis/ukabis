using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class PhysicalRepositoryModel
    {
        public string PhysicalRepositoryId { get; set; }

        public string RepositoryGroupId { get; set; }

        public string ConnectionString { get; set; }

        public bool IsFull { get; set; }

        public bool IsActive { get; set; }
    }
}
