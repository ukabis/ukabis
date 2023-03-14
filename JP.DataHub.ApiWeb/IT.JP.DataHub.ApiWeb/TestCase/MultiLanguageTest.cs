using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// マルチランゲージのテスト
    /// (英語が有効である必要があるためマルチランゲージ有効な環境のみ実行する)
    /// </summary>
    [Ignore("廃止：ポータル向け機能")]
    [TestClass]
    public class MultiLanguageTest : ApiWebItTestCase
    {
#if false
        [TestInitialize]
        public void TestInitialize()
        {
            Initialize();
        }


        #region GetApiDescription

        private readonly Dictionary<string, string[]> ApiExpectedTexts = new Dictionary<string, string[]>()
        {
            { "ControllerName", new string[] { "ControllerName日本語", "ControllerName英語" } },
            { "Documentation", new string[] { "Documentation日本語", "Documentation英語" } },
            { "FeeDescription", new string[] { "FeeDescription日本語", "FeeDescription英語" } },
            { "ResourceCreateUser", new string[] { "ResourceCreateUser日本語", "ResourceCreateUser英語" } },
            { "ResourceMaintainer", new string[] { "ResourceMaintainer日本語", "ResourceMaintainer英語" } },
            { "UpdateFrequency", new string[] { "UpdateFrequency日本語", "UpdateFrequency英語" } },
            { "ContactInformation", new string[] { "ContactInformation日本語", "ContactInformation英語" } },
            { "Version", new string[] { "Version日本語", "Version英語" } },
            { "AgreeDescription", new string[] { "AgreeDescription日本語", "AgreeDescription英語" } }
        };

        private readonly Dictionary<string, string[]> MethodExpectedTexts = new Dictionary<string, string[]>()
        {
            { "Documentation", new string[] { "Documentation日本語", "Documentation英語" } }
        };


        /// <summary>
        /// GetApiDescriptionのマルチランゲージ対応テスト
        /// </summary>
        [TestMethod]
        public void GetApiDescription_Multilanguage()
        {
            const string apiRelativeUrl = "/API/IntegratedTest/ManageApi/ApiDescription";
            const string methodRelativeUrl = "Get";
            var locales = new string[] { "ja", "en", "de" };

            foreach (var locale in locales)
            {
                var context = new WebApiContext(TestContext);
                context.HttpHeader.Add("Accept-Language", locale);
                var resource = new AdminApi.ManageApiApi(context);
                var result = context.ActionAndAssert<List<GetApiDescriptionResultJson>>(resource.GetApiDescription(false).Request, resource.GetSuccessExpectStatusCode);

                var api = result.Single(x => x.RelativeUrl == apiRelativeUrl);
                ValidateApi(api, locale);

                var method = api.ApiList.Single(y => y.MethodRelativeUrl == methodRelativeUrl);
                ValidateMethod(method, locale);
            }
        }


        private void ValidateApi(GetApiDescriptionResultJson api, string locale)
        {
            var index = (locale == "en" ? 1 : 0);
            api.ControllerName.Is(ApiExpectedTexts["ControllerName"][index]);
            api.Documentation.Is(ApiExpectedTexts["Documentation"][index]);
            api.FeeDescription.Is(ApiExpectedTexts["FeeDescription"][index]);
            api.ResourceCreateUser.Is(ApiExpectedTexts["ResourceCreateUser"][index]);
            api.ResourceMaintainer.Is(ApiExpectedTexts["ResourceMaintainer"][index]);
            api.UpdateFrequency.Is(ApiExpectedTexts["UpdateFrequency"][index]);
            api.ContactInformation.Is(ApiExpectedTexts["ContactInformation"][index]);
            api.Version.Is(ApiExpectedTexts["Version"][index]);
            api.AgreeDescription.Is(ApiExpectedTexts["AgreeDescription"][index]);
        }

        private void ValidateMethod(GetApiDescriptionApiListItem method, string locale)
        {
            var index = (locale == "en" ? 1 : 0);
            method.Documentation.Is(MethodExpectedTexts["Documentation"][index]);
        }

        #endregion

        #region GetSchemaDescription

        private Dictionary<string, string[]> SchemaExpectedTexts = new Dictionary<string, string[]>()
        {
            { "Documentation", new string[] { "Documentation日本語", "Documentation英語" } }
        };


        /// <summary>
        /// GetSchemaDescriptionのマルチランゲージ対応テスト
        /// </summary>
        [TestMethod]
        public void GetSchemaDescription_Multilanguage()
        {
            const string schemaName = "/API/IntegratedTest/ManageApi/ApiDescription";
            var locales = new string[] { "ja", "en", "de" };

            foreach (var locale in locales)
            {
                var context = new WebApiContext(TestContext);
                context.HttpHeader.Add("Accept-Language", locale);
                var resource = new AdminApi.ManageApiApi(context);
                var result = context.ActionAndAssert<List<GetSchemaDescriptionResultJson>>(resource.GetSchemaDescription().Request, resource.GetSuccessExpectStatusCode);

                var schema = result.Single(x => x.SchemaName == schemaName);
                var index = (locale == "en" ? 1 : 0);
                schema.SampleData.Is(SchemaExpectedTexts["Documentation"][index]);
            }
        }
        #endregion

        [TestMethod]
        public void GetRFC7807_Multilanguage()
        {
            string enTitle = "one or more errors occurred.";
            string jaTitle = "1つ以上のエラーが発生しました。";

            var expectedTexts = new Dictionary<string, string[]>
            {
                { "en", new string[]{ enTitle}},
                { "ja", new string[]{ jaTitle}},
                { "de", new string[]{ jaTitle,enTitle}},
            };

            foreach (var locale in expectedTexts.Keys)
            {
                WebApiContext contextX = new WebApiContext(TestContext);
                contextX.HttpHeader.Add("Accept-Language", locale);
                var resource = new MultiLanguageApi(contextX);

                //Jsonバリデーションエラーを発生させる為
                var api = resource.Register("{{{test");

                var res = contextX.ActionAndAssertErrorCode(api.Request, resource.BadRequestStatusCode, "E10403").ToJson(); ;
                expectedTexts[locale].Contains(res["title"].ToString()).Is(true);
            }
        }
#endif
    }
}
