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
    public partial class InformationTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        /// <summary>
        /// お知らせの正常系テスト
        /// </summary>
        [TestMethod]
        public void Information_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IInformationApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetInformationList()).Assert(GetExpectStatusCodes);

            var listDelete = list.Result.Where(x => InformationRegDataTitle.Contains(x.Title) == true).Select(x => x.InformationId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteInformation(x)).Assert(DeleteExpectStatusCodes));

            // 新規登録
            string date = DateTime.Now.ToString("yyyy/MM/dd hh:mm");
            var regData = InformationRegData(null, date);
            var regId = client.GetWebApiResponseResult(api.RegisterInformation(regData)).Assert(GetExpectStatusCodes).Result.InformationId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetInformation(regId)).Assert(GetExpectStatusCodes).Result;

            // 更新登録
            var updData = InformationRegData(regId, date);
            var updId = client.GetWebApiResponseResult(api.UpdateInformation(updData)).Assert(GetExpectStatusCodes).Result.InformationId;

            // 新規登録したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetInformation(updId)).Assert(GetSuccessExpectStatusCode).Result;

            // 全件取得
            client.GetWebApiResponseResult(api.GetInformationList()).Assert(GetSuccessExpectStatusCode);

            // 削除
            client.GetWebApiResponseResult(api.DeleteInformation(regId)).Assert(DeleteExpectStatusCodes);

            // 削除したものを再度削除
            client.GetWebApiResponseResult(api.DeleteInformation(regId)).Assert(NotFoundStatusCode);

            // 新規登録したものを更新登録したものが同じか
            getRegData.IsStructuralEqual(getUpdData);
        }

        /// <summary>
        /// お知らせの異常系テスト
        /// </summary>
        [TestMethod]
        public void Information_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IInformationApi>();

            // RegisterInformationのValidationError
            RegisterInformationValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterInformation(x)).Assert(BadRequestStatusCode)
            );

            // RegisterInformationのNullBody
            client.GetWebApiResponseResult(api.RegisterInformation(null)).Assert(BadRequestStatusCode);

            // UpdateInformationのValidationError
            UpdateInformationValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.UpdateInformation(x)).Assert(BadRequestStatusCode)
            );

            // UpdateInformationのNullBody
            client.GetWebApiResponseResult(api.UpdateInformation(null)).Assert(BadRequestStatusCode);

            // DeleteInformationのValidationError
            DeleteInformationValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteInformation(x.InformationId)).Assert(BadRequestStatusCode)
            );

            // GetInformationのValidationError
            GetInformationValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetInformation(x.InformationId)).Assert(BadRequestStatusCode)
            );
        }
        public string InformationRegDataTitle = "IntegratedTestInformation";

        /// <summary>
        /// お知らせ正常系データ
        /// </summary>
        public InformationModel InformationRegData(string id,string date)
        {
            return new InformationModel()
            {
                InformationId = id,
                Title = InformationRegDataTitle,
                Detail = "ITで登録",
                Date = DateTime.Now.ToString("yyyy/MM/dd hh:mm"),
                IsVisibleAdmin = false,
                IsVisibleApi = false
            };
        }

        #region Validation

        private InformationModel InformationValidationBaseModel = new InformationModel()
        {
            InformationId = Guid.NewGuid().ToString(),
            Title = "InformationValidationErrorData_Title",
            Detail = "InformationValidationErrorData_Detail",
            Date = DateTime.Now.ToString("yyyy/MM/dd hh:mm"),
            IsVisibleAdmin = false,
            IsVisibleApi = false
        };

        /// <summary>
        /// お知らせ異常系データ(RegisterInformation)
        /// </summary>
        public List<InformationModel> RegisterInformationValidationErrorData
        {
            get
            {
                // Titleがnull
                var titleNullModel = DeepCopy(InformationValidationBaseModel);
                titleNullModel.Title = null;

                // Titleが文字数オーバー
                var titleOverModel = DeepCopy(InformationValidationBaseModel);
                titleOverModel.Title = new string('a', 513);

                // 内容がNULL
                var detailNullModel = DeepCopy(InformationValidationBaseModel);
                detailNullModel.Detail = null;

                // 日付がNULL
                var dateNullModel = DeepCopy(InformationValidationBaseModel);
                dateNullModel.Date = null;

                // 日付が日付型でない
                var dateNotDateTypeModel = DeepCopy(InformationValidationBaseModel);
                dateNotDateTypeModel.Date = "hoge";

                return new List<InformationModel>()
                {
                    titleNullModel,
                    titleOverModel,
                    detailNullModel,
                    dateNullModel,
                };
            }
        }

        /// <summary>
        /// お知らせ異常系データ(UpdateInformation)
        /// </summary>
        public List<InformationModel> UpdateInformationValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = DeepCopy(RegisterInformationValidationErrorData);
                return baseModel;
            }
        }

        /// <summary>
        /// お知らせ異常系データ(DeleteInformation)
        /// </summary>
        public List<InformationModel> DeleteInformationValidationErrorData
        {
            get
            {
                // InformationIdがnull
                var InformationIdNullModel = DeepCopy(InformationValidationBaseModel);
                InformationIdNullModel.InformationId = null;

                // InformationIdがGuidでない
                var InformationIdNotGuidModel = DeepCopy(InformationValidationBaseModel);
                InformationIdNotGuidModel.InformationId = "hoge";

                return new List<InformationModel>()
                {
                    InformationIdNullModel,
                    InformationIdNotGuidModel
                };
            }
        }

        /// <summary>
        /// お知らせ異常系データ(GetInformation)
        /// </summary>
        public List<InformationModel> GetInformationValidationErrorData
        {
            get
            {
                // 削除と同じ
                var baseModel = DeepCopy(DeleteInformationValidationErrorData);
                return baseModel;
            }
        }

        #endregion
    }
}
