using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Transaction;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;
using Unity.Injection;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_ResourceChangeEventHubStoreRepository : UnitTestBase
    {
        private string vendorId = Guid.NewGuid().ToString();
        private string systemId = Guid.NewGuid().ToString();
        private string openId = Guid.NewGuid().ToString();
        private string controllerId = Guid.NewGuid().ToString();
        private string apiId = Guid.NewGuid().ToString();


#if Oracle
        private Mock<IEventHubStreamingService> eventHubConnect;
#else
        private Mock<IJPDataHubEventHub> eventHubConnect;
#endif

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

#if Oracle
            eventHubConnect = new Mock<IEventHubStreamingService>();
#else
            eventHubConnect = new Mock<IJPDataHubEventHub>();
#endif
            eventHubConnect.Setup(s => s.SendMessageAsync(It.IsAny<JToken>(), It.IsAny<string>())).ReturnsAsync(true);
            UnityContainer.RegisterInstance(eventHubConnect.Object);
        }

        [TestMethod]
        public void Register()
        {
            string farmerId = Guid.NewGuid().ToString();

            var action = new RegistAction
            {
                ActionType = new ActionTypeVO(ActionType.Regist),
                VendorId = new VendorId(vendorId),
                SystemId = new SystemId(systemId),
                OpenId = new OpenId(openId),
                ControllerId = new ControllerId(controllerId),
                ApiId = new ApiId(apiId),
                RepositoryKey = new RepositoryKey("/API/Test/{key}")
            };

            var target = UnityContainer.Resolve<IResourceChangeEventHubStoreRepository>();
            // テストメソッド実行
            target.Register(action, JObject.FromObject(new { FarmerId = farmerId, FarmerName = "aaaa" })).IsTrue();
            // モックの検証
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["RequestInfo"].Value<string>("VendorId") == vendorId
                && match["Data"].Value<string>("FarmerId") == farmerId), It.IsAny<string>()));
        }

        [TestMethod]
        public void Register_PartitionKeyIsNull()
        {
            IUnityContainer container = new UnityContainer();
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
            var configuration = builder.Build();
            container.RegisterType<IEventHubStreamingService, EventHubStreamingService>("LoggingStreamingService", new InjectionConstructor(
                configuration.GetValue<string>("OciCredential:ConfigurationFilePath"),
                configuration.GetValue<string>("OciCredential:Profile"),
                configuration.GetValue<string>("OciCredential:PemFilePath"),
                configuration.GetValue<string>("EventHubStreamingService:Ocid"),
                configuration.GetValue<string>("EventHubStreamingService:EndPoint")
                ));

            var target = UnityContainer.Resolve<IEventHubStreamingService>("LoggingStreamingService");

            Task<bool> result = target.SendMessageAsync($"{this.GetType().Name}-{MethodBase.GetCurrentMethod().Name}-{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}", null);
            Assert.IsTrue(result.Result);
        }

        [TestMethod]
        public void Update()
        {
            string farmerId = Guid.NewGuid().ToString();

            var action = new UpdateAction
            {
                ActionType = new ActionTypeVO(ActionType.Update),
                VendorId = new VendorId(vendorId),
                SystemId = new SystemId(systemId),
                OpenId = new OpenId(openId),
                ControllerId = new ControllerId(controllerId),
                ApiId = new ApiId(apiId),
                RepositoryKey = new RepositoryKey("/API/Test/{FarmerId}"),
                Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>
                {
                    { new QueryStringKey("FarmerId"), new QueryStringValue(farmerId) }
                })
            };

            var target = UnityContainer.Resolve<IResourceChangeEventHubStoreRepository>();
            // テストメソッド実行
            target.Update(action, JObject.FromObject(new { FarmerId = farmerId, FarmerName = "aaaa" }),
                JObject.FromObject(new { FarmerName = "bbbb" })).IsTrue();
            // モックの検証
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["RequestInfo"].Value<string>("VendorId") == vendorId
                && match["Data"].Value<string>("FarmerId") == farmerId
                && match["Query"].Value<string>("FarmerId") == farmerId
                && match["Input"].Value<string>("FarmerName") == "bbbb"), It.IsAny<string>()));
        }

        [TestMethod]
        public void Delete()
        {
            string queryKey = Guid.NewGuid().ToString();

            var action = new DeleteDataAction
            {
                ActionType = new ActionTypeVO(ActionType.DeleteData),
                VendorId = new VendorId(vendorId),
                SystemId = new SystemId(systemId),
                OpenId = new OpenId(openId),
                ControllerId = new ControllerId(controllerId),
                ApiId = new ApiId(apiId),
                RepositoryKey = new RepositoryKey("/API/Test/{key}"),
                Query = new QueryStringVO(new Dictionary<QueryStringKey, QueryStringValue>
                {
                    { new QueryStringKey("key"), new QueryStringValue(queryKey) }
                })
            };

            var target = UnityContainer.Resolve<IResourceChangeEventHubStoreRepository>();
            // テストメソッド実行
            target.Delete(action).IsTrue();
            // モックの検証
            eventHubConnect.Verify(x => x.SendMessageAsync(It.Is<JToken>(match => match["RequestInfo"].Value<string>("VendorId") == vendorId
                && match["Query"].Value<string>("key") == queryKey), It.IsAny<string>()));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Delete_Error()
        {
            var action = new DeleteDataAction
            {
                ApiQuery = new ApiQuery("select c.key from c")
            };

            var target = UnityContainer.Resolve<IResourceChangeEventHubStoreRepository>();
            // テストメソッド実行
            target.Delete(action);
        }
    }
}
