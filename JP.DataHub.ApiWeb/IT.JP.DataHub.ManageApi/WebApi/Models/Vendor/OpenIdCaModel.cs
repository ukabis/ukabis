using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.Vendor
{
    public class RegisterOpenIdCaModel
    {
        public string VendorId { get; set; }
        public string VendorOpenidCaId { get; set; }
        public string ApplicationId { get; set; }
        public string AccessControl { get; set; }
    }
    public class OpenIdCaModel
    {
        /// <summary>
        /// ベンダーOpenId認証局ID
        /// </summary>
        public string VendorOpenidCaId { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public string ApplicationId { get; set; }
        public string VendorId { get; set; }

        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string ApplicationName { get; set; }

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
