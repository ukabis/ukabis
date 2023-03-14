using System;
using System.Collections.Generic;

namespace JP.DataHub.ManageApi.Models.System
{
    public class SystemViewModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public Guid VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// システムID
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        public string SystemName { get; set; }

        /// <summary>
        /// 代表メールアドレス
        /// </summary>
        public string RepresentativeMailAddress { get; set; }

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
        /// 管理者認証の有効/無効
        /// </summary>
        public bool AdminSecretIsActive { get; set; }

        /// <summary>
        /// 認証情報リスト
        /// </summary>
        public IList<ClientViewModel> ClientList { get; set; }

        /// <summary>
        /// システムリンクリスト
        /// </summary>
        public IList<SystemLinkViewModel> SystemLinkList { get; set; }

        /// <summary>
        /// 管理者認証
        /// </summary>
        public SystemAdminViewModel SystemAdmin { get; set; }

        /// <summary>
        /// ファンクション
        /// </summary>
        //public IList<SystemFunctionViewModel> Functions { get; set; }
    }
}