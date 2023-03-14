using System.Collections.Generic;
using System.Linq;
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
    [TestClass]
    public class ResourceSharingPersonToVendorTest : ApiWebItTestCase
    {
        #region TestData

        private class ResourceSharingPersonToVendorTestData : TestDataBase
        {
            public List<AreaUnitModel> DataUserA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AAA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BBB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CCC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                }
            };
            public List<AreaUnitModel> DataExpectUserA = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AAA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BBB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = WILDCARD,
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CCC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                }
            };

            public AreaUnitModel DataAgentRegistBUpdated = new AreaUnitModel()
            {
                AreaUnitName = "bbb_alter",
                ConversionSquareMeters = 200
            };
            public AreaUnitModel DataExpectAgentRegistBUpdated = new AreaUnitModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "BBB",
                AreaUnitName = "bbb_alter",
                ConversionSquareMeters = 200
            };

            public AreaUnitModel DataAgentRegistCUpdated = new AreaUnitModel()
            {
                AreaUnitName = "ccc_alter",
                ConversionSquareMeters = 3000
            };
            public AreaUnitModel DataExpectAgentRegistCUpdated = new AreaUnitModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "CCC",
                AreaUnitName = "ccc_alter",
                ConversionSquareMeters = 3000
            };

            public AreaUnitModel DataAgentRegistBCreated = new AreaUnitModel()
            {
                AreaUnitCode = "DDD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 4
            };
            public AreaUnitModel DataExpectAgentRegistBCreated = new AreaUnitModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "DDD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 4
            };

            public AreaUnitModel DataAgentRegistCCreated = new AreaUnitModel()
            {
                AreaUnitCode = "EEE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 5
            };
            public AreaUnitModel DataExpectAgentRegistCCreated = new AreaUnitModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "EEE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 5
            };

            public AreaUnitModel DataRegistBCreated = new AreaUnitModel()
            {
                AreaUnitCode = "FFF",
                AreaUnitName = "fff",
                ConversionSquareMeters = 6
            };
            public AreaUnitModel DataExpectRegistBCreated = new AreaUnitModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "FFF",
                AreaUnitName = "fff",
                ConversionSquareMeters = 6
            };

            public AreaUnitModel DataAgentRegistACreated = new AreaUnitModel()
            {
                AreaUnitCode = "GGG",
                AreaUnitName = "ggg",
                ConversionSquareMeters = 7
            };
            public AreaUnitModel DataExpectAgentRegistACreated = new AreaUnitModel()
            {
                id = WILDCARD,
                _Owner_Id = WILDCARD,
                AreaUnitCode = "GGG",
                AreaUnitName = "ggg",
                ConversionSquareMeters = 7
            };

            public ResourceSharingPersonToVendorTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ResorceSharingPersonToVendorTest_NormalSenario(Repository repository)
        {
            // A,B,Cを用意
            // A:データ登録者
            // B:代理入力可ベンダーのユーザー
            // C:代理入力可ベンダーのユーザー2
            var clientA = new IntegratedTestClient("test1", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChainPortal") { TargetRepository = repository };
            var clientC = new IntegratedTestClient("test3", "SmartFoodChainPortal") { TargetRepository = repository };

            var api = UnityCore.Resolve<IResourceSharingPersonToVendorApi>();
            var testData = new ResourceSharingPersonToVendorTestData(repository, api.ResourceUrl);

            // リフレッシュ
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientC.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Aがデータを3件登録
            clientA.GetWebApiResponseResult(api.RegisterList(testData.DataUserA)).Assert(RegisterSuccessExpectStatusCode);

            // AがAの全取得（3レコード）
            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.DataExpectUserA);

            // BとCからはデータ取得できない
            clientB.GetWebApiResponseResult(api.OData()).Assert(GetErrorExpectStatusCode);
            clientC.GetWebApiResponseResult(api.OData()).Assert(GetErrorExpectStatusCode);

            // "X-ResourceSharingPrivate指定して、他人のデータを変更できるか確認
            // BがAのデータとして登録できるか確認
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, clientA.GetOpenId());
            clientB.GetWebApiResponseResult(api.AgentRegister(testData.DataAgentRegistBCreated)).Assert(RegisterSuccessExpectStatusCode);

            // AでBから登録したデータが取得できるか
            clientA.GetWebApiResponseResult(api.Get("DDD")).Assert(GetSuccessExpectStatusCode, testData.DataExpectAgentRegistBCreated);

            // BがAのデータを更新できるか確認
            clientB.GetWebApiResponseResult(api.AgentUpdate("BBB", testData.DataAgentRegistBUpdated)).Assert(UpdateSuccessExpectStatusCode);

            // AでBから更新したデータが取得できるか
            clientA.GetWebApiResponseResult(api.Get("BBB")).Assert(GetSuccessExpectStatusCode, testData.DataExpectAgentRegistBUpdated);

            // Bと同一ベンダーのCがAのデータとして登録できるか確認
            clientC.GetWebApiResponseResult(api.AgentRegister(testData.DataAgentRegistCCreated)).Assert(RegisterSuccessExpectStatusCode);

            // AでCから登録したデータが取得できるか
            clientA.GetWebApiResponseResult(api.Get("EEE")).Assert(GetSuccessExpectStatusCode, testData.DataExpectAgentRegistCCreated);

            // CがAのデータを更新できるか確認
            clientC.GetWebApiResponseResult(api.AgentUpdate("CCC", testData.DataAgentRegistCUpdated)).Assert(UpdateSuccessExpectStatusCode);

            // AでCから更新したデータが取得できるか
            clientA.GetWebApiResponseResult(api.Get("CCC")).Assert(GetSuccessExpectStatusCode, testData.DataExpectAgentRegistCUpdated);


            // ヘッダーを付与していても AgentでないRegister, Updateでは代理入力はできない
            // 通常のRegister
            clientB.GetWebApiResponseResult(api.Register(testData.DataRegistBCreated)).Assert(RegisterSuccessExpectStatusCode);
            // Aではデータを取得できない
            clientA.GetWebApiResponseResult(api.Get("FFF")).Assert(NotFoundStatusCode);

            // Bでは取得できる (Bのデータとして登録されているから)
            clientB.GetWebApiResponseResult(api.Get("FFF")).Assert(GetSuccessExpectStatusCode, testData.DataExpectRegistBCreated);

            // 通常のUpdate Aのデータを取得できず失敗する
            clientB.GetWebApiResponseResult(api.Update("AAA", testData.DataAgentRegistBCreated)).Assert(UpdateErrorExpectStatusCode);


            // 代理入力先として指定されていないベンダーからではヘッダーを付与して AgentRegisterを使用しても代理入力はできない
            // AからBのデータを登録しようとする
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingPerson);
            api.AddHeaders.Add(HeaderConst.X_ResourceSharingPerson, clientB.GetOpenId());
            clientA.GetWebApiResponseResult(api.AgentRegister(testData.DataAgentRegistACreated)).Assert(RegisterSuccessExpectStatusCode);
            api.AddHeaders.Remove(HeaderConst.X_ResourceSharingPerson);

            // Bでは取得できない(Aのデータとして登録されているから)
            clientB.GetWebApiResponseResult(api.Get("GGG")).Assert(NotFoundStatusCode);

            // Aでは取得できる
            clientA.GetWebApiResponseResult(api.Get("GGG")).Assert(GetSuccessExpectStatusCode, testData.DataExpectAgentRegistACreated);
        }
    }
}
