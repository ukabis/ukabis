using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using cache=JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass]
    public class UnitTest_CacheHelper : UnitTestBase
    {
        private static readonly int s_roslynScriptCacheExpirationDefaultSecond = 1800;
        private static readonly int s_roslynScriptCacheExpirationMaxSecond = 86400;
        private static readonly int s_roslynScriptCacheValueMaxSize = 1048576;

        private string _vendorId = Guid.NewGuid().ToString();
        private string _systemId = Guid.NewGuid().ToString();
        private string _apiId = Guid.NewGuid().ToString();
        private string _openId = Guid.NewGuid().ToString();

        private class TestCacheObject
        {
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        #region Setup

        private void SetUpContainer(bool isRoslynScriptCacheEnable = true, bool isVendor = true, bool isPerson = false)
        {
            base.TestInitialize(true);

            var dataContainer = new Mock<IPerRequestDataContainer>();
            dataContainer.SetupGet(x => x.OpenId).Returns(_openId);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer.Object);

            UnityContainer.RegisterInstance("IsRoslynScriptCacheEnable", isRoslynScriptCacheEnable);

            var mockIDynamicApiAction = new Mock<IDynamicApiAction>();
            mockIDynamicApiAction.SetupGet(x => x.VendorId).Returns(new VendorId(_vendorId));
            mockIDynamicApiAction.SetupGet(x => x.SystemId).Returns(new SystemId(_systemId));
            mockIDynamicApiAction.SetupGet(x => x.ApiId).Returns(new ApiId(_apiId));
            mockIDynamicApiAction.SetupGet(x => x.IsVendor).Returns(new IsVendor(isVendor));
            mockIDynamicApiAction.SetupGet(x => x.IsPerson).Returns(new IsPerson(isPerson));

            var mockIDynamicApiDataContainer = new Mock<IDynamicApiDataContainer>();
            mockIDynamicApiDataContainer.SetupGet(x => x.baseApiAction).Returns(mockIDynamicApiAction.Object);
            UnityContainer.RegisterInstance(mockIDynamicApiDataContainer.Object);
        }

        #endregion

        [TestMethod]
        public void Add_正常系()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", "CacheValue");

            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", "CacheValue", new TimeSpan(0, 0, s_roslynScriptCacheExpirationDefaultSecond), s_roslynScriptCacheValueMaxSize), Times.Once);
            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Never);
        }

        [TestMethod]
        public void Add_正常系_string以外()
        {
            this.SetUpContainer();

            var expect = new TestCacheObject { Value1 = "CacheValue", Value2 = 1 };
            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", expect);

            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", expect, new TimeSpan(0, 0, s_roslynScriptCacheExpirationDefaultSecond), s_roslynScriptCacheValueMaxSize), Times.Once);
            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Never);
        }

        [TestMethod]
        public void Add_正常系_有効期限指定()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", "CacheValue", new TimeSpan(1, 1, 1));

            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", "CacheValue", new TimeSpan(1, 1, 1), s_roslynScriptCacheValueMaxSize), Times.Once);
            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Never);
        }

        [TestMethod]
        public void Add_正常系_更新()
        {
            this.SetUpContainer();

            var isContains = true;
            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Contains(It.IsAny<string>())).Returns(isContains);
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", "CacheValue");

            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", "CacheValue", new TimeSpan(0, 0, s_roslynScriptCacheExpirationDefaultSecond), s_roslynScriptCacheValueMaxSize), Times.Once);
            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Once);
        }

        [TestMethod]
        public void Add_正常系_RoslynCacheが無効()
        {
            this.SetUpContainer(isRoslynScriptCacheEnable: false);

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", "CacheValue", new TimeSpan(1, 1, 1));

            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", "CacheValue", new TimeSpan(1, 1, 1), s_roslynScriptCacheValueMaxSize), Times.Never);
            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_異常系_KeyがNull()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add(null, "CacheValue");
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "key length should not be more than 1000")]
        public void Add_異常系_Keyのlengthが最大値超過()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add(string.Join("", Enumerable.Repeat<int>(1, 1001).Select(m => m.ToString())), "CacheValue");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_異常系_valueがNull()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", null);
        }

        [TestMethod]
        public void Add_正常系_expirationが最大値を超えた場合()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Add("CacheKey", "CacheValue", new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond + 1));

            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", "CacheValue", new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond), s_roslynScriptCacheValueMaxSize), Times.Once);
        }

        [TestMethod]
        public void Get_正常系()
        {
            this.SetUpContainer();

            var expect = "CacheValue";
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(expect);
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.Get<string>("CacheKey");

            mockCache.Verify(m => m.Get<string>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Once);
            Assert.AreEqual(result, expect);
        }

        [TestMethod]
        public void Get_正常系_string以外()
        {
            this.SetUpContainer();

            var expect = new TestCacheObject { Value1 = "CacheValue", Value2 = 1 };
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<TestCacheObject>(It.IsAny<string>(), out outValue, false)).Returns(expect);
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.Get<TestCacheObject>("CacheKey");

            mockCache.Verify(m => m.Get<TestCacheObject>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Once);

            result.IsStructuralEqual(expect);
        }

        [TestMethod]
        public void Get_正常系_RoslynCacheが無効()
        {
            this.SetUpContainer(isRoslynScriptCacheEnable: false);

            var expect = "CacheValue";
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(expect);
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.Get<string>("CacheKey");

            mockCache.Verify(m => m.Get<string>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Never);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_異常系_KeyがNull()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns("CacheValue");
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Get<string>(null);
        }

        [TestMethod]
        public void GetOrAdd_正常系_キャッシュされている場合()
        {
            this.SetUpContainer();

            var expect = "CacheValue";
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(expect);
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.GetOrAdd("CacheKey", new TimeSpan(1, 1, 1), () => expect);

            mockCache.Verify(m => m.Get<string>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            Assert.AreEqual(result, expect);
        }

        [TestMethod]
        public void GetOrAdd_正常系_キャッシュされてない場合()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(default(string));
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var expect = "CacheValue";
            var cacheHelper = new CacheHelper();
            var result = cacheHelper.GetOrAdd("CacheKey", new TimeSpan(1, 1, 1), () => expect);

            mockCache.Verify(m => m.Get<string>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", expect, new TimeSpan(1, 1, 1), s_roslynScriptCacheValueMaxSize), Times.Once);
            Assert.AreEqual(result, expect);
        }

        [TestMethod]
        public void GetOrAdd_正常系_RoslynCacheが無効()
        {
            this.SetUpContainer(isRoslynScriptCacheEnable: false);

            var expect = "CacheValue";
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(expect);
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.GetOrAdd("CacheKey", new TimeSpan(1, 1, 1), () => expect);

            mockCache.Verify(m => m.Get<string>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Never);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetOrAdd_正常系_string以外()
        {
            this.SetUpContainer();

            var expect = new TestCacheObject { Value1 = "CacheValue", Value2 = 1 };
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<TestCacheObject>(It.IsAny<string>(), out outValue, false)).Returns(expect);
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.GetOrAdd("CacheKey", new TimeSpan(1, 1, 1), () => expect);

            mockCache.Verify(m => m.Get<TestCacheObject>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);

            result.IsStructuralEqual(expect);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrAdd_異常系_KeyがNull()
        {
            this.SetUpContainer();

            var expect = "CacheValue";
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(default(string));
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.GetOrAdd(null, new TimeSpan(1, 1, 1), () => expect);
        }

        [TestMethod]
        public void GetOrAdd_正常系_expirationが最大値を超えた場合()
        {
            this.SetUpContainer();

            var expect = "CacheValue";
            var mockCache = new Mock<cache.ICache>();
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(default(string));
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            var result = cacheHelper.GetOrAdd("CacheKey", new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond + 1), () => expect);

            mockCache.Verify(m => m.Get<string>($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add($"{_apiId}_{_vendorId}_{_systemId}_CacheKey", expect, new TimeSpan(0, 0, s_roslynScriptCacheExpirationMaxSecond), s_roslynScriptCacheValueMaxSize), Times.Once);
            Assert.AreEqual(result, expect);
        }

        [TestMethod]
        public void Remove_正常系()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Remove("CacheKey");

            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Once);
        }

        [TestMethod]
        public void Remove_正常系_RoslynCacheが無効()
        {
            this.SetUpContainer(isRoslynScriptCacheEnable: false);

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Remove("CacheKey");

            mockCache.Verify(m => m.Remove($"{_apiId}_{_vendorId}_{_systemId}_CacheKey"), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_正常系_KeyがNull()
        {
            this.SetUpContainer();

            var mockCache = new Mock<cache.ICache>();
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            UnityContainer.RegisterInstance("RoslynCache", mockCache.Object);

            var cacheHelper = new CacheHelper();
            cacheHelper.Remove(null);
        }

        [TestMethod]
        public void GetKeyPrefix_正常系_Vender依存_個人依存()
        {
            this.SetUpContainer(isVendor: false, isPerson: false);

            Assert.AreEqual(GetKeyPrefix(), $"{_apiId}_");

            this.SetUpContainer(isVendor: true, isPerson: false);

            Assert.AreEqual(GetKeyPrefix(), $"{_apiId}_{_vendorId}_{_systemId}_");

            this.SetUpContainer(isVendor: false, isPerson: true);

            Assert.AreEqual(GetKeyPrefix(), $"{_apiId}_{_openId}_");

            this.SetUpContainer(isVendor: true, isPerson: true);

            Assert.AreEqual(GetKeyPrefix(), $"{_apiId}_{_vendorId}_{_systemId}_{_openId}_");
        }

        private static string GetKeyPrefix()
        {
            var helper = new CacheHelper();
            return (string)helper.GetType().InvokeMember("GetKeyPrefix", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, helper, new object[] { });
        }
    }
}
