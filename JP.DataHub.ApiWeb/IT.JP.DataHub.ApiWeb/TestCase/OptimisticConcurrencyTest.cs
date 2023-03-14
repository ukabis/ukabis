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
    [TestClass]
    public class OptimisticConcurrencyTest : ApiWebItTestCase
    {
        #region TestData

        private class OptimisticConcurrencyTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~OptimisticConcurrency~1~AA"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                id = "API~IntegratedTest~OptimisticConcurrency~1~AA",
                _Owner_Id = WILDCARD,
                _etag = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public AreaUnitModel Data2Get = new AreaUnitModel()
            {
                id = "API~IntegratedTest~OptimisticConcurrency~1~AA",
                _Owner_Id = WILDCARD,
                _etag = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa2",
                ConversionSquareMeters = 1
            };

            public List<AreaUnitModel> DataList1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "b",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "c",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "DD",
                    AreaUnitName = "d",
                    ConversionSquareMeters = 4
                }
            };
            public List<AreaUnitModel> DataList1Get = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~AA",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "a",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~BB",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "b",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~CC",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "c",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~DD",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "d",
                    ConversionSquareMeters = 4
                }
            };
            public List<AreaUnitModel> DataList1_1Get = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~AA",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "aa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~BB",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~CC",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "cc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~DD",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "dd",
                    ConversionSquareMeters = 4
                }
            };
            public List<AreaUnitModel> DataList1_2Get = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~AA",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~BB",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~CC",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "cc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~DD",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                }
            };
            public List<AreaUnitModel> DataList1_3Get = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~AA",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~BB",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~CC",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "cc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    id = "API~IntegratedTest~OptimisticConcurrency~1~DD",
                    _Owner_Id = WILDCARD,
                    _etag = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                }
            };

            public OptimisticConcurrencyTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
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
        [DataRow(Repository.SqlServer)]
        public void OptimisticConcurrencyNormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IOptimisticConcurrencyApi>();
            var testData = new OptimisticConcurrencyTestData(repository, api.ResourceUrl);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録　単件
            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // 登録した１件を取得(Etagが取得できること)
            var result1 = client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data1Get).Result;

            // 最新のEtagを設定してデータ登録 ⇒ 正しく登録できること
            testData.Data1.AreaUnitName = "aaa2";
            testData.Data1._etag = result1._etag;
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // Etagを更新せずに再度登録⇒409になること
            testData.Data1.AreaUnitName = "aaa3";
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(ConflictExpectStatusCode);

            // データ取得　409になった更新は実施されないこと
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // Etagを設定せずに登録する⇒409になること
            testData.Data1._etag = null;
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(ConflictExpectStatusCode);

            // データ取得　409になった更新は実施されないこと
            client.GetWebApiResponseResult(api.Get(testData.Data1.AreaUnitCode)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void OptimisticConcurrencyListSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IOptimisticConcurrencyApi>();
            var testData = new OptimisticConcurrencyTestData(repository, api.ResourceUrl);

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録　複数件
            client.GetWebApiResponseResult(api.RegistList(testData.DataList1)).Assert(RegisterSuccessExpectStatusCode);

            // リストデータ取得（Etagが取得できること）
            var result1 = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.DataList1Get).Result;

            // すべてのデータに対して最新のEtagを設定してデータ登録⇒正しく登録できること
            var etags = result1.Select(x => x._etag).ToList();
            testData.DataList1[0].AreaUnitName = "aa";
            testData.DataList1[0]._etag = etags[0];
            testData.DataList1[1].AreaUnitName = "bb";
            testData.DataList1[1]._etag = etags[1];
            testData.DataList1[2].AreaUnitName = "cc";
            testData.DataList1[2]._etag = etags[2];
            testData.DataList1[3].AreaUnitName = "dd";
            testData.DataList1[3]._etag = etags[3];
            client.GetWebApiResponseResult(api.RegistList(testData.DataList1)).Assert(RegisterSuccessExpectStatusCode);

            // データ取得（すべてのデータが更新されていること）
            var result2 = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.DataList1_1Get).Result;
            
            // 一部データに対して最新のEtagを設定せずに登録⇒409を返却、エラーのデータのみ更新されていないこと
            var etags2 = result2.Select(x => x._etag).ToList();
            testData.DataList1[0].AreaUnitName = "aaa";
            testData.DataList1[0]._etag = etags2[0];
            testData.DataList1[1].AreaUnitName = "bbb";
            testData.DataList1[1]._etag = etags2[1];
            testData.DataList1[2].AreaUnitName = "ccc";
            testData.DataList1[3].AreaUnitName = "ddd";
            testData.DataList1[3]._etag = etags2[3];
            client.GetWebApiResponseResult(api.RegistList(testData.DataList1)).Assert(ConflictExpectStatusCode);
            var result3 = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.DataList1_2Get).Result;

            // X-RegisterConflictStop=trueを設定する。一部データに対して最新のEtagを設定せずに登録⇒409を返却、エラーが発生した時点で更新は停止されること
            api.AddHeaders.Add(HeaderConst.X_RegisterConflictStop, "true");
            var etags3 = result3.Select(x => x._etag).ToList();
            testData.DataList1[0].AreaUnitName = "aaaa";
            testData.DataList1[0]._etag = etags3[0];
            testData.DataList1[1].AreaUnitName = "bbbb";
            testData.DataList1[1]._etag = etags3[1];
            testData.DataList1[2].AreaUnitName = "cccc";
            testData.DataList1[3].AreaUnitName = "dddd";
            testData.DataList1[3]._etag = etags3[3];
            client.GetWebApiResponseResult(api.RegistList(testData.DataList1)).Assert(ConflictExpectStatusCode);
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, testData.DataList1_3Get);
        }
    }
}