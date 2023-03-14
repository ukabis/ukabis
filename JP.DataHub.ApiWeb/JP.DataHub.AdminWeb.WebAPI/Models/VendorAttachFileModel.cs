using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    /// <summary>
    /// ベンダー情報
    /// </summary>
    public class VendorAttachFileModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// 添付ファイルストレージID
        /// </summary>
        public string AttachFileStorageId { get; set; }
    }
}
