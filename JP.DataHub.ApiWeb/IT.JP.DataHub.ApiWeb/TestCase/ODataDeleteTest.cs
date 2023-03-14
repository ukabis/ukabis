using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class ODataDeleteTest : ApiWebItTestCase
    {
        #region TestData

        private class ODataDeleteTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~ODataDeleteTest";

            public ODataDeleteModel Data1 = new ODataDeleteModel()
            {
                id = "API~IntegratedTest~ODataDeleteTest~1~AA",
                name = "aaa",
                data = "data1"
            };
            public ODataDeleteModel Data1Rdbms = new ODataDeleteModel()
            {
                key = "AA",
                name = "aaa",
                data = "data1"
            };

            public List<ODataDeleteModel> Data2 = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~AA",
                    name = "aaa",
                    data = "AAA",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~BB",
                    name = "bbb",
                    data = "BBB",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~CC",
                    name = "ccc",
                    data = "CCC",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~DD",
                    name = "ddd",
                    data = "DDD",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~EE",
                    name = "eee",
                    data = "EEE",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~FF",
                    name = "fff",
                    data = "FFF",
                }
            };
            public List<ODataDeleteModel> Data2Rdbms = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    key = "AA",
                    name = "aaa",
                    data = "AAA",
                },
                new ODataDeleteModel()
                {
                    key = "BB",
                    name = "bbb",
                    data = "BBB",
                },
                new ODataDeleteModel()
                {
                    key = "CC",
                    name = "ccc",
                    data = "CCC",
                },
                new ODataDeleteModel()
                {
                    key = "DD",
                    name = "ddd",
                    data = "DDD",
                },
                new ODataDeleteModel()
                {
                    key = "EE",
                    name = "eee",
                    data = "EEE",
                },
                new ODataDeleteModel()
                {
                    key = "FF",
                    name = "fff",
                    data = "FFF",
                }
            };

            public List<ODataDeleteModel> Data2GetExpected = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~DD",
                    name = "ddd",
                    data = "DDD",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~EE",
                    name = "eee",
                    data = "EEE",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~FF",
                    name = "fff",
                    data = "FFF",
                }
            };

            public List<ODataDeleteModel> Data3 = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~AA",
                    name = "6",
                    data = "AAA",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~BB",
                    name = "5",
                    data = "BBB",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~CC",
                    name = "4",
                    data = "CCC",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~DD",
                    name = "3",
                    data = "DDD",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~EE",
                    name = "2",
                    data = "EEE",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~FF",
                    name = "1",
                    data = "FFF",
                }
            };
            public List<ODataDeleteModel> Data3Rdbms = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    key = "AA",
                    name = "6",
                    data = "AAA",
                },
                new ODataDeleteModel()
                {
                    key = "BB",
                    name = "5",
                    data = "BBB",
                },
                new ODataDeleteModel()
                {
                    key = "CC",
                    name = "4",
                    data = "CCC",
                },
                new ODataDeleteModel()
                {
                    key = "DD",
                    name = "3",
                    data = "DDD",
                },
                new ODataDeleteModel()
                {
                    key = "EE",
                    name = "2",
                    data = "EEE",
                },
                new ODataDeleteModel()
                {
                    key = "FF",
                    name = "1",
                    data = "FFF",
                }
            };
            public List<ODataDeleteModel> Data3GetExpect_orderbyname = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~AA",
                    name = "6",
                    data = "AAA",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~BB",
                    name = "5",
                    data = "BBB",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~CC",
                    name = "4",
                    data = "CCC",
                }
            };
            public List<ODataDeleteModel> Data3GetExpect_orderbyid = new List<ODataDeleteModel>()
            {
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~DD",
                    name = "3",
                    data = "DDD",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~EE",
                    name = "2",
                    data = "EEE",
                },
                new ODataDeleteModel()
                {
                    id = "API~IntegratedTest~ODataDeleteTest~1~FF",
                    name = "1",
                    data = "FFF",
                }
            };

            public RegisterResponseModel DataRegistExpectedA = new RegisterResponseModel()
            {
                id = "API~IntegratedTest~ODataDeleteTest~1~AA"
            };

            public ODataDeleteTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, client: client) { }
        }

        private class ODataDeleteForcheckTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~ODataDeleteTestForCheck";

            public ODataDeleteModel Data1 = new ODataDeleteModel()
            {
                id = "API~IntegratedTest~ODataDeleteTestForCheck~1~AA",
                name = "aaa",
                data = "data1"
            };
            public ODataDeleteModel Data1Rdbms = new ODataDeleteModel()
            {
                key = "AA",
                name = "aaa",
                data = "data1"
            };

            public ODataDeleteModel Data1Expected = new ODataDeleteModel()
            {
                id = "API~IntegratedTest~ODataDeleteTestForCheck~1~AA",
                name = "aaa",
                data = "data1"
            };

            public ODataDeleteForcheckTestData(Repository repository, string resourceUrl, IntegratedTestClient client) : base(repository, resourceUrl, client: client) { }
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
        public void ODataDeleteTest_NormalScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteApi>();
            var apiCheck = UnityCore.Resolve<IODataDeleteForcheckApi>();
            var testData = new ODataDeleteTestData(repository, api.ResourceUrl, client);
            var testDataCheck = new ODataDeleteForcheckTestData(repository, apiCheck.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(apiCheck.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(apiCheck.GetAll()).Assert(NotFoundStatusCode);

            // データを5件登録
            var data = (repository == Repository.SqlServer ? testData.Data2Rdbms : testData.Data2);
            client.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // チェック用の方にもデータを1件登録
            var dataCheck = (repository == Repository.SqlServer ? testDataCheck.Data1Rdbms : testDataCheck.Data1);
            client.GetWebApiResponseResult(apiCheck.Regist(dataCheck)).Assert(RegisterSuccessExpectStatusCode);

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete($"$filter=id eq '{testData.DataRegistExpectedA.id}'")).Assert(DeleteSuccessStatusCode);

            // データ非存在確認
            client.GetWebApiResponseResult(api.Get(testData.DataRegistExpectedA.id)).Assert(NotFoundStatusCode);

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete("$top=2")).Assert(DeleteSuccessStatusCode);

            // データ取得
            // 指定のもの以外が消えていないことを確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete("$top=100")).Assert(DeleteSuccessStatusCode);

            // 全件削除確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(NotFoundStatusCode);

            // チェック用の方のデータを取得し、存在することを確認
            client.GetWebApiResponseResult(apiCheck.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, new List<ODataDeleteModel>() { testDataCheck.Data1Expected });

            // チェック用の方のデータ削除
            client.GetWebApiResponseResult(apiCheck.ODataDelete("$top=100")).Assert(DeleteSuccessStatusCode);

            // 全件削除確認
            client.GetWebApiResponseResult(apiCheck.OData("$select=id,name,data")).Assert(NotFoundStatusCode);
        }

        // 順序付き削除の確認
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ODataDeleteTest_OrderDeleteSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteApi>();
            var testData = new ODataDeleteTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データを登録
            var data = (repository == Repository.SqlServer ? testData.Data3Rdbms : testData.Data3);
            client.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // nameを順序を指定して、削除
            client.GetWebApiResponseResult(api.ODataDelete("$top=3&$orderby=name")).Assert(DeleteSuccessStatusCode);

            // データ取得
            // 指定のもの以外が消えていないことを確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, testData.Data3GetExpect_orderbyname);

            // 全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);

            // データを登録
            client.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // idを順序を指定して、削除
            client.GetWebApiResponseResult(api.ODataDelete("$top=3&$orderby=id")).Assert(DeleteSuccessStatusCode);

            // データ取得
            // 指定のもの以外が消えていないことを確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, testData.Data3GetExpect_orderbyid);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ODataDeleteTest_QueryParttern(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteApi>();
            var testData = new ODataDeleteTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // $filter/$top なし-1
            client.GetWebApiResponseResult(api.ODataDelete("$select=hoge,hoge/fuga&$count=true&$orderby=id")).Assert(DeleteSuccessStatusCode);

            // データが消えていることを確認
            CheckNoData(client, api);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // $filter/$top なし-2
            client.GetWebApiResponseResult(api.ODataDelete("$count=true&$select=hoge,hoge/fuga&$orderby=id")).Assert(DeleteSuccessStatusCode);

            // データが消えていることを確認
            CheckNoData(client, api);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // $filter/$top なし-3
            client.GetWebApiResponseResult(api.ODataDelete("$count=false&$select=hoge,hoge/fuga&$orderby=id")).Assert(DeleteSuccessStatusCode);

            // データが消えていることを確認
            CheckNoData(client, api);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // クエリストリングなし
            client.GetWebApiResponseResult(api.ODataDelete("")).Assert(DeleteSuccessStatusCode);

            // データが消えていることを確認
            CheckNoData(client, api);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // $filter/$top あるが、他クエリもあり(成功)
            client.GetWebApiResponseResult(api.ODataDelete($"$filter=id eq '{testData.Data1.id}'&$top=1&$count=false&$select=hoge,hoge/fuga&$orderby=id")).Assert(DeleteSuccessStatusCode);

            // データが消えていることを確認
            CheckNoData(client, api);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ODataDeleteTest_NotFoundSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteApi>();
            var testData = new ODataDeleteTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // 存在しないデータを指定して削除しようとする
            client.GetWebApiResponseResult(api.ODataDelete("$filter=name eq 'bbb'")).Assert(NotFoundStatusCode);

            // 存在しないカラムを指定して、ソートして削除しようとする
            client.GetWebApiResponseResult(api.ODataDelete("$filter=hoge eq 'bbb'&$orderby=id")).Assert(NotFoundStatusCode);

            // 存在しないカラムを指定して削除しようとする
            client.GetWebApiResponseResult(api.ODataDelete("$filter=hoge eq 'hoge'")).Assert(NotFoundStatusCode);

            // 存在しないカラムでソートしようとする
            client.GetWebApiResponseResult(api.ODataDelete("$filter=id eq 'hoge'&$orderby=hoge")).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ODataDeleteTest_BadRequestSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteApi>();
            var testData = new ODataDeleteTestData(repository, api.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データを１件登録
            RegisterAndCheckData(client, api, testData);

            // クエリストリングを間違える(&で区切られていない)
            client.GetWebApiResponseResult(api.ODataDelete("$top=1$orderby=id")).Assert(BadRequestStatusCode);

            // ODataクエリを間違える
            client.GetWebApiResponseResult(api.ODataDelete("$hoge=1")).Assert(DeleteSuccessStatusCode);

            // データ非存在確認
            CheckNoData(client, api);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ODataDeleteTest_NormalScenarioByDynamicPartitionkeySetting(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IODataDeleteDynamicPartitionApi>();
            var apiCheck = UnityCore.Resolve<IODataDeleteForcheckApi>();
            var testData = new ODataDeleteTestData(repository, api.ResourceUrl, client);
            var testDataCheck = new ODataDeleteForcheckTestData(repository, apiCheck.ResourceUrl, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(apiCheck.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(apiCheck.GetAll()).Assert(NotFoundStatusCode);

            // データを5件登録
            var data = (repository == Repository.SqlServer ? testData.Data2Rdbms : testData.Data2);
            client.GetWebApiResponseResult(api.RegistList(data)).Assert(RegisterSuccessExpectStatusCode);

            // チェック用の方にもデータを1件登録
            var dataCheck = (repository == Repository.SqlServer ? testDataCheck.Data1Rdbms : testDataCheck.Data1);
            client.GetWebApiResponseResult(apiCheck.Regist(dataCheck)).Assert(RegisterSuccessExpectStatusCode);

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete($"$filter=id eq '{testData.Data1.id}'")).Assert(DeleteSuccessStatusCode);

            // データ非存在確認
            client.GetWebApiResponseResult(api.Get(testData.Data1.id)).Assert(NotFoundStatusCode);

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete("$top=2")).Assert(DeleteSuccessStatusCode);

            // データ取得
            // 指定のもの以外が消えていないことを確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, testData.Data2GetExpected);

            // ODataDeleteでデータ削除
            client.GetWebApiResponseResult(api.ODataDelete("$top=100")).Assert(DeleteSuccessStatusCode);

            // 全件削除確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(NotFoundStatusCode);

            // チェック用の方のデータを取得し、存在することを確認
            client.GetWebApiResponseResult(apiCheck.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, new List<ODataDeleteModel>() { testDataCheck.Data1Expected });

            //チェック用の方のデータ削除
            client.GetWebApiResponseResult(apiCheck.ODataDelete("$top=100")).Assert(DeleteSuccessStatusCode);

            // 全件削除確認
            client.GetWebApiResponseResult(apiCheck.OData("$select=id,name,data")).Assert(NotFoundStatusCode);
        }

        private void RegisterAndCheckData(IntegratedTestClient client, IODataDeleteApi api, ODataDeleteTestData testData)
        {
            // データを1件登録
            var data = (client.TargetRepository == Repository.SqlServer ? testData.Data1Rdbms : testData.Data1);
            client.GetWebApiResponseResult(api.Regist(data)).Assert(RegisterSuccessExpectStatusCode);

            // データ存在確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(GetSuccessExpectStatusCode, new List<ODataDeleteModel>() { testData.Data1 });
        }

        private void CheckNoData(IntegratedTestClient client, IODataDeleteApi api)
        {
            // データ非存在確認
            client.GetWebApiResponseResult(api.OData("$select=id,name,data")).Assert(NotFoundStatusCode);
        }
    }
}