using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.System;
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
    public partial class SystemTest : ManageApiTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        /// <summary>
        /// システムの正常系テスト
        /// </summary>
        [TestMethod]
        public void System_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<ISystemApi>();
            string vendorId = AppConfig.AdminVendorId;

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetList(vendorId)).Assert(GetExpectStatusCodes);

            var listDelete = list.Result.Where(x => RegisterSystemName.Contains(x.SystemName) == true
            || UpdateSystemName.Contains(x.SystemName) == true).Select(x => x.SystemId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteSystem(x)).Assert(DeleteExpectStatusCodes));

            // システム新規登録
            var regData = RegisterSystemData;
            regData.VendorId = vendorId;
            var regDataResult = client.GetWebApiResponseResult(api.RegisterSystem(regData)).Assert(RegisterSuccessExpectStatusCode).Result;
            var regSystemId = regDataResult.SystemId;

            // システム更新登録
            var updData = UpdateSystemData;
            updData.SystemId = regSystemId;
            var updResult = client.GetWebApiResponseResult(api.UpdateSystem(updData)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 登録したものが一覧から取得できるか
            var getList = client.GetWebApiResponseResult(api.GetList(vendorId)).Assert(GetSuccessExpectStatusCode).Result;
            getList.Where(x => x.SystemId == regSystemId).Any().IsTrue();

            // 登録したものが取得できるか
            var get = client.GetWebApiResponseResult(api.GetSystem(regSystemId)).Assert(GetSuccessExpectStatusCode).Result;
            get.SystemId.Is(regSystemId);

            // システムAdmin認証のテスト
            this.SystemAdmin_NormalSenario(client,regSystemId);

            // システムリンクのテスト
            this.SystemLink_NormalSenario(client,regSystemId);

            // クライアント認証のテスト
            this.Client_NormalSenario(client,regSystemId);

            // システム削除
            client.GetWebApiResponseResult(api.DeleteSystem(regSystemId)).Assert(DeleteSuccessStatusCode);

            // 削除したものがNotFoundになるか(更新/削除)
            client.GetWebApiResponseResult(api.UpdateSystem(updData)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteSystem(regSystemId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// システムの異常系テスト
        /// </summary>
        [TestMethod]
        public void System_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<ISystemApi>();

            string regSystemId1 = string.Empty;
            string regSystemId2 = string.Empty;
            string vendorId = AppConfig.AdminVendorId;

            #region 準備

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetList(vendorId)).Assert(GetExpectStatusCodes);
            var listDelete = list.Result.Where(x => RegisterSystemErrorName1.Contains(x.SystemName) == true
            || RegisterSystemErrorName2.Contains(x.SystemName) == true).Select(x => x.SystemId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteSystem(x)).Assert(DeleteExpectStatusCodes));

            // 【準備】必要になるベンダーを登録
            var regData1 = RegisterSystemErrorModel1;
            regData1.VendorId = vendorId;
            regSystemId1 = client.GetWebApiResponseResult(api.RegisterSystem(regData1)).Assert(RegisterSuccessExpectStatusCode).Result.SystemId;
            var regData2 = RegisterSystemErrorModel2;
            regData2.VendorId = vendorId;
            regSystemId2 = client.GetWebApiResponseResult(api.RegisterSystem(regData2)).Assert(RegisterSuccessExpectStatusCode).Result.SystemId;

            #endregion

            // システム名重複(新規)
            client.GetWebApiResponseResult(api.RegisterSystem(regData1)).Assert(BadRequestStatusCode);
            // システム名重複(更新・システム2にシステム1の名前で更新)
            var updData = new UpdateSystemModel() { SystemId = regSystemId2, SystemName = RegisterSystemErrorName1 };
            client.GetWebApiResponseResult(api.UpdateSystem(updData)).Assert(BadRequestStatusCode);

            // システム新規登録時のValidation
            RegisterSystemValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterSystem(x)).Assert(BadRequestStatusCode)
            );

            // システム新規登録時のNullBody
            client.GetWebApiResponseResult(api.RegisterSystem(null)).Assert(BadRequestStatusCode);
            // システム更新登録時のValidation
            UpdateSystemValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateSystem(x)).Assert(BadRequestStatusCode)
            );

            // システム更新登録時のNullBody
            client.GetWebApiResponseResult(api.UpdateSystem(null)).Assert(BadRequestStatusCode);

            // システムAdmin認証の異常系テスト
            SystemAdmin_ErrorSenario(client);

            // システムリンクの異常系テスト
            SystemLink_ErrorSenario(client, regSystemId1);

            // クライアント認証の異常系テスト
            Clinet_ErrorSenario(client);

            #region 後始末

            client.GetWebApiResponseResult(api.DeleteSystem(regSystemId1)).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(api.DeleteSystem(regSystemId2)).Assert(DeleteSuccessStatusCode);

            #endregion
        }

        #region システムAdmin認証

        /// <summary>
        /// システムAdmin認証の正常系テスト
        /// </summary>
        private void SystemAdmin_NormalSenario(DynamicApiClient client, string regSystemId)
        {
            var api = UnityCore.Resolve<ISystemApi>();

            // 新規登録
            var regData = new SystemAdminModel() { SystemId = regSystemId, AdminSecret = "ITTest", IsActive = true };
            var regDataResult = client.GetWebApiResponseResult(api.RegisterSystemAdmin(regData)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 更新登録
            regData.AdminSecret = "ITTest_UPD";

            var updDataResult = client.GetWebApiResponseResult(api.RegisterSystemAdmin(regData)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 取得
            var getData = client.GetWebApiResponseResult(api.GetSystemAdmin(regSystemId)).Assert(GetSuccessExpectStatusCode).Result;
            getData.AdminSecret.Is(regData.AdminSecret);

            // 削除
            client.GetWebApiResponseResult(api.DeleteSystemAdmin(regSystemId)).Assert(DeleteSuccessStatusCode);

            // 再度削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteSystemAdmin(regSystemId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// システムAdmin認証の異常系テスト
        /// </summary>        
        private void SystemAdmin_ErrorSenario(DynamicApiClient client)
        {
            var api = UnityCore.Resolve<ISystemApi>();

            // 登録及び更新時のValidation
            RegisterSystemAdminValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterSystemAdmin(x)).Assert(BadRequestStatusCode)
            );

            // 登録及び更新時のNullBody
            client.GetWebApiResponseResult(api.RegisterSystemAdmin(null)).Assert(BadRequestStatusCode);
        }

        #endregion

        #region システムリンク

        /// <summary>
        /// システムリンクの正常系テスト
        /// </summary>
        /// <param name="regSystemId"></param>
        private void SystemLink_NormalSenario(DynamicApiClient client, string regSystemId)
        {
            var api = UnityCore.Resolve<ISystemApi>();

            // 新規登録
            var regDataResult = client.GetWebApiResponseResult(api.RegisterSystemLink(RegisterSystemLinkData(regSystemId))).Assert(RegisterSuccessExpectStatusCode).Result;
            var regSysLinkId = regDataResult[0].SystemLinkId;

            // 更新登録
            var updData = UpdateSystemLinkData(regSystemId, regSysLinkId);
            updData[0].SystemLinkId = regSysLinkId;
            var updDataResult = client.GetWebApiResponseResult(api.RegisterSystemLink(updData)).Assert(RegisterSuccessExpectStatusCode).Result;

            // 一覧取得
            var getList = client.GetWebApiResponseResult(api.GetSystemLinkList(regSystemId)).Assert(GetSuccessExpectStatusCode).Result;
            getList.Where(x => x.SystemLinkId == regSysLinkId).Any().IsTrue();

            // 1件取得
            var getData = client.GetWebApiResponseResult(api.GetSystemLink(regSysLinkId)).Assert(GetSuccessExpectStatusCode).Result;
            getData.Detail.ToString().Is(updData[0].Detail);

            // 削除
            client.GetWebApiResponseResult(api.DeleteSystemLink(regSysLinkId)).Assert(DeleteSuccessStatusCode);

            // 削除後NotFound
            client.GetWebApiResponseResult(api.DeleteSystemLink(regSysLinkId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetSystemLink(regSysLinkId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// システムリンクの異常系テスト
        /// </summary>
        private void SystemLink_ErrorSenario(DynamicApiClient client, string regSystemId)
        {
            var api = UnityCore.Resolve<ISystemApi>();

            // 登録及び更新時のValidation
            RegisterSystemLinkValidationErrorData(regSystemId).ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterSystemLink(x)).Assert(BadRequestStatusCode)
            );

            // 登録及び更新時のNullBody
            client.GetWebApiResponseResult(api.RegisterSystemLink(null)).Assert(BadRequestStatusCode);
        }

        #endregion

        #region クライアント認証

        /// <summary>
        /// クライアント認証の正常系テスト
        /// </summary>
        /// <param name="regSystemId"></param>
        private void Client_NormalSenario(DynamicApiClient client, string regSystemId)
        {
            var api = UnityCore.Resolve<ISystemApi>();

            // 新規登録
            var regClientData = RegisterClientData(regSystemId);
            regClientData.SystemId = regSystemId;
            var regClientId = client.GetWebApiResponseResult(api.RegisterClient(regClientData)).Assert(RegisterSuccessExpectStatusCode).Result.ClientId;

            // 1件取得
            var getClientData = client.GetWebApiResponseResult(api.GetClient(regClientId)).Assert(GetSuccessExpectStatusCode).Result;
            getClientData.ClientId.Is(regClientId);

            // 更新登録
            var updClientData = UpdateClientData(regSystemId, regClientId);
            updClientData.SystemId = regSystemId;
            updClientData.ClientId = regClientId;
            var updClientId = client.GetWebApiResponseResult(api.UpdateClient(updClientData)).Assert(RegisterSuccessExpectStatusCode).Result.ClientId;

            // 全件取得
            var getClientDataList = client.GetWebApiResponseResult(api.GetClientList(regSystemId)).Assert(GetSuccessExpectStatusCode).Result;
            getClientDataList.Where(x => x.ClientId == updClientId).Any().IsTrue();

            // 削除
            client.GetWebApiResponseResult(api.DeleteClient(regClientId)).Assert(DeleteSuccessStatusCode);

            // 削除後エラー
            client.GetWebApiResponseResult(api.GetClient(regClientId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetClientList(regSystemId)).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.DeleteClient(regClientId)).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// クライアント認証の異常系テスト
        /// </summary>
        /// <param name="regSystemId"></param>
        private void Clinet_ErrorSenario(DynamicApiClient client)
        {
            var api = UnityCore.Resolve<ISystemApi>();

            // 登録時のValidation
            RegisterClientValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterClient(x)).Assert(BadRequestStatusCode)
            );

            // 登録時のNullBody
            client.GetWebApiResponseResult(api.RegisterClient(null)).Assert(BadRequestStatusCode);

            // 更新時のValidation
            UpdateClientValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateClient(x)).Assert(BadRequestStatusCode)
            );

            // 更新時のNullBody
            client.GetWebApiResponseResult(api.UpdateClient(null)).Assert(BadRequestStatusCode);

            // 取得時のValidation
            client.GetWebApiResponseResult(api.GetClient(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.GetClient("hoge")).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.GetClientList(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.GetClientList("hoge")).Assert(BadRequestStatusCode);
        }

        #endregion

        #region Data

        #region システム

        public string RegisterSystemName = "------IntegratedTestSystemName------";
        public string UpdateSystemName = "------IntegratedTestUpdateSystemName------";

        /// <summary>
        /// システム新規登録情報
        /// </summary>
        public RegisterSystemModel RegisterSystemData
        {
            get
            {
                return new RegisterSystemModel()
                {
                    SystemName = RegisterSystemName,
                    OpenIdApplicationId = Guid.NewGuid().ToString(),
                    OpenIdClientSecret = "hogeOCS",
                    IsEnable = true
                };
            }
        }

        /// <summary>
        /// システム更新登録情報
        /// </summary>
        public UpdateSystemModel UpdateSystemData
        {
            get
            {
                return new UpdateSystemModel()
                {
                    SystemName = UpdateSystemName,
                    OpenIdApplicationId = Guid.NewGuid().ToString(),
                    OpenIdClientSecret = "hogeOCS_UPD",
                    IsEnable = true
                };
            }
        }

        #endregion

        #region システムリンク

        /// <summary>
        /// システムリンク登録正常系データ
        /// </summary>
        public List<SystemLinkModel> RegisterSystemLinkData(string systemId)
        {
            return new List<SystemLinkModel>()
            {
                new SystemLinkModel()
                {
                    Title = "------IntegratedTestRegisterSystemLink------",
                    Url = "https://test.net",
                    Detail = "------IntegratedTestRegisterSystemLinkDetail------",
                    IsVisible = false,
                    IsDefault = false,
                    SystemId = systemId
                }
            };
        }

        /// <summary>
        /// システムリンク更新正常系データ
        /// </summary>
        public List<SystemLinkModel> UpdateSystemLinkData(string systemId,string systemLinkId)
        {
            return  new List<SystemLinkModel>()
            {
                new SystemLinkModel()
                {
                    SystemLinkId = systemLinkId,
                    Title = "------IntegratedTestUpdateSystemLink------",
                    Url = "https://testUpdate.net",
                    Detail = "------IntegratedTestUpdateSystemLinkDetail------",
                    IsVisible = false,
                    IsDefault = false,
                    SystemId = systemId
                }
            };
        }

        #endregion

        #region クライアント認証

        /// <summary>
        /// クライアント認証登録正常系データ
        /// </summary>
        public RegisterClientModel RegisterClientData(string systemId)
        {
            return new RegisterClientModel()
            {
                SystemId = systemId,
                ClientSecret = "itTestHoge123",
                AccessTokenExpirationTimeSpan = "12:00"
            };
        }

        /// <summary>
        /// クライアント認証登録更新系データ
        /// </summary>
        public UpdateClientModel UpdateClientData(string systemId,string clientId)
        {
            return new UpdateClientModel()
            {
                ClientId = clientId,
                SystemId = systemId,
                ClientSecret = "itTestHoge123Upd",
                AccessTokenExpirationTimeSpan = "24:00"
            };
        }

        #endregion

        #endregion

        #region Validation

        #region システム

        public string RegisterSystemErrorName1 = "------IntegratedTestSystemName_Error1------";
        public string RegisterSystemErrorName2 = "------IntegratedTestSystemName_Error2------";

        /// <summary>
        /// システム異常確認用登録1
        /// </summary>
        public RegisterSystemModel RegisterSystemErrorModel1
        {
            get
            {
                return new RegisterSystemModel()
                {
                    SystemName = RegisterSystemErrorName1,
                    IsEnable = false,
                    OpenIdApplicationId = "reg_OAI",
                    OpenIdClientSecret = "reg_OCS",
                    VendorId = Guid.NewGuid().ToString()
                };
            }
        }

        /// <summary>
        /// システム異常確認用登録2
        /// </summary>
        public RegisterSystemModel RegisterSystemErrorModel2
        {
            get
            {
                return new RegisterSystemModel()
                {
                    SystemName = RegisterSystemErrorName2,
                    IsEnable = false,
                    OpenIdApplicationId = "reg2_OAI",
                    OpenIdClientSecret = "reg2_OCS",
                    VendorId = Guid.NewGuid().ToString()
                };
            }
        }

        private RegisterSystemModel RegisterSystemValidationBaseModel = new RegisterSystemModel()
        {
            SystemName = "hoge_systemName",
            IsEnable = false,
            OpenIdApplicationId = "hoge_OAI",
            OpenIdClientSecret = "hoge_OCS",
            VendorId = Guid.NewGuid().ToString()
        };

        /// <summary>
        /// システム登録異常系データ
        /// </summary>
        public List<RegisterSystemModel> RegisterSystemValidationErrorData
        {
            get
            {
                // VendorIdがNull
                var vendorIdNull = RegisterSystemValidationBaseModel;
                vendorIdNull.VendorId = null;

                // VendorIdがGuidでない
                var vendorIdFormatError = RegisterSystemValidationBaseModel;
                vendorIdFormatError.VendorId = "hoge";

                // システム名がNull
                var systemNameNull = RegisterSystemValidationBaseModel;
                systemNameNull.SystemName = null;

                // システム名が文字数制限越え
                var systemNameOver = RegisterSystemValidationBaseModel;
                systemNameOver.SystemName = new string('a', 101);

                // OpenIdApplicationIdが文字数制限越え
                var openIdApplicationIdOver = RegisterSystemValidationBaseModel;
                openIdApplicationIdOver.OpenIdApplicationId = new string('a', 129);

                // OpenIdClientSecretが文字数制限越え
                var openIdClientSecretOver = RegisterSystemValidationBaseModel;
                openIdClientSecretOver.OpenIdClientSecret = new string('a', 261);

                return new List<RegisterSystemModel>()
                {
                    vendorIdNull,
                    vendorIdFormatError,
                    systemNameNull,
                    systemNameOver,
                    openIdApplicationIdOver,
                    openIdClientSecretOver
                };
            }
        }

        private UpdateSystemModel UpdateSystemValidationBaseModel = new UpdateSystemModel()
        {
            SystemName = "hoge_systemName",
            IsEnable = false,
            OpenIdApplicationId = "hoge_OAI",
            OpenIdClientSecret = "hoge_OCS",
            SystemId = Guid.NewGuid().ToString()
        };

        /// <summary>
        /// システム更新異常系データ
        /// </summary>
        public List<UpdateSystemModel> UpdateSystemValidationErrorData
        {
            get
            {
                // SystemIdがNull
                var systemIdNull = UpdateSystemValidationBaseModel;
                systemIdNull.SystemId = null;

                // VendorIdがGuidでない
                var systemIdFormatError = UpdateSystemValidationBaseModel;
                systemIdFormatError.SystemId = "hoge";

                // システム名がNull
                var systemNameNull = UpdateSystemValidationBaseModel;
                systemNameNull.SystemName = null;

                // システム名が文字数制限越え
                var systemNameOver = UpdateSystemValidationBaseModel;
                systemNameOver.SystemName = new string('a', 101);

                // OpenIdApplicationIdが文字数制限越え
                var openIdApplicationIdOver = UpdateSystemValidationBaseModel;
                openIdApplicationIdOver.OpenIdApplicationId = new string('a', 129);

                // OpenIdClientSecretが文字数制限越え
                var openIdClientSecretOver = UpdateSystemValidationBaseModel;
                openIdClientSecretOver.OpenIdClientSecret = new string('a', 261);

                return new List<UpdateSystemModel>()
                {
                    systemIdNull,
                    systemIdFormatError,
                    systemNameNull,
                    systemNameOver,
                    openIdApplicationIdOver,
                    openIdClientSecretOver
                };
            }
        }

        #endregion

        #region システムAdmin認証

        private SystemAdminModel RegisterSystemAdminValidationBaseModel = new SystemAdminModel()
        {
            SystemId = Guid.NewGuid().ToString(),
            AdminSecret = "hoge"
        };

        /// <summary>
        /// システムAdmin認証登録異常系データ
        /// </summary>
        public List<SystemAdminModel> RegisterSystemAdminValidationErrorData
        {
            get
            {
                // SystemIdがNull
                var systemIdNullModel = RegisterSystemAdminValidationBaseModel;
                systemIdNullModel.SystemId = null;

                // SystemIdがGuiでない
                var systemIdFormatError = RegisterSystemAdminValidationBaseModel;
                systemIdFormatError.SystemId = "hoge";

                return new List<SystemAdminModel>()
                {
                    systemIdNullModel,
                    systemIdFormatError
                };
            }
        }

        #endregion

        #region システムリンク

        private List<SystemLinkModel> RegisterSystemLinkValidationBaseModel(string systemId)
        {
            return new List<SystemLinkModel>()
            {
                new SystemLinkModel()
                {
                    SystemId = systemId,
                    Title = "hoge_LinkTitle",
                    Detail = "hoge_LinkDetail",
                    Url = "https://test.net",
                    IsDefault = false,
                    IsVisible = false,
                }
            };
        }

        /// <summary>
        /// システムリンク新規登録異常系確認データ
        /// </summary>
        public List<List<SystemLinkModel>> RegisterSystemLinkValidationErrorData(string systemId)
        {
            // リンクタイトルがNULL
            var linkTitleNullModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkTitleNullModel[0].Title = null;

            // リンクタイトルが桁数越え
            var linkTitleOverlModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkTitleOverlModel[0].Title = new string('a', 101);

            // リンク詳細がNULL
            var linkDetailNullModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkDetailNullModel[0].Detail = null;

            // リンク詳細が桁数越え
            var linkDetailOverlModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkDetailOverlModel[0].Detail = new string('a', 1001);

            // リンクURLがNULL
            var linkUrlNullModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkUrlNullModel[0].Url = null;

            // リンクURLが桁数越え
            var linkUrlOverModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkUrlOverModel[0].Url += new string('a', 497); // 16+497=513

            // リンクURLがURL形式でない
            var linkUrlFormatErrorModel = RegisterSystemLinkValidationBaseModel(systemId);
            linkUrlFormatErrorModel[0].Url = "hoge";

            return new List<List<SystemLinkModel>>()
            {
                linkTitleNullModel,
                linkTitleOverlModel,
                linkDetailNullModel,
                linkDetailOverlModel
            };
        }

        #endregion

        #region クライアント認証

        private RegisterClientModel RegisterClientValidationBaseModel = new RegisterClientModel()
        {
            SystemId = Guid.NewGuid().ToString(),
            ClientSecret = "hogeFuga123",
            AccessTokenExpirationTimeSpan = "12:00"
        };

        /// <summary>
        /// クライアント認証新規登録異常系確認データ
        /// </summary>
        public List<RegisterClientModel> RegisterClientValidationErrorData
        {
            get
            {
                // SystemIdがNull
                var systemIdNullModel = RegisterClientValidationBaseModel;
                systemIdNullModel.SystemId = null;

                // SystemIdがGuidでない
                var systemIdFormatErrorModel = RegisterClientValidationBaseModel;
                systemIdFormatErrorModel.SystemId = "hoge";

                // ClientSecretがNull
                var clientSecretNullModel = RegisterClientValidationBaseModel;
                clientSecretNullModel.ClientSecret = null;

                // ClientSecretが桁数不足
                var clientSecretLengthMissing = RegisterClientValidationBaseModel;
                clientSecretLengthMissing.ClientSecret = "Hoge12";

                // ClientSecretの形式エラー(大文字・小文字のみ)
                var clientSecretFormatError1 = RegisterClientValidationBaseModel;
                clientSecretFormatError1.ClientSecret = "HogeFugaFugo";

                // ClientSecretの形式エラー(大文字・数字のみ)
                var clientSecretFormatError2 = RegisterClientValidationBaseModel;
                clientSecretFormatError2.ClientSecret = "HOGEFUGA123";

                // ClientSecretの形式エラー(小文字・記号のみ)
                var clientSecretFormatError3 = RegisterClientValidationBaseModel;
                clientSecretFormatError3.ClientSecret = "hogefuga@#$";

                // ClientSecretの形式エラー(全角あり)
                var clientSecretFormatError4 = RegisterClientValidationBaseModel;
                clientSecretFormatError4.ClientSecret = "HogeＦuga123";

                // 有効期限がNull
                var expireNullModel = RegisterClientValidationBaseModel;
                expireNullModel.AccessTokenExpirationTimeSpan = null;

                // 有効期限の形式エラー(日付でない)
                var expireFormatError1 = RegisterClientValidationBaseModel;
                expireFormatError1.AccessTokenExpirationTimeSpan = "hoge";

                // 有効期限の形式エラー(00:00はNG)
                var expireFormatError2 = RegisterClientValidationBaseModel;
                expireFormatError2.AccessTokenExpirationTimeSpan = "0:00";

                // 有効期限の形式エラー(24:01はNG)
                var expireFormatError3 = RegisterClientValidationBaseModel;
                expireFormatError3.AccessTokenExpirationTimeSpan = "24:01";

                return new List<RegisterClientModel>()
                {
                    systemIdNullModel,
                    systemIdFormatErrorModel,
                    clientSecretNullModel,
                    clientSecretLengthMissing,
                    clientSecretFormatError1,
                    clientSecretFormatError2,
                    clientSecretFormatError3,
                    clientSecretFormatError4,
                    expireNullModel,
                    expireFormatError1,
                    expireFormatError2,
                    expireFormatError3
                };
            }
        }

        private UpdateClientModel UpdateClientValidationBaseModel = new UpdateClientModel()
        {
            SystemId = Guid.NewGuid().ToString(),
            ClientId = Guid.NewGuid().ToString(),
            ClientSecret = "hogeFuga123",
            AccessTokenExpirationTimeSpan = "12:00"
        };


        /// <summary>
        /// クライアント認証更新登録異常系確認データ
        /// </summary>
        public List<UpdateClientModel> UpdateClientValidationErrorData
        {
            get
            {
                // SystemIdがNull
                var systemIdNullModel = UpdateClientValidationBaseModel;
                systemIdNullModel.SystemId = null;

                // SystemIdがGuidでない
                var systemIdFormatErrorModel = UpdateClientValidationBaseModel;
                systemIdFormatErrorModel.SystemId = "hoge";

                // ClientIdがNull
                var clientIdNullModel = UpdateClientValidationBaseModel;
                clientIdNullModel.ClientId = null;

                // ClientIdがGuidでない
                var clientIdFormatErrorModel = UpdateClientValidationBaseModel;
                clientIdFormatErrorModel.ClientId = "hoge";

                // ClientSecretがNull
                var clientSecretNullModel = UpdateClientValidationBaseModel;
                clientSecretNullModel.ClientSecret = null;

                // ClientSecretが桁数不足
                var clientSecretLengthMissing = UpdateClientValidationBaseModel;
                clientSecretLengthMissing.ClientSecret = "Hoge12";

                // ClientSecretの形式エラー(大文字・小文字のみ)
                var clientSecretFormatError1 = UpdateClientValidationBaseModel;
                clientSecretFormatError1.ClientSecret = "HogeFugaFugo";

                // ClientSecretの形式エラー(大文字・数字のみ)
                var clientSecretFormatError2 = UpdateClientValidationBaseModel;
                clientSecretFormatError2.ClientSecret = "HOGEFUGA123";

                // ClientSecretの形式エラー(小文字・記号のみ)
                var clientSecretFormatError3 = UpdateClientValidationBaseModel;
                clientSecretFormatError3.ClientSecret = "hogefuga@#$";

                // ClientSecretの形式エラー(全角あり)
                var clientSecretFormatError4 = UpdateClientValidationBaseModel;
                clientSecretFormatError4.ClientSecret = "HogeＦuga123";

                // 有効期限がNull
                var expireNullModel = UpdateClientValidationBaseModel;
                expireNullModel.AccessTokenExpirationTimeSpan = null;

                // 有効期限の形式エラー(日付でない)
                var expireFormatError1 = UpdateClientValidationBaseModel;
                expireFormatError1.AccessTokenExpirationTimeSpan = "hoge";

                // 有効期限の形式エラー(00:00はNG)
                var expireFormatError2 = UpdateClientValidationBaseModel;
                expireFormatError2.AccessTokenExpirationTimeSpan = "0:00";

                // 有効期限の形式エラー(24:01はNG)
                var expireFormatError3 = UpdateClientValidationBaseModel;
                expireFormatError3.AccessTokenExpirationTimeSpan = "24:01";

                return new List<UpdateClientModel>()
                {
                    systemIdNullModel,
                    systemIdFormatErrorModel,
                    clientIdNullModel,
                    clientIdFormatErrorModel,
                    clientSecretNullModel,
                    clientSecretLengthMissing,
                    clientSecretFormatError1,
                    clientSecretFormatError2,
                    clientSecretFormatError3,
                    clientSecretFormatError4,
                    expireNullModel,
                    expireFormatError1,
                    expireFormatError2,
                    expireFormatError3
                };
            }
        }

        #endregion

        #endregion
    }
}
