using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.Api;
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
    public partial class ApiTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }
        /// <summary>
        /// RegisterStaticApiの正常系テスト
        /// </summary>
        [TestMethod]
        public void RegisterStaticApi_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiApi>();

            // ソース上に指定のIDに対応するControllerがある かつ DBにIDに対応する有効なControllerがある(更新)
            var updResult = client.GetWebApiResponseResult(api.RegisterStaticApi(RegisterStaticApiRegisterData)).Assert(RegisterSuccessExpectStatusCode).Result;
            updResult.IsStructuralEqual(RegisterStaticApiResponseData);
        }

        /// <summary>
        /// RegisterStaticApiの異常系テスト
        /// </summary>
        [TestMethod]
        public void RegisterStaticApi_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiApi>();

            var obj = RegisterStaticApiRegisterData;

            // ApiIdがnull
            obj.ApiId = null;
            client.GetWebApiResponseResult(api.RegisterStaticApi(obj)).Assert(BadRequestStatusCode);
            // ApiIdがGUIDでない
            obj.ApiId = "hoge";
            client.GetWebApiResponseResult(api.RegisterStaticApi(obj)).Assert(BadRequestStatusCode);
            // ソース上に指定のIDに対応するControllerがない かつ DBにIDに対応する有効なControllerがない
            obj.ApiId = Guid.NewGuid().ToString();
            client.GetWebApiResponseResult(api.RegisterStaticApi(obj)).Assert(BadRequestStatusCode);
            // Bodyがnull
            client.GetWebApiResponseResult(api.RegisterStaticApi(null)).Assert(BadRequestStatusCode);
            // BodyがJSONでない
           // client.GetWebApiResponseResult(api.RegisterStaticApi("hoge")).Assert(BadRequestStatusCode);
        }
        #region Data
        public RegisterStaticApiRequestModel RegisterStaticApiRegisterData
        {
            get => new RegisterStaticApiRequestModel
            {
                ApiId = "4987fa7d-f709-4a48-a04e-3bc0230ed887" // RegisterStaticApi自身が含まれるID(常に存在するID)
            };
        }

        public RegisterStaticApiResponseModel RegisterStaticApiResponseData
        {
            get => new RegisterStaticApiResponseModel
            {
                ApiId = "4987fa7d-f709-4a48-a04e-3bc0230ed887", // RegisterStaticApi自身が含まれるID(常に存在するID)
                Url = "/Manage/Api",
                Message = "Registered or Updated" // 登録更新時Message
            };
        }
        #endregion

    }
}
