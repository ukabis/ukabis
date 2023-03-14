using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.ApiWebhook
{
    public class ApiWebhookViewModel
    {

        /// <summary>ApiWebhookId</summary>
        public string ApiWebhookId { get; set; }

        /// <summary>API ID</summary>
        public string ApiId { get; set; }

        /// <summary>ベンダーID</summary>
        public string VendorId { get; set; }

        /// <summary>Url</summary>
        public string Url { get; set; }

        /// <summary>Httpヘッダー</summary>
        public IEnumerable<HttpHeaderViewModel> Headers { get; set; }

        /// <summary>登録時通知フラグ</summary>
        public bool NotifyRegister { get; set; }

        /// <summary>更新時通知フラグ</summary>
        public bool NotifyUpdate { get; set; }

        /// <summary>削除時通知フラグ</summary>
        public bool NotifyDelete { get; set; }
    }
}
