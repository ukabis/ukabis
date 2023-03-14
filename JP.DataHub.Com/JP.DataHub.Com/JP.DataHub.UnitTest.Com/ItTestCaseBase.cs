using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Unity;
using Unity.Lifetime;
using Unity.Interception;
using Microsoft.Extensions.Configuration;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.UnitTest.Com.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JP.DataHub.UnitTest.Com
{
    public class ItTestCaseBase : UnitTestBase
    {
        protected IAuthenticationManager AccountManager { get; set; }
        protected IServerManager ServerManager { get; set; }

        protected HttpStatusCode[] GetExpectStatusCodes = new HttpStatusCode[] { HttpStatusCode.OK, HttpStatusCode.NotFound };
        protected HttpStatusCode[] DeleteExpectStatusCodes = new HttpStatusCode[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound };
        protected HttpStatusCode[] UpdateExpectStatusCodes = new HttpStatusCode[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound };
        protected HttpStatusCode[] RegisterExpectStatusCodes = new HttpStatusCode[] { HttpStatusCode.Created };
        protected HttpStatusCode CountSuccessExpectStatusCode = HttpStatusCode.OK;
        protected HttpStatusCode DeleteSuccessStatusCode = HttpStatusCode.NoContent;
        protected HttpStatusCode DeleteErrorExpectStatusCode = HttpStatusCode.NotFound;
        protected HttpStatusCode RegisterSuccessExpectStatusCode = HttpStatusCode.Created;
        protected HttpStatusCode RegisterErrorExpectStatusCode = HttpStatusCode.BadRequest;
        protected HttpStatusCode GetSuccessExpectStatusCode = HttpStatusCode.OK;
        protected HttpStatusCode GetErrorExpectStatusCode = HttpStatusCode.NotFound;
        protected HttpStatusCode UpdateSuccessExpectStatusCode = HttpStatusCode.NoContent;
        protected HttpStatusCode UpdateErrorExpectStatusCode = HttpStatusCode.NotFound;
        protected HttpStatusCode UpdateValidationErrorExpectStatusCode = HttpStatusCode.BadRequest;
        protected HttpStatusCode ForbiddenExpectStatusCode = HttpStatusCode.Forbidden;
        protected HttpStatusCode NotImplementedExpectStatusCode = HttpStatusCode.NotImplemented;
        protected HttpStatusCode BadRequestStatusCode = HttpStatusCode.BadRequest;
        protected HttpStatusCode NotFoundStatusCode = HttpStatusCode.NotFound;
        protected HttpStatusCode ExistsSuccessExpectStatusCode = HttpStatusCode.OK;
        protected HttpStatusCode ExistsErrorExpectStatusCode = HttpStatusCode.NotFound;
        protected HttpStatusCode ConflictExpectStatusCode = HttpStatusCode.Conflict;
        protected HttpStatusCode InternalServerErrorStatusCode = HttpStatusCode.InternalServerError;

        public TestContext TestContext { get; set; }

        public override void TestInitialize(bool isUnityInitialize = true, ITypeLifetimeManager typeLifetimeManager = null, IConfiguration configuration = null)
        {
            base.TestInitialize(isUnityInitialize, typeLifetimeManager);

            if (configuration == null)
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{TestContext.Properties["TestEnvironment"]}.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();
            }

            UnityCore.RegisterInstance<IConfiguration>(configuration);
            AccountManager = UnityCore.Resolve<IAuthenticationManager>(JP.DataHub.Com.Web.Authentication.AuthenticationManager.AccountJsonFileName.ToCI());
            ServerManager = UnityCore.Resolve<IServerManager>(JP.DataHub.Com.Web.WebRequest.ServerManager.ServerJsonFileName.ToCI());
            var server = ServerManager.Find(configuration.GetValue<string>(ConfigConst.CONFIG_SERVER));
            UnityCore.RegisterInstance<IServer>(server);
            var environment = server.Find(configuration.GetValue<string>(ConfigConst.CONFIG_ENVIRONMENT));
            UnityCore.RegisterInstance<IServerEnvironment>(environment);

            var vendorAuthenticationInfo = environment.GetAuthenticationInfo();
            // デフォルトのものを登録
            var openidAuthenticationInfo = AccountManager.Find(configuration.GetValue<string>(ConfigConst.CONFIG_ACCOUNT));
            if (openidAuthenticationInfo != null)
            {
                var authenticationInfo = vendorAuthenticationInfo.Merge(openidAuthenticationInfo);
                UnityCore.RegisterInstance<IAuthenticationInfo>(authenticationInfo);
            }
            // アカウント名をキーにした、名前付きで登録
            var accounts = AccountManager.AccountList();
            accounts.ToList().ForEach(x =>
            {
                var xAuthInfo = AccountManager.Find(x);
                var xAuthCombime = vendorAuthenticationInfo.Merge(xAuthInfo);
                UnityCore.RegisterInstance<IAuthenticationInfo>(x, xAuthCombime);
            });
        }
    }
}
