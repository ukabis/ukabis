using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JP.DataHub.MVC.Session.Models;

namespace JP.DataHub.MVC.Session
{
    public interface IAuthorizationSessionManager
    {
        AuthorizationModel Get();

        void Remove();

        void Save(AuthorizationModel value);
    }
}
