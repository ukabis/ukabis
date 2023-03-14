using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using EasyCaching.Core;
using Cosmos = Microsoft.Azure.Cosmos;
using MessagePack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using Unity.Injection;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Exceptions;
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
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_ContainerDynamicSeparationRepository : UnitTestBase
    {
        public TestContext TestContext { get; set; }

        private string OpenId { get; set; } = "hoge_Id";


        #region SetUp

        private void SetUpContainer(bool isOpenIdAuthentication = true)
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance(new Mock<IServiceBusEventRepository>().Object);

            DateTimeUtil dateTimeUtil = new DateTimeUtil("yyyy/MM/dd", "yyyy/MM/dd hh:mm:ss tt", "yyyy/M/d");
            var mockPerRequestDataContainer = new Mock<IPerRequestDataContainer>();
            mockPerRequestDataContainer.Setup(x => x.GetDateTimeUtil()).Returns(dateTimeUtil);
            if (isOpenIdAuthentication)
            {
                mockPerRequestDataContainer.SetupProperty(x => x.OpenId, OpenId);
            }
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(mockPerRequestDataContainer.Object);
        }

        #endregion

        #region GetOrRegisterContainerName

        [TestMethod]
        public void GetOrRegisterContainerName_正常系_登録済_キャッシュヒット()
        {
            var expectedContainerName = Guid.NewGuid().ToString();
            var physicalRepositoryId = new PhysicalRepositoryId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());

            var expectedCacheKey = CacheManager.CreateKey(
                ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY,
                physicalRepositoryId.Value,
                controllerId.Value,
                vendorId.Value,
                systemId.Value,
                openId.Value);

            this.SetUpContainer();

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Callback<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    key.Is(expectedCacheKey);
                })
                .Returns(expectedContainerName);
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var mockDB = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

            var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
            var result = testClass.GetOrRegisterContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId, out var isRegistered);
            result.Is(expectedContainerName);
            isRegistered.IsFalse();
        }

        [TestMethod]
        public void GetOrRegisterContainerName_正常系_登録済_ミスヒット()
        {
            var expectedContainerName = Guid.NewGuid().ToString();
            var physicalRepositoryId = new PhysicalRepositoryId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());

            var expectedCacheKey = CacheManager.CreateKey(
                ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY,
                physicalRepositoryId.Value,
                controllerId.Value,
                vendorId.Value,
                systemId.Value,
                openId.Value);

            this.SetUpContainer();

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Callback<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    key.Is(expectedCacheKey);
                })
                .Returns<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    return action().ToString();
                });
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var mockDB = new Mock<IJPDataHubDbConnection>();
            mockDB.Setup(x => x.QuerySingleOrDefault<string>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .Callback<string, object, IDbTransaction, int?, CommandType?>((sql, param, tran, timeout, commandType) =>
                {
                    (param.GetType().GetProperty("physical_repository_id").GetValue(param) as string).Is(physicalRepositoryId.Value);
                    (param.GetType().GetProperty("controller_id").GetValue(param) as string).Is(controllerId.Value);
                    (param.GetType().GetProperty("vendor_id").GetValue(param) as string).Is(vendorId.Value);
                    (param.GetType().GetProperty("system_id").GetValue(param) as string).Is(systemId.Value);
                })
                .Returns(expectedContainerName);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

            var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
            var result = testClass.GetOrRegisterContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId, out var isRegistered);
            result.Is(expectedContainerName);
            isRegistered.IsFalse();
            mockDB.Verify(x => x.QuerySingleOrDefault<string>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()), Times.Exactly(1));
        }

        [TestMethod]
        [TestCase(true)]
        [TestCase(false)]
        public void GetOrRegisterContainerName_正常系_未登録()
        {
            TestContext.Run((bool isOpenIdAuthentication) =>
            {
                var expectedContainerName = Guid.Empty;
                var physicalRepositoryId = new PhysicalRepositoryId(Guid.NewGuid().ToString());
                var controllerId = new ControllerId(Guid.NewGuid().ToString());
                var vendorId = new VendorId(Guid.NewGuid().ToString());
                var systemId = new SystemId(Guid.NewGuid().ToString());
                var openId = new OpenId(Guid.NewGuid().ToString());

                var cacheKeyPrefix = ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY;
                var allCacheKeyPrefix = ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY_ALL;
                var expectedCacheKey = CacheManager.CreateKey(
                    cacheKeyPrefix,
                    physicalRepositoryId.Value,
                    controllerId.Value,
                    vendorId.Value,
                    systemId.Value,
                    openId.Value);
                var expectedAllCacheKey = CacheManager.CreateKey(
                    allCacheKeyPrefix,
                    physicalRepositoryId.Value,
                    controllerId.Value);

                this.SetUpContainer(isOpenIdAuthentication);

                var mockCache = new Mock<ICache>();
                mockCache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                    .Callback<string, TimeSpan, ActionObject>((key, timespan, action) =>
                    {
                        key.Is(expectedCacheKey);
                    })
                    .Returns<string, TimeSpan, ActionObject>((key, timespan, action) =>
                    {
                        return action()?.ToString();
                    });
                mockCache.Setup(x => x.Remove(It.IsAny<string>()))
                    .Callback<string>(x =>
                    {
                        if (x.StartsWith(allCacheKeyPrefix))
                        {
                            x.Is(expectedAllCacheKey);
                        }
                        else
                        {
                            x.Is(expectedCacheKey);
                        }
                    });
                UnityContainer.RegisterInstance<ICache>(mockCache.Object);

                var mockDB = new Mock<IJPDataHubDbConnection>();
                mockDB.Setup(x => x.QuerySingleOrDefault<string>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                    .Callback<string, object, IDbTransaction, int?, CommandType?>((sql, param, tran, timeout, commandType) =>
                    {
                        (param.GetType().GetProperty("physical_repository_id").GetValue(param) as string).Is(physicalRepositoryId.Value);
                        (param.GetType().GetProperty("controller_id").GetValue(param) as string).Is(controllerId.Value);
                        (param.GetType().GetProperty("vendor_id").GetValue(param) as string).Is(vendorId.Value);
                        (param.GetType().GetProperty("system_id").GetValue(param) as string).Is(systemId.Value);
                    })
                    .Returns((string)null);
                mockDB.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                    .Callback<string, object, IDbTransaction, int?, CommandType?>((sql, param, tran, timeout, commandType) =>
                    {
                        Guid.TryParse((param.GetType().GetProperty("container_dynamic_separation_id").GetValue(param) as string), out _).IsTrue();
                        (param.GetType().GetProperty("physical_repository_id").GetValue(param) as string).Is(physicalRepositoryId.Value);
                        (param.GetType().GetProperty("controller_id").GetValue(param) as string).Is(controllerId.Value);
                        (param.GetType().GetProperty("vendor_id").GetValue(param) as string).Is(vendorId.Value);
                        (param.GetType().GetProperty("system_id").GetValue(param) as string).Is(systemId.Value);
                        Guid.TryParse((param.GetType().GetProperty("container_name").GetValue(param) as string), out expectedContainerName).IsTrue();
                        (param.GetType().GetProperty("reg_username").GetValue(param) as string).Is(isOpenIdAuthentication ? OpenId : "system");
                        (param.GetType().GetProperty("upd_username").GetValue(param) as string).Is(isOpenIdAuthentication ? OpenId : "system");
                        (param.GetType().GetProperty("is_active").GetValue(param) as bool?).Value.Is(true);
                    });
                UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

                var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
                var result = testClass.GetOrRegisterContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId, out var isRegistered);
                result.Is(expectedContainerName.ToString());
                isRegistered.IsTrue();
                mockDB.Verify(x => x.QuerySingleOrDefault<string>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()), Times.Exactly(1));
                mockDB.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()), Times.Exactly(1));
                mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(2));
            });
        }

        [TestMethod]
        public void GetOrRegisterContainerName_正常系_未登録_キー重複()
        {
            var expectedContainerName = Guid.Empty;
            var physicalRepositoryId = new PhysicalRepositoryId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());
            var openId = new OpenId(Guid.NewGuid().ToString());

            var expectedCacheKey = CacheManager.CreateKey(
                ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY,
                physicalRepositoryId.Value,
                controllerId.Value,
                vendorId.Value,
                systemId.Value,
                openId.Value);

            this.SetUpContainer();

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Callback<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    key.Is(expectedCacheKey);
                })
                .Returns<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    return action()?.ToString();
                });
            mockCache.Setup(x => x.Remove(It.IsAny<string>()))
                .Callback<string>(x => x.Is(expectedCacheKey));
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var times = 0;
            var mockDB = new Mock<IJPDataHubDbConnection>();
            mockDB.Setup(x => x.QuerySingleOrDefault<string>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .Callback<string, object, IDbTransaction, int?, CommandType?>((sql, param, tran, timeout, commandType) =>
                {
                    (param.GetType().GetProperty("physical_repository_id").GetValue(param) as string).Is(physicalRepositoryId.Value);
                    (param.GetType().GetProperty("controller_id").GetValue(param) as string).Is(controllerId.Value);
                    (param.GetType().GetProperty("vendor_id").GetValue(param) as string).Is(vendorId.Value);
                    (param.GetType().GetProperty("system_id").GetValue(param) as string).Is(systemId.Value);
                    times++;
                })
                .Returns(() =>
                {
                    return times == 1 ? null : expectedContainerName.ToString();
                });
            mockDB.Setup(x => x.Execute(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()))
                .Callback<string, object, IDbTransaction, int?, CommandType?>((sql, param, tran, timeout, commandType) =>
                {
                    Guid.TryParse((param.GetType().GetProperty("container_dynamic_separation_id").GetValue(param) as string), out _).IsTrue();
                    (param.GetType().GetProperty("physical_repository_id").GetValue(param) as string).Is(physicalRepositoryId.Value);
                    (param.GetType().GetProperty("controller_id").GetValue(param) as string).Is(controllerId.Value);
                    (param.GetType().GetProperty("vendor_id").GetValue(param) as string).Is(vendorId.Value);
                    (param.GetType().GetProperty("system_id").GetValue(param) as string).Is(systemId.Value);
                    Guid.TryParse((param.GetType().GetProperty("container_name").GetValue(param) as string), out expectedContainerName).IsTrue();
                    (param.GetType().GetProperty("reg_username").GetValue(param) as string).Is(OpenId);
                    (param.GetType().GetProperty("upd_username").GetValue(param) as string).Is(OpenId);
                    (param.GetType().GetProperty("is_active").GetValue(param) as bool?).Value.Is(true);
                })
                .Throws(CreateSqlException(2627));
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

            var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
            var result = testClass.GetOrRegisterContainerName(physicalRepositoryId, controllerId, vendorId, systemId, openId, out var isRegistered);
            result.Is(expectedContainerName.ToString());
            isRegistered.IsFalse();
            mockDB.Verify(x => x.QuerySingleOrDefault<string>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()), Times.Exactly(2));
            mockDB.Verify(x => x.Execute(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>(), It.IsAny<int?>(), It.IsAny<CommandType?>()), Times.Exactly(1));
            mockCache.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(1));
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOrRegisterContainerName_異常系_物理リポジトリIDなし()
        {
            ArgumentExceptionTest(null, new ControllerId(Guid.NewGuid().ToString()), new VendorId(Guid.NewGuid().ToString()), new SystemId(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOrRegisterContainerName_異常系_コントローラIDなし()
        {
            ArgumentExceptionTest(new PhysicalRepositoryId(Guid.NewGuid().ToString()), null, new VendorId(Guid.NewGuid().ToString()), new SystemId(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOrRegisterContainerName_異常系_ベンダーIDなし()
        {
            ArgumentExceptionTest(new PhysicalRepositoryId(Guid.NewGuid().ToString()), new ControllerId(Guid.NewGuid().ToString()), null, new SystemId(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetOrRegisterContainerName_異常系_システムIDなし()
        {
            ArgumentExceptionTest(new PhysicalRepositoryId(Guid.NewGuid().ToString()), new ControllerId(Guid.NewGuid().ToString()), new VendorId(Guid.NewGuid().ToString()), null);
        }

        private void ArgumentExceptionTest(PhysicalRepositoryId physicalRepositoryId, ControllerId controllerId, VendorId vendorId, SystemId systemId)
        {
            this.SetUpContainer();

            var mockCache = new Mock<ICache>();
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var mockDB = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

            var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
            testClass.GetOrRegisterContainerName(physicalRepositoryId, controllerId, vendorId, systemId);
        }

        private SqlException CreateSqlException(int number)
        {
            var collectionConstructor = typeof(SqlErrorCollection).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance);
            var errorCollection = (SqlErrorCollection)collectionConstructor.Invoke(null);
            var errorConstructor = typeof(SqlError).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int), typeof(uint), typeof(Exception) }, null);
            var error = errorConstructor.Invoke(new object[] { number, (byte)0, (byte)0, "server", "errMsg", "proccedure", 100, (uint)0, null });

            addMethod.Invoke(errorCollection, new[] { error });

            var constructor = typeof(SqlException).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new[] { typeof(string), typeof(SqlErrorCollection), typeof(Exception), typeof(Guid) }, null);
            return (SqlException)constructor.Invoke(new object[] { null, errorCollection, new DataException(), Guid.NewGuid() });
        }

        #endregion

        #region GetAllContainerNames

        [TestMethod]
        public void GetAllContainerNames_正常系_キャッシュヒット()
        {
            var expectedContainerNames = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var physicalRepositoryId = new PhysicalRepositoryId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());

            var expectedCacheKey = CacheManager.CreateKey(
                ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY_ALL,
                physicalRepositoryId.Value,
                controllerId.Value);

            this.SetUpContainer();

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<List<string>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Callback<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    key.Is(expectedCacheKey);
                })
                .Returns(expectedContainerNames);
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var mockDB = new Mock<IJPDataHubDbConnection>();
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

            var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
            var result = testClass.GetAllContainerNames(physicalRepositoryId, controllerId);
            result.IsStructuralEqual(expectedContainerNames);
        }

        [TestMethod]
        public void GetAllContainerNames_正常系_ミスヒット()
        {
            var expectedContainerNames = new List<string>() { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            var physicalRepositoryId = new PhysicalRepositoryId(Guid.NewGuid().ToString());
            var controllerId = new ControllerId(Guid.NewGuid().ToString());

            var expectedCacheKey = CacheManager.CreateKey(
                ContainerDynamicSeparationRepository.CACHE_CONTAINER_DYNAMIC_SEPARATION_REPOSITORY_ALL,
                physicalRepositoryId.Value,
                controllerId.Value);

            this.SetUpContainer();

            var mockCache = new Mock<ICache>();
            mockCache.Setup(x => x.Get<List<string>>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Callback<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    key.Is(expectedCacheKey);
                })
                .Returns<string, TimeSpan, ActionObject>((key, timespan, action) =>
                {
                    return (List<string>)action();
                });
            UnityContainer.RegisterInstance<ICache>(mockCache.Object);

            var mockDB = new Mock<IJPDataHubDbConnection>();
            mockDB.Setup(x => x.Query<string>(It.IsAny<string>(), It.IsAny<object>()))
                .Callback<string, object>((sql, param) =>
                {
                    (param.GetType().GetProperty("physical_repository_id").GetValue(param) as string).Is(physicalRepositoryId.Value);
                    (param.GetType().GetProperty("controller_id").GetValue(param) as string).Is(controllerId.Value);
                })
                .Returns(expectedContainerNames);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("DynamicApi", mockDB.Object);

            var testClass = UnityContainer.Resolve<IContainerDynamicSeparationRepository>();
            var result = testClass.GetAllContainerNames(physicalRepositoryId, controllerId);
            result.IsStructuralEqual(expectedContainerNames);
            mockDB.Verify(x => x.Query<string>(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(1));
        }

        #endregion
    }
}
