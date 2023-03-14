using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EasyCaching.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Cache
{
    [TestClass]
    public class UnitTest_AbstractCache : ComUnitTestBase
    {
        private string providerName = "targetprovider";
        private string providerDefaultReturn = "hogehoge";
        private string key01 = "Key.aaa.bbb.ccc";
        private string key02 = "Key.aaa.bbb.ddd";
        private string key03 = "Key.eee.fff.ggg";
        private string key04 = "Key.hhh.aaa.iii";

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
        }

        private UnityContainer SetUpContainer(Mock<IEasyCachingProvider> mock)
        {
            var container = new UnityContainer();
            container.RegisterInstance<IEasyCachingProvider>(providerName, mock.Object);
            container.RegisterInstance<IConfiguration>(CreateConfigurationMock().Object);
            UnityCore.UnityContainer = container;
            return container;
        }

        private Mock<IEasyCachingProvider> CreateProviderMock()
        {
            AbstractCache.DeInit();

            var mock = new Mock<IEasyCachingProvider>();
            mock.Setup(x => x.Name).Returns(providerName);
            mock.Setup(x => x.Set(It.IsAny<String>(), It.IsAny<object>(), It.IsAny<TimeSpan>()));
            mock.Setup(x => x.Get<object>(It.IsAny<String>())).Returns(new CacheValue<object>(providerDefaultReturn, true));
            mock.Setup(x => x.RemoveByPrefix(It.IsAny<String>()));
            mock.Setup(x => x.Remove(It.IsAny<String>()));
            return mock;
        }

        # region Mocking IConfiguration
        class IConfigurationProxy : IConfiguration, IConfigurationSection
        {
            public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public string Key => throw new NotImplementedException();
            public string Path => throw new NotImplementedException();
            public string Value { get; set; }
            public IEnumerable<IConfigurationSection> GetChildren() => throw new NotImplementedException();
            public IChangeToken GetReloadToken() => throw new NotImplementedException();
            public IConfigurationSection GetSection(string key) => this;
            public bool GetValue<T>(string key) => false; //　UTでProfilerは考慮しない
        }
        private Mock<IConfiguration> CreateConfigurationMock()
        {
            var mockConfig = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            var mockValue = new Mock<IConfigurationSection>();
            mockValue.SetupGet(x => x.Key).Returns("UseProfiler");
            mockValue.SetupGet(x => x.Value).Returns("false");
            mockSection.Setup(x => x.GetSection(It.IsAny<string>())).Returns(mockValue.Object);
            mockConfig.Setup(x => x.GetSection(It.IsAny<string>())).Returns(mockSection.Object);
            return mockConfig;
        }
        #endregion

        [TestMethod]
        public void Contains()
        {
            string key1 = "aa";
            string key2 = "bb";
            var mock = CreateProviderMock();
            mock.Setup(x => x.Exists(It.Is<String>(y => y.Equals(key1)))).Returns(true);
            mock.Setup(x => x.Exists(It.Is<String>(y => y.Equals(key2)))).Returns(false);
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);

            target.Contains(key1).Is(true);
            target.Contains(key2).Is(false);
        }

        [TestMethod]
        public void Clear()
        {
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.Clear();
            mock.Verify(x => x.Flush(), Times.Once);
        }
        [TestMethod]
        public void Add()
        {
            int secExpiration = 3600;
            var ts = new TimeSpan(0, 0, secExpiration);
            string key = "aa";
            string obj = "fugafuga";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.Add(key, obj);
            mock.Verify(x => x.Set(It.Is<string>(y => y.Equals(key)), It.Is<object>(y => y.Equals(obj)), It.Is<TimeSpan>(y => y.Equals(ts))), Times.Once);
        }

        [TestMethod]
        public void Add_Timespan()
        {
            int secExpiration = 30;
            var ts = new TimeSpan(0, 0, secExpiration);
            string key = "aa";
            string obj = "fugafuga";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.Add(key, obj, secExpiration);
            mock.Verify(x => x.Set(It.Is<string>(y => y.Equals(key)), It.Is<object>(y => y.Equals(obj)), It.Is<TimeSpan>(y => y.Equals(ts))), Times.Once);
        }

        [TestMethod]
        public void Add_Timespan_2()
        {
            int hourExpiration = 1;
            int minuteExpiration = 2;
            int secExpiration = 30;
            var ts = new TimeSpan(hourExpiration, minuteExpiration, secExpiration);
            string key = "aa";
            string obj = "fugafuga";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.Add(key, obj, hourExpiration, minuteExpiration, secExpiration);
            mock.Verify(x => x.Set(It.Is<string>(y => y.Equals(key)), It.Is<object>(y => y.Equals(obj)), It.Is<TimeSpan>(y => y.Equals(ts))), Times.Once);
        }
        [TestMethod]
        public void Add_Timespan_3()
        {
            int secExpiration = 30;
            var ts = new TimeSpan(0, 0, secExpiration);
            string key = "aa";
            string obj = "fugafuga";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.Add(key, obj, ts);
            mock.Verify(x => x.Set(It.Is<string>(y => y.Equals(key)), It.Is<object>(y => y.Equals(obj)), It.Is<TimeSpan>(y => y.Equals(ts))), Times.Once);
        }

        [TestMethod]
        public void Get()
        {
            string key = "aa";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            var result = target.Get<string>(key, out var isNullValue);
            result.Is(providerDefaultReturn);
            mock.Verify(x => x.Get<object>(It.Is<string>(y => y.Equals(key))), Times.Once);
        }
        [TestMethod]
        public void Get_misshit_isflash()
        {
            ActionObject ao = () =>
            {
                return "fuga";
            };
            string key = "aa";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.IsFlash = true;
            var result = target.Get<string>(key, ao);
            result.Is("fuga");
            mock.Verify(x => x.Get<object>(It.IsAny<string>()), Times.Never);
        }
        [TestMethod]
        public void Get_hit()
        {
            ActionObject ao = () =>
            {
                return "fuga";
            };
            string key = "aa";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            var result = target.Get<string>(key, ao);
            result.Is(providerDefaultReturn);
            mock.Verify(x => x.Get<object>(It.Is<string>(y => y.Equals(key))), Times.Once);
        }

        [TestMethod]
        public void Remove()
        {
            string key = "aa";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.Remove(key);
            mock.Verify(x => x.Remove(It.Is<string>(y => y.Equals(key))), Times.Once);
        }

        [TestMethod]
        public void RemoveFirstMatch()
        {
            string key = "aa";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.RemoveFirstMatch(key);
            mock.Verify(x => x.RemoveByPrefix(It.Is<string>(y => y.Equals(key))), Times.Once);
        }

        [TestMethod]
        public void RemovePatternByKeyOnly()
        {
            string key = "aa";
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName);
            target.RemovePatternByKeyOnly(key);
            mock.Verify(x => x.RemoveByPrefix(It.Is<string>(y => y.Equals(key + "."))), Times.Once);
        }

        [TestMethod]
        public void RemovePattern()
        {
            var key = new object[] { "Key", "aaa", "bbb", "ccc" };
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName, new string[] { key01, key02, key03, key04 });
            target.RemovePattern(key.ToList());
            mock.Verify(x => x.Remove(It.Is<string>(y => y.Equals(key01))), Times.Once);
            mock.Verify(x => x.Remove(It.Is<string>(y => y.Equals(key02))), Times.Once);
        }
        [TestMethod]
        public void RemovePattern_2()
        {
            var key = new object[] { "Key", "aaa", "bbb", "ggg" };
            var mock = CreateProviderMock();
            var container = SetUpContainer(mock);
            var target = GetTestClass(container, providerName, new string[] { key01, key02, key03, key04 });
            target.RemovePattern(key.ToList());
            mock.Verify(x => x.Remove(It.Is<string>(y => y.Equals(key03))), Times.Once);
        }

        private AbstractCache GetTestClass(IUnityContainer container, string name)
        {
            return new TestAbstractCache(name);
        }

        private AbstractCache GetTestClass(IUnityContainer container, string name, IEnumerable<string> keys)
        {
            return new TestAbstractCache(name, keys);
        }
    }

    internal class TestAbstractCache : AbstractCache
    {
        private List<string> keys;

        public TestAbstractCache(string name) : base(name)
        {
        }

        public TestAbstractCache(string name, IEnumerable<string> keys) : base(name)
        {
            this.keys = keys.ToList();
        }

        public override IEnumerable<string> Keys()
        {
            return keys;
        }

    }
}
