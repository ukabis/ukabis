using System;

namespace JP.DataHub.ManageApi.Models.Document
{
    public class FileViewModel
    {
        /// <summary>
        /// ファイルID
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// ファイル名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 有効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 更新日時
        /// </summary>
        public DateTime UpdDate { get; set; }

        /// <summary>
        /// 未削除
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// 並び順
        /// </summary>
        public int OrderNo { get; set; }
    }
}