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
    public partial class AgreementTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void Agreement_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAgreementApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetAgreementList()).Assert(GetExpectStatusCodes).Result;

            var listDelete = list.Where(x => UpdDataTitle.Contains(x.Title) == true).Select(x => x.AgreementId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteAgreement(x)).Assert(DeleteExpectStatusCodes));

            // 新規登録
            var regData = RegData(null, AppConfig.AdminVendorId);
            var regId = client.GetWebApiResponseResult(api.RegisterAgreement(regData)).Assert(RegisterSuccessExpectStatusCode).Result.AgreementId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetAgreement(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 更新登録
            var updData = UpdData( regId, AppConfig.AdminVendorId);
            var updId = client.GetWebApiResponseResult(api.UpdateAgreement(updData)).Assert(RegisterSuccessExpectStatusCode).Result.AgreementId;

            // 更新登録したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetAgreement(updId)).Assert(GetSuccessExpectStatusCode).Result;

            // 全件取得
            client.GetWebApiResponseResult(api.GetAgreementList()).Assert(GetSuccessExpectStatusCode);

            // 全件取得(VendorId指定)
            client.GetWebApiResponseResult(api.GetAgreementListByVendorId(AppConfig.AdminVendorId)).Assert(GetSuccessExpectStatusCode);

            // 削除
            client.GetWebApiResponseResult(api.DeleteAgreement(regId)).Assert(DeleteExpectStatusCodes);

            // 削除したものを再度削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteAgreement(regId)).Assert(NotFoundStatusCode);

            // 削除したものを取得(NotFound)
            client.GetWebApiResponseResult(api.GetAgreement(regId)).Assert(NotFoundStatusCode);

            // 削除したものを更新(NotFound)
            client.GetWebApiResponseResult(api.UpdateAgreement(updData)).Assert(BadRequestStatusCode);

            // 新規登録したものを更新登録したものが同じか(IDが同じか)
            getRegData.AgreementId.IsStructuralEqual(getUpdData.AgreementId);
        }

        [TestMethod]
        public void Agreement_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAgreementApi>();
            // RegisterのValidationError
            RegisterValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterAgreement(x)).Assert(BadRequestStatusCode)
            );

            // RegisterのNullBody
            client.GetWebApiResponseResult(api.RegisterAgreement(null)).Assert(BadRequestStatusCode);

            // UpdateのValidationError
            UpdateValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateAgreement(x)).Assert(BadRequestStatusCode)
            );

            // UpdateのNullBody
            client.GetWebApiResponseResult(api.UpdateAgreement(null)).Assert(BadRequestStatusCode);

            // DeleteのValidationError
            DeleteValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteAgreement(x.AgreementId)).Assert(BadRequestStatusCode)
            );

            // GetのValidationError
            GetValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetAgreement(x.AgreementId)).Assert(BadRequestStatusCode)
            );
        }

        #region Data

        public string RegDataTitle = "ITタイトル";
        public string UpdDataTitle = "ITタイトル_更新";

        /// <summary>
        /// 規約正常系データ
        /// </summary>
        public AgreementModel RegData(string id,string vendorId)
        {
            return
                new AgreementModel()
                {
                    AgreementId = id,
                    Detail = "IT内容",
                    Title = RegDataTitle,
                    VendorId = vendorId
                };
        }

        /// <summary>
        /// 規約更新正常系データ
        /// </summary>
        public AgreementModel UpdData(string id, string vendorId)
        {
            return
                new AgreementModel()
                {
                    AgreementId = id,
                    Detail = "IT内容_更新",
                    Title = UpdDataTitle,
                    VendorId = vendorId
                };
        }

        #endregion

        #region Validation

        private AgreementModel ValidationBaseModel = new AgreementModel()
        {
            AgreementId = Guid.NewGuid().ToString(),
            Detail = "IT_Validation_Detail",
            Title = "IT_Validation_Title",
            VendorId = Guid.NewGuid().ToString()
        };

        /// <summary>
        /// 異常系データ(Register)
        /// </summary>
        public List<AgreementModel> RegisterValidationErrorData
        {
            get
            {
                // TitleがNull
                var titleNullModel = DeepCopy(ValidationBaseModel);
                titleNullModel.Title = null;

                // Titleが文字数オーバー
                var titleOverModel = DeepCopy(ValidationBaseModel);
                titleOverModel.Title = new string('a', 1001);

                // 内容がNull
                var detailNullModel = DeepCopy(ValidationBaseModel);
                detailNullModel.Detail = null;

                // VendorIdがNull
                var vendorIdNullModel = DeepCopy(ValidationBaseModel);
                vendorIdNullModel.VendorId = null;

                return new List<AgreementModel>()
                {
                    titleNullModel,
                    titleOverModel,
                    detailNullModel,
                    vendorIdNullModel
                };
            }
        }

        /// <summary>
        /// 異常系データ(Update)
        /// </summary>
        public List<AgreementModel> UpdateValidationErrorData
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
        public List<AgreementModel> DeleteValidationErrorData
        {
            get
            {
                // AgreementIdがnull
                var agreementIdNullModel = DeepCopy(ValidationBaseModel);
                agreementIdNullModel.AgreementId = null;

                // AgreementIdがGuidでない
                var agreementIdNotGuidModel = DeepCopy(ValidationBaseModel);
                agreementIdNotGuidModel.AgreementId = "hoge";

                return new List<AgreementModel>()
                {
                    agreementIdNullModel,
                    agreementIdNotGuidModel
                };
            }
        }

        /// <summary>
        /// 異常系データ(Get)
        /// </summary>
        public List<AgreementModel> GetValidationErrorData
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
