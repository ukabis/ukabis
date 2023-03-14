using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.DataHub.Api.Core.Authentication
{
    public interface IAccessTokenCache
    {
        void Store(AccessToken accessToken);

        AccessToken Get(string accessTokenId);
    }
}
