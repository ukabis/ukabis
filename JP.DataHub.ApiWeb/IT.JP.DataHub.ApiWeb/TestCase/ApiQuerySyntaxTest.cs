using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
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
    [TestCategory("ManageAPI")]
    public class ApiQuerySyntaxTest : ApiWebItTestCase
    {
        #region TestData

        private class ApiQuerySyntaxTestData : TestDataBase
        {
            public List<ApiQuerySyntaxModel> Data1 = new List<ApiQuerySyntaxModel>()
            {
                new ApiQuerySyntaxModel()
                {
                    Cd = "1",
                    Name = "Name1",
                    No = 1,
                    Children = new List<ApiQuerySyntaxChild>()
                    {
                        new ApiQuerySyntaxChild()
                        {
                            ChildCd = "ChildCd1",
                            ChildName = "ChildName1",
                            ChildNo = 1
                        }
                    }
                },
                new ApiQuerySyntaxModel()
                {
                    Cd = "1",
                    Name = "Name1_2",
                    Children = new List<ApiQuerySyntaxChild>()
                },
                new ApiQuerySyntaxModel()
                {
                    Cd = "2",
                    Name = "Name2",
                    No = 2,
                    Children = new List<ApiQuerySyntaxChild>()
                    {
                        new ApiQuerySyntaxChild()
                        {
                            ChildCd = "ChildCd2",
                            ChildName = "ChildName2",
                            ChildNo = 2
                        }
                    }
                },
                new ApiQuerySyntaxModel()
                {
                    Cd = "3",
                    Name = "Name3",
                    No = 3,
                    Children = new List<ApiQuerySyntaxChild>()
                },
                new ApiQuerySyntaxModel()
                {
                    Cd = "4",
                    Name = "Name4",
                    Children = new List<ApiQuerySyntaxChild>()
                }
            };

            public ApiQuerySyntaxTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// DynamicApiQuery構文テスト
        /// StaticCacheを使用する環境では実行しないこと
        /// AppService上複数インスタンスになったときにテストが失敗する
        /// (API定義変更をされたインスタンス≠テスト対象のAPIを実行するインスタンスになってしまう場合があるから)
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ApiQuerySyntaxTest_NormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IApiQuerySyntaxApi>();
            var testData = new ApiQuerySyntaxTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data1)).Assert(RegisterExpectStatusCodes);

            // クエリのテスト実行
            this.QueryTest(repository);
        }

        /// <summary>
        /// DynamicApiのクエリのテストを実行する
        /// </summary>
        private void QueryTest(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IApiQuerySyntaxApi>();
            var testData = new ApiQuerySyntaxTestData(repository, api.ResourceUrl);

            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();

            // テストで使用するDynamicApi取得
            var apiDef = clientM.GetWebApiResponseResult(manageApi.GetApiResourceFromUrl(testData.ResourceUrl)).Assert(GetSuccessExpectStatusCode).Result;
            var regMethod = apiDef.MethodList.Where(x => x.MethodUrl == "GetAll").First();

            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            var apiId = apiDef.ApiId;
            var reqSchemaId = regMethod.RequestSchemaId;
            var repositoryId = regMethod.RepositoryGroupId;

            // テスト用の値に置換する
            var regApi = new RegisterMethodModel()
            {
                ApiId = apiId,
                Url = "GetAll",
                HttpMethodTypeCd = "GET",
                ActionTypeCd = "quy",
                RequestModelId = reqSchemaId,
                IsPostDataTypeArray = true,
                IsHeaderAuthentication = true,
                RepositoryGroupId = repositoryId,
                IsEnable = true,
                IsHidden = true,
                QueryType = "cdb"
            };

            // クラスに定義してあるSQLを全て実行する
            var queryList = TestQueryList.CreateInstance(repository);
            foreach (var item in queryList.GetType().GetProperties())
            {
                System.Diagnostics.Debug.WriteLine("対象プロパティ名：" + item.Name);

                var queryItem = (TestQueryList.QueryItem)item.GetValue(queryList);
                regApi.Query = queryItem.Query;

                // Api更新
                clientM.GetWebApiResponseResult(manageApi.RegisterMethod(regApi)).Assert(RegisterSuccessExpectStatusCode);

                // 更新したクエリで値が取得できるか
                var result = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).RawContentString.ToJson();
                if (queryItem.Value != "*")
                {
                    result[0][queryItem.Key].ToString().Is(queryItem.Value);
                }
                else
                {
                    // データ順不問のケースは項目の存在のみ確認
                    result[0].FindProperty(queryItem.Key).IsNotNull();
                }
            }
        }

        [TestMethod]
        public void QueryTest_E50405レスポンス変更検知()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiQuerySyntaxApi>();

            api.AddHeaders.Add("Accept-Language", "ja");
            client.GetWebApiResponseResult(api.Get("1")).Assert(BadRequestStatusCode).RawContentString
                .StartsWith("{\"error_code\":\"E50405\",\"title\":\"クエリ構文エラー例外が発生しました\",\"status\":400,\"detail\":\"Queryの構文に誤りがあります。").IsTrue();
        }

        [TestMethod]
        public void QueryTest_E50405_クエリエラー抑制確認()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IApiQuerySyntaxApi>();

            api.AddHeaders.Add("Accept-Language", "ja");
            client.GetWebApiResponseResult(api.GetSuppressError("1")).Assert(BadRequestStatusCode).RawContentString
                .Is("{\"error_code\":\"E50405\",\"title\":\"クエリが失敗しました\",\"status\":400}");
        }
    }

    #region QueryDef

    /// <summary>
    /// 確認用クエリ一覧（共通クラス）
    /// </summary>
    public abstract class TestQueryList
    {
        public static TestQueryList CreateInstance(Repository repository)
        {
            // リポジトリタイプによってQueryListを分岐
            switch (repository)
            {
                case Repository.CosmosDb:
                    return new TestCosmosDbQueryList();
                case Repository.MongoDb:
                case Repository.MongoDbCds:
                    return new TestMongoDbQueryList();
                case Repository.SqlServer:
                    return new TestSqlServerQueryList();
                default:
                    throw new NotImplementedException();
            }
        }


        public class QueryItem
        {
            public string Query { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }

            public QueryItem(string query, string key, string value)
            {
                this.Query = query;
                this.Key = key;
                this.Value = value;
            }
        }
    }

    /// <summary>
    /// 確認用クエリ一覧（CosmosDB）
    /// </summary>
    public class TestCosmosDbQueryList : TestQueryList
    {
        /// <summary>
        /// SelectとFromのみ
        /// </summary>
        public QueryItem SelectFromQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No
FROM
    c",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// Where
        /// </summary>
        public QueryItem WhereQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No
FROM
    c
WHERE
    c.Cd = '2'",
                    "Cd",
                    "2"
                );
            }
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        public QueryItem OrderByQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No    
FROM
    c
