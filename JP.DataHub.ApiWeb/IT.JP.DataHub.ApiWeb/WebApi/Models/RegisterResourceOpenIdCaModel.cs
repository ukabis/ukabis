using System.Collections.Generic;
using Newtonsoft.Json;

namespace IT.JP.DataHub.ApiWeb.WebApi.Models
{
    public class RegisterResourceOpenIdCaModel
    {
        /// <summary>
        /// 認証局ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// アクセス制御（alw:許可 / dny:拒否 / inh:継承）
        /// </summary>
        public string AccessControl { get; set; }
    }
}
