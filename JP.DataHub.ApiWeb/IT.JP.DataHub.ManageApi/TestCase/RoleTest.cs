using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class RoleTest : ManageApiTestCase
    {
        private const string registerRoleName = "--itRegister--";
        private const string updateRoleName = "--itUpdate--";
        private static IList<string> s_listTestTitleName = new List<string>() { registerRoleName, updateRoleName };

        #region TestData

        private RoleModel _registerData = new RoleModel()
        {
            RoleId = Guid.NewGuid().ToString(),
            RoleName = registerRoleName
        };

        private RoleModel _updateData = new RoleModel()
        {
            RoleId = Guid.NewGuid().ToString(),
            RoleName = registerRoleName
        };

        private RoleModel _roleValidationBaseModel = new RoleModel()
        {
            RoleId = Guid.NewGuid().ToString(),
            RoleName = "Rolename"
        };

        /// <summary>
        /// 権限-一覧異常系データ(RegisterRole)
        /// </summary>
        private List<RoleModel> RegisterRoleValidationErrorData
        {
            get
            {
                // Nameがnull
                var roleNullModel = new RoleModel()
                {
                    RoleId = Guid.NewGuid().ToString(),
                    RoleName = null
                };

                // Nameが100桁を超える
                var roleMaxLengthOverModel = new RoleModel()
                {
                    RoleId = Guid.NewGuid().ToString(),
                    RoleName =
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "_"
                };

                return new List<RoleModel>()
                {
                    roleNullModel,
                    roleMaxLengthOverModel
                };
            }
        }

        /// <summary>
        /// 権限-一覧異常系データ(RegisterRole)
        /// </summary>
        private List<RoleModel> _registerRoleExistsErrorData = new List<RoleModel>()
        {
            new RoleModel()
            {
                RoleId = Guid.NewGuid().ToString(),
                RoleName = "システム管理者"
            }
        };

        /// <summary>
        /// 権限-一覧異常系データ(UpdateRole)
        /// </summary>
        private List<RoleModel> UpdateRoleValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = RegisterRoleValidationErrorData.DeepCopy();

                // RoleIdがnull
                var roleIdNullModel = new RoleModel()
                {
                    RoleId = null,
                    RoleName = "Rolename"
                };

                // RoleIdがGuidでない
                var roleIdNotGuidModel = new RoleModel()
                {
                    RoleId = "aaa",
                    RoleName = "Rolename"
                };

                baseModel.Add(roleIdNullModel);
                baseModel.Add(roleIdNotGuidModel);

                return baseModel;
            }
        }

        /// <summary>
        /// 権限-一覧異常系データ(UpdateRole)
        /// </summary>
        private List<RoleModel> _updateRoleExistsErrorData = new List<RoleModel>()
        {
            new RoleModel()
            {
                RoleId = Guid.NewGuid().ToString(),
                RoleName = "システム管理者"
            }
        };

        /// <summary>
        /// 権限-一覧異常系データ(UpdateRole)
        /// </summary>
        private List<RoleModel> _updateRoleNotFoundErrorData = new List<RoleModel>(){
            new RoleModel()
            {
                RoleId = Guid.NewGuid().ToString(),
                RoleName = "RoleNotFound"
            }
        };

        /// <summary>
        /// 権限-一覧異常系データ(DeleteRole)
        /// </summary>
        private List<string> _deleteRoleValidationErrorData = new List<string>()
        {
            // RoleIdがない。
            null,
            // RoleIdがGuidでない。
            "hogehoge"
        };

        /// <summary>
        /// 権限-一覧異常系データ(GetRole)
        /// </summary>
        private List<string> _getRoleValidationErrorData = new List<string>()
        {
            // RoleIdがない。
            null,
            // RoleIdがGuidでない。
            "hogehoge"
        };

        #endregion

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void Role_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var role = UnityCore.Resolve<IRoleApi>();

            #region テストデータ削除
            // 【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var listRole = client.GetWebApiResponseResult(role.GetRoleList()).Assert(GetExpectStatusCodes);

            var listDelete = listRole.Result.Where(x => s_listTestTitleName.Contains(x.RoleName) == true).Select(x => x.RoleId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(role.DeleteRole(x)).Assert(DeleteExpectStatusCodes));
            #endregion

            // 新規登録
            var regRole = client.GetWebApiResponseResult(role.RegisterRole(_registerData)).Assert(RegisterSuccessExpectStatusCode).Result;
            var regId = regRole.RoleId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(role.GetRole(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 更新データ
            _updateData.RoleId = regId;
            var updRole = client.GetWebApiResponseResult(role.UpdateRole(_updateData)).Assert(RegisterSuccessExpectStatusCode).Result;
            var updId = updRole.RoleId;

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(role.GetRole(updId)).Assert(GetSuccessExpectStatusCode).Result;

            // 一覧取得
            client.GetWebApiResponseResult(role.GetRoleList()).Assert(GetSuccessExpectStatusCode);

            // 削除
            client.GetWebApiResponseResult(role.DeleteRole(regId)).Assert(DeleteExpectStatusCodes);

            // 削除したものを再度削除(NotFound)
            client.GetWebApiResponseResult(role.DeleteRole(regId)).Assert(NotFoundStatusCode);

            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.RoleId.IsStructuralEqual(getUpdData.RoleId);
        }

        [TestMethod]
        public void Role_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var role = UnityCore.Resolve<IRoleApi>();

            // GetRoleのValidationError
            _getRoleValidationErrorData.ForEach(x =>
                client.Request(role.GetRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // GetRoleのNotFound
            client.Request(role.GetRole(Guid.NewGuid().ToString())).StatusCode.Is(NotFoundStatusCode);

            // GetRoleListのValidationError
            // Roleテーブルの全レコードを削除する必要があるため、ITでは省略

            // RegisterRoleのValidationError
            RegisterRoleValidationErrorData.ForEach(x =>
                client.Request(role.RegisterRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // RegisterRoleのNullBody
            client.Request(role.RegisterRole(null)).StatusCode.Is(BadRequestStatusCode);

            // RegisterRoleのExistsRoleName(BadRequest)
            _registerRoleExistsErrorData.ForEach(x =>
                client.Request(role.RegisterRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // UpdateRoleのValidationError
            UpdateRoleValidationErrorData.ForEach(x =>
                client.Request(role.UpdateRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // UpdateRoleのNullBody
            client.Request(role.UpdateRole(null)).StatusCode.Is(BadRequestStatusCode);

            // UpdateRoleのExistsRoleName(BadRequest)
            _updateRoleExistsErrorData.ForEach(x =>
                client.Request(role.UpdateRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // UpdateRoleのNotFound
            _updateRoleNotFoundErrorData.ForEach(x =>
                client.Request(role.UpdateRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // DeleteRoleのValidationError
            _deleteRoleValidationErrorData.ForEach(x =>
                client.Request(role.DeleteRole(x)).StatusCode.Is(BadRequestStatusCode)
            );

            // DeleteRoleのNotFound
            client.Request(role.DeleteRole(Guid.NewGuid().ToString())).StatusCode.Is(NotFoundStatusCode);
        }
    }
}