ORDER BY
    c.Cd DESC",
                    "Cd",
                    "4"
                );
            }
        }

        /// <summary>
        /// サブクエリ(深さ1)
        /// </summary>
        public QueryItem SubQueryNestLevel1
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No 
FROM
    c
JOIN
    (
        SELECT 
            c.Cd
        FROM 
            c
        WHERE 
            c.Cd = '2'
    ) AS a
WHERE 
    a.Cd = '2'",
                    "Cd",
                    "2"
                );
            }
        }

        /// <summary>
        /// サブクエリ(深さ2)
        /// </summary>
        public QueryItem SubQueryNestLevel2
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No 
FROM
    c
JOIN
    (
        SELECT 
            c.Cd
        FROM 
            c
        JOIN
            (
                SELECT 
                    c.Cd
                FROM 
                    c
                WHERE 
                    c.Cd = '3'
            ) AS b
        WHERE 
            b.Cd = '3'
    ) AS a
WHERE 
    a.Cd = '3'",
                    "Cd",
                    "3"
                );
            }
        }

        /// <summary>
        /// エイリアス化
        /// </summary>
        public QueryItem AliasQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    { 'cd': c.Cd, 'name': c.Name } AS CD_NAME
FROM
    c",
                    "CD_NAME",
                    @"{
  ""cd"": ""1"",
  ""name"": ""Name1""
}"
                );
            }
        }

        /// <summary>
        /// Distinct
        /// </summary>
        public QueryItem DistinctQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    DISTINCT c.Cd
