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
    public partial class ApiMailTemplateTest : ManageApiTestCase
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
        /// APIメールテンプレートの正常系テスト
        /// </summary>
        [TestMethod]
        public void ApiMailTemplate_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiMailTemplateApi>();
            var mailTemplateApi = UnityCore.Resolve<IMailTemplateApi>();
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            const string apiUrl = "/API/IntegratedTest/ManageApiMailTemplate/Test";
            var dynamicApi = client.GetWebApiResponseResult(manageDynamicApi.GetApiResourceFromUrl(apiUrl, true)).Assert(GetExpectStatusCodes).Result;
            if (dynamicApi?.ApiId != null)
            {
                var listDelete = client.GetWebApiResponseResult(api.GetApiMailTemplateList(dynamicApi.ApiId,_vendorId)).Assert(GetExpectStatusCodes).Result;
                listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteApiMailTemplate(x.ApiMailTemplateId)).Assert(DeleteExpectStatusCodes));
            }
            string apiId;
            {
                // Api
                CleanUpApiByUrl(client, apiUrl, true);
                var baseData = CreateRequestModel();
                baseData.Url = apiUrl;
                apiId = client.GetWebApiResponseResult(manageDynamicApi.RegisterApi(baseData)).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;

            }

            string mailTemplateId;
            string mailTemplateId2;
            {
                var registerName = "---itRegister-apiMailTemplate---";
                var updateName = "---itUpdate-apiMailTemplate---";
                // MailTemplate
                var mailTemplateList = client.GetWebApiResponseResult(mailTemplateApi.GetMailTemplateList(_vendorId)).Assert(GetSuccessExpectStatusCode).Result.ToList();

                mailTemplateList.Where(x => x.MailTemplateName == registerName || x.MailTemplateName == updateName).ToList().ForEach(
                    x => client.GetWebApiResponseResult(mailTemplateApi.DeleteMailTemplate(x.MailTemplateId)).Assert(DeleteSuccessStatusCode));

                // 新規登録
                var mailTemplateRegObj = MailTemplateTest.MailTemplateRegData;
                mailTemplateRegObj.MailTemplateName = registerName;
                mailTemplateRegObj.VendorId = _vendorId;
                mailTemplateId = client.GetWebApiResponseResult(mailTemplateApi.RegisterMailTemplate(mailTemplateRegObj)).Assert(RegisterSuccessExpectStatusCode).Result.MailTemplateId;

                // 更新データで新規登録
                var mailTemplateUpdObj = MailTemplateTest.MailTemplateUpdData;
                mailTemplateUpdObj.MailTemplateName = updateName;
                mailTemplateUpdObj.VendorId = _vendorId;
                mailTemplateId2 = client.GetWebApiResponseResult(mailTemplateApi.RegisterMailTemplate(mailTemplateUpdObj)).Assert(RegisterSuccessExpectStatusCode).Result.MailTemplateId;

            }

            // 新規登録
            var regObj = ApiMailTemplateRegData;
            regObj.ApiId = apiId;
            regObj.VendorId = _vendorId;
            regObj.MailTemplateId = mailTemplateId;
            var regId = client.GetWebApiResponseResult(api.RegisterApiMailTemplate(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.ApiMailTemplateId;

            //ApiId FkError
            {
                var fkData = ApiMailTemplateRegData;
                fkData.ApiId = Guid.NewGuid().ToString();
                client.GetWebApiResponseResult(api.RegisterApiMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            //指定したベンダーがメールテンプレートを保持していない場合(VendorId の指定誤り)
            {
                var fkData = ApiMailTemplateRegData;
                fkData.VendorId = Guid.NewGuid().ToString();
                client.GetWebApiResponseResult(api.RegisterApiMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            //指定したベンダーがメールテンプレートを保持していない場合(MailTemplateId の指定誤り)
            {
                var fkData = ApiMailTemplateRegData;
                fkData.MailTemplateId = Guid.NewGuid().ToString();
                client.GetWebApiResponseResult(api.RegisterApiMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            // ApiId & VendorId & mailTemplateId 重複登録エラー
            {
                // ApiId & VendorId & mailTemplateId が重複するような登録データを投げる。
                client.GetWebApiResponseResult(api.RegisterApiMailTemplate(regObj)).Assert(BadRequestStatusCode);
            }

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetApiMailTemplate(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 関係ないIDで検索（NotFound)
            client.GetWebApiResponseResult(api.GetApiMailTemplate(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // 更新
            var updObj = ApiMailTemplateUpdData;
            updObj.ApiMailTemplateId = regId;
            updObj.ApiId = apiId;
            updObj.VendorId = _vendorId;
            updObj.MailTemplateId = mailTemplateId;

            var updData = client.GetWebApiResponseResult(api.UpdateApiMailTemplate(updObj)).Assert(RegisterSuccessExpectStatusCode).Result;

            //ApiId FkError
            {
                var fkData = ApiMailTemplateUpdData;
                fkData.ApiMailTemplateId = regId;
                fkData.ApiId = Guid.NewGuid().ToString();
                fkData.VendorId = _vendorId;
                fkData.MailTemplateId = mailTemplateId;
                client.GetWebApiResponseResult(api.UpdateApiMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            //指定したベンダーがメールテンプレートを保持していない場合(VendorId の指定誤り)
            {
                var fkData = ApiMailTemplateUpdData;
                fkData.ApiMailTemplateId = regId;
                fkData.ApiId = apiId;
                fkData.VendorId = Guid.NewGuid().ToString();
                fkData.MailTemplateId = mailTemplateId;
                client.GetWebApiResponseResult(api.UpdateApiMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            //指定したベンダーがメールテンプレートを保持していない場合(MailTemplateId の指定誤り)
            {
                var fkData = ApiMailTemplateUpdData;
                fkData.ApiMailTemplateId = regId;
                fkData.ApiId = apiId;
                fkData.VendorId = _vendorId;
                fkData.MailTemplateId = Guid.NewGuid().ToString();
                client.GetWebApiResponseResult(api.UpdateApiMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            // ApiId & VendorId & mailTemplateId 重複登録エラー
            {
                // 新規レコードを追加し、
                regObj.ApiId = apiId;
                regObj.VendorId = _vendorId;
                regObj.MailTemplateId = mailTemplateId2;//新規レコード追加時は重複しないようにしておく。
                var duplicateObjId = client.GetWebApiResponseResult(api.RegisterApiMailTemplate(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.ApiMailTemplateId;

                // ApiId & VendorId & mailTemplateId が登録済みデータと重複するような更新データを投げる。
                var duplicateObj = ApiMailTemplateUpdData;
                duplicateObj.ApiMailTemplateId = duplicateObjId;
                duplicateObj.ApiId = apiId;
                duplicateObj.VendorId = _vendorId;
                duplicateObj.MailTemplateId = mailTemplateId;//重複するよう更新する。
                client.GetWebApiResponseResult(api.UpdateApiMailTemplate(duplicateObj)).Assert(BadRequestStatusCode);

                // 後始末：新規レコードは削除しておく。
                client.GetWebApiResponseResult(api.DeleteApiMailTemplate(duplicateObjId)).Assert(DeleteSuccessStatusCode);
            }

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetApiMailTemplate(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.ToJson()["ApiMailTemplateId"].ToString().IsStructuralEqual(getUpdData.ToJson()["ApiMailTemplateId"].ToString());
            // 新規登録したものを更新登録したものが異なること（更新したため）
            getRegData.IsNotStructuralEqual(getUpdData);

            // 一覧取得ができること(vendorIdなし)
            client.GetWebApiResponseResult(api.GetApiMailTemplateList(apiId)).Assert(GetSuccessExpectStatusCode);
            // 一覧取得ができること
            var list = client.GetWebApiResponseResult(api.GetApiMailTemplateList(apiId, _vendorId)).Assert(GetSuccessExpectStatusCode).Result;
            // リストの中に新規登録したデータ（登録後、更新したデータ）が存在することを確認
            // 新規登録したデータ（登録後、更新したデータ）が存在する
            list.Exists(x => x.ApiMailTemplateId == getUpdData.ApiMailTemplateId && x.NotifyRegister == getUpdData.NotifyRegister).IsTrue();

            // 削除
            client.GetWebApiResponseResult(api.DeleteApiMailTemplate(regId)).Assert(DeleteSuccessStatusCode);

            // 削除したものを再度削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteApiMailTemplate(regId)).Assert(NotFoundStatusCode);
            // 削除したものを取得(NotFound)
            client.GetWebApiResponseResult(api.GetApiMailTemplate(regId)).Assert(NotFoundStatusCode);
            // 削除したものを更新(BadRequest)
            client.GetWebApiResponseResult(api.UpdateApiMailTemplate(updObj)).Assert(BadRequestStatusCode);

            {
                // API
                CleanUpApiByUrl(client, apiUrl, true);
            }
            {
                // MailTemplate
                client.GetWebApiResponseResult(mailTemplateApi.DeleteMailTemplate(mailTemplateId)).Assert(DeleteSuccessStatusCode);
                client.GetWebApiResponseResult(mailTemplateApi.DeleteMailTemplate(mailTemplateId2)).Assert(DeleteSuccessStatusCode);
            }
        }

        /// <summary>
        /// APIメールテンプレートのエラー系テスト
        /// </summary>
        [TestMethod]
        public void ApiMailTemplate_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiMailTemplateApi>();

            // RegisterApiMailTemplateのValidationError
            RegisterApiMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterApiMailTemplate(x)).Assert(BadRequestStatusCode)
            );

            // RegisterApiMailTemplateのNullBody
            client.GetWebApiResponseResult(api.RegisterApiMailTemplate(null)).Assert(BadRequestStatusCode);

            // UpdateApiMailTemplateのValidationError
            UpdateApiMailTemplateValidationErrorData2.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateApiMailTemplate(x)).Assert(BadRequestStatusCode)
            );

            // UpdfateApiMailTemplateのNullBody
            client.GetWebApiResponseResult(api.UpdateApiMailTemplate(null)).Assert(BadRequestStatusCode);

            // GetApiMailTemplateのValidationError
            GetApiMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetApiMailTemplate(x)).Assert(BadRequestStatusCode)
            );

            // GetApiMailTemplateListのValidationError
            GetApiMailTemplateListValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetApiMailTemplateList(x.ApiId, x.VendorId)).Assert(BadRequestStatusCode)
            );

            // DeleteApiMailTemplateのValidationError
            DeleteApiMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteApiMailTemplate(x)).Assert(BadRequestStatusCode)
            );
        }

        #region Data

        /// <summary>
        /// APIメールテンプレート正常系データ
        /// </summary>
        public RegisterApiMailTemplateModel ApiMailTemplateRegData
        {
            get =>
                new RegisterApiMailTemplateModel()
                {
                    ApiId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateId = Guid.NewGuid().ToString(),
                    NotifyRegister = bool.FalseString,
                    NotifyUpdate = bool.FalseString,
                    NotifyDelete = bool.FalseString
                };
        }

        public ApiMailTemplateModel ApiMailTemplateUpdData
        {
            get =>
                new ApiMailTemplateModel()
                {
                    ApiMailTemplateId = Guid.NewGuid().ToString(), //登録データで代入
                    ApiId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateId = Guid.NewGuid().ToString(),
                    NotifyRegister = bool.TrueString,
                    NotifyUpdate = bool.TrueString,
                    NotifyDelete = bool.TrueString
                };
        }

        #endregion

        #region Validation/Error
        public RegisterApiMailTemplateModel RegisterApiMailTemplateValidationBaseModel
        {
            get =>
                new RegisterApiMailTemplateModel()
                {
                    ApiId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateId = Guid.NewGuid().ToString(),
                    NotifyRegister = bool.FalseString,
                    NotifyUpdate = bool.FalseString,
                    NotifyDelete = bool.FalseString
                };
        }

        public ApiMailTemplateModel ApiMailTemplateValidationBaseModel
        {
            get =>
                new ApiMailTemplateModel()
                {
                    ApiMailTemplateId = Guid.NewGuid().ToString(),
                    ApiId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateId = Guid.NewGuid().ToString(),
                    NotifyRegister = bool.FalseString,
                    NotifyUpdate = bool.FalseString,
                    NotifyDelete = bool.FalseString
                };
        }

        public GetListParams GetListParamsValidationBaseModel
        {
            get =>
                new GetListParams()
                {
                    ApiId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString()
                };
        }


        /// <summary>
        /// APIメールテンプレート異常系データ(RegisterApiMailTemplate)
        /// </summary>
        public List<RegisterApiMailTemplateModel> RegisterApiMailTemplateValidationErrorData
        {
            get
            {
                // ApiId がnull
                var apiIdNullModel = RegisterApiMailTemplateValidationBaseModel;
                apiIdNullModel.ApiId = null;

                // ApiId がGuidでない
                var apiIdNotGuidModel = RegisterApiMailTemplateValidationBaseModel;
                apiIdNotGuidModel.ApiId = "aaa";

                // VendorId がnull
                var vendorIdNullModel = RegisterApiMailTemplateValidationBaseModel;
                vendorIdNullModel.VendorId = null;

                // VendorId がGuidでない
                var vendorIdNotGuidModel = RegisterApiMailTemplateValidationBaseModel;
                vendorIdNotGuidModel.VendorId = "aaa";

                // MailTemplateId がnull
                var mailTemplateIdNullModel = RegisterApiMailTemplateValidationBaseModel;
                mailTemplateIdNullModel.MailTemplateId = null;

                // MailTemplateId がGuidでない
                var mailTemplateIdNotGuidModel = RegisterApiMailTemplateValidationBaseModel;
                mailTemplateIdNotGuidModel.MailTemplateId = "aaa";

                // NotifyRegister がnull
                var notifyRegisterNullModel = RegisterApiMailTemplateValidationBaseModel;
                notifyRegisterNullModel.NotifyRegister = null;

                // NotifyRegister がboolでない
                var notifyRegisterNotBoolModel = RegisterApiMailTemplateValidationBaseModel;
                notifyRegisterNotBoolModel.NotifyRegister = "aaa";

                // NotifyUpdate がnull
                var notifyUpdateNullModel = RegisterApiMailTemplateValidationBaseModel;
                notifyUpdateNullModel.NotifyUpdate = null;

                // NotifyUpdate がboolでない
                var notifyUpdateNotBoolModel = RegisterApiMailTemplateValidationBaseModel;
                notifyUpdateNotBoolModel.NotifyUpdate = "aaa";

                // NotifyDelete がnull
                var notifyDeleteNullModel = RegisterApiMailTemplateValidationBaseModel;
                notifyDeleteNullModel.NotifyDelete = null;

                // NotifyDelete がboolでない
                var notifyDeleteNotBoolModel = RegisterApiMailTemplateValidationBaseModel;
                notifyDeleteNotBoolModel.NotifyDelete = "aaa";

                return new List<RegisterApiMailTemplateModel>()
                {
                    apiIdNullModel,
                    apiIdNotGuidModel,
                    vendorIdNullModel,
                    vendorIdNotGuidModel,
                    mailTemplateIdNullModel,
                    mailTemplateIdNotGuidModel,
                    notifyRegisterNullModel,
                    notifyRegisterNotBoolModel,
                    notifyUpdateNullModel,
                    notifyUpdateNotBoolModel,
                    notifyDeleteNullModel,
                    notifyDeleteNotBoolModel
                };
            }
        }
        public List<ApiMailTemplateModel> UpdateApiMailTemplateValidationErrorData
        {
            get
            {
                // ApiId がnull
                var apiIdNullModel = ApiMailTemplateValidationBaseModel;
                apiIdNullModel.ApiId = null;

                // ApiId がGuidでない
                var apiIdNotGuidModel = ApiMailTemplateValidationBaseModel;
                apiIdNotGuidModel.ApiId = "aaa";

                // VendorId がnull
                var vendorIdNullModel = ApiMailTemplateValidationBaseModel;
                vendorIdNullModel.VendorId = null;

                // VendorId がGuidでない
                var vendorIdNotGuidModel = ApiMailTemplateValidationBaseModel;
                vendorIdNotGuidModel.VendorId = "aaa";

                // MailTemplateId がnull
                var mailTemplateIdNullModel = ApiMailTemplateValidationBaseModel;
                mailTemplateIdNullModel.MailTemplateId = null;

                // MailTemplateId がGuidでない
                var mailTemplateIdNotGuidModel = ApiMailTemplateValidationBaseModel;
                mailTemplateIdNotGuidModel.MailTemplateId = "aaa";

                // NotifyRegister がnull
                var notifyRegisterNullModel = ApiMailTemplateValidationBaseModel;
                notifyRegisterNullModel.NotifyRegister = null;

                // NotifyRegister がboolでない
                var notifyRegisterNotBoolModel = ApiMailTemplateValidationBaseModel;
                notifyRegisterNotBoolModel.NotifyRegister = "aaa";

                // NotifyUpdate がnull
                var notifyUpdateNullModel = ApiMailTemplateValidationBaseModel;
                notifyUpdateNullModel.NotifyUpdate = null;

                // NotifyUpdate がboolでない
                var notifyUpdateNotBoolModel = ApiMailTemplateValidationBaseModel;
                notifyUpdateNotBoolModel.NotifyUpdate = "aaa";

                // NotifyDelete がnull
                var notifyDeleteNullModel = ApiMailTemplateValidationBaseModel;
                notifyDeleteNullModel.NotifyDelete = null;

                // NotifyDelete がboolでない
                var notifyDeleteNotBoolModel = ApiMailTemplateValidationBaseModel;
                notifyDeleteNotBoolModel.NotifyDelete = "aaa";

                return new List<ApiMailTemplateModel>()
                {
                    apiIdNullModel,
                    apiIdNotGuidModel,
                    vendorIdNullModel,
                    vendorIdNotGuidModel,
                    mailTemplateIdNullModel,
                    mailTemplateIdNotGuidModel,
                    notifyRegisterNullModel,
                    notifyRegisterNotBoolModel,
                    notifyUpdateNullModel,
                    notifyUpdateNotBoolModel,
                    notifyDeleteNullModel,
                    notifyDeleteNotBoolModel
                };
            }
        }


        /// <summary>
        /// APIメールテンプレート異常系データ(UpdateApiMailTemplate)
        /// </summary>
        public List<ApiMailTemplateModel> UpdateApiMailTemplateValidationErrorData2
        {
            get
            {
                // 新規と同じ
                var baseModel = UpdateApiMailTemplateValidationErrorData;

                // ApiMailTemplateId がnull
                var ApiMailTemplateIdNullModel = ApiMailTemplateValidationBaseModel;
                ApiMailTemplateIdNullModel.ApiMailTemplateId = null;

                // ApiMailTemplateId がGuidでない
                var ApiMailTemplateIdNotGuidModel = ApiMailTemplateValidationBaseModel;
                ApiMailTemplateIdNotGuidModel.ApiMailTemplateId = "aaa";

                baseModel.Add(ApiMailTemplateIdNullModel);
                baseModel.Add(ApiMailTemplateIdNotGuidModel);

                return baseModel;
            }
        }

        /// <summary>
        /// APIメールテンプレート異常系データ(GetApiMailTemplate)
        /// </summary>
        public List<string> GetApiMailTemplateValidationErrorData
        {
            get
            {
                return new List<string>()
                {
                    // ApiMailTemplateIdがない。
                    null,
                    // ApiMailTemplateIdがGuidでない。
                    "hogehoge"
                };
            }
        }

        /// <summary>
        /// APIメールテンプレート異常系データ(GetApiMailTemplateList)
        /// </summary>
        public List<GetListParams> GetApiMailTemplateListValidationErrorData
        {
            get
            {
                // ApiId がnull
                var apiIdNullModel = GetListParamsValidationBaseModel;
                apiIdNullModel.ApiId = null;

                // ApiId がGuidでない
                var apiIdNotGuidModel = GetListParamsValidationBaseModel;
                apiIdNotGuidModel.ApiId = "aaa";

                // VendorId がGuidでない
                var vendorIdNotGuidModel = GetListParamsValidationBaseModel;
                vendorIdNotGuidModel.VendorId = "aaa";

                return new List<GetListParams>()
                {
                    apiIdNullModel,
                    apiIdNotGuidModel,
                    vendorIdNotGuidModel
                };
            }
        }

        /// <summary>
        /// APIメールテンプレート異常系データ(DeleteApiMailTemplate)
        /// </summary>
        public List<string> DeleteApiMailTemplateValidationErrorData
            => DeepCopy(GetApiMailTemplateValidationErrorData);

        [Serializable]
        public class GetListParams
        {
            public string ApiId { get; set; }
            public string VendorId { get; set; }
        }
        #endregion
    }
}
