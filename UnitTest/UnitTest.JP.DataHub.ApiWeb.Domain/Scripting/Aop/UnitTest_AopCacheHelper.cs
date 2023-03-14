using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Scripting.Aop;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Aop
{
    [TestClass]
    public class UnitTest_CacheHelper : UnitTestBase
    {
        private static readonly int DEFAULT_AOPCACHE_EXPIRATION_MAX_SECOND = 86400;
        private static readonly int DEFAULT_AOPCACHE_EXPIRATION_SECOND = 1800;
        private static readonly int DEFAULT_AOPCACHE_KEY_MAX_LENGTH = 1000;
        private static readonly int DEFAULT_AOPCACHE_VALUE_MAX_SIZE = 1048576;

        private class TestCacheObject
        {
            public string Value1 { get; set; }
            public int Value2 { get; set; }
        }

        #region Setup

        private (UnityContainer, Mock<ICache>) SetUpContainer(bool isAopCacheEnable = true)
        {
            var container = new UnityContainer();
            container.RegisterInstance(Configuration);
            container.RegisterInstance("IsAopCacheEnable", isAopCacheEnable);

            var mockCache = new Mock<ICache>();
            mockCache.Setup(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()));
            mockCache.Setup(m => m.Remove(It.IsAny<string>()));
            container.RegisterInstance("AopCache", mockCache.Object);

            UnityCore.UnityContainer = container;

            return (container, mockCache);
        }

        #endregion

        #region Add

        [TestMethod]
        public void Add_正常系()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue);

            mockCache.Verify(m => m.Add(actualCacheKey, cacheValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_SECOND), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            mockCache.Verify(m => m.Remove(actualCacheKey), Times.Never);
        }

        [TestMethod]
        public void Add_正常系_string以外()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = new TestCacheObject { Value1 = Guid.NewGuid().ToString(), Value2 = 1 };
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue);

            mockCache.Verify(m => m.Add(actualCacheKey, cacheValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_SECOND), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            mockCache.Verify(m => m.Remove(actualCacheKey), Times.Never);
        }

        [TestMethod]
        public void Add_正常系_有効期限指定()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue, new TimeSpan(1, 2, 3));

            mockCache.Verify(m => m.Add(actualCacheKey, cacheValue, new TimeSpan(1, 2, 3), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            mockCache.Verify(m => m.Remove(actualCacheKey), Times.Never);
        }

        [TestMethod]
        public void Add_正常系_更新()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            mockCache.Setup(m => m.Contains(It.IsAny<string>())).Returns(true);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue);

            mockCache.Verify(m => m.Add(actualCacheKey, cacheValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_SECOND), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            mockCache.Verify(m => m.Remove(actualCacheKey), Times.Once);
        }

        [TestMethod]
        public void Add_正常系_AopCacheが無効()
        {
            var (container, mockCache) = SetUpContainer(false);

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue, new TimeSpan(1, 2, 3));

            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_異常系_KeyがNull()
        {
            _ = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(null, cacheValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "key length should not be more than 1000")]
        public void Add_異常系_Keyのlengthが最大値超過()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = string.Join("", Enumerable.Repeat<int>(1, DEFAULT_AOPCACHE_KEY_MAX_LENGTH + 1).Select(m => 1.ToString()));
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_異常系_valueがNull()
        {
            _ = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, null);
        }

        [TestMethod]
        public void Add_正常系_expirationが最大値を超えた場合()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Add(cacheKeyInput, cacheValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_MAX_SECOND + 1));

            mockCache.Verify(m => m.Add(actualCacheKey, cacheValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_MAX_SECOND), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
        }

        #endregion

        #region Get

        [TestMethod]
        public void Get_正常系()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            
            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.Get<string>(cacheKeyInput);

            mockCache.Verify(m => m.Get<string>(actualCacheKey, out outValue, false), Times.Once);
            Assert.AreEqual(result, cacheValue);
        }

        [TestMethod]
        public void Get_正常系_string以外()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = new TestCacheObject { Value1 = Guid.NewGuid().ToString(), Value2 = 1 };
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            
            var outValue = false;
            mockCache.Setup(m => m.Get<TestCacheObject>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.Get<TestCacheObject>(cacheKeyInput);

            mockCache.Verify(m => m.Get<TestCacheObject>(actualCacheKey, out outValue, false), Times.Once);
            result.IsStructuralEqual(cacheValue);
        }

        [TestMethod]
        public void Get_正常系_AopCacheが無効()
        {
            var (container, mockCache) = SetUpContainer(false);

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.Get<TestCacheObject>(cacheKeyInput);
            
            var outValue = false;
            mockCache.Verify(m => m.Get<string>(It.IsAny<string>(), out outValue, false), Times.Never);
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_異常系_KeyがNull()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();

            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Get<string>(null);
        }

        #endregion

        #region GetOrAdd

        [TestMethod]
        public void GetOrAdd_正常系_キャッシュされている場合()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = Guid.NewGuid().ToString();
            var outValue = false;

            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd(cacheKeyInput, new TimeSpan(1, 2, 3), () => newValue);

            mockCache.Verify(m => m.Get<string>(actualCacheKey, out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            Assert.AreEqual(result, cacheValue);
        }

        [TestMethod]
        public void GetOrAdd_正常系_キャッシュされてない場合()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = default(string);
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = Guid.NewGuid().ToString();
            var outValue = false;

            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd(cacheKeyInput, new TimeSpan(1, 2, 3), () => newValue);

            mockCache.Verify(m => m.Get<string>(actualCacheKey, out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(actualCacheKey, newValue, new TimeSpan(1, 2, 3), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            Assert.AreEqual(result, newValue);
        }

        [TestMethod]
        public void GetOrAdd_正常系_キャッシュされてない場合_null()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = default(string);
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = default(string);

            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd(cacheKeyInput, new TimeSpan(1, 2, 3), () => newValue);

            mockCache.Verify(m => m.Get<string>(actualCacheKey, out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            Assert.AreEqual(result, newValue);
        }

        [TestMethod]
        public void GetOrAdd_正常系_AopCacheが無効()
        {
            var (container, mockCache) = SetUpContainer(false);

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = default(string);
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = Guid.NewGuid().ToString();
            var outValue = false;

            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd(cacheKeyInput, new TimeSpan(1, 2, 3), () => newValue);

            mockCache.Verify(m => m.Get<string>(It.IsAny<string>(), out outValue, false), Times.Never);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            Assert.AreEqual(result, newValue);
        }

        [TestMethod]
        public void GetOrAdd_正常系_string以外()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = new TestCacheObject { Value1 = Guid.NewGuid().ToString(), Value2 = 1 };
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = new TestCacheObject { Value1 = Guid.NewGuid().ToString(), Value2 = 2 };

            var outValue = false;
            mockCache.Setup(m => m.Get<TestCacheObject>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd(cacheKeyInput, new TimeSpan(1, 2, 3), () => newValue);

            mockCache.Verify(m => m.Get<TestCacheObject>(actualCacheKey, out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan>(), It.IsAny<int>()), Times.Never);
            result.IsStructuralEqual(cacheValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetOrAdd_異常系_KeyがNull()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var newValue = Guid.NewGuid().ToString();

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd<string>(null, new TimeSpan(1, 2, 3), () => newValue);
        }

        [TestMethod]
        public void GetOrAdd_正常系_expirationが最大値を超えた場合()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = default(string);
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = Guid.NewGuid().ToString();

            var outValue = false;
            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd<string>(cacheKeyInput, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_MAX_SECOND + 1), () => newValue);

            mockCache.Verify(m => m.Get<string>(actualCacheKey, out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(actualCacheKey, newValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_MAX_SECOND), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            Assert.AreEqual(result, newValue);
        }


        [TestMethod]
        public void GetOrAdd_正常系_有効期限指定なし()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var cacheValue = default(string);
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";
            var newValue = Guid.NewGuid().ToString();
            var outValue = false;

            mockCache.Setup(m => m.Get<string>(It.IsAny<string>(), out outValue, false)).Returns(cacheValue);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            var result = cacheHelper.GetOrAdd(cacheKeyInput, () => newValue);

            mockCache.Verify(m => m.Get<string>(actualCacheKey, out outValue, false), Times.Once);
            mockCache.Verify(m => m.Add(actualCacheKey, newValue, new TimeSpan(0, 0, DEFAULT_AOPCACHE_EXPIRATION_SECOND), DEFAULT_AOPCACHE_VALUE_MAX_SIZE), Times.Once);
            Assert.AreEqual(result, newValue);
        }

        #endregion

        #region Remove

        [TestMethod]
        public void Remove_正常系()
        {
            var (container, mockCache) = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Remove(cacheKeyInput);

            mockCache.Verify(m => m.Remove(actualCacheKey), Times.Once);
        }

        [TestMethod]
        public void Remove_正常系_AopCacheが無効()
        {
            var (container, mockCache) = SetUpContainer(false);

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = Guid.NewGuid().ToString();
            var actualCacheKey = $"{keyPrefix}.{cacheKeyInput}";

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Remove(cacheKeyInput);

            mockCache.Verify(m => m.Remove(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Remove_正常系_KeyがNull()
        {
            _ = SetUpContainer();

            var keyPrefix = Guid.NewGuid().ToString();
            var cacheKeyInput = default(string);

            var cacheHelper = new AopCacheHelper(keyPrefix);
            cacheHelper.Remove(cacheKeyInput);
        }

        #endregion
    }
}
