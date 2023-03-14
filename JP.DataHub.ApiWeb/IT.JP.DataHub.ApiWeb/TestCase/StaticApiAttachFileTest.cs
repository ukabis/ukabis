using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// 添付ファイルのStaticApiのテスト
    /// </summary>
    [TestClass]
    public class StaticApiAttachFileTest : ApiWebItTestCase
    {
        #region TestData

        private class StaticApiAttachFileTestData : TestDataBase
        {
            public CreateAttachFileRequestModel Data1 = new CreateAttachFileRequestModel()
            {
                fileName = "---itCreate---",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey1",
                        metaValue = "TestValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey2",
                        metaValue = "TestValue2"
                    }
                }
            };
            public GetAttachFileResponseModel Data1MetaExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = false,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD,
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                }
            };
            public GetAttachFileResponseModel Data1MetaExExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                isDrm = false,
                isUploaded = false,
                metaList = new List<GetAttachFileResponseModel.MetaInfo>()
                {
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = WILDCARD,
                    },
                    new GetAttachFileResponseModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    Credentials = new List<string>() { "***" },
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
                },
                _Version = 1,
                _partitionkey = $"API~IntegratedTest~ExternalAttachfileAzureBlob~{WILDCARD}",
                _Type = "API~IntegratedTest~ExternalAttachfileAzureBlob",
                _Owner_Id = WILDCARD,
                _Upduser_Id = WILDCARD,
                _Upddate = WILDCARD,
                _Reguser_Id = WILDCARD,
                _Regdate = WILDCARD,
                _Vendor_Id = WILDCARD,
                _System_Id = WILDCARD
            };

            public CreateAttachFileRequestModel DataFileNameNull = new CreateAttachFileRequestModel()
            {
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey1",
                        metaValue = "TestValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey2",
                        metaValue = "TestValue2"
                    }
                }
            };

            public CreateAttachFileRequestModel DataContentTypeNull = new CreateAttachFileRequestModel()
            {
                fileName = "---itCreate---",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey1",
                        metaValue = "TestValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey2",
                        metaValue = "TestValue2"
                    }
                }
            };

            public CreateAttachFileRequestModelEx2 DataFileLengthNull = new CreateAttachFileRequestModelEx2()
            {
                fileName = "---itCreate---",
                contentType = "image/jpeg",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey1",
                        metaValue = "TestValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey2",
                        metaValue = "TestValue2"
                    }
                }
            };

            public CreateAttachFileRequestModelEx2 DataFileLengthNotLong = new CreateAttachFileRequestModelEx2()
            {
                fileName = "---itCreate---",
                contentType = "image/jpeg",
                fileLength = "aaa",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey1",
                        metaValue = "TestValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey2",
                        metaValue = "TestValue2"
                    }
                }
            };
        }

        #endregion


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// 添付ファイル-一覧の正常系テスト
        /// DynamicApi側の詳細なテストはAttachfileTest側で実施する
        /// 本テストでは、Static API経由のDynamic API呼び出し部分に絞ってテストする
        /// </summary>
        [TestMethod]
        public void StaticApiAttachFile_NormalSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IStaticApiAttachFileApi>();
            var testData = new StaticApiAttachFileTestData();

            // 新規登録
            var fileId = client.GetWebApiResponseResult(api.CreateFile(testData.Data1)).Assert(GetSuccessExpectStatusCode).Result.fileId;

            // ファイルアップロード
            client.ChunkUploadAttachFile(fileId, client.SmallContentsPath, api);

            // ファイルをダウンロード
            client.GetWebApiResponseResult(api.GetImage(fileId)).Assert(GetSuccessExpectStatusCode);

            // ファイルのメタ情報を取得
            client.GetWebApiResponseResult(api.GetMeta(fileId)).Assert(GetSuccessExpectStatusCode);

            // ファイルのメタ情報を検索
            var meta = testData.Data1.metaList[0];
            var keyValStr = $"{meta.metaKey}={meta.metaValue}";
            client.GetWebApiResponseResult(api.MetaQuery(keyValStr)).Assert(GetSuccessExpectStatusCode);

            // 削除
            client.GetWebApiResponseResult(api.DeleteFile(fileId)).Assert(DeleteSuccessStatusCode);
        }

        /// <summary>
        /// 添付ファイル-一覧のエラー系テスト
        /// </summary>
        [TestMethod]
        public void StaticApiAttachFile_ErrorSenario()
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IStaticApiAttachFileApi>();
            var testData = new StaticApiAttachFileTestData();

            // CreateFile のValidationError
            client.GetWebApiResponseResult(api.CreateFile(testData.DataFileNameNull)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.CreateFile(testData.DataContentTypeNull)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.CreateFile(testData.DataFileLengthNull)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.CreateFile(testData.DataFileLengthNotLong)).Assert(BadRequestStatusCode);

            // UploadFile のValidationError
            // FileId がnull
            var content = new MemoryStream();
            client.GetWebApiResponseResult(api.UploadFile(content, null)).Assert(BadRequestStatusCode);
            // FileId がGuidでない
            content = new MemoryStream();
            client.GetWebApiResponseResult(api.UploadFile(content, "aaa")).Assert(BadRequestStatusCode);

            // File がnull ※StreamContentをnullにしてもbyte[0]でstreamが認識されるためテストできず。
            //context.ActionAndAssert(attachFileResource.UploadFile(Guid.Empty.ToString() ,null).Request, attachFileResource.BadRequestStatusCode);

            // GetImage のValidationError
            client.GetWebApiResponseResult(api.GetImage(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.GetImage("aaa")).Assert(BadRequestStatusCode);

            // GetImage のNotFound
            client.GetWebApiResponseResult(api.GetImage(Guid.Empty.ToString())).Assert(NotFoundStatusCode);

            // GetMeta のValidationError
            client.GetWebApiResponseResult(api.GetMeta(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.GetMeta("aaa")).Assert(BadRequestStatusCode);

            // MetaQuery のValidationError
            // Query がnull
            client.GetWebApiResponseResult(api.MetaQuery(null)).Assert(BadRequestStatusCode);

            // Delete のValidationError
            client.GetWebApiResponseResult(api.DeleteFile(null)).Assert(BadRequestStatusCode);
            client.GetWebApiResponseResult(api.DeleteFile("aaa")).Assert(BadRequestStatusCode);
        }
    }
}
