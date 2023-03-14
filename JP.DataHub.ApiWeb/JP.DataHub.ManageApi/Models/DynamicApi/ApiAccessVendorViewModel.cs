using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    /// <summary>
    /// APIアクセスベンダーモデル
    /// </summary>
    public class ApiAccessVendorViewModel
    {
        /// <summary>
        /// APIアクセスベンダーID
        /// </summary>
        public string ApiAccessVendorId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// アクセスキー
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// 有効か
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
