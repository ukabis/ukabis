using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// VendorRepositoryGroup有効・無効化情報
    /// </summary>
    public class ActivateVendorRepositoryGroupModel
    {
        public string VendorId { get; set; }
        public string RepositoryGroupId { get; set; }
        public bool Active { get; set; }
    }
}
