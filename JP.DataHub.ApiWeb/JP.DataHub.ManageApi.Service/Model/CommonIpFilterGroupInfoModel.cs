using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.ManageApi.Service.Model
{
    public class CommonIpFilterGroupInfoModel
    {
        /// <summary>
        /// CommonIpFilterGroup
        /// </summary>
        public string CommonIpFilterGroupName { get; }
        public List<string> IpAddress { get; }

        /// <summary>
        /// コンストラクター
        /// </summary>
        /// <param name="commonIpFilterGroupName">CommonIpFilterGroupName</param>
        /// <param name="ipAddress">ipAddress</param>

        public CommonIpFilterGroupInfoModel(string commonIpFilterGroupName, List<string> ipAddress)
        {
            CommonIpFilterGroupName = commonIpFilterGroupName;
            IpAddress = ipAddress;
        }
    }
}
