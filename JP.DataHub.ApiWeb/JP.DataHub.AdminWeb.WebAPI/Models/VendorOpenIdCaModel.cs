﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.AdminWeb.WebAPI.Models
{
    public class VendorOpenIdCaModel
    {
        /// <summary>
        /// ベンダーOpenId認証局ID
        /// </summary>
        public string VendorOpenidCaId { get; set; }

        /// <summary>
        /// ベンダーID
        /// </summary>
        public string VendorId { get; set; }

        /// <summary>
        /// アプリケーションID
        /// </summary>
        public string ApplicationId { get; set; }

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

        /// <summary>
        /// このインスタンスの簡易コピーを取得する。
        /// </summary>
        /// <returns>このインスタンスの簡易コピー。</returns>
        public VendorOpenIdCaModel Clone()
        {
            return (VendorOpenIdCaModel)MemberwiseClone();
        }
    }
}
