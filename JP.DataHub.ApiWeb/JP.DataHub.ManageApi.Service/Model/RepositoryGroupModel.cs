using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class RepositoryGroupModel
    {
        public string RepositoryGroupId { get; set; }

        public string RepositoryGroupName { get; set; }

        public string RepositoryTypeCd { get; set; }

        public string RepositoryTypeName { get; set; }

        public int SortNo { get; set; }

        public bool IsDefault { get; set; }

        public bool? IsEnable { get; set; }

        public IEnumerable<RepositoryGroupVendorModel> VendorList { get; set; }

        public List<PhysicalRepositoryModel> PhysicalRepositoryList { get; set; }
    }

    public class RepositoryGroupVendorModel
    {
        public string VendorId { get; set; }

        public bool IsUsed { get; set; }
    }
}

