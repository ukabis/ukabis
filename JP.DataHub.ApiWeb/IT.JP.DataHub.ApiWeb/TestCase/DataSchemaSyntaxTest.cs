using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// データスキーマ構文テスト
    /// StaticCacheを使用する環境では実行しないこと
    /// AppService上複数インスタンスになったときにテストが失敗する
    /// (API定義変更をされたインスタンス≠テスト対象のAPIを実行するインスタンスになってしまう場合があるから)
    /// </summary>
    [TestClass]
    [TestCategory("ManageAPI")]
    public class DataSchemaSyntaxTest : ApiWebItTestCase
    {
        #region TestData

        private class DataSchemaSyntaxTestData : TestDataBase
        {
            public List<DataSchemaSyntaxModel> Data1 = new List<DataSchemaSyntaxModel>()
            {
                new DataSchemaSyntaxModel()
                {
                    Cd = "CD1",
                    Name = "Name1",
                    No = 1
                },
                new DataSchemaSyntaxModel()
                {
                    Cd = "hogeCd",
                    Name = "Name2",
                    No = 2
                }
            };

            public List<DataSchemaSyntaxModel> Data2 = new List<DataSchemaSyntaxModel>()
            {
                new DataSchemaSyntaxModel()
                {
                    SelectCd = "CD1",
                    Name = "Name1",
                    No = 1
                },
                new DataSchemaSyntaxModel()
                {
                    SelectCd = "hogeCd",
                    Name = "Name2",
                    No = 2
                }
            };

            public List<DataSchemaSyntaxModel> Data3 = new List<DataSchemaSyntaxModel>()
            {
                new DataSchemaSyntaxModel()
                {
                    CdFrom = "CD1",
                    Name = "Name1",
                    No = 1
                },
                new DataSchemaSyntaxModel()
                {
                    CdFrom = "hogeCd",
                    Name = "Name2",
                    No = 2
                }
            };

            public List<DataSchemaSyntaxModel> Data4 = new List<DataSchemaSyntaxModel>()
            {
                new DataSchemaSyntaxModel()
                {
                    AbcWhere123 = "CD1",
                    Name = "Name1",
                    No = 1
                },
                new DataSchemaSyntaxModel()
                {
                    AbcWhere123 = "hogeCd",
                    Name = "Name2",
                    No = 2
                }
            };

            public List<DataSchemaSyntaxModel> Data5 = new List<DataSchemaSyntaxModel>()
            {
                new DataSchemaSyntaxModel()
                {
                    OrderBy = "CD1",
                    Name = "Name1",
                    No = 1
                },
                new DataSchemaSyntaxModel()
                {
                    OrderBy = "hogeCd",
                    Name = "Name2",
                    No = 2
                }
            };


            public string Data1Schema = @"
{
    'type': 'object',
    'additionalProperties': false,
    'properties': {
        'Cd': {
            'title': 'Cd',
            'type': 'string',
            'required':true
        },
        'Name': {
            'title': '名前',
            'type': 'string',
            'required':true
        },
        'No': {
            'title': 'No',
            'type': 'number',
            'required':false
        }
    }
}";

            public string Data2Schema = @"
{
    'type': 'object',
    'additionalProperties': false,
    'properties': {
        'SelectCd': {
            'title': 'SelectCd',
            'type': 'string',
            'required':true
        },
        'Name': {
            'title': '名前',
            'type': 'string',
            'required':true
        },
        'No': {
            'title': 'No',
            'type': 'number',
            'required':false
        }
    }
}";

            public string Data3Schema = @"
{
    'type': 'object',
    'additionalProperties': false,
    'properties': {
        'CdFrom': {
            'title': 'CdFrom',
            'type': 'string',
            'required':true
        },
        'Name': {
            'title': '名前',
            'type': 'string',
            'required':true
        },
        'No': {
            'title': 'No',
            'type': 'number',
            'required':false
        }
    }
}";

            public string Data4Schema = @"
{
    'type': 'object',
    'additionalProperties': false,
    'properties': {
        'AbcWhere123': {
            'title': 'AbcWhere123',
            'type': 'string',
            'required':true
        },
        'Name': {
            'title': '名前',
            'type': 'string',
            'required':true
        },
        'No': {
            'title': 'No',
            'type': 'number',
            'required':false
        }
    }
}";

            public string Data5Schema = @"
{
    'type': 'object',
    'additionalProperties': false,
    'properties': {
        'OrderBy': {
            'title': 'OrderBy',
            'type': 'string',
            'required':true
        },
        'Name': {
            'title': '名前',
            'type': 'string',
            'required':true
        },
        'No': {
            'title': 'No',
            'type': 'number',
            'required':false
        }
    }
}";

            public string Data6Schema = @"
{
    'type': 'object',
    'additionalProperties': false,
    'properties': {
        '_etag': {
            'title': '_etag',
            'type': 'string',
            'required':true
        },
        'Name': {
            'title': '名前',
            'type': 'string',
            'required':true
        },
        'No': {
            'title': 'No',
            'type': 'number',
            'required':false
        }
    }
}";


            public string HasDefinedKeyword_properties = @"
{
    'description': 'テスト１',
    'type': 'object',
    'properties': {
        'test': {
            'title': 'test',
            'type': 'string'
        },
        'id': {
            'title': 'id',
            'type': 'string'
        },
        '_test': {
            'title': '_test',
            'type': 'string'
        }
    }
}
";

            public string HasDefinedKeyword_ref = @"
{
    'description': 'テスト１',
    'type': 'object',
    'definitions': {
        'DetailType': {
            'type': 'object',
            'properties': {
                'test': {
                    'type': 'string'
                },
                'id': {
                    'type': 'string'
                },
                '_test': {
                    'type': 'string'
                }
            }
        }
    }
}
";

            public string HasDefinedKeyword_oneOf = @"
{
    'description': 'テスト１',
    'type': 'object',
    'oneOf': [
        {
            '$ref': '#/definitions/DetailType'
        }
    ],
    'definitions': {
        'DetailType': {
            'type': 'object',
            'properties': {
                'test': {
                    'type': 'string'
                },
                'id': {
                    'type': 'string'
                },
                '_test': {
                    'type': 'string'
                }
            }
        }
    }
}
";

            public string HasDefinedKeyword_anyOf = @"
{
    'description': 'テスト１',
    'type': 'object',
    'anyOf': [
        {
            '$ref': '#/definitions/DetailType'
        }
    ],
    'definitions': {
        'DetailType': {
            'type': 'object',
            'properties': {
                'test': {
                    'type': 'string'
                },
                'id': {
                    'type': 'string'
                },
                '_test': {
                    'type': 'string'
                }
            }
        }
    }
}
";

            public string HasDefinedKeyword_allOf = @"
{
    'description': 'テスト１',
    'type': 'object',
    'allOf': [
        {
            '$ref': '#/definitions/DetailType'
        }
    ],
    'definitions': {
        'DetailType': {
            'type': 'object',
            'properties': {
                'test': {
                    'type': 'string'
                },
                'id': {
                    'type': 'string'
                },
                '_test': {
                    'type': 'string'
                }
            }
        }
    }
}
";

            public string HasDefinedKeyword_multipleOfArray = @"
{
    'description': 'テスト１',
    'type': 'object',
    'oneOf': [
        {
            '$ref': '#/definitions/DetailType'
        },
        {
            '$ref': '#/definitions/DetailType2'
        }
    ],
    'anyOf': [
        {
            '$ref': '#/definitions/DetailType'
        },
        {
            '$ref': '#/definitions/DetailType2'
        }
    ],
    'allOf': [
        {
            '$ref': '#/definitions/DetailType'
        },
        {
            '$ref': '#/definitions/DetailType2'
        }
    ],
    'definitions': {
        'DetailType': {
            'type': 'object',
            'properties': {
                'test': {
                    'type': 'string'
                },
                'id': {
                    'type': 'string'
                },
                '_test': {
                    'type': 'string'
                }
            }
        },
        'DetailType2': {
            'type': 'object',
            'properties': {
                'test': {
                    'type': 'string'
                },
                'id': {
                    'type': 'string'
                },
                '_test': {
                    'type': 'string'
                }
            }
        }
    }
}
";


            public string ExpectHasDefinedKeywordBody = "既に定義済みのキーワードです。Keywords=id,_test";

            public string ExpectHasDefinedKeywordMultipleOfArrayBody = "既に定義済みのキーワードです。Keywords=id,_test,id,_test,id,_test,id,_test,id,_test,id,_test";
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// データスキーマ構文テスト
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_NormalSenario()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            // モデルを追加or更新
            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax",
                JsonSchema = testData.Data1Schema
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(RegisterSuccessExpectStatusCode);


            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDataSchemaSyntaxApi>();
            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // テストデータ1件取得
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result.First().Cd.Is(testData.Data1.First().Cd);
        }

        /// <summary>
        /// データスキーマ構文テスト_項目名にSelectを含む
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_NormalSenario_Select()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            // TODO: ManageAPIでID指定なしのモデル更新ができなくなっている
            // モデルを追加or更新
            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax",
                JsonSchema = testData.Data2Schema
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(RegisterSuccessExpectStatusCode);


            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDataSchemaSyntaxApi>();
            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data2)).Assert(RegisterSuccessExpectStatusCode);

            // テストデータ1件取得
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result.First().SelectCd.Is(testData.Data2.First().SelectCd);
        }

        /// <summary>
        /// データスキーマ構文テスト_項目名にFromを含む
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_NormalSenario_From()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            // TODO: ManageAPIでID指定なしのモデル更新ができなくなっている
            // モデルを追加or更新
            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax",
                JsonSchema = testData.Data3Schema
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(RegisterSuccessExpectStatusCode);


            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDataSchemaSyntaxApi>();
            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data3)).Assert(RegisterSuccessExpectStatusCode);

            // テストデータ1件取得
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result.First().CdFrom.Is(testData.Data3.First().CdFrom);
        }

        /// <summary>
        /// データスキーマ構文テスト_項目名にWhereを含む
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_NormalSenario_Where()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            // TODO: ManageAPIでID指定なしのモデル更新ができなくなっている
            // モデルを追加or更新
            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax",
                JsonSchema = testData.Data4Schema
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(RegisterSuccessExpectStatusCode);


            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDataSchemaSyntaxApi>();
            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data4)).Assert(RegisterSuccessExpectStatusCode);

            // テストデータ1件取得
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result.First().AbcWhere123.Is(testData.Data4.First().AbcWhere123);
        }

        /// <summary>
        /// データスキーマ構文テスト_項目名にOrderByを含む
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_NormalSenario_OrderBy()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            // TODO: ManageAPIでID指定なしのモデル更新ができなくなっている
            // モデルを追加or更新
            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax",
                JsonSchema = testData.Data5Schema
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(RegisterSuccessExpectStatusCode);


            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDataSchemaSyntaxApi>();
            api.AddHeaders.Add(HeaderConst.X_Cache, "true");

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // テストデータ登録
            client.GetWebApiResponseResult(api.RegistList(testData.Data5)).Assert(RegisterSuccessExpectStatusCode);

            // テストデータ1件取得
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result.First().OrderBy.Is(testData.Data5.First().OrderBy);
        }

        /// <summary>
        /// データスキーマ構文テスト_項目名に許可キーワード（_etag）を設定できる
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_NormalScenario_etag()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            // TODO: ManageAPIでID指定なしのモデル更新ができなくなっている
            // モデルを追加or更新
            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax",
                JsonSchema = testData.Data6Schema
            };
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(RegisterSuccessExpectStatusCode);
        }

        /// <summary>
        /// データスキーマ構文テスト
        /// スキーマのrootプロパティに定義済みキーワード(^id$|^_.*$)が含まれているかチェックする
        /// </summary>
        [TestMethod]
        public void DataSchemaSyntaxTest_ErrorScenario_HasDefinedKeyword()
        {
            var clientM = new IntegratedTestClient(AppConfig.Account, "ManageApi");
            var manageApi = UnityCore.Resolve<IManageDynamicApi>();
            var testData = new DataSchemaSyntaxTestData();

            var model = new SchemaModel()
            {
                SchemaName = "IntegratedTestDataSchemaSyntax"
            };

            // モデルを追加or更新
            // モデルのrootプロパティ名に定義済みキーワードの指定があった場合、エラーになること。
            model.JsonSchema = testData.HasDefinedKeyword_properties;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(BadRequestStatusCode).ContentString.Contains(testData.ExpectHasDefinedKeywordBody).IsTrue();

            /* ライブラリ更新によりrootプロパティの$refは解決されなくなったため割愛
            // モデルのrootプロパティ名に$refがあり、$ref解決後のrootプロパティに定義済みキーワードの指定があった場合、エラーになること。
            model.JsonSchema = testData.HasDefinedKeyword_ref;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(BadRequestStatusCode).ContentString.Is(testData.ExpectHasDefinedKeywordBody);
            */

            // モデルのrootプロパティ名にoneOf[$ref]があり、$ref解決後のrootプロパティに定義済みキーワードの指定があった場合、エラーになること。
            model.JsonSchema = testData.HasDefinedKeyword_oneOf;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(BadRequestStatusCode).ContentString.Contains(testData.ExpectHasDefinedKeywordBody).IsTrue();

            // モデルのrootプロパティ名にanyOf[$ref]があり、$ref解決後のrootプロパティに定義済みキーワードの指定があった場合、エラーになること。
            model.JsonSchema = testData.HasDefinedKeyword_anyOf;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(BadRequestStatusCode).ContentString.Contains(testData.ExpectHasDefinedKeywordBody).IsTrue();

            // モデルのrootプロパティ名にallOf[$ref]があり、$ref解決後のrootプロパティに定義済みキーワードの指定があった場合、エラーになること。
            model.JsonSchema = testData.HasDefinedKeyword_allOf;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(BadRequestStatusCode).ContentString.Contains(testData.ExpectHasDefinedKeywordBody).IsTrue();

            // モデルのrootプロパティ名にoneOf[$ref, $ref], anyOf[$ref, $ref], allOf[$ref, $ref]があり、$ref解決後のrootプロパティに定義済みキーワードの指定があった場合、エラーになること。
            model.JsonSchema = testData.HasDefinedKeyword_multipleOfArray;
            clientM.GetWebApiResponseResult(manageApi.RegisterSchema(model)).Assert(BadRequestStatusCode).ContentString.Contains(testData.ExpectHasDefinedKeywordMultipleOfArrayBody).IsTrue();
        }
    }
}
