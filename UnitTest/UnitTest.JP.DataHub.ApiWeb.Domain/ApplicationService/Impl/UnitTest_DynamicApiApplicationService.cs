using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Unity;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.ApplicationService.Impl
{
    [TestClass]
    public class UnitTest_DynamicApiApplicationService : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance<IDataContainer>(new PerRequestDataContainer());
        }

        [TestMethod]
        public void Get_正常系()
        {
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Returns<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.GET);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        getQueryParam.Value.Is(requestQuery.Value);
                        return mockIMethod.Object;
                    });
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            var result = target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null);
            IsEqual(result, mockResult);


            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(1));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Get_Exception_FindApiで発生()
        {
            var expectException = new Exception();

            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotOpenIdAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Callback<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.GET);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        getQueryParam.Value.Is(requestQuery.Value);
                    })
                .Throws(expectException);
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            AssertEx.Catch<Exception>(() => target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.GET),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null))
                .IsSameReferenceAs(expectException);

            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(0));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Post_正常系()
        {
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Returns<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.POST);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        return mockIMethod.Object;
                    });
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            var result = target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null);
            IsEqual(result, mockResult);


            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(1));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Post_Exception_FindApiで発生()
        {
            var expectException = new Exception();

            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotOpenIdAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Callback<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.POST);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                    })
                .Throws(expectException);
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            AssertEx.Catch<Exception>(() => target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.POST),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null))
                .IsSameReferenceAs(expectException);

            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(0));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Put_正常系()
        {
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Returns<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.PUT);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        return mockIMethod.Object;
                    });
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            var result = target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null);
            IsEqual(result, mockResult);


            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(1));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Put_Exception_FindApiで発生()
        {
            var expectException = new Exception();

            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotOpenIdAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Callback<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.PUT);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                    })
                .Throws(expectException);
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            AssertEx.Catch<Exception>(() => target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.PUT),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null))
                .IsSameReferenceAs(expectException);

            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(0));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Delete_正常系()
        {
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Returns<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.DELETE);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        getQueryParam.Value.Is(requestQuery.Value);
                        return mockIMethod.Object;
                    });
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            var result = target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null);
            IsEqual(result, mockResult);


            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(1));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Delete_Exception_FindApiで発生()
        {
            var expectException = new Exception();

            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotOpenIdAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Callback<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.DELETE);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        getQueryParam.Value.Is(requestQuery.Value);
                    })
                .Throws(expectException);
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            AssertEx.Catch<Exception>(() => target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.DELETE),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null))
                .IsSameReferenceAs(expectException);

            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(0));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Patch_正常系()
        {
            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Returns<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.PATCH);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        getQueryParam.Value.Is(requestQuery.Value);
                        return mockIMethod.Object;
                    });
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            var result = target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null);
            IsEqual(result, mockResult);


            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(1));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }

        [TestMethod]
        public void Patch_Exception_FindApiで発生()
        {
            var expectException = new Exception();

            var requestContents = new Contents(Guid.NewGuid().ToString());
            var requestMediaType = new MediaType(Guid.NewGuid().ToString());
            var requestRelativeUri = new RequestRelativeUri(Guid.NewGuid().ToString());
            var requestQuery = new QueryString(new Dictionary<string, string>() { { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() } }, Guid.NewGuid().ToString());
            var requestNotOpenIdAuthentication = new NotAuthentication(false);

            var mockResult = new HttpResponseMessage()
            {
                Content = new StringContent(Guid.NewGuid().ToString())
            };
            mockResult.Content.Headers.ContentType = new MediaTypeHeaderValue("hoge/fuga");
            var mockIMethod = new Mock<IMethod>();
            mockIMethod.SetupGet(x => x.ApiId).Returns(new ApiId(Guid.NewGuid().ToString()));
            mockIMethod.SetupGet(x => x.ControllerId).Returns(new ControllerId(Guid.NewGuid().ToString()));
            mockIMethod.Setup(x => x.Request(It.IsAny<ActionId>(),
                    It.Is<MediaType>(param => param == requestMediaType),
                    It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                    It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()))
                .Returns(mockResult);

            var mockIDynamicApiRepository = new Mock<IDynamicApiRepository>();
            mockIDynamicApiRepository.Setup(x => x.FindApi(
                    It.IsAny<HttpMethodType>(),
                    It.IsAny<RequestRelativeUri>(),
                    It.IsAny<GetQuery>())
                ).Callback<HttpMethodType, RequestRelativeUri, GetQuery>(
                    (httpMethodParamType, requestRelativeUriParam, getQueryParam) =>
                    {
                        httpMethodParamType.Value.Is(HttpMethodType.MethodTypeEnum.PATCH);
                        requestRelativeUriParam.IsSameReferenceAs(requestRelativeUri);
                        getQueryParam.Value.Is(requestQuery.Value);
                    })
                .Throws(expectException);
            UnityContainer.RegisterInstance(mockIDynamicApiRepository.Object);

            var mockIAsyncDyanamicApiRepository = new Mock<IAsyncDyanamicApiRepository>();
            UnityContainer.RegisterInstance(mockIAsyncDyanamicApiRepository.Object);

            var target = UnityContainer.Resolve<IDynamicApiApplicationService>();
            AssertEx.Catch<Exception>(() => target.Request(
                new HttpMethodType(HttpMethodType.MethodTypeEnum.PATCH),
                requestRelativeUri,
                requestContents,
                requestQuery,
                null,
                requestMediaType,
                null,
                null,
                null,
                null))
                .IsSameReferenceAs(expectException);

            mockIMethod.Verify(x => x.Request(It.IsAny<ActionId>(),
                It.Is<MediaType>(param => param == requestMediaType),
                It.Is<NotAuthentication>(param => param == requestNotOpenIdAuthentication),
                It.IsAny<Contents>(), It.IsAny<Accept>(), It.IsAny<ContentRange>()), Times.Exactly(0));

            mockIDynamicApiRepository.Verify(x => x.FindApi(
                It.IsAny<HttpMethodType>(),
                It.IsAny<RequestRelativeUri>(),
                It.IsAny<GetQuery>()),
                Times.Exactly(1)
            );
        }


        private void IsEqual(DynamicApiResponse actual, HttpResponseMessage expected)
        {
            actual.StatusCode.Is(expected.StatusCode);
            actual.Headers.IsStructuralEqual(expected.Headers.ToDomainHttpHeader());
            actual.Contents.ReadAsString().Is(expected.Content.Headers.ContentType is not null
                ? new DynamicApiReponseContents(expected.Content, expected.Headers.TransferEncoding.Any()).ReadAsString()
                : null);
            actual.ReasonPhrase.Is(expected.ReasonPhrase);
        }
    }
}
