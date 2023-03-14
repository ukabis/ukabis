using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// RDBMS系リポジトリのテスト
    /// </summary>
    [TestClass]
    public class RdbmsTableTest : ApiWebItTestCase
    {
        #region TestData

        private class RdbmsTableBaseTestData : TestDataBase
        {
            public List<ODataPatchModel> Data1 = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    STR_VALUE = "data1",
                    STR_NULL = null,
                    INT_VALUE = 1234,
                    DBL_VALUE = 1234.5678m,
                    NUM_NULL = null,
                    OBJ_VALUE = new ODataPatchObject() { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE = new List<string>() { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE = true,
                    BOL_NULL = null,
                    DAT_VALUE = "2018-11-13T20:20:39",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    STR_VALUE = "data2",
                    STR_NULL = null,
                    INT_VALUE = 5678,
                    DBL_VALUE = 5678.1234m,
                    NUM_NULL = null,
                    OBJ_VALUE = new ODataPatchObject() { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE = new List<string>() { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE = false,
                    BOL_NULL = null,
                    DAT_VALUE = "2018-12-13T20:20:39",
                    DAT_NULL = null
                }
            };
            public List<RegisterResponseModel> Data1RegistExpected = new List<RegisterResponseModel>()
            {
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~RdbmsTableJoinTestBase~1~data1"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~RdbmsTableJoinTestBase~1~data2"
                }
            };
            public List<ODataPatchModel> Data1Get = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestBase~1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    STR_VALUE = "data1",
                    STR_NULL = null,
                    INT_VALUE = 1234,
                    DBL_VALUE = 1234.5678m,
                    NUM_NULL = null,
                    OBJ_VALUE = new ODataPatchObject() { key1 = "value1" },
                    OBJ_NULL = null,
                    ARY_VALUE = new List<string>() { "value1-1", "value1-2" },
                    ARY_NULL = null,
                    BOL_VALUE = true,
                    BOL_NULL = null,
                    DAT_VALUE = "2018-11-13T20:20:39",
                    DAT_NULL = null
                },
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestBase~1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    STR_VALUE = "data2",
                    STR_NULL = null,
                    INT_VALUE = 5678,
                    DBL_VALUE = 5678.1234m,
                    NUM_NULL = null,
                    OBJ_VALUE = new ODataPatchObject() { key2 = "value2" },
                    OBJ_NULL = null,
                    ARY_VALUE = new List<string>() { "value2-1", "value2-2" },
                    ARY_NULL = null,
                    BOL_VALUE = false,
                    BOL_NULL = null,
                    DAT_VALUE = "2018-12-13T20:20:39",
                    DAT_NULL = null
                }
            };

            public List<RdbmsTableJoinModel> DataJoined = new List<RdbmsTableJoinModel>()
            {
                new RdbmsTableJoinModel()
                {
                    STR_VALUE = "data1",
                    JOIN_VALUE = "AA"
                },
                new RdbmsTableJoinModel()
                {
                    STR_VALUE = "data1",
                    JOIN_VALUE = "BB"
                }
            };

            public List<ODataPatchModel> Data2 = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    STR_VALUE = "data2",
                    STR_NULL = "hoge",
                    INT_VALUE = 1111,
                    DBL_VALUE = 2222.3333m,
                    NUM_NULL = 4444,
                    OBJ_VALUE = new ODataPatchObject() { key1 = "value1" },
                    OBJ_NULL = new ODataPatchObject() { key2 = "value2" },
                    ARY_VALUE = new List<string>() { "value1-1", "value1-2" },
                    ARY_NULL = new List<string>() { "value2-1", "value2-2" },
                    BOL_VALUE = true,
                    BOL_NULL = false,
                    DAT_VALUE = "2018-11-13T20:20:39",
                    DAT_NULL = "2019-11-13T20:20:39"
                }
            };
            public List<ODataPatchModel> Data2GetExpected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestBase~1~data2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE = "data2",
                    STR_NULL = "hoge",
                    INT_VALUE = 1111,
                    DBL_VALUE = 2222.3333m,
                    NUM_NULL = 4444,
                    OBJ_VALUE = new ODataPatchObject() { key1 = "value1" },
                    OBJ_NULL = new ODataPatchObject() { key2 = "value2" },
                    ARY_VALUE = new List<string>() { "value1-1", "value1-2" },
                    ARY_NULL = new List<string>() { "value2-1", "value2-2" },
                    BOL_VALUE = true,
                    BOL_NULL = false,
                    DAT_VALUE = "2018-11-13T20:20:39",
                    DAT_NULL = "2019-11-13T20:20:39"
                }
            };
            public ODataPatchModelForUpd Data2Update = new ODataPatchModelForUpd()
            {
                STR_NULL = "piyo"
            };
            public List<ODataPatchModel> Data2UpdateGetExpected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestBase~1~data2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE = "data2",
                    STR_NULL = "piyo",
                    INT_VALUE = 1111,
                    DBL_VALUE = 2222.3333m,
                    NUM_NULL = 4444,
                    OBJ_VALUE = new ODataPatchObject() { key1 = "value1" },
                    OBJ_NULL = new ODataPatchObject() { key2 = "value2" },
                    ARY_VALUE = new List<string>() { "value1-1", "value1-2" },
                    ARY_NULL = new List<string>() { "value2-1", "value2-2" },
                    BOL_VALUE = true,
                    BOL_NULL = false,
                    DAT_VALUE = "2018-11-13T20:20:39",
                    DAT_NULL = "2019-11-13T20:20:39"
                }
            };
            public List<ODataPatchModel> Data2ReRegist = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    STR_VALUE = "data2",
                    INT_VALUE = 5555,
                    DBL_VALUE = 6666.7777m,
                    OBJ_VALUE = new ODataPatchObject() { key3 = "value4" },
                    ARY_VALUE = new List<string>() { "value3-1", "value3-2" },
                    BOL_VALUE = false,
                    DAT_VALUE = "2020-11-13T20:20:39"
                }
            };
            public List<ODataPatchModel> Data2ReRegistGetExpected = new List<ODataPatchModel>()
            {
                new ODataPatchModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestBase~1~data2",
                    _Owner_Id = WILDCARD,
                    STR_VALUE = "data2",
                    STR_NULL = null,
                    INT_VALUE = 5555,
                    DBL_VALUE = 6666.7777m,
                    NUM_NULL = null,
                    OBJ_VALUE = new ODataPatchObject() { key3 = "value4" },
                    OBJ_NULL = null,
                    ARY_VALUE = new List<string>() { "value3-1", "value3-2" },
                    ARY_NULL = null,
                    BOL_VALUE = false,
                    BOL_NULL = null,
                    DAT_VALUE = "2020-11-13T20:20:39",
                    DAT_NULL = null
                }
            };

            public RdbmsTableBaseTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        private class RdbmsTableJoinTestData : TestDataBase
        {
            public List<RdbmsTableJoinModel> Data1 = new List<RdbmsTableJoinModel>()
            {
                new RdbmsTableJoinModel()
                {
                    JOIN_KEY = "data1",
                    JOIN_VALUE = "AA"
                },
                new RdbmsTableJoinModel()
                {
                    JOIN_KEY = "data1",
                    JOIN_VALUE = "BB"
                },
                new RdbmsTableJoinModel()
                {
                    JOIN_KEY = "data2",
                    JOIN_VALUE = "AA"
                },
                new RdbmsTableJoinModel()
                {
                    JOIN_KEY = "data2",
                    JOIN_VALUE = "BB"
                }
            };
            public List<RegisterResponseModel> Data1RegistExpected = new List<RegisterResponseModel>()
            {
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~RdbmsTableJoinTestJoin~1~data1~AA"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~RdbmsTableJoinTestJoin~1~data1~BB"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~RdbmsTableJoinTestJoin~1~data2~AA"
                },
                new RegisterResponseModel()
                {
                    id = "API~IntegratedTest~RdbmsTableJoinTestJoin~1~data2~BB"
                }
            };
            public List<RdbmsTableJoinModel> Data1Get = new List<RdbmsTableJoinModel>()
            {
                new RdbmsTableJoinModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestJoin~1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    JOIN_KEY = "data1",
                    JOIN_VALUE = "AA"
                },
                new RdbmsTableJoinModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestJoin~1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    JOIN_KEY = "data1",
                    JOIN_VALUE = "BB"
                },
                new RdbmsTableJoinModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestJoin~1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    JOIN_KEY = "data2",
                    JOIN_VALUE = "AA"
                },
                new RdbmsTableJoinModel()
                {
                    id = $"API~IntegratedTest~RdbmsTableJoinTestJoin~1~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    JOIN_KEY = "data2",
                    JOIN_VALUE = "BB"
                }
            };

            public RdbmsTableJoinTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void RdbmsTableTest_TableJoinSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var baseApi = UnityCore.Resolve<IRdbmsTableJoinTestBaseApi>();
            var joinApi = UnityCore.Resolve<IRdbmsTableJoinTestJoinApi>();
            var testDataBase = new RdbmsTableBaseTestData(repository, baseApi.ResourceUrl);
            var testDataJoin = new RdbmsTableJoinTestData(repository, joinApi.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(baseApi.DeleteAll()).Assert(DeleteExpectStatusCodes);
            client.GetWebApiResponseResult(joinApi.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(baseApi.OData()).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(joinApi.OData()).Assert(NotFoundStatusCode);

            // データを登録
            client.GetWebApiResponseResult(baseApi.RegistList(testDataBase.Data1)).Assert(RegisterSuccessExpectStatusCode, testDataBase.Data1RegistExpected);
            // 登録したデータを取得
            client.GetWebApiResponseResult(baseApi.GetAll()).Assert(GetSuccessExpectStatusCode, testDataBase.Data1Get);

            // 結合テーブルにもデータ登録
            client.GetWebApiResponseResult(joinApi.RegistList(testDataJoin.Data1)).Assert(RegisterSuccessExpectStatusCode, testDataJoin.Data1RegistExpected);
            // 登録したデータを取得
            client.GetWebApiResponseResult(joinApi.GetAll()).Assert(GetSuccessExpectStatusCode, testDataJoin.Data1Get);

            // 結合クエリを実行
            client.GetWebApiResponseResult(baseApi.GetJoinedData("data1")).Assert(GetSuccessExpectStatusCode, testDataBase.DataJoined);
        }

        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void RdbmsTableTest_TableJoinFailedSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var baseApi = UnityCore.Resolve<IRdbmsTableJoinTestBaseApi>();
            var testDataBase = new RdbmsTableBaseTestData(repository, baseApi.ResourceUrl);

            // 結合不許可
            client.GetWebApiResponseResult(baseApi.GetFailedByJoinNotAllowed()).AssertErrorCode(BadRequestStatusCode, "E10433");

            // アクセス権なし(結合元：OPENID認証なし、結合対象：OPENID認証あり)
            client.GetWebApiResponseResult(baseApi.GetJoinedDataWithAccessControll("data1")).AssertErrorCode(ForbiddenExpectStatusCode, "E02401");

            // 結合テーブルなし
            client.GetWebApiResponseResult(baseApi.GetFailedByUnexistingResource()).AssertErrorCode(BadRequestStatusCode, "E10432");

            // クエリにリソース直接指定が含まれている(古い書き方)
            client.GetWebApiResponseResult(baseApi.GetResourceSpecifiedJoin("data1")).AssertErrorCode(BadRequestStatusCode, "E10431");
        }

        /// <summary>
        /// RDBMS系のRegistとUpdateの動作検証
        /// </summary>
        /// <remarks>
        /// RDBMSの登録・更新はNoSQLのようなドキュメントの差し替えではないがNoSQLと同等の動作となるように実装されている。
        /// NoSQL同様以下の動作となっていることを確認するシナリオ。
        /// 　Regist：未指定項目はnullで更新
        /// 　Update：未指定項目はそのまま
        /// </remarks>
        [TestMethod]
        [DataRow(Repository.SqlServer)]
        public void RdbmsTableTest_RegistAndUpdateSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var baseApi = UnityCore.Resolve<IRdbmsTableJoinTestBaseApi>();
            var testDataBase = new RdbmsTableBaseTestData(repository, baseApi.ResourceUrl);

            // 既存データクリア
            client.GetWebApiResponseResult(baseApi.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            client.GetWebApiResponseResult(baseApi.RegistList(testDataBase.Data2)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(baseApi.OData()).Assert(GetSuccessExpectStatusCode, testDataBase.Data2GetExpected);

            // データ更新(Update)
            client.GetWebApiResponseResult(baseApi.UpdateEx(testDataBase.Data2[0].STR_VALUE, testDataBase.Data2Update)).Assert(UpdateSuccessExpectStatusCode);
            client.GetWebApiResponseResult(baseApi.OData()).Assert(GetSuccessExpectStatusCode, testDataBase.Data2UpdateGetExpected);

            // データ更新(Regist)
            client.GetWebApiResponseResult(baseApi.RegistList(testDataBase.Data2ReRegist)).Assert(RegisterSuccessExpectStatusCode);
            client.GetWebApiResponseResult(baseApi.OData()).Assert(GetSuccessExpectStatusCode, testDataBase.Data2ReRegistGetExpected);
        }
    }
}
