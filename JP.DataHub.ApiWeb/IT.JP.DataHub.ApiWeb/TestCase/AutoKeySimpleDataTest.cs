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
using IT.JP.DataHub.ApiWeb.Config;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class AutoKeySimpleDataTest : ApiWebItTestCase
    {
        #region TestData

        private class AutoKeySimpleDataTestData : TestDataBase
        {
            public AreaUnitModel Data1 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1
            };
            public RegisterResponseModel Data1RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}"
            };
            public AreaUnitModel Data1Get = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1GetFull = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~AutoKeySimpleData~1",
                _Type = $"API~IntegratedTest~AutoKeySimpleData",
                _Owner_Id = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };
            public AreaUnitModel Data1GetFullWithoutTypeAndPartitionKey = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Version = 1,
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1SystemPropertiesInclude = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 1,
                id = "hogehoge_id",
                _Reguser_Id = "hogehoge_regu",
                _Regdate = "hogehoge_regd",
                _Upduser_Id = "hogehoge_updu",
                _Upddate = "hogehoge_updd",
                _Version = 999,
                _Owner_Id = "hogehoge_owner"
            };
            public AreaUnitModel Data1Patch = new AreaUnitModel()
            {
                ConversionSquareMeters = 2
            };
            public AreaUnitModel Data1Patched = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 2,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1Overwrite = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                ConversionSquareMeters = 999,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1AdditionalPropertiesFalseExpected = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };
            public AreaUnitModel Data1AdditionalPropertiesFalseFullExpected = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~AutoKeySimpleData~1",
                _Type = $"API~IntegratedTest~AutoKeySimpleData",
                _Owner_Id = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };
            public AreaUnitModel Data1AdditionalPropertiesFalseFullExpectedWithoutTypeAndPartitionKey = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa",
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Version = 1,
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel data2 = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10
            };
            public RegisterResponseModel Data2RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}"
            };
            public AreaUnitModel Data2Get = new AreaUnitModel()
            {
                AreaUnitCode = "BB",
                AreaUnitName = "bbb",
                ConversionSquareMeters = 10,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data3 = new AreaUnitModel()
            {
                AreaUnitCode = "CC",
                AreaUnitName = "ccc",
                ConversionSquareMeters = 100
            };
            public RegisterResponseModel Data3RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}"
            };
            public AreaUnitModel Data3Get = new AreaUnitModel()
            {
                AreaUnitCode = "CC",
                AreaUnitName = "ccc",
                ConversionSquareMeters = 100,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data4 = new AreaUnitModel()
            {
                AreaUnitCode = "DD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 1000
            };
            public RegisterResponseModel Data4RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}"
            };
            public AreaUnitModel Data4Get = new AreaUnitModel()
            {
                AreaUnitCode = "DD",
                AreaUnitName = "ddd",
                ConversionSquareMeters = 1000,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel Data5 = new AreaUnitModel()
            {
                AreaUnitCode = "EE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 10000
            };
            public RegisterResponseModel Data5RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}"
            };
            public AreaUnitModel Data5Get = new AreaUnitModel()
            {
                AreaUnitCode = "EE",
                AreaUnitName = "eee",
                ConversionSquareMeters = 10000,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public AreaUnitModel DataEx = new AreaUnitModel()
            {
                AreaUnitCode = "ZZ",
                AreaUnitName = "111",
                ConversionSquareMeters = 1
            };
            public AreaUnitModel DataExGet = new AreaUnitModel()
            {
                AreaUnitCode = "ZZ",
                AreaUnitName = "111",
                ConversionSquareMeters = 1,
                id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                _Owner_Id = WILDCARD
            };

            public List<AreaUnitModelEx> DataODataTest = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta() 
                        { 
                            MetaKey = "TestKeyA", 
                            MetaValue = "value1"
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2"
                        }
                    }
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta()
                        {
                            MetaKey = "TestKeyA",
                            MetaValue = "value1"
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2"
                        }
                    }
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta()
                        {
                            MetaKey = "TestKeyA",
                            MetaValue = "value2"
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2"
                        }
                    }
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta()
                        {
                            MetaKey = "TestKeyB",
                            MetaValue = "value2"
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2"
                        }
                    }
                },
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "EE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta()
                        {
                            MetaKey = "TestKeyC",
                            MetaValue = "value2"
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2"
                        }
                    }
                }
            };

            public List<AreaUnitModelEx> DataODataTestAnyResult = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta()
                        {
                            MetaKey = "TestKeyB",
                            MetaValue = "value2",
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2",
                        }
                    },
                    id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                    _Owner_Id = WILDCARD
                }
            };

            public List<AreaUnitModelEx> DataODataTestAnyResult2 = new List<AreaUnitModelEx>()
            {
                new AreaUnitModelEx()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                    MetaList = new List<AreaUnitMeta>()
                    {
                        new AreaUnitMeta()
                        {
                            MetaKey = "TestKeyA",
                            MetaValue = "value1",
                        },
                        new AreaUnitMeta()
                        {
                            MetaKey = "Key2",
                            MetaValue = "value2",
                        }
                    },
                    id = $"API~IntegratedTest~AutoKeySimpleData~1~{WILDCARD}",
                    _Owner_Id = WILDCARD
                }
            };


            public AutoKeySimpleDataTestData(Repository repository, string resourceUrl) : base(repository, resourceUrl) {}
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
        public void AutoKeySimpleDataTest_NormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();
            var testData = new AutoKeySimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 消えていることを確認(OData)
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetAll()).Assert(NotFoundStatusCode);

            // データ1を１件登録
            var reg1 = client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected).Result;

            // 登録した１件を取得
            client.GetWebApiResponseResult(api.Get(reg1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // 登録していないキーのデータを取得（ヒットしない）
            client.GetWebApiResponseResult(api.Get("Dummy")).Assert(NotFoundStatusCode);

            // 全取得（1レコードのはず）
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });

            // データ2を１件登録
            var reg2 = client.GetWebApiResponseResult(api.Regist(testData.data2)).Assert(RegisterSuccessExpectStatusCode, testData.Data2RegistExpected).Result;

            // １つ目のデータを取得
            client.GetWebApiResponseResult(api.Get(reg1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // ２つ目のデータを取得
            client.GetWebApiResponseResult(api.Get(reg2.id)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // idでQueryStringで取得
            client.GetWebApiResponseResult(api.GetById(reg1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            client.GetWebApiResponseResult(api.GetById(reg2.id)).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // AreaUnitCodeでQueryStringで取得
            client.GetWebApiResponseResult(api.GetByAreaUnitCode($"AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            client.GetWebApiResponseResult(api.GetByAreaUnitCode($"BB")).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // Odataでデータ取得
            client.GetWebApiResponseResult(api.OData($"$filter=AreaUnitCode eq 'AA'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data1Get });
            client.GetWebApiResponseResult(api.OData($"$filter=AreaUnitCode eq 'BB'")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data2Get });

            // 全取得（２レコード）
            var array = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get });

            // 全データを登録
            client.GetWebApiResponseResult(api.RegistList(new List<AreaUnitModel>() { testData.Data3, testData.Data4, testData.Data5 }))
                .Assert(RegisterSuccessExpectStatusCode, new List<RegisterResponseModel>() { testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected });

            // 全部のデータが入ったことを確認
            array = client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get });

            // 全件のレコード確認
            client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(5);

            // SKIPでデータ取得(現状SQLServerのみ対応)
            if (repository == Repository.SqlServer)
            {
                // TOPあり
                client.GetWebApiResponseResult(api.OData($"$skip=3&$top=1&$orderby=AreaUnitCode desc")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data2Get });

                // TOPなし
                client.GetWebApiResponseResult(api.OData($"$skip=3&$orderby=AreaUnitCode asc")).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data4Get, testData.Data5Get });
            }

            // OdataQueryでデータ取得
            client.GetWebApiResponseResult(api.GetByODataQuery("BB")).Assert(GetSuccessExpectStatusCode, testData.Data2Get);

            // OdataQueryでデータ取得(パラメータ以上)
            array = client.GetWebApiResponseResult(api.GetByODataQueryOverMeters1000("bb")).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(new List<AreaUnitModel>() { testData.Data4Get, testData.Data5Get });

            if (IsIgnoreGetInternalAllField)
            {
                // X-GetInternalAllField指定しても、システムプロパティが返却されないことを確認
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                client.GetWebApiResponseResult(api.Get(reg1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
                client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(5);
                api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);
            }
            else
            {
                // X-GetInternalAllField指定して、システムプロパティを返すか確認
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                client.GetWebApiResponseResult(api.Get(reg1.id)).Assert(GetSuccessExpectStatusCode, 
                    repository != Repository.SqlServer ? testData.Data1GetFull : testData.Data1GetFullWithoutTypeAndPartitionKey);

                // システム依存プロパティを指定してデータを登録、システム依存プロパティが正しくセットされていることを確認(ユーザーから指定したシステムプロパティは無視される）
                client.GetWebApiResponseResult(api.Regist(repository != Repository.SqlServer ? testData.Data1SystemPropertiesInclude : testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
                client.GetWebApiResponseResult(api.Get(reg1.id)).Assert(GetSuccessExpectStatusCode,
                    repository != Repository.SqlServer ? testData.Data1GetFull : testData.Data1GetFullWithoutTypeAndPartitionKey);
                api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

                client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(6);
            }

            // "AA"データをPATCHする
            client.GetWebApiResponseResult(api.Update(reg1.id, testData.Data1Patch)).Assert(UpdateSuccessExpectStatusCode);

            // "AA"を取得して正しく変更されているか確認
            var result = client.GetWebApiResponseResult(api.Get(reg1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Patched).Result;

            // 既存データの書き換え（idを指定してregistしているから）
            // SqlServerはAdditionalProperties=falseのため割愛
            if (repository != Repository.SqlServer)
            {
                result.ConversionSquareMeters = 999;
                var overwrite1 = client.GetWebApiResponseResult(api.Regist(result)).Assert(RegisterSuccessExpectStatusCode, reg1).Result;

                // そのデータを取得
                client.GetWebApiResponseResult(api.Get(overwrite1.id)).Assert(GetSuccessExpectStatusCode, testData.Data1Overwrite);
            }

            if (IsIgnoreGetInternalAllField)
            {
                // 結果レコード数は5
                client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(5);
            }
            else
            {
                // 結果レコード数は6
                client.GetWebApiResponseResult(api.GetCount()).Assert(GetSuccessExpectStatusCode).Result.Count.Is(6);
            }

            // additionalProperties_falseのレスポンスモデル設定
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);
            client.GetWebApiResponseResult(api.GetAdditionalPropertiesFalse("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1AdditionalPropertiesFalseExpected);

            if (!IsIgnoreGetInternalAllField)
            {
                api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
                client.GetWebApiResponseResult(api.GetAdditionalPropertiesFalse("AA")).Assert(GetSuccessExpectStatusCode, 
                    repository != Repository.SqlServer ? testData.Data1AdditionalPropertiesFalseFullExpected : testData.Data1AdditionalPropertiesFalseFullExpectedWithoutTypeAndPartitionKey);
            }
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AutoKeySimpleDataTest_DeleteAllScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();
            var testData = new AutoKeySimpleDataTestData(repository, api.ResourceUrl);

            // １件登録（ダミー用）
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 全件削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 再度全件削除(NotFound)
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(NotFoundStatusCode);

            // 取得する(０件）
            client.GetWebApiResponseResult(api.OData()).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AutoKeySimpleDataTest_OtherVendorAccess_Public(Repository repository)
        {
            // ベンダー依存しないAPIなので、異なるベンダーでアクセスして同一のデータが取得できることを確認する
            var clientA = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var clientC = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem") { TargetRepository = repository };

            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();
            var testData = new AutoKeySimpleDataTestData(repository, api.ResourceUrl);


            // ベンダーA,B,Cで全データを削除
            clientA.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(DeleteExpectStatusCodes, (HttpStatusCode.NotFound, "E10421"));
            clientB.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(DeleteExpectStatusCodes, (HttpStatusCode.NotFound, "E10421"));
            clientC.GetWebApiResponseResult(api.DeleteAll()).AssertErrorCode(DeleteExpectStatusCodes, (HttpStatusCode.NotFound, "E10421"));

            // ベンダーAで全データを登録
            var allData = new List<AreaUnitModel>() 
            { 
                testData.Data1, testData.data2, testData.Data3, testData.Data4, testData.Data5 
            };
            var allDataRegistExpected = new List<RegisterResponseModel>() 
            { 
                testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected 
            };
            clientA.GetWebApiResponseResult(api.RegistList(allData)).Assert(RegisterSuccessExpectStatusCode, allDataRegistExpected);

            // ベンダーA,B,Cで全データを取得（ベンダーB,Cでも全件が取得できること=ベンダーAで登録し、ベンダー境界がないため）
            var array = clientA.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get });
            array = clientB.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get });
            array = clientC.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode).Result;
            array.OrderBy(x => x.AreaUnitCode).ToList().IsStructuralEqual(new List<AreaUnitModel>() { testData.Data1Get, testData.Data2Get, testData.Data3Get, testData.Data4Get, testData.Data5Get });
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AutoKeySimpleDataTest_NotSetAccessTokenAuth(Repository repository)
        {
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();

            // 認証情報があるのでアクセスできる
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetExpectStatusCodes);

            // OPENIDの認証情報を設定していないためアクセスできない
            client = new IntegratedTestClient(null, "SmartFoodChainPortal") { TargetRepository = repository };
            // TODO:現在はOpenID認証を必須にしていないため
            client.GetWebApiResponseResult(api.GetAll()); //.Assert(ForbiddenExpectStatusCode);

            // アクセストークンの認証情報を設定していないためアクセスできない
            client = new IntegratedTestClient(AppConfig.Account, null) { TargetRepository = repository };
            client.GetWebApiResponseResult(api.GetAll()).AssertErrorCode(ForbiddenExpectStatusCode, "E02402");
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AutoKeySimpleDataTest_ODataContinuationSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();
            var testData = new AutoKeySimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 全データを登録(5件)
            var allData = new List<AreaUnitModel>()
            {
                testData.Data1, testData.data2, testData.Data3, testData.Data4, testData.Data5
            };
            var allDataRegistExpected = new List<RegisterResponseModel>()
            {
                testData.Data1RegistExpected, testData.Data2RegistExpected, testData.Data3RegistExpected, testData.Data4RegistExpected, testData.Data5RegistExpected
            };
            client.GetWebApiResponseResult(api.RegistList(allData)).Assert(RegisterSuccessExpectStatusCode, allDataRegistExpected);

            // ページングを有効にする
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, " ");

            // 2件ずつ取得 Orderby なし　順不同なので件数のみチェック
            var response = client.GetWebApiResponseResult(api.OData("$top=2")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(2);

            var continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

            response = client.GetWebApiResponseResult(api.OData("$top=2")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(2);

            continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

            response = client.GetWebApiResponseResult(api.OData("$top=2")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(1);

            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");

            // 3件ずつ取得 Orderby　あり
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { " " };
            response = client.GetWebApiResponseResult(api.OData("$top=3&$orderby=AreaUnitCode desc")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(3);
            response.Result[0].IsStructuralEqual(testData.Data5Get);
            response.Result[1].IsStructuralEqual(testData.Data4Get);
            response.Result[2].IsStructuralEqual(testData.Data3Get);

            continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

            response = client.GetWebApiResponseResult(api.OData("$top=3&$orderby=AreaUnitCode desc")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(2);
            response.Result[0].IsStructuralEqual(testData.Data2Get);
            response.Result[1].IsStructuralEqual(testData.Data1Get);

            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");

            // 2件ずつ取得 絞り込みあり
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { " " };
            response = client.GetWebApiResponseResult(api.OData("$top=2&$orderby=AreaUnitCode&$filter=ConversionSquareMeters le 100")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(2);
            response.Result[0].IsStructuralEqual(testData.Data1Get);
            response.Result[1].IsStructuralEqual(testData.Data2Get);

            continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

            response = client.GetWebApiResponseResult(api.OData("$top=2&$orderby=AreaUnitCode&$filter=ConversionSquareMeters le 100")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(1);
            response.Result[0].IsStructuralEqual(testData.Data3Get);

            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");

            // 2件ずつ取得 絞り込みあり
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { " " };
            response = client.GetWebApiResponseResult(api.OData("$top=2&$filter=ConversionSquareMeters ge 11 and ConversionSquareMeters le 10000&$orderby=ConversionSquareMeters")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(2);
            response.Result[0].IsStructuralEqual(testData.Data3Get);
            response.Result[1].IsStructuralEqual(testData.Data4Get);

            continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

            response = client.GetWebApiResponseResult(api.OData("$top=2&$filter=ConversionSquareMeters ge 11 and ConversionSquareMeters le 10000&$orderby=ConversionSquareMeters")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(1);
            response.Result[0].IsStructuralEqual(testData.Data5Get);

            //全部読んだのでX-ResponseContinuationnには空文字が設定されている
            response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");

            // SKIPありで2件ずつ取得(現状SQLServerのみ対応)
            if (repository == Repository.SqlServer)
            {
                api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { " " };
                response = client.GetWebApiResponseResult(api.OData("$skip=2&$top=2&$orderby=AreaUnitCode desc")).Assert(GetSuccessExpectStatusCode);
                response.Result.Count.Is(2);
                response.Result[0].IsStructuralEqual(testData.Data3Get);
                response.Result[1].IsStructuralEqual(testData.Data2Get);

                continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
                api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

                response = client.GetWebApiResponseResult(api.OData("$skip=2&$top=2&$orderby=AreaUnitCode desc")).Assert(GetSuccessExpectStatusCode);
                response.Result.Count.Is(1);
                response.Result[0].IsStructuralEqual(testData.Data1Get);

                // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
                response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");
            }

            // 追加データを登録(1件)
            client.GetWebApiResponseResult(api.Regist(testData.DataEx)).Assert(RegisterSuccessExpectStatusCode);

            // データ登録順とソート順が不一致
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { " " };
            response = client.GetWebApiResponseResult(api.OData("$top=3&$orderby=AreaUnitName")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(3);

            //データは降順で入っている
            response.Result[0].IsStructuralEqual(testData.DataExGet);
            response.Result[1].IsStructuralEqual(testData.Data1Get);
            response.Result[2].IsStructuralEqual(testData.Data2Get);

            continuation = response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First();
            api.AddHeaders[HeaderConst.X_RequestContinuation] = new string[] { continuation };

            response = client.GetWebApiResponseResult(api.OData("$top=3&$orderby=AreaUnitName")).Assert(GetSuccessExpectStatusCode);
            response.Result.Count.Is(3);
            response.Result[0].IsStructuralEqual(testData.Data3Get);
            response.Result[1].IsStructuralEqual(testData.Data4Get);
            response.Result[2].IsStructuralEqual(testData.Data5Get);

            // 全部読んだのでX-ResponseContinuationnには空文字が設定されている
            response.Headers.GetValues(HeaderConst.X_ResponseContinuation).First().Is("");
        }

        // any中心のシナリオのためSQLServerは割愛
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void AutoKeySimpleDataTest_ODataSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();
            var testData = new AutoKeySimpleDataTestData(repository, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データを5件登録
            client.GetWebApiResponseResult(api.RegistListEx(testData.DataODataTest)).Assert(RegisterSuccessExpectStatusCode);

            // 件数取得
            var response = client.GetWebApiResponseResult(api.ODataCount("$count=true")).Assert(GetSuccessExpectStatusCode);
            response.Result.IsStructuralEqual(new List<int>() { 5 });

            // 条件付き件数取得
            response = client.GetWebApiResponseResult(api.ODataCount("$count=true&$filter=ConversionSquareMeters lt 3")).Assert(GetSuccessExpectStatusCode);
            response.Result.IsStructuralEqual(new List<int>() { 2 });

            // Any条件付き件数取得
            response = client.GetWebApiResponseResult(api.ODataCount("$count=true&$filter=MetaList/any(o: o/MetaKey eq 'TestKeyA')")).Assert(GetSuccessExpectStatusCode);
            response.Result.IsStructuralEqual(new List<int>() { 3 });

            // Any条件付き件数取得2
            response = client.GetWebApiResponseResult(api.ODataCount("$count=true&$filter=MetaList/any(o: o/MetaKey eq 'TestKeyA' or o/MetaValue eq 'value2')")).Assert(GetSuccessExpectStatusCode);
            response.Result.IsStructuralEqual(new List<int>() { 5 });

            // Any条件付き件数取得3
            // 【要確認】件数不一致
            response = client.GetWebApiResponseResult(api.ODataCount("$count=true&$filter=MetaList/any(o: o/MetaKey eq 'TestKeyB') and MetaList/any(o: o/MetaValue eq 'value2') and MetaList/any(o: o/MetaKey ne 'TestKeyA')")).Assert(GetSuccessExpectStatusCode);
            response.Result.IsStructuralEqual(new List<int>() { 1 });

            // Any条件付き取得
            client.GetWebApiResponseResult(api.ODataEx("$filter=MetaList/any(o: o/MetaKey eq 'TestKeyB')")).Assert(GetSuccessExpectStatusCode, testData.DataODataTestAnyResult);

            // Any条件付き取得2
            client.GetWebApiResponseResult(api.ODataEx("$filter=MetaList/any(o: o/MetaKey eq 'TestKeyB') and MetaList/any(o: o/MetaValue eq 'value2') and MetaList/any(o: o/MetaKey ne 'TestKeyA')")).Assert(GetSuccessExpectStatusCode, testData.DataODataTestAnyResult);

            // Any条件付き取得 TOP追加
            client.GetWebApiResponseResult(api.ODataEx("$filter=MetaList/any(o: o/MetaKey eq 'TestKeyB')&$top=1")).Assert(GetSuccessExpectStatusCode, testData.DataODataTestAnyResult);

            // Any条件付き取得 TOP追加 orderby 追加
            client.GetWebApiResponseResult(api.ODataEx("$filter=MetaList/any(o: o/MetaKey eq 'TestKeyA')&$top=1&$orderby=AreaUnitCode")).Assert(GetSuccessExpectStatusCode, testData.DataODataTestAnyResult2);
        }

        [TestMethod]
        public void AutoKeySimpleDataTest_URLMatchSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();
            var testData = new AutoKeySimpleDataTestData(Repository.Default, api.ResourceUrl);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Regist(testData.Data1)).Assert(RegisterSuccessExpectStatusCode, testData.Data1RegistExpected);

            // OKパターン
            client.GetWebApiResponseResult(api.GetByAreaUnitCode("AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            client.GetWebApiResponseResult(api.GetByAreaUnitCodeAndName("AA", "aaa")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            // QueryStringのKeyの順番が変わってもOK
            client.GetWebApiResponseResult(api.GetByAreaUnitNameAndCode("aaa", "AA")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            // URLパスとQueryString
            client.GetWebApiResponseResult(api.GetByUrlParamAndQueryString("AA", "aaa")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);
            // 2階層
            client.GetWebApiResponseResult(api.GetByUrlParam2layer("AA", "aaa")).Assert(GetSuccessExpectStatusCode, testData.Data1Get);

            // NGパターン
            // 定義されていないKeyが含まれる
            client.GetWebApiResponseResult(api.GetWithInvalidQueryString2("AA", "AA")).Assert(NotImplementedExpectStatusCode);
            // 定義されていないKeyのみ
            client.GetWebApiResponseResult(api.GetWithInvalidQueryString("AA")).Assert(NotImplementedExpectStatusCode);
            // 定義されていないパス
            client.GetWebApiResponseResult(api.GetByInvalidPath()).Assert(NotImplementedExpectStatusCode);
        }

        [TestMethod]
        public void AutoKeySimpleDataTest_Regist_BadRequestCase()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();

            // 空値
            client.GetWebApiResponseResult(api.RegistEmptyString()).AssertErrorCode(BadRequestStatusCode, "E10404");

            // 空Json
            client.GetWebApiResponseResult(api.RegistEmptyObject(new AreaUnitModel())).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void AutoKeySimpleDataTest_Success_EmptyArray()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAutoKeySimpleDataApi>();

            // 空配列
            var response = client.GetWebApiResponseResult(api.RegistEmptyArray(new List<AreaUnitModel>())).Assert(RegisterSuccessExpectStatusCode);
            // 結果は成功となるが何も処理されない(idの返却が無い)
            response.RawContentString.Is("[]");
        }
    }
}
