using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class DynamicApiTest : ManageApiTestCase
    {
        private string _attachFileMetaRepositoryGroupId;
        private string _attachFileBlobRepositoryGroupId;
        private string _documentHistoryRepositoryGroupId;
        private string _vendorId;
        private string _systemId;
        private string _vendorId2;
        private string _systemId2;
        private string _repositoryGroupId;
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
            _vendorId = AppConfig.AdminVendorId;
            _systemId = AppConfig.AdminSystemId;
            _vendorId2 = AppConfig.NormalVendorId;
            _systemId2 = AppConfig.NormalSystemId;
            _repositoryGroupId = AppConfig.RepositoryGroupId;
            _attachFileMetaRepositoryGroupId = AppConfig.AttachFileMetaRepositoryId;
            _attachFileBlobRepositoryGroupId = AppConfig.AttachFileBlobRepositoryId;
            _documentHistoryRepositoryGroupId = AppConfig.DocumentHistoryRepositoryId;
        }
        /// <summary>
        /// 正常登録のテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_NormalSenario_DefaultSettingsTest()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            CleanUpApiByUrl(client, "/API/IntegratedTest/ManageDynamicApi01");
            // 必須以外の項目を指定しない場合の項目確認
            var apiId = client.GetWebApiResponseResult(api.RegisterApi(CreateRequestModel())).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;
            var apiResult = client.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiId)).Assert(GetSuccessExpectStatusCode).Result;

            CheckOtherSettings(apiResult, false);
            CheckDefaultTrueSettings(apiResult, true);

            CheckAttachFileSettings(apiResult, false);
            CheckBlockchainSettings(apiResult, false);
        }

        /// <summary>
        /// 正常登録のテスト(任意項目を設定)
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_NormalScenario_OptionalSettingsTest()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            CleanUpApiByUrl(client, "/API/IntegratedTest/ManageDynamicApi02");
            var apiId = client.GetWebApiResponseResult(api.RegisterApi(Data1())).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;
            var apiResult = client.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiId, true)).Assert(GetSuccessExpectStatusCode).Result;

            CheckOtherSettings(apiResult, true);
            CheckDefaultTrueSettings(apiResult, false);
            CheckAttachFileSettings(apiResult, true);
            CheckBlockchainSettings(apiResult, true);
        }

        /// <summary>
        /// 正常登録のテスト(運用管理ベンダー)
        /// </summary>
        /// <remarks>
        /// 運用管理ベンダーによる依存設定変更を禁止している環境では失敗する。
        /// (AllowResourceDependencyChangeByManageApi=false)
        /// AdminTestの実行環境では依存設定変更を禁止しないことが前提。
        /// もし不都合が生じた場合には適宜Ignore化などの対応を行うこと。
        /// </remarks>
        [TestMethod]
        public void ManageDynamicApiTest_NormalScenario_OperatiingVendorTest()
        {

            var client = new DynamicApiClient(AppConfig.Account);
            var client2 = new ManageApiIntegratedTestClient("testStaff", "SmartFoodChain2TestSystem");
            var api = UnityCore.Resolve<IDynamicApiApi>();

            CleanUpApiByUrl(client, "/API/Individual/IntegratedTest/ManageDynamicApi03");

            var apiId = client2.GetWebApiResponseResult(api.RegisterApi(Data2(false, client2.VendorSystemInfo.VendorId, client2.VendorSystemInfo.SystemId))).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;
            var apiResult = client2.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiId)).Assert(GetSuccessExpectStatusCode).Result;
            apiResult.IsVendor.Is(false);

            // 運用管理ベンダー以外は依存設定変更不可
            apiId = client2.GetWebApiResponseResult(api.RegisterApi(Data2(true, client2.VendorSystemInfo.VendorId, client2.VendorSystemInfo.SystemId))).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;
            apiResult = client2.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiId)).Assert(GetSuccessExpectStatusCode).Result;
            apiResult.IsVendor.Is(false);

            // 運用管理ベンダーは依存設定変更可能
            apiId = client.GetWebApiResponseResult(api.RegisterApi(Data2(true, client2.VendorSystemInfo.VendorId, client2.VendorSystemInfo.SystemId))).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;
            apiResult = client.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiId)).Assert(GetSuccessExpectStatusCode).Result;
            apiResult.IsVendor.Is(true);

        }

        /// <summary>
        /// 正常登録のテスト(運用管理ベンダー、依存設定変更禁止)
        /// </summary>
        /// <remarks>
        /// 運用管理ベンダーによる依存設定変更が禁止されている場合に依存設定が変更されないことを確認するテスト。
        /// AdminTestの実行環境では依存設定変更を禁止しないことが前提のため通常はIgnore。
        /// </remarks>
        [Ignore]
        [TestMethod]
        public void ManageDynamicApiTest_NormalScenario_ResourceDependencyChangeNotAllowedTest()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            CleanUpApiByUrl(client, "/API/IntegratedTest/ManageDynamicApi04");
            var apiId = client.GetWebApiResponseResult(api.RegisterApi(Data3(false, false))).Assert(RegisterSuccessExpectStatusCode).Result.ApiId;
            var apiResult = client.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiId)).Assert(GetSuccessExpectStatusCode).Result;

            apiResult.IsVendor.Is(false);
            apiResult.IsPerson.Is(false);

            // 運用管理ベンダーでも依存設定変更不可
            client.GetWebApiResponseResult(api.RegisterApi(Data3(true, false))).Assert(ForbiddenExpectStatusCode);
            client.GetWebApiResponseResult(api.RegisterApi(Data3(false, true))).Assert(ForbiddenExpectStatusCode);
        }


        /// <summary>
        /// 異常登録(バリデーションエラー)のテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_ValidationError()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/ManageDynamicApiError01";
            CleanUpApiByUrl(client, apiUrl);

            // スキーマ登録
            var schemaResult = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;

            // リクエストモデルのバリデーションテスト
            // 添付ファイル
            var result = client.Request(api.RegisterApi(ValidationErrorData_AttachFileSettings(this._vendorId, this._systemId, apiUrl, schemaResult.SchemaId)));
            result.StatusCode.Is(BadRequestStatusCode);
            result.Content.ReadAsStringAsync().Result.Contains("添付ファイルを有効にする場合はMetaRepositoryIdを指定してください").IsTrue();
            // 規約同意
            client.GetWebApiResponseResult(api.RegisterApi(ValidationErrorData_Agreement(this._vendorId, this._systemId, apiUrl, schemaResult.SchemaId))).Assert(BadRequestStatusCode);
            // 履歴（HistoryRepositoryId = null）
            result = client.Request(api.RegisterApi(ValidationErrorData_DocumentHistorySettings_HistoryRepositoryId_null(this._vendorId, this._systemId, apiUrl, schemaResult.SchemaId)));
            result.StatusCode.Is(BadRequestStatusCode);
            result.Content.ReadAsStringAsync().Result.Contains("履歴機能を有効にする場合はHistoryRepositoryIdを指定してください。").IsTrue();

            // 履歴（HistoryRepositoryId = 履歴用リポジトリでないGuidを設定）
            // NOTE ネイティブのForeign Key Errorメッセージが出力される。環境毎に微妙にメッセージが異なる可能性があるため、messageの検証は行わない。
            client.GetWebApiResponseResult(api.RegisterApi(ValidationErrorData_DocumentHistorySettings_HistoryRepositoryId_NotFound(this._vendorId, this._systemId, apiUrl, schemaResult.SchemaId))).Assert(BadRequestStatusCode);

            // SystemIdがVendorに属していない（登録時）
            result = client.Request(api.RegisterApi(ValidationErrorData_Default(this._vendorId, this._systemId2, apiUrl, schemaResult.SchemaId)));
            result.StatusCode.Is(BadRequestStatusCode);
            result.Content.ReadAsStringAsync().Result.Contains("指定されたVendorIdに所属するSystemIdを指定してください。").IsTrue();

            // SystemIdがVendorに属していない（更新時）
            result = client.Request(api.RegisterApi(ValidationErrorData_Default(this._vendorId, this._systemId2, apiUrl, schemaResult.SchemaId)));
            result.StatusCode.Is(BadRequestStatusCode);
            result.Content.ReadAsStringAsync().Result.Contains("指定されたVendorIdに所属するSystemIdを指定してください。").IsTrue();
        }


        // リソースのその他設定のテスト
        private void CheckOtherSettings(ApiModel result, bool expectBool)
        {
            result.IsDocumentHistory.Is(expectBool);
            result.IsOptimisticConcurrency.Is(expectBool);
            result.IsVisibleAgreement.Is(expectBool);
        }
        private void CheckDefaultTrueSettings(ApiModel result, bool expectBool)
        {
            result.IsEnableResourceVersion.Is(expectBool);
        }

        private void CheckAttachFileSettings(ApiModel result, bool expectBool)
        {
            result.IsEnableAttachFile.Is(expectBool);
            if (!expectBool) { return; }

            // リポジトリグループの設定確認 Blockchainはここでは検証しない
            var attachFileApi = result.MethodList.Where(x => x.MethodUrl.Contains("AttachFile") && !x.MethodUrl.Contains("Blockchain") && !x.MethodUrl.Contains("Document"))
                .Select(y => new MethodModel
                {
                    RepositoryGroupId = y.RepositoryGroupId,
                    MethodType = y.MethodType,
                    MethodUrl = y.MethodUrl,
                    SecondaryRepositoryMapList = y.SecondaryRepositoryMapList.Select(z => new SecondaryRepositoryModel
                    {
                        IsPrimary = z.IsPrimary,
                        RepositoryGroupId = z.RepositoryGroupId
                    }).ToList()
                }).OrderBy(x => x.MethodUrl).ToList();
            attachFileApi.IsStructuralEqual(AttachFileExpect());
        }

        private void CheckBlockchainSettings(ApiModel result, bool expectBool)
        {
            result.IsEnableBlockchain.Is(expectBool);
            if (!expectBool) { return; }

            // リポジトリグループの設定確認
            var blockchainApi = result.MethodList.Where(x => x.MethodUrl.Contains("Blockchain"))
                .Select(y => new MethodModel
                {
                    RepositoryGroupId = y.RepositoryGroupId,
                    MethodType = y.MethodType,
                    MethodUrl = y.MethodUrl,
                    SecondaryRepositoryMapList = y.SecondaryRepositoryMapList.Select(z => new SecondaryRepositoryModel
                    {
                        IsPrimary = z.IsPrimary,
                        RepositoryGroupId = z.RepositoryGroupId
                    }).ToList()
                }).OrderBy(x => x.MethodUrl).ToList();
            blockchainApi.IsStructuralEqual(BlockchainExpect());
        }


        /// <summary>
        /// API/メソッド/スキーマのCRUDのテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_NormalSenario_Crud()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/Crudtest";

            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);

            // API削除(BadRequest:パラメータNG)
            client.GetWebApiResponseResult(api.DeleteApi(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteApi("test")).Assert(BadRequestStatusCode);
            // API削除(NotFound:存在チェックNG)
            client.GetWebApiResponseResult(api.DeleteApi(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // メソッド削除(BadRequest:パラメータNG)
            client.GetWebApiResponseResult(api.DeleteMethod(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteMethod("test")).Assert(BadRequestStatusCode);
            // メソッド削除(NotFound:存在チェックNG)
            client.GetWebApiResponseResult(api.DeleteMethod(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // スキーマ削除(BadRequest:パラメータNG)
            client.GetWebApiResponseResult(api.DeleteSchema(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteSchema("test")).Assert(BadRequestStatusCode);
            // メソッド削除(NotFound:存在チェックNG)
            client.GetWebApiResponseResult(api.DeleteSchema(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // スキーマ登録
            var schemaResult1 = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaResult1.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド登録
            var methodResult1 = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForCrudTest(apiResult1.ApiId, schemaResult1.SchemaId, this._repositoryGroupId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // スキーマ取得
            client.GetWebApiResponseResult(api.GetSchemaById(schemaResult1.SchemaId)).Assert(GetExpectStatusCodes);
            // API取得
            client.GetWebApiResponseResult(api.GetApiResourceFromApiId(apiResult1.ApiId, false)).Assert(GetSuccessExpectStatusCode);
            // メソッド取得
            client.GetWebApiResponseResult(api.GetApiMethod(methodResult1.MethodId)).Assert(GetSuccessExpectStatusCode);
            // スキーマ更新
            var schemaResult2 = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;
            Assert.AreEqual(schemaResult1.SchemaId, schemaResult2.SchemaId);
            // API更新
            var apiResult2 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaResult1.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            Assert.AreEqual(apiResult1.ApiId, apiResult2.ApiId);
            // メソッド更新
            var methodResult2 = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForCrudTest(apiResult1.ApiId, schemaResult1.SchemaId, this._repositoryGroupId))).Assert(RegisterSuccessExpectStatusCode).Result;
            Assert.AreEqual(methodResult1.MethodId, methodResult2.MethodId);
            // スキーマ削除(BadRequest:使用中)
            client.GetWebApiResponseResult(api.DeleteSchema(schemaResult1.SchemaId)).Assert(BadRequestStatusCode);
            // メソッド削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteMethod(methodResult1.MethodId)).Assert(DeleteSuccessStatusCode);
            // スキーマ削除(BadRequest:使用中)
            client.GetWebApiResponseResult(api.DeleteSchema(schemaResult1.SchemaId)).Assert(BadRequestStatusCode);
            // API削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteApi(apiResult1.ApiId)).Assert(DeleteSuccessStatusCode);
            // スキーマ削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteSchema(schemaResult1.SchemaId)).Assert(DeleteSuccessStatusCode);
        }
        /// <summary>
        /// API/メソッド/スキーマのCRUDのテスト(VendorIdを指定したスキーマ登録のテスト)
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_NormalSenario_Crud_VendorIdを指定したスキーマ登録のテスト()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            const string SpecifiedVendorApiUrl = "/API/IntegratedTest/ManageDynamicApi/Crud/SpecifiedVendorApiUrltest";
            // Initialize
            CleanUpApiByUrl(client, SpecifiedVendorApiUrl, true);
            // VendorIdを指定してスキーマ登録
            var schemaSpecifiedVendorResult = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(SpecifiedVendorApiUrl, this._vendorId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiSpecifiedVendorResult = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, SpecifiedVendorApiUrl, schemaSpecifiedVendorResult.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド登録
            var methodSpecifiedVendorResult = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForCrudTest(apiSpecifiedVendorResult.ApiId, schemaSpecifiedVendorResult.SchemaId, this._repositoryGroupId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // 存在しないベンダーを指定(BadRequest)
            client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(SpecifiedVendorApiUrl, Guid.NewGuid().ToString()))).Assert(BadRequestStatusCode);
            // メソッドで使用中はベンダー変更できない(BadRequest)
            client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(SpecifiedVendorApiUrl, this._vendorId2))).Assert(BadRequestStatusCode);
            // メソッド削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteMethod(methodSpecifiedVendorResult.MethodId)).Assert(DeleteSuccessStatusCode);
            // APIで使用中はベンダー変更できない(BadRequest)
            client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(SpecifiedVendorApiUrl, this._vendorId2))).Assert(BadRequestStatusCode);
            // API削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteApi(apiSpecifiedVendorResult.ApiId)).Assert(DeleteSuccessStatusCode);
            // スキーマ削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteSchema(schemaSpecifiedVendorResult.SchemaId)).Assert(DeleteSuccessStatusCode);

        }
        /// <summary>
        /// API/メソッド/スキーマのCRUDのテスト(定義済みキーワードを含むスキーマ登録のテスト)
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_NormalSenario_Crud_定義済みキーワードを含むスキーマ登録のテスト()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/Crud3";

            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);

            const string DefinedKeywordSchemaName = "/API/IntegratedTest/ManageDynamicApi/Crud/DefinedKeyword";
            var schemaModel = new RegisterSchemaRequestModel()
            {
                SchemaName = DefinedKeywordSchemaName,
                JsonSchema = "{\r\n  'description':'IntegratedTestRegisterSchema',\r\n  'properties': {\r\n    'id': {\r\n      'title': 'テストID',\r\n      'type': 'string',\r\n      'required':true\r\n    },\r\n    'TestName': {\r\n      'title': 'テスト名',\r\n      'type': 'string',\r\n      'required':true\r\n    }\r\n    \r\n  },\r\n  'type': 'object',\r\n  'additionalProperties' : false\r\n}",
            };

            // リクエストモデルにidが含まれている場合は登録できない
            var schemaDefinedKeyword = JsonConvert.SerializeObject(schemaModel);
            var schemaDefinedKeywordResult = client.GetWebApiResponseResult(api.RegisterSchema(schemaModel)).Assert(RegisterErrorExpectStatusCode).Result;
            // レスポンスモデルにidが含まれていても登録できるか
            schemaDefinedKeywordResult = client.GetWebApiResponseResult(api.RegisterUriOrResponseSchema(schemaModel)).Assert(RegisterSuccessExpectStatusCode).Result;
            // レスポンスモデルにidが含まれていても更新できるか
            schemaDefinedKeywordResult = client.GetWebApiResponseResult(api.RegisterUriOrResponseSchema(schemaModel)).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiDefinedKeywordResult = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaDefinedKeywordResult.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド登録(idを含むレスポンスモデルを指定しても登録できるか)
            var definedKeywordMethodModel = MethodDataForCrudTest(apiDefinedKeywordResult.ApiId, schemaDefinedKeywordResult.SchemaId, this._repositoryGroupId);
            var methodDefinedKeywordResult = client.GetWebApiResponseResult(api.RegisterMethod(definedKeywordMethodModel)).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteMethod(methodDefinedKeywordResult.MethodId)).Assert(DeleteSuccessStatusCode);
            // メソッド登録(idを含むURLモデルを指定しても登録できるか)
            definedKeywordMethodModel.UrlModelId = definedKeywordMethodModel.ResponseModelId;
            definedKeywordMethodModel.ResponseModelId = null;
            methodDefinedKeywordResult = client.GetWebApiResponseResult(api.RegisterMethod(definedKeywordMethodModel)).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteMethod(methodDefinedKeywordResult.MethodId)).Assert(DeleteSuccessStatusCode);
            // メソッド登録(idを含むリクエストモデルを指定するとエラーになるか)
            definedKeywordMethodModel.RequestModelId = definedKeywordMethodModel.UrlModelId;
            definedKeywordMethodModel.UrlModelId = null;
            client.GetWebApiResponseResult(api.RegisterMethod(definedKeywordMethodModel)).Assert(RegisterErrorExpectStatusCode);
            // API削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteApi(apiDefinedKeywordResult.ApiId)).Assert(DeleteSuccessStatusCode);
            // スキーマ削除(NoContent)
            client.GetWebApiResponseResult(api.DeleteSchema(schemaDefinedKeywordResult.SchemaId)).Assert(DeleteSuccessStatusCode);
        }
        /// <summary>
        /// メソッドのデフォルト値確認
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_DefaultValue()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/DefaultValue";
            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);
            // スキーマ登録
            var action = api.RegisterSchema(SchemaDataForCrudTest(apiUrl));
            var schemaResult1 = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaResult1.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド登録
            var registMethod = MethodDataForCrudTest(apiResult1.ApiId, schemaResult1.SchemaId, this._repositoryGroupId);
            registMethod.IsClientCertAuthentication = false;
            var registMethodJson = JsonConvert.SerializeObject(registMethod);
            var jsonObj = registMethodJson.ToJson();
            //必須プロパティ
            string[] requiredProperty = new string[]
            {
                nameof(RegisterMethodRequestModel.ApiId),
                nameof(RegisterMethodRequestModel.Url),
                nameof(RegisterMethodRequestModel.ActionTypeCd),
                nameof(RegisterMethodRequestModel.HttpMethodTypeCd),
                nameof(RegisterMethodRequestModel.RepositoryGroupId)
            };
            //全プロパティ
            var allProperty = typeof(RegisterMethodRequestModel).GetProperties().Select(p => p.Name);
            //デフォルト値を確認するために、必須要素以外を削除
            foreach (var property in allProperty.Where(p => requiredProperty.Contains(p) == false))
            {
                jsonObj.RemoveField(property);
            }
            var methodResult1 = client.GetWebApiResponseResult(api.RegisterMethod(JsonConvert.DeserializeObject<RegisterMethodRequestModel>(jsonObj.ToString()))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド取得
            var method = client.GetWebApiResponseResult(api.GetApiMethod(methodResult1.MethodId)).Assert(GetSuccessExpectStatusCode).Result;
            var resultData = method.ToJson();
            //デフォルト値確認
            var expectedDatas = new (string, string)[]
            {
                 ("IsEnable","True"),
                 ("IsHeaderAuthentication","True"),
                 ("IsOpenIdAuthentication","False"),
                 ("IsAdminAuthentication","False"),
                 ("IsOverPartition","False"),
                 ("IsHidden","True"),
                 ("IsCache","False"),
                 ("AccessKey","False"),
                 ("Automatic","False"),
                 ("IsActive","True"),
                 ("IsTransparent","False"),
                 ("IsVendorSystemAuthenticationAllowNull","False"),
                 ("IsVisibleSigninUserOnly","False"),
                 ("IsOtherResourceSqlAccess","False"),
                 ("IsClientCertAuthentication","False"),
            };
            foreach (var data in expectedDatas)
            {
                var expected = resultData[data.Item1].ToString();
                Assert.AreEqual(expected, data.Item2);
            }
        }

        /// <summary>
        /// メソッドの保存値確認
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_SaveValue()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/SaveValue";
            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);
            var repositoryGroupApi = UnityCore.Resolve<IRepositoryGroupApi>();
            var repositoryGroupList = client.GetWebApiResponseResult(repositoryGroupApi.GetRepositoryGroupList()).Assert(GetSuccessExpectStatusCode).Result;
            var repositoryGroupId2 = repositoryGroupList.FirstOrDefault(p => p.RepositoryGroupName == "CosmosDB2[IntegratedTest]").RepositoryGroupId;

            // スキーマ登録
            var schemaResult1 = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaResult1.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド登録
            var registMethod = MethodDataForCrudTest(apiResult1.ApiId, schemaResult1.SchemaId, this._repositoryGroupId, new List<string> { this._repositoryGroupId });
            registMethod.IsHidden = false;
            var registMethodJson = JsonConvert.SerializeObject(registMethod);
            var jsonObj = registMethodJson.ToJson();
            var methodResult1 = client.GetWebApiResponseResult(api.RegisterMethod(JsonConvert.DeserializeObject<RegisterMethodRequestModel>(jsonObj.ToString()))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド取得
            var method = client.GetWebApiResponseResult(api.GetApiMethod(methodResult1.MethodId)).Assert(GetSuccessExpectStatusCode).Result;
            var resultData = method.ToJson();
            var properties = new (string, string)[]
            {
                ("ApiId", "ApiId"),
                ("Url", "MethodUrl"),
                ("HttpMethodTypeCd", "MethodType"),
                ("ActionTypeCd", "ActionTypeCd"),
                ("MethodDescriptiveText", "MethodDescription"),
                ("ResponseModelId", "ResponseSchemaId"),
                ("RepositoryGroupId", "RepositoryGroupId"),
                ("IsEnable", "IsEnable"),
                ("IsHidden", "IsHidden"),
                ("IsCache", "IsCache"),
                ("CacheKey", "CacheKey"),
                ("Script", "Script"),
                ("IsHidden","IsHidden"),
                ("IsClientCertAuthentication","IsClientCertAuthentication"),
            };
            foreach (var data in properties)
            {
                var expected = resultData[data.Item2].ToString();
                Assert.AreEqual(expected.ToLower(), jsonObj[data.Item1].ToString().ToLower());
            }
            Assert.AreEqual(resultData["SecondaryRepositoryMapList"][0]["RepositoryGroupId"].ToString().ToLower(), this._repositoryGroupId.ToLower());
            resultData["SecondaryRepositoryMapList"].Count().Is(1);

            //API更新時の保存値確認（削除せず、同じURL指定で更新）
            registMethod = MethodDataForCrudTest(apiResult1.ApiId, schemaResult1.SchemaId, this._repositoryGroupId, new List<string> { repositoryGroupId2 });
            registMethod.IsHidden = false;
            registMethodJson = JsonConvert.SerializeObject(registMethod);
            jsonObj = registMethodJson.ToJson();

            methodResult1 = client.GetWebApiResponseResult(api.RegisterMethod(registMethod)).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド取得
            var updateMethod = client.GetWebApiResponseResult(api.GetApiMethod(methodResult1.MethodId)).Assert(GetSuccessExpectStatusCode).Result;
            var updateResultData = updateMethod.ToJson();
            foreach (var data in properties)
            {
                var expected = updateResultData[data.Item2].ToString();
                Assert.AreEqual(expected.ToLower(), jsonObj[data.Item1].ToString().ToLower());
            }
            updateResultData["SecondaryRepositoryMapList"].Count().Is(1);
            Assert.AreEqual(updateResultData["SecondaryRepositoryMapList"][0]["RepositoryGroupId"].ToString().ToLower(), repositoryGroupId2.ToLower());
        }

        /// <summary>
        /// メソッドの全保存値確認
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_MethodDataForAllPropertiesTest()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/AllProperties";

            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);

            // スキーマ登録
            var schemaResult1 = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaResult1.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;

            // メソッド登録
            JProperty languageFirstId = null;
            {
                var result = client.GetWebApiResponseResult(api.GetLanguageList()).Assert(GetSuccessExpectStatusCode).Result;
                if (result != null)
                {
                    languageFirstId = result.ToJson()?.First?.FindProperty("LanguageId");
                }
            }
            JProperty openIdCaFirstApplicationId = null;
            {
                var result = client.GetWebApiResponseResult(api.GetOpenIdCaList()).Assert(GetSuccessExpectStatusCode).Result;
                if (result != null)
                {
                    openIdCaFirstApplicationId = result.ToJson()?.First?.FindProperty("ApplicationId");
                }
            }
            var registerMethod = MethodDataAllPropertiesTest(apiResult1.ApiId, schemaResult1.SchemaId,
                this._repositoryGroupId, new List<string> { _repositoryGroupId }, languageFirstId?.Value?.ToString(),
                openIdCaFirstApplicationId?.Value?.ToString());
            var registerMethodResult = client.GetWebApiResponseResult(api.RegisterMethod(registerMethod)).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド取得
            var methodGetResult = client.GetWebApiResponseResult(api.GetApiMethod(registerMethodResult.MethodId)).Assert(GetSuccessExpectStatusCode).Result;
            //比較
            var expected = GetExpectedMethodDataForAllPropertiesTest(registerMethodResult.MethodId, registerMethod, methodGetResult);
            methodGetResult.IsStructuralEqual(expected);
        }

        /// <summary>
        /// リソースをURLで削除するテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_ResourceDeleteFromUrlSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/ApiDelete";
            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);
            // スキーマ登録
            var schemaResult1 = client.GetWebApiResponseResult(api.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;
            // API登録
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForCrudTest(this._vendorId, this._systemId, apiUrl, schemaResult1.SchemaId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // メソッド登録
            var methodResult1 = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForCrudTest(apiResult1.ApiId, schemaResult1.SchemaId, this._repositoryGroupId))).Assert(RegisterSuccessExpectStatusCode).Result;
            // URLでメソッド削除(NotFoundStatusCode・引数がURLでない)
            client.GetWebApiResponseResult(api.DeleteMethodFromUrl("hoge")).Assert(NotFoundStatusCode);
            // URLでメソッド削除(NotFoundStatusCode・存在しないURL)
            client.GetWebApiResponseResult(api.DeleteMethodFromUrl(apiUrl + "A")).Assert(NotFoundStatusCode);
            // URLでApi削除(NotFoundStatusCode・引数がURLでない)
            client.GetWebApiResponseResult(api.DeleteApiFromUrl("hoge")).Assert(NotFoundStatusCode);
            // URLでApi削除(NotFoundStatusCode・存在しないURL)
            client.GetWebApiResponseResult(api.DeleteApiFromUrl(apiUrl + "A")).Assert(NotFoundStatusCode);
            // URLでメソッド削除(NoContents)
            client.GetWebApiResponseResult(api.DeleteMethodFromUrl(apiUrl + "/GetAll")).Assert(DeleteSuccessStatusCode);
            // URLでApi削除(NoContents)
            client.GetWebApiResponseResult(api.DeleteApiFromUrl(apiUrl)).Assert(DeleteSuccessStatusCode);
        }

        /// <summary>
        /// メソッドのURL重複チェックのテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_NormalSenario_Duplicate()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            var runId = Guid.NewGuid().ToString();
            var existingApis = new (string id, string url)[]
            {
                (id: null, url: "/API/IntegratedTest/ManageDynamicApi/Duplicate"),
                (id: null, url: "/API/IntegratedTest/ManageDynamicApi/Duplicate/Segment")
            };
            var existingMethods = new (int apiIndex, string id, string actionType, string url)[]
            {
                // 大文字・小文字違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get"),
                // クエリストリングの変数名・項目順違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get?Param1={{Var1}}&Param2={{Var2}}"),
                // セグメントの変数名違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get/{{Var}}"),
                // Gatewayのクエリストリング違い
                (apiIndex: 0, id: null, actionType: "gtw", url: "{0}/Gateway"),
                // 別APIとの重複
                (apiIndex: 1, id: null, actionType: "quy", url: "{0}/Get")
            };
            var additionalMethods = new (int apiIndex, string id, string actionType, string url, HttpStatusCode expectedStatus, int? existingApiIndex, string msg)[]
            {
                // 大文字・小文字違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/get", RegisterSuccessExpectStatusCode, 0, msg: null),
                // クエリストリングの変数名・項目順違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get?Param1={{Key1}}&Param2={{Key2}}", RegisterSuccessExpectStatusCode, 1, msg: null),
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get?Param2={{Var2}}&Param1={{Var1}}", RegisterSuccessExpectStatusCode, 1, msg: null),
                // セグメントの変数名違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get/{{Key}}", RegisterSuccessExpectStatusCode, 2, msg: null),
                // Gatewayのクエリストリング違い
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Gateway?Param1={{Key1}}", RegisterSuccessExpectStatusCode, 3, msg: null),
                (apiIndex: 0, id: null, actionType: "gtw", url: "{0}/Gateway?Param1={{Key1}}&Param2={{Key2}}", RegisterSuccessExpectStatusCode, 3, msg: null),
                // 別APIとの重複
                (apiIndex: 0, id: null, actionType: "quy", url: "Segment/{0}/Get", BadRequestStatusCode, null, msg: "指定したURLは既に使用されています。"),
                // 重複なし(セグメント数違い)
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get/{{Key1}}/{{Key2}}", RegisterSuccessExpectStatusCode, null, msg: null),
                // 重複なし(パラメータ数違い)
                (apiIndex: 0, id: null, actionType: "quy", url: "{0}/Get?Param1={{Var1}}&Param2={{Var2}}&Param3={{Var3}}", RegisterSuccessExpectStatusCode, null, msg: null),
                // 重複なし(URL違い(Gateway))
                (apiIndex: 0, id: null, actionType: "gtw", url: "{0}/Gateway2", RegisterSuccessExpectStatusCode, null, msg: null),
                // 重複なし(メソッドタイプ違い)
                (apiIndex: 0, id: null, actionType: "reg", url: "{0}/Get", RegisterSuccessExpectStatusCode, null, msg: null)
            };
            // Initialize
            existingApis.ToList().ForEach(x => CleanUpApiByUrl(client, x.url, true));

            // 既存APIを登録
            for (var i = 0; i < existingApis.Length; i++)
            {
                var resultJson = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForDuplicateTest(this._vendorId, this._systemId, existingApis[i].url))).Assert(RegisterSuccessExpectStatusCode).Result;
                existingApis[i].id = resultJson.ApiId;
            }
            // 既存メソッドを登録
            for (var i = 0; i < existingMethods.Length; i++)
            {
                var method = existingMethods[i];
                var resultJson = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForDuplicateTest(method.actionType, existingApis[method.apiIndex].id, string.Format(method.url, runId), this._repositoryGroupId))).Assert(RegisterSuccessExpectStatusCode).Result;
                existingMethods[i].id = resultJson.MethodId;
            }
            // テスト実施
            for (var i = 0; i < additionalMethods.Length; i++)
            {
                var method = additionalMethods[i];
                var action = api.RegisterMethod(MethodDataForDuplicateTest(method.actionType, existingApis[method.apiIndex].id, string.Format(method.url, runId), this._repositoryGroupId));
                if (method.expectedStatus != RegisterSuccessExpectStatusCode)
                {
                    var response = client.Request(action).Content.ReadAsStringAsync().Result;
                    response.ToJson()["Detail"].Value<string>().Contains(additionalMethods[i].msg).IsTrue();
                }
                else
                {
                    var response = client.GetWebApiResponseResult(action).Assert(method.expectedStatus).Result;
                    additionalMethods[i].id = response?.MethodId;

                    if (method.existingApiIndex.HasValue)
                    {
                        response.MethodId.Is(existingMethods[method.existingApiIndex.Value].id);
                    }
                    else
                    {
                        existingMethods.All(x => x.id != response.MethodId).IsTrue();
                        additionalMethods.Where(x => !(x.apiIndex == method.apiIndex && x.url == method.url) && !x.existingApiIndex.HasValue).All(x => x.id != response.MethodId).IsTrue();
                    }
                }
            }
            // CleanUp
            existingApis.ToList().ForEach(x => CleanUpApiByUrl(client, x.url, true));
        }

        // API削除(URL指定)
        private void CleanUpApiByUrl(DynamicApiClient client, string apiUrl, bool cascade = false)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();
            var response = client.GetWebApiResponseResult(api.GetApiResourceFromUrl(apiUrl, true)).Assert(GetExpectStatusCodes).Result;
            if (response == null || string.IsNullOrEmpty(response.ApiId))
            {
                return;
            }
            // API削除
            CleanUpApiById(client,response.ApiId);
            if (!cascade)
            {
                return;
            }
            // メソッド削除
            foreach (var method in response.MethodList)
            {
                CleanUpMethodById(client,method.MethodId);
            }
            // スキーマ削除
            if (Guid.TryParse(response.ApiSchemaId, out _))
            {
                CleanUpSchemaById(client,response.ApiSchemaId);
            }
        }

        // API削除(ID指定)
        private void CleanUpApiById(DynamicApiClient client, string apiId)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(api.DeleteApi(apiId)).Assert(DeleteExpectStatusCodes);
        }

        /// <summary>
        /// API削除(URL指定)
        /// </summary>
        /// <param name="url"></param>
        private void CleanUpApiByUrl(DynamicApiClient client, string url)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();
            // APIが存在しない場合はBadRequestとなるためBadRequestも許容
            var expectedStatus = new HttpStatusCode[] { HttpStatusCode.NoContent, HttpStatusCode.NotFound, HttpStatusCode.BadRequest };
            client.GetWebApiResponseResult(api.DeleteApiFromUrl(url)).Assert(expectedStatus);
        }

        // メソッド削除(ID指定)
        private void CleanUpMethodById(DynamicApiClient client, string methodId)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(api.DeleteMethod(methodId)).Assert(DeleteExpectStatusCodes);
        }

        /// <summary>
        /// メソッド削除(URL指定)
        /// </summary>
        /// <param name="url"></param>
        private void CleanUpMethodByUrl(DynamicApiClient client, string url)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(api.DeleteMethodFromUrl(url)).Assert(DeleteExpectStatusCodes);
        }

        // スキーマ削除(ID指定)
        private void CleanUpSchemaById(DynamicApiClient client, string schemaId)
        {
            var api = UnityCore.Resolve<IDynamicApiApi>();
            client.GetWebApiResponseResult(api.DeleteSchema(schemaId)).Assert(DeleteExpectStatusCodes);
        }

        /// <summary>
        /// APIに必要なマスター系のテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_MasterDataSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            // OkまたはNotFoundになること(環境によって無い場合有)
            client.GetWebApiResponseResult(api.GetCategories()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetFields()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetTags()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetRepositoryGroups()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetActionTypes()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetHttpMethods()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetSchemas()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetAttachFileBlobStorage()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetAttachFileMetaStorage()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetControllerCommonIpFilterGroupList()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetOpenIdCaList()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetLanguageList()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetScriptTypeList()).Assert(GetExpectStatusCodes);
            client.GetWebApiResponseResult(api.GetQueryTypeList()).Assert(GetExpectStatusCodes);
        }


        /// <summary>
        /// クエリパターンテスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_QueryPattern()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/QueryPattern";
            // Initialize
            CleanUpApiByUrl(client,apiUrl, true);

            // API登録
            var action = api.RegisterApi(ApiDataForQueryPatternTest(this._vendorId, this._systemId, apiUrl));
            var apiResult1 = client.GetWebApiResponseResult(action).Assert(RegisterSuccessExpectStatusCode).Result;

            /* 共通 */
            // NG:改行のみ
            string query = "\r\n\r\n\r\n";
            client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, this._repositoryGroupId, query))).Assert(RegisterErrorExpectStatusCode);

            /* MongoDb */
            // NG:混在
            query = @"{ ""Select"": """", ""Aggregate"": [] }";
            var message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(BadRequestStatusCode);
            message.Content.ReadAsStringAsync().Result.Contains("Select").IsTrue();

            // NG:禁止オペレータ
            query = @"{ ""Select"": """", ""Where"": { ""$where"": """" } }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(BadRequestStatusCode);
            message.Content.ReadAsStringAsync().Result.Contains("$where").IsTrue();

            // NG:禁止オペレータ(ネスト)
            query = @"{ ""Aggregate"": [ { ""$match"": { ""key"": ""val"" } }, { ""$unionWith"": { ""coll"": {COLLECTION_NAME}, ""pipeline"": [ { ""$match"": { ""$where"": """" } } ] } } ] }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(BadRequestStatusCode);
            message.Content.ReadAsStringAsync().Result.Contains("$where").IsTrue();

            // NG:禁止ステージ
            query = @"{ ""Aggregate"": [ { ""$currentOp"": {} } ] }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(BadRequestStatusCode);
            message.Content.ReadAsStringAsync().Result.Contains("$currentOp").IsTrue();

            // NG:禁止ステージ(ネスト)
            query = @"{ ""Aggregate"": [ { ""$match"": { ""key"": ""val"" } }, { ""$unionWith"": { ""coll"": {COLLECTION_NAME}, ""pipeline"": [ { ""$currentOp"": {} } ] } } ] }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(BadRequestStatusCode);
            message.Content.ReadAsStringAsync().Result.Contains("$currentOp").IsTrue();

            // NG:禁止オペレータ(ネスト)
            query = @"{ ""Aggregate"": [ { ""$match"": { ""key"": ""val"" } }, { ""$unionWith"": { ""coll"": ""OtherCollection"", ""pipeline"": [ { ""$match"": { ""key"": ""val"" } } ] } } ] }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(BadRequestStatusCode);

            // OK:Find
            query = @"{ ""Select"": { ""Cd"": 1, ""Name"": 1, ""No"": 1 }, ""Where"": { ""$and"": [{ ""Cd"": ""2"" }] }, ""OrderBy"": { ""Cd"": -1 }, ""Top"": 1, ""Skip"": 1 }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(RegisterSuccessExpectStatusCode);

            // OK:Aggregate
            query = @"{ ""Aggregate"": [ { ""$match"": { ""key"": ""val"" } }, { ""$unionWith"": { ""coll"": {COLLECTION_NAME}, ""pipeline"": [ { ""$match"": { ""key"": ""val"" } } ] } } ] }";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(RegisterSuccessExpectStatusCode);

            /* OData */
            query = @"$filter=key eq 'val'";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(RegisterSuccessExpectStatusCode);

            /* CosmosDB */
            query = @"SELECT * FROM c";
            message = client.Request(api.RegisterMethod(MethodDataForQueryPatternTest(apiResult1.ApiId, _repositoryGroupId, query)));
            message.StatusCode.Is(RegisterSuccessExpectStatusCode);

        }

        /// <summary>
        /// 透過APIの更新テスト
        /// </summary>
        [TestMethod]
        public void ManageDynamicApiTest_TransparentApi()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/TransparentApi";
            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);

            // API登録
            var action = api.RegisterApi(ApiDataForQueryPatternTest(this._vendorId, this._systemId, apiUrl));
            var apiResult1 = client.GetWebApiResponseResult(api.RegisterApi(ApiDataForQueryPatternTest(this._vendorId, this._systemId, apiUrl))).Assert(RegisterSuccessExpectStatusCode).Result;

            var transparentApiUrlList = new (string, string)[]
            {
                ("POST","CompleteRegisterVersion"),
                ("POST","CreateAttachFile"),
                ("POST","CreateRegisterVersion"),
                ("DELETE","DeleteAttachFile/{FileId}"),
                ("GET","DriveOutAttachFileDocument/{FileId}"),
                ("GET","DriveOutDocument"),
                ("GET","GetAttachFile/{FileId}"),
                ("GET","GetAttachFileDocumentHistory/{FileId}?version={version}"),
                ("GET","GetAttachFileDocumentVersion/{id}"),
                ("GET","GetAttachFileMeta/{FileId}"),
                ("GET","GetAttachFileMetaList"),
                ("GET","GetCount"),
                ("GET","GetCurrentVersion"),
                ("GET","GetDocumentHistory"),
                ("GET","GetDocumentVersion"),
                ("GET","GetRegisterVersion"),
                ("GET","GetVersionInfo"),
                ("GET","OData"),
                ("DELETE","ODataDelete"),
                ("GET","ReturnAttachFileDocument/{FileId}"),
                ("GET","ReturnDocument"),
                ("POST","SetNewVersion"),
                ("POST","UploadAttachFile/{FileId}"),
                ("GET","ValidateAttachFileWithBlockchain/{fileid}"),
                ("GET","ValidateWithBlockchain/{id}")
            };

            // OK:変更可能な値を更新後確認
            foreach (var apiArgs in transparentApiUrlList)
            {
                // default値確認
                var methodResult = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, apiArgs.Item2, apiArgs.Item1, true, _repositoryGroupId, "cdb"))).Assert(RegisterSuccessExpectStatusCode).Result;

                var method = client.GetWebApiResponseResult(api.GetApiMethod(methodResult.MethodId)).Assert(GetExpectStatusCodes).Result;
                var resultData = method.ToJson();
                var expectedDatas = new (string, string)[]
                {
                    ("QueryTypeCd","cdb"),
                    ("ScriptTypeCd",""),
                    ("IsEnable","True"),
                    ("IsHeaderAuthentication","True"),
                    ("IsOpenIdAuthentication","False"),
                    ("IsAdminAuthentication","False"),
                    ("IsOverPartition","False"),
                    ("IsHidden","False"),
                    ("IsCache","False"),
                    ("Automatic","False"),
                    ("IsTransparent","True"),
                    ("IsVendorSystemAuthenticationAllowNull","False")
                };

                foreach (var expectedData in expectedDatas)
                {
                    Assert.AreEqual(expectedData.Item2.ToLower(), resultData[expectedData.Item1].ToString().ToLower());
                }

                // 値を更新後確認
                methodResult = client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, apiArgs.Item2, apiArgs.Item1, true, _repositoryGroupId, "cdb", null, false, false, true, true, true, true, true, true, true))).Assert(RegisterSuccessExpectStatusCode).Result;
                method = client.GetWebApiResponseResult(api.GetApiMethod(methodResult.MethodId)).Assert(GetSuccessExpectStatusCode).Result;
                resultData = method.ToJson();

                var expectedUpdatedDatas = new (string, string)[]
                {
                    ("QueryTypeCd","cdb"),
                    ("ScriptTypeCd",""),
                    ("IsEnable","False"),
                    ("IsHeaderAuthentication","False"),
                    ("IsOpenIdAuthentication","True"),
                    ("IsAdminAuthentication","True"),
                    ("IsOverPartition","True"),
                    ("IsHidden","True"),
                    ("IsCache","True"),
                    ("Automatic","True"),
                    ("IsTransparent","True"),
                    ("IsVendorSystemAuthenticationAllowNull","True")
                };
                foreach (var expectedUpdatedData in expectedUpdatedDatas)
                {
                    Assert.AreEqual(expectedUpdatedData.Item2.ToLower(), resultData[expectedUpdatedData.Item1].ToString().ToLower());
                }
            }
            // NG:DynamicApiに定義されてない透過APIの場合
            client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, "CompleteRegisterVersion", "GET", true, _repositoryGroupId, "cdb"))).Assert(RegisterErrorExpectStatusCode);
            client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, "CompleteRegisterVersionVersion", "POST", true, _repositoryGroupId, "cdb"))).Assert(RegisterErrorExpectStatusCode);
            // NG:DynamicApiに定義されている透過APIのisTransparentApiがFalseの場合
            client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, "CompleteRegisterVersionVersion", "POST", true, _repositoryGroupId, "cdb"))).Assert(RegisterErrorExpectStatusCode);
            // NG:変更負荷な値が更新された場合（QueryTypeCd、ScriptTypeCd）
            client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, "CompleteRegisterVersionVersion", "POST", true, _repositoryGroupId, "cdb"))).Assert(RegisterErrorExpectStatusCode);
            client.GetWebApiResponseResult(api.RegisterMethod(MethodDataForTransparentApiTest(apiResult1.ApiId, "CompleteRegisterVersionVersion", "POST", true, _repositoryGroupId, "cdb"))).Assert(RegisterErrorExpectStatusCode);
            CleanUpApiByUrl(client, apiUrl, true);
        }

        #region GetControllerResourceList

        /// <summary>
        /// 運用ベンダーによる取得テスト
        /// </summary>
        /// <remarks>
        /// APIの作成によって期待値は変動するため結果の検証は割愛
        /// </remarks>
        [TestMethod]
        public void ManageDynamicApiTest_GetApiResourceMethodList_運用ベンダー()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IDynamicApiApi>();

            var vendorResult = client.GetWebApiResponseResult(api.GetApiResourceMethodList(false)).Assert(GetSuccessExpectStatusCode).Result;
            var vendorCount = vendorResult.Count();
            var allResult = client.GetWebApiResponseResult(api.GetApiResourceMethodList(true)).Assert(GetSuccessExpectStatusCode).Result;
            var allCount = allResult.Count();
            // 包含関係のみ確認
            (vendorCount <= allCount).IsTrue();
            foreach (var vendorResultItem in vendorResult)
            {
                var allResultItem = allResult.Single(x => x.ApiId == vendorResultItem.ApiId);
                allResultItem.IsNotNull();
            }
        }


        #endregion

        #region リクエストモデルデータ

        #region オプション項目データ
        public RegisterApiRequestModel Data1()
        {
            var name = "/API/IntegratedTest/ManageDynamicApi02";
            return CreateRequestModel(
                url: name,
                apiName: name,
                isOptimisticConcurrency: true,
                isEnableBlockchain: true,
                attachFileSettings:
                new DynamicApiAttachFileSettingsModel
                {
                    IsEnable = true,
                    MetaRepositoryId = _attachFileMetaRepositoryGroupId,
                    BlobRepositoryId = _attachFileBlobRepositoryGroupId
                },
                documentHistorySettings:
                new DocumentHistorySettingsModel
                {
                    IsEnable = true,
                    HistoryRepositoryId = _documentHistoryRepositoryGroupId
                },
                agreeDescription: "testDescription",
                isVisibleAgreement: true,
                isEnableResourceVersion: false);
        }
        #endregion

        #region 運用管理ベンダーデータ
        public RegisterApiRequestModel Data2(bool isVendor = false,string vendorId = null,string systemId = null)
        {
            var name = "/API/Individual/IntegratedTest/ManageDynamicApi03";
            return CreateRequestModel(
                vendorId : vendorId,
                systemId : systemId,
                url: name,
                apiName: name,
                isVendor: isVendor
            );
        }

        public RegisterApiRequestModel Data3(bool isVendor = false, bool isPerson = false)
        {
            var name = "/API/IntegratedTest/ManageDynamicApi04";
            return CreateRequestModel(
                url: name,
                apiName: name,
                isVendor: isVendor,
                isPerson: isPerson
            );
        }

        #endregion

        #region バリデーショテスト用データ

        public RegisterApiRequestModel ValidationErrorData_AttachFileSettings(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                modelId: schemaId,
                repositoryKey: apiUrl,
                partitionKey: apiUrl,
                // 以下テスト対象
                attachFileSettings: new DynamicApiAttachFileSettingsModel
                {
                    IsEnable = true,
                    BlobRepositoryId = _attachFileBlobRepositoryGroupId
                }
                );
        }

        public RegisterApiRequestModel ValidationErrorData_Agreement(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                modelId: schemaId,
                repositoryKey: apiUrl,
                partitionKey: apiUrl,
                // 以下テスト対象
                agreeDescription: "",
                isVisibleAgreement: true
            );
        }

        public RegisterApiRequestModel ValidationErrorData_DocumentHistorySettings_HistoryRepositoryId_null(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                modelId: schemaId,
                repositoryKey: apiUrl,
                // 以下テスト対象
                partitionKey: apiUrl,
                documentHistorySettings: new DocumentHistorySettingsModel()
                {
                    IsEnable = true
                }
            );
        }

        public RegisterApiRequestModel ValidationErrorData_DocumentHistorySettings_HistoryRepositoryId_NotFound(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                modelId: schemaId,
                repositoryKey: apiUrl,
                // 以下テスト対象
                documentHistorySettings: new DocumentHistorySettingsModel()
                {
                    IsEnable = true,
                    HistoryRepositoryId = Guid.NewGuid().ToString()
                }
            );
        }

        static public RegisterApiRequestModel ValidationErrorData_Default(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                modelId: schemaId,
                repositoryKey: apiUrl
            );
        }
        #endregion

        #region CRUDテスト用データ

        static public RegisterApiRequestModel ApiDataForCrudTest(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                modelId: schemaId,
                repositoryKey: apiUrl,
                partitionKey: apiUrl
                );
        }

        static public RegisterMethodRequestModel MethodDataForCrudTest(string apiId, string schemaId, string repositoryGroupId, List<string> secondaryRepositoryGroupIds = null)
        {
            return CreateMethodRequestModel(
                apiId: apiId,
                url: "GetAll",
                httpMethodTypeCd: "GET",
                actionTypeCd: "quy",
                methodDescriptiveText: "GetAll",
                responseModelId: schemaId,
                repositoryGroupId: repositoryGroupId,
                secondaryRepositoryGroupIds: secondaryRepositoryGroupIds
                );
        }

        public RegisterMethodRequestModel MethodDataAllPropertiesTest(string apiId, string schemaId, string repositoryGroupId, List<string> secondaryRepositoryGroupIds = null, string languageId = null, string openIdCaApplicationId = null, bool isOtherResourceSqlAccess = false)
        {
            var registerSampleCodeModels = new List<RegisterSampleCodeModel>();
            if (languageId != null)
            {
                registerSampleCodeModels.Add(
                    new RegisterSampleCodeModel
                    {
                        LanguageId = languageId,
                        Code = "Code",
                        IsActive = true
                    }
                    );
            }
            var registerApiLinkModels = new List<RegisterApiLinkModel>
                {
                    new RegisterApiLinkModel
                    {
                        LinkTitle = "LinkTitle",
                        LinkDetail = "LinkDetail",
                        LinkUrl = "LinkUrl"
                    }
                };
            var registerAccessVendorModels = new List<RegisterAccessVendorModel>
                {
                    new RegisterAccessVendorModel
                    {
                        VendorId = this._vendorId,
                        SystemId = this._systemId,
                        AccessKey = Guid.NewGuid().ToString(),
                        IsEnable = true,
                    }
                };
            var registerResourceOpenIdCaModels = new List<RegisterResourceOpenIdCaModel>();
            if (openIdCaApplicationId != null)
            {
                registerResourceOpenIdCaModels.Add(
                    new RegisterResourceOpenIdCaModel
                    {
                        ApplicationId = openIdCaApplicationId,
                        IsActive = true,
                        AccessControl = "alw"
                    }
                );
            }

            return new RegisterMethodRequestModel
            {
                ApiId = apiId,
                Url = "GetAll",
                ActionTypeCd = "quy",
                HttpMethodTypeCd = "GET",
                MethodDescriptiveText = "MethodDescriptiveText",
                IsPostDataTypeArray = true,
                IsAutomaticId = false,
                UrlModelId = schemaId,
                RequestModelId = schemaId,
                ResponseModelId = schemaId,
                IsEnable = true,
                IsHidden = true,
                IsVisibleSigninuserOnly = false,
                IsHeaderAuthentication = true,
                IsVendorSystemAuthenticationAllowNull = false,
                IsOpenIdAuthentication = false,
                IsAdminAuthentication = false,
                IsSkipJsonSchemaValidation = false,
                IsAccessKey = true,
                ApiAccessVendorList = registerAccessVendorModels,
                IsInternalOnly = true,
                InternalOnlyKeyword = "InternalOnlyKeyword",
                IsOverPartition = true,
                IsCache = true,
                CacheMinute = "10",
                CacheKey = "CacheKey",
                GatewayUrl = "GatewayUrl",
                GatewayCredentialUserName = "GatewayCredentialUserName",
                GatewayCredentialPassword = "GatewayCredentialPassword",
                GatewayRelayHeader = "GatewayRelayHeader",
                RepositoryGroupId = repositoryGroupId,
                SecondaryRepositoryGroupIds = secondaryRepositoryGroupIds,
                QueryType = "cdb",
                Query = "Query",
                ScriptType = "rss",
                Script = @"
var api = new ApiHelper();
var result = api.ExecutePostApi(""/API/IntegratedTest/ManageDynamicApi/RegisterInternalMethod/Register"", ""{'TestId': 'testId','TestName': 'testName'}"", null, ""internal"");

if (!result.IsSuccessStatusCode) 
{ 
    return HttpResponseHelper.Create(JsonConvert.SerializeObject(new {
        Message = $""Failed StatusCode:{result.StatusCode} Msg:{result.Content.ReadAsStringAsync().Result}""
    }), result.StatusCode, ""application/json""); 
 }

return HttpResponseHelper.Create(JsonConvert.SerializeObject(new {
    Result= JsonHelper.ToJson(result.Content.ReadAsStringAsync().Result)
}), HttpStatusCode.Created, ""application/json""); 
",
                SampleCodeList = registerSampleCodeModels,
                ApiLinkList = registerApiLinkModels,
                OpenIdCaList = registerResourceOpenIdCaModels,
            };
        }


        public RegisterMethodRequestModel CreateMethodModelForRegisterInternalMethodTest(string apiId, string schemaId, string repositoryGroupId, List<string> secondaryRepositoryGroupIds = null)
        {
            return CreateMethodRequestModel(
                apiId: apiId,
                url: "Register",
                httpMethodTypeCd: "POST",
                actionTypeCd: "reg",
                methodDescriptiveText: "Register",
                responseModelId: schemaId,
                repositoryGroupId: repositoryGroupId,
                secondaryRepositoryGroupIds: secondaryRepositoryGroupIds,
                isInternalOnly: true,
                internalOnlyKeyword: "internal"
            );
        }

        public RegisterMethodRequestModel CreateMethodModelForRegisterMethodInternalCallTest(string apiId, string schemaId, string repositoryGroupId, List<string> secondaryRepositoryGroupIds = null)
        {
            return CreateMethodRequestModel(
                apiId: apiId,
                url: "RegisterScript",
                httpMethodTypeCd: "POST",
                actionTypeCd: "reg",
                methodDescriptiveText: "RegisterScript",
                repositoryGroupId: repositoryGroupId,
                secondaryRepositoryGroupIds: secondaryRepositoryGroupIds,
                scriptType: "rss",
                script: @"
var api = new ApiHelper();
var result = api.ExecutePostApi(""/API/IntegratedTest/ManageDynamicApi/RegisterInternalMethod/Register"", ""{'TestId': 'testId','TestName': 'testName'}"", null, ""internal"");

if (!result.IsSuccessStatusCode) 
{ 
    return HttpResponseHelper.Create(JsonConvert.SerializeObject(new {
        Message = $""Failed StatusCode:{result.StatusCode} Msg:{result.Content.ReadAsStringAsync().Result}""
    }), result.StatusCode, ""application/json""); 
 }

return HttpResponseHelper.Create(JsonConvert.SerializeObject(new {
    Result= JsonHelper.ToJson(result.Content.ReadAsStringAsync().Result)
}), HttpStatusCode.Created, ""application/json""); 
"
            );
        }


        public RegisterSchemaRequestModel SchemaDataForCrudTest(string schemaName, string vendorId = null)
        {
            return CreateSchemaaRequestModel(
                schemaName: schemaName,
                jsonSchema: "{\r\n  'description':'IntegratedTestRegisterSchema',\r\n  'properties': {\r\n    'TestId': {\r\n      'title': 'テストID',\r\n      'type': 'string',\r\n      'required':true\r\n    },\r\n    'TestName': {\r\n      'title': 'テスト名',\r\n      'type': 'string',\r\n      'required':true\r\n    }\r\n    \r\n  },\r\n  'type': 'object',\r\n  'additionalProperties' : false\r\n}",
                vendorId: vendorId
                );
        }

        public MethodModel GetExpectedMethodDataForAllPropertiesTest(string methodId,
            RegisterMethodRequestModel registerMethod,
            MethodModel methodGetResult)
        {
            return new MethodModel
            {
                MethodId = methodId,
                MethodDescription = registerMethod.MethodDescriptiveText,
                ApiId = registerMethod.ApiId,
                MethodType = registerMethod.HttpMethodTypeCd,
                MethodUrl = registerMethod.Url,
                RepositoryKey = methodGetResult.RepositoryKey, // register対象外
                RequestSchemaId = registerMethod.RequestModelId,
                RequestSchemaName = methodGetResult.RequestSchemaName, // register対象外
                ResponseSchemaId = registerMethod.ResponseModelId,
                ResponseSchemaName = methodGetResult.ResponseSchemaName, //register対象外
                UrlSchemaId = registerMethod.UrlModelId,
                UrlSchemaName = methodGetResult.UrlSchemaName, //register対象外
                PostDataType = registerMethod.IsPostDataTypeArray ? "array" : "",
                Query = registerMethod.Query,
                RepositoryGroupId = registerMethod.RepositoryGroupId,
                RepositoryGroupName = methodGetResult.RepositoryGroupName, //register対象外
                IsEnable = registerMethod.IsEnable,
                IsHeaderAuthentication = registerMethod.IsHeaderAuthentication,
                IsOpenIdAuthentication = registerMethod.IsOpenIdAuthentication,
                IsAdminAuthentication = registerMethod.IsAdminAuthentication,
                IsOverPartition = registerMethod.IsOverPartition,
                GatewayUrl = registerMethod.GatewayUrl,
                GatewayCredentialUserName = registerMethod.GatewayCredentialUserName,
                IsHidden = registerMethod.IsHidden,
                Script = registerMethod.Script,
                ActionTypeCd = registerMethod.ActionTypeCd,
                ScriptTypeCd = registerMethod.ScriptType,
                IsCache = registerMethod.IsCache,
                CacheMinute = int.Parse(registerMethod.CacheMinute),
                CacheKey = registerMethod.CacheKey,
                AccessKey = registerMethod.IsAccessKey.ToString(),
                Automatic = registerMethod.IsAutomaticId.ToString(),
                ActionTypeVersion = methodGetResult.ActionTypeVersion, //register対象外
                PartitionKey = methodGetResult.PartitionKey, //register対象外
                GatewayRelayHeader = registerMethod.GatewayRelayHeader,
                RegUserName = methodGetResult.RegUserName, //register対象外
                UpdUserName = methodGetResult.UpdUserName, //register対象外
                RegDate = methodGetResult.RegDate, //register対象外
                UpdDate = methodGetResult.UpdDate, //register対象外
                IsActive = true, //register対象外 true固定
                IsTransparent = false, //register対象外 false固定
                IsVendorSystemAuthenticationAllowNull = registerMethod.IsVendorSystemAuthenticationAllowNull,
                IsVisibleSigninUserOnly = registerMethod.IsVisibleSigninuserOnly,
                QueryTypeCd = registerMethod.QueryType,
                IsInternalOnly = registerMethod.IsInternalOnly,
                InternalOnlyKeyword = registerMethod.InternalOnlyKeyword,
                IsSkipJsonSchemaValidation = registerMethod.IsSkipJsonSchemaValidation,
                SecondaryRepositoryMapList = registerMethod.SecondaryRepositoryGroupIds.Select(x =>
                    new SecondaryRepositoryModel
                    {
                        SecondaryRepositoryMapId = methodGetResult.SecondaryRepositoryMapList
                            .First(y => y.RepositoryGroupId.ToString().ToLower() == x.ToLower())
                            .SecondaryRepositoryMapId,
                        RepositoryGroupId = x,
                        RepositoryGroupName = methodGetResult.SecondaryRepositoryMapList
                            .First(y => y.RepositoryGroupId.ToString().ToLower() == x.ToLower())
                            .RepositoryGroupName, // register対象外
                        IsPrimary = methodGetResult.SecondaryRepositoryMapList
                            .First(y => y.RepositoryGroupId.ToString().ToLower() == x.ToLower())
                            .IsPrimary // register対象外
                    }
                ).ToList(),
                SampleCodeList = registerMethod.SampleCodeList.Select(x =>
                    new DynamicApiSampleCodeModel
                    {
                        SampleCodeId = methodGetResult.SampleCodeList.First(y => y.LanguageId == x.LanguageId)
                            .SampleCodeId, // register対象外
                        LanguageId = x.LanguageId,
                        Language =
                            methodGetResult.SampleCodeList.First(y => y.LanguageId == x.LanguageId)
                                .Language, // register対象外
                        Code = x.Code
                    }
                ).ToList(),
                MethodLinkList = registerMethod.ApiLinkList.Select(x =>
                    new DynamicApiMethodLinkModel
                    {
                        MethodLinkId = methodGetResult.MethodLinkList.First(y => y.Title.ToString() == x.LinkTitle)
                            .MethodLinkId, // register対象外
                        Title = x.LinkTitle,
                        Url = x.LinkUrl,
                        Detail = x.LinkDetail,
                        IsVisible = true //register対象外 true固定
                    }
                ).ToList(),
                AccessVendorList = registerMethod.ApiAccessVendorList.Select(x =>
                    new AccessVendorModel
                    {
                        AccessVendorId = methodGetResult.AccessVendorList.First()
                            .AccessVendorId, // register対象外
                        VendorId = x.VendorId,
                        SystemId = x.SystemId,
                        IsEnable = x.IsEnable,
                        AccessKey = x.AccessKey,
                        VendorName = methodGetResult.AccessVendorList.First(y => y.VendorId.ToLower() == x.VendorId.ToLower()).VendorName,
                        SystemName = methodGetResult.AccessVendorList.First(y => y.SystemId.ToLower() == x.SystemId.ToLower()).SystemName,
                    }
                ).ToList(),
                MethodOpenIdCAList = registerMethod.OpenIdCaList.Select(x =>
                    new MethodOpenIdCAModel
                    {
                        MethodOpenIdCaId = methodGetResult.MethodOpenIdCAList
                            .First(y => y.ApplicationId == x.ApplicationId).MethodOpenIdCaId, // register対象外
                        ApplicationId = x.ApplicationId,
                        ApplicationName = methodGetResult.MethodOpenIdCAList
                            .First(y => y.ApplicationId == x.ApplicationId).ApplicationName, // register対象外
                        AccessControl = x.AccessControl
                    }
                ).Concat(
                    methodGetResult.MethodOpenIdCAList.Where(x => !registerMethod.OpenIdCaList.Any(y => y.ApplicationId == x.ApplicationId) && x.AccessControl == "inh")  // registerしていない認証局はアクセスコントロール未設定で取得される
                ).ToList()
            };
        }

        #endregion

        #region 重複チェックテストデータ

        public RegisterApiRequestModel ApiDataForDuplicateTest(string vendorId, string systemId, string apiUrl)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                repositoryKey: apiUrl,
                partitionKey: apiUrl
                );
        }

        public RegisterMethodRequestModel MethodDataForDuplicateTest(string actionType, string apiId, string methodUrl, string repositoryGroupId)
        {
            switch (actionType)
            {
                case "quy":
                    return CreateMethodRequestModel(
                                apiId: apiId,
                                url: methodUrl,
                                httpMethodTypeCd: "GET",
                                actionTypeCd: "quy",
                                repositoryGroupId: repositoryGroupId);
                case "reg":
                    return CreateMethodRequestModel(
                                apiId: apiId,
                                url: methodUrl,
                                httpMethodTypeCd: "POST",
                                actionTypeCd: "reg",
                                repositoryGroupId: repositoryGroupId);
                case "gtw":
                    return CreateMethodRequestModel(
                                apiId: apiId,
                                url: methodUrl,
                                httpMethodTypeCd: "GET",
                                actionTypeCd: "gtw",
                                gatewayUrl: "http://localhost/Test");
                default:
                    throw new ArgumentException($"ActionType {actionType} is not expected.");
            }
        }

        #endregion

        #region クエリパターンテスト用データ

        public RegisterApiRequestModel ApiDataForQueryPatternTest(string vendorId, string systemId, string apiUrl)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                repositoryKey: apiUrl,
                partitionKey: apiUrl
                );
        }

        public RegisterMethodRequestModel MethodDataForQueryPatternTest(string apiId, string repositoryGroupId, string query)
        {
            return CreateMethodRequestModel(
                apiId: apiId,
                url: "QueryPatternTest",
                httpMethodTypeCd: "GET",
                actionTypeCd: "quy",
                repositoryGroupId: repositoryGroupId,
                query: query
                );
        }

        #endregion


        #region 透過APIの更新テスト用データ

        public RegisterApiRequestModel ApiDataForTransparentApiTest(string vendorId, string systemId, string apiUrl)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                repositoryKey: apiUrl,
                partitionKey: apiUrl
                );
        }

        public RegisterMethodRequestModel MethodDataForTransparentApiTest(
            string apiId,
            string methodUrl,
            string httpMethodTypeCd,
            bool isTransparentApi,
            string repositoryGroupId,
            string queryType = null,
            string scriptType = null,
            bool isEnable = true,
            bool isHeaderAuthentication = true,
            bool isOpenIdAuthentication = false,
            bool isAdminAuthentication = false,
            bool isOverPartition = false,
            bool isHidden = false,
            bool isCache = false,
            bool isAutomaticId = false,
            bool isVendorSystemAuthenticationAllowNull = false)
        {
            return CreateMethodRequestModel(
                        apiId: apiId,
                        url: methodUrl,
                        httpMethodTypeCd: httpMethodTypeCd,
                        isTransparentApi: isTransparentApi,
                        actionTypeCd: httpMethodTypeCd == "GET" ? "quy" : "reg",
                        queryType: queryType,
                        scriptType: scriptType,
                        isEnable: isEnable,
                        isHeaderAuthentication: isHeaderAuthentication,
                        isOpenIdAuthentication: isOpenIdAuthentication,
                        isAdminAuthentication: isAdminAuthentication,
                        isOverPartition: isOverPartition,
                        isHidden: isHidden,
                        isCache: isCache,
                        isAutomaticId: isAutomaticId,
                        isVendorSystemAuthenticationAllowNull: isVendorSystemAuthenticationAllowNull,
                        repositoryGroupId: repositoryGroupId);
        }

        #endregion

        #region RoslynScript実行テスト用データ

        public RegisterApiRequestModel RoslynScriptExcuteTest(string vendorId, string systemId, string apiUrl)
        {
            return CreateRequestModel(
                vendorId: vendorId,
                systemId: systemId,
                apiName: apiUrl,
                url: apiUrl,
                repositoryKey: apiUrl,
                partitionKey: apiUrl
                );
        }

        public RegisterMethodRequestModel MethodDataForRoslynScriptExcuteTest(
            string apiId,
            string methodUrl,
            string httpMethodTypeCd,
            string repositoryGroupId,
            string scriptType = null,
            string script = null)
        {
            return CreateMethodRequestModel(
                        apiId: apiId,
                        url: methodUrl,
                        httpMethodTypeCd: httpMethodTypeCd,
                        actionTypeCd: httpMethodTypeCd == "GET" ? "quy" : "reg",
                        scriptType: scriptType,
                        script: script,
                        repositoryGroupId: repositoryGroupId);
        }

        #endregion

        #endregion

        #region Expect
        #region 添付ファイルデータ

        public List<MethodModel> AttachFileExpect()
        {
            return new List<MethodModel>
                {
                    new MethodModel
                    {
                        MethodType = "POST",
                        MethodUrl = "CreateAttachFile",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel> { }
                    },
                    new MethodModel
                    {
                        MethodType = "DELETE",
                        MethodUrl = "DeleteAttachFile/{FileId}",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel>
                        {
                            new SecondaryRepositoryModel
                            {
                                IsPrimary = false,
                                RepositoryGroupId = _attachFileBlobRepositoryGroupId,
                            }
                        }
                    },
                    new MethodModel
                    {
                        MethodType = "GET",
                        MethodUrl = "GetAttachFile/{FileId}",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel>
                        {
                            new SecondaryRepositoryModel
                            {
                                IsPrimary = false,
                                RepositoryGroupId = _attachFileBlobRepositoryGroupId,
                            }
                        }
                    },
                    new MethodModel
                    {
                        MethodType = "GET",
                        MethodUrl = "GetAttachFileMeta/{FileId}",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel> { }
                    },
                    new MethodModel
                    {
                        MethodType = "GET",
                        MethodUrl = "GetAttachFileMetaList",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel> { }
                    },
                    new MethodModel
                    {
                        MethodType = "POST",
                        MethodUrl = "UploadAttachFile/{FileId}",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel>
                        {
                            new SecondaryRepositoryModel
                            {
                                IsPrimary = false,
                                RepositoryGroupId = _attachFileBlobRepositoryGroupId,
                            }
                        }
                    }
                };
        }

        #endregion

        #region ブロックチェーンデータ
        public List<MethodModel> BlockchainExpect()
        {
            return new List<MethodModel>
                {
                    new MethodModel
                    {
                        MethodType = "GET",
                        MethodUrl = "ValidateAttachFileWithBlockchain/{fileid}",
                        RepositoryGroupId = _attachFileMetaRepositoryGroupId,
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel>
                        {
                            new SecondaryRepositoryModel
                            {
                                IsPrimary = false,
                                RepositoryGroupId = _attachFileBlobRepositoryGroupId,
                            }
                        }
                    },
                    new MethodModel
                    {
                        MethodType = "GET",
                        MethodUrl = "ValidateWithBlockchain/{id}",
                        SecondaryRepositoryMapList = new List<SecondaryRepositoryModel> { }
                    }

                };
        }

        #endregion

        #endregion


        static public RegisterMethodRequestModel CreateMethodRequestModel(string apiId = null, string url = null, string httpMethodTypeCd = null, string actionTypeCd = null, string methodDescriptiveText = null, string query = null, string requestModelId = null, string responseModelId = null, string urlModelId = null, bool isPostDataTypeArray = false, string gatewayUrl = null, string gatewayCredentialUserName = null, string gatewayCredentialPassword = null, string gatewayRelayHeader = null, bool isHeaderAuthentication = true, bool isVendorSystemAuthenticationAllowNull = false, bool isOpenIdAuthentication = false, bool isAdminAuthentication = false, bool isInternalOnly = false, string internalOnlyKeyword = null, bool isOverPartition = false, string repositoryGroupId = null, bool isEnable = true, bool isAutomaticId = false, bool isHidden = false, bool isCache = false, string cacheMinute = null, string cacheKey = null, List<string> secondaryRepositoryGroupIds = null, List<RegisterApiLinkModel> apiLinkList = null, string script = null, string scriptType = null, string queryType = "cdb", bool isTransparentApi = false, bool isOtherResourceSqlAccess = false)
        {
            return new RegisterMethodRequestModel
            {
                ApiId = apiId,
                Url = url,
                HttpMethodTypeCd = httpMethodTypeCd,
                ActionTypeCd = actionTypeCd,
                MethodDescriptiveText = methodDescriptiveText,
                Query = query,
                RequestModelId = requestModelId,
                ResponseModelId = responseModelId,
                UrlModelId = urlModelId,
                IsPostDataTypeArray = isPostDataTypeArray,
                GatewayUrl = gatewayUrl,
                GatewayCredentialUserName = gatewayCredentialUserName,
                GatewayCredentialPassword = gatewayCredentialPassword,
                GatewayRelayHeader = gatewayRelayHeader,
                IsHeaderAuthentication = isHeaderAuthentication,
                IsVendorSystemAuthenticationAllowNull = isVendorSystemAuthenticationAllowNull,
                IsOpenIdAuthentication = isOpenIdAuthentication,
                IsAdminAuthentication = isAdminAuthentication,
                IsInternalOnly = isInternalOnly,
                InternalOnlyKeyword = internalOnlyKeyword,
                IsOverPartition = isOverPartition,
                RepositoryGroupId = repositoryGroupId,
                IsEnable = isEnable,
                IsAutomaticId = isAutomaticId,
                IsHidden = isHidden,
                IsCache = isCache,
                CacheMinute = cacheMinute,
                CacheKey = cacheKey,
                SecondaryRepositoryGroupIds = secondaryRepositoryGroupIds,
                ApiLinkList = apiLinkList,
                Script = script,
                ScriptType = scriptType,
                QueryType = queryType,
                IsTransparentApi = isTransparentApi,
                IsOtherResourceSqlAccess = isOtherResourceSqlAccess
            };
        }
        public RegisterSchemaRequestModel CreateSchemaaRequestModel(string schemaName = null, string jsonSchema = null, string description = null, string vendorId = null)
        {
            return new RegisterSchemaRequestModel
            {
                SchemaName = schemaName,
                JsonSchema = jsonSchema,
                Description = description,
                VendorId = vendorId
            };
        }
    }

}
