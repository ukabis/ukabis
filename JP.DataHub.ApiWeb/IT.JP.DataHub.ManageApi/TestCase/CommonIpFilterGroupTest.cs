using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
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
    public partial class CommonIpFilterGroupTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        [TestMethod]
        public void CommonIpFilterGroup_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<ICommonIpFilterGroupApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetCommonIpFilterGroupList()).Assert(GetExpectStatusCodes);

            var listDelete = list.Result.Where(x => RegisterCommonIpFilterGroupName.Contains(x.CommonIpFilterGroupName) == true 
            || UpdateCommonIpFilterGroupName.Contains(x.CommonIpFilterGroupName) == true).Select(x => x.CommonIpFilterGroupId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteCommonIpFilterGroup(x)).Assert(DeleteExpectStatusCodes));

            // 新規登録
            var regId = client.GetWebApiResponseResult(api.RegisterCommonIpFilterGroup(RegData)).Assert(RegisterSuccessExpectStatusCode).Result.CommonIpFilterGroupId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetCommonIpFilterGroup(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 更新登録
            var updData = getRegData;
            CommonIpFilterGroup_Resource_Save(updData.CommonIpFilterGroupId, client);

            updData.CommonIpFilterGroupName = UpdateCommonIpFilterGroupName;
            updData.IpList.Add(UpdIpFilter);
            var updId = client.GetWebApiResponseResult(api.UpdateCommonIpFilterGroup(updData)).Assert(RegisterSuccessExpectStatusCode).Result.CommonIpFilterGroupId;

            // 再度更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetCommonIpFilterGroup(updId)).Assert(GetSuccessExpectStatusCode).Result;

            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.CommonIpFilterGroupId.Is(getUpdData.CommonIpFilterGroupId);
            // 新規登録したものを更新登録したものが異なること（更新したため）
            getRegData.IsNotStructuralEqual(getUpdData);

            // 登録済みのIPアドレスを、新規登録扱い（CommonIpFilterId=null）で指定した場合、BadRequestになる
            client.GetWebApiResponseResult(api.UpdateCommonIpFilterGroup(updData)).Assert(BadRequestStatusCode);

            // リスト取得
            var getList = client.GetWebApiResponseResult(api.GetCommonIpFilterGroupList()).Assert(GetSuccessExpectStatusCode).Result;
            // リストの中に新規登録したデータ（登録後、更新したデータ）が存在することを確認
            var isExists = getList.Exists(x => x.CommonIpFilterGroupId == getUpdData.CommonIpFilterGroupId
                                            && x.CommonIpFilterGroupName == getUpdData.CommonIpFilterGroupName);
            Assert.IsTrue(isExists); // 新規登録したデータ（登録後、更新したデータ）が存在する

            // 削除
            client.GetWebApiResponseResult(api.DeleteCommonIpFilterGroup(regId)).Assert(DeleteExpectStatusCodes);

            // 削除したものを再度削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteCommonIpFilterGroup(regId)).Assert(NotFoundStatusCode);
            // 削除したものを取得(NotFound)
            client.GetWebApiResponseResult(api.GetCommonIpFilterGroup(regId)).Assert(NotFoundStatusCode);
            // 削除したものを更新(NotFound)
            client.GetWebApiResponseResult(api.UpdateCommonIpFilterGroup(updData)).Assert(BadRequestStatusCode);

        }

        /// <summary>
        /// 作成したIpFilterGroupがResourceに登録できるか確認
        /// </summary>
        /// <param name="commonIpFilterGroupId"></param>
        private void CommonIpFilterGroup_Resource_Save(string commonIpFilterGroupId,DynamicApiClient client)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();

            const string apiUrl = "/API/IntegratedTest/CommonIpFilterGroup/SaveValue";

            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);

            // API登録
            RegisterApiRequestModel data = new RegisterApiRequestModel()
            {
                VendorId = AppConfig.AdminVendorId,
                SystemId = AppConfig.AdminSystemId,
                Url= apiUrl,
                RepositoryKey = apiUrl,
                PartitionKey = apiUrl,
                ApiCommonIpFilterGroupList = new List<RegisterApiCommonIpFilterGroupModel>()
                { new RegisterApiCommonIpFilterGroupModel(){ CommonIpFilterGroupId = commonIpFilterGroupId, IsActive = true} }
            };
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(data)).Assert(RegisterSuccessExpectStatusCode).Result;

            // API取得
            var res = client.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiResult1.ApiId)).Assert(GetExpectStatusCodes).Result;

            Assert.AreEqual(res.CommonIpFilterGroupList.First().CommonIpFilterGroupId, commonIpFilterGroupId);
        }

        [TestMethod]
        public void CommonIpFilterGroup_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<ICommonIpFilterGroupApi>();
            // RegisterのValidationError
            RegisterValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterCommonIpFilterGroup(x)).Assert(BadRequestStatusCode)
            );

            // RegisterのNullBody
            client.GetWebApiResponseResult(api.RegisterCommonIpFilterGroup(null)).Assert(BadRequestStatusCode);

            // UpdateのValidationError
            UpdateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateCommonIpFilterGroup(x)).Assert(BadRequestStatusCode)
            );

            // UpdateのNullBoyd
            client.GetWebApiResponseResult(api.UpdateCommonIpFilterGroup(null)).Assert(BadRequestStatusCode);

            // DeleteのValidationError
            DeleteValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteCommonIpFilterGroup(x.CommonIpFilterGroupId)).Assert(BadRequestStatusCode)
            );

            // GetのValidationError
            GetValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetCommonIpFilterGroup(x.CommonIpFilterGroupId)).Assert(BadRequestStatusCode)
            );
        }
        #region Data

        /// <summary>
        /// 正常系データ
        /// </summary>
        public string RegisterCommonIpFilterGroupName = "--itRegister--";
        public string UpdateCommonIpFilterGroupName = "--itUpdate--";
        public CommonIpFilterGroupModel RegData
        {
            get => new CommonIpFilterGroupModel()
            {
                CommonIpFilterGroupId = null,
                CommonIpFilterGroupName = RegisterCommonIpFilterGroupName,
                IpList = new List<CommonIpFilterModel>()
                    {
                        new CommonIpFilterModel()
                        {
                            CommonIpFilterId = null,
                            IpAddress = "127.0.0.1/32",
                            IsActive = false,
                            IsEnable = false
                        }
                    }
            };
        }

        public CommonIpFilterModel UpdIpFilter
        {
            get => new CommonIpFilterModel()
            {
                CommonIpFilterId = null, //追加するためnull
                IpAddress = "127.0.0.2/32",
                IsActive = true,
                IsEnable = false
            };

        }

        #endregion

        #region Validation

        private CommonIpFilterGroupModel ValidationBaseModel = new CommonIpFilterGroupModel()
        {
            CommonIpFilterGroupId = Guid.NewGuid().ToString(),
            CommonIpFilterGroupName = "hoge_Name",
            IpList = new List<CommonIpFilterModel>()
                    {
                        new CommonIpFilterModel()
                        {
                            CommonIpFilterId = Guid.NewGuid().ToString(),
                            IpAddress = "127.0.0.1/32",
                            IsActive = false,
                            IsEnable = false
                        },
                        new CommonIpFilterModel()
                        {
                            CommonIpFilterId = Guid.NewGuid().ToString(),
                            IpAddress = "127.0.0.2/32",
                            IsActive = false,
                            IsEnable = false
                        }
                    }
        };

        /// <summary>
        /// 異常系データ(Register)
        /// </summary>
        public List<CommonIpFilterGroupModel> RegisterValidationErrorData
        {
            get
            {
                // CommonIpFilterGroupNameがNull
                var nameNullModel = DeepCopy(ValidationBaseModel);
                nameNullModel.CommonIpFilterGroupName = null;

                // CommonIpFilterGroupNameが100文字オーバー
                var nameOverModel = DeepCopy(ValidationBaseModel);
                nameOverModel.CommonIpFilterGroupName = new string('a', 101);

                // IpListがNull
                var ipListNullModel = DeepCopy(ValidationBaseModel);
                ipListNullModel.IpList = null;

                // ipAddressがNull
                var ipAddressNullModel = DeepCopy(ValidationBaseModel);
                ipAddressNullModel.IpList[0].IpAddress = null;

                //ipAddressの形式エラー
                var ipAddressFormatErrorModel = DeepCopy(ValidationBaseModel);
                ipAddressFormatErrorModel.IpList[0].IpAddress = "hoge";

                //ipAddressの重複エラー
                var ipAddressDuplicateModel = DeepCopy(ValidationBaseModel);
                ipAddressDuplicateModel.IpList.Add(ipAddressDuplicateModel.IpList[0]);
                ipAddressDuplicateModel.IpList.Add(ipAddressDuplicateModel.IpList[1]);

                return new List<CommonIpFilterGroupModel>()
                {
                    nameNullModel,
                    nameOverModel,
                    ipListNullModel,
                    ipAddressNullModel,
                    ipAddressFormatErrorModel,
                    ipAddressDuplicateModel
                };
            }
        }

        /// <summary>
        /// 異常系データ(Update)
        /// </summary>
        public List<CommonIpFilterGroupModel> UpdateValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = DeepCopy(RegisterValidationErrorData);

                return baseModel;
            }
        }

        /// <summary>
        /// 異常系データ(Delete)
        /// </summary>
        public List<CommonIpFilterGroupModel> DeleteValidationErrorData
        {
            get
            {
                // CommonIpFilterGroupIdがnull
                var commonIpFilterGroupIdNullModel = DeepCopy(ValidationBaseModel);
                commonIpFilterGroupIdNullModel.CommonIpFilterGroupId = null;

                return new List<CommonIpFilterGroupModel>()
                {
                    commonIpFilterGroupIdNullModel
                };
            }
        }

        /// <summary>
        /// 異常系データ(Get)
        /// </summary>
        public List<CommonIpFilterGroupModel> GetValidationErrorData
        {
            get
            {
                // 削除と同じ
                var baseModel = DeepCopy(DeleteValidationErrorData);
                return baseModel;
            }
        }

        #endregion

    }
}
