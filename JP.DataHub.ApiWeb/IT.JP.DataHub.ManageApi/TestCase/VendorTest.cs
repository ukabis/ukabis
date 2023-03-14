using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models.Vendor;
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
    public partial class VendorTest : ManageApiTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// ベンダーの正常系テスト
        /// </summary>
        [TestMethod]
        public void Vendor_NormalScenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IVendorApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetList()).Assert(GetExpectStatusCodes);

            var listDelete = list.Result.Where(x => RegisterVendorName.Contains(x.VendorName) == true
            || UpdateVendorName.Contains(x.VendorName) == true).Select(x => x.VendorId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.Delete(x)).Assert(DeleteExpectStatusCodes));


            // ベンダー新規登録
            var regDataResult = client.GetWebApiResponseResult(api.Register(RegisterVendorData)).Assert(RegisterSuccessExpectStatusCode).Result;
            var regVendorId = regDataResult.VendorId;

            // ベンダー更新登録
            var updData = UpdateVendorData;
            updData.VendorId = regVendorId;
            var updDataResult = client.GetWebApiResponseResult(api.Update(updData)).Assert(RegisterSuccessExpectStatusCode).Result;

            // スタッフのテスト
            VendorStaff_NormalScenario(client, regVendorId);

            // ベンダーリンクのテスト
            VendorLink_NormalScenario(client, regVendorId);

            //// 添付ファイルのテスト
            VendorAttachFile_NormalScenario(client, regVendorId);

            // ベンダーOpenId認証局のテスト
            VendorOpenIdCa_NormalScenario(client, regVendorId);

            if (AppConfig.AllowMailTest)
            {
                // 招待のテスト
                this.Invitation_NormalScenario(client, regVendorId);
            }

            // 削除
            client.GetWebApiResponseResult(api.Delete(regVendorId)).Assert(DeleteExpectStatusCodes);

            // 削除後NotFound
            client.GetWebApiResponseResult(api.GetVendor(regVendorId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.Update(updData)).Assert(BadRequestStatusCode);
        }

        /// <summary>
        /// ベンダーの異常系テスト
        /// </summary>
        [TestMethod]
        public void Vendor_ErrorScenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IVendorApi>();

            string regVendorId1 = string.Empty;
            string regVendorId2 = string.Empty;

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetList()).Assert(GetExpectStatusCodes);
            var listDelete = list.Result.Where(x => RegisterVendorErrorName1.Contains(x.VendorName) == true
            || RegisterVendorErrorName2.Contains(x.VendorName) == true).Select(x => x.VendorId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.Delete(x)).Assert(DeleteExpectStatusCodes));

            // 【準備】必要になるベンダーを登録
            var regDataResult1 = client.GetWebApiResponseResult(api.Register(RegisterVendorValidationErrorData1)).Assert(RegisterSuccessExpectStatusCode).Result;
            regVendorId1 = regDataResult1.VendorId;
            var regDataResult2 = client.GetWebApiResponseResult(api.Register(RegisterVendorValidationErrorData2)).Assert(RegisterSuccessExpectStatusCode).Result;
            regVendorId2 = regDataResult2.VendorId;

            // ベンダー新規登録時のValidation
            RegisterVendorValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.Register(x)).Assert(BadRequestStatusCode)
            );

            // ベンダー新規登録時のNullBody
            client.GetWebApiResponseResult(api.Register(null)).Assert(BadRequestStatusCode);

            // ベンダー更新登録時のValidation
            UpdateVendorValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.Update(x)).Assert(BadRequestStatusCode)
            );

            // ベンダー更新登録時のNullBody
            client.GetWebApiResponseResult(api.Update(null)).Assert(BadRequestStatusCode);

            // ベンダー名重複(新規)
            client.GetWebApiResponseResult(api.Register(RegisterVendorValidationErrorData1)).Assert(BadRequestStatusCode);
            // ベンダー名重複(更新・ベンダー2にベンダー1の名前で更新)
            var updData = new UpdateVendorModel() { VendorId = regVendorId2, VendorName = RegisterVendorErrorName1 };
            client.GetWebApiResponseResult(api.Update(updData)).Assert(BadRequestStatusCode);

            // スタッフの異常系テスト
            VendorStaff_ErrorScenario(client,regVendorId1);

            // ベンダーリンクの異常系テスト
            this.VendorLink_ErrorScenario(client,regVendorId1);

            //// 添付ファイルの異常系テスト
            this.VendorAttachFile_ErrorScenario(client, regVendorId1);

            // ベンダーOpenId認証局の異常系テスト
            this.VendorOpenIdCa_ErrorScenario(client, regVendorId1);

            //// 招待の異常系テスト
            this.Invitation_ErrorScenario(client,regVendorId1);

            client.GetWebApiResponseResult(api.Delete(regVendorId1)).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(api.Delete(regVendorId2)).Assert(DeleteExpectStatusCodes);
        }

        /// <summary>
        /// ベンダーのスタッフの正常系テスト
        /// </summary>
        /// <param name="regVendorId"></param>
        public void VendorStaff_NormalScenario(DynamicApiClient client,string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();
            //ロール一覧取得
            var roleList = client.GetWebApiResponseResult(roleApi.GetRoleList()).Assert(GetSuccessExpectStatusCode).Result;

            // スタッフ新規追加
            var addStaffAccount = Guid.NewGuid().ToString();
            var addStaffData = new RegisterStafforModel() { VendorId = regVendorId, Account = addStaffAccount, RoleId = roleList.FirstOrDefault()?.RoleId };
            client.GetWebApiResponseResult(api.AddStaff(addStaffData)).Assert(RegisterSuccessExpectStatusCode);

            // スタッフ一覧取得
            var staffList = client.GetWebApiResponseResult(api.GetStaffList(regVendorId)).Assert(GetSuccessExpectStatusCode).Result;
            var staff = staffList.First();
            staff.Account.Is(addStaffAccount);

            // スタッフ更新
            var updateStaffData = new UpdateStaffModel()
            {
                VendorId = regVendorId,
                StaffId = staff.StaffId,
                Account = addStaffAccount,
                EmailAddress = "itEmail@exsample.com",
                RoleId = roleList.LastOrDefault().RoleId
            };
            var updResult = client.GetWebApiResponseResult(api.UpdateStaff(updateStaffData)).Assert(RegisterSuccessExpectStatusCode).Result;

            // スタッフ取得・更新確認
            var getStaff = client.GetWebApiResponseResult(api.GetStaff(staff.StaffId)).Assert(GetSuccessExpectStatusCode).Result;
            getStaff.EmailAddress.Is(updateStaffData.EmailAddress);

            // スタッフ削除
            client.GetWebApiResponseResult(api.DeleteStaff(updateStaffData.StaffId)).Assert(DeleteSuccessStatusCode);

            // 削除したスタッフの更新・取得・削除(NotFound)
            client.GetWebApiResponseResult(api.GetStaff(updateStaffData.StaffId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.UpdateStaff(updateStaffData)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteStaff(updateStaffData.StaffId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// ベンダーのスタッフの異常系テスト
        /// </summary>
        /// <param name="regVendorId"></param>
        public void VendorStaff_ErrorScenario(DynamicApiClient client,string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();
            //ロール一覧取得
            var roleList = client.GetWebApiResponseResult(roleApi.GetRoleList()).Assert(GetSuccessExpectStatusCode).Result;

            // スタッフ新規登録のValidation
            AddStaffValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.AddStaff(x)).Assert(BadRequestStatusCode)
            );

            // スタッフ新規登録のNullBody
            client.GetWebApiResponseResult(api.AddStaff(null)).Assert(BadRequestStatusCode);

            // スタッフ新規登録時にベンダーが存在しない
            var addStaffData = new RegisterStafforModel() { VendorId = Guid.Empty.ToString(), Account = Guid.NewGuid().ToString() };
            client.GetWebApiResponseResult(api.AddStaff(addStaffData)).Assert(BadRequestStatusCode);

            // スタッフ新規登録時にアカウントの形式がGUIDでない
            var invalidFormatStaffData = new RegisterStafforModel() { VendorId = regVendorId, Account = "foobar" };
            client.GetWebApiResponseResult(api.AddStaff(invalidFormatStaffData)).Assert(BadRequestStatusCode);
            var invalidFormatStaffDataUpdate = new UpdateStaffModel() { VendorId = regVendorId, Account = "foobar" };
            client.GetWebApiResponseResult(api.UpdateStaff(invalidFormatStaffDataUpdate)).Assert(BadRequestStatusCode);

            // スタッフ更新登録のValidation
            UpdateStaffValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateStaff(x)).Assert(BadRequestStatusCode)
            );
            // 更新時のベンダー存在チェックは先にスタッフの存在チェックが走るためスタッフのNotFoundになる(正常系で確認)

            // スタッフ更新登録のNullBody
            client.GetWebApiResponseResult(api.UpdateStaff(null)).Assert(BadRequestStatusCode);

            // スタッフ新規追加
            var addStaffAccount = Guid.NewGuid().ToString();
            addStaffData = new RegisterStafforModel() { VendorId = regVendorId, Account = addStaffAccount, RoleId = roleList.FirstOrDefault()?.RoleId };
            client.GetWebApiResponseResult(api.AddStaff(addStaffData)).Assert(RegisterSuccessExpectStatusCode);

            // スタッフ一覧取得
            var staffList = client.GetWebApiResponseResult(api.GetStaffList(regVendorId)).Assert(GetSuccessExpectStatusCode).Result;
            var staff = staffList.First();

            // スタッフ更新
            var updateStaffData = new UpdateStaffModel()
            {
                VendorId = regVendorId,
                StaffId = staff.StaffId,
                Account = "foobar",
                EmailAddress = "itEmail@exsample.com"
            };

            // スタッフ更新登録のアカウントの形式がGUIDでない
            client.GetWebApiResponseResult(api.UpdateStaff(updateStaffData)).Assert(BadRequestStatusCode);
        }

        /// <summary>
        /// ベンダーリンクの正常系テスト
        /// </summary>
        /// <param name="regVendorId"></param>
        public void VendorLink_NormalScenario(DynamicApiClient client,string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();

            // 新規登録
            var regResult = client.GetWebApiResponseResult(api.RegisterVendorLink(RegisterVendorLinkData(regVendorId))).Assert(RegisterSuccessExpectStatusCode).Result;
            var regVendorLinkId = regResult.First().VendorLinkId;
            // 更新登録
            var updData = RegisterVendorLinkData(regVendorId, regVendorLinkId);
            //updData[0].VendorLinkId = regVendorLinkId;
            updData[0].LinkTitle = "itUpdVendorLink";
            client.GetWebApiResponseResult(api.RegisterVendorLink(updData)).Assert(RegisterSuccessExpectStatusCode);

            //1件 取得
            var getResult = client.GetWebApiResponseResult(api.GetVendorLink(regVendorLinkId)).Assert(GetSuccessExpectStatusCode).Result;
            getResult.LinkTitle.IsStructuralEqual(updData[0].LinkTitle);

            // 一覧取得
            var getResultList = client.GetWebApiResponseResult(api.GetVendorLinkList(regVendorId)).Assert(GetSuccessExpectStatusCode).Result;
            getResultList.Where(x => x.VendorLinkId == regVendorLinkId).Count().Is(1);

            // 削除
            client.GetWebApiResponseResult(api.DeleteVendorLink(regVendorLinkId)).Assert(DeleteSuccessStatusCode);

            // 削除したベンダーリンクの取得・削除(NotFound)
            client.GetWebApiResponseResult(api.GetVendorLink(regVendorLinkId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetVendorLinkList(regVendorId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.DeleteVendorLink(regVendorLinkId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// ベンダーリンクの異常系テスト
        /// </summary>
        public void VendorLink_ErrorScenario(DynamicApiClient client,string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();

            // ベンダーリンク新規及び更新登録ValidationError
            RegisterVendorLinkValidationErrorData(regVendorId).ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterVendorLink(x)).Assert(BadRequestStatusCode)
            );

            // ベンダーリンク新規登録及び更新登録NullBody
            client.GetWebApiResponseResult(api.RegisterVendorLink(null)).Assert(BadRequestStatusCode);

            // ベンダーリンク削除ValidationError
            client.GetWebApiResponseResult(api.DeleteVendorLink(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteVendorLink("hoge")).Assert(BadRequestStatusCode);
        }


        /// <summary>
        /// ベンダーの添付ファイル正常系テスト
        /// </summary>
        public void VendorAttachFile_NormalScenario(DynamicApiClient client,string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();

            // 使用可能な添付ファイル一覧を取得する
            var attachFileList = client.GetWebApiResponseResult(api.GetAttachFileList()).Assert(GetExpectStatusCodes).Result;
            if (attachFileList == null || !attachFileList.Any())
            {
                // 存在しない環境の場合はテストしない
                return;
            }
            var attachFileId = attachFileList[0].AttachFileStorageId;

            // 新規登録
            var regResult = client.GetWebApiResponseResult(api.RegisterAttachFile(regVendorId, attachFileId)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 1件取得
            var getResult = client.GetWebApiResponseResult(api.GetAttachFile(regVendorId)).Assert(GetExpectStatusCodes).Result;

            // 削除
            client.GetWebApiResponseResult(api.DeleteAttachFile(regVendorId)).Assert(DeleteSuccessStatusCode);

            // 削除したベンダーリンクの取得・削除(NotFound)
            client.GetWebApiResponseResult(api.GetAttachFile(regVendorId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.DeleteAttachFile(regVendorId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// ベンダーの添付ファイル異常系テスト
        /// </summary>
        public void VendorAttachFile_ErrorScenario(DynamicApiClient client,string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();

            // 使用可能な添付ファイル一覧を取得する
            var attachFileList = client.GetWebApiResponseResult(api.GetAttachFileList()).Assert(GetExpectStatusCodes).Result;
            if (attachFileList == null || !attachFileList.Any())
            {
                // 存在しない環境の場合はテストしない
                return;
            }

            // 添付ファイルの新規及び更新登録ValidationError
            RegisterAttachFileValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterAttachFile(x.VendorId, x.AttachFileId)).Assert(BadRequestStatusCode)
            );

            // 添付ファイル削除ValidationError
            DeleteAttachFileValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteAttachFile(x)).Assert(BadRequestStatusCode)
            );
        }


        /// <summary>
        /// ベンダーOpenId認証局の正常系テスト
        /// </summary>
        /// <param name="regVendorId"></param>
        public void VendorOpenIdCa_NormalScenario(DynamicApiClient client, string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();
            var dapi = UnityCore.Resolve<IDynamicApiApi>();

            var openIdCaList = client.GetWebApiResponseResult(dapi.GetOpenIdCaList()).Assert(GetExpectStatusCodes).Result;
            if (openIdCaList == null || !openIdCaList.Any())
            {
                // 存在しない環境の場合はテストしない
                return;
            }
            var appId = openIdCaList[0].ApplicationId;

            // 新規登録
            var regModel = new List<RegisterOpenIdCaModel>() { new RegisterOpenIdCaModel { VendorId = regVendorId, AccessControl = "alw", ApplicationId = appId } };
            var regResult = client.GetWebApiResponseResult(api.RegisterVendorOpenIdCa(regModel)).Assert(RegisterExpectStatusCodes).Result;
            var regOpenIdCaId = regResult[0].VendorOpenidCaId;
            regResult[0].AccessControl.Is("alw");

            // 更新登録
            var updModel = new List<RegisterOpenIdCaModel>() { new RegisterOpenIdCaModel { VendorId = regVendorId, AccessControl = "inh", ApplicationId = appId, VendorOpenidCaId = regOpenIdCaId } };
            var updResult = client.GetWebApiResponseResult(api.RegisterVendorOpenIdCa(updModel)).Assert(RegisterExpectStatusCodes).Result;
            updResult[0].AccessControl.Is("inh");

            // 取得
            var getList = client.GetWebApiResponseResult(api.GetVendorOpenIdCa(regVendorId)).Assert(GetExpectStatusCodes).Result;
            getList.Where(x => !string.IsNullOrEmpty(x.VendorOpenidCaId) && x.VendorOpenidCaId == regOpenIdCaId)
                .Count().Is(1);

            // 削除
            client.GetWebApiResponseResult(api.DeleteVendorOpenIdCa(regVendorId, regOpenIdCaId)).Assert(DeleteSuccessStatusCode);

            // 削除したもの削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteVendorOpenIdCa(regVendorId, regOpenIdCaId)).Assert(NotFoundStatusCode);

            // 一覧取得に含まれるがIdはNull
            client.GetWebApiResponseResult(api.GetVendorOpenIdCa(regVendorId)).Assert(GetExpectStatusCodes).Result
                .Where(x => x.ApplicationId == appId).FirstOrDefault().VendorOpenidCaId.IsNullOrEmpty();
        }

        /// <summary>
        /// ベンダーOpenId認証局の異常系テスト
        /// </summary>
        public void VendorOpenIdCa_ErrorScenario(DynamicApiClient client, string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();

            var openIdCaList = client.GetWebApiResponseResult(api.GetVendorOpenIdCa(regVendorId)).Assert(GetExpectStatusCodes).Result;
            if (openIdCaList == null || !openIdCaList.Any())
            {
                // 存在しない環境の場合はテストしない
                return;
            }

            // ベンダーOpenId認証局登録及び更新ValidationError
            RegisterVendorOpenIdCaValidationErrorData(regVendorId).ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterVendorOpenIdCa(x)).Assert(BadRequestStatusCode)
            );

            // ベンダーOpenId認証局登録及び更新NullBody
            client.GetWebApiResponseResult(api.RegisterVendorOpenIdCa(null)).Assert(BadRequestStatusCode);

            // ベンダーOpenId認証局削除ValidationError
            DeleteVendorOpenIdCaValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteVendorOpenIdCa(x.VendorId, x.VendorOpenIdCaId)).Assert(BadRequestStatusCode)
            );
        }


        public void Invitation_NormalScenario(DynamicApiClient client, string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();

            var mailAddress = AppConfig.MailTestAddressTo;
            // 適当なRoleIdを引いてくる
            var roleId = client.GetWebApiResponseResult(roleApi.GetRoleList()).Assert(GetExpectStatusCodes).Result.First().RoleId;

            // SendInvitation(メールが正常に受信までされたかは確認できない)
            var body = new SendInvitationModel
            {
                VendorId = regVendorId,
                RoleId = roleId,
                MailAddress = mailAddress
            };
            var userInvitaionId = client.GetWebApiResponseResult(api.SendInvitation(body)).Assert(RegisterSuccessExpectStatusCode).Result;

        }

        public void Invitation_ErrorScenario(DynamicApiClient client, string regVendorId)
        {
            var api = UnityCore.Resolve<IVendorApi>();
            var roleApi = UnityCore.Resolve<IRoleApi>();

            var mailAddress = AppConfig.MailTestAddressTo;
            // 適当なRoleIdを引いてくる
            var roleId = client.GetWebApiResponseResult(roleApi.GetRoleList()).Assert(GetExpectStatusCodes).Result.First().RoleId;

            // SendInvitation Vendor NotFound
            var body = new SendInvitationModel
            {
                VendorId = Guid.NewGuid().ToString(),
                RoleId = roleId,
                MailAddress = mailAddress
            };
            client.GetWebApiResponseResult(api.SendInvitation(body)).Assert(BadRequestStatusCode);

            // SendInvitation Role NotFound
            body = new SendInvitationModel
            {
                VendorId = regVendorId,
                RoleId = Guid.NewGuid().ToString(),
                MailAddress = mailAddress
            };
            client.GetWebApiResponseResult(api.SendInvitation(body)).Assert(BadRequestStatusCode);

            // AddInvitationUser InvitationId NotFound
            var body2 = new AddInvitedUserModel
            {
                InvitationId = Guid.NewGuid().ToString(),
                OpenId = AppConfig.Account,
                MailAddress = mailAddress
            };
            client.GetWebApiResponseResult(api.AddInvitedUser(body2)).Assert(NotFoundStatusCode);
        }


        #region Data

        #region ベンダー

        public string RegisterVendorName = "------IntegratedTestVendorName------";
        public string UpdateVendorName = "------IntegratedTestUpdateVendorName------";

        /// <summary>
        /// 新規登録正常系データ
        /// </summary>
        public RegisterVendorModel RegisterVendorData
        {
            get =>
                new RegisterVendorModel()
                {
                    VendorName = RegisterVendorName,
                    IsDataOffer = false,
                    IsDataUse = false,
                    IsEnable = true,
                };
        }

        /// <summary>
        /// 更新登録正常系データ
        /// </summary>
        public UpdateVendorModel UpdateVendorData
        {
            get => new UpdateVendorModel()
            {
                VendorName = UpdateVendorName,
                IsDataOffer = false,
                IsDataUse = false,
                IsEnable = true,
            };
        }

        #endregion

        #region ベンダーリンク

        /// <summary>
        /// 更新登録正常系データ
        /// </summary>
        public List<RegisterVendorLinkModel> RegisterVendorLinkData(string vendorId,string vendorLinkId = null)
        {
            return new List<RegisterVendorLinkModel>()
            {
                new RegisterVendorLinkModel()
                {
                    VendorLinkId = vendorLinkId,
                    VendorId = vendorId,
                    LinkTitle = "------IntegratedTestRegisterVendorLink------",
                    LinkUrl = "https://test.net",
                    LinkDetail = "------IntegratedTestRegisterLinkDetail------",
                    IsVisible = false,
                    IsDefault = false,
                }
            };
        }
        public List<VendorLinkModel> UpdateVendorLinkModel(string vendorId)
        {
            return new List<VendorLinkModel>()
            {
                new VendorLinkModel()
                {
                    VendorId = vendorId,
                    LinkTitle = "------IntegratedTestRegisterVendorLink------",
                    LinkUrl = "https://test.net",
                    LinkDetail = "------IntegratedTestRegisterLinkDetail------",
                    IsVisible = false,
                    IsDefault = false,
                }
            };
        }
        #endregion

        #endregion

        #region Validation

        #region ベンダー

        public string RegisterVendorErrorName1 = "------IntegratedTestVendorName_Error1------";
        public string RegisterVendorErrorName2 = "------IntegratedTestVendorName_Error2------";

        private RegisterVendorModel RegisterVendorValidationBaseModel = new RegisterVendorModel()
        {
            VendorName = "regVendorName",
            IsDataOffer = false,
            IsDataUse = false,
            IsEnable = true
        };

        private UpdateVendorModel UpdateVendorValidationBaseModel = new UpdateVendorModel()
        {
            VendorId = Guid.Empty.ToString(),
            VendorName = "regVendorName",
            IsDataOffer = false,
            IsDataUse = false,
            IsEnable = true
        };

        /// <summary>
        /// ベンダー新規登録異常系データ
        /// </summary>
        public List<RegisterVendorModel> RegisterVendorValidationErrorData
        {
            get
            {
                // NameがNULL
                var vendorNameNull = RegisterVendorValidationBaseModel;
                vendorNameNull.VendorName = null;

                // Nameの桁数越え
                var vendorNameOver = RegisterVendorValidationBaseModel;
                vendorNameOver.VendorName = new string('a', 101);

                return new List<RegisterVendorModel>()
                {
                    vendorNameNull,
                    vendorNameOver
                };
            }
        }

        /// <summary>
        /// ベンダー更新登録異常系データ
        /// </summary>
        public List<UpdateVendorModel> UpdateVendorValidationErrorData
        {
            get
            {
                // IdがNULL
                var vendorIdNull = UpdateVendorValidationBaseModel;
                vendorIdNull.VendorId = null;

                // NameがNULL
                var vendorNameNull = UpdateVendorValidationBaseModel;
                vendorNameNull.VendorName = null;

                // Nameの桁数越え
                var vendorNameOver = UpdateVendorValidationBaseModel;
                vendorNameOver.VendorName = new string('a', 101);

                return new List<UpdateVendorModel>()
                {
                    vendorNameNull,
                    vendorNameOver
                };
            }
        }

        /// <summary>
        /// 新規登録異常系確認データ1
        /// </summary>
        public RegisterVendorModel RegisterVendorValidationErrorData1
        {
            get =>
                new RegisterVendorModel()
                {
                    VendorName = RegisterVendorErrorName1,
                    IsDataOffer = false,
                    IsDataUse = false,
                    IsEnable = true,
                };
        }

        /// <summary>
        /// 新規登録異常系確認データ2
        /// </summary>
        public RegisterVendorModel RegisterVendorValidationErrorData2
        {
            get =>
                new RegisterVendorModel()
                {
                    VendorName = RegisterVendorErrorName2,
                    IsDataOffer = false,
                    IsDataUse = false,
                    IsEnable = true,
                };
        }

        #endregion

        #region スタッフ

        private RegisterStafforModel AddStafforValidationBaseModel = new RegisterStafforModel()
        {
            VendorId = Guid.NewGuid().ToString(),
            Account = Guid.NewGuid().ToString()
        };

        private UpdateStaffModel UpdateStaffValidationBaseModel = new UpdateStaffModel()
        {
            VendorId = Guid.NewGuid().ToString(),
            Account = Guid.NewGuid().ToString(),
            StaffId = Guid.NewGuid().ToString(),
        };

        /// <summary>
        /// スタッフ新規登録異常系確認データ
        /// </summary>
        public List<RegisterStafforModel> AddStaffValidationErrorData
        {
            get
            {
                // VendorIdがNull
                var vendorIdNullModel = AddStafforValidationBaseModel;
                vendorIdNullModel.VendorId = null;

                // AccountがNull
                var accountNullModel = AddStafforValidationBaseModel;
                accountNullModel.Account = null;

                // Accountが桁数超え
                var accountLengthOverModel = AddStafforValidationBaseModel;
                accountLengthOverModel.Account = new string('a', 101);

                return new List<RegisterStafforModel>()
                {
                    vendorIdNullModel,
                    accountNullModel,
                    accountLengthOverModel
                };
            }
        }

        /// <summary>
        /// スタッフ更新登録異常系確認データ
        /// </summary>
        public List<UpdateStaffModel> UpdateStaffValidationErrorData
        {
            get
            {
                // VendorIdがNull
                var vendorIdNullModel = UpdateStaffValidationBaseModel;
                vendorIdNullModel.VendorId = null;

                // AccountがNull
                var accountNullModel = UpdateStaffValidationBaseModel;
                accountNullModel.Account = null;

                // Accountが桁数超え
                var accountLengthOverModel = UpdateStaffValidationBaseModel;
                accountLengthOverModel.Account = new string('a', 101);

                // StaffIdがNull
                var staffIdNullModel = UpdateStaffValidationBaseModel;
                staffIdNullModel.StaffId = null;

                // EmailがEmail形式でない
                var emailFormatError = UpdateStaffValidationBaseModel;
                emailFormatError.EmailAddress = "hogeEmail";
                return new List<UpdateStaffModel>()
                {
                    vendorIdNullModel,
                    accountNullModel,
                    accountLengthOverModel,
                    staffIdNullModel,
                    emailFormatError
                };
            }
        }

        #endregion

        #region ベンダーリンク

        private List<RegisterVendorLinkModel> RegisterVendorLinkValidationBaseModel
        {
            get
            {
                return new List<RegisterVendorLinkModel>()
                {
                    new RegisterVendorLinkModel()
                    {
                        LinkTitle = "hoge_LinkTitle",
                        LinkDetail = "hoge_LinkDetail",
                        LinkUrl = "https://test.net",
                        IsDefault = false,
                        IsVisible = false,
                    }
                };
            }
        }

        /// <summary>
        /// ベンダーリンク新規登録異常系確認データ
        /// </summary>
        public List<List<RegisterVendorLinkModel>> RegisterVendorLinkValidationErrorData(string vendorId)
        {
            // リンクタイトルがNULL
            var linkTitleNullModel = RegisterVendorLinkValidationBaseModel;
            linkTitleNullModel[0].LinkTitle = null;
            linkTitleNullModel[0].VendorId = vendorId;

            // リンクタイトルが桁数越え
            var linkTitleOverlModel = RegisterVendorLinkValidationBaseModel;
            linkTitleOverlModel[0].LinkTitle = new string('a', 101);
            linkTitleOverlModel[0].VendorId = vendorId;

            // リンク詳細がNULL
            var linkDetailNullModel = RegisterVendorLinkValidationBaseModel;
            linkDetailNullModel[0].LinkDetail = null;
            linkDetailNullModel[0].VendorId = vendorId;

            // リンク詳細が桁数越え
            var linkDetailOverlModel = RegisterVendorLinkValidationBaseModel;
            linkDetailOverlModel[0].LinkDetail = new string('a', 1001);
            linkDetailOverlModel[0].VendorId = vendorId;

            // リンクURLがNULL
            var linkUrlNullModel = RegisterVendorLinkValidationBaseModel;
            linkUrlNullModel[0].LinkUrl = null;
            linkUrlNullModel[0].VendorId = vendorId;

            // リンクURLが桁数越え
            var linkUrlOverModel = RegisterVendorLinkValidationBaseModel;
            linkUrlOverModel[0].LinkUrl += new string('a', 497); // 16+497=513
            linkUrlOverModel[0].VendorId = vendorId;

            // リンクURLがURL形式でない
            var linkUrlFormatErrorModel = RegisterVendorLinkValidationBaseModel;
            linkUrlFormatErrorModel[0].LinkUrl = "hoge";
            linkUrlFormatErrorModel[0].VendorId = vendorId;

            return new List<List<RegisterVendorLinkModel>>()
            {
                linkTitleNullModel,
                linkTitleOverlModel,
                linkDetailNullModel,
                linkDetailOverlModel
            };
        }


        #endregion

        #region 添付ファイル

        private (string VendorId, string AttachFileId) AttachFileBaseData = (Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

        /// <summary>
        /// 添付ファイル新規登録異常系確認データ
        /// </summary>
        public List<(string VendorId, string AttachFileId)> RegisterAttachFileValidationErrorData
        {
            get
            {
                // ベンダーIDがNull
                var vendorIdNull = AttachFileBaseData;
                vendorIdNull.VendorId = null;

                // ベンダーIDがGuidでない
                var vendorIdFormatError = AttachFileBaseData;
                vendorIdFormatError.VendorId = "hoge";

                // 添付ファイルIDがnull
                var attachFileIdNull = AttachFileBaseData;
                attachFileIdNull.AttachFileId = null;

                // 添付ファイルIDがGuidでない
                var attachFileFormatError = AttachFileBaseData;
                attachFileFormatError.AttachFileId = "hoge";

                return new List<(string vendorId, string attachFileId)>()
                {
                    vendorIdNull,
                    vendorIdFormatError,
                    attachFileIdNull,
                    attachFileFormatError
                };
            }
        }

        /// <summary>
        /// 添付ファイル削除異常系確認データ
        /// </summary>
        public List<string> DeleteAttachFileValidationErrorData
        {
            get
            {
                // ベンダーIDがNull
                string vendorIdNull = null;

                // ベンダーIDがGuidでない
                string vendorIdFormatError = "hoge";

                return new List<string>()
                {
                    vendorIdNull,
                    vendorIdFormatError
                };
            }
        }

        #endregion

        #region ベンダーOpenId認証局

        private List<RegisterOpenIdCaModel> RegisterVendorOpenIdCaValidationBase(string venderId)
        {
            return new List<RegisterOpenIdCaModel>()
            {
                new RegisterOpenIdCaModel()
                {
                    VendorId = venderId,
                    VendorOpenidCaId = Guid.NewGuid().ToString(),
                    ApplicationId = Guid.NewGuid().ToString(),
                    AccessControl = "inh"
                }
            };
        }

        private (string VendorId, string VendorOpenIdCaId) DeleteVendorOpenIdCaValidationBase
        {
            get
            {
                return (Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            }
        }


        /// <summary>
        /// ベンダーOpenId認証局登録異常系確認データ
        /// </summary>
        public List<List<RegisterOpenIdCaModel>> RegisterVendorOpenIdCaValidationErrorData(string vendorId)
        {
            // ApplicationIdがNull
            var appIdNull = RegisterVendorOpenIdCaValidationBase(vendorId);
            appIdNull[0].ApplicationId = null;

            // アクセス制御がNull
            var aclNull = RegisterVendorOpenIdCaValidationBase(vendorId);
            aclNull[0].AccessControl = null;

            // アクセス制御が想定外
            var aclFormatError = RegisterVendorOpenIdCaValidationBase(vendorId);
            aclFormatError[0].AccessControl = "hoge";

            // VendorIdがNull
            var vendorIdNull = RegisterVendorOpenIdCaValidationBase(null);

            // VendorIdがGuidでない
            var vendorIdFormatError = RegisterVendorOpenIdCaValidationBase("hoge");

            return new List<List<RegisterOpenIdCaModel>>()
            {
                appIdNull,
                aclNull,
                aclFormatError,
                vendorIdNull,
                vendorIdFormatError
            };
        }

        /// <summary>
        /// ベンダーOenId認証局削除異常系確認データ
        /// </summary>
        public List<(string VendorId, string VendorOpenIdCaId)> DeleteVendorOpenIdCaValidationErrorData
        {
            get
            {
                // VendorIdがNull
                var vendorIdNull = DeleteVendorOpenIdCaValidationBase;
                vendorIdNull.VendorId = null;

                // VendorIdがGuidでない
                var vendorIdFormatError = DeleteVendorOpenIdCaValidationBase;
                vendorIdFormatError.VendorId = "hoge";

                // VendorOpenIdCaIdがNull
                var vendorOpenIdCaIdNull = DeleteVendorOpenIdCaValidationBase;
                vendorOpenIdCaIdNull.VendorOpenIdCaId = null;

                // VendorOpenIdCaIdがGuidでない
                var vendorOpenIdCaIdFormatError = DeleteVendorOpenIdCaValidationBase;
                vendorOpenIdCaIdFormatError.VendorOpenIdCaId = "hoge";

                return new List<(string VendorId, string VendorOpenIdCaId)>()
                {
                    vendorIdNull,
                    vendorIdFormatError,
                    vendorOpenIdCaIdNull,
                    vendorOpenIdCaIdFormatError
                };
            }
        }

        #endregion

        #endregion



    }
}
