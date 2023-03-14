using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using EasyCaching.Core;
using EasyCaching.InMemory;
using Cosmos = Microsoft.Azure.Cosmos;
using MessagePack;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using Unity.Injection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Configuration;
using JP.DataHub.Com.Misc;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.Api.Core.Repository;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Cryptography;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Models.Database;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_DynamicApiRepository_GetAuthority : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance<ICache>(new NoCache());
            UnityContainer.RegisterInstance<ICache>("DynamicApi", new NoCache());

            UnityContainer.RegisterType<IDynamicApiRepository, DynamicApiRepository>();
            UnityContainer.RegisterInstance<bool>("EnableIpFilter", true);

            UnityContainer.RegisterType<IJPDataHubTransactionManager, JPDataHubTransactionManager>();
        }

        [TestMethod]
        public void GetOpenIdAllowedApplications()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            Guid appId = Guid.NewGuid();
            mock.Setup(x => x.Query<Guid>(It.IsAny<string>(), It.IsAny<object>())).Returns(new[] { appId });
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = new ApiId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.GetOpenIdAllowedApplications(vendorId, controllerId, apiId);

            // 結果をチェック
            result.First().Value.Is(appId.ToString());
        }

        [TestMethod]
        public void GetIpFilter()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            string ip = "123.45.67.89";
            mock.Setup(x => x.Query<string>(It.IsAny<string>(), It.IsAny<object>())).Returns(new[] { ip });
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = new ApiId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.GetIpFilter(vendorId, systemId, controllerId, apiId);

            // 結果をチェック
            result.First().Value.Is(ip);
        }

        [TestMethod]
        public void GetApiAccessVendor()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            var accessKey = new DB_ApiAccessVendorSelect { access_key = Guid.NewGuid(), system_id = Guid.NewGuid(), vendor_id = Guid.NewGuid() };
            mock.Setup(x => x.Query<DB_ApiAccessVendorSelect>(It.IsAny<string>(), It.IsAny<object>())).Returns(new[] { accessKey });
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var apiId = new ApiId(Guid.NewGuid().ToString());
            var targetVendorId = new VendorId(accessKey.vendor_id.ToString());
            var targetSystemId = new SystemId(accessKey.system_id.ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.GetApiAccessVendor(vendorId, systemId, controllerId, apiId, targetVendorId, targetSystemId);

            var expected = new ApiAccessVendor()
            {
                AccessKey = new ApiAccessKey(accessKey.access_key),
                VendorId = new VendorId(accessKey.vendor_id.ToString()),
                SystemId = new SystemId(accessKey.system_id.ToString())
            };

            // 結果をチェック
            result.IsStructuralEqual(expected);
        }

        [TestMethod]
        public void HasMailTemplate()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingle<int>(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.HasMailTemplate(controllerId, vendorId);

            // 結果をチェック
            result.IsTrue();
        }

        [TestMethod]
        public void HasWebhook()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingle<int>(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.HasWebhook(controllerId, vendorId);

            // 結果をチェック
            result.IsTrue();
        }

        [TestMethod]
        public void HasApiAccessOpenid_True()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingle<int>(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(1);
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var apiId = new ApiId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.HasApiAccessOpenid(apiId, openId);

            // 結果をチェック
            result.IsTrue();
        }

        [TestMethod]
        public void HasApiAccessOpenid_False()
        {
            // モックの作成
            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingle<int>(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(0);
            UnityContainer.RegisterInstance("DynamicApi", mock.Object);

            // パラメータ
            var apiId = new ApiId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());

            // テスト対象のインスタンスを作成
            var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

            // テスト対象のメソッド実行
            var result = testClass.HasApiAccessOpenid(apiId, openId);

            // 結果をチェック
            result.IsFalse();
        }

        [TestMethod]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        [TestCase(false, false)]
        public void GetPhysicalRepositoryIdByControllerId()
        {
            TestContext.Run((bool hit, bool exists) =>
            {
                var physicalRepositoryId = exists ? Guid.NewGuid() : (Guid?)null;
                var physicalRepositoryIdObj = physicalRepositoryId.HasValue ? new PhysicalRepositoryId(physicalRepositoryId.Value.ToString()) : null;

                // モックの作成
                var mock = new Mock<IJPDataHubDbConnection>();
                mock.Setup(x => x.QuerySingleOrDefault<Guid?>(It.IsAny<string>(), It.IsAny<object>(), null, null, null)).Returns(physicalRepositoryId);
                UnityContainer.RegisterInstance("DynamicApi", mock.Object);

                var mockCache = new Mock<ICache>();
                if (hit)
                {
                    mockCache.Setup(x => x.Get<PhysicalRepositoryId>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                        .Returns(physicalRepositoryIdObj);
                }
                else
                {
                    mockCache.Setup(x => x.Get<PhysicalRepositoryId>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                        .Returns<string, TimeSpan, ActionObject>((key, timespan, action) => (PhysicalRepositoryId)action.Invoke());
                }
                UnityContainer.RegisterInstance<ICache>(mockCache.Object);

                // パラメータ
                var controllerId = new ControllerId(Guid.NewGuid().ToString());

                // テスト対象のインスタンスを作成
                var testClass = UnityContainer.Resolve<IDynamicApiRepository>();

                // テスト対象のメソッド実行
                var result = testClass.GetPhysicalRepositoryIdByControllerId(controllerId, RepositoryType.SQLServer2);

                // 結果をチェック
                result.IsStructuralEqual(physicalRepositoryIdObj);
            });
        }
    }
}
