using System;
using System.Collections.Generic;

namespace IT.JP.DataHub.ManageApi.WebApi.Models.System
{
    public class SystemModel
    {
        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// ベンダー名
        /// </summary>
        public string VendorName { get; set; }

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
        /// 管理者認証
        /// </summary>
        public string AdminSecret { get; set; }

        /// <summary>
        /// 管理者認証の有効/無効
        /// </summary>
        public bool AdminSecretIsActive { get; set; }

        /// <summary>
        /// 認証情報リスト
        /// </summary>
        public IList<ClientModel> ClientList { get; set; }

        /// <summary>
        /// リステムリンクリスト
        /// </summary>
        public IList<SystemLinkModel> SystemLinkList { get; set; }

        /// <summary>
        /// ファンクション
        /// </summary>
        //public IList<SystemFunctionViewModel> Functions { get; set; }
    }
}
