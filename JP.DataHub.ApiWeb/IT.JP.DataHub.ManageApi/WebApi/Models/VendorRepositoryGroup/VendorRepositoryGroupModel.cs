﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.VendorRepositoryGroup
{
    public class VendorRepositoryGroupModel
    {
        public string VendorId { get; set; }
        public string RepositoryGroupId { get; set; }
        public string RepositoryGroupName { get; set; }
        public bool Used { get; set; }
    }
    /// <summary>
    /// VendorRepositoryGroup情報
    /// </summary>
    public class VendorRepositoryGroupListModel
    {
        public string VendorId { get; set; }
        public List<VendorRepositoryGroupListItemsModel> VendorRepositoryGroupItems { get; set; }
    }

    public class VendorRepositoryGroupListItemsModel
    {
        public string RepositoryGroupId { get; set; }
        public string RepositoryGroupName { get; set; }
        public bool Used { get; set; }
    }
    public class ActivateVendorRepositoryGroupModel
    {
        public string VendorId { get; set; }
        public string RepositoryGroupId { get; set; }
        public bool Active { get; set; }
    }
}
