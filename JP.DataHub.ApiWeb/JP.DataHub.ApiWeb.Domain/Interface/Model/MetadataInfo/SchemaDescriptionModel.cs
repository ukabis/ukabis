
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    ///  スキーマ情報
    /// </summary>
    public class SchemaDescriptionModel
    {
        /// <summary>スキーマID</summary>
        public Guid SchemaId { get; set; }

        /// <summary>スキーマ名</summary>
        public string SchemaName { get; set; }

        /// <summary>JSONスキーマ</summary>
        public string JsonSchema { get; set; }

        /// <summary>説明</summary>
        public string Description { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
