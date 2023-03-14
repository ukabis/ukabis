using System.Collections.Generic;
using System.Net;
using AspNetCoreHttp=Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Json.Schema;
using Unity;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Domain.ApplicationService;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.JsonValidator
{
    [TestClass]
    public class UnitTest_JsonFormatCustomValidator : UnitTestBase
    {
        private UnityContainer _container;

        [TestInitialize]
        public void TestInitialize()
        {
            _container = new UnityContainer();
            UnityCore.UnityContainer = _container;
            _container.RegisterInstance("UseForeignKeyCache", true);
            _container.RegisterInstance(Configuration);

            var datacontainer = new PerRequestDataContainer();
            _container.RegisterInstance<IPerRequestDataContainer>(datacontainer);

            _container.RegisterType<AspNetCoreHttp.IHttpContextAccessor, AspNetCoreHttp.HttpContextAccessor>();
            //HttpContext.Current = new HttpContext(new HttpRequest("", "http://localhost", ""), new HttpResponse(new StringWriter()));
        }

        const string model_fk_none = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null']
            }
          },
          'required':['a0']
        }";


        const string model_fk_single = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null'],
              'format':'ForeignKey /API/Test/Hoge/Exists/{value}'
            }
          },
          'required':['a0']
        }";
        const string model_fk_single_qs = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null'],
              'format':'ForeignKey /API/Test/Hoge/Get?id={value}'
            }
          },
          'required':['a0']
        }";

        const string model_fk_double = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null'],
              'format':'ForeignKey /API/Test/Hoge/Exists/{value}'
            },
            'a2': {
              'title': 'a2',
              'type': ['string','null'],
              'format':'ForeignKey /API/Test/Fuga/Exists/{value}'
            }
          },
          'required':['a0']
        }";

        const string model_fk_double_qs = @"
        {
          'title': 'testModel',
          'description': '',
          'type': 'object',
          'additionalProperties': false,
          'properties': {
             'a0': {
              'title': 'a0',
              'type': 'number'
            },
            'a1': {
              'title': 'a1',
              'type': ['string','null'],
              'format':'ForeignKey /API/Test/Hoge/Get?id={value}'
            },
            'a2': {
              'title': 'a2',
              'type': ['string','null'],
              'format':'ForeignKey /API/Test/Fuga/Get?id={value}'
            }
          },
          'required':['a0']
        }";


        [TestMethod]
        public void Validate_正常系_KeyValue()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(), 
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '1'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_single, readerSetting);
            input.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Hoge/Exists/1"), It.IsAny<Contents>(), It.IsAny<QueryString>(), 
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }

        [TestMethod]
        public void Validate_異常系_KeyValue()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.NotFound));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '1'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_single, readerSetting);
            input.IsValid(model).IsFalse();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Hoge/Exists/1"), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }


        [TestMethod]
        public void Validate_正常系_QueryString()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK))
                ;
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '1'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_single_qs, readerSetting);
            input.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Hoge/Get"), It.IsAny<Contents>(), It.Is<QueryString>(x => x.Value == "?id=1"),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }

        [TestMethod]
        public void Validate_正常系_KeyValue_複数()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '1', 'a2': '2'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_double, readerSetting);
            input.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Hoge/Exists/1"), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Fuga/Exists/2"), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }

        [TestMethod]
        public void Validate_正常系_QueryString_複数()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '1', 'a2': '2'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_double_qs, readerSetting);
            input.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Hoge/Get"), It.IsAny<Contents>(), It.Is<QueryString>(x => x.Value == "?id=1"),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Fuga/Get"), It.IsAny<Contents>(), It.Is<QueryString>(x => x.Value == "?id=2"),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }

        [TestMethod]
        public void Validate_正常系_KeyValue_複数_同一キー()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '2', 'a2': '3'}");
            var input2 = JToken.Parse(@"{ 'a0' : 4, 'a1': '5', 'a2': '3'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_double, readerSetting);
            input.IsValid(model).IsTrue();
            input2.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value.StartsWith("/API/Test/Hoge/Exists/")), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Exactly(2));
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value.StartsWith("/API/Test/Fuga/Exists/")), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }

        [TestMethod]
        public void Validate_正常系_QueryString_複数_同一キー()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '2', 'a2': '3'}");
            var input2 = JToken.Parse(@"{ 'a0' : 4, 'a1': '5', 'a2': '3'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_double_qs, readerSetting);
            input.IsValid(model).IsTrue();
            input2.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value.StartsWith("/API/Test/Hoge/Get")), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Exactly(2));
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value.StartsWith("/API/Test/Fuga/Get")), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Once);
        }

        [TestMethod]
        public void Validate_正常系_KeyValue_複数_同一キー_configなし()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);
            _container.RegisterInstance("UseForeignKeyCache", false);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '2', 'a2': '3'}");
            var input2 = JToken.Parse(@"{ 'a0' : 4, 'a1': '5', 'a2': '3'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_double, readerSetting);
            input.IsValid(model).IsTrue();
            input2.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value.StartsWith("/API/Test/Hoge/Exists/")), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Exactly(2));
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value.StartsWith("/API/Test/Fuga/Exists/")), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Validate_正常系_QueryString_複数_同一キー_configなし()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK))
                ;
            _container.RegisterInstance(mock.Object);
            _container.RegisterInstance("UseForeignKeyCache", false);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '2', 'a2': '3'}");
            var input2 = JToken.Parse(@"{ 'a0' : 4, 'a1': '5', 'a2': '3'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_double_qs, readerSetting);
            input.IsValid(model).IsTrue();
            input2.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Hoge/Get"), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Exactly(2));
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.Is<RequestRelativeUri>(x => x.Value == "/API/Test/Fuga/Get"), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Validate_正常系_ForeignKey無し()
        {
            var mock = new Mock<IDynamicApiApplicationService>();
            mock.Setup(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                    It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()))
                .Returns(new DynamicApiResponse(HttpStatusCode.OK));
            _container.RegisterInstance(mock.Object);

            var input = JToken.Parse(@"{ 'a0' : 1, 'a1': '1'}");
            var readerSetting = new JSchemaReaderSettings
            { Validators = new List<IJsonValidator>() { new JsonFormatCustomValidator() } };

            var model = JSchema.Parse(model_fk_none, readerSetting);
            input.IsValid(model).IsTrue();
            mock.Verify(_ => _.Request(It.IsAny<HttpMethodType>(), It.IsAny<RequestRelativeUri>(), It.IsAny<Contents>(), It.IsAny<QueryString>(),
                It.IsAny<HttpHeader>(), It.IsAny<MediaType>(), It.IsAny<Accept>(), It.IsAny<ContentRange>(), It.IsAny<ContentType>(), It.IsAny<ContentLength>(), It.IsAny<NotAuthentication>()), Times.Never);
        }
    }
}
