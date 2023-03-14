using JP.DataHub.Api.Core.Database;
using JP.DataHub.ManageApi.Attributes;
using JP.DataHub.ManageApi.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JP.DataHub.ManageApi.Models.CommonIpFilterGroup
{
    public class CommonIpFilterGroupIpListViewModel
    {
        /// <summary>
        /// IPフィルタグループID
        /// </summary>
        public string CommonIpFilterGroupId { get; set; }

        /// <summary>
        /// IPフィルタグループ名
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        [JpDataHubMaxLength(Domains.DynamicApi, DynamicApiDatabase.TABLE_COMMONIPFILTERGROUP, DynamicApiDatabase.COLUMN_COMMONIPFILTERGROUP_COMMON_IP_FILTER_GROUP_NAME)]
        public string CommonIpFilterGroupName { get; set; }

        /// <summary>
        /// IPリスト
        /// </summary>
        [Required(ErrorMessage = "必須項目です。")]
        public List<CommonIpFilterViewModel> IpList { get; set; }
    }
}
