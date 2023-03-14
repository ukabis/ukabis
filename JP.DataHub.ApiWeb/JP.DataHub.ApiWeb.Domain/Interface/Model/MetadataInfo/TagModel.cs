
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    /// タグ情報
    /// </summary>
    public class TagModel
    {
        /// <summary>API ID</summary>
        public Guid ApiId { get; set; }

        /// <summary>タグID</summary>
        public Guid TagId { get; set; }

        /// <summary>親タグID</summary>
        public Guid ParentTagId { get; set; }

        /// <summary>タグタイプID</summary>
        public Guid TagTypeId { get; set; }

        /// <summary>タグタイプ名</summary>
        public string TagTypeName { get; set; }

        /// <summary>タグ名</summary>
        public string TagName { get; set; }

        /// <summary>コード</summary>
        public string Code { get; set; }

        /// <summary>コード2</summary>
        public string Code2 { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
