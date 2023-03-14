using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.Authentication
{
    public class OidcAccessTokenValidatorSetting
    {
        /// <summary>テナントUrl</summary>
        public string TenantUrl { get; set; }

        /// <summary>テナントUrlR1</summary>
        public string TenantUrlForSwitching { get; set; }

        /// <summary>ポリシー</summary>
        public string Policy { get; set; }

        /// <summary>ApiWebのアプリケーションId</summary>
        public string Audience { get; set; }
    }
}
