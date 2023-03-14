
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    /// サンプルコード情報
    /// </summary>
    public class SampleCodeModel
    {
        /// <summary>メソッドID</summary>
        public Guid MethodId { get; set; }

        /// <summary>サンプルコードID</summary>
        public Guid SampleCodeId { get; set; }

        /// <summary>言語</summary>
        public string Language { get; set; }

        /// <summary>表示順</summary>
        public int DisplayOrder { get; set; }

        /// <summary>サンプルコード</summary>
        public string Code { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
