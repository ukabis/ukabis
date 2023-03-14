using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using JP.DataHub.Com.Validations.Attributes;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using JP.DataHub.Api.Core.Database;

namespace JP.DataHub.ManageApi.Models.System
{
    public class UpdateSystemViewModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string SystemId { get; set; }

        /// <summary>
        /// システム名
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_SYSTEM, AuthorityDatabase.COLUMN_SYSTEM_SYSTEM_NAME)]
        public string SystemName { get; set; }

        /// <summary>
        /// OpenId認証のアプリケーションID
        /// </summary>
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_SYSTEM, AuthorityDatabase.COLUMN_SYSTEM_OPENID_APPLICATIONID)]
        public string OpenIdApplicationId { get; set; }

        /// <summary>
        /// OpenId認証のクライアントシークレット
        /// </summary>
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_SYSTEM, AuthorityDatabase.COLUMN_SYSTEM_OPENID_CLIENT_SECRET)]
        public string OpenIdClientSecret { get; set; }

        /// <summary>
        /// 代表メールアドレス
        /// </summary>
        [JpDataHubMaxLength(Domains.Authority, AuthorityDatabase.TABLE_SYSTEM, AuthorityDatabase.COLUMN_SYSTEM_REPRESENTATIVE_MAIL_ADDRESS)]
        public string RepresentativeMailAddress { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
