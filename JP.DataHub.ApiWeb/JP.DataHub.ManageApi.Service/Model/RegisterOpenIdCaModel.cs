using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class RegisterOpenIdCaModel
    {
        public Guid VendorId { get; set; }

        /// <summary>
        /// ベンダーOpenId認証局ID
        /// </summary>
        public Guid? VendorOpenidCaId { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// アクセス制御（alw:許可 / dny:拒否 / inh:継承）
        /// </summary>
        public string AccessControl { get; set; }
    }
}
