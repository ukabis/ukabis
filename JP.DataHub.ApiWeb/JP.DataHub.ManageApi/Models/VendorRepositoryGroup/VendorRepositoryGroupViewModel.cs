using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.VendorRepositoryGroup
{
    /// <summary>
    /// VendorRepositoryGroup情報
    /// </summary>
    public class VendorRepositoryGroupListViewModel
    {
        public string VendorId { get; set; }
        public List<VendorRepositoryGroupListItemsViewModel> VendorRepositoryGroupItems { get; set; }
    }

    public class VendorRepositoryGroupListItemsViewModel
    {
        public string RepositoryGroupId { get; set; }
        public string RepositoryGroupName { get; set; }
        public bool Used { get; set; }
    }
}