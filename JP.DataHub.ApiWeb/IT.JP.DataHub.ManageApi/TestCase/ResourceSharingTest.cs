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
    public partial class ResourceSharingTest : ManageApiTestCase
    {

        private string _vendorId;
        private string _systemId;

        [Serializable]
        public class GetListParams
        {
            public string VendorId { get; set; }
            public string SystemId { get; set; }
            public string ApiId { get; set; }
        }
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
            _vendorId = AppConfig.AdminVendorId;
            _systemId = AppConfig.AdminSystemId;
        }
        /// <summary>
        /// データ共有の正常系テスト
        /// </summary>
        [TestMethod]
        public void ResourceSharing_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IResourceSharingApi>();
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageResourceSharing/Test";

            #region 準備
            var dynamicApi = client.GetWebApiResponseResult(manageDynamicApi.GetApiResourceFromUrl(apiUrl, true)).Assert(GetExpectStatusCodes).Result;
            if (dynamicApi?.ApiId != null)
            {
                var list = client.GetWebApiResponseResult(api.GetResourceSharingList(_vendorId, _systemId, dynamicApi.ApiId)).Assert(GetExpectStatusCodes);

                var listDelete = list.Result.Where(x => ResourceSharingRuleNameRegData.Contains(x.ResourceSharingRuleName) == true || ResourceSharingRuleNameUpdData.Contains(x.ResourceSharingRuleName) == true)
                    .Select(x => x.ResourceSharingRuleId).ToList();
                listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteResourceSharing(x)).Assert(DeleteExpectStatusCodes));
            }
            string apiId;
            {
                // Api
                CleanUpApiByUrl(client, apiUrl, true);
                var baseData = CreateRequestModel();
                baseData.Url = apiUrl;
                apiId = client.GetWebApiResponseResult(manageDynamicApi.RegisterApi(baseData)).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;

            }
            #endregion
            // 新規登録
            var regObj = ResourceSharingRegData;
            regObj.SharingFromVendorId = _vendorId;
            regObj.SharingFromSystemId = _systemId;
            regObj.ApiId = apiId;
            regObj.SharingToVendorId = _vendorId;
            regObj.SharingToSystemId = _systemId;
            var regId = client.GetWebApiResponseResult(api.RegisterResourceSharing(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.ResourceSharingRuleId;

            #region RegisterResourceSharing:Error系
            //SharingFromVendorId FkError
            {
                var fkData = ResourceSharingRegData;
                fkData.SharingFromVendorId = Guid.NewGuid().ToString(); //target
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.RegisterResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //SharingFromSystemId FkError
            {
                var fkData = ResourceSharingRegData;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = Guid.NewGuid().ToString(); //target
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.RegisterResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //ApiId FkError
            {
                var fkData = ResourceSharingRegData;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = Guid.NewGuid().ToString(); //target
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.RegisterResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //SharingToVendorId FkError
            {
                var fkData = ResourceSharingRegData;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = Guid.NewGuid().ToString(); //target
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.RegisterResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //SharingToSystemId FkError
            {
                var fkData = ResourceSharingRegData;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = Guid.NewGuid().ToString(); //target
                client.GetWebApiResponseResult(api.RegisterResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            #endregion

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetResourceSharing(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 更新
            var updObj = ResourceSharingUpdData;
            updObj.ResourceSharingRuleId = regId;
            updObj.SharingFromVendorId = _vendorId;
            updObj.SharingFromSystemId = _systemId;
            updObj.ApiId = apiId;
            updObj.SharingToVendorId = _vendorId;
            updObj.SharingToSystemId = _systemId;
            var updData = client.GetWebApiResponseResult(api.UpdateResourceSharing(updObj)).Assert(RegisterSuccessExpectStatusCode).Result.ResourceSharingRuleId;

            #region UpdateResourceSharing：Error系
            //SharingFromVendorId FkError
            {
                var fkData = ResourceSharingUpdData;
                fkData.ResourceSharingRuleId = regId;
                fkData.SharingFromVendorId = Guid.NewGuid().ToString(); //target
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.UpdateResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //SharingFromSystemId FkError
            {
                var fkData = ResourceSharingUpdData;
                fkData.ResourceSharingRuleId = regId;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = Guid.NewGuid().ToString(); //target
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.UpdateResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //ApiId FkError
            {
                var fkData = ResourceSharingUpdData;
                fkData.ResourceSharingRuleId = regId;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = Guid.NewGuid().ToString(); //target
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.UpdateResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //SharingToVendorId FkError
            {
                var fkData = ResourceSharingUpdData;
                fkData.ResourceSharingRuleId = regId;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = Guid.NewGuid().ToString(); //target
                fkData.SharingToSystemId = _systemId;
                client.GetWebApiResponseResult(api.UpdateResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            //SharingToSystemId FkError
            {
                var fkData = ResourceSharingUpdData;
                fkData.ResourceSharingRuleId = regId;
                fkData.SharingFromVendorId = _vendorId;
                fkData.SharingFromSystemId = _systemId;
                fkData.ApiId = apiId;
                fkData.SharingToVendorId = _vendorId;
                fkData.SharingToSystemId = Guid.NewGuid().ToString(); //target
                client.GetWebApiResponseResult(api.UpdateResourceSharing(fkData)).Assert(BadRequestStatusCode);
            }
            #endregion

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetResourceSharing(regId)).Assert(GetSuccessExpectStatusCode).Result;


            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.ResourceSharingRuleId.IsStructuralEqual(getUpdData.ResourceSharingRuleId);
            // 新規登録したものを更新登録したものが異なること（更新したため）
            getRegData.IsNotStructuralEqual(getUpdData);

            // 一覧取得ができること
            var rlist = client.GetWebApiResponseResult(api.GetResourceSharingList(_vendorId, _systemId, apiId)).Assert(GetSuccessExpectStatusCode).Result;

            // リストの中に新規登録したデータ（登録後、更新したデータ）が存在することを確認
            var isExists = rlist.Exists(x => x.ResourceSharingRuleId == getUpdData.ResourceSharingRuleId
                                                           && x.ResourceSharingRuleName == getUpdData.ResourceSharingRuleName);
            Assert.IsTrue(isExists); // 新規登録したデータ（登録後、更新したデータ）が存在する

            // 削除
            client.GetWebApiResponseResult(api.DeleteResourceSharing(regId)).Assert(DeleteSuccessStatusCode);
            // 削除したものを再度削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteResourceSharing(regId)).Assert(NotFoundStatusCode);
            // 削除したものを取得(NotFound)
            client.GetWebApiResponseResult(api.GetResourceSharing(regId)).Assert(NotFoundStatusCode);
            //// 削除したものを更新(NotFound)
            client.GetWebApiResponseResult(api.UpdateResourceSharing(updObj)).Assert(BadRequestStatusCode);
            // API
            CleanUpApiByUrl(client,apiUrl, true);
        }

        /// <summary>
        /// データ共有のエラー系テスト
        /// </summary>
        [TestMethod]
        public void ResourceSharing_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IResourceSharingApi>();

            // RegisterResourceSharingのValidationError
            RegisterResourceSharingValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterResourceSharing(x)).Assert(BadRequestStatusCode)
            );

            // RegisterResourceSharingのNullBody
            client.GetWebApiResponseResult(api.RegisterResourceSharing(null)).Assert(BadRequestStatusCode);

            // UpdateResourceSharingのValidationError
            UpdateResourceSharingValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateResourceSharing(x)).Assert(BadRequestStatusCode)
            );

            // UpdateResourceSharingのNullBody
            client.GetWebApiResponseResult(api.UpdateResourceSharing(null)).Assert(BadRequestStatusCode);

            // GetResourceSharingのValidationError
            GetResourceSharingValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetResourceSharing(x)).Assert(BadRequestStatusCode)
            );

            // GetResourceSharingListのValidationError
            GetResourceSharingListValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetResourceSharingList(x.VendorId, x.SystemId, x.ApiId)).Assert(BadRequestStatusCode)
            );

            // DeleteResourceSharingのValidationError
            DeleteResourceSharingValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteResourceSharing(x)).Assert(BadRequestStatusCode)
            );
        }

        #region Data

        public string ResourceSharingRuleNameRegData = "---itRegister---";
        public string ResourceSharingRuleNameUpdData = "---itUpdate---";

        /// <summary>
        /// データ共有正常系データ
        /// </summary>
        public ResourceSharingModel ResourceSharingRegData
        {
            get =>
                new ResourceSharingModel()
                {
                    ResourceSharingRuleId = null,//登録時は指定しない
                    SharingFromVendorId = Guid.NewGuid().ToString(),
                    SharingFromSystemId = Guid.NewGuid().ToString(),
                    ApiId = Guid.NewGuid().ToString(),
                    SharingToVendorId = Guid.NewGuid().ToString(),
                    SharingToSystemId = Guid.NewGuid().ToString(),
                    ResourceSharingRuleName = ResourceSharingRuleNameRegData,
                    Query = "query",
                    //IsEnable = bool.TrueString
                };
        }

        public ResourceSharingModel ResourceSharingUpdData
        {
            get =>
                new ResourceSharingModel()
                {
                    ResourceSharingRuleId = Guid.NewGuid().ToString(),//登録時のIDを代入する
                    SharingFromVendorId = Guid.NewGuid().ToString(),
                    SharingFromSystemId = Guid.NewGuid().ToString(),
                    ApiId = Guid.NewGuid().ToString(),
                    SharingToVendorId = Guid.NewGuid().ToString(),
                    SharingToSystemId = Guid.NewGuid().ToString(),
                    ResourceSharingRuleName = ResourceSharingRuleNameUpdData,
                    Query = "upd_query",
                    //IsEnable = bool.TrueString
                };
        }

        #endregion

        #region Validation/Error
        public string ResourceSharingNameErrorCaseRegData = "---itRegister-ErrorCase---";
        public ResourceSharingModel ResourceSharingValidationBaseModel
        {
            get =>
                new ResourceSharingModel()
                {
                    ResourceSharingRuleId = Guid.NewGuid().ToString(),//登録時のIDを代入する
                    SharingFromVendorId = Guid.NewGuid().ToString(),
                    SharingFromSystemId = Guid.NewGuid().ToString(),
                    ApiId = Guid.NewGuid().ToString(),
                    SharingToVendorId = Guid.NewGuid().ToString(),
                    SharingToSystemId = Guid.NewGuid().ToString(),
                    ResourceSharingRuleName = ResourceSharingNameErrorCaseRegData,
                    Query = "upd_query",
                    //IsEnable = null
                };
        }

        public GetListParams ResourceSharingGetListValidationBaseModel
        {
            get =>
                new GetListParams()
                {
                    VendorId = Guid.NewGuid().ToString(),
                    SystemId = Guid.NewGuid().ToString(),
                    ApiId = Guid.NewGuid().ToString()
                };
        }


        /// <summary>
        /// データ共有異常系データ(RegisterResourceSharing)
        /// </summary>
        public List<ResourceSharingModel> RegisterResourceSharingValidationErrorData
        {
            get
            {
                // SharingFromVendorId がnull
                var sharingFromVendorIdNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingFromVendorIdNullModel.SharingFromVendorId = null;

                // SharingFromVendorId がGuidでない
                var sharingFromVendorIdNotGuidModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingFromVendorIdNotGuidModel.SharingFromVendorId = "aaa";

                // SharingFromSystemId がnull
                var sharingFromSystemIdNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingFromSystemIdNullModel.SharingFromSystemId = null;

                // SharingFromSystemId がGuidでない
                var sharingFromSystemIdNotGuidModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingFromSystemIdNotGuidModel.SharingFromSystemId = "aaa";

                // ApiId がnull
                var apiIdNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                apiIdNullModel.ApiId = null;

                // ApiId がGuidでない
                var apiIdNotGuidModel = DeepCopy(ResourceSharingValidationBaseModel);
                apiIdNotGuidModel.ApiId = "aaa";

                // SharingToVendorId がnull
                var sharingToVendorIdNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingToVendorIdNullModel.SharingToVendorId = null;

                // SharingToVendorId がGuidでない
                var sharingToVendorIdNotGuidModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingToVendorIdNotGuidModel.SharingToVendorId = "aaa";

                // SharingToSystemId がnull
                var sharingToSystemIdNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingToSystemIdNullModel.SharingToSystemId = null;

                // SharingToSystemId がGuidでない
                var sharingToSystemIdNotGuidModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingToSystemIdNotGuidModel.SharingToSystemId = "aaa";

                // ResourceSharingRuleName がnull
                var resourceSharingRuleNameNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                resourceSharingRuleNameNullModel.ResourceSharingRuleName = null;

                // ResourceSharingRuleName が1000桁を超える
                var resourceSharingRuleNameMaxLengthOverModel = DeepCopy(ResourceSharingValidationBaseModel);
                resourceSharingRuleNameMaxLengthOverModel.ResourceSharingRuleName = new string('a', 1001);

                // Query がnull
                var queryNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                sharingToSystemIdNullModel.Query = null;

                return new List<ResourceSharingModel>()
                {
                    sharingFromVendorIdNullModel,
                    sharingFromVendorIdNotGuidModel,
                    sharingFromSystemIdNullModel,
                    sharingFromSystemIdNotGuidModel,
                    apiIdNullModel,
                    apiIdNotGuidModel,
                    sharingToVendorIdNullModel,
                    sharingToVendorIdNotGuidModel,
                    sharingToSystemIdNullModel,
                    sharingToSystemIdNotGuidModel,
                    resourceSharingRuleNameNullModel,
                    resourceSharingRuleNameMaxLengthOverModel,
                    queryNullModel
                };
            }
        }

        /// <summary>
        /// データ共有異常系データ(UpdateResourceSharing)
        /// </summary>
        public List<ResourceSharingModel> UpdateResourceSharingValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = DeepCopy(RegisterResourceSharingValidationErrorData);

                // ResourceSharingIdがnull
                var resourceSharingRuleIdNullModel = DeepCopy(ResourceSharingValidationBaseModel);
                resourceSharingRuleIdNullModel.ResourceSharingRuleId = null;

                // ResourceSharingIdがGuidでない
                var resourceSharingRuleIdNotGuidModel = DeepCopy(ResourceSharingValidationBaseModel);
                resourceSharingRuleIdNotGuidModel.ResourceSharingRuleId = "aaa";

                baseModel.Add(resourceSharingRuleIdNullModel);
                baseModel.Add(resourceSharingRuleIdNotGuidModel);

                return baseModel;
            }
        }

        /// <summary>
        /// データ共有異常系データ(GetResourceSharing)
        /// </summary>
        public List<string> GetResourceSharingValidationErrorData
        {
            get
            {
                return new List<string>()
                {
                    // ResourceSharingIdがない。
                    null,
                    // ResourceSharingIdがGuidでない。
                    "hogehoge"
                };
            }
        }

        /// <summary>
        /// データ共有異常系データ(DeleteResourceSharing)
        /// </summary>
        public List<string> DeleteResourceSharingValidationErrorData
        {
            get
            {
                // Getと同じ
                return DeepCopy(GetResourceSharingValidationErrorData);
            }
        }

        /// <summary>
        /// データ共有異常系データ(GetResourceSharingList)
        /// </summary>
        public List<GetListParams> GetResourceSharingListValidationErrorData
        {
            get
            {
                // VendorId がnull
                var vendorIdNullModel = DeepCopy(ResourceSharingGetListValidationBaseModel);
                vendorIdNullModel.VendorId = null;

                // VendorId がGuidでない
                var vendorIdNotGuidModel = DeepCopy(ResourceSharingGetListValidationBaseModel);
                vendorIdNotGuidModel.VendorId = "aaa";

                // SystemId がnull
                var systemIdNullModel = DeepCopy(ResourceSharingGetListValidationBaseModel);
                systemIdNullModel.SystemId = null;

                // SystemId がGuidでない
                var systemIdNotGuidModel = DeepCopy(ResourceSharingGetListValidationBaseModel);
                systemIdNotGuidModel.SystemId = "aaa";

                // ApiId がnull
                var apiIdNullModel = DeepCopy(ResourceSharingGetListValidationBaseModel);
                apiIdNullModel.ApiId = null;

                // ApiId がGuidでない
                var apiIdNotGuidModel = DeepCopy(ResourceSharingGetListValidationBaseModel);
                apiIdNotGuidModel.ApiId = "aaa";

                return new List<GetListParams>()
                {
                    vendorIdNullModel,
                    vendorIdNotGuidModel,
                    systemIdNullModel,
                    systemIdNotGuidModel,
                    apiIdNullModel,
                    apiIdNotGuidModel
                };
            }
        }

        #endregion

    }
}
