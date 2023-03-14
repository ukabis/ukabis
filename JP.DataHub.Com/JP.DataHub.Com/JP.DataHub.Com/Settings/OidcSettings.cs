using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Settings
{
    public class OidcSettings
    {
        public string Scope { get; set; }

        /// <summary>
        /// .well-known/openid-configuration URL
        /// </summary>
        public string MetadataAddress { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

    }
}
