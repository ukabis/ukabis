using System.Collections.Generic;
using System.Net.Http.Headers;
using JP.DataHub.Com.Consts;

namespace JP.DataHub.Com.Web.Authentication
{
    public class VendorAuthenticationServerInfo : AbstractAuthenticationServerInfo
    {
        private const string DefaultAdminKey = "DefaultAdminKey";


        public string Url { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string AdminKey { get; set; }

        public IEnumerable<VendorSystemInfo> VendorSystemList { get; set; }


        public VendorAuthenticationServerInfo()
        {
            Type = AuthenticationServerType.Vendor;
            AdminKey ??= DefaultAdminKey;
        }

        public override void AddHeader(HttpRequestHeaders headers)
        {
            if (!string.IsNullOrEmpty(AdminKey))
            {
                headers.Add(HeaderConst.XAdmin, AdminKey);
            }
        }


        public class VendorSystemInfo
        {
            public string Name { get; set; }

            public string ClientId { get; set; }

            public string ClientSecret { get; set; }

            public string AdminKey { get; set; }

            public string VendorId { get; set; }

            public string SystemId { get; set; }

            /// <summary>
            /// 設定ありの場合はVendorAuthenticationServerInfo.Urlより優先
            /// </summary>
            public string Url { get; set; }
        }
    }
}
