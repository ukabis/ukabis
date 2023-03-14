using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class SchemaModel
    {
        /// <summary>
        /// スキーマID
        /// </summary>
        public Guid SchemaId { get; set; }

        /// <summary>
        /// スキーマ名
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// JSONスキーマ
        /// </summary>
        public string JsonSchema { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdDate { get; set; }
    }
}
