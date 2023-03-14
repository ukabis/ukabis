using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models
{
    [Serializable]
    public class RepositoryGroupModel
    {
        public string RepositoryGroupId { get; set; }
        public string RepositoryGroupName { get; set; }
        public string RepositoryTypeCd { get; set; }
        public string SortNo { get; set; }

        public string IsDefault { get; set; }

        public string IsEnable { get; set; }

        public List<PhysicalRepositoryModel> PhysicalRepositoryList { get; set; }
    }
    [Serializable]
    public class PhysicalRepositoryModel
    {
        public string PhysicalRepositoryId { get; set; }

        public string ConnectionString { get; set; }

        public bool IsFull { get; set; }

        public bool IsActive { get; set; }
    }
}
