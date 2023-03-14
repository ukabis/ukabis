using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_RepositoryKey : UnitTestBase
    {
        [TestMethod]
        public void RepositoryKey_LogicalKey_Single()
        {
            RepositoryKey rep = new RepositoryKey("/Api/Public/Hoge/{Id1}");
            rep.Value.Is("/Api/Public/Hoge/{Id1}");
            rep.IsExsitsLogicalKey.IsTrue();
            rep.LogicalKeys.Count().Is(1);
            rep.LogicalKeys.ToList()[0].Is("Id1");
            rep.Type.Is("Api~Public~Hoge");
        }

        [TestMethod]
        public void RepositoryKey_LogicalKey_Double()
        {
            RepositoryKey rep = new RepositoryKey("/Api/Public/Hoge/{Id1}/{Id2}");
            rep.Value.Is("/Api/Public/Hoge/{Id1}/{Id2}");
            rep.IsExsitsLogicalKey.IsTrue();
            rep.LogicalKeys.Count().Is(2);
            rep.LogicalKeys.ToList()[0].Is("Id1");
            rep.LogicalKeys.ToList()[1].Is("Id2");
            rep.Type.Is("Api~Public~Hoge");
        }

        [TestMethod]
        public void RepositoryKey_NoLogicalKey()
        {
            RepositoryKey rep = new RepositoryKey("/Api/Public/Hoge");
            rep.Value.Is("/Api/Public/Hoge");
            rep.IsExsitsLogicalKey.IsFalse();
            rep.Type.Is("Api~Public~Hoge");
        }
    }
}
