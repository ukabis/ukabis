
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    /// カテゴリー情報
    /// </summary>
    public class CategoryModel
    {
        /// <summary>API ID</summary>
        public Guid ApiId { get; set; }

        /// <summary>カテゴリーID</summary>
        public Guid CategoryId { get; set; }

        /// <summary>カテゴリー名</summary>
        public string CategoryName { get; set; }

        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
