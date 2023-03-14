using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using EasyCaching.Core;
using EasyCaching.InMemory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Exceptions;
using JP.DataHub.Com.Transaction;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Authentication;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_AuthenticationRepository : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        public void AuthenticationRepository_Login_正常()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var returnValue = new List<AuthenticationRepository.VendorSystemFunction>
            {
                GetVendorSystemFunction(
                )
            };
            var userId = new UserId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(returnValue[0].vendor_id.ToString());
            var systemId = new SystemId(returnValue[0].system_id.ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(returnValue);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var expectVendor = Vendor.Create(
                new VendorId(returnValue[0].vendor_id.ToString()),
                new VendorName(returnValue[0].vendor_name),
                new IsDataOffer(returnValue[0].is_data_offer),
                new IsDataUse(returnValue[0].is_data_use),
                new IsEnable(returnValue[0].is_enable)
            );

            expectVendor.ApiFunction = new FunctionNames(new List<string>
            {
                returnValue[0].function_name
            });

            var expectSystem = new SystemEntity(
                new SystemId(returnValue[0].system_id.ToString()),
                new SystemName(returnValue[0].system_name)
            );

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();
            var result = target.Login(vendorId, systemId, userId);
            cache.Clear();

            result.Vendor.IsStructuralEqual(expectVendor);
            result.System.IsStructuralEqual(expectSystem);
            result.GetType().GetProperty("UserId", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(result).IsSameReferenceAs(userId);

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_Login_正常_userId無し()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var returnValue = new List<AuthenticationRepository.VendorSystemFunction>
            {
                GetVendorSystemFunction(
                )
            };
            var vendorId = new VendorId(returnValue[0].vendor_id.ToString());
            var systemId = new SystemId(returnValue[0].system_id.ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(returnValue);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var expectVendor = Vendor.Create(
                new VendorId(returnValue[0].vendor_id.ToString()),
                new VendorName(returnValue[0].vendor_name),
                new IsDataOffer(returnValue[0].is_data_offer),
                new IsDataUse(returnValue[0].is_data_use),
                new IsEnable(returnValue[0].is_enable)
            );

            expectVendor.ApiFunction = new FunctionNames(new List<string>
            {
                returnValue[0].function_name
            });
            var expectSystem = new SystemEntity(
                new SystemId(returnValue[0].system_id.ToString()),
                new SystemName(returnValue[0].system_name)
            );

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();
            var result = target.Login(vendorId, systemId);
            cache.Clear();

            result.Vendor.IsStructuralEqual(expectVendor);
            result.System.IsStructuralEqual(expectSystem);
            result.GetType().GetProperty("UserId", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(result).IsSameReferenceAs(null);

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_Login_正常_function無し()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var vendorSystemFunction = GetVendorSystemFunction();
            vendorSystemFunction.function_name = null;

            var returnValue = new List<AuthenticationRepository.VendorSystemFunction>
            {
                vendorSystemFunction
            };

            var userId = new UserId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(returnValue[0].vendor_id.ToString());
            var systemId = new SystemId(returnValue[0].system_id.ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(returnValue);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var expectVendor = Vendor.Create(
                new VendorId(returnValue[0].vendor_id.ToString()),
                new VendorName(returnValue[0].vendor_name),
                new IsDataOffer(returnValue[0].is_data_offer),
                new IsDataUse(returnValue[0].is_data_use),
                new IsEnable(returnValue[0].is_enable)
            );

            expectVendor.ApiFunction = new FunctionNames(new List<string>
            {
            });
            var expectSystem = new SystemEntity(
                new SystemId(returnValue[0].system_id.ToString()),
                new SystemName(returnValue[0].system_name)
            );

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();
            var result = target.Login(vendorId, systemId, userId);
            cache.Clear();

            result.Vendor.IsStructuralEqual(expectVendor);
            result.System.IsStructuralEqual(expectSystem);
            result.GetType().GetProperty("UserId", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(result).IsSameReferenceAs(userId);

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_Login_正常_function2つ()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var vendorSystemFunction = GetVendorSystemFunction();
            var returnValue = new List<AuthenticationRepository.VendorSystemFunction>
            {
                vendorSystemFunction,
                GetVendorSystemFunction(
                    vendorSystemFunction.vendor_id,
                    vendorSystemFunction.system_id,
                    vendorSystemFunction.vendor_name,
                    vendorSystemFunction.system_name,
                    Guid.NewGuid().ToString(),
                    vendorSystemFunction.is_data_offer,
                    vendorSystemFunction.is_data_use,
                    vendorSystemFunction.is_enable
                )
            };

            var userId = new UserId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(returnValue[0].vendor_id.ToString());
            var systemId = new SystemId(returnValue[0].system_id.ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(returnValue);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var expectVendor = Vendor.Create(
                new VendorId(returnValue[0].vendor_id.ToString()),
                new VendorName(returnValue[0].vendor_name),
                new IsDataOffer(returnValue[0].is_data_offer),
                new IsDataUse(returnValue[0].is_data_use),
                new IsEnable(returnValue[0].is_enable)
            );

            expectVendor.ApiFunction = new FunctionNames(new List<string>
            {
                returnValue[0].function_name,
                returnValue[1].function_name
            });

            var expectSystem = new SystemEntity(
                new SystemId(returnValue[0].system_id.ToString()),
                new SystemName(returnValue[0].system_name)
            );

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();
            var result = target.Login(vendorId, systemId, userId);
            cache.Clear();

            result.Vendor.IsStructuralEqual(expectVendor);
            result.System.IsStructuralEqual(expectSystem);
            result.GetType().GetProperty("UserId", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance).GetValue(result).IsSameReferenceAs(userId);

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_Login_データ無し()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var returnValue = new List<AuthenticationRepository.VendorSystemFunction>();
            var userId = new UserId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(returnValue);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();

            AssertEx.Catch<NotFoundException>(() => target.Login(vendorId, systemId, userId))
                .Message.Is("vendor");

            cache.Clear();

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_Login_vendorId_Null()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var userId = new UserId(Guid.NewGuid().ToString());
            var systemId = new SystemId(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ));
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();
            
            AssertEx.Catch<NotFoundException>(() => target.Login(null, systemId, userId))
                .Message.Is("vendor");

            cache.Clear();

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(0));
        }

        [TestMethod]
        public void AuthenticationRepository_Login_systemId_Null()
        {
            var cachingProvider = new DefaultInMemoryCachingProvider("Authority", new[] { new InMemoryCaching("Authority", new InMemoryCachingOptions()) }, new InMemoryOptions(), null);
            UnityContainer.RegisterInstance<IEasyCachingProvider>(cachingProvider);
            UnityContainer.RegisterType<ICache, InMemoryCache>("Authority", new PerResolveLifetimeManager(), new InjectionConstructor("Authority", 100));

            var userId = new UserId(Guid.NewGuid().ToString());
            var vendorId = new VendorId(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ));
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();

            var cache = new InMemoryCache("Authority");
            cache.Clear();
            
            AssertEx.Catch<NotFoundException>(() => target.Login(vendorId, null, userId))
                .Message.Is("system");

            cache.Clear();

            mock.Verify(x => x.Query<AuthenticationRepository.VendorSystemFunction>
            (
                It.IsAny<string>(),
                It.IsAny<object>()
            ), Times.Exactly(0));
        }

        [TestMethod]
        public void AuthenticationRepository_IsAdmin_正常系()
        {
            var requestAdminKeyword = new AdminKeyword(Guid.NewGuid().ToString());
            var requestSystemId = new SystemId(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingleOrDefault<string>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()
                ))
                .Returns(requestAdminKeyword.Value);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, expireTime, action) => (string)action.Invoke());
            UnityContainer.RegisterInstance("Authority", mockICache.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();
            var result = target.IsAdmin(requestAdminKeyword, requestSystemId);

            result.Value.Is(true);

            mock.Verify(x => x.QuerySingleOrDefault<string>
            (
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_IsAdmin_正常系_systemIdなし()
        {
            var requestAdminKeyword = new AdminKeyword(UnityCore.Resolve<string>("AdminKeyword"));
            SystemId requestSystemId = null;

            var mockICache = new Mock<ICache>();
            UnityContainer.RegisterInstance(mockICache.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();
            var result = target.IsAdmin(requestAdminKeyword, requestSystemId);

            result.Value.Is(true);
        }

        [TestMethod]
        public void AuthenticationRepository_IsAdmin_正常系_認証エラー_SystemIdなし()
        {
            var requestAdminKeyword = new AdminKeyword(Guid.NewGuid().ToString());
            SystemId requestSystemId = null;

            var mockICache = new Mock<ICache>();
            UnityContainer.RegisterInstance("Authority", mockICache.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();
            var result = target.IsAdmin(requestAdminKeyword, requestSystemId);

            result.Value.Is(false);
        }

        [TestMethod]
        public void AuthenticationRepository_IsAdmin_正常系_認証エラー_SystemIdあり()
        {
            var requestAdminKeyword = new AdminKeyword(Guid.NewGuid().ToString());
            var requestSystemId = new SystemId(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingleOrDefault<string>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()
                ))
                .Returns(Guid.NewGuid().ToString());
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, expireTime, action) => (string)action.Invoke());
            UnityContainer.RegisterInstance("Authority", mockICache.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();
            var result = target.IsAdmin(requestAdminKeyword, requestSystemId);

            result.Value.Is(false);

            mock.Verify(x => x.QuerySingleOrDefault<string>
            (
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()
            ), Times.Exactly(1));
        }

        [TestMethod]
        public void AuthenticationRepository_IsAdmin_正常系_認証エラー_adminSecretなし()
        {
            var requestAdminKeyword = new AdminKeyword(Guid.NewGuid().ToString());
            var requestSystemId = new SystemId(Guid.NewGuid().ToString());

            var mock = new Mock<IJPDataHubDbConnection>();
            mock.Setup(x => x.QuerySingleOrDefault<string>(
                    It.IsAny<string>(),
                    It.IsAny<object>(),
                    It.IsAny<IDbTransaction>(),
                    It.IsAny<int?>(),
                    It.IsAny<CommandType?>()
                ))
                .Returns((string)null);
            UnityContainer.RegisterInstance<IJPDataHubDbConnection>("Authority", mock.Object);

            var mockICache = new Mock<ICache>();
            mockICache.Setup(x => x.Get<string>(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<ActionObject>()))
                .Returns<string, TimeSpan, ActionObject>((key, expireTime, action) => (string)action.Invoke());
            UnityContainer.RegisterInstance("Authority", mockICache.Object);

            var target = UnityContainer.Resolve<IAuthenticationRepository>();
            var result = target.IsAdmin(requestAdminKeyword, requestSystemId);

            result.Value.Is(false);

            mock.Verify(x => x.QuerySingleOrDefault<string>
            (
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>(),
                It.IsAny<int?>(),
                It.IsAny<CommandType?>()
            ), Times.Exactly(1));
        }

        private AuthenticationRepository.VendorSystemFunction GetVendorSystemFunction(
            Guid? vendorId = null,
            Guid? systemId = null,
            string vendorName = null,
            string systemName = null,
            string functionName = null,
            bool? isDataOffer = null,
            bool? isDataUse = null,
            bool? isEnable = null
        )
        {
            return new AuthenticationRepository.VendorSystemFunction
            {
                vendor_id = vendorId ?? Guid.NewGuid(),
                vendor_name = vendorName ?? Guid.NewGuid().ToString(),
                system_id = systemId ?? Guid.NewGuid(),
                system_name = systemName ?? Guid.NewGuid().ToString(),
                function_name = functionName ?? Guid.NewGuid().ToString(),
                is_data_offer = isDataOffer ?? false,
                is_data_use = isDataUse ?? false,
                is_enable = isEnable ?? false
            };
        }
    }
}
