using IT.JP.DataHub.ManageApi.Config;
using IT.JP.DataHub.ManageApi.WebApi;
using IT.JP.DataHub.ManageApi.WebApi.Models;
using IT.JP.DataHub.ManageApi.WebApi.Models.DynamicApi;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IT.JP.DataHub.ManageApi.TestCase
{
    [TestClass]
    public partial class RepositoryGroupTest : ManageApiTestCase
    {

        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// リポジトリグループ-一覧の正常系テスト
        /// </summary>
        [TestMethod]
        public void RepositoryGroup_NormalSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRepositoryGroupApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetRepositoryGroupList()).Assert(GetExpectStatusCodes).Result;

            var listDelete = list.Where(x => RepositoryGroupNameRegData.Contains(x.RepositoryGroupName) == true || RepositoryGroupNameUpdData.Contains(x.RepositoryGroupName) == true)
                .Select(x => x.RepositoryGroupId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteRepositoryGroup(x)).Assert(DeleteExpectStatusCodes));


            // 一覧取得ができること
            client.GetWebApiResponseResult(api.GetRepositoryGroupList()).Assert(GetSuccessExpectStatusCode);

            // 一覧取得ができること(type)
            var typeList = client.GetWebApiResponseResult(api.GetRepositoryGroupTypeList()).Assert(GetSuccessExpectStatusCode).Result;

            // 【準備】タイプの一覧から１番目の要素を抜き出す。
            var firstRepositoryTypeCd = typeList.First().RepositoryTypeCd;

            // 新規登録
            var regObj = RepositoryGroupRegData;
            regObj.RepositoryTypeCd = firstRepositoryTypeCd;
            var regId = client.GetWebApiResponseResult(api.RegisterRepositoryGroup(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetRepositoryGroup(regId)).Assert(GetSuccessExpectStatusCode).Result;

            // 更新
            var updObj = RepositoryGroupUpdData;
            updObj.RepositoryGroupId = regId;
            updObj.RepositoryTypeCd = firstRepositoryTypeCd;
            updObj.PhysicalRepositoryList[0].PhysicalRepositoryId = getRegData.PhysicalRepositoryList.First().PhysicalRepositoryId;

            var updData = client.GetWebApiResponseResult(api.RegisterRepositoryGroup(updObj)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;

            // 更新したものを取得
            var getUpdData = client.GetWebApiResponseResult(api.GetRepositoryGroup(regId)).Assert(GetSuccessExpectStatusCode).Result;

            //1件更新、1件新規登録
            getUpdData.PhysicalRepositoryList.Count().Is(2);
            //PhysicalRepository内容確認
            var expected = RepositoryGroupUpdData;
            var actual = getUpdData;

            var actual1 = actual.PhysicalRepositoryList.First(p => p.ConnectionString == expected.PhysicalRepositoryList[0].ConnectionString);
            (actual1.IsFull == expected.PhysicalRepositoryList[0].IsFull).Is(true);

            var actual2 = actual.PhysicalRepositoryList.First(p => p.ConnectionString == expected.PhysicalRepositoryList[1].ConnectionString);
            (actual2.IsFull == expected.PhysicalRepositoryList[1].IsFull).Is(true);

            // PhysicalRepository削除
            var updObj2 = RepositoryGroupUpdData;
            updObj.RepositoryGroupId = regId;
            updObj.RepositoryTypeCd = firstRepositoryTypeCd;
            updObj.PhysicalRepositoryList[0].PhysicalRepositoryId = getUpdData.PhysicalRepositoryList[0].PhysicalRepositoryId;
            updObj.PhysicalRepositoryList[0].IsActive = false;
            updObj.PhysicalRepositoryList[1].PhysicalRepositoryId = getUpdData.PhysicalRepositoryList[1].PhysicalRepositoryId;
            updObj.PhysicalRepositoryList[1].IsActive = false;

            var updData2 = client.GetWebApiResponseResult(api.RegisterRepositoryGroup(updObj)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;

            // 更新したものを取得
            var getUpdData2 = client.GetWebApiResponseResult(api.GetRepositoryGroup(regId)).Assert(GetSuccessExpectStatusCode).Result;
            getUpdData2.PhysicalRepositoryList.Count().Is(0);

            // 削除
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(regId)).Assert(DeleteExpectStatusCodes);

            // 新規登録したものを更新登録したもののIDが同じか
            getRegData.RepositoryGroupId.ToString().IsStructuralEqual(getUpdData.RepositoryGroupId.ToString());
        }

        /// <summary>
        /// リポジトリグループ-一覧のエラー系テスト
        /// </summary>
        [TestMethod]
        public void RepositoryGroup_ErrorSenario()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRepositoryGroupApi>();

            //【準備】テストで使用するデータが存在しないことを確認し、存在するなら消す。
            var list = client.GetWebApiResponseResult(api.GetRepositoryGroupList()).Assert(GetExpectStatusCodes).Result;

            var listDelete = list.Where(x => RepositoryGroupNameRegData.Contains(x.RepositoryGroupName) == true || RepositoryGroupNameUpdData.Contains(x.RepositoryGroupName) == true)
                .Select(x => x.RepositoryGroupId).ToList();
            listDelete.ForEach(x => client.GetWebApiResponseResult(api.DeleteRepositoryGroup(x)).Assert(DeleteExpectStatusCodes));

            // 一覧取得ができること
            client.GetWebApiResponseResult(api.GetRepositoryGroupList()).Assert(GetSuccessExpectStatusCode);

            // 一覧取得ができること(type)
            var typeList = client.GetWebApiResponseResult(api.GetRepositoryGroupTypeList()).Assert(GetSuccessExpectStatusCode).Result;

            // 【準備】タイプの一覧から１番目の要素を抜き出す。
            var firstRepositoryTypeCd = typeList.First().RepositoryTypeCd;

            // 新規登録
            var regObj = RepositoryGroupRegData;
            regObj.RepositoryTypeCd = firstRepositoryTypeCd;
            var regId = client.GetWebApiResponseResult(api.RegisterRepositoryGroup(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;


            //----テスト開始
            // GetRepositoryGroupのValidationError
            GetRepositoryGroupValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.GetRepositoryGroup(x)).Assert(BadRequestStatusCode)
            );

            // GetRepositoryGroupのNotFound
            client.GetWebApiResponseResult(api.GetRepositoryGroup(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // RegisterRepositoryGroupのValidationError
            RegisterRepositoryGroupValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterRepositoryGroup(x)).Assert(BadRequestStatusCode)
            );

            // RegisterRepositoryGroupのNullBody
            client.GetWebApiResponseResult(api.RegisterRepositoryGroup(null)).Assert(BadRequestStatusCode);

            // RegisterRepositoryGroupのFkError(BadRequest)
            RegisterRepositoryGroupFkErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterRepositoryGroup(x)).Assert(BadRequestStatusCode)
            );

            // UpdateRepositoryGroupのValidationError
            UpdateRepositoryGroupValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.RegisterRepositoryGroup(x)).Assert(BadRequestStatusCode)
            );

            // DeleteRepositoryGroupのValidationError
            DeleteRepositoryGroupValidationErrorData.ForEach(x =>
                client.GetWebApiResponseResult(api.DeleteRepositoryGroup(x)).Assert(BadRequestStatusCode)
            );

            // DeleteRepositoryGroupのNotFound
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(Guid.NewGuid().ToString())).Assert(NotFoundStatusCode);

            // 【後処理】削除
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(regId)).Assert(DeleteExpectStatusCodes);

        }

        /// <summary>
        /// リポジトリグループ-削除エラーテスト
        /// </summary>
        [TestMethod]
        public void RepositoryGroup_DeleteError()
        {
            var client = new DynamicApiClient(AppConfig.Account);
            var api = UnityCore.Resolve<IRepositoryGroupApi>();
            var manageDynamicApi = UnityCore.Resolve<IDynamicApiApi>();

            // 新規登録/プライマリーレポジトリ
            var regObj = RepositoryGroupRegData;
            regObj.RepositoryTypeCd = "ddb";
            var regId = client.GetWebApiResponseResult(api.RegisterRepositoryGroup(regObj)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;

            //セカンダリーレポジトリ
            var regObjSecond = RepositoryGroupRegData;
            regObjSecond.RepositoryTypeCd = "ddb";
            var regIdSecond = client.GetWebApiResponseResult(api.RegisterRepositoryGroup(regObjSecond)).Assert(RegisterSuccessExpectStatusCode).Result.RepositoryGroupId;

            // 新規登録したものを取得
            var getRegData = client.GetWebApiResponseResult(api.GetRepositoryGroup(regId)).Assert(GetSuccessExpectStatusCode).Result;
            var getRegDataSecond = client.GetWebApiResponseResult(api.GetRepositoryGroup(regIdSecond)).Assert(GetSuccessExpectStatusCode).Result;

            //使用中のリポジトリグループ削除ができないこと
            var repoGroupId = getRegData.RepositoryGroupId;
            var repoGroupIdSecond = getRegDataSecond.RepositoryGroupId;
            const string apiUrl = "/API/IntegratedTest/ManageDynamicApi/UseApiDeleteTest";

            // Initialize
            CleanUpApiByUrl(client, apiUrl, true);
            // スキーマ登録
            var schemaResult1 = client.GetWebApiResponseResult(manageDynamicApi.RegisterSchema(SchemaDataForCrudTest(apiUrl))).Assert(RegisterExpectStatusCodes).Result;

            // API登録
            var apiResult1 = client.GetWebApiResponseResult(manageDynamicApi.RegisterApi(
                ApiDataForCrudTest(AppConfig.AdminVendorId, AppConfig.AdminSystemId, apiUrl, schemaResult1.SchemaId)))
                .Assert(RegisterSuccessExpectStatusCode).Result;

            // メソッド登録
            var methodResult1 = client.GetWebApiResponseResult(manageDynamicApi.RegisterMethod(new RegisterMethodRequestModel() { 
                ApiId = apiResult1.ApiId ,ResponseModelId = schemaResult1.SchemaId,RepositoryGroupId = repoGroupId,SecondaryRepositoryGroupIds = new List<string> { repoGroupIdSecond },
                Url="GetAll",HttpMethodTypeCd="GET",ActionTypeCd = "quy",MethodDescriptiveText = "GetAll"
            })).Result;

            // 削除(指定されたリポジトリグループは他で使用されている為削除できません。)
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(regId)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(regIdSecond)).Assert(BadRequestStatusCode);

            // メソッド削除(NoContent)
            client.GetWebApiResponseResult(manageDynamicApi.DeleteMethod(methodResult1.MethodId)).Assert(DeleteSuccessStatusCode);

            //API削除
            client.GetWebApiResponseResult(manageDynamicApi.DeleteApiFromUrl(apiUrl)).Assert(DeleteSuccessStatusCode);

            // 削除
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(regId)).Assert(DeleteSuccessStatusCode);
            client.GetWebApiResponseResult(api.DeleteRepositoryGroup(regIdSecond)).Assert(DeleteSuccessStatusCode);
        }

        public RegisterSchemaRequestModel SchemaDataForCrudTest(string schemaName, string vendorId = null)
        {
            return CreateSchemaaRequestModel(
                schemaName: schemaName,
                jsonSchema: "{\r\n  'description':'IntegratedTestRegisterSchema',\r\n  'properties': {\r\n    'TestId': {\r\n      'title': 'テストID',\r\n      'type': 'string',\r\n      'required':true\r\n    },\r\n    'TestName': {\r\n      'title': 'テスト名',\r\n      'type': 'string',\r\n      'required':true\r\n    }\r\n    \r\n  },\r\n  'type': 'object',\r\n  'additionalProperties' : false\r\n}",
                vendorId: vendorId
                );
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
        public RegisterApiRequestModel ApiDataForCrudTest(string vendorId, string systemId, string apiUrl, string schemaId)
        {
            return new RegisterApiRequestModel()
            {
                VendorId = vendorId,
                SystemId = systemId,
                ApiName = apiUrl,
                Url = apiUrl,
                ModelId = schemaId,
                RepositoryKey = apiUrl,
                PartitionKey = apiUrl
            };
        }
        #region Data

        public string RepositoryGroupNameRegData = "---itRegister---";
        public string RepositoryGroupNameUpdData = "---itUpdate---";

        public string RepositoryGroupConnectionStringRegData = "---CONNECTION-STRING-REG---";
        public string RepositoryGroupConnectionStringUpdData1 = "---CONNECTION-STRING-UPD1---";
        public string RepositoryGroupConnectionStringUpdData2 = "---CONNECTION-STRING-UPD2---";
        /// <summary>
        /// リポジトリグループ-一覧正常系データ
        /// </summary>
        public RepositoryGroupModel RepositoryGroupRegData
        {
            get =>
                new RepositoryGroupModel()
                {
                        //RepositoryGroupId = Guid.Empty.ToString(), 入力不要。
                    RepositoryGroupName = RepositoryGroupNameRegData,
                    RepositoryTypeCd = null, //FK要素なので、タイプの一覧から取得する。
                    SortNo = "0",
                    IsDefault = bool.FalseString,
                    IsEnable = bool.TrueString,
                    PhysicalRepositoryList = new List<PhysicalRepositoryModel>
                    {
                            new PhysicalRepositoryModel
                            {
                                ConnectionString = RepositoryGroupConnectionStringRegData,
                                IsActive = true,
                                IsFull = true
                            }
                    }
                };
        }

        public RepositoryGroupModel RepositoryGroupUpdData
        {
            get =>
                new RepositoryGroupModel()
                {
                        //RepositoryGroupId = Guid.Empty.ToString(), 入力不要。
                    RepositoryGroupName = RepositoryGroupNameUpdData,
                    RepositoryTypeCd = null, //FK要素なので、タイプの一覧から取得する。
                    SortNo = "100",
                    IsDefault = bool.TrueString,
                    IsEnable = bool.FalseString,
                    PhysicalRepositoryList = new List<PhysicalRepositoryModel>
                    {
                            new PhysicalRepositoryModel
                            {
                                ConnectionString = RepositoryGroupConnectionStringUpdData1,
                                IsActive = true,
                                IsFull = true
                            },
                            new PhysicalRepositoryModel
                            {
                                ConnectionString = RepositoryGroupConnectionStringUpdData2,
                                IsActive = true,
                                IsFull = false
                            }
                    }
                };
        }

        #endregion

        #region Validation/Error
        public string RepositoryGroupNameErrorCaseRegData = "---itRegister-ErrorCase---";
        public RepositoryGroupModel RepositoryGroupValidationBaseModel
        {
            get =>
                new RepositoryGroupModel()
                {
                    RepositoryGroupId = Guid.NewGuid().ToString(),
                    RepositoryGroupName = RepositoryGroupNameRegData,
                    RepositoryTypeCd = "ddb", //FK要素なので、タイプの一覧から取得する。
                    SortNo = "0",
                    IsDefault = bool.FalseString,
                    IsEnable = bool.TrueString
                };
        }


        /// <summary>
        /// リポジトリグループ-一覧異常系データ(RegisterRepositoryGroup)
        /// </summary>
        public List<RepositoryGroupModel> RegisterRepositoryGroupValidationErrorData
        {
            get
            {
                // Nameがnull
                var RepositoryGroupNameNullModel = DeepCopy(RepositoryGroupValidationBaseModel);
                RepositoryGroupNameNullModel.RepositoryGroupName = null;

                // Nameが256桁を超える
                var RepositoryGroupNameMaxLengthOverModel = DeepCopy(RepositoryGroupValidationBaseModel);
                RepositoryGroupNameMaxLengthOverModel.RepositoryGroupName =
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +//100
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +//200 
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +
                    "1234567890" +//250
                    "123456_";

                // RepositoryTypeCdがnull
                var RepositoryTypeCdNullModel = DeepCopy(RepositoryGroupValidationBaseModel);
                RepositoryTypeCdNullModel.RepositoryTypeCd = null;

                // RepositoryTypeCdが3桁を超える
                var RepositoryTypeCdMaxLengthOverModel = DeepCopy(RepositoryGroupValidationBaseModel);
                RepositoryTypeCdMaxLengthOverModel.RepositoryTypeCd = "123_";

                // SortNoがnull
                var SortNoNullModel = DeepCopy(RepositoryGroupValidationBaseModel);
                SortNoNullModel.SortNo = null;

                // SortNoが非int
                var SortNoNotInt = DeepCopy(RepositoryGroupValidationBaseModel);
                SortNoNotInt.SortNo = "hoge";

                // IsDefaultが非bool
                var IsDefaultNotBool = DeepCopy(RepositoryGroupValidationBaseModel);
                IsDefaultNotBool.IsDefault = "hoge";

                // IsEnableが非bool
                var IsEnableNotBool = DeepCopy(RepositoryGroupValidationBaseModel);
                IsEnableNotBool.IsEnable = "hoge";


                return new List<RepositoryGroupModel>()
                {
                    RepositoryGroupNameNullModel,
                    RepositoryGroupNameMaxLengthOverModel,
                    RepositoryTypeCdNullModel,
                    RepositoryTypeCdMaxLengthOverModel,
                    SortNoNullModel,
                    SortNoNotInt,
                    IsDefaultNotBool,
                    IsEnableNotBool
                };
            }
        }

        /// <summary>
        /// リポジトリグループ-一覧異常系データ(RegisterRepositoryGroup)
        /// </summary>
        public List<RepositoryGroupModel> RegisterRepositoryGroupFkErrorData
        {
            get
            {
                // RepositoryTypeCdがRepositoryTypeCodeTableに存在しない
                var RepositoryGroupTypeCdFkErrorModel = DeepCopy(RepositoryGroupValidationBaseModel);
                RepositoryGroupTypeCdFkErrorModel.RepositoryTypeCd = "xxx";

                return new List<RepositoryGroupModel>()
                {
                    RepositoryGroupTypeCdFkErrorModel
                };
            }
        }

        /// <summary>
        /// リポジトリグループ-一覧異常系データ(UpdateRepositoryGroup)
        /// </summary>
        public List<RepositoryGroupModel> UpdateRepositoryGroupValidationErrorData
        {
            get
            {
                // 新規と同じ
                var baseModel = DeepCopy(RegisterRepositoryGroupValidationErrorData);

                // ConnectionStringがnull
                baseModel[0].PhysicalRepositoryList = new List<PhysicalRepositoryModel>
                    {
                        new PhysicalRepositoryModel
                        {
                            ConnectionString = string.Empty
                        }
                    };

                return baseModel;
            }
        }

        /// <summary>
        /// リポジトリグループ-一覧異常系データ(DeleteRepositoryGroup)
        /// </summary>
        public List<string> DeleteRepositoryGroupValidationErrorData
        {
            get
            {
                return new List<string>()
                {
                    // RepositoryGroupIdがない。
                    null,
                    // RepositoryGroupIdがGuidでない。
                    "hogehoge"
                };
            }
        }

        /// <summary>
        /// リポジトリグループ-一覧異常系データ(GetRepositoryGroup)
        /// </summary>
        public List<string> GetRepositoryGroupValidationErrorData
        {
            get
            {
                // 削除と同じ
                return DeepCopy(DeleteRepositoryGroupValidationErrorData);
            }
        }

        /// <summary>
        /// リポジトリグループ-一覧異常系データ(GetRepositoryGrouplist)
        /// </summary>
        public List<RepositoryGroupModel> GetRepositoryGroupListValidationErrorData
        {
            get
            {
                // なし
                return null;
            }
        }


        #endregion
    }
}
