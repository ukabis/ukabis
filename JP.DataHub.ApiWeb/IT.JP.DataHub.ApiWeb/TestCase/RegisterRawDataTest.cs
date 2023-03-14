using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class RegisterRawDataTest : ApiWebItTestCase
    {
        #region TestData

        private class RegisterRawDataTestData : TestDataBase
        {
            protected override string BaseRepositoryKeyPrefix { get; set; } = "API~IntegratedTest~RegisterRawDataTest";

            public List<AreaUnitModel> Data1 = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "EE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5
                },
                new AreaUnitModel()
                {
                    AreaUnitCode = "FF",
                    AreaUnitName = "fff",
                    ConversionSquareMeters = 6
                }
            };

            public List<AreaUnitModel> Data1Expected = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "EE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "FF",
                    AreaUnitName = "fff",
                    ConversionSquareMeters = 6
                }
            };
            public List<AreaUnitModel> Data1ExpectedFull = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "AA",
                    AreaUnitName = "aaa",
                    ConversionSquareMeters = 1,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "DD",
                    AreaUnitName = "ddd",
                    ConversionSquareMeters = 4,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "EE",
                    AreaUnitName = "eee",
                    ConversionSquareMeters = 5,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "FF",
                    AreaUnitName = "fff",
                    ConversionSquareMeters = 6,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                }
            };

            public AreaUnitModel DataVendorPrivate = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~RegisterRawDataTest~1~AA",
                AreaUnitCode = "AA",
                AreaUnitName = "AA",
                ConversionSquareMeters = 1,
                _partitionkey = $"API~IntegratedTest~RegisterRawDataTest~1",
                _Type = $"API~IntegratedTest~RegisterRawDataTest",
                _Upddate = "2021-09-09T09:11:20.3286826Z",
                _Regdate = "2021-09-09T09:11:20.3286826Z",
                _Version = 1
            };

            public AreaUnitModel DataPublic = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~RegisterRawDataTest~1~AA",
                AreaUnitCode = "AA",
                AreaUnitName = "AA",
                ConversionSquareMeters = 1,
                _partitionkey = $"API~IntegratedTest~RegisterRawDataTest~1",
                _Type = $"API~IntegratedTest~RegisterRawDataTest",
                _Upddate = "2021-09-09T09:11:20.3286826Z",
                _Regdate = "2021-09-09T09:11:20.3286826Z",
                _Version = 1
            };

            public AreaUnitModel DataPersonPrivate = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~RegisterRawDataTest~1~AA",
                AreaUnitCode = "AA",
                AreaUnitName = "AA",
                ConversionSquareMeters = 1,
                _partitionkey = $"API~IntegratedTest~RegisterRawDataTest~1",
                _Type = $"API~IntegratedTest~RegisterRawDataTest",
                _Upddate = "2021-09-09T09:11:20.3286826Z",
                _Regdate = "2021-09-09T09:11:20.3286826Z",
                _Version = 1
            };

            public List<AreaUnitModel> Data1ExpectRawDataBB = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "BB",
                    AreaUnitName = "bbb",
                    ConversionSquareMeters = 2,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                }
            };

            public List<AreaUnitModel> Data1ExpectRawDataCC = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                },
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3,
                    _partitionkey =  $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Type =  $"API~IntegratedTest~RegisterRawDataTest",
                    _Upduser_Id = WILDCARD,
                    _Upddate = WILDCARD,
                    _Reguser_Id = WILDCARD,
                    _Regdate = WILDCARD,
                    _Vendor_Id = WILDCARD,
                    _System_Id = WILDCARD,
                    _Version = 1
                }
            };

            public List<AreaUnitModel> Data1ExpectCC = new List<AreaUnitModel>()
            {
                new AreaUnitModel()
                {
                    id = $"API~IntegratedTest~RegisterRawDataTest~{WILDCARD}",
                    _Owner_Id = WILDCARD,
                    AreaUnitCode = "CC",
                    AreaUnitName = "ccc",
                    ConversionSquareMeters = 3
                }
            };

            public RegisterRawDataTestData(Repository repository, string resourceUrl, bool isVendor, bool isPerson, IntegratedTestClient client = null)
                : base(repository, resourceUrl, isVendor, isPerson, client) { }
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        #region RegisterRawData

        // データ追加のみ
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RegisterRawData_NormalScenario_OverWriteDataTest(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // GetInternalAllField でGet
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var changeContent = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 一旦データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // そのまま登録されている確認のため、データを変える
            changeContent[0].id = changeContent[0].id.Replace("AA", "GA");
            changeContent[1].id = changeContent[1].id.Replace("BB", "HA");
            changeContent[2].id = changeContent[2].id.Replace("CC", "IA");
            changeContent[3].id = changeContent[3].id.Replace("DD", "JA");
            changeContent[4].id = changeContent[4].id.Replace("EE", "KA");
            changeContent[5].id = changeContent[5].id.Replace("FF", "LA");

            // このままRegisterRawDataで登録する
            var check = client.GetWebApiResponseResult(api.RegisterRawData(changeContent)).Assert(RegisterSuccessExpectStatusCode).Result;

            // IDが返って来るはず
            check[0].id.Is(changeContent[0].id);
            check[1].id.Is(changeContent[1].id);
            check[2].id.Is(changeContent[2].id);
            check[3].id.Is(changeContent[3].id);
            check[4].id.Is(changeContent[4].id);
            check[5].id.Is(changeContent[5].id);

            // データ取得
            var content_aft = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;
            // RegDate、UpdDate含めて、すべて同一のはず
            content_aft.IsStructuralEqual(changeContent);
        }

        // データ追加のみ(管理項目欠落)
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RegisterRawData_NormalScenario_NoAdminProp(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // GetInternalAllField 無しで、Get
            var content = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // チェック用
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var contentforCheck = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 一旦データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // このままRegisterRawDataで登録する
            var check = client.GetWebApiResponseResult(api.RegisterRawData(content)).Assert(RegisterSuccessExpectStatusCode).Result;

            // IDが返って来るはず
            check[0].id.Is(contentforCheck[0].id);
            check[1].id.Is(contentforCheck[1].id);
            check[2].id.Is(contentforCheck[2].id);
            check[3].id.Is(contentforCheck[3].id);
            check[4].id.Is(contentforCheck[4].id);
            check[5].id.Is(contentforCheck[5].id);

            // データ取得
            var content_aft = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // 管理項目が振られ直されているので、同一ではない
            content_aft.IsNotStructuralEqual(contentforCheck);
            if (IsIgnoreGetInternalAllField)
            {
                //同一ではないが、問題なく登録されていること
                content_aft.IsStructuralEqual(testData.Data1Expected);
            }
            else
            {
                //同一ではないが、管理項目含めて、問題なく登録されていること
                content_aft.IsStructuralEqual(testData.Data1ExpectedFull);
            }
        }

        // 既存データがあり（上書き）且つ、追加データ有り
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RegisterRawData_NormalScenario_AddDataTest(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            client.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // GetInternalAllField でGet
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var changeContent = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;

            // そのまま登録されている確認のため、データを変える
            // 後ろに足していく
            var adds = DeepClone(changeContent[0]);
            adds.id = adds.id.Replace("AA", "GA");
            changeContent.Add(adds);
            adds = DeepClone(changeContent[1]);
            adds.id = adds.id.Replace("BB", "HA");
            changeContent.Add(adds);
            adds = DeepClone(changeContent[2]);
            adds.id = adds.id.ToString().Replace("CC", "IA");
            changeContent.Add(adds);
            adds = DeepClone(changeContent[3]);
            adds.id = adds.id.ToString().Replace("DD", "JA");
            changeContent.Add(adds);
            adds = DeepClone(changeContent[4]);
            adds.id = adds.id.ToString().Replace("EE", "KA");
            changeContent.Add(adds);
            adds = DeepClone(changeContent[5]);
            adds.id = adds.id.ToString().Replace("FF", "LA");
            changeContent.Add(adds);

            // RegisterRawDataで登録する
            var check = client.GetWebApiResponseResult(api.RegisterRawData(changeContent)).Assert(RegisterSuccessExpectStatusCode).Result;

            //IDが返って来るはず
            check[0].id.Is(changeContent[0].id);
            check[1].id.Is(changeContent[1].id);
            check[2].id.Is(changeContent[2].id);
            check[3].id.Is(changeContent[3].id);
            check[4].id.Is(changeContent[4].id);
            check[5].id.Is(changeContent[5].id);
            check[6].id.Is(changeContent[6].id);
            check[7].id.Is(changeContent[7].id);
            check[8].id.Is(changeContent[8].id);
            check[9].id.Is(changeContent[9].id);
            check[10].id.Is(changeContent[10].id);
            check[11].id.Is(changeContent[11].id);

            //データ取得
            var content_aft = client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode).Result;
            // RegDate、UpdDate含めて、すべて同一のはず
            content_aft.IsStructuralEqual(changeContent);
        }

        /// <summary>
        /// ベンダー依存のAPIへの登録
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RegisterRawData_VendorPrivateTest(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testDataA = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false, clientA);
            var testDataB = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false, clientB);

            testDataA.DataVendorPrivate._Vendor_Id = clientA.VendorSystemInfo.VendorId;
            testDataA.DataVendorPrivate._System_Id = clientA.VendorSystemInfo.SystemId;
            testDataA.DataVendorPrivate._Reguser_Id = Guid.NewGuid().ToString();
            testDataA.DataVendorPrivate._Upduser_Id = Guid.NewGuid().ToString();
            testDataA.DataVendorPrivate._Owner_Id = Guid.NewGuid().ToString();

            testDataB.DataVendorPrivate._Vendor_Id = clientB.VendorSystemInfo.VendorId;
            testDataB.DataVendorPrivate._System_Id = clientB.VendorSystemInfo.SystemId;
            testDataB.DataVendorPrivate._Reguser_Id = Guid.NewGuid().ToString();
            testDataB.DataVendorPrivate._Upduser_Id = Guid.NewGuid().ToString();
            testDataB.DataVendorPrivate._Owner_Id = Guid.NewGuid().ToString();

            var dataList = new List<AreaUnitModel>() { testDataA.DataVendorPrivate, testDataB.DataVendorPrivate };

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            clientA.GetWebApiResponseResult(api.RegisterRawData(dataList)).Assert(RegisterSuccessExpectStatusCode);

            // GetInternalAllField でGet
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testDataA.DataVendorPrivate });
            clientB.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testDataB.DataVendorPrivate });
        }

        /// <summary>
        /// 依存なしのAPIへの登録
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RegisterRawData_PublicTest(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataPublicApi>();
            var testData = new RegisterRawDataTestData(repository, api.ResourceUrl, false, false, client);

            testData.DataPublic._Reguser_Id = Guid.NewGuid().ToString();
            testData.DataPublic._Upduser_Id = Guid.NewGuid().ToString();
            testData.DataPublic._Owner_Id = Guid.NewGuid().ToString();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            client.GetWebApiResponseResult(api.RegisterRawData(new List<AreaUnitModel>() { testData.DataPublic })).Assert(RegisterSuccessExpectStatusCode);

            // GetInternalAllField でGet
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            client.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.DataPublic });
        }

        /// <summary>
        /// 個人依存のAPIへの登録
        /// </summary>
        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void RegisterRawData_PersonPrivateTest(Repository repository)
        {
            var clientA = new IntegratedTestClient("test1") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2") { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataPersonPrivateApi>();
            var testDataA = new RegisterRawDataTestData(repository, api.ResourceUrl, false, true, clientA);
            var testDataB = new RegisterRawDataTestData(repository, api.ResourceUrl, false, true, clientB);

            testDataA.DataVendorPrivate._Reguser_Id = Guid.NewGuid().ToString();
            testDataA.DataVendorPrivate._Upduser_Id = Guid.NewGuid().ToString();
            testDataA.DataVendorPrivate._Owner_Id = clientA.GetOpenId();

            testDataB.DataVendorPrivate._Reguser_Id = Guid.NewGuid().ToString();
            testDataB.DataVendorPrivate._Upduser_Id = Guid.NewGuid().ToString();
            testDataB.DataVendorPrivate._Owner_Id = clientB.GetOpenId();

            var dataList = new List<AreaUnitModel>() { testDataA.DataVendorPrivate, testDataB.DataVendorPrivate };

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ登録
            clientA.GetWebApiResponseResult(api.RegisterRawData(dataList)).Assert(RegisterSuccessExpectStatusCode);

            // GetInternalAllField でGet
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testDataA.DataVendorPrivate });
            clientB.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testDataB.DataVendorPrivate });
        }

        [TestMethod]
        public void RegisterRawData_BadRequestTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(Repository.Default, api.ResourceUrl, true, false, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 空
            client.GetWebApiResponseResult(api.RegisterRawDataAsString(string.Empty)).AssertErrorCode(BadRequestStatusCode, "E10405");
            // 空Json
            client.GetWebApiResponseResult(api.RegisterRawDataAsString("{}")).Assert(BadRequestStatusCode);
            // JsonObject
            client.GetWebApiResponseResult(api.RegisterRawDataAsString("{'AreaUnitCode':'AA'}")).Assert(BadRequestStatusCode);
            // Json異常
            client.GetWebApiResponseResult(api.RegisterRawDataAsString("{'AreaUnitCode''AA'}")).Assert(BadRequestStatusCode);
        }

        [TestMethod]
        public void RegisterRawData_AdminHeaderAuthFail_InvalidStringTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(Repository.Default, api.ResourceUrl, true, false, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 間違えたAdminヘッダ設定
            api.AddHeaders.Add(HeaderConst.XAdmin, "HOGE");

            // Forbidden が返って来ること
            client.GetWebApiResponseResult(api.RegisterRawData(testData.Data1)).AssertErrorCode(ForbiddenExpectStatusCode, "E02404");
        }

        [TestMethod]
        public void RegisterRawData_AdminHeaderAuthFail_NoAdminHeaderTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(Repository.Default, api.ResourceUrl, true, false, client);

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Adminヘッダ無し
            client.DisableAdminAuthentication();

            // Forbidden が返って来ること
            client.GetWebApiResponseResult(api.RegisterRawData(testData.Data1)).Assert(ForbiddenExpectStatusCode);
        }

        [TestMethod]
        public void RegisterRawData_NonOperatingVendorUserTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem");
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(Repository.Default, api.ResourceUrl, true, false, client);

            // Forbidden が返って来ること
            client.GetWebApiResponseResult(api.RegisterRawData(testData.Data1)).AssertErrorCode(ForbiddenExpectStatusCode, "E10441");
        }

        #endregion

        #region ODataRawData

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void ODataRawData_NormalSenario(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false);

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // ベンダーごとにデータを登録
            clientA.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 領域越え+管理項目ありでデータ取得されること
            clientA.GetWebApiResponseResult(api.ODataRawData("$filter=AreaUnitCode eq 'BB'")).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectRawDataBB);
        }

        [TestMethod]
        public void ODataRawData_AdminHeaderAuthFail_InvalidStringTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRegisterRawDataApi>();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // 間違えたAdminヘッダ設定
            api.AddHeaders.Add(HeaderConst.XAdmin, "HOGE");

            // Forbidden が返って来ること
            client.GetWebApiResponseResult(api.ODataRawData()).AssertErrorCode(ForbiddenExpectStatusCode, "E02404");
        }

        [TestMethod]
        public void ODataRawData_AdminHeaderAuthFail_NoAdminHeaderTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRegisterRawDataApi>();

            // 最初に全データの消去
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // Adminヘッダ無し
            client.DisableAdminAuthentication();

            // Forbidden が返って来ること
            client.GetWebApiResponseResult(api.ODataRawData()).Assert(ForbiddenExpectStatusCode);
        }

        [TestMethod]
        public void ODataRawData_NonOperatingVendorUserTest()
        {
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChain2TestSystem");
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(Repository.Default, api.ResourceUrl, true, false, client);

            // Forbidden が返って来ること
            client.GetWebApiResponseResult(api.ODataRawData()).AssertErrorCode(ForbiddenExpectStatusCode, "E10441");
        }

        #endregion

        #region Migration

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        public void Migration_NormalSenario(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IRegisterRawDataApi>();
            var testData = new RegisterRawDataTestData(repository, api.ResourceUrl, true, false);

            // 最初に全データの消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // ベンダーごとにデータを登録
            clientA.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.RegisterList(testData.Data1)).Assert(RegisterSuccessExpectStatusCode);

            // 移行データ取得
            var migrationData = clientA.GetWebApiResponseResult(api.ODataRawData("$filter=AreaUnitCode eq 'CC'")).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectRawDataCC).Result;

            // データ消去
            clientA.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);
            clientB.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteSuccessStatusCode);

            // 移行データ登録
            clientA.GetWebApiResponseResult(api.RegisterRawData(migrationData)).Assert(RegisterSuccessExpectStatusCode);

            // 移行結果取得
            clientA.GetWebApiResponseResult(api.ODataRawData("$filter=AreaUnitCode eq 'CC'")).Assert(GetSuccessExpectStatusCode, migrationData);

            clientA.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectCC);
            clientB.GetWebApiResponseResult(api.OData()).Assert(GetSuccessExpectStatusCode, testData.Data1ExpectCC);
        }

        #endregion


        private AreaUnitModel DeepClone(AreaUnitModel model)
        {
            return JsonConvert.DeserializeObject<AreaUnitModel>(JsonConvert.SerializeObject(model));
        }
    }
}
