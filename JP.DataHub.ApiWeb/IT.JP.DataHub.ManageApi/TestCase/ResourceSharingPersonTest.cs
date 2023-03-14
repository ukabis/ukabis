using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
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
    public partial class ResourceSharingPersonTest : ManageApiTestCase
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
        [TestMethod]
        public void ResourceSharingPerson_NormalScenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IResourceSharingPersonApi>();
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();

            const string apiUrl = "/API/IntegratedTest/ManageResourceSharingPerson/Test";

            var dynamicApi = client.GetWebApiResponseResult(manageDynamicApi.GetApiResourceFromUrl(apiUrl, true)).Assert(GetExpectStatusCodes).Result;
            if (dynamicApi?.ApiId != null)
            {
                var list = client.GetWebApiResponseResult(api.GetResourceSharingListByResourcePath(ResourceSharingTestApiPath)).Assert(GetExpectStatusCodes);

                var listDelete = list.Result.Where(x => ResourceSharingRuleNameRegData.Contains(x.ResourceSharingRuleName) == true || ResourceSharingRuleNameUpdData.Contains(x.ResourceSharingRuleName) == true)
                    .Select(x => x.ResourceSharingPersonRuleId).ToList();
                listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteResourceSharingPerson(x)).Assert(DeleteExpectStatusCodes));
            }
            string apiId;
            {
                // Api
                CleanUpApiByUrl(client, apiUrl, true);
                var baseData = CreateRequestModel();
                baseData.Url = apiUrl;
                apiId = client.GetWebApiResponseResult(manageDynamicApi.RegisterApi(baseData)).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;

            }


            //通常の登録
            //新規登録
            var regobj = ResourceSharingRegData;
            var regData = client.GetWebApiResponseResult(api.RegisterResourceSharingPerson(regobj)).Assert(RegisterSuccessExpectStatusCode).Result;
            var regId = regData.ResourceSharingPersonRuleId;
            var getRegData = client.GetWebApiResponseResult(api.GetResourceSharingPerson(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 更新
            var updObj = ResourceSharingUpdData;
            updObj.ResourceSharingPersonRuleId = regId;

            var updData = client.GetWebApiResponseResult(api.UpdateResourceSharingPerson(updObj)).Assert(RegisterSuccessExpectStatusCode).Result;
            var getUpdData = client.GetWebApiResponseResult(api.GetResourceSharingPerson(regId)).Assert(GetSuccessExpectStatusCode).Result;
            getUpdData.IsStructuralEqual(updData);

            //一覧取得の確認
            var getListData = client.GetWebApiResponseResult(api.GetResourceSharingListByResourcePath(ResourceSharingTestApiPath)).Assert(GetSuccessExpectStatusCode).Result;
            getListData.Single().IsStructuralEqual(getUpdData);

            //削除
            client.GetWebApiResponseResult(api.DeleteResourceSharingPerson(regId)).Assert(DeleteSuccessStatusCode);

            client.GetWebApiResponseResult(api.GetResourceSharingPerson(regId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.UpdateResourceSharingPerson(updObj)).Assert(BadRequestStatusCode);

            //個人対ベンダー向けの登録
            //新規登録
            regobj = ResourceSharingPersonToVenderRegData(_vendorId, _systemId);
            regData = client.GetWebApiResponseResult(api.RegisterResourceSharingPerson(regobj)).Assert(RegisterSuccessExpectStatusCode).Result;
            regId = regData.ResourceSharingPersonRuleId;
            getRegData = client.GetWebApiResponseResult(api.GetResourceSharingPerson(regId)).Assert(GetSuccessExpectStatusCode).Result;

            //更新
            updObj = new ResourceSharingPersonModel()
            {
                ResourceSharingPersonRuleId = regId,
                ResourceSharingRuleName = "upd",
                SharingFromUserId = null,
                SharingFromMailAddress = "*",
                SharingToUserId = null,
                SharingToMailAddress = "*",
                ResourcePath = ResourceSharingTestApiPath,
                SharingToVendorId = _vendorId,
                SharingToSystemId = _systemId,
                Query = "newquery",
                Script = "newscript",
            };

            updData = client.GetWebApiResponseResult(api.UpdateResourceSharingPerson(updObj)).Assert(RegisterSuccessExpectStatusCode).Result;
            getUpdData = client.GetWebApiResponseResult(api.GetResourceSharingPerson(regId)).Assert(GetSuccessExpectStatusCode).Result;
            getUpdData.IsStructuralEqual(updData);

            //削除
            client.GetWebApiResponseResult(api.DeleteResourceSharingPerson(regId)).Assert(DeleteSuccessStatusCode);

            client.GetWebApiResponseResult(api.GetResourceSharingPerson(regId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.UpdateResourceSharingPerson(updObj)).Assert(BadRequestStatusCode);

            //後処理
            CleanUpApiByUrl(client,ResourceSharingTestApiPath, true);
        }

        public string ResourceSharingRuleNameRegData = "---itRegister---";
        public string ResourceSharingRuleNameUpdData = "---itUpdate---";
        const string ResourceSharingTestApiPath = "/API/IntegratedTest/ManageResourceSharingPerson/Test";

        /// <summary>
        /// データ共有正常系データ
        /// </summary>
        public RegisterResourceSharingPersonModel ResourceSharingRegData
        {
            get =>
                new RegisterResourceSharingPersonModel()
                {
                    SharingFromUserId = Guid.NewGuid().ToString(),
                    SharingFromMailAddress = "hoge@example.com",
                    SharingToUserId = Guid.NewGuid().ToString(),
                    SharingToMailAddress = "fuga@example.com",
                    ResourcePath = ResourceSharingTestApiPath,
                    ResourceSharingRuleName = ResourceSharingRuleNameRegData,
                    Query = "query",
                    Script = "script",
                };
        }

        public ResourceSharingPersonModel ResourceSharingUpdData
        {
            get =>
                new ResourceSharingPersonModel()
                {
                    ResourceSharingPersonRuleId = Guid.NewGuid().ToString(),//登録時のIDを代入する
                    SharingFromUserId = Guid.NewGuid().ToString(),
                    SharingFromMailAddress = "hogeupd@example.com",
                    SharingToUserId = Guid.NewGuid().ToString(),
                    SharingToMailAddress = "fugaupd@example.com",
                    ResourcePath = ResourceSharingTestApiPath,
                    ResourceSharingRuleName = ResourceSharingRuleNameUpdData,
                    Query = "upd_query",
                    Script = "upd_script",
                };
        }


        /// <summary>
        /// データ共有対ベンダー共有正常系データ
        /// </summary>
        public RegisterResourceSharingPersonModel ResourceSharingPersonToVenderRegData(string vendorId, string systemId)
        {
            return new RegisterResourceSharingPersonModel()
            {
                SharingFromUserId = null,
                SharingFromMailAddress = "*",
                SharingToUserId = null,
                SharingToMailAddress = "*",
                ResourcePath = ResourceSharingTestApiPath,
                ResourceSharingRuleName = ResourceSharingRuleNameRegData,
                SharingToVendorId = _vendorId,
                SharingToSystemId = _systemId,
                Query = "query",
                Script = "script",
            };
        }
    }
}
