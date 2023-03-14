using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using Unity;
using JP.DataHub.Com.Cache;
using JP.DataHub.Com.DataContainer;
using JP.DataHub.Com.DDD;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Core.DataContainer;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.DataStoreRepository;
using JP.DataHub.UnitTest.Com;

namespace UnitTest.JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions
{
    [TestClass]
    public class UnitTest_ODataRawDataAction : UnitTestBase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);

            UnityContainer.RegisterInstance(new Mock<ICache>().Object);
            UnityContainer.RegisterInstance<IDataContainer>(new PerRequestDataContainer());
            UnityContainer.RegisterType<IHttpContextAccessor, HttpContextAccessor>();
        }

        [TestMethod]
        public void ExecuteAction_異常系_非運用ベンダー()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Guid.NewGuid().ToString();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);

            var action = CreateODataRawDataAction();
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.POST);

            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.Forbidden);
        }

        [TestMethod]
        public void ExecuteAction_リポジトリ1つ_データ有()
        {
            var perRequestDataContainer = new PerRequestDataContainer();
            perRequestDataContainer.VendorId = Configuration.GetSection("AppConfig:OperatingVendorVendorId").Get<string[]>().First();
            UnityContainer.RegisterInstance<IPerRequestDataContainer>(perRequestDataContainer);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentHistory", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentReference", true);
            UnityContainer.RegisterInstance<bool>("EnableJsonDocumentKeepRegDate", true);

            var action = CreateODataRawDataAction();
            var exceptionResult = new List<JsonDocument>
            {
                new JsonDocument(new JObject
                {
                    ["field"] = "value"
                })
            };
            XResponseContinuation responseContinuation = null;
            var mockRepository = CreateINewDynamicApiDataStoreRepositoryMock(exceptionResult, ValueObjectUtil.Create<QueryParam>(action), responseContinuation);
            action.DynamicApiDataStoreRepository = new ReadOnlyCollection<INewDynamicApiDataStoreRepository>(new List<INewDynamicApiDataStoreRepository>
            {
                mockRepository.Object
            });
            var result = action.ExecuteAction();
            result.StatusCode.Is(HttpStatusCode.OK);
            result.Content.ReadAsStringAsync().Result.Is(exceptionResult[0].Value.ToString());
            result.Headers.Contains("X-ResponseContinuation").Is(false);

            // 引数はCallbackで判定
            mockRepository.Verify(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation), Times.Exactly(1));
        }


        private Mock<INewDynamicApiDataStoreRepository> CreateINewDynamicApiDataStoreRepositoryMock(
            List<JsonDocument> jsonDocuments, QueryParam queryParam,
            XResponseContinuation responseContinuation, string returnresponseContinuation = null,
            Exception exception = null)
        {
            var mockRepository = new Mock<INewDynamicApiDataStoreRepository>();
            if (exception == null)
            {
                mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                    .Callback((QueryParam para, out XResponseContinuation continuation) =>
                    {
                        continuation = null;
                        if (returnresponseContinuation != null)
                        {
                            continuation = new XResponseContinuation(returnresponseContinuation);
                        }
                        if (queryParam != null)
                        {
                            para.IsNotStructuralEqual(queryParam);
                        }
                    })
                    .Returns(jsonDocuments);
            }
            else
            {
                mockRepository.Setup(x => x.Query(It.IsAny<QueryParam>(), out responseContinuation))
                    .Callback((QueryParam para, out XResponseContinuation continuation) =>
                    {
                        continuation = null;
                        if (responseContinuation != null)
                        {
                            continuation = new XResponseContinuation(returnresponseContinuation);
                        }

                        if (queryParam != null)
                        {
                            para.IsNotStructuralEqual(queryParam);
                        }
                    })
                    .Throws(exception);
            }
            return mockRepository;
        }

        private ODataAction CreateODataRawDataAction(bool isHistoryTest = false, string designationRepokey = null)
        {
            ODataRawDataAction action = UnityCore.Resolve<ODataRawDataAction>();
            action.ApiId = new ApiId(Guid.NewGuid().ToString());
            action.ControllerId = new ControllerId(Guid.NewGuid().ToString());
            action.SystemId = new SystemId(Guid.NewGuid().ToString());
            action.VendorId = new VendorId(Guid.NewGuid().ToString());
            action.MethodType = new HttpMethodType(HttpMethodType.MethodTypeEnum.GET);
            action.RepositoryKey = new RepositoryKey("/API/Private/QueryTest/{Id}");
            action.ControllerRelativeUrl = new ControllerUrl("/API/Private/QueryTest");
            action.RepositoryKey = new RepositoryKey(string.IsNullOrEmpty(designationRepokey) ? "/API/Private/QueryTest/{Id}" : designationRepokey);
            action.ActionType = new ActionTypeVO(ActionType.OData);
            action.ActionTypeVersion = new ActionTypeVersion(1);
            action.MediaType = new MediaType("application/json");
            action.CacheInfo = new CacheInfo(false, 0, "");
            action.XGetInnerAllField = new XGetInnerField(false);
            if (isHistoryTest)
            {
                action.IsDocumentHistory = new IsDocumentHistory(true);
                action.HistoryEvacuationDataStoreRepository = new Mock<INewDynamicApiDataStoreRepository>().Object;
            }
            action.PostDataType = new PostDataType("");
            action.ApiQuery = new ApiQuery("");
            action.Accept = new Accept("*/*");
            return action;
        }
    }
}
