using System;

namespace JP.DataHub.ManageApi.Models.Vendor
{
    public class OpenIdCaViewModel
    {
        /// <summary>
        /// ベンダーOpenId認証局ID
        /// </summary>
        public Guid? VendorOpenidCaId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public Guid? VendorId { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public Guid ApplicationId { get; set; }

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