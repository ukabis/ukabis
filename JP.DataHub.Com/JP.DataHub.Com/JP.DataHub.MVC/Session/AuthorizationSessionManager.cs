using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.MVC.Session;
using JP.DataHub.MVC.Session.Models;

namespace JP.DataHub.MVC.Session
{
    public class AuthorizationSessionManager : UnityAutoInjection, IAuthorizationSessionManager
    {
        private const string SESSION_AUTHORIZATION_KEY = "auth";

        [Dependency]
        public IHttpContextAccessor HttpContextAccessor;


        public AuthorizationModel Get() => HttpContextAccessor.HttpContext.Session.GetObject<AuthorizationModel>(SESSION_AUTHORIZATION_KEY);

        public void Remove() => HttpContextAccessor.HttpContext.Session.Remove(SESSION_AUTHORIZATION_KEY);

        public void Save(AuthorizationModel value) => HttpContextAccessor.HttpContext.Session.SetObject<AuthorizationModel>(SESSION_AUTHORIZATION_KEY, value);
    }
}
