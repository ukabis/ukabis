using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_QueryStringVO : UnitTestBase
    {
        [TestMethod]
        public void QueryString_Normal()
        {
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("test"), new QueryStringValue("test"));
            data.Add(new QueryStringKey("test2"), new QueryStringValue("test"));
            var target = new QueryStringVO(data);
            target.GetQueryString().Is("test=test&test2=test");
        }

        [TestMethod]
        public void QueryString_ValueEmpty()
        {
            var data = new Dictionary<QueryStringKey, QueryStringValue>();
            data.Add(new QueryStringKey("test"), new QueryStringValue("test"));
            data.Add(new QueryStringKey("test2"), new QueryStringValue(""));
            var target = new QueryStringVO(data);
            target.GetQueryString().Is("test=test&test2");
        }
    }
}
