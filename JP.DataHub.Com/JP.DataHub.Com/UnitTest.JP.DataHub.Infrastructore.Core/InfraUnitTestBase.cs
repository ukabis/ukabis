using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.Interception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Unity.Microsoft.DependencyInjection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Serializer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.UnitTest.Com.Config;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Infrastructore.Core
{
    public class InfraUnitTestBase : UnitTestBase
    {
        protected IAuthenticationManager AccountManager { get; set; }
        protected IServerManager ServerMan { get; set; }

        public override void TestInitialize()
        {
            TestInitialize(false, null);
        }

        public override void TestInitialize(bool isUnityInitialize = true, ITypeLifetimeManager typeLifetimeManager = null, IConfiguration configuration = null)
        {
            if (configuration == null)
            {
                configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
            }

            base.TestInitialize(isUnityInitialize, typeLifetimeManager, configuration);

            var hostBuilder = new HostBuilder()
                .UseUnityServiceProvider(UnityCore.UnityContainer)
                .ConfigureServices((context, services) =>
                {
                    services.AddCaching(configuration);
                });

            if (isUnityInitialize == true)
            {
                UnityCore.UnityContainer.RegisterType<ISerializer, ComMessagePackSerializer>(new PerResolveLifetimeManager());
                UnityCore.UnityContainer.RegisterType<ISerializer, ComMessagePackSerializer>("InMemoryCache", new PerResolveLifetimeManager());
                //UnityCore.RegisterInstance<IConfiguration>(configuration);
                //AccountManager = UnityCore.Resolve<IAuthenticationManager>(AuthenticationManager.AccountJsonFileName.ToCI());
                //ServerMan = UnityCore.Resolve<IServerManager>(ServerManager.ServerJsonFileName.ToCI());
                //UnityCore.RegisterInstance<IServer>(ServerMan.Find(configuration.GetValue<string>(ConfigConst.CONFIG_SERVER)));
                //Environment = UnityCore.Resolve<IServer>().Find(configuration.GetValue<string>(ConfigConst.CONFIG_ENVIRONMENT));
                //UnityCore.RegisterInstance<IServerEnvironment>(Environment);
                //var accountAuthenticationInfo = AccountManager.Find(configuration.GetValue<string>(ConfigConst.CONFIG_ACCOUNT));
                //AuthenticationInfo = accountAuthenticationInfo.Merge(Environment.GetAuthenticationInfo());
                //UnityCore.RegisterInstance<IAuthenticationInfo>(AuthenticationInfo);
            }
        }
    }
}
