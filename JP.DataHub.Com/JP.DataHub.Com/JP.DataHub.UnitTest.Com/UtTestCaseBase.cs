using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Lifetime;
using Unity.Interception;

namespace JP.DataHub.UnitTest.Com
{
    public class UnitTestCaseBase : UnitTestBase
    {
        public override void TestInitialize(bool isUnityInitialize = false, ITypeLifetimeManager typeLifetimeManager = null, IConfiguration configuration = null)
        {
            base.TestInitialize(isUnityInitialize, typeLifetimeManager, configuration);
        }
    }
}
