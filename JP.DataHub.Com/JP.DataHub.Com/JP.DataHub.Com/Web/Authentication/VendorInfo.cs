using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public class VendorInfo
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        /// <summary>
        /// 設定ありの場合はVendorAuthenticationServerInfo.Urlより優先
        /// </summary>
        public string Url { get; set; }
    }
}
