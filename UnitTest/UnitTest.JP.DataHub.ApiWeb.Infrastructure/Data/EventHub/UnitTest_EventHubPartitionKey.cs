using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Infrastructure.Data.EventHub;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Data.EventHub
{
    [TestClass]
    public class UnitTest_EventHubPartitionKey : UnitTestBase
    {
        [TestMethod]
        public void EventHubPartitionKey_CreateNonVendor()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var isVendor = new IsVendor(false);
            var partitionKey = new PartitionKey("/API/Public/Farmer");
            var body = JToken.FromObject(new { hoge = "aaa", fooo = "aaa", pro = "aaa" });
            var target = EventHubPartitionKey.CreateRegisterPartition(partitionKey, isVendor, vendorId, systemId, body);
            target.Value.Is("API~Public~Farmer");
        }

        [TestMethod]
        public void EventHubPartitionKey_CreateVendor()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var isVendor = new IsVendor(true);
            var partitionKey = new PartitionKey("/API/Public/Farmer");
            var body = JToken.FromObject(new { hoge = "aaa", fooo = "aaa", pro = "aaa" });
            var target = EventHubPartitionKey.CreateRegisterPartition(partitionKey, isVendor, vendorId, systemId, body);
            target.Value.Is($"API~Public~Farmer~{vendorId.Value}~{systemId.Value}");
        }

        [TestMethod]
        public void EventHubPartitionKey_CreateBody()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var isVendor = new IsVendor(true);
            var partitionKey = new PartitionKey("/API/Public/Farmer/{hoge}");
            var body = JToken.FromObject(new { hoge = "aaa", fooo = "aaa", pro = "aaa" });
            var target = EventHubPartitionKey.CreateRegisterPartition(partitionKey, isVendor, vendorId, systemId, body);
            target.Value.Is($"API~Public~Farmer~{vendorId.Value}~{systemId.Value}~{body["hoge"]}");
        }

        [TestMethod]
        public void EventHubPartitionKey_CreateBodyDouble()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var isVendor = new IsVendor(true);
            var partitionKey = new PartitionKey("/API/Public/Farmer/{hoge}/{pro}");
            var body = JToken.FromObject(new { hoge = "aaa", fooo = "aaa", pro = "aaa" });
            var target = EventHubPartitionKey.CreateRegisterPartition(partitionKey, isVendor, vendorId, systemId, body);
            target.Value.Is($"API~Public~Farmer~{vendorId.Value}~{systemId.Value}~{body["hoge"]}~{body["pro"]}");
        }

        [TestMethod]
        public void EventHubPartitionKey_CreateNonPartitionKey()
        {
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var isVendor = new IsVendor(true);
            var partitionKey = new PartitionKey("");
            var body = JToken.FromObject(new { hoge = "aaa", fooo = "aaa", pro = "aaa" });
            var target = EventHubPartitionKey.CreateRegisterPartition(partitionKey, isVendor, vendorId, systemId, body);
            target.IsNull();
        }
    }
}
