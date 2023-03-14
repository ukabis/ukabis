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
using IT.JP.DataHub.ODataOverPartition.Config;
using IT.JP.DataHub.ODataOverPartition.WebApi.Models;

namespace IT.JP.DataHub.ODataOverPartition.TestCase
{
    public class ApiWebItTestCase : ItTestCaseBase
    {
        protected const string WILDCARD = "{{*}}";

        protected AppConfig AppConfig { get; set; }

        public override void TestInitialize(bool isUnityInitialize = true, ITypeLifetimeManager typeLifetimeManager = null, IConfiguration configuration = null)
        {
            base.TestInitialize(isUnityInitialize, typeLifetimeManager, configuration);
            AppConfig = UnityCore.Resolve<IConfiguration>().Get<AppConfig>();
        }
    }
}
