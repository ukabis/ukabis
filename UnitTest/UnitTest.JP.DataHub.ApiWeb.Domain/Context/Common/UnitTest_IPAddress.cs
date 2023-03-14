using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_IPAddress : UnitTestBase
    {
        [TestMethod]
        public void IPAddress_Serialize()
        {
            var testData = new IpAddress("244.244.244.244/32");
            var target = MessagePackSerializer.Serialize(testData);
            MessagePackSerializer.Deserialize<IpAddress>(target).IsStructuralEqual(testData);
        }

        [TestMethod]
        public void IPAddress_SerializeValueNull()
        {
            var testData = new IpAddress(null);
            var target = MessagePackSerializer.Serialize(testData);
            MessagePackSerializer.Deserialize<IpAddress>(target).IsStructuralEqual(testData);
        }

        [TestMethod]
        public void IPAddress_SerializeNull()
        {
            IpAddress testData = null;
            var target = MessagePackSerializer.Serialize(testData);
            MessagePackSerializer.Deserialize<IpAddress>(target).IsStructuralEqual(testData);
        }
    }
}