FROM
    c
WHERE
    c.Cd = '1'",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// IN
        /// </summary>
        public QueryItem InQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No 
FROM
    c
WHERE
    c.Cd IN('1', '3')
ORDER BY
    c.Cd ASC",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// Scalar
        /// </summary>
        public QueryItem ScalarQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT 
    ((2 + 11 % 7)-2)/3 AS Scalar
FROM 
    c",
                    "Scalar",
                    "1.3333333333333333"
                );
            }
        }

        /// <summary>
        /// 集計関数
        /// </summary>
        public QueryItem AggregateQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    SUM(c.No)　SumNo
FROM
    c",
                    "SumNo",
                    "6"
                );
            }
        }

        /// <summary>
        /// システム関数
        /// </summary>
        public QueryItem SystemQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    CONCAT(c.Cd, c.Name) AS Concat,
    GetCurrentDateTime() AS currentUtcDateTime
FROM
    c",
                    "Concat",
                    "1Name1"
                );
            }
        }

        /// <summary>
        /// ANDとOR
        /// </summary>
        public QueryItem AndOrQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
SELECT
    c.Cd,
    c.Name,
    c.No
FROM
    c
WHERE
    c.Cd = '1' AND c.Name = 'Name1' OR c.Cd = '3'
ORDER BY
    c.Cd DESC",
                    "Cd",
                    "3"
                );
            }
        }
    }

    /// <summary>
    /// 確認用クエリ一覧（MongoDB）
    /// </summary>
    public class TestMongoDbQueryList : TestQueryList
    {
        /// <summary>
        /// Selectのみ
        /// </summary>
        public QueryItem SelectQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": null,
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// Where
        /// </summary>
        public QueryItem WhereQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Cd"": ""2""
            }
        ]
    },
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "2"
                );
            }
        }

        /// <summary>
        /// OrderBy（降順）
        /// </summary>
        public QueryItem OrderByDescQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": null,
    ""OrderBy"": {
            ""Cd"": -1
            },
    ""Top"": null
}
                            ",
                    "Cd",
                    "4"
                );
            }
        }


        /// <summary>
        /// OrderBy（昇順）
        /// </summary>
        public QueryItem OrderByAscQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": null,
    ""OrderBy"": {
            ""Cd"": 1
            },
    ""Top"": null
}
                            ",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// IN
        /// </summary>
        public QueryItem InQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Cd"" : { ""$in"" : [ ""1"", ""3"" ] }
            }
        ]
    },
    ""OrderBy"": {
            ""Cd"": 1
            },
    ""Top"": null
}
                    ",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// ANDとOR
        /// </summary>
        public QueryItem AndOrQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""$and"": [
            { ""$or"" : [ { ""$and"" : [ { ""cd"" : ""1"" } , { ""Name"" : ""Name1"" } ] } , { ""Cd"" : ""3"" } ] }
        ]
    },
    ""OrderBy"": {
            ""Cd"": -1
            },
    ""Top"": null
}
                    ",
                    "Cd",
                    "3"
                );
            }
        }

        /// <summary>
        /// Selectがnull
        /// </summary>
        public QueryItem SelectNullQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": null,
    ""Where"": {
        ""$and"": [
            {
                ""Cd"": ""2""
            }
        ]
    },
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "2"
                );
            }
        }

        /// <summary>
        /// Top
        /// </summary>
        public QueryItem TopQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""$and"": [
            {
                ""Name"": /Name/
            }
        ]
    },
    ""OrderBy"": null,
    ""Top"": 1
}
                            ",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// すべての項目がnull
        /// </summary>
        public QueryItem AllNullQuery
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": null,
    ""Where"": null,
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// Where(OR)
        /// </summary>
        public QueryItem WhereQueryOr
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""$or"": [
            {
                ""Cd"": ""1""
            },
            {
                ""Cd"": ""2""
            }
        ]
    },
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// Where(単一)
        /// </summary>
        public QueryItem WhereQuerySingle
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""Cd"": ""2""
    },
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "2"
                );
            }
        }

        /// <summary>
        /// Where(複数)
        /// </summary>
        public QueryItem WhereQueryDouble
        {
            get
            {
                return new QueryItem
                (
                    @"
{
    ""Select"": {
        ""Cd"": 1,
        ""Name"": 1,
        ""No"": 1
    },
    ""Where"": {
        ""Cd"": ""2"",
        ""Name"": ""Name2"",
    },
    ""OrderBy"": null,
    ""Top"": null
}
                            ",
                    "Cd",
                    "2"
                );
            }
        }
    }

    /// <summary>
    /// 確認用クエリ一覧（SQLServer）
    /// </summary>
    public class TestSqlServerQueryList : TestQueryList
    {
        /// <summary>
        /// SelectとFromのみ
        /// </summary>
        public QueryItem SelectFromQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT ""Cd"", ""Name"", ""No"", ""_Version"" FROM {TABLE_NAME}",
                    "Cd",
                    "*"
                );
            }
        }

        /// <summary>
        /// Where
        /// </summary>
        public QueryItem WhereQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT ""Cd"", ""Name"", ""No"", ""_Version"" FROM {TABLE_NAME} WHERE ""Cd"" = '2'",
                    "Cd",
                    "2"
                );
            }
        }

        /// <summary>
        /// OrderBy
        /// </summary>
        public QueryItem OrderByQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT ""Cd"", ""Name"", ""No"", ""_Version"" FROM {TABLE_NAME} ORDER BY ""Cd"" DESC",
                    "Cd",
                    "4"
                );
            }
        }

        /// <summary>
        /// Distinct
        /// </summary>
        public QueryItem DistinctQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT DISTINCT ""Cd"", ""_Version"" FROM {TABLE_NAME} WHERE ""Cd"" = '1'",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// IN
        /// </summary>
        public QueryItem InQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT ""Cd"", ""Name"", ""No"", ""_Version"" FROM {TABLE_NAME} WHERE ""Cd"" IN('1', '3') ORDER BY ""Cd"" ASC",
                    "Cd",
                    "1"
                );
            }
        }

        /// <summary>
        /// Scalar
        /// </summary>
        public QueryItem ScalarQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT ((2.0 + 11.0 % 7.0)-2.0)/5.0 AS Scalar, ""_Version"" FROM {TABLE_NAME}",
                    "Scalar",
                    "0.8"
                );
            }
        }

        /// <summary>
        /// 集計関数
        /// </summary>
        public QueryItem AggregateQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT SUM(""No"") SumNo, ""_Version"" FROM {TABLE_NAME} GROUP BY ""_Version""",
                    "SumNo",
                    "6"
                );
            }
        }

        /// <summary>
        /// システム関数
        /// </summary>
        public QueryItem SystemQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT CONCAT(""Cd"", ""Name"") AS Concat, GETDATE() AS currentUtcDateTime, ""_Version"" FROM {TABLE_NAME} WHERE ""Cd"" = '2'",
                    "Concat",
                    "2Name2"
                );
            }
        }

        /// <summary>
        /// ANDとOR
        /// </summary>
        public QueryItem AndOrQuery
        {
            get
            {
                return new QueryItem
                (
                    @"SELECT ""Cd"", ""Name"", ""No"", ""_Version"" FROM {TABLE_NAME} WHERE ""Cd"" = '1' AND ""Name"" = 'Name1' OR ""Cd"" = '3' ORDER BY ""Cd"" DESC",
                    "Cd",
                    "3"
                );
            }
        }
    }

    #endregion
}