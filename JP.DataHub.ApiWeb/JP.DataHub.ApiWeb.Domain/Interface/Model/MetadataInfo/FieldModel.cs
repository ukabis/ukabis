
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    /// 分野情報
    /// </summary>
    public class FieldModel
    {
        /// <summary>API ID</summary>
        public Guid ApiId { get; set; }

        /// <summary>分野ID</summary>
        public Guid FieldId { get; set; }

        /// <summary>親分野ID</summary>
        public Guid ParentFieldId { get; set; }

        /// <summary>分野名</summary>
        public string FieldName { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
