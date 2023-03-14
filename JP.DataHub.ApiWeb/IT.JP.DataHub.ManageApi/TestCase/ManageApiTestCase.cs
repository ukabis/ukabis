using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Unity;
using Unity.Lifetime;
using Unity.Interception;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.ManageApi.Config;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using JP.DataHub.Com.Net.Http;
using IT.JP.DataHub.ManageApi.WebApi;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    public class ManageApiTestCase : ItTestCaseBase
    {
        //protected TestConfig TestConfig = null;
        protected AppConfig AppConfig = null;

        public override void TestInitialize(bool isUnityInitialize = true, ITypeLifetimeManager typeLifetimeManager = null, IConfiguration configuration = null)
        {
            base.TestInitialize(isUnityInitialize, typeLifetimeManager, configuration);
            //TestConfig = TestConfiguration.GetSection("Environment").Get<TestConfig>();
            AppConfig = UnityCore.Resolve<IConfiguration>().Get<AppConfig>();
        }
        // API削除(URL指定)
        protected void CleanUpApiByUrl(DynamicApiClient client,string apiUrl, bool cascade = false)
        {
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();
            var api = client.GetWebApiResponseResult(manageDynamicApi.GetApiResourceFromUrl(apiUrl, true)).Assert(GetExpectStatusCodes).Result;

            if (string.IsNullOrEmpty(api.ApiId))
            {
                return;
            }

            // API削除
            CleanUpApiById(api.ApiId, client);

            if (!cascade)
            {
                return;
            }

            // メソッド削除
            foreach (var method in api.MethodList)
            {
                CleanUpMethodById(method.MethodId, client);
            }

            // スキーマ削除
            if (Guid.TryParse(api.SystemId, out _))
            {
                CleanUpSchemaById(api.SystemId, client);
            }
        }
        // API削除(ID指定)
        private void CleanUpApiById(string apiId,DynamicApiClient client)
        {
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(manageDynamicApi.DeleteApi(apiId)).Assert(DeleteExpectStatusCodes);
        }
        // メソッド削除(ID指定)
        private void CleanUpMethodById(string methodId, DynamicApiClient client)
        {
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(manageDynamicApi.DeleteMethod(methodId)).Assert(DeleteExpectStatusCodes);
        }
        // スキーマ削除(ID指定)
        private void CleanUpSchemaById(string schemaId, DynamicApiClient client)
        {
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(manageDynamicApi.DeleteSchema(schemaId)).Assert(DeleteExpectStatusCodes);
        }

        public static T DeepCopy<T>(T target)
        {
            T result;
            BinaryFormatter b = new BinaryFormatter();
            MemoryStream mem = new MemoryStream();

            try
            {
                b.Serialize(mem, target);
                mem.Position = 0;
                result = (T)b.Deserialize(mem);
            }
            finally
            {
                mem.Close();
            }

            return result;
        }
        static public string _defaultUrl = "/API/IntegratedTest/ManageDynamicApi01";

        static public RegisterApiRequestModel CreateRequestModel(
    string vendorId = null, string systemId = null, string apiName = null, string url = null, string apiDescriptiveText = null, bool isVendor = false, bool isPerson = false,
    bool isEnable = true, string modelId = null, string repositoryKey = null, string partitionKey = null,
    bool isStaticApi = false, List<RegisterApiTagModel> apiTagInfoList = null, List<RegisterApiCategoryModel> categoryList = null,
    List<RegisterApiFieldModel> apiFieldInfoList = null, bool isData = false, bool isBusinessLogic = false,
    bool isPay = false, string feeDescription = null, string resourceCreateUser = null, string resourceMaintainer = null,
    string resourceCreateDate = null, string resourceLatestDate = null, string updateFrequency = null, bool isContract = false,
    string contactInformation = null, string version = null, string agreeDescription = null, bool isVisibleAgreement = false,
    DynamicApiAttachFileSettingsModel attachFileSettings = null, DocumentHistorySettingsModel documentHistorySettings = null,
    bool isEnableBlockchain = false, bool isOptimisticConcurrency = false, bool isEnableIpFilter = false, List<RegisterApiCommonIpFilterGroupModel> apiCommonIpFilterGroupList = null,
    List<RegisterApiIpFilterModel> apiIpFilterList = null, List<RegisterResourceOpenIdCaModel> openIdCaList = null,
    bool isUseBlobCache = false, bool? isEnableResourceVersion = null)
        {
            return new RegisterApiRequestModel
            {
                VendorId = string.IsNullOrEmpty(vendorId) ? UnityCore.Resolve<IConfiguration>().Get<AppConfig>().AdminVendorId : vendorId,
                SystemId = string.IsNullOrEmpty(systemId) ? UnityCore.Resolve<IConfiguration>().Get<AppConfig>().AdminSystemId : systemId,
                ApiName = string.IsNullOrEmpty(apiName) ? _defaultUrl : apiName,
                Url = string.IsNullOrEmpty(url) ? _defaultUrl : url,
                ApiDescriptiveText = apiDescriptiveText,
                IsVendor = isVendor,
                IsPerson = isPerson,
                IsEnable = isEnable,
                ModelId = modelId,
                RepositoryKey = repositoryKey,
                PartitionKey = partitionKey,
                IsStaticApi = isStaticApi,
                ApiTagInfoList = apiTagInfoList,
                CategoryList = categoryList,
                ApiFieldInfoList = apiFieldInfoList,
                IsData = isData,
                IsBusinessLogic = isBusinessLogic,
                IsPay = isPay,
                FeeDescription = feeDescription,
                ResourceCreateUser = resourceCreateUser,
                ResourceMaintainer = resourceMaintainer,
                ResourceCreateDate = resourceCreateDate,
                ResourceLatestDate = resourceLatestDate,
                UpdateFrequency = updateFrequency,
                IsContract = isContract,
                ContactInformation = contactInformation,
                Version = version,
                AgreeDescription = agreeDescription,
                IsVisibleAgreement = isVisibleAgreement,
                AttachFileSettings = attachFileSettings,
                DocumentHistorySettings = documentHistorySettings,
                IsEnableBlockchain = isEnableBlockchain,
                IsOptimisticConcurrency = isOptimisticConcurrency,
                IsEnableIpFilter = isEnableIpFilter,
                ApiCommonIpFilterGroupList = apiCommonIpFilterGroupList,
                ApiIpFilterList = apiIpFilterList,
                OpenIdCaList = openIdCaList,
                IsUseBlobCache = isUseBlobCache,
                IsEnableResourceVersion = isEnableResourceVersion ?? true
            };
        }
    }
}
