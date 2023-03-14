using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.System
{
    public class RegisterSystemResultViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// OpenId認証のアプリケーションID
        /// </summary>
        public string OpenIdApplicationId { get; set; }

        /// <summary>
        /// OpenId認証のクライアントシークレット
        /// </summary>
        public string OpenIdClientSecret { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 代表メールアドレス
        /// </summary>
        public string RepresentativeMailAddress { get; set; }

        /// <summary>
        /// 認証情報リスト
        /// </summary>
        public IList<RegisterClientViewModel> ClientList { get; set; }

        /// <summary>
        /// システムリンクリスト
        /// </summary>
        public IList<RegisterSystemLinkViewModel> SystemLinkList { get; set; }

        /// <summary>
        /// 管理者認証
        /// </summary>
        public SystemAdminViewModel SystemAdmin { get; set; }
    }
}
