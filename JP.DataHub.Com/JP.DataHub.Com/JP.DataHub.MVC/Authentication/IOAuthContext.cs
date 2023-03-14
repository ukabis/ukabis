using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace JP.DataHub.MVC.Authentication
{
    public interface IOAuthContext
    {
        public ITokenAcquisition TokenAcquisition { get; set; }
        public string OpenIdAccessToken { get; }
        public string GetOpenIdToken(string scopes = null);
    }
}
