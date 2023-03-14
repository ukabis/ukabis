using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Moq;
using Newtonsoft.Json;
using Unity;
using JP.DataHub.Com.HashAlgorithm;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.Data.AzureStorage;
using JP.DataHub.UnitTest.Com;
using UnitTest.JP.DataHub.ApiWeb.Infrastructure.MockClass;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_AsyncDyanamicApiRepository : UnitTestCaseBase
    {
        public TestContext TestContext { get; set; }
        const string ResultHeadJson = @"{""StatusCode"":200,""MediaType"":""application/json"",""CharSet"":""utf-8"",""HttpHeaders"":[{""_CanConvert"":[""true""]},{""X-ResponseContinuation"":[""""]}],""HttpContentHeaders"":[{""Content-Type"":[""application/json; charset=utf-8""]},{""Content-Length"":[""569""]}]}";

        private string _requestId = Guid.NewGuid().ToString();


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        [TestCase("End")]
        [TestCase("Error")]
        public void GetResult_正常系_フォルダ()
        {
            TestContext.Run((string status) =>
            {
                var result = new AsyncResultJsonModel()
                {
                    RequestId = _requestId,
                    Status = status,
                    ResultPath = "fuga"
                };
                var response = new DynamicApiReponseContents(JsonConvert.SerializeObject(result), null, null, null, null);

                var mockInterface = new Mock<IDynamicApiInterface>();
                mockInterface.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                    .Returns(new DynamicApiResponse(HttpStatusCode.OK, response));
                UnityContainer.RegisterInstance(mockInterface.Object);
                var perRequestDataContainer = new PerRequestDataContainer();
                UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

                var bodyCont = "fuga";
                var msBody = new MemoryStream(Encoding.UTF8.GetBytes(bodyCont));
                var msHead = new MemoryStream(Encoding.UTF8.GetBytes(ResultHeadJson));
                var contentType = "application/json";

                var mockBlob = new Mock<IBlobStorage>();
                mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Header.bin"))))
                    //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                    .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), msHead, contentType));

                mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Body.bin"))))
                    //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                    .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), msBody, contentType));
                UnityContainer.RegisterInstance<IBlobStorage>("AsyncDynamicApiBlobStorage", mockBlob.Object);

                var target = UnityContainer.Resolve<IAsyncDyanamicApiRepository>();
                var res = target.GetResult(new AsyncRequestId(_requestId));

                res.StatusCode.Is(System.Net.HttpStatusCode.OK);
                res.Content.Headers.ContentType.MediaType.Is("application/json");
                res.Content.Headers.ContentType.CharSet.Is("utf-8");
                res.Content.Headers.ContentLength.Is(bodyCont.Length);
                bodyCont.Is(res.Content.ReadAsStringAsync().Result);
            });
        }

        [TestMethod]
        [TestCase("End")]
        [TestCase("Error")]
        public void GetResult_正常系_ページング()
        {
            TestContext.Run((string status) =>
            {
                var result = new AsyncResultJsonModel()
                {
                    RequestId = _requestId,
                    Status = status,
                    ResultPath = "fuga"
                };
                var response = new DynamicApiReponseContents(JsonConvert.SerializeObject(result), null, null, null, null);

                var mockInterface = new Mock<IDynamicApiInterface>();
                mockInterface.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                    .Returns(new DynamicApiResponse(HttpStatusCode.OK, response));
                UnityContainer.RegisterInstance(mockInterface.Object);
                var perRequestDataContainer = new PerRequestDataContainer();
                perRequestDataContainer.XRequestContinuation = "hoge";
                UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

                var bodyCont = "fuga";
                var msBody = new MemoryStream(Encoding.UTF8.GetBytes(bodyCont));
                var msHead = new MemoryStream(Encoding.UTF8.GetBytes(ResultHeadJson));
                var contentType = "application/json";
                var requestContinuationPath = HashCalculation.ComputeHashString(Encoding.Unicode.GetBytes("hoge"));

                var mockBlob = new Mock<IBlobStorage>();
                mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Header.bin") && i.Contains(requestContinuationPath))))
                    //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                    .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), msHead, contentType));

                mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Body.bin") && i.Contains(requestContinuationPath))))
                    //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                    .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), msBody, contentType));
                UnityContainer.RegisterInstance<IBlobStorage>("AsyncDynamicApiBlobStorage", mockBlob.Object);

                var target = UnityContainer.Resolve<IAsyncDyanamicApiRepository>();
                var res = target.GetResult(new AsyncRequestId(_requestId));

                res.StatusCode.Is(System.Net.HttpStatusCode.OK);
                res.Content.Headers.ContentType.MediaType.Is("application/json");
                res.Content.Headers.ContentType.CharSet.Is("utf-8");
                res.Content.Headers.ContentLength.Is(bodyCont.Length);
                bodyCont.Is(res.Content.ReadAsStringAsync().Result);
            });
        }

        [TestMethod]
        [ExpectedException(typeof(AsyncApiResultNotFoundException))]
        public void GetResult_異常系_フォルダ_Head無し()
        {
            var result = new AsyncResultJsonModel()
            {
                RequestId = _requestId,
                Status = "End",
                ResultPath = $"{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month}/{DateTime.UtcNow.Day}/Header.bin"
            };
            var response = new DynamicApiReponseContents(JsonConvert.SerializeObject(result), null, null, null, null);

            var mockInterface = new Mock<IDynamicApiInterface>();
            mockInterface.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK, response));
            UnityContainer.RegisterInstance(mockInterface.Object);
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var bodyCont = "fuga";
            var msBody = new MemoryStream(Encoding.UTF8.GetBytes(bodyCont));
            var msHead = new MemoryStream(Encoding.UTF8.GetBytes(ResultHeadJson));
            var contentType = "application/json";

            var requestResult = (RequestResult)typeof(RequestResult).GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { });
            requestResult.GetType().GetProperty("HttpStatusCode").SetValue(requestResult, 404);
            var exception = new StorageException(requestResult, "", new Exception());

            var mockBlob = new Mock<IBlobStorage>();
            mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Header.bin"))))
                .Throws(new AggregateException(exception));
            mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Body.bin"))))
                //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), msBody, contentType));

            UnityContainer.RegisterInstance<IBlobStorage>("AsyncDynamicApiBlobStorage", mockBlob.Object);

            var target = UnityContainer.Resolve<IAsyncDyanamicApiRepository>();
            var res = target.GetResult(new AsyncRequestId(_requestId));

            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(AsyncApiResultNotFoundException))]
        public void GetResult_異常系_フォルダ_Body無し()
        {
            var result = new AsyncResultJsonModel() 
            {
                RequestId = _requestId,
                Status = "End",
                ResultPath = $"{DateTime.UtcNow.Year}/{DateTime.UtcNow.Month}/{DateTime.UtcNow.Day}/fuga.json"
            };
            var response = new DynamicApiReponseContents(JsonConvert.SerializeObject(result), null, null, null, null);

            var mockInterface = new Mock<IDynamicApiInterface>();
            mockInterface.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK, response));
            UnityContainer.RegisterInstance(mockInterface.Object);
            var perRequestDataContainer = new PerRequestDataContainer();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var bodyCont = "fuga";
            var msBody = new MemoryStream(Encoding.UTF8.GetBytes(bodyCont));
            var msHead = new MemoryStream(Encoding.UTF8.GetBytes(ResultHeadJson));
            var contentType = "application/json";

            var requestResult = (RequestResult)typeof(RequestResult).GetConstructors(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(new object[] { });
            requestResult.GetType().GetProperty("HttpStatusCode").SetValue(requestResult, 404);
            var exception = new StorageException(requestResult, "", new Exception());

            var mockBlob = new Mock<IBlobStorage>();
            mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Header.bin"))))
                //このURIはblobオブジェクトを作成するために接続しているもの 実際にblobのオブジェクトを操作しているわけではない
                .ReturnsAsync(new MockCloudAppendBlob(new Uri("https://jpdatahubdev2.blob.core.windows.net/jpdatahubscriptruntimelog"), msHead, contentType));
            mockBlob.Setup(x => x.GetAppendBlobAsync(It.IsAny<string>(), It.Is<string>((i) => i.Contains("Body.bin"))))
                .Throws(new AggregateException(exception));

            UnityContainer.RegisterInstance<IBlobStorage>("AsyncDynamicApiBlobStorage", mockBlob.Object);

            var target = UnityContainer.Resolve<IAsyncDyanamicApiRepository>();
            var res = target.GetResult(new AsyncRequestId(_requestId));

            Assert.Fail();
        }
    }
}
