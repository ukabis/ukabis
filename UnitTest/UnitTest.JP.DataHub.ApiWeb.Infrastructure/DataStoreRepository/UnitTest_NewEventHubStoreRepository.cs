using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.DataStoreRepository
{
    [TestClass]
    public class UnitTest_NewEventHubStoreRepository : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>();
        }

        [TestMethod]
        public void EventHubStoreRepository_RegistDataNonPartition()
        {
#if Oracle
            var eventHubConnect = new Mock<IEventHubStreamingService>();
            eventHubConnect.SetupProperty(x => x.ConnectionString, "hoge");
            var dummyAction = new Mock<IDynamicApiAction>();
            var datas = JToken.FromObject(new { FarmerId = "aaaaa", FarmerName = "aaaa" });
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());
            dummyAction.SetupAllProperties();
            dummyAction.SetupProperty(x => x.PartitionKey, new PartitionKey(""));
            dummyAction.SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/Public/Farmer/{FarmerId}"));
            dummyAction.SetupProperty(x => x.IsVendor, new IsVendor(false));
            dummyAction.SetupProperty(x => x.VendorId, vendorId);
            dummyAction.SetupProperty(x => x.SystemId, systemId);
            dummyAction.SetupProperty(x => x.OpenId, openId);
            dummyAction.SetupProperty(x => x.RepositoryInfo, new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo> { }));
            dummyAction.SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false));
            eventHubConnect.Setup(x => x.SendMessageAsync(It.IsAny<JToken>(), It.IsAny<string>())).ReturnsAsync(true);
            UnityContainer.RegisterInstance<IEventHubStreamingService>(eventHubConnect.Object);
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>(RepositoryType.EventHub.ToCode());

            target.RepositoryInfo = new RepositoryInfo("ehb", new Dictionary<string, bool>() { { "hoge", false } });
            target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(datas, dummyAction.Object)).IsNull();
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["FarmerId"].Value<string>() == "aaaaa"), It.IsAny<string>()));
#else
            var eventHubConnect = new Mock<IJPDataHubEventHub>();
            eventHubConnect.SetupProperty(x => x.ConnectionString, "hoge");
            var dummyAction = new Mock<IDynamicApiAction>();
            var datas = JToken.FromObject(new { FarmerId = "aaaaa", FarmerName = "aaaa" });
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());
            dummyAction.SetupAllProperties();
            dummyAction.SetupProperty(x => x.PartitionKey, new PartitionKey(""));
            dummyAction.SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/Public/Farmer/{FarmerId}"));
            dummyAction.SetupProperty(x => x.IsVendor, new IsVendor(false));
            dummyAction.SetupProperty(x => x.VendorId, vendorId);
            dummyAction.SetupProperty(x => x.SystemId, systemId);
            dummyAction.SetupProperty(x => x.OpenId, openId);
            dummyAction.SetupProperty(x => x.RepositoryInfo, new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo> { }));
            dummyAction.SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false));
            eventHubConnect.Setup(x => x.SendMessageAsync(It.IsAny<JToken>(), It.IsAny<string>())).ReturnsAsync(true);
            UnityContainer.RegisterInstance<IJPDataHubEventHub>(eventHubConnect.Object);
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>(RepositoryType.EventHub.ToCode());

            target.RepositoryInfo = new RepositoryInfo("ehb", new Dictionary<string, bool>() { { "hoge", false } });
            target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(datas, dummyAction.Object)).IsNull();
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["FarmerId"].Value<string>() == "aaaaa"), It.IsAny<string>()));
#endif
        }

        [TestMethod]
        public void EventHubStoreRepository_RegistDataPartition()
        {
#if Oracle
            var eventHubConnect = new Mock<IEventHubStreamingService>();
            eventHubConnect.SetupProperty(x => x.ConnectionString, "hoge");
            var dummyAction = new Mock<IDynamicApiAction>();
            var datas = JToken.FromObject(new { FarmerId = "aaaaa", FarmerName = "aaaa" });
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());
            dummyAction.SetupAllProperties();
            dummyAction.SetupProperty(x => x.PartitionKey, new PartitionKey("/API/Public/Farmer/{FarmerId}"));
            dummyAction.SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/Public/Farmer/{FarmerId}"));
            dummyAction.SetupProperty(x => x.IsVendor, new IsVendor(false));
            dummyAction.SetupProperty(x => x.VendorId, vendorId);
            dummyAction.SetupProperty(x => x.SystemId, systemId);
            dummyAction.SetupProperty(x => x.OpenId, openId);
            dummyAction.SetupProperty(x => x.RepositoryInfo, new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo> { }));
            dummyAction.SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false));
            eventHubConnect.Setup(x => x.SendMessageAsync(It.IsAny<JToken>(), It.IsAny<string>())).ReturnsAsync(true);
            UnityContainer.RegisterInstance<IEventHubStreamingService>(eventHubConnect.Object);
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>(RepositoryType.EventHub.ToCode());

            target.RepositoryInfo = new RepositoryInfo("ehb", new Dictionary<string, bool>() { { "hoge", false } });
            target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(datas, dummyAction.Object)).IsNull();
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["FarmerId"].Value<string>() == "aaaaa"), It.Is<string>(match => match == "API~Public~Farmer~aaaaa")));
#else
            var eventHubConnect = new Mock<IJPDataHubEventHub>();
            eventHubConnect.SetupProperty(x => x.ConnectionString, "hoge");
            var dummyAction = new Mock<IDynamicApiAction>();
            var datas = JToken.FromObject(new { FarmerId = "aaaaa", FarmerName = "aaaa" });
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());
            dummyAction.SetupAllProperties();
            dummyAction.SetupProperty(x => x.PartitionKey, new PartitionKey("/API/Public/Farmer/{FarmerId}"));
            dummyAction.SetupProperty(x => x.RepositoryKey, new RepositoryKey("/API/Public/Farmer/{FarmerId}"));
            dummyAction.SetupProperty(x => x.IsVendor, new IsVendor(false));
            dummyAction.SetupProperty(x => x.VendorId, vendorId);
            dummyAction.SetupProperty(x => x.SystemId, systemId);
            dummyAction.SetupProperty(x => x.OpenId, openId);
            dummyAction.SetupProperty(x => x.RepositoryInfo, new ReadOnlyCollection<RepositoryInfo>(new List<RepositoryInfo> { }));
            dummyAction.SetupProperty(x => x.IsAutomaticId, new IsAutomaticId(false));
            eventHubConnect.Setup(x => x.SendMessageAsync(It.IsAny<JToken>(), It.IsAny<string>())).ReturnsAsync(true);
            UnityContainer.RegisterInstance<IJPDataHubEventHub>(eventHubConnect.Object);
            var target = UnityContainer.Resolve<INewDynamicApiDataStoreRepository>(RepositoryType.EventHub.ToCode());

            target.RepositoryInfo = new RepositoryInfo("ehb", new Dictionary<string, bool>() { { "hoge", false } });
            target.RegisterOnce(ValueObjectUtil.Create<RegisterParam>(datas, dummyAction.Object)).IsNull();
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["FarmerId"].Value<string>() == "aaaaa"), It.Is<string>(match => match == "API~Public~Farmer~aaaaa")));
#endif
        }
    }
}
