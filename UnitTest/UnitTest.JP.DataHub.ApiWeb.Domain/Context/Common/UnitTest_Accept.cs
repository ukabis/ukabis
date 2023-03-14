using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_Accept : UnitTestBase
    {

        [TestMethod]
        public void Accept_通常_Json()
        {
            string acceptString = "*/*";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/json");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/json");
        }

        [TestMethod]
        public void Accept_通常_Xml()
        {
            string acceptString = "*/*";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/xml");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/xml");
        }

        [TestMethod]
        public void Accept_AcceptJson指定_Json()
        {
            string acceptString = "application/json";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/json");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/json");
        }

        [TestMethod]
        public void Accept_AcceptXML指定_Json()
        {
            string acceptString = "application/xml";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/json");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/xml");
        }

        [TestMethod]
        public void Accept_AcceptXML指定_xml()
        {
            string acceptString = "application/xml";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/xml");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/xml");
        }

        [TestMethod]
        public void Accept_Acceptw指定_xml()
        {
            string acceptString = "application/*";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/xml");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/xml");
        }

        [TestMethod]
        public void Accept_AcceptCsv指定_csv()
        {
            string acceptString = "text/csv";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("text/csv");
            var result = testTarget.GetResponseMediaType(contentsMediaType);
            result[0].Value.Is("text/csv");
        }

        [TestMethod]
        public void Accept_AcceptCsv指定_Json()
        {
            string acceptString = "text/csv";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/json");
            var result = testTarget.GetResponseMediaType(contentsMediaType);
            result[0].Value.Is("text/csv");
        }

        [TestMethod]
        public void Accept_優先度_xml()
        {
            string acceptString = "application/json;q=0.5,application/xml";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/txt");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/xml");
            reslt[1].Value.Is("application/json");
        }

        [TestMethod]
        public void Accept_優先度2_xml()
        {
            string acceptString = "application/json;q=0.5,application/xml;q=0.4";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/txt");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/json");
            reslt[1].Value.Is("application/xml");
        }

        [TestMethod]
        public void Accept_優先度パース失敗_xml()
        {
            string acceptString = "application/json;q=aa,application/xml;q=0.4";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType("application/txt");
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/xml");
            reslt[1].Value.Is("application/json");
        }

        [TestMethod]
        public void Accept_MediaType_ValueNull()
        {
            string acceptString = "application/json;q=aa,application/xml;q=0.4";
            Accept testTarget = new Accept(acceptString);
            MediaType contentsMediaType = new MediaType(null);
            var reslt = testTarget.GetResponseMediaType(contentsMediaType);
            reslt[0].Value.Is("application/json");
        }

        [TestMethod]
        public void Accept_MediaType_Null()
        {
            string acceptString = "application/json;q=aa,application/xml;q=0.4";
            Accept testTarget = new Accept(acceptString);
            var reslt = testTarget.GetResponseMediaType(null);
            reslt[0].Value.Is("application/json");
        }
    }
}
