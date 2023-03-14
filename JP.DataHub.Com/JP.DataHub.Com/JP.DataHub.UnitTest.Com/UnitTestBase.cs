using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Lifetime;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;

namespace JP.DataHub.UnitTest.Com
{
    public class UnitTestBase
    {
        protected IServerEnvironment Environment { get; set; }
        protected IAuthenticationInfo AuthenticationInfo { get; set; }
        protected IAuthenticationService AuthenticationService { get; set; }
        protected IAuthenticationResult AuthenticationResult { get; set; }
        protected IUnityContainer UnityContainer { get; set; }
        protected IConfiguration Configuration => _lazyConfiguration.Value;
        private Lazy<IConfiguration> _lazyConfiguration = new Lazy<IConfiguration>(() =>
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        });

        public virtual void TestInitialize()
        {
            TestInitialize(false, null);
        }

        public virtual void TestInitialize(bool isUnityInitialize = false, ITypeLifetimeManager typeLifetimeManager= null, IConfiguration configuration = null)
        {
            if (configuration != null)
            {
                _lazyConfiguration = new Lazy<IConfiguration>(() => configuration);
            }

            if (isUnityInitialize == true)
            {
                UnityCore.UnityContainer = null;
                if (UnityCore.UnityContainer == null)
                {
                    UnityCore.DataContainerLifetimeManager = typeLifetimeManager ?? new PerThreadLifetimeManager/*PerResolveLifetimeManager*/();
                    UnityCore.IsEnableDiagnostic = true;
                    UnityCore.Buildup(new UnityContainer(), "UnityBuildup.json", Configuration);
                    UnityContainer = UnityCore.UnityContainer;
                }
            }
        }

        public virtual void TestCleanup()
        {
            UnityCore.UnityContainer = null;
        }
    }
}
