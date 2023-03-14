using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    [TestCategory("AOP")]
    public class ModifySchemaTest : ApiWebItTestCase
    {
        #region TestData

        private class ModifySchemaTestData : TestDataBase
        {
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

            public ProtectSchemaModel DataExpected = new ProtectSchemaModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
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

            public ModifySchemaTestData(string resourceUrl) : base(Repository.Default, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void ModifySchemaTest_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IModifySchemaApi>();
            var testData = new ModifySchemaTestData(api.ResourceUrl);

            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 元データの登録
            var original = client.GetWebApiResponseResult(api.Regist(testData.DataOriginal)).Assert(RegisterSuccessExpectStatusCode).Result;

            // キーを指定して更新
            var update = client.GetWebApiResponseResult(api.Regist(testData.DataUpdate)).Assert(RegisterSuccessExpectStatusCode).Result;
            original.id.Is(update.id);

            // 通常ならProtect属性がJsonSchemaにあるので更新されないが、AopFilterによってJsonSchemaを無効にしているため更新されてしまう
            // のでRegistで更新したものが、そのまま取得できてしまう
            client.GetWebApiResponseResult(api.Get(testData.DataOriginal.Code)).Assert(GetSuccessExpectStatusCode, testData.DataExpected);
        }
    }
}
