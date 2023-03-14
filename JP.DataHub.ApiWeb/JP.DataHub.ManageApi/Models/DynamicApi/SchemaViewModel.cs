using Newtonsoft.Json;
using System;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class SchemaViewModel
    {
        /// <summary>
        /// スキーマID
        /// </summary>
        public string SchemaId { get; set; }

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
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdDate { get; set; }
    }
}
