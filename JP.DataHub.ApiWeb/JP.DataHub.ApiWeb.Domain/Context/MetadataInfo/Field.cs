using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// 分野情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class Field : IValueObject
    {
        /// <summary>API ID</summary>
        [Key(0)]
        public Guid ApiId { get; set; }

        /// <summary>分野ID</summary>
        [Key(1)]
        public Guid FieldId { get; set; }

        /// <summary>親分野ID</summary>
        [Key(2)]
        public Guid ParentFieldId { get; set; }

        /// <summary>分野名</summary>
        [Key(3)]
        public string FieldName { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(4)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(5)]
        public DateTime UpdDate { get; set; }
    }
}
