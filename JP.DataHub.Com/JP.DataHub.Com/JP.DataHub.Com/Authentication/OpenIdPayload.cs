using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Authentication
{
    public class OpenIdPayload
    {
        public string oid { get; set; }
        public long exp { get; set; }
    }
}
