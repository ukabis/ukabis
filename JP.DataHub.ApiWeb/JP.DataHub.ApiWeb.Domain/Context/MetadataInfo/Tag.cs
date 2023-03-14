using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// タグ情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class Tag : IValueObject
    {
        /// <summary>API ID</summary>
        [Key(0)]
        public Guid ApiId { get; set; }

        /// <summary>タグID</summary>
        [Key(1)]
        public Guid TagId { get; set; }

        /// <summary>親タグID</summary>
        [Key(2)]
        public Guid ParentTagId { get; set; }

        /// <summary>タグタイプID</summary>
        [Key(3)]
        public Guid TagTypeId { get; set; }

        /// <summary>タグタイプ名</summary>
        [Key(4)]
        public string TagTypeName { get; set; }

        /// <summary>タグ名</summary>
        [Key(5)]
        public string TagName { get; set; }

        /// <summary>コード</summary>
        [Key(6)]
        public string Code { get; set; }

        /// <summary>コード2</summary>
        [Key(7)]
        public string Code2 { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(8)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(9)]
        public DateTime UpdDate { get; set; }
    }
}
