using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class CacheDeleteTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
            _vendorId = AppConfig.AdminVendorId;
            _systemId = AppConfig.AdminSystemId;
            _repositoryGroupId = AppConfig.RepositoryGroupId;
        }
        private string _vendorId;
        private string _systemId;
        private string _repositoryGroupId;

        [TestMethod]
        public void CacheDeleteCheck_NormalScenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();

            //メソッド作成後、即DynamicApiを叩き、キャッシュが消えてDynamicApiが正常に叩けることを確認
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/CacheDeleteCheck";

            // Initialize
            CleanUpApiByUrl(client ,apiUrl, true);

            // API登録
            var apiResult1 = client.GetWebApiResponseResult(manageDynamicApi.RegisterApi(DynamicApiTest.ApiDataForCrudTest(_vendorId, _systemId, apiUrl, null))).Assert(RegisterSuccessExpectStatusCode).Result;

            // メソッド登録
            var registMethod = DynamicApiTest.MethodDataForCrudTest(apiResult1.ApiId, null, _repositoryGroupId);
            client.GetWebApiResponseResult(manageDynamicApi.RegisterMethod(registMethod)).Assert(RegisterSuccessExpectStatusCode);

            //データ取得
            var dynamicApiClient = new ManageApiIntegratedTestClient("test1", "DynamicApi", true);
            var api = UnityCore.Resolve<IIntegratedTestDynamicApi>();
            dynamicApiClient.GetWebApiResponseResult(api.ManageDynamicApiCacheDeleteCheckGetAll()).Assert(NotFoundStatusCode);
        }
    }
}
