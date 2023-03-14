using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.DynamicApi
{
    public class RegisterSchemaRequestViewModel
    {
        /// <summary>
        /// スキーマID(未指定の場合は自動生成)
        /// </summary>
        public string SchemaId { get; set; }

        /// <summary>
        /// スキーマ名
        /// </summary>
        [Required]
        [StringLength(260)]
        public string SchemaName { get; set; }

        /// <summary>
        /// JSONスキーマ
        /// </summary>
        [Required]
        public string JsonSchema { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        [StringLength(4000)]
        public string Description { get; set; }

        /// <summary>
        /// ベンダーID
        /// Nullの場合はRequestDataの中のVendorIdを使用する。
        /// このプロパティを指定した場合、こちらが優先される
        /// </summary>
        public string VendorId { get; set; }
    }
}
