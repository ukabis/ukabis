using System;

namespace JP.DataHub.ManageApi.Models.CommonIpFilterGroup
{
    public class GetListCommonIpFilterGroupViewModel
    {
        /// <summary>
        /// IPフィルタグループID
        /// </summary>
        public string CommonIpFilterGroupId { get; set; }

        /// <summary>
        /// IPフィルタグループ名
        /// </summary>
        public string CommonIpFilterGroupName { get; set; }
    }
}
