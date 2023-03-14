using JP.DataHub.Api.Core.Database;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.CommonIpFilterGroup
{
    public class RegisterCommonIpFilterViewModel
    {
        /// <summary>
        /// IPアドレス
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubIpAddress]
        public string IpAddress { get; set; }

        /// <summary>
        /// 有効/無効
        /// </summary>
        public bool IsEnable { get; set; } = true;
    }
}
