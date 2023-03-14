using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class VendorRepositoryGroupModel
    {
        public string VendorId { get; set; }
        public string RepositoryGroupId { get; set; }
        public string RepositoryGroupName { get; set; }
        public bool Used { get; set; }
        public bool Active { get; set; }
    }

    public class VendorRepositoryGroupListModel
    {
        public string VendorId { get; set; }

        public List<VendorRepositoryGroupListItemsModel> VendorRepositoryGroupListItems { get; set; }
    }

    public class VendorRepositoryGroupListItemsModel
    {
        public string RepositoryGroupId { get; set; }

        public string RepositoryGroupName { get; set; }

        public bool Used { get; set; }
    }
}