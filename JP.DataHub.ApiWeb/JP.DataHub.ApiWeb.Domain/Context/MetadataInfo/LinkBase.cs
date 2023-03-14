using MessagePack;
using JP.DataHub.Com.DDD;

namespace JP.DataHub.ApiWeb.Domain.Context.MetadataInfo
{
    /// <summary>
    /// リンク情報の基底クラス
    /// </summary>
    [Serializable]
    [MessagePackObject]
    public abstract class LinkBase : IValueObject
    {
        /// <summary>タイトル</summary>
        [Key(0)]
        public string Title { get; set; }

        /// <summary>詳細</summary>
        [Key(1)]
        public string Detail { get; set; }

        /// <summary>URL</summary>
        [Key(2)]
        public string Url { get; set; }

        /// <summary>表示フラグ</summary>
        [Key(3)]
        public bool IsVisible { get; set; }

        /// <summary>有効フラグ</summary>
        [Key(4)]
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        [Key(5)]
        public DateTime UpdDate { get; set; }
    }
}
