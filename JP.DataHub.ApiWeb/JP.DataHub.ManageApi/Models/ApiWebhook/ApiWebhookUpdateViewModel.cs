using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.ApiWebhook
{
    /// <summary>
    /// WebhookのViewModel
    /// </summary>
    public class ApiWebhookUpdateViewModel
    {
        /// <summary>
        /// APIWebhookID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid ApiWebhookId { get; set; }

        /// <summary>
        /// API ID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid ApiId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public Guid VendorId { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [Url]
        public string Url { get; set; }

        /// <summary>
        /// Httpヘッダー
        /// </summary>
        public List<HttpHeaderViewModel> Headers { get; set; }

        /// <summary>
        /// 登録時通知フラグ
        /// </summary>
        public bool NotifyRegister { get; set; }

        /// <summary>
        /// 更新時通知フラグ
        /// </summary>
        public bool NotifyUpdate { get; set; }

        /// <summary>
        /// 削除時通知フラグ
        /// </summary>
        public bool NotifyDelete { get; set; }
    }
}