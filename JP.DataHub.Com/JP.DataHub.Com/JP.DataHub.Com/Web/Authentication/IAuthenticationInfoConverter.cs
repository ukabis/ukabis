using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public class IAuthenticationInfoConverter : InterfaceJsonConverter<IAuthenticationInfo>
    {
        public IAuthenticationInfoConverter()
        {
            TypePropertyName = "Type";
            Mapping.Add(nameof(AuthenticationInfo), typeof(AuthenticationInfo));
        }
    }
}
