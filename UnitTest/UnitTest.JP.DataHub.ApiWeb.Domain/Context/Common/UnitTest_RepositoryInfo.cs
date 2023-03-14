using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.Common
{
    [TestClass]
    public class UnitTest_RepositoryInfo : UnitTestBase
    {
        [TestMethod]
        public void RepositoryInfo_Blob()
        {
            string testConnectionString = "DefaultEndpointsProtocol=https;AccountName=;AccountKey=;EndpointSuffix=core.windows.net";
            var target = new RepositoryInfo("afb", new System.Collections.Generic.Dictionary<string, bool>() { { testConnectionString, false } });
            target.ConnectionString.Is(testConnectionString);
            target.Endpoint.Is("");
        }

        [TestMethod]
        public void RepositoryInfo_Queue()
        {
            string testConnectionString = "DefaultEndpointsProtocol=https;AccountName=;AccountKey=;EndpointSuffix=core.windows.net;QueueName=";
            var target = new RepositoryInfo("qus", new System.Collections.Generic.Dictionary<string, bool>() { { testConnectionString, false } });
            target.ConnectionString.Is(testConnectionString);
            target.Endpoint.Is("");
        }

        [TestMethod]
        public void RepositoryInfo_CosmosDB()
        {
            string testConnectionString = "AccountEndpoint=;AccountKey=;DatabaseId=;CollectionId=;";
            var target = new RepositoryInfo("ddb", new System.Collections.Generic.Dictionary<string, bool>() { { testConnectionString, false } });
            target.ConnectionString.Is(testConnectionString);
            target.Endpoint.Is("");
        }
        [TestMethod]
        public void RepositoryInfo_SqlServer()
        {
            string testConnectionString = "Server=tcp:,1433;Initial Catalog=;Persist Security Info=False;User ID=;Password=;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            var target = new RepositoryInfo("ssd", new System.Collections.Generic.Dictionary<string, bool>() { { testConnectionString, false } });
            target.ConnectionString.Is(testConnectionString);
            target.Endpoint.Is("tcp:,1433");
        }

        [TestMethod]
        public void RepositoryInfo_NotDefined()
        {
            //var target = new RepositoryInfo("bfs", "aaaa");
            //target.Type.GetClassName().Is("");
        }
    }
}
