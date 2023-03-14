using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Cache
{
    [TestClass]
    public class UnitTest_CacheManager : ComUnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        private (Mock<ICache> mock, CacheManager manager) CreateCacheMock()
        {
            var mockCache = new Mock<ICache>();
            if (UnityContainer.IsRegistered<ICache>() == true)
            {
                UnityContainer.Resolve<ICache>();
            }
            UnityContainer.RegisterInstance(mockCache.Object);
            var returnKeys = new List<string>() { "abc.{'test_id1':'hoge'}", "abc.{'test_id1':'abc'}", "xyz.{'test_id1':'hoge'}", "ABC.{'idx':'NS'}" };
            mockCache.Setup(x => x.Keys()).Returns(returnKeys);
            mockCache.Setup(x => x.Remove(It.IsAny<string>()));
            mockCache.Setup(x => x.RemovePatternByKeyOnly(It.IsAny<string>()));
            var target = new CacheManager("UnitTest.JP.DataHub.Core");
            return (mockCache, target);
        }

        [TestMethod]
        public void Fire_Id_1つ()
        {
            var dummy = CreateCacheMock();
            dummy.manager.FireId("test_id1", "hoge");
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(2));
            dummy.mock.Verify(x => x.Remove(It.Is<string>(x => x == "abc.{'test_id1':'hoge'}")), Times.Exactly(1));
            dummy.mock.Verify(x => x.Remove(It.Is<string>(x => x == "xyz.{'test_id1':'hoge'}")), Times.Exactly(1));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Fire_Id_1つ_other()
        {
            var dummy = CreateCacheMock();
            dummy.manager.FireId("test_id1", "abc");
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(1));
            dummy.mock.Verify(x => x.Remove(It.Is<string>(x => x == "abc.{'test_id1':'abc'}")), Times.Exactly(1));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Fire_Id_1つ_からぶり()
        {
            // FireIdしたものがCacheKeysに存在しないから、実際はRemoveされない
            var dummy = CreateCacheMock();
            dummy.manager.FireId("abcdefg", "abcdefg");
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Fire_Entity_1つ()
        {
            var dummy = CreateCacheMock();
            dummy.manager.FireEntity("test_entity1");
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(1));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.Is<string>(x => x == "Prefix_entity1")), Times.Exactly(1));
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Fire_Entity_2つ()
        {
            var dummy = CreateCacheMock();
            dummy.manager.FireEntity("test_entity2");
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(2));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.Is<string>(x => x == "Prefix_entity2-1")), Times.Exactly(1));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.Is<string>(x => x == "Prefix_entity2-2")), Times.Exactly(1));
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Fire_Entity1_and_2()
        {
            var dummy = CreateCacheMock();
            dummy.manager.FireEntity("test_entity1", "test_entity2");
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(3));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.Is<string>(x => x == "Prefix_entity1")), Times.Exactly(1));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.Is<string>(x => x == "Prefix_entity2-1")), Times.Exactly(1));
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.Is<string>(x => x == "Prefix_entity2-2")), Times.Exactly(1));
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        [TestMethod]
        public void Fire_Entity対象なし()
        {
            var dummy = CreateCacheMock();
            dummy.manager.FireEntity("test_entity_nothing");
            dummy.mock.Verify(x => x.RemoveFirstMatch(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.Remove(It.IsAny<string>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePattern(It.IsAny<List<object>>()), Times.Exactly(0));
            dummy.mock.Verify(x => x.RemovePatternByKeyOnly(It.IsAny<string>()), Times.Exactly(0));
        }

        public class Class1
        {
            public string ABC { get; set; }
            public string DEF { get; set; }
            public Class1(string abc, string def)
            {
                ABC = abc;
                DEF = def;
            }
            public override string ToString()
            {
                return $"{ABC}.{DEF}";
            }
        }

        public class Class2
        {
            public string ABC { get; set; }
            public string DEF { get; set; }
            public Class2(string abc, string def)
            {
                ABC = abc;
                DEF = def;
            }
        }
    }

    internal class TestCacheManagerCacheAttribute
    {
        [Cache("Prefix")]
        public static string TEST_ID1() { return null; }

        [CacheEntity("test_entity1")]
        [Cache("Prefix_entity1")]
        public static string TEST_ENTITY1() { return null; }

        [CacheEntity("test_entity2")]
        [Cache("Prefix_entity2-1")]
        public static string TEST_ENTITY2_1() { return null; }

        [CacheEntity("test_entity2")]
        [Cache("Prefix_entity2-2")]
        public static string TEST_ENTITY2_2() { return null; }
    }
}