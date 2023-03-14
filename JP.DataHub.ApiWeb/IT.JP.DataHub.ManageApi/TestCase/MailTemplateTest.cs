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
    public partial class MailTemplateTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// メールテンプレートの正常系テスト
        /// </summary>
        [TestMethod]
        public void MailTemplate_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IMailTemplateApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetMailTemplateList(AppConfig.AdminVendorId)).Assert(GetExpectStatusCodes);

            var listDelete = list.Result.Where(x => MailTemplateNameRegData.Contains(x.MailTemplateName) == true).Select(x => x.MailTemplateId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteMailTemplate(x)).Assert(DeleteExpectStatusCodes));

            // 新規登録
            var regObj = MailTemplateRegData;
            regObj.VendorId = AppConfig.AdminVendorId;
            var regId = client.GetWebApiResponseResult(api.RegisterMailTemplate(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.MailTemplateId;

            #region RegisterMailTemplate:Error系
            //VendorId FkError
            {
                var fkData = MailTemplateRegData;
                fkData.VendorId = Guid.NewGuid().ToString();

                client.GetWebApiResponseResult(api.RegisterMailTemplate(fkData)).Assert(BadRequestStatusCode);
            }
            // MailTemplateName重複登録エラー
            {
                // MailTemplateNameが重複するような登録データを投げる。
                client.GetWebApiResponseResult(api.RegisterMailTemplate(regObj)).Assert(BadRequestStatusCode);
            }
            #endregion

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetMailTemplate(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 関係ないIDで検索（NotFound)
            client.GetWebApiResponseResult(api.GetMailTemplate(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // 更新
            var updObj = MailTemplateUpdData;
            updObj.MailTemplateId = regId;
            updObj.VendorId = AppConfig.AdminVendorId;

            var updData = client.GetWebApiResponseResult(api.UpdateMailTemplate(updObj)).Assert(RegisterSuccessExpectStatusCode).Result.MailTemplateId;

            #region UpdateMailTemplate：Error系
            //VendorId FkError
            {
                var fkObj = MailTemplateUpdData;
                fkObj.MailTemplateId = regId;
                fkObj.VendorId = Guid.NewGuid().ToString();
                client.GetWebApiResponseResult(api.UpdateMailTemplate(fkObj)).Assert(BadRequestStatusCode);
            }
            // MailTemplateName重複登録エラー
            {
                // 新規レコードを追加し、
                var duplicateObjId = client.GetWebApiResponseResult(api.RegisterMailTemplate(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.MailTemplateId;

                // MailTemplateNameが重複するような更新データを投げる。
                var duplicateObj = MailTemplateUpdData;
                duplicateObj.MailTemplateId = duplicateObjId;
                duplicateObj.VendorId = AppConfig.AdminVendorId;

                client.GetWebApiResponseResult(api.UpdateMailTemplate(duplicateObj)).Assert(BadRequestStatusCode);

                // 後始末：新規レコードは削除しておく。
                client.GetWebApiResponseResult(api.DeleteMailTemplate(duplicateObjId)).Assert(DeleteExpectStatusCodes);
            }
            #endregion

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetMailTemplate(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.MailTemplateId.Is(getUpdData.MailTemplateId);
            // 新規登録したものを更新登録したものが異なること（更新したため）
            getRegData.IsNotStructuralEqual(getUpdData);

            // 一覧取得ができること
            var getlist = client.GetWebApiResponseResult(api.GetMailTemplateList(AppConfig.AdminVendorId)).Assert(GetSuccessExpectStatusCode).Result;
            // リストの中に新規登録したデータ（登録後、更新したデータ）が存在することを確認
            var isExists = getlist.Exists(x => x.MailTemplateId == getUpdData.MailTemplateId
                                                           && x.MailTemplateName == getUpdData.MailTemplateName);
            Assert.IsTrue(isExists); // 新規登録したデータ（登録後、更新したデータ）が存在する

            // 削除
            client.GetWebApiResponseResult(api.DeleteMailTemplate(regId)).Assert(DeleteSuccessStatusCode);

            // 削除したものを再度削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteMailTemplate(regId)).Assert(NotFoundStatusCode);
            // 削除したものを取得(NotFound)
            client.GetWebApiResponseResult(api.GetMailTemplate(regId)).Assert(NotFoundStatusCode);
            // 削除したものを更新(NotFound)
            client.GetWebApiResponseResult(api.UpdateMailTemplate(updObj)).Assert(BadRequestStatusCode);
        }

        /// <summary>
        /// メールテンプレートのエラー系テスト
        /// </summary>
        [TestMethod]
        public void MailTemplate_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IMailTemplateApi>();

            // RegisterMailTemplateのValidationError
            RegisterMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterMailTemplate(x)).Assert(BadRequestStatusCode)
            );

            // RegisterMailTemplateのNullBody
            client.GetWebApiResponseResult(api.RegisterMailTemplate(null)).Assert(BadRequestStatusCode);

            // UpdateMailTemplateのValidationError
            UpdateMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateMailTemplate(x)).Assert(BadRequestStatusCode)
            );

            // UpdateMailTemplateのNullBody
            client.GetWebApiResponseResult(api.UpdateMailTemplate(null)).Assert(BadRequestStatusCode);

            // GetMailTemplateのValidationError
            GetMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetMailTemplate(x)).Assert(BadRequestStatusCode)
            );

            // GetMailTemplateListのValidationError
            GetMailTemplateListValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetMailTemplateList(x)).Assert(BadRequestStatusCode)
            );

            // DeleteMailTemplateのValidationError
            DeleteMailTemplateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteMailTemplate(x)).Assert(BadRequestStatusCode)
            );
        }

        #region Data

        public static string MailTemplateNameRegData = "---itRegisterNet6---";
        public static string MailTemplateNameUpdData = "---itUpdateNet6---";

        /// <summary>
        /// メールテンプレート正常系データ
        /// </summary>
        public static MailTemplateModel MailTemplateRegData
        {
            get =>
                new MailTemplateModel()
                {
                    MailTemplateId = null,
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateName = MailTemplateNameRegData,
                    From = "from@example.com",
                    To = new string[] { "to@example.com", "to1@example.com" },
                    Cc = new string[] { "cc@example.com", "cc1@example.com" },
                    Bcc = new string[] { "bcc@example.com", "bcc1@example.com" },
                    Subject = "subject",
                    Body = "body"
                };
        }

        public static MailTemplateModel MailTemplateUpdData
        {
            get =>
                new MailTemplateModel()
                {
                    MailTemplateId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateName = MailTemplateNameUpdData,
                    From = "updfrom@example.com",
                    To = new string[] { "updto@example.com", "to1@example.com" },
                    Cc = new string[] { "updcc@example.com", "cc1@example.com" },
                    Bcc = new string[] { "updbcc@example.com", "bcc1@example.com" },
                    Subject = "updsubject",
                    Body = "updbody"
                };
        }

        #endregion

        #region Validation/Error
        public string MailTemplateNameErrorCaseRegData = "---itRegister-ErrorCase---";
        public MailTemplateModel MailTemplateValidationBaseModel
        {
            get =>
                new MailTemplateModel()
                {
                    MailTemplateId = Guid.NewGuid().ToString(),
                    VendorId = Guid.NewGuid().ToString(),
                    MailTemplateName = MailTemplateNameErrorCaseRegData,
                    From = "updfrom@example.com",
                    To = new string[] { "updto@example.com", "to1@example.com" },
                    Cc = null, //null許可項目
                    Bcc = null, //null許可項目
                    Subject = "updsubject",
                    Body = "updbody"
                };
        }


        /// <summary>
        /// メールテンプレート異常系データ(RegisterMailTemplate)
        /// </summary>
        public List<MailTemplateModel> RegisterMailTemplateValidationErrorData
        {
            get
            {
                // VendorId がnull
                var vendorIdNullModel = DeepCopy(MailTemplateValidationBaseModel);
                vendorIdNullModel.VendorId = null;

                // VendorId がGuidでない
                var vendorIdNotGuidModel = DeepCopy(MailTemplateValidationBaseModel);
                vendorIdNotGuidModel.VendorId = "aaa";

                // MailTemplateName がnull
                var mailTemplateNameNullModel = DeepCopy(MailTemplateValidationBaseModel);
                mailTemplateNameNullModel.MailTemplateName = null;

                // MailTemplateName が100桁を超える
                var mailTemplateNameMaxLengthOverModel = DeepCopy(MailTemplateValidationBaseModel);
                mailTemplateNameMaxLengthOverModel.MailTemplateName = new string('a', 101);

                // From がnull
                var fromNullModel = DeepCopy(MailTemplateValidationBaseModel);
                fromNullModel.From = null;

                // From が1000桁を超える
                var fromeNameMaxLengthOverModel = DeepCopy(MailTemplateValidationBaseModel);
                fromeNameMaxLengthOverModel.From = new string('a', 1001);

                // To がnull
                var toNullModel = DeepCopy(MailTemplateValidationBaseModel);
                toNullModel.To = null;

                // To が1000桁を超える
                var toMaxLengthOverModel = DeepCopy(MailTemplateValidationBaseModel);
                toMaxLengthOverModel.To = new string[] { new string('a', 1001) };

                // Cc が1000桁を超える
                var ccMaxLengthOverModel = DeepCopy(MailTemplateValidationBaseModel);
                ccMaxLengthOverModel.Cc = new string[] { new string('a', 1001) };

                // Bcc が1000桁を超える
                var bcceMaxLengthOverModel = DeepCopy(MailTemplateValidationBaseModel);
                bcceMaxLengthOverModel.Bcc = new string[] { new string('a', 1001) };

                // Subject がnull
                var subjectNullModel = DeepCopy(MailTemplateValidationBaseModel);
                subjectNullModel.Subject = null;

                // Subject が200桁を超える
                var subjectMaxLengthOverModel = DeepCopy(MailTemplateValidationBaseModel);
                subjectMaxLengthOverModel.Subject = new string('a', 201);

                // Body がnull
                var bodyNullModel = DeepCopy(MailTemplateValidationBaseModel);
                bodyNullModel.Body = null;


                return new List<MailTemplateModel>()
                {
                    vendorIdNullModel,
                    vendorIdNotGuidModel,
                    mailTemplateNameNullModel,
                    mailTemplateNameMaxLengthOverModel,
                    fromNullModel,
                    fromeNameMaxLengthOverModel,
                    toNullModel,
                    toMaxLengthOverModel,
                    ccMaxLengthOverModel,
                    bcceMaxLengthOverModel,
                    subjectNullModel,
                    subjectMaxLengthOverModel,
                    bodyNullModel
                };
            }
        }


        /// <summary>
        /// メールテンプレート異常系データ(UpdateMailTemplate)
        /// </summary>
        public List<MailTemplateModel> UpdateMailTemplateValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = DeepCopy(RegisterMailTemplateValidationErrorData);

                // MailTemplateIdがnull
                var MailTemplateIdNullModel = DeepCopy(MailTemplateValidationBaseModel);
                MailTemplateIdNullModel.MailTemplateId = null;

                // MailTemplateIdがGuidでない
                var MailTemplateIdNotGuidModel = DeepCopy(MailTemplateValidationBaseModel);
                MailTemplateIdNotGuidModel.MailTemplateId = "aaa";

                baseModel.Add(MailTemplateIdNullModel);
                baseModel.Add(MailTemplateIdNotGuidModel);

                return baseModel;
            }
        }

        /// <summary>
        /// メールテンプレート異常系データ(GetMailTemplate)
        /// </summary>
        public List<string> GetMailTemplateValidationErrorData
        {
            get
            {
                return new List<string>()
                {
                    // MailTemplateIdがない。
                    null,
                    // MailTemplateIdがGuidでない。
                    "hogehoge"
                };
            }
        }

        /// <summary>
        /// メールテンプレート異常系データ(GetMailTemplateList)
        /// </summary>
        public List<string> GetMailTemplateListValidationErrorData
        {
            get
            {
                // Getと同じ(documentId→vendorIdになっただけ。）
                return DeepCopy(GetMailTemplateValidationErrorData);
            }
        }

        /// <summary>
        /// メールテンプレート異常系データ(DeleteMailTemplate)
        /// </summary>
        public List<string> DeleteMailTemplateValidationErrorData
        {
            get
            {
                // Getと同じ
                return DeepCopy(GetMailTemplateValidationErrorData);
            }
        }
        #endregion
    }
}
