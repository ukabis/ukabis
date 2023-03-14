using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ForeignKeyTest : ApiWebItTestCase
    {
        #region TestData

        private class ForeignKeyTestData : TestDataBase
        {
            public CropModel DataCrop1 = new CropModel()
            {
                CropCode = "001",
                CropName = "トマト"
            };
            public RegisterResponseModel DataCrop1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Crop~1~001"
            };
            public CropModel Data1Get = new CropModel()
            {
                CropCode = "001",
                CropName = "トマト",
                id = $"API~IntegratedTest~Crop~1~001",
                _Owner_Id = WILDCARD
            };

            public CropModel Data2 = new CropModel()
            {
                CropCode = "002",
                CropName = "キュウリ"
            };
            public RegisterResponseModel DataCrop2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~Crop~1~002"
            };
            public CropModel Data2Get = new CropModel()
            {
                CropCode = "002",
                CropName = "キュウリ",
                id = $"API~IntegratedTest~Crop~1~002",
                _Owner_Id = WILDCARD
            };

            public ForeignKeyModel DataFk1 = new ForeignKeyModel()
            {
                TestId = "1234567",
                CropCode = "001"
            };
            public ForeignKeyModel DataFk2 = new ForeignKeyModel()
            {
                TestId = "ABCDEFG",
                CropCode = "999"
            };
            public ForeignKeyModel DataFk3 = new ForeignKeyModel()
            {
                TestId = "1234567",
                CropCode = "999"
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void ForeignKeyTest_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var cropApi = UnityCore.Resolve<ICropApi>();
            var fkApi = UnityCore.Resolve<IForeignKeyApi>();
            var testData = new ForeignKeyTestData();

            // Crop削除⇒登録
            client.GetWebApiResponseResult(cropApi.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(cropApi.Regist(testData.DataCrop1)).Assert(RegisterSuccessExpectStatusCode, testData.DataCrop1RegistExpected);
            client.GetWebApiResponseResult(cropApi.Regist(testData.Data2)).Assert(RegisterSuccessExpectStatusCode, testData.DataCrop2RegistExpected);

            client.GetWebApiResponseResult(cropApi.Exists(testData.DataCrop1.CropCode)).Assert(ExistsSuccessExpectStatusCode);
            client.GetWebApiResponseResult(cropApi.Exists(testData.Data2.CropCode)).Assert(ExistsSuccessExpectStatusCode);
            client.GetWebApiResponseResult(cropApi.Exists(testData.Data2.CropCode + "ABC")).Assert(ExistsErrorExpectStatusCode);

            // FK登録
            client.GetWebApiResponseResult(fkApi.DeleteAll()).Assert(DeleteExpectStatusCodes);

            client.GetWebApiResponseResult(fkApi.Regist(testData.DataFk1)).Assert(RegisterSuccessExpectStatusCode);
            var response = client.GetWebApiResponseResult(fkApi.Regist(testData.DataFk2)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402");
            response.RawContentString.Contains("There are no results from").IsTrue();

            // Crop & FK削除
            client.GetWebApiResponseResult(cropApi.DeleteAll()).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(fkApi.DeleteAll()).Assert(DeleteSuccessStatusCode);

            // FK再登録
            response = client.GetWebApiResponseResult(fkApi.Regist(testData.DataFk1)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402");
            response.RawContentString.Contains("There are no results from").IsTrue();

            // 複数のFKエラーのチェック
            client.GetWebApiResponseResult(fkApi.DeleteAll()).Assert(DeleteExpectStatusCodes);
            response = client.GetWebApiResponseResult(fkApi.RegistErrors(testData.DataFk3)).AssertErrorCode(RegisterErrorExpectStatusCode, "E10402");
            JToken.Parse(response.RawContentString)["errors"].Children().Count().Is(2);
        }
    }
}
