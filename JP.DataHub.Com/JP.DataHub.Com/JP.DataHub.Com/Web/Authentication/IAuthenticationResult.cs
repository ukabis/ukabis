using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace JP.DataHub.Com.Web.Authentication
{
    public interface IAuthenticationResult
    {
        IAuthenticationInfo Info { get; set; }
        bool IsSuccessfull { get; }
        string Error { get; }

        //void Refresh(IAuthenticationResult newResult);

        void AddHeader(HttpRequestHeaders headers);
    }
}
