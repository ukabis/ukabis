using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_AsyncApiResult : UnitTestBase
    {
        public TestContext TestContext { get; set; }
        public static object[] asyncApiResult = new[]
        {
            new object[] {AsyncStatus.Request,null,null,""},
            new object[] {AsyncStatus.Start,DateTime.UtcNow,null,""},
            new object[] {AsyncStatus.End,DateTime.UtcNow, DateTime.UtcNow, "/aaaa/aaaa"},
            new object[] {AsyncStatus.Error,DateTime.UtcNow, DateTime.UtcNow, ""},
        };

        [TestMethod]
        [TestCaseSource("asyncApiResult")]
        public void AsyncApiResult_NomalCase()
        {
            TestContext.Run((AsyncStatus status, DateTime? request, DateTime? end, string path) =>
            {
                var requestId = Guid.NewGuid().ToString();
                var target = new AsyncApiResult(requestId, status.ToString(), request, end, path);
                target.RequestId.Is(requestId);
                target.Status.Is(status);
                target.RequestDate.Is(request);
                target.EndDate.Is(end);
                target.ResultPath.Is(path);
            });
        }

        [TestMethod]
        public void AsyncApiResult_ErrorCase()
        {
            var target = AssertEx.Catch<FormatException>(() =>
            {
                new AsyncApiResult(Guid.NewGuid().ToString(), "aaaa", null, null, "");
            });
            target.Message.Is("Status is not Valid");
        }
    }
}
