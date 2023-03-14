using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// サンプルコード情報
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public class SampleCode : IValueObject
    {
        /// <summary>メソッドID</summary>
        [Key(0)]
        public Guid MethodId { get; set; }

        /// <summary>サンプルコードID</summary>
        [Key(1)]
        public Guid SampleCodeId { get; set; }

        /// <summary>言語</summary>
        [Key(2)]
        public string Language { get; set; }

        /// <summary>表示順</summary>
        [Key(3)]
        public int DisplayOrder { get; set; }

        /// <summary>サンプルコード</summary>
        [Key(4)]
        public string Code { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(5)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(6)]
        public DateTime UpdDate { get; set; }
    }
}
