using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Unity;
using Unity.Lifetime;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.ApiWeb.Config;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    public class ApiWebItTestCase : ItTestCaseBase
    {
        protected const string WILDCARD = "{{*}}";

        public static bool IsUnityBuilt = false;

        protected AppConfig AppConfig { get; set; }
        protected bool IsIgnoreGetInternalAllField { get; set; } = false;


        public override void TestInitialize(bool isUnityInitialize = true, ITypeLifetimeManager typeLifetimeManager = null, IConfiguration configuration = null)
        {
            // スキップ
            if (configuration == null)
            {
                var settingFileName = TestContext.Properties.Contains("TestEnvironment") 
                    ? $"appsettings.{TestContext.Properties["TestEnvironment"]}.json"
                    : "appsettings.json";

                configuration = new ConfigurationBuilder()
                    .AddJsonFile(settingFileName, true, true)
                    .AddEnvironmentVariables()
                    .Build();
            }
            AppConfig = configuration.Get<AppConfig>();

            if (AppConfig.SkipList != null && AppConfig.SkipList.Count > 0)
            {
                var testClassName = TestContext.FullyQualifiedTestClassName.Split('.').Last();
                if (AppConfig.SkipList.Any(x => x == testClassName))
                {
                    Assert.Inconclusive("スキップリストに登録されています。");
                }
                if (AppConfig.SkipList.Any(x => x == $"{testClassName}.{TestContext.TestName}"))
                {
                    Assert.Inconclusive("スキップリストに登録されています。");
                }
            }

            // 全体で一度だけビルドアップする
            if (isUnityInitialize && !IsUnityBuilt)
            {
                UnityCore.UnityContainer = null;
                if (UnityCore.UnityContainer == null)
                {
                    UnityCore.DataContainerLifetimeManager = typeLifetimeManager ?? new PerThreadLifetimeManager/*PerResolveLifetimeManager*/();
                    UnityCore.IsEnableDiagnostic = true;
                    UnityCore.Buildup(new UnityContainer(), "UnityBuildup.json", configuration);
                    UnityContainer = UnityCore.UnityContainer;
                }
                IsUnityBuilt = true;
                isUnityInitialize = false;
            }
            else if (IsUnityBuilt)
            {
                isUnityInitialize = false;
            }

            base.TestInitialize(isUnityInitialize, typeLifetimeManager, configuration);
        }


        public byte[] GetContentsByte(string filePath) => File.ReadAllBytes(filePath);


        protected Dictionary<string, string> GetLoggingIdDictionary(IEnumerable<string> headerValue)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (var value in headerValue)
            {
                var dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(value);
                result = dic;
            }

            return result;
        }
    }
}
