using JP.DataHub.ManageApi.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.CommonIpFilterGroup
{
    public class CommonIpFilterViewModel
    {
        /// <summary>
        /// CommonIpFilterId
        /// </summary>
        public string CommonIpFilterId { get; set; }

        /// <summary>
        /// IPアドレス
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubIpAddress]
        public string IpAddress { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool IsActive { get; set; }
    }
}
