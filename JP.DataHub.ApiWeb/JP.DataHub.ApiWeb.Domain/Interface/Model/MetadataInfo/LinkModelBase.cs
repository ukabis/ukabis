
namespace JP.DataHub.ApiWeb.Domain.Interface.Model.MetadataInfo
{
    /// <summary>
    /// リンク情報の基底クラス
    /// </summary>
    public abstract class LinkModelBase
    {
        /// <summary>タイトル</summary>
        public string Title { get; set; }

        /// <summary>詳細</summary>
        public string Detail { get; set; }

        /// <summary>URL</summary>
        public string Url { get; set; }

        /// <summary>表示フラグ</summary>
        public bool IsVisible { get; set; }

        /// <summary>有効フラグ</summary>
        public bool IsActive { get; set; }

        /// <summary>更新日時</summary>
        public DateTime UpdDate { get; set; }
    }
}
