using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using Unity;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.Unity;
using JP.DataHub.Api.Core.ErrorCode;
using JP.DataHub.ApiWeb.Core.Model;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Interface;
using JP.DataHub.ApiWeb.Domain.Scripting;
using JP.DataHub.ApiWeb.Domain.Scripting.Roslyn;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Scripting.Roslyn
{
    [TestClass]
    public class UnitTest_ApiHelper : UnitTestBase
    {
        #region Setup

        [TestInitialize]
        public void TestInitialize()
        {

        }

        private void SetUpContainer(bool returnJsonCalidatorErrorDetail = true, bool returnNeedsJsonValidatorErrorDetail = false)
        {
            base.TestInitialize(true);

            var dataContainer = new PerRequestDataContainer();
            dataContainer.ReturnNeedsJsonValidatorErrorDetail = returnNeedsJsonValidatorErrorDetail;

            UnityContainer.RegisterInstance<IDataContainer>(dataContainer);
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(dataContainer);
            UnityContainer.RegisterInstance("UseForeignKeyCache", false);
            UnityContainer.RegisterInstance("ThresholdJsonSchemaValidaitonParallelize", 100);
            UnityContainer.RegisterInstance("Return.JsonValidator.ErrorDetail", returnJsonCalidatorErrorDetail);
        }

        #endregion


        [TestMethod]
        public void ExecuteGetApi_正常系_CustomHeader指定あり()
        {
            this.SetUpContainer();

            var url = "/API/test";
            var contents = "testContens";
            var querystring = Guid.NewGuid().ToString();
            var internalCallKeyword = Guid.NewGuid().ToString();
            var personsharing = Guid.NewGuid().ToString();
            var headers = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } },
                { "X-Admin", new List<string>() { "HOGE" } },
                { "X-IsAsync", new List<string>() { "True" } },
                { "X-ResourceSharingPerson", new List<string>() { personsharing } },
            };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    model.RelativeUri.Is(url);
                    model.Contents.Is(contents);
                    model.QueryString.Is($"?{querystring}");
                    auth.IsFalse();

                    var calbackperRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                    calbackperRequestDataContainer.RequestHeaders.IsStructuralEqual(headers);
                    calbackperRequestDataContainer.Xadmin.Is("HOGE");
                    calbackperRequestDataContainer.XAsync.Is(true);
                    calbackperRequestDataContainer.XResourceSharingPerson.Is(personsharing);
                })
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = apiHelper.ExecuteGetApi(url, contents, querystring, internalCallKeyword, headers);

            // 検証
            result.IsStructuralEqual(httpResponseMessage);
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            perRequestDataContainer.Xadmin.IsNull();
            perRequestDataContainer.XAsync.Is(false);
            perRequestDataContainer.XResourceSharingPerson.IsNull();
        }

        [TestMethod]
        public void ExecuteGetApi_正常系_Header書き換え()
        {
            this.SetUpContainer();
            var headers = new Dictionary<string, List<string>>() { { Guid.NewGuid().ToString(), new List<string>() { "orignalheadervalue" } } };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    //ヘッダー書き換え
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.Values.First()[0] = "hogehoge";
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            _ = apiHelper.ExecuteGetApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, headers);

            headers.Values.First()[0].Is("orignalheadervalue");
        }

        [TestMethod]
        public void ExecuteGetApi_正常系_Headerなし()
        {
            this.SetUpContainer();
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.IsNull();
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders = null;

            _ = apiHelper.ExecuteGetApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, null);
        }

        [TestMethod]
        public void ExecuteDeleteApi_正常系_CustomHeader指定あり()
        {
            this.SetUpContainer();

            var url = "/API/test";
            var contents = "testContens";
            var querystring = Guid.NewGuid().ToString();
            var internalCallKeyword = Guid.NewGuid().ToString();
            var personsharing = Guid.NewGuid().ToString();
            var headers = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } },
                { "X-Admin", new List<string>() { "HOGE" } },
                { "X-IsAsync", new List<string>() { "True" } },
                { "X-ResourceSharingPerson", new List<string>() { personsharing } },
            };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    model.RelativeUri.Is(url);
                    model.Contents.Is(contents);
                    model.QueryString.Is($"?{querystring}");
                    auth.IsFalse();

                    var calbackperRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                    calbackperRequestDataContainer.RequestHeaders.IsStructuralEqual(headers);
                    calbackperRequestDataContainer.Xadmin.Is("HOGE");
                    calbackperRequestDataContainer.XAsync.Is(true);
                    calbackperRequestDataContainer.XResourceSharingPerson.Is(personsharing);
                })
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = apiHelper.ExecuteDeleteApi(url, contents, querystring, internalCallKeyword, headers);

            // 検証
            result.IsStructuralEqual(httpResponseMessage);
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            perRequestDataContainer.Xadmin.IsNull();
            perRequestDataContainer.XAsync.Is(false);
            perRequestDataContainer.XResourceSharingPerson.IsNull();
        }

        [TestMethod]
        public void ExecuteDeleteApi_正常系_Header書き換え()
        {
            this.SetUpContainer();
            var headers = new Dictionary<string, List<string>>() { { Guid.NewGuid().ToString(), new List<string>() { "orignalheadervalue" } } };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    //ヘッダー書き換え
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.Values.First()[0] = "hogehoge";
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            _ = apiHelper.ExecuteDeleteApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, headers);

            headers.Values.First()[0].Is("orignalheadervalue");
        }

        [TestMethod]
        public void ExecuteDeleteApi_正常系_Headerなし()
        {
            this.SetUpContainer();
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.IsNull();
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders = null;

            _ = apiHelper.ExecuteDeleteApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, null);
        }

        [TestMethod]
        public void ExecutePatchApi_正常系_CustomHeader指定あり()
        {
            this.SetUpContainer();

            var url = "/API/test";
            var contents = "testContens";
            var querystring = Guid.NewGuid().ToString();
            var internalCallKeyword = Guid.NewGuid().ToString();
            var personsharing = Guid.NewGuid().ToString();
            var headers = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } },
                { "X-Admin", new List<string>() { "HOGE" } },
                { "X-IsAsync", new List<string>() { "True" } },
                { "X-ResourceSharingPerson", new List<string>() { personsharing } },
            };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    model.RelativeUri.Is(url);
                    model.Contents.Is(contents);
                    model.QueryString.Is($"?{querystring}");
                    auth.IsFalse();

                    var calbackperRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                    calbackperRequestDataContainer.RequestHeaders.IsStructuralEqual(headers);
                    calbackperRequestDataContainer.Xadmin.Is("HOGE");
                    calbackperRequestDataContainer.XAsync.Is(true);
                    calbackperRequestDataContainer.XResourceSharingPerson.Is(personsharing);
                })
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = apiHelper.ExecutePatchApi(url, contents, querystring, internalCallKeyword, headers);

            // 検証
            result.IsStructuralEqual(httpResponseMessage);
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            perRequestDataContainer.Xadmin.IsNull();
            perRequestDataContainer.XAsync.Is(false);
            perRequestDataContainer.XResourceSharingPerson.IsNull();
        }

        [TestMethod]
        public void ExecutePatchApi_正常系_Header書き換え()
        {
            this.SetUpContainer();
            var headers = new Dictionary<string, List<string>>() { { Guid.NewGuid().ToString(), new List<string>() { "orignalheadervalue" } } };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    //ヘッダー書き換え
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.Values.First()[0] = "hogehoge";
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            _ = apiHelper.ExecutePatchApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, headers);

            headers.Values.First()[0].Is("orignalheadervalue");
        }

        [TestMethod]
        public void ExecutePatchApi_正常系_Headerなし()
        {
            this.SetUpContainer();
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.IsNull();
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders = null;

            _ = apiHelper.ExecutePatchApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, null);
        }

        [TestMethod]
        public void ExecutePostApi_正常系_ContentType指定なし()
        {
            this.SetUpContainer();

            var url = "/API/test";
            var contents = "testContens";
            var querystring = Guid.NewGuid().ToString();
            var internalCallKeyword = Guid.NewGuid().ToString();
            var headers = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } },
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } }
            };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    model.RelativeUri.Is(url);
                    model.Contents.Is(contents);
                    model.MediaType.Is("application/json");
                    model.QueryString.Is($"?{querystring}");
                    auth.IsFalse();

                    var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                    perRequestDataContainer.RequestHeaders.IsStructuralEqual(headers);
                })
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = apiHelper.ExecutePostApi(url, contents, querystring, internalCallKeyword, headers);

            // 検証
            result.IsStructuralEqual(httpResponseMessage);
        }

        [TestMethod]
        public void ExecutePostApi_正常系_ContentType指定あり_CustomHeader指定あり()
        {
            this.SetUpContainer();

            var url = "/API/test";
            var contents = "testContens";
            var querystring = Guid.NewGuid().ToString();
            var internalCallKeyword = Guid.NewGuid().ToString();
            var personsharing = Guid.NewGuid().ToString();
            var headers = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } },
                { "Content-Type", new List<string>() { Guid.NewGuid().ToString() } },
                { "X-Admin", new List<string>() { "HOGE" } },
                { "X-IsAsync", new List<string>() { "True" } },
                { "X-ResourceSharingPerson", new List<string>() { personsharing } },
            };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    model.RelativeUri.Is(url);
                    model.Contents.Is(contents);
                    model.MediaType.Is(headers["Content-Type"].First());
                    model.QueryString.Is($"?{querystring}");
                    auth.IsFalse();

                    var calbackperRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                    calbackperRequestDataContainer.RequestHeaders.IsStructuralEqual(headers);
                    calbackperRequestDataContainer.Xadmin.Is("HOGE");
                    calbackperRequestDataContainer.XAsync.Is(true);
                    calbackperRequestDataContainer.XResourceSharingPerson.Is(personsharing);
                })
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = apiHelper.ExecutePostApi(url, contents, querystring, internalCallKeyword, headers);

            // 検証
            result.IsStructuralEqual(httpResponseMessage);
            var perRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
            perRequestDataContainer.Xadmin.IsNull();
            perRequestDataContainer.XAsync.Is(false);
            perRequestDataContainer.XResourceSharingPerson.IsNull();
        }

        [TestMethod]
        public void ExecutePostApi_正常系_Header書き換え()
        {
            this.SetUpContainer();
            var headers = new Dictionary<string, List<string>>() { { Guid.NewGuid().ToString(), new List<string>() { "orignalheadervalue" } } };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    //ヘッダー書き換え
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.Values.First()[0] = "hogehoge";
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            _ = apiHelper.ExecutePostApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, headers);

            headers.Values.First()[0].Is("orignalheadervalue");
        }

        [TestMethod]
        public void ExecutePostApi_正常系_Headerなし()
        {
            this.SetUpContainer();
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.IsNull();
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders = null;

            _ = apiHelper.ExecutePostApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, null);
        }
        [TestMethod]
        public void ExecutePutApi_正常系()
        {
            for (int i = 0; i < 3; i++)
            {
                this.SetUpContainer();

                var url = "/API/test";
                var contents = "testContens";
                var apiHelper = new ApiHelper();

                var mock = new Mock<IDynamicApiInterface>();
                var httpResponseMessage = new HttpResponseMessage();
                mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                    .Returns(new DynamicApiResponse(httpResponseMessage));
                UnityContainer.RegisterInstance(mock.Object);

                var result = apiHelper.ExecutePutApi(url, contents);

                // 検証
                result.IsStructuralEqual(httpResponseMessage);
            }
        }


        [TestMethod]
        public void ExecutePutApi_正常系_CustomHeader指定あり()
        {
            {
                this.SetUpContainer();

                var url = "/API/test";
                var contents = "testContens";
                var querystring = Guid.NewGuid().ToString();
                var internalCallKeyword = Guid.NewGuid().ToString();
                var personsharing = Guid.NewGuid().ToString();
                var headers = new Dictionary<string, List<string>>()
            {
                { Guid.NewGuid().ToString(), new List<string>() { Guid.NewGuid().ToString() } },
                { "Content-Type", new List<string>() { Guid.NewGuid().ToString() } },
                { "X-Admin", new List<string>() { "HOGE" } },
                { "X-IsAsync", new List<string>() { "True" } },
                { "X-ResourceSharingPerson", new List<string>() { personsharing } },
            };

                var apiHelper = new ApiHelper();

                var mock = new Mock<IDynamicApiInterface>();
                var httpResponseMessage = new HttpResponseMessage();
                mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                    .Returns(new DynamicApiResponse(httpResponseMessage))
                    .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                    {
                        model.RelativeUri.Is(url);
                        model.Contents.Is(contents);
                        model.QueryString.Is($"?{querystring}");
                        auth.IsFalse();

                        var calbackperRequestDataContainer = UnityCore.Resolve<IPerRequestDataContainer>();
                        calbackperRequestDataContainer.RequestHeaders.IsStructuralEqual(headers);
                        calbackperRequestDataContainer.Xadmin.Is("HOGE");
                        calbackperRequestDataContainer.XAsync.Is(true);
                        calbackperRequestDataContainer.XResourceSharingPerson.Is(personsharing);
                    })
                ;
                UnityContainer.RegisterInstance(mock.Object);

                var result = apiHelper.ExecutePutApi(url, contents, querystring, internalCallKeyword, headers);

                // 検証
                result.IsStructuralEqual(httpResponseMessage);
            }
        }

        [TestMethod]
        public void ExecutePutApi_正常系_Header書き換え()
        {
            this.SetUpContainer();
            var headers = new Dictionary<string, List<string>>() { { Guid.NewGuid().ToString(), new List<string>() { "orignalheadervalue" } } };
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    //ヘッダー書き換え
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.Values.First()[0] = "hogehoge";
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            _ = apiHelper.ExecutePutApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, headers);

            headers.Values.First()[0].Is("orignalheadervalue");
        }

        [TestMethod]
        public void ExecutePutApi_正常系_Headerなし()
        {
            this.SetUpContainer();
            var apiHelper = new ApiHelper();

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Callback<DynamicApiRequestModel, bool>((model, auth) =>
                {
                    UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders.IsNull();
                })
                .Returns(new DynamicApiResponse(new HttpResponseMessage()));
            UnityContainer.RegisterInstance(mock.Object);
            UnityCore.Resolve<IPerRequestDataContainer>().RequestHeaders = null;

            _ = apiHelper.ExecutePutApi("/API/test", "testContens", Guid.NewGuid().ToString(), null, null);
        }

        [TestMethod]
        public void ExecuteGetApiToObject_StringContent()
        {
            var data = new TestClass { Name = Guid.NewGuid().ToString(), Value = 101 };
            SetUpContainer();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage { Content = new StringContent(JsonConvert.SerializeObject(data)) };
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = new ApiHelper().ExecuteGetApiToObject<TestClass>("/API/test");
            // 検証
            result.IsStructuralEqual(data);
        }

        [TestMethod]
        public void ExecuteGetApiToObject_StreamContent()
        {
            var data = new TestClass { Name = Guid.NewGuid().ToString(), Value = 102 };
            SetUpContainer();

            var mock = new Mock<IDynamicApiInterface>();
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))))
            };
            httpResponseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            mock.Setup(x => x.Request(It.IsAny<DynamicApiRequestModel>(), It.IsAny<bool>()))
                .Returns(new DynamicApiResponse(httpResponseMessage));
            UnityContainer.RegisterInstance(mock.Object);

            var result = new ApiHelper().ExecuteGetApiToObject<TestClass>("/API/test");
            // 検証
            result.IsStructuralEqual(data);
        }

        [TestMethod]
        public void ValidateWithRequestModel_正常系()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-01'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            result.IsNull();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValidateWithRequestModel_異常系_model_is_null()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-01'}}";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(null));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            result.IsNull();
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_ContentsIsNotValidJson()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["Unparsable"].Single().Is("Unterminated string. Expected delimiter: '. Path 'a1', line 1, position 18.");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_FormatError()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is("String '2020-01-32' does not validate against format 'date'.(code:23)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_Format複数指定_JSchemaDefined項目あり_Format項目以外でエラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32a'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date;xxxx;yyyy'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is("String '2020-01-32a' exceeds maximum length of 10.(code:4)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_Format複数指定_JSchemaDefined項目あり_Format項目でエラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date;xxxx;yyyy'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is("String '2020-01-32' does not validate against format 'date'. Path ''.(code:26)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_Format複数指定_JSchemaDefined項目なし_Format以外の項目でエラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32a'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'xxxx;yyyy'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is("String '2020-01-32a' exceeds maximum length of 10.(code:4)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_Format複数指定_JSchemaDefined項目の判定テスト用()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'? 🔰 ?'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'host-name;ip-address;hostname;date-time;date;time;utc-millisec;regex;color;style;phone;uri;uri-reference;uri-template;json-pointer;ipv6;ipv4;email'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors.Single().Value.ToList();
            chk.Count.Is(18 - 2);
            chk[0].ToString().Is("String '? 🔰 ?' does not validate against format 'host-name'. Path ''.(code:26)");
            chk[1].ToString().Is("String '? 🔰 ?' does not validate against format 'ip-address'. Path ''.(code:26)");
            chk[2].ToString().Is("String '? 🔰 ?' does not validate against format 'hostname'. Path ''.(code:26)");
            chk[3].ToString().Is("String '? 🔰 ?' does not validate against format 'date-time'. Path ''.(code:26)");
            chk[4].ToString().Is("String '? 🔰 ?' does not validate against format 'date'. Path ''.(code:26)");
            chk[5].ToString().Is("String '? 🔰 ?' does not validate against format 'time'. Path ''.(code:26)");
            chk[6].ToString().Is("String '? 🔰 ?' does not validate against format 'utc-millisec'. Path ''.(code:26)");
            chk[7].ToString().Is("String '? 🔰 ?' does not validate against format 'regex'. Path ''.(code:26)");
            chk[8].ToString().Is("String '? 🔰 ?' does not validate against format 'color'. Path ''.(code:26)");
            // style: JsonSchemaにvalidation処理がない
            // phone: JsonSchemaにvalidation処理がない
            chk[9].ToString().Is("String '? 🔰 ?' does not validate against format 'uri'. Path ''.(code:26)");
            chk[10].ToString().Is("String '? 🔰 ?' does not validate against format 'uri-reference'. Path ''.(code:26)");
            chk[11].ToString().Is("String '? 🔰 ?' does not validate against format 'uri-template'. Path ''.(code:26)");
            chk[12].ToString().Is("String '? 🔰 ?' does not validate against format 'json-pointer'. Path ''.(code:26)");
            chk[13].ToString().Is("String '? 🔰 ?' does not validate against format 'ipv6'. Path ''.(code:26)");
            chk[14].ToString().Is("String '? 🔰 ?' does not validate against format 'ipv4'. Path ''.(code:26)");
            chk[15].ToString().Is("String '? 🔰 ?' does not validate against format 'email'. Path ''.(code:26)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_1件()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = err.errors["a2"].ToList();
            chk.Count.Is(2);
            chk[0].ToString().Is("String 'hoge' exceeds maximum length of 1.(code:4)");
            chk[1].ToString().Is("Value \"hoge\" is not defined in enum.(code:17)");
            chk = err.errors["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = err.errors["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_Array1件()
        {
            var designationId = new Guid().ToString();
            var contents = "[{'a1':'hoge','a2':'hoge2'}]";
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors["RootInvalid"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected Object but got Array.(code:18)");
        }


        [TestMethod]
        public void ValidateWithRequestModel_異常系_複数件_初回エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a1':10000, 'a2': 1000, 'a3':'fuga'}},
                                        {{'a0':2,'a1':'20000', 'a2':'hoge2','a4':2}},
                                        {{'a0':3,'a1':'30000', 'a2':'hoge3','a4':3}},
                                       ]";
            var model = @"
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
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            mockAction.SetupGet(x => x.PostDataType).Returns(new PostDataType("array"));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.errors.Count().Is(4);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = err.errors["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = err.errors["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = err.errors["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_複数件_2件目エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a0':1,'a1':'10000', 'a2':'hoge1','a4':1}},
                                        {{'a1':200, 'a2':220, 'a3':'fuga' }},
                                        {{'a0':3,'a1':'30000', 'a2':'hoge3','a4':3}},
                                       ]";
            var model = @"
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
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            mockAction.SetupGet(x => x.PostDataType).Returns(new PostDataType("array"));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.errors.Count().Is(4);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = err.errors["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = err.errors["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = err.errors["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_複数件_3件目エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a0':1,'a1':'10000', 'a2':'hoge1','a4':1}},
                                        {{'a0':2,'a1':'20000', 'a2':'hoge2','a4':2}},
                                        {{'a1':3000, 'a2':3300, 'a3':'fuga3'}},
                                       ]";
            var model = @"
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
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            mockAction.SetupGet(x => x.PostDataType).Returns(new PostDataType("array"));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            var err = result.Single();
            err.errors.Count().Is(4);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = err.errors["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = err.errors["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = err.errors["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_異常系_複数件_全部エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a1':10000, 'a2':1100,'a3':'fuga1'}},
                                        {{'a1':20000, 'a2':2200,'a3':'fuga2'}},
                                        {{'a1':30000, 'a2':3300,'a3':'fuga3'}},
                                       ]";
            var model = @"
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
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            mockAction.SetupGet(x => x.PostDataType).Returns(new PostDataType("array"));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);
            result.Count().Is(3);
            result.All(x => x.error_code == ErrorCodeMessage.Code.E10402.ToString()).IsTrue();
            result.All(x => x.status == 400).IsTrue();
            var msg = result.ToList();
            var messages = msg[0].errors.ToDictionary(x => x.Key, y => y.Value.ToList());
            messages.Count().Is(4);
            messages["a1"].Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");

            messages = msg[1].errors.ToDictionary(x => x.Key, y => y.Value.ToList());
            messages.Count().Is(4);
            messages["a1"].Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");

            messages = msg[2].errors.ToDictionary(x => x.Key, y => y.Value.ToList());
            messages.Count().Is(4);
            messages["a1"].Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModel_正常系()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-01'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithRequestModel(contents);
            result.StatusCode.Is(HttpStatusCode.OK);
        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModelByResourceUrl_正常系()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-01'}}";
            var contents2 = $@"{{'a1':'2020-01-50'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.GetControllerSchemaByUrl(It.IsAny<string>())).Returns(model);
            this.SetUpContainer();
            UnityContainer.RegisterInstance(mock.Object);

            // バリデーション発生しないパターン
            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithModelByResourceUrl("/API/Master/Test", contents);
            result.StatusCode.Is(HttpStatusCode.OK);

            // バリデーション発生時
            result = target.CreateHttpResponseFromValidateWithModelByResourceUrl("/API/Master/Test", contents2);
            var err = JsonConvert.DeserializeObject<Rfc7807>(result.Content.ReadAsStringAsync().Result);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is($"String '2020-01-50' does not validate against format 'date'.(code:23)");
        }

        /// <summary>
        /// URLからモデルが見つからない想定のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateHttpResponseFromValidateWithRequestModelByResourceUrl_異常系()
        {
            var contents = $@"{{'a1':'2020-01-01'}}";
            string model = null;

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.GetControllerSchemaByUrl(It.IsAny<string>())).Returns(model);
            this.SetUpContainer();
            UnityContainer.RegisterInstance(mock.Object);

            var target = new ApiHelper();
            target.CreateHttpResponseFromValidateWithModelByResourceUrl("/API/Master/Test", contents);
        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModelByModelName_正常系()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-01'}}";
            var contents2 = $@"{{'a1':'2020-13-01'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.GetSchemaModelByName(It.IsAny<string>())).Returns(model);
            this.SetUpContainer();
            UnityContainer.RegisterInstance(mock.Object);

            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithModelByModelName("データモデル", contents);
            result.StatusCode.Is(HttpStatusCode.OK);

            // バリデーション発生時
            result = target.CreateHttpResponseFromValidateWithModelByModelName("データモデル", contents2);
            var err = JsonConvert.DeserializeObject<Rfc7807>(result.Content.ReadAsStringAsync().Result);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is($"String '2020-13-01' does not validate against format 'date'.(code:23)");
        }

        /// <summary>
        /// モデル名が見つからない想定のテスト
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateHttpResponseFromValidateWithRequestModelByModelName_異常系()
        {
            var contents = $@"{{'a1':'2020-01-01'}}";
            string model = null;

            var mock = new Mock<IDynamicApiInterface>();
            mock.Setup(x => x.GetSchemaModelByName(It.IsAny<string>())).Returns(model);
            this.SetUpContainer();
            UnityContainer.RegisterInstance(mock.Object);

            var target = new ApiHelper();
            target.CreateHttpResponseFromValidateWithModelByModelName("データモデル", contents);
        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModel_異常系_FormatError()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32'}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithRequestModel(contents);
            var err = JsonConvert.DeserializeObject<Rfc7807>(result.Content.ReadAsStringAsync().Result);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["a1"].Single().Is("String '2020-01-32' does not validate against format 'date'.(code:23)");

        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModel_異常系_ContentsIsNotValidJson()
        {
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':'2020-01-32}}";
            var model = @"
{
    'description': 'test',
    'type': 'object',
    'properties' :{
        'a1': {
            'title': '日',
            'type': [
                'string'
            ],
            'minLength': 0,
            'maxLength': 10,
            'format': 'date'
        }
    }
}
";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithRequestModel(contents);
            var err = JsonConvert.DeserializeObject<Rfc7807>(result.Content.ReadAsStringAsync().Result);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors["Unparsable"].Single().Is("Unterminated string. Expected delimiter: '. Path 'a1', line 1, position 18.");
        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModel_異常系_複数件_全部エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a1':10000, 'a2':1100,'a3':'fuga1'}},
                                        {{'a1':20000, 'a2':2200,'a3':'fuga2'}},
                                        {{'a1':30000, 'a2':3300,'a3':'fuga3'}},
                                       ]";
            var model = @"
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
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            mockAction.SetupGet(x => x.PostDataType).Returns(new PostDataType("array"));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithRequestModel(contents);

            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var msg = JsonConvert.DeserializeObject<List<Rfc7807>>(result.Content.ReadAsStringAsync().Result);
            var messages = msg[0].errors.ToDictionary(x => x.Key, y => y.Value.ToList());
            messages.Count().Is(4);
            messages["a1"].Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");

            messages = msg[1].errors.ToDictionary(x => x.Key, y => y.Value.ToList());
            messages.Count().Is(4);
            messages["a1"].Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");

            messages = msg[2].errors.ToDictionary(x => x.Key, y => y.Value.ToList());
            messages.Count().Is(4);
            messages["a1"].Count().Is(1);
            messages["a1"][0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            messages["a2"].Count().Is(1);
            messages["a2"][0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            messages["a3"].Count().Is(1);
            messages["a3"][0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            messages["a0,a4"].Count().Is(1);
            messages["a0,a4"][0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void CreateHttpResponseFromValidateWithRequestModel_異常系_複数件_初回エラー()
        {
            var designationId = new Guid().ToString();
            var contents = $@"[ {{'a1':10000, 'a2': 1000, 'a3':'fuga'}},
                                        {{'a0':2,'a1':'20000', 'a2':'hoge2','a4':2}},
                                        {{'a0':3,'a1':'30000', 'a2':'hoge3','a4':3}},
                                       ]";
            var model = @"
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
            },
            'a2':  {
              'title': 'a2',
              'maxLength': 5,
              'enum': [ 'hoge', 'hoge1', 'hoge2', 'hoge3', null],
              'type': 'string'
            },
             'a4': {
              'title': 'a4',
              'type': 'number'
            }
          },
          'required':['a0','a4']
        }";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            mockAction.SetupGet(x => x.PostDataType).Returns(new PostDataType("array"));
            this.SetUpContainer();
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.CreateHttpResponseFromValidateWithRequestModel(contents);
            result.StatusCode.Is(HttpStatusCode.BadRequest);
            var err = JsonConvert.DeserializeObject<Rfc7807>(result.Content.ReadAsStringAsync().Result);
            err.errors.Count().Is(4);
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            var chk = err.errors["a1"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String, Null but got Integer.(code:18)");
            chk = err.errors["a2"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Invalid type. Expected String but got Integer.(code:18)");
            chk = err.errors["a3"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Property 'a3' has not been defined and the schema does not allow additional properties.(code:15)");
            chk = err.errors["a0,a4"].ToList();
            chk.Count.Is(1);
            chk[0].ToString().Is("Required properties are missing from object: a0, a4.(code:14)");
        }

        [TestMethod]
        public void ValidateWithRequestModel_DetailOff()
        {
            // JsonSchemaとJsonの組み合わせは、いっぱいエラーが出るパターンである
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            SetUpContainer(false, false);
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);

            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors.Count.Is(0);
        }

        [TestMethod]
        public void ValidateWithRequestModel_DetailOff_NeedsDetail()
        {
            // JsonSchemaとJsonの組み合わせは、いっぱいエラーが出るパターンである
            var designationId = new Guid().ToString();
            var contents = $@"{{'a1':10000, 'a2':'hoge', 'a3':'fuga'}}";
            var model = @"
{
  'title': 'testModel',
  'description': '',
  'type': 'object',
  'additionalProperties': false,
  'properties': {
     'a1': {
      'title': 'a1',
      'type': ['string','null']
    },
    'a2':  {
      'title': 'a2',
      'maxLength': 1,
      'enum': [ 'hoghogehogheo', null],
      'type': 'string'
    }
  },
  'required':['a0','a4']
}";
            var mockAction = new Mock<IDynamicApiAction>();
            mockAction.SetupGet(x => x.RequestSchema).Returns(new DataSchema(model));
            SetUpContainer(false, true);
            var datacontainer = new DynamicApiDataContainer() { baseApiAction = mockAction.Object };
            UnityContainer.RegisterInstance(datacontainer);

            var target = new ApiHelper();
            var result = target.ValidateWithRequestModel(contents);

            var err = result.Single();
            err.error_code.Is(ErrorCodeMessage.Code.E10402.ToString());
            err.errors.Count.Is(4);
        }

        private class TestClass
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }
    }
}
