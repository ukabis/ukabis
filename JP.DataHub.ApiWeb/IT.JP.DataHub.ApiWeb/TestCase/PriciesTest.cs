using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [Ignore("廃止：不要機能")]
    [TestClass]
    public class PriciesTest : ApiWebItTestCase
    {
#if false
        private WebApiContext context;
        private PriciesApi resource;
        private string dynamicApiTestApiId;
        private string manageApiTestApiId;

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();

            context = new WebApiContext(TestContext);
            resource = new PriciesApi(context);
            var manage = new DynamicApi(context);

            // テスト対象のAPIのContollerIdを取得
            // ManageApi
            var manageApiTestApi = manage.GetApiResourceFromUrl(resource.DynamicApiManageApiPriciesTest.TrimEnd('/'));
            var manageTestData = context.ActionAndAssert(manageApiTestApi.Request, resource.GetSuccessExpectStatusCode).ToJson();
            manageApiTestApiId = manageTestData["ApiId"].ToString();

            // DynamicApi
            var dynamicApiTestApi = manage.GetApiResourceFromUrl(resource.DynamicApiPriciesTest.TrimEnd('/'));
            var dynamicTestData = context.ActionAndAssert(dynamicApiTestApi.Request, resource.GetSuccessExpectStatusCode).ToJson();
            dynamicApiTestApiId = dynamicTestData["ApiId"].ToString();
        }

        /// <summary>
        /// Priciesに関するStaticApiのテスト
        /// </summary>
        [TestMethod]
        public void PriciesStaticApiNormalSenario()
        {
            var apiId = manageApiTestApiId;

            // 最初に削除
            context.ActionAndAssert(resource.Delete(manageApiTestApiId).Request, resource.DeleteExpectStatusCodes);

            // 登録
            var regData = resource.RegisterPriciesData.Replace("@ApiId", apiId);
            var regResult = context.ActionAndAssert(resource.Register(regData).Request, resource.RegistSuccessExpectStatusCode).ToJson();

            // 取得
            var getRegData = context.ActionAndAssert(resource.PriciesGet(apiId).Request, resource.GetSuccessExpectStatusCode).ToJson();

            // 取得(前後の空白があっても取得できるか)
            context.ActionAndAssert(resource.PriciesGet(" " + apiId + " ").Request, resource.GetSuccessExpectStatusCode);

            // 登録値と取得値が同じか
            regData.IsStructuralEqual(getRegData.ToString());

            // 更新
            var updData = resource.UpdatePriciesData.Replace("@ApiId", apiId);
            var updResult = context.ActionAndAssert(resource.Register(updData).Request, resource.RegistSuccessExpectStatusCode).ToJson();

            // 更新したものを取得
            var getUpdData = context.ActionAndAssert(resource.PriciesGet(apiId).Request, resource.GetSuccessExpectStatusCode).ToJson();

            // 更新値と取得値が同じか
            updData.IsStructuralEqual(getUpdData.ToString());

        #region 各項目の取得API

            context.ActionAndAssert(resource.GetPriceExcludingPurchaseTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["PriceExcludingPurchaseTax"]);
            context.ActionAndAssert(resource.GetPriceIncludingPurchaseTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["PriceIncludingPurchaseTax"]);
            context.ActionAndAssert(resource.GetPriceExcludingSalesTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["PriceExcludingSalesTax"]);
            context.ActionAndAssert(resource.GetPriceIncludingSalesTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["PriceIncludingSalesTax"]);
            context.ActionAndAssert(resource.GetSwitchedDatetime(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SwitchedDatetime"]);
            context.ActionAndAssert(resource.GetSwitchedPriceExcludingPurchaseTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SwitchedPriceExcludingPurchaseTax"]);
            context.ActionAndAssert(resource.GetSwitchedPriceIncludingPurchaseTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SwitchedPriceIncludingPurchaseTax"]);
            context.ActionAndAssert(resource.GetSwitchedPriceExcludingSalesTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SwitchedPriceExcludingSalesTax"]);
            context.ActionAndAssert(resource.GetSwitchedPriceIncludingSalesTax(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SwitchedPriceIncludingSalesTax"]);
            context.ActionAndAssert(resource.GetSalesStartDatetime(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SalesStartDatetime"]);
            context.ActionAndAssert(resource.GetSalesEndDatetime(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["SalesEndDatetime"]);
            context.ActionAndAssert(resource.GetPublicStartDatetime(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["PublicStartDatetime"]);
            context.ActionAndAssert(resource.GetPublicEndDatetime(apiId).Request, resource.GetSuccessExpectStatusCode, getUpdData["PublicEndDatetime"]);

        #endregion

            // 削除
            context.ActionAndAssert(resource.Delete(apiId).Request, resource.DeleteExpectStatusCodes);

            // 削除後NotFound
            context.ActionAndAssert(resource.PriciesGet(apiId).Request, resource.NotFoundStatusCode);
        }

        /// <summary>
        /// Priciesに関するStaticApiのエラー系テスト
        /// </summary>
        [TestMethod]
        public void PriciesStaticApiErrorSenario()
        {
            // 取得系のエラーテスト
            this.PriciesGetErrorSenario();

            // 登録のエラーテスト
            resource.RegisterErrorDataList.ForEach(x =>
                context.ActionAndAssert(resource.Register(JsonConvert.SerializeObject(x)).Request, resource.BadRequestStatusCode)
            );

            // 削除のエラーテスト
            context.ActionAndAssert(resource.Delete(null).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.Delete("hoge").Request, resource.BadRequestStatusCode);
        }

        /// <summary>
        /// 取得系のValidation
        /// </summary>
        private void PriciesGetErrorSenario()
        {
            string apiIdIsNull = null;
            string apiIdFormatError = "hoge";

            context.ActionAndAssert(resource.PriciesGet(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.PriciesGet(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceExcludingPurchaseTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceExcludingPurchaseTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceIncludingPurchaseTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceIncludingPurchaseTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceExcludingSalesTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceExcludingSalesTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceIncludingSalesTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPriceIncludingSalesTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedDatetime(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedDatetime(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceExcludingPurchaseTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceExcludingPurchaseTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceIncludingPurchaseTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceIncludingPurchaseTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceExcludingSalesTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceExcludingSalesTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceIncludingSalesTax(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSwitchedPriceIncludingSalesTax(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSalesStartDatetime(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSalesStartDatetime(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSalesEndDatetime(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetSalesEndDatetime(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPublicStartDatetime(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPublicStartDatetime(apiIdFormatError).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPublicEndDatetime(apiIdIsNull).Request, resource.BadRequestStatusCode);
            context.ActionAndAssert(resource.GetPublicEndDatetime(apiIdFormatError).Request, resource.BadRequestStatusCode);
        }

        private void PriciesRegisterErrorSenario()
        {

        }

        /// <summary>
        /// Priciesに関するDynamicApiのテスト
        /// </summary>
        [TestMethod]
        public void PriciesDynamicApiNormalSenario()
        {
            context.SetCache(true);
            // Priciesが存在するAPI(公開期間のものしか取得できない)
            PriciesDynamicApiTest(true);

            // Priciesが存在しないAPI(公開期間関係なく取得できる(既存))
            PriciesDynamicApiTest(false);
        }

        /// <summary>
        /// 秒単位で指定すると呼び出し中に公開期間を過ぎてしまうので10分単位で指定
        /// </summary>
        /// <param name="existPricies">APIに紐づくPriciesが存在するか</param>
        private void PriciesDynamicApiTest(bool existPricies)
        {
            // 引数にあわせてPriciesの登録及び削除する
            if (existPricies)
            {
                var regData = resource.RegisterPriciesData.Replace("@ApiId", dynamicApiTestApiId);
                context.ActionAndAssert(resource.Register(regData).Request, resource.RegistSuccessExpectStatusCode);
            }
            else
            {
                context.ActionAndAssert(resource.Delete(dynamicApiTestApiId).Request, resource.DeleteExpectStatusCodes);
            }

            var TestUtcDate = DateTime.UtcNow;

        #region 公開期間内(OK or NotFound)

            // 当日～数日以内
            DynamicApiAssert(TestUtcDate, TestUtcDate.AddDays(10), true, existPricies);
            // 前日～当日(now+10分後)
            DynamicApiAssert(TestUtcDate.AddDays(-1), TestUtcDate.AddMinutes(10), true, existPricies);

        #endregion

        #region  公開期間外(BadRequest)

            // 数日前～前日
            DynamicApiAssert(TestUtcDate.AddDays(-10), TestUtcDate.AddDays(-1), !existPricies, existPricies);
            // 前日～当日(now-10分前)
            DynamicApiAssert(TestUtcDate.AddDays(-1), TestUtcDate.AddMinutes(-10), !existPricies, existPricies);
            // 当日(now+10分後)～数日後
            DynamicApiAssert(TestUtcDate.AddMinutes(10), TestUtcDate.AddMinutes(5), !existPricies, existPricies);

        #endregion
        }

        private void DynamicApiAssert(DateTime start, DateTime end, bool isSuccess, bool isUpdate)
        {
            if (isUpdate)
            {
                var regData = resource.PublicDateTimeTestData.
                    Replace("@ApiId", dynamicApiTestApiId).
                    Replace("@PublicStartDatetime", start.ToString()).
                    Replace("@PublicEndDatetime", end.ToString());
                context.ActionAndAssert(resource.Register(regData).Request, resource.RegistSuccessExpectStatusCode);
            }

            if (isSuccess)
            {
                context.ActionAndAssert(resource.GetAll().Request, resource.GetExpectStatusCodes);
            }
            else
            {
                context.ActionAndAssert(resource.GetAll().Request, resource.BadRequestStatusCode);
            }
        }
#endif
    }
}
