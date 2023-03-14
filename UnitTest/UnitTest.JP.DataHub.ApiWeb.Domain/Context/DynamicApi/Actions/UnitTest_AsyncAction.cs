using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_AsyncAction : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterType<IDynamicApiAction, AsyncAction>();
            UnityContainer.RegisterInstance(Configuration);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(new PerRequestDataContainer());

            var moqRepository = new Mock<IDynamicApiRepository>();
            UnityContainer.RegisterInstance<ICache>(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<IDynamicApiRepository>(moqRepository.Object);
            UnityContainer.RegisterInstance<ICache>("DynamicApiBlobCache", new Mock<ICache>().Object);
        }

        private static Accept defaultAccept = new Accept("*/*");
        [TestMethod]
        public void AsyncAction_GetCase()
        {
            var moqRepository = new Mock<IAsyncDyanamicApiRepository>();
            moqRepository.Setup(x =>
                x.Request(It.IsAny<AsyncRequestId>(), It.IsAny<JsonDocument>(), It.IsAny<JsonDocument>())).Callback<AsyncRequestId, JsonDocument, JsonDocument>((request, body, log) =>
                {
                    log.Value["Status"].Is("Request");
                    body.Value["Url"].Is("/Api/hoge/get");
                    body.Value["QueryString"].Is("?hoge=aaaa");
                    body.Value["MethodType"].Is("GET");
                });
            UnityContainer.RegisterInstance<IAsyncDyanamicApiRepository>(moqRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiAction>();
            target.RelativeUri = new RelativeUri("/Api/hoge/get");
            target.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            target.Contents = new Contents("");
            var tempQuery = new Dictionary<QueryStringKey, QueryStringValue>();
            tempQuery.Add(new QueryStringKey("hoge"), new QueryStringValue("aaaa"));
            target.Query = new QueryStringVO(tempQuery);
            target.MediaType = new MediaType("application/json");
            target.Accept = defaultAccept;
            target.AsyncOriginalActionType = new ActionTypeVO(ActionType.Gateway);
            var result = target.ExecuteAction();
            result.StatusCode = HttpStatusCode.Accepted;
        }

        [TestMethod]
        public void AsyncAction_GetCaseExistsKeyValue()
        {
            var moqRepository = new Mock<IAsyncDyanamicApiRepository>();
            moqRepository.Setup(x =>
                x.Request(It.IsAny<AsyncRequestId>(), It.IsAny<JsonDocument>(), It.IsAny<JsonDocument>())).Callback<AsyncRequestId, JsonDocument, JsonDocument>((request, body, log) =>
                {
                    log.Value["Status"].Is("Request");
                    body.Value["Url"].Is("/Api/hoge/get");
                    body.Value["QueryString"].Is("?hoge=aaaa&foo=bbbb");
                    body.Value["MethodType"].Is("GET");
                });
            UnityContainer.RegisterInstance<IAsyncDyanamicApiRepository>(moqRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiAction>();
            target.RelativeUri = new RelativeUri("/Api/hoge/get");
            target.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            target.Contents = new Contents("");
            var tempQuery = new Dictionary<QueryStringKey, QueryStringValue>();
            tempQuery.Add(new QueryStringKey("hoge"), new QueryStringValue("aaaa"));
            tempQuery.Add(new QueryStringKey("foo"), new QueryStringValue("bbbb"));

            var tempKeyValue = new Dictionary<UrlParameterKey, UrlParameterValue>();
            tempKeyValue.Add(new UrlParameterKey("foo"), new UrlParameterValue("bbbb"));
            target.Query = new QueryStringVO(tempQuery);
            target.MediaType = new MediaType("application/json");
            target.Accept = defaultAccept;
            target.AsyncOriginalActionType = new ActionTypeVO(ActionType.Gateway);

            var result = target.ExecuteAction();
            result.StatusCode = HttpStatusCode.Accepted;
        }
    }
}
