using System;

namespace JP.DataHub.ManageApi.Models.VendorRepositoryGroup
{
    /// <summary>
    /// VendorRepositoryGroup情報
    /// </summary>
    public class VendorRepositoryGroupViewModel
    {
        public Guid VendorId { get; set; }
        public Guid RepositoryGroupId { get; set; }
        public string RepositoryGroupName { get; set; }
        public bool Used { get; set; }
    }
}