using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Service.Model
{
    /// <summary>
    /// Webhookのモデル
    /// </summary>
    public class ApiWebhookModel
    {
        /// <summary>ApiWebhookID</summary>
        public string ApiWebhookId { get; set; }

        /// <summary>API ID</summary>
        public string ApiId { get; set; }

        /// <summary>ベンダーID</summary>
        public string VendorId { get; set; }

        /// <summary>Url</summary>
        public string Url { get; set; }

        /// <summary>Httpヘッダー</summary>
        public IList<HttpHeaderModel> Headers { get; set; }

        /// <summary>登録時通知フラグ</summary>
        public bool NotifyRegister { get; set; }

        /// <summary>更新時通知フラグ</summary>
        public bool NotifyUpdate { get; set; }

        /// <summary>削除時通知フラグ</summary>
        public bool NotifyDelete { get; set; }
    }
}