using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// カテゴリー情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class Category : IValueObject
    {
        /// <summary>API ID</summary>
        [Key(0)]
        public Guid ApiId { get; set; }

        /// <summary>カテゴリーID</summary>
        [Key(1)]
        public Guid CategoryId { get; set; }

        /// <summary>カテゴリー名</summary>
        [Key(2)]
        public string CategoryName { get; set; }

        /// <summary>表示順</summary>
        [Key(3)]
        public int DisplayOrder { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(4)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(5)]
        public DateTime UpdDate { get; set; }
    }
}
