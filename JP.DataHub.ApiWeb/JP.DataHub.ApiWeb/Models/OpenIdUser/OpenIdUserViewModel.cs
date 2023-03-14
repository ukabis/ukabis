using System;
using System.ComponentModel;
using Newtonsoft.Json;

namespace JP.DataHub.ApiWeb.Models.OpenIdUser
{
    /// <summary>
    /// OpenIDユーザー
    /// </summary>
    public class OpenIdUserViewModel
    {
        /// <summary>
        /// ユーザーID(電子メールアドレス)
        /// </summary>
        [DisplayName("ユーザーID(電子メールアドレス)")]
        public string UserId { get; set; }

        /// <summary>
        /// 表示名
        /// </summary>
        [DisplayName("表示名")]
        public string UserName { get; set; }

        /// <summary>
        /// 作成日時
        /// </summary>
        [DisplayName("作成日時")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? CreatedDateTime { get; set; }

        /// <summary>
        /// OpenID
        /// </summary>
        [DisplayName("OpenID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string OpenId { get; set; }
    }
}