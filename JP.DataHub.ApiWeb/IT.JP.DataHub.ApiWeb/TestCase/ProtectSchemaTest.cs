using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ProtectSchemaTest : ApiWebItTestCase
    {
        #region TestData

        private class ProtectSchemaTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~ProtectShema";

            public ProtectSchemaModel DataOriginal = new ProtectSchemaModel()
            {
                Code = "99",
                Name = "999",
                temp = 9,
                ap = new List<string>() { "1", "2", "3" },
                ap2 = new List<string>() { "11", "22", "33" },
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/01",
                        ObservationItemCode = "1",
                        ObservationValue = "11"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/02",
                        ObservationItemCode = "2",
                        ObservationValue = "22"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "3",
                    prop2 = "33",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "333"
                    }
                },
                ArrayObjectProtect = new List<ArrayObjectProtectItem>()
                {
                    new ArrayObjectProtectItem()
                    {
                        Date = "2020/01/01"
                    },
                    new ArrayObjectProtectItem()
                    {
                        Date = "2020/02/02"
                    }
                }
            };

            public ProtectSchemaModel DataUpdate = new ProtectSchemaModel()
            {
                id = "XXXXX",
                Code = "99",
                Name = "999999999999999",
                temp = 99999999999999,
                ap = new List<string>() { "9999", "999999", "99999999999" },
                ap2 = new List<string>() { "9999", "999999", "99999999999" },
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/011111111",
                        ObservationItemCode = "111111",
                        ObservationValue = "1111111111111"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/022222",
                        ObservationItemCode = "222222",
                        ObservationValue = "222222"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "333333",
                    prop2 = "33333333",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "333333333"
                    }
                },
                ArrayObjectProtect = new List<ArrayObjectProtectItem>()
                {
                    new ArrayObjectProtectItem()
                    {
                        Date = "1"
                    },
                    new ArrayObjectProtectItem()
                    {
                        Date = "2"
                    }
                }
            };

            public ProtectSchemaModel DataUpdated = new ProtectSchemaModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                Code = "99",
                Name = "999999999999999",
                temp = 9,
                ap = new List<string>() { "1", "2", "3" },
                ap2 = new List<string>() { "11", "22", "33" },
                PropArray = new List<PropArrayItem>()
                {
                    new PropArrayItem()
                    {
                        Date = "2020/01/011111111",
                        ObservationItemCode = "1",
                        ObservationValue = "1111111111111"
                    },
                    null,
                    new PropArrayItem()
                    {
                        Date = "2020/02/022222",
                        ObservationItemCode = "2",
                        ObservationValue = "222222"
                    }
                },
                PropObject = new PropObject()
                {
                    prop1 = "333333",
                    prop2 = "33",
                    prop3 = new PropObjectChild()
                    {
                        prop31 = "333"
                    }
                },
                ArrayObjectProtect = new List<ArrayObjectProtectItem>()
                {
                    new ArrayObjectProtectItem()
                    {
                        Date = "2020/01/01"
                    },
                    new ArrayObjectProtectItem()
                    {
                        Date = "2020/02/02"
                    }
                }
            };

            public ProtectSchemaTestData(string resourceUrl) : base(Repository.Default, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }


        [TestMethod]
        public void ProtectSchemaTest_NormalSenario_Key()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IProtectSchemaApi>();
            var testData = new ProtectSchemaTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // 元データの登録
            var original = client.GetWebApiResponseResult(api.Regist(testData.DataOriginal)).Assert(RegisterSuccessExpectStatusCode).Result;

            // キーを指定して更新
            var update = client.GetWebApiResponseResult(api.Regist(testData.DataUpdate)).Assert(RegisterSuccessExpectStatusCode).Result;
            original.id.Is(update.id);

            // Protectは更新されていないことを確認
            client.GetWebApiResponseResult(api.Get(testData.DataOriginal.Code)).Assert(GetSuccessExpectStatusCode, testData.DataUpdated);

            // PATCHで一部更新
            var patch = new ProtectSchemaModel()
            {
                ap = new List<string>() { "9999", "999999", "99999999999" }
            };
            client.GetWebApiResponseResult(api.Update("99", patch)).Assert(UpdateSuccessExpectStatusCode);

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.Get(testData.DataOriginal.Code)).Assert(GetSuccessExpectStatusCode, testData.DataUpdated);
        }

        [TestMethod]
        public void ProtectSchemaTest_NormalSenario_AutoNum()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IProtectSchemaAutoNumApi>();
            var testData = new ProtectSchemaTestData(api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);

            // 元データの登録
            var original = client.GetWebApiResponseResult(api.Regist(testData.DataOriginal)).Assert(RegisterSuccessExpectStatusCode).Result;

            // キーを指定して更新
            testData.DataUpdate.id = original.id;
            var update = client.GetWebApiResponseResult(api.Regist(testData.DataUpdate)).Assert(RegisterSuccessExpectStatusCode).Result;
            original.id.Is(update.id);

            var all = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;
            all.Count.Is(1);

            // PATCHで一部更新
            var patch = new ProtectSchemaModel()
            {
                ap = new List<string>() { "9999", "999999", "99999999999" }
            };
            client.GetWebApiResponseResult(api.Update(original.id, patch)).Assert(UpdateSuccessExpectStatusCode);

            // 登録した１件を取得
            all = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;
            all.Count.Is(1);
        }
    }
}
