using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Scripting.Roslyn
{
    [TestClass]
    public class UnitTest_CSharpScriptingExecuter : UnitTestBase
    {
        private Uri _uri = new Uri("https://google.com");


        [TestInitialize()]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IPerRequestDataContainer, PerRequestDataContainer>("multiThread");
        }

        [TestMethod]
        public void CSharpScriptingExecuter_ExecuteScript_正常系()
        {
            var testClass = UnityContainer.Resolve<IScriptingExecuter>("rss");
            var contents = "";
            var queryParams = new Dictionary<string, string>();
            var keyValues = new Dictionary<string, string>();
            var vendorId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var args = new ScriptArgumentParameters
            {
                SystemId = systemId.ToString(),
                VendorId = vendorId.ToString(),
                Contents = contents,
                QueryString = queryParams,
                KeyValue = keyValues,
                ScriptHelper = new ScriptHelper(vendorId)
            };
            var script = @"
using System;
using System.Net.Http;
return HttpResponseHelper.Create(""test"", HttpStatusCode.Created, ""text/plain"");
";
            var isEnableScriptRuntimeException = true;
            var res = testClass.ExecuteScript<HttpResponseMessage>(args, script, isEnableScriptRuntimeException);
            res.StatusCode.Is(System.Net.HttpStatusCode.Created);
            res.Content.Headers.ContentType.ToString().Is("text/plain; charset=utf-8");
            res.Content.ReadAsStringAsync().Result.Is("test");
        }

        [TestMethod]
        public void CSharpScriptingExecuter_ExecuteScript_正常系_usingあり_例外取得()
        {
            var testClass = UnityContainer.Resolve<IScriptingExecuter>("rss");
            var contents = "";
            var queryParams = new Dictionary<string, string>();
            var keyValues = new Dictionary<string, string>();
            var vendorId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var args = new ScriptArgumentParameters
            {
                SystemId = systemId.ToString(),
                VendorId = vendorId.ToString(),
                Contents = contents,
                QueryString = queryParams,
                KeyValue = keyValues,
                ScriptHelper = new ScriptHelper(vendorId)
            };
            var script = @"
using System;
using System.Net.Http;
throw new System.Exception(""test"");
return HttpResponseHelper.Create(""test"", HttpStatusCode.Created, ""text/plain"");
";
            var isEnableScriptRuntimeException = true;
            try
            {
                var res = testClass.ExecuteScript<HttpResponseMessage>(args, script, isEnableScriptRuntimeException);
                Assert.Fail();
            }
            catch (AggregateException e) when (e.InnerException is RoslynScriptRuntimeException)
            {
                e.InnerException.InnerException.GetType().Is(typeof(Exception));
                e.InnerException.InnerException.Message.Is("test");
            }
        }

        [TestMethod]
        public void CSharpScriptingExecuter_ExecuteScript_正常系_usingあり_例外取得しない()
        {
            var testClass = UnityContainer.Resolve<IScriptingExecuter>("rss");
            var contents = "";
            var queryParams = new Dictionary<string, string>();
            var keyValues = new Dictionary<string, string>();
            var vendorId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var args = new ScriptArgumentParameters
            {
                SystemId = systemId.ToString(),
                VendorId = vendorId.ToString(),
                Contents = contents,
                QueryString = queryParams,
                KeyValue = keyValues,
                ScriptHelper = new ScriptHelper(vendorId)
            };
            var script = @"
using System;
using System.Net.Http;
throw new System.Exception(""test"");
return HttpResponseHelper.Create(""test"", HttpStatusCode.Created, ""text/plain"");
";
            var isEnableScriptRuntimeException = false;
            try
            {
                var res = testClass.ExecuteScript<HttpResponseMessage>(args, script, isEnableScriptRuntimeException);
                Assert.Fail();
            }
            catch (AggregateException e) when (e.InnerException is System.Exception)
            {
                e.InnerException.GetType().Is(typeof(Exception));
                e.InnerException.Message.Is("test");
            }
        }

        [TestMethod]
        public void CSharpScriptingExecuter_ExecuteScript_正常系_usingなし_例外取得()
        {
            var testClass = UnityContainer.Resolve<IScriptingExecuter>("rss");
            var contents = "";
            var queryParams = new Dictionary<string, string>();
            var keyValues = new Dictionary<string, string>();
            var vendorId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var args = new ScriptArgumentParameters
            {
                SystemId = systemId.ToString(),
                VendorId = vendorId.ToString(),
                Contents = contents,
                QueryString = queryParams,
                KeyValue = keyValues,
                ScriptHelper = new ScriptHelper(vendorId)
            };
            var script = @"
throw new System.Exception(""test"");
";
            var isEnableScriptRuntimeException = true;
            try
            {
                var res = testClass.ExecuteScript<HttpResponseMessage>(args, script, isEnableScriptRuntimeException);
                Assert.Fail();
            }
            catch (AggregateException e) when (e.InnerException is RoslynScriptRuntimeException)
            {
                e.InnerException.InnerException.GetType().Is(typeof(Exception));
                e.InnerException.InnerException.Message.Is("test");
            }
        }

        [TestMethod]
        public void CSharpScriptingExecuter_ExecuteScript_正常系_usingなし_例外取得しない()
        {
            var testClass = UnityContainer.Resolve<IScriptingExecuter>("rss");
            var contents = "";
            var queryParams = new Dictionary<string, string>();
            var keyValues = new Dictionary<string, string>();
            var vendorId = Guid.NewGuid();
            var systemId = Guid.NewGuid();
            var args = new ScriptArgumentParameters
            {
                SystemId = systemId.ToString(),
                VendorId = vendorId.ToString(),
                Contents = contents,
                QueryString = queryParams,
                KeyValue = keyValues,
                ScriptHelper = new ScriptHelper(vendorId)
            };
            var script = @"
throw new System.Exception(""test"");
";
            var isEnableScriptRuntimeException = false;
            try
            {
                var res = testClass.ExecuteScript<HttpResponseMessage>(args, script, isEnableScriptRuntimeException);
                Assert.Fail();
            }
            catch (AggregateException e) when (e.InnerException is Exception)
            {
                e.InnerException.GetType().Is(typeof(Exception));
                e.InnerException.Message.Is("test");
            }
        }

    }
}
