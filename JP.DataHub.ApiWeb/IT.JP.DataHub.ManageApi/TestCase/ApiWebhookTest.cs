using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.ApiWebhook;
using IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class ApiWebhookTest : ManageApiTestCase
    {
        private string _vendorId;
        private string _systemId;


        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
            _vendorId = AppConfig.AdminVendorId;
            _systemId = AppConfig.AdminSystemId;
        }

        /// <summary>
        /// Webhookの正常系テスト
        /// </summary>
        [TestMethod]
        public void Webhook_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiWebhookApi>();
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var apiWebhookTestUrl = "http://integratedtest.jpdatahub.jp/apiwebhook/";
            var apiWebhookList = client.GetWebApiResponseResult(api.GetApiWebhookList(_vendorId)).Assert(GetSuccessExpectStatusCode).Result;

            apiWebhookList?
                .Where(x => x.Url.ToString() == apiWebhookTestUrl).ToList()
                .ForEach(x => client.GetWebApiResponseResult(api.DeleteApiWebhook(x.ApiWebhookId)).Assert(DeleteSuccessStatusCode));

            // 【準備】テストで使用するDynamicApiが存在しないことを確認し、存在するなら消す。
            var apiTestUrl = "/API/IntegratedTest/ApiWebhook";
            var registeredApi = client.GetWebApiResponseResult(manageDynamicApi.GetApiResourceFromUrl(apiTestUrl, false)).Assert(GetExpectStatusCodes).Result;
            if (registeredApi != null && !string.IsNullOrEmpty(registeredApi.ApiId))
            {
                client.GetWebApiResponseResult(manageDynamicApi.DeleteApi(registeredApi.ApiId)).Assert(DeleteSuccessStatusCode);
            }

            // 【準備】テストで使用するDynamicApiを作成する。
            string testApiData = TestApiData;
            var testApiId = client.GetWebApiResponseResult(manageDynamicApi.RegisterApi(JsonConvert.DeserializeObject<RegisterApiRequestModel>(string.Format(testApiData, _vendorId, _systemId, apiTestUrl)))).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;

            // 新規登録
            var regId = client.GetWebApiResponseResult(api.RegisterApiWebhook(ApiWebhookData(testApiId, _vendorId, apiWebhookTestUrl))).Assert(RegisterSuccessExpectStatusCode).Result.ApiWebhookId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetApiWebhook(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 異常系テストだが登録済のApiIdが必要なのでここで実施する。
            // 既にWebhookがある場合のBadRequestError
            client.GetWebApiResponseResult(api.RegisterApiWebhook(ApiWebhookData(testApiId, _vendorId, apiWebhookTestUrl))).Assert(BadRequestStatusCode);

            // 更新登録
            var updId = client.GetWebApiResponseResult(api.UpdateApiWebhook(UpdateApiWebhookData(regId, testApiId, _vendorId, apiWebhookTestUrl))).Assert(RegisterSuccessExpectStatusCode).Result.ApiWebhookId;

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetApiWebhook(updId)).Assert(GetSuccessExpectStatusCode).Result;

            // 異常系テストだが登録済のApiIdが必要なのでここで実施する。
            // ApiWebhookIdが存在しない場合のNotFoundError
            client.GetWebApiResponseResult(api.UpdateApiWebhook(UpdateApiWebhookData(Guid.NewGuid().ToString(), testApiId, _vendorId, apiWebhookTestUrl))).Assert(BadRequestStatusCode);

            // 新規登録したものと更新登録したものが同じか
            getRegData.ToString().IsStructuralEqual(getUpdData.ToString());

            // 全件取得
            var getListData = client.GetWebApiResponseResult(api.GetApiWebhookList(_vendorId)).Assert(GetSuccessExpectStatusCode).Result;

            // 登録したものが含まれているか
            var hasRegData = false;
            foreach (var item in getListData)
            {
                if (getRegData.ToString().Equals(item.ToString()))
                {
                    hasRegData = true;
                    break;
                }
            }
            Assert.IsTrue(hasRegData);

            // 削除
            client.GetWebApiResponseResult(api.DeleteApiWebhook(regId)).Assert(DeleteSuccessStatusCode);

            // 削除したものを取得（NotFound）
            client.GetWebApiResponseResult(api.GetApiWebhook(regId)).Assert(NotFoundStatusCode);

            // テスト用に作成したAPIを削除する。
            client.GetWebApiResponseResult(manageDynamicApi.DeleteApi(testApiId)).Assert(DeleteSuccessStatusCode);
        }

        /// <summary>
        /// Webhookの異常系テスト
        /// </summary>
        [TestMethod]
        public void Webhook_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiWebhookApi>();
            var apiWebhookTestUrl = "http://integratedtest.jpdatahub.jp/apiwebhook/";

            // RegisterApiWebhookのValidationError
            client.GetWebApiResponseResult(api.RegisterApiWebhook(ApiWebhookData(null, null, null))).Assert(BadRequestStatusCode);

            // RegisterApiWebhookのNullBody
            client.GetWebApiResponseResult(api.RegisterApiWebhook(null)).Assert(BadRequestStatusCode);

            // UpdateApiWebhookのValidationError
            client.GetWebApiResponseResult(api.UpdateApiWebhook(UpdateApiWebhookData(null, null, null, null))).Assert(BadRequestStatusCode);

            // UpdateApiWebhookのNullBody
            client.GetWebApiResponseResult(api.UpdateApiWebhook(null)).Assert(BadRequestStatusCode);

            // DeleteWebhookのValidationError
            client.GetWebApiResponseResult(api.DeleteApiWebhook("hoge")).Assert(BadRequestStatusCode);

            // GetWebhookのValidationError
            client.GetWebApiResponseResult(api.GetApiWebhook("hoge")).Assert(BadRequestStatusCode);

            // GetWebhookListのValidationError
            client.GetWebApiResponseResult(api.GetApiWebhookList("hoge")).Assert(BadRequestStatusCode);

            // DeleteApiWebhookのNotFoundError
            client.GetWebApiResponseResult(api.DeleteApiWebhook(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // GetApiWebhookのNotFoundError
            client.GetWebApiResponseResult(api.GetApiWebhook(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // RegisterApiWebhookのVendorIdが存在しない場合のBadRequestError
            client.GetWebApiResponseResult(api.RegisterApiWebhook(ApiWebhookData(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), apiWebhookTestUrl))).Assert(BadRequestStatusCode);

            // RegisterApiWebhookのApiIdが存在しない場合のBadRequestError
            client.GetWebApiResponseResult(api.RegisterApiWebhook(ApiWebhookData(Guid.NewGuid().ToString(), _vendorId, apiWebhookTestUrl))).Assert(BadRequestStatusCode);

            // UpdateApiWebhookのVendorIdが存在しない場合のBadRequestError
            client.GetWebApiResponseResult(api.UpdateApiWebhook(UpdateApiWebhookData(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), apiWebhookTestUrl))).Assert(BadRequestStatusCode);

            // UpdateApiWebhookのApiIdが存在しない場合のBadRequestError
            client.GetWebApiResponseResult(api.UpdateApiWebhook(UpdateApiWebhookData(string.Empty, Guid.NewGuid().ToString(), _vendorId, apiWebhookTestUrl))).Assert(BadRequestStatusCode);
        }
        #region Data

        public ApiWebhookRegisterModel ApiWebhookData(string apiId, string vendorId, string url)
        {
            return JsonConvert.DeserializeObject<ApiWebhookRegisterModel>(string.Format(_apiWebhookData, string.Empty, apiId, vendorId, url));
        }
        public ApiWebhookUpdateModel UpdateApiWebhookData(string id, string apiId, string vendorId, string url)
        {
            return JsonConvert.DeserializeObject<ApiWebhookUpdateModel>(string.Format(_apiWebhookData, id, apiId, vendorId, url));
        }
        /// <summary>
        /// Webhook正常系データ
        /// </summary>
        public string _apiWebhookData = @"
{{
  'ApiWebhookId': '{0}',
  'ApiId': '{1}',
  'VendorId': '{2}',
  'Url': '{3}',
  'Headers': [
    {{
      'FieldName': 'field1', 
      'Value': 'value1'
    }}
  ],
  'NotifyRegister': true,
  'NotifyUpdate': true,
  'NotifyDelete': true
}}";


        /// <summary>
        /// Webhookテスト用APIデータ
        /// </summary>
        public string TestApiData = @"
{{
  'VendorId': '{0}',
  'SystemId': '{1}',
  'ApiName': '{2}',
  'Url': '{2}',
  'ApiDescriptiveText': null,
  'ApiTagInfoList': null,
  'IsVendor': false,
  'IsEnable': true,
  'ModelId': null,
  'IsStaticApi': false,
  'CategoryList': null,
  'ApiFieldInfoList': null,
  'RepositoryKey': '{2}',
  'PartitionKey': '{2}',
  'IsData': true,
  'IsBusinessLogic': false,
  'IsPay': false,
  'FeeDescription': null,
  'ResourceCreateUser': null,
  'ResourceMaintainer': null,
  'ResourceCreateDate': null,
  'ResourceLatestDate': null,
  'UpdateFrequency': null,
  'IsContract': false,
  'ContactInformation': null,
  'Version': null,
  'AgreeDescription': null,
  'IsPerson': false,
  'ApiCommonIpFilterGroupList': null,
  'ApiIpFilterList': null
}}";

        #endregion
    }
}
