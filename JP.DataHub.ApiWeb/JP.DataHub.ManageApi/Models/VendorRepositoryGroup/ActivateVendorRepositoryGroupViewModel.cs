using System;

namespace JP.DataHub.ManageApi.Models.VendorRepositoryGroup
{
    /// <summary>
    /// VendorRepositoryGroup有効・無効化情報
    /// </summary>
    public class ActivateVendorRepositoryGroupViewModel
    {
        public Guid VendorId { get; set; }
        public Guid RepositoryGroupId { get; set; }
        public bool Active { get; set; }
    }
}