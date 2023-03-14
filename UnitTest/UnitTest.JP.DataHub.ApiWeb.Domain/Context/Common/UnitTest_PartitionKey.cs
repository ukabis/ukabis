using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_PartitionKey : UnitTestBase
    {
        [TestMethod]
        public void PartitionKey_LogicalKey_Single()
        {
            var part = new PartitionKey("/Api/Public/Hoge/{Id1}");
            part.Value.Is("/Api/Public/Hoge/{Id1}");
            part.IsExsitsLogicalKey.IsTrue();
            part.LogicalKeys.Count().Is(1);
            part.LogicalKeys.ToList()[0].Is("Id1");
            part.BaseString.Is("Api~Public~Hoge");
        }

        [TestMethod]
        public void PartitionKey_LogicalKey_Double()
        {
            var part = new PartitionKey("/Api/Public/Hoge/{Id1}/{Id2}");
            part.Value.Is("/Api/Public/Hoge/{Id1}/{Id2}");
            part.IsExsitsLogicalKey.IsTrue();
            part.LogicalKeys.Count().Is(2);
            part.LogicalKeys.ToList()[0].Is("Id1");
            part.LogicalKeys.ToList()[1].Is("Id2");
            part.BaseString.Is("Api~Public~Hoge");
        }

        [TestMethod]
        public void PartitionKey_NoLogicalKey()
        {
            var part = new PartitionKey("/Api/Public/Hoge");
            part.Value.Is("/Api/Public/Hoge");
            part.IsExsitsLogicalKey.IsFalse();
            part.BaseString.Is("Api~Public~Hoge");
        }
    }
}
