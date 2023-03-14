using System;

namespace JP.DataHub.ManageApi.Models.System
{
    public class SystemLinkViewModel
    {
        /// <summary>
        /// SystemリンクID
        /// </summary>
        public string SystemLinkId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// リンク表示名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// リンク詳細
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// リンクURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// APIリンク表示フラグ
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// デフォルトかどうか
        /// </summary>
        public bool IsDefault { get; set; } = false;
    }
}
