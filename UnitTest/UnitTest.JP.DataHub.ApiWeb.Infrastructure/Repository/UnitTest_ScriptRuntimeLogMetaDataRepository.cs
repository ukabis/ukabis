using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Api.Core.Exceptions;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.Context.ScriptRuntimeLog;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Infrastructure.Repository
{
    [TestClass]
    public class UnitTest_ScriptRuntimeLogMetaDataRepository : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        public void ScriptRuntimeLogMetaDataRepository_正常系_HTTPStatusCode_Created()
        {
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, Guid.NewGuid().ToString());
            perRequestDataContainer.SetupProperty(x => x.VendorId, Guid.NewGuid().ToString());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);

            var postReturnValue = new HttpResponseMessage(HttpStatusCode.Created);
            postReturnValue.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("hoge")));
            postReturnValue.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            postReturnValue.Content.Headers.ContentType.CharSet = Encoding.UTF8.WebName;
            var mock = new Mock<IDynamicApiInterface>();
            var expected = new DynamicApiResponse(postReturnValue);
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), true)).Returns(expected);
            UnityContainer.RegisterInstance(mock.Object);

            var repository = UnityContainer.Resolve<IScriptRuntimeLogMetaDataRepository>();
            var res = repository.Create(new ScriptRuntimeLogMetaData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddHours(9), 100, false, DateTime.UtcNow.AddHours(9), Guid.NewGuid()));
            res.IsStructuralEqual(expected.ToHttpResponseMessage());
        }

        [TestMethod]
        [ExpectedException(typeof(ApiException))]
        public void ScriptRuntimeLogMetaDataRepository_異常系_HTTPStatusCode_NotImplemented()
        {
            var perRequestDataContainer = new Mock<IPerRequestDataContainer>();
            perRequestDataContainer.SetupProperty(x => x.SystemId, Guid.NewGuid().ToString());
            perRequestDataContainer.SetupProperty(x => x.VendorId, Guid.NewGuid().ToString());
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer.Object);

            var postReturnValue = new HttpResponseMessage(HttpStatusCode.NotImplemented);
            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), true)).Returns(new DynamicApiResponse(postReturnValue));
            UnityContainer.RegisterInstance(mock.Object);

            var repository = UnityContainer.Resolve<IScriptRuntimeLogMetaDataRepository>();
            var res = repository.Create(new ScriptRuntimeLogMetaData(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow.AddHours(9), 100, false, DateTime.UtcNow.AddHours(9), Guid.NewGuid()));
        }
    }
}
