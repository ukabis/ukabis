using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using Unity.Interception.PolicyInjection.Pipeline;
using Unity.Lifetime;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.Cache.Attributes;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.TimeZone;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.Com.Cache
{
    [TestClass]
    public class UnitTest_CacheHelper : ComUnitTestBase
    {
        private const string CachePrefix = "TestPrefix";

        private class SampleDataContainer : IDataContainer
        {
            public string Id { get; set; }
            public string VendorId { get; set; }
            public string SystemId { get; set; }
            public bool XgetInternalAllField { get; set; }
            public string OpenId { get; set; }
            public bool VendorSystemAuthenticated { get; set; }
            public string Xadmin { get; set; }
            public int Xversion { get; set; }
            public string XRequestContinuation { get; set; }
            public bool XNotAuthenticationRequest { get; set; }
            public string InternalCallKeyword { get; set; }
            public bool XVendorSystemAuthenticated { get; }
            public Dictionary<string, string> Claims { get; set; }
            public string AuthorizationError { get; set; }
            public bool IsDeveloper { get; set; }
            public string ClientIpAddress { get; set; }
            public string OriginalAccessToken { get; set; }
            public CultureInfo CultureInfo { get; set; }
            public bool IsInternalCall { get; set; }
            public string AccessBeyondVendorKey { get; }
            public string XResourceSharingPerson { get; set; }
            public Dictionary<string, string> XResourceSharingWith { get; set; }
            public Dictionary<string, List<string>> RequestHeaders { get; set; }
            public bool ProfilerDisabled { get; set; }
            public string ControllerName { get; set; }
            public string ActionName { get; set; }
            public object Argument { get; set; }

            public DateTimeUtil GetDateTimeUtil() => new DateTimeUtil("yyyy/MM/dd",
            new string[] { "yyyy/MM/dd hh:mm:ss tt", "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd h:mm:ss" }, "yyyy/M/d");
        }

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(req => req.HttpContext.Request.Headers.ContainsKey("abc")).Returns(true);
            mockHttpContextAccessor.Setup(req => req.HttpContext.Request.Headers["abc"]).Returns(new string[] { "ABC" });
            mockHttpContextAccessor.Setup(req => req.HttpContext.Request.Headers.ContainsKey("ContentType")).Returns(true);
            mockHttpContextAccessor.Setup(req => req.HttpContext.Request.Headers["ContentType"]).Returns(new string[] { "hogehoge" });
            UnityContainer.RegisterInstance<IHttpContextAccessor>(mockHttpContextAccessor.Object);

            var mockDataContainer = new Mock<IDataContainer>();
            UnityContainer.RegisterType<IDataContainer, SampleDataContainer>(new SingletonLifetimeManager());
            var x = UnityContainer.Resolve<IDataContainer>() as SampleDataContainer;
            x.Id = new Guid("{5E9460E9-C4F9-42BA-B16C-54FF9B52CCF3}").ToString();
        }

        [TestMethod]
        public void Parameter_String1()
        {
            var input = new Dictionary<string, string>() { { "test", "1" } };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input);
            cacheKey.Is($"{CachePrefix}.{{'test':'1'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("1");
            result.Header.Count().Is(0);
        }

        [TestMethod]
        public void Parameter_StringDot()
        {
            var input = new Dictionary<string, string>() { { "test", "123.xyz" } };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input);
            cacheKey.Is($"{CachePrefix}.{{'test':'123[dot]xyz'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("123[dot]xyz");
            result.Header.Count().Is(0);
        }

        [TestMethod]
        public void Parameter_StringDot2()
        {
            var input = new Dictionary<string, string>() { { "test", "{123{'a'.'x'}xyz}" } };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input);
            cacheKey.Is($"{CachePrefix}.{{'test':'{{123{{[s-quote]a[s-quote][dot][s-quote]x[s-quote]}}xyz}}'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("{123{[s-quote]a[s-quote][dot][s-quote]x[s-quote]}xyz}");
            result.Header.Count().Is(0);
        }

        [TestMethod]
        public void Parameter_String2()
        {
            var input = new Dictionary<string, string>() { { "test", "1" }, { "hoge", "abc" } };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input);
            cacheKey.Is($"{CachePrefix}.{{'test':'1'}}.{{'hoge':'abc'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(2);
            result.Param["test"].Is("1");
            result.Param["hoge"].Is("abc");
            result.Header.Count().Is(0);
        }

        [TestMethod]
        public void Parameter_null()
        {
            var input = new Dictionary<string, string>() { { "test", null } };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input);
            cacheKey.Is($"{CachePrefix}.{{'test':'[null]'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("[null]");
            result.Header.Count().Is(0);
        }

        [TestMethod]
        public void Parameter_Header1()
        {
            var input = new Dictionary<string, string>() { { "test", "1" } };
            var header = new string[] { "abc" };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input, header);
            cacheKey.Is($"{CachePrefix}.{{'test':'1'}}.{{'Header-abc':'ABC'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("1");
            result.Header.Count().Is(1);
            result.Header["abc"].Is("ABC");
        }

        [TestMethod]
        public void Parameter_Header2()
        {
            var input = new Dictionary<string, string>() { { "test", "1" } };
            var header = new string[] { "abc", "ContentType" };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input, header);
            cacheKey.Is($"{CachePrefix}.{{'test':'1'}}.{{'Header-abc':'ABC'}}.{{'Header-ContentType':'hogehoge'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("1");
            result.Header.Count().Is(2);
            result.Header["abc"].Is("ABC");
            result.Header["ContentType"].Is("hogehoge");
        }

        [TestMethod]
        public void Parameter_HeaderNull()
        {
            var input = new Dictionary<string, string>() { { "test", "1" } };
            var header = new string[] { "abc", "ContentType", "123" };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input, header);
            cacheKey.Is($"{CachePrefix}.{{'test':'1'}}.{{'Header-abc':'ABC'}}.{{'Header-ContentType':'hogehoge'}}.{{'Header-123':'[null]'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("1");
            result.Header.Count().Is(3);
            result.Header["abc"].Is("ABC");
            result.Header["ContentType"].Is("hogehoge");
            result.Header["123"].Is("[null]");
        }

        [TestMethod]
        public void Parameter_DataContainer()
        {
            var input = new Dictionary<string, string>() { { "test", "1" } };
            var dc = new Dictionary<string, string>() { { "abc", "Id" } };
            var cacheKey = CacheHelper.CreateCacheKey(CachePrefix, input, null, dc);
            cacheKey.Is($"{CachePrefix}.{{'test':'1'}}.{{'DC-abc':'5e9460e9-c4f9-42ba-b16c-54ff9b52ccf3'}}");

            var result = CacheHelper.ParseCacheKeyParam(cacheKey);
            result.Prefix.Is(CachePrefix);
            result.Param.Count().Is(1);
            result.Param["test"].Is("1");
            result.DataContainer.Count().Is(1);
            result.DataContainer["abc"].Is("5e9460e9-c4f9-42ba-b16c-54ff9b52ccf3");
        }

        [TestMethod]
        public void MethodArgumentToValue_Shallow()
        {
            var type = GetType();
            var methods = type.GetMethods();
            MethodInfo dummy = methods.Where(x => x.Name == "Dummy_Shallow").FirstOrDefault();
            var parameters = dummy.GetParameters();
            var param = new ParameterCollection(new object[] { "abc" }, parameters, (param) => true);
            var result = param.MethodArgumentToValue("x");
            result.Is("abc");
        }

        public class Sample
        {
            public SampleLevel2 l2 { get; set; } = new SampleLevel2();
        }
        public class SampleLevel2
        {
            public string Value { get; set; } = "xyz";
        }

        public void Dummy_Deep(Sample x)
        {
        }

        public void Dummy_Shallow(string x)
        {
        }

        [TestMethod]
        public void MethodArgumentToValue_Deep()
        {
            var type = GetType();
            var methods = type.GetMethods();
            MethodInfo dummy = methods.Where(x => x.Name == "Dummy_Deep").FirstOrDefault();
            var parameters = dummy.GetParameters();
            var param = new ParameterCollection(new object[] { new Sample() }, parameters, (param) => true);
            var result = param.MethodArgumentToValue("x.l2.Value");
            result.Is("xyz");

            result = param.MethodArgumentToParamValue("x.l2.A");
            result.IsNull();
        }

        [TestMethod]
        public void MethodArgumentToValue_DeepNull()
        {
            var type = GetType();
            var methods = type.GetMethods();
            MethodInfo dummy = methods.Where(x => x.Name == "Dummy_Deep").FirstOrDefault();
            var parameters = dummy.GetParameters();
            var param = new ParameterCollection(new object[] { new Sample() }, parameters, (param) => true);
            var result = param.MethodArgumentToValue("x.l2.A");
            result.IsNull();
        }

        [TestMethod]
        public void MethodArgumentToParamValue_Deep()
        {
            var type = GetType();
            var methods = type.GetMethods();
            MethodInfo dummy = methods.Where(x => x.Name == "Dummy_Deep").FirstOrDefault();
            var parameters = dummy.GetParameters();
            var param = new ParameterCollection(new object[] { new Sample() }, parameters, (param) => true);
            var result = param.MethodArgumentToParamValue("x.l2.Value");
            result.Is("xyz");
        }

        [TestMethod]
        public void MethodArgumentToParamValue_DeepNull()
        {
            var type = GetType();
            var methods = type.GetMethods();
            MethodInfo dummy = methods.Where(x => x.Name == "Dummy_Deep").FirstOrDefault();
            var parameters = dummy.GetParameters();
            var param = new ParameterCollection(new object[] { new Sample() }, parameters, (param) => true);
            var result = param.MethodArgumentToParamValue("x.l2.A");
            result.IsNull();
        }
    }
}
