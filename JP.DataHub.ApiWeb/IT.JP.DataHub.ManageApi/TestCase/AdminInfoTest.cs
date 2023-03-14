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
    public partial class AdminInfoTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        /// <summary>
        /// 権限-管理画面の正常系テスト
        /// </summary>
        [TestMethod]
        public void AdminInfo_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAdminInfoApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var testRoleData = RoleRegData; // 新規登録するデータ
            testRoleData.RoleName = TestDataTitle;

            var list = client.GetWebApiResponseResult(roleApi.GetRoleList()).Assert(GetExpectStatusCodes).Result;

            var listDelete = list.Where(x => TestDataTitle.Contains(x.RoleName) == true).Select(x => x.RoleId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(roleApi.DeleteRole(x)).Assert(DeleteExpectStatusCodes));

            // 権限-管理画面の一覧取得ができること
            var adminList = client.GetWebApiResponseResult(api.GetAdminInfoList()).Assert(GetSuccessExpectStatusCode).Result;

            // 【準備】一覧から１番目の要素を抜き出す。
            var firstAdminFuncId = adminList.First().AdminFuncInfoList.First().AdminFuncId;

            // 【準備】Roleを新規登録
            var testRoleId = client.GetWebApiResponseResult(roleApi.RegisterRole(testRoleData)).Assert(RegisterSuccessExpectStatusCode).Result.RoleId;

            // 権限-管理画面の新規登録
            var regObj = AdminInfoRegData;
            regObj.RoleId = testRoleId; // 準備したRoleIdを代入
            regObj.AdminFuncId = firstAdminFuncId; // 準備したAdminFuncIdを代入
            var regId = client.GetWebApiResponseResult(api.RegisterAdminInfo(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.AdminFuncRoleId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetAdminInfo(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 権限-管理画面の更新
            var updObj = AdminFuncInfoUpdData;
            updObj.AdminFuncRoleId = regId;
            //isRead = false, isWrite= trueに更新

            var updId = client.GetWebApiResponseResult(api.UpdateAdminInfo(updObj)).Assert(RegisterSuccessExpectStatusCode).Result.AdminFuncRoleId;

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetAdminInfo(updId)).Assert(GetSuccessExpectStatusCode).Result;

            // 削除
            client.GetWebApiResponseResult(api.DeleteAdminInfo(regId)).Assert(DeleteExpectStatusCodes);

            // 削除したものを再度削除（NotFound）
            client.GetWebApiResponseResult(api.DeleteAdminInfo(regId)).Assert(NotFoundStatusCode);

            // 新規登録したものと更新登録したもののIDが同じか
            getRegData.AdminFuncRoleId.IsStructuralEqual(getUpdData.AdminFuncRoleId);

            //【後処理】作成した権限を削除
            client.GetWebApiResponseResult(roleApi.DeleteRole(testRoleId)).Assert(DeleteExpectStatusCodes);

        }

        /// <summary>
        /// 権限-管理画面の異常系テスト
        /// </summary>
        [TestMethod]
        public void AdminInfo_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAdminInfoApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();

            // GetAdminInfoのValidationError
            GetAdminInfoValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetAdminInfo(x)).Assert(BadRequestStatusCode)
            );

            // GetAdminInfoのNotFound(適当なGuidで検索)
            client.GetWebApiResponseResult(api.GetAdminInfo(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // GetAdminInfoListのNotFound
            // 全レコードを削除する必要があるため、ITでは省略

            // RegistrationAdminFuncInformationのValidationError
            RegisterAdminInfoValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterAdminInfo(x)).Assert(BadRequestStatusCode)
            );

            // RegisterAdminInfoのInternalError(FK Error)
            RegisterAdminInfoFKErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterAdminInfo(x)).Assert(BadRequestStatusCode)
            );

            // RegisterAdminInfoのNullBody
            client.GetWebApiResponseResult(api.RegisterAdminInfo(null)).Assert(BadRequestStatusCode);

            // UpdateAdminInfoのValidationError
            UpdateAdminInfoValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateAdminInfo(x)).Assert(BadRequestStatusCode)
            );

            // UpdateAdminInfoのNotFound
            UpdateAdminInfoNotFoundErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateAdminInfo(x)).Assert(BadRequestStatusCode)
            );

            // UpdateAdminInfoのNullBody
            client.GetWebApiResponseResult(api.UpdateAdminInfo(null)).Assert(BadRequestStatusCode);

            // DeleteAdminInfo のValidationError
            DeleteAdminInfoValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteAdminInfo(x)).Assert(BadRequestStatusCode)
            );

            // DeleteAdminInfo のNotFound(適当なGuidで削除)
            client.GetWebApiResponseResult(api.DeleteAdminInfo(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);
        }
        #region Data

        public string TestDataTitle = "---itRegister-AdminInfo---";

        /// <summary>
        /// 権限-管理画面正常系データ
        /// </summary>
        public AdminInfoModel AdminInfoRegData
        {
            get =>
                new AdminInfoModel()
                {
                    AdminFuncId = Guid.Empty.ToString(),// テストの中でAdminFuncの一覧を検索し、割り当てる。
                    RoleId = Guid.Empty.ToString(), // テストの中で権限を作成し、割り当てる。
                    IsRead = bool.TrueString,
                    IsWrite = bool.FalseString,
                };
        }
        public AdminInfoModel AdminFuncInfoUpdData
        {
            get =>
                new AdminInfoModel()
                {
                    AdminFuncRoleId = Guid.Empty.ToString(),
                    IsRead = bool.FalseString,
                    IsWrite = bool.TrueString,
                };
        }
        public string RegisterRoleName = "---itRegister---";
        public string UpdateRoleName = "---itUpdate---";
        /// <summary>
        /// 権限-一覧正常系データ
        /// </summary>
        public RoleModel RoleRegData
        {
            get =>
                new RoleModel()
                {
                    RoleId = Guid.Empty.ToString(),
                    RoleName = RegisterRoleName,
                };
        }

        #endregion

        #region Validation/Error
        /// <summary>
        /// 権限-管理画面異常系データ(RegisterAdminInfo)
        /// Validation Error
        /// </summary>
        public List<AdminInfoModel> RegisterAdminInfoValidationErrorData
        {
            get
            {
                // AdminFuncId がnull
                var AdminFuncIdNullModel = DeepCopy(AdminInfoRegData);
                AdminFuncIdNullModel.AdminFuncId = null;

                // AdminFuncId がGuidでない
                var AdminFuncIdNotGuidModel = DeepCopy(AdminInfoRegData);
                AdminFuncIdNotGuidModel.AdminFuncId = "aaa";

                // RoleId がnull
                var RoleIdNullModel = DeepCopy(AdminInfoRegData);
                RoleIdNullModel.RoleId = null;

                // RoleId がGuidでない
                var RoleIdNotGuidModel = DeepCopy(AdminInfoRegData);
                RoleIdNotGuidModel.RoleId = "aaa";

                // IsRead がnull
                var IsReadNullModel = DeepCopy(AdminInfoRegData);
                IsReadNullModel.IsRead = null;

                // IsWrite がnull
                var IsWriteNullModel = DeepCopy(AdminInfoRegData);
                IsWriteNullModel.IsWrite = null;


                return new List<AdminInfoModel>()
                {
                    AdminFuncIdNullModel,
                    AdminFuncIdNotGuidModel,
                    RoleIdNullModel,
                    RoleIdNotGuidModel,
                    IsReadNullModel,
                    IsWriteNullModel
                };
            }
        }

        /// <summary>
        /// 権限-管理画面異常系データ(RegisterAdminInfo)
        /// FK Error
        /// </summary>
        public List<AdminInfoModel> RegisterAdminInfoFKErrorData
        {
            get
            {
                // AdminFuncId がAdminFuncに存在しない
                var AdminFuncIdFKErrorModel = DeepCopy(AdminInfoRegData);
                AdminFuncIdFKErrorModel.AdminFuncId = Guid.Empty.ToString();


                // RoleId がAdminFuncに存在しない
                var RoleIdNotGuidModel = DeepCopy(AdminInfoRegData);
                RoleIdNotGuidModel.RoleId = Guid.Empty.ToString();


                return new List<AdminInfoModel>()
                {
                    AdminFuncIdFKErrorModel,
                    RoleIdNotGuidModel,
                };
            }
        }


        /// <summary>
        /// 権限-管理画面異常系データ(UpdateAdminInfo)
        /// Validation Error
        /// </summary>
        public List<AdminInfoModel> UpdateAdminInfoValidationErrorData

        {
            get
            {
                // AdminFuncRoleId がnull
                var AdminFuncRoleIdIdNullModel = DeepCopy(AdminFuncInfoUpdData);
                AdminFuncRoleIdIdNullModel.AdminFuncRoleId = null;

                // AdminFuncId がGuidでない
                var AdminFuncRoleIdNotGuidModel = DeepCopy(AdminFuncInfoUpdData);
                AdminFuncRoleIdNotGuidModel.AdminFuncRoleId = "aaa";

                // IsRead がnull
                var IsReadNullModel = DeepCopy(AdminFuncInfoUpdData);
                IsReadNullModel.IsRead = null;

                // IsWrite がnull
                var IsWriteNullModel = DeepCopy(AdminFuncInfoUpdData);
                IsWriteNullModel.IsWrite = null;

                return new List<AdminInfoModel>()
                {
                    AdminFuncRoleIdIdNullModel,
                    AdminFuncRoleIdNotGuidModel,
                    IsReadNullModel,
                    IsWriteNullModel
                };
            }
        }

        /// <summary>
        /// 権限-管理画面異常系データ(UpdateAdminFuncInfo)
        /// Not Found
        /// </summary>
        public List<AdminInfoModel> UpdateAdminInfoNotFoundErrorData
        {
            get
            {
                // NotFound
                var NotFound = DeepCopy(AdminFuncInfoUpdData);
                NotFound.AdminFuncRoleId = Guid.NewGuid().ToString();
                return new List<AdminInfoModel>()
                {
                    NotFound
                };
            }
        }

        /// <summary>
        /// 権限-管理画面異常系データ(GetAdminInfo)
        /// </summary>
        public List<string> GetAdminInfoValidationErrorData
        {
            get
            {
                return new List<string>()
                {
                    // AdminFuncRoleIdがない。
                    null,
                    // AdminFuncRoleIdがGuidでない。
                    "hogehoge"
                };
            }
        }

        /// <summary>
        /// 権限-管理画面異常系データ(DeleteAdminInfo)
        /// </summary>
        public List<string> DeleteAdminInfoValidationErrorData
        {
            get
            {
                // Getと同じ
                return DeepCopy(GetAdminInfoValidationErrorData);
            }
        }

        #endregion
    }
}
