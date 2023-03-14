using System;
using System.ComponentModel.DataAnnotations;
using JP.DataHub.Api.Core.Validations;
using JP.DataHub.ManageApi.Attributes;

namespace JP.DataHub.ManageApi.Models.System
{
    public class UpdateClientViewModel
    {
        /// <summary>
        /// システムID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string SystemId { get; set; }

        /// <summary>
        /// クライアントID
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [ValidateGuid]
        public string ClientId { get; set; }

        /// <summary>クライアントシークレット</summary>
        [ValidateClientSecret]
        public string ClientSecret { get; set; }

        /// <summary>
        /// 有効期限
        /// </summary>
        [ValidateAccessTokenExpirationTimeSpan]
        [Required(ErrorMessage = "必須項目です。")]
        public string AccessTokenExpirationTimeSpan { get; set; }
    }
}
