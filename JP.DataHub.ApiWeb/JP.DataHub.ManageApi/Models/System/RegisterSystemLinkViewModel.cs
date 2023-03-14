using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.System
{
    public class RegisterSystemLinkViewModel
    {
        /// <summary>
        /// SystemリンクID
        /// </summary>
        [ValidateGuid]
        public string SystemLinkId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string SystemId { get; set; }

        /// <summary>
        /// リンク表示名
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "SystemLink", "title")]
        [Required(ErrorMessage = "必須項目です。")]
        public string Title { get; set; }

        /// <summary>
        /// リンク詳細
        /// </summary>
        [JpDataHubMaxLength(Domains.DynamicApi, "SystemLink", "detail")]
        [Required(ErrorMessage = "必須項目です。")]
        public string Detail { get; set; }

        /// <summary>
        /// リンクURL
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [Url(ErrorMessage = "正しいURLではありません。")]
        [JpDataHubMaxLength(Domains.DynamicApi, "SystemLink", "url")]
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
