using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.ApiWebhook
{
    public class ApiWebhookModel
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
        public List<HttpHeaderModel> Headers { get; set; }

        /// <summary>登録時通知フラグ</summary>
        public bool NotifyRegister { get; set; }

        /// <summary>更新時通知フラグ</summary>
        public bool NotifyUpdate { get; set; }

        /// <summary>削除時通知フラグ</summary>
        public bool NotifyDelete { get; set; }
    }
    public class HttpHeaderModel
    {
        /// <summary>
        /// フィールド名
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 値
        /// </summary>
        public string Value { get; set; }
    }
    public class ApiWebhookRegisterResponseModel
    {
        public string ApiWebhookId { get; set; }
    }
    public class ApiWebhookRegisterModel
    {
        /// <summary>
        /// API ID
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Httpヘッダー
        /// </summary>
        public List<HttpHeaderModel> Headers { get; set; }

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
    public class ApiWebhookUpdateModel
    {
        /// <summary>
        /// APIWebhookID
        /// </summary>
        public string ApiWebhookId { get; set; }

        /// <summary>
        /// API ID
        /// </summary>
        public string ApiId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Httpヘッダー
        /// </summary>
        public List<HttpHeaderModel> Headers { get; set; }

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
