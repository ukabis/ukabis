using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.RFC7807;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.ResourceSchemaAdapter;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_AdaptResourceSchemaAction : UnitTestBase
    {
        private string _errorTitle = Guid.NewGuid().ToString();
        private string _errorDetail = Guid.NewGuid().ToString();


        [TestInitialize]
        public void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestMethod]
        public void ExecuteAction_AllAdaptable()
        {
            (var action, var mockOk, var mockNg) = Setup(2, -1);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            string.IsNullOrEmpty(result.Content.ReadAsStringAsync().Result).IsTrue();

            Verify(mockOk, 2, 2, mockNg, 0, 0);
        }

        [TestMethod]
        public void ExecuteAction_WithNotAdaptable()
        {
            (var action, var mockOk, var mockNg) = Setup(3, 1);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var content = JToken.Parse(result.Content.ReadAsStringAsync().Result);
            content["title"].Value<string>().Is(_errorTitle);
            content["detail"].Value<string>().Is(_errorDetail);

            Verify(mockOk, 1, 0, mockNg, 1, 0);
        }

        [TestMethod]
        public void ExecuteAction_NoRepository()
        {
            (var action, var mockOk, var mockNg) = Setup(0);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            string.IsNullOrEmpty(result.Content.ReadAsStringAsync().Result).IsTrue();

            Verify(mockOk, 0, 0, mockNg, 0, 0);
        }

        private (AdaptResourceSchemaAction, Mock<IResourceSchemaAdapter>, Mock<IResourceSchemaAdapter>) Setup(int repositoryCount, int ngIndex = -1)
        {
            var repositoryInfoList = new List<RepositoryInfo>();
            for (var i = 0; i < repositoryCount; i++)
            {
                repositoryInfoList.Add(new RepositoryInfo(Guid.NewGuid(), i == ngIndex ? "ss2" : "ddb", new Tuple<string, bool, Guid?>(Guid.NewGuid().ToString(), false, Guid.NewGuid())));
            }

            // 重複レコード
            if (repositoryInfoList.Count > 0)
            {
                repositoryInfoList.Add(repositoryInfoList[0]);
            }

            RFC7807ProblemDetailExtendErrors mockOut = null;
            var mockAdapterOk = new Mock<IResourceSchemaAdapter>();
            mockAdapterOk.Setup(x => x.IsAdaptable(out mockOut)).Returns(true);
            mockAdapterOk.Setup(x => x.Adapt());

            var mockAdapterNg = new Mock<IResourceSchemaAdapter>();
            mockAdapterNg.Setup(x => x.IsAdaptable(out mockOut))
                .Callback((out RFC7807ProblemDetailExtendErrors rfc7807) =>
                {
                    rfc7807 = new RFC7807ProblemDetailExtendErrors();
                    rfc7807.Title = _errorTitle;
                    rfc7807.Detail = _errorDetail;
                })
                .Returns(false);
            mockAdapterNg.Setup(x => x.Adapt());

            UnityContainer.RegisterInstance<IResourceSchemaAdapter>(mockAdapterOk.Object);
            UnityContainer.RegisterInstance<IResourceSchemaAdapter>(RepositoryType.SQLServer2.ToCode(), mockAdapterNg.Object);

            var action = new AdaptResourceSchemaAction();
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.ControllerSchema = new DataSchema(Guid.NewGuid().ToString());
            action.RepositoryInfoList = new ReadOnlyCollection<RepositoryInfo>(repositoryInfoList);

            return (action, mockAdapterOk, mockAdapterNg);
        }

        private void Verify(
            Mock<IResourceSchemaAdapter> mockOk, int okIsAdaptableCount, int okAdaptCount,
            Mock<IResourceSchemaAdapter> mockNg, int ngIsAdaptableCount, int ngAdaptCount)
        {
            RFC7807ProblemDetailExtendErrors mockOut;
            mockOk.Verify(x => x.IsAdaptable(out mockOut), Times.Exactly(okIsAdaptableCount));
            mockOk.Verify(x => x.Adapt(), Times.Exactly(okAdaptCount));
            mockNg.Verify(x => x.IsAdaptable(out mockOut), Times.Exactly(ngIsAdaptableCount));
            mockNg.Verify(x => x.Adapt(), Times.Exactly(ngAdaptCount));
        }
    }
}
