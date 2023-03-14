using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ExternalAttachfileAzureBlobTest : ApiWebItTestCase
    {
        #region TestData

        private class ExternalAttachfileAzureBlobTestData : TestDataBase
        {
            public AreaUnitModel Data0 = new AreaUnitModel()
            {
                AreaUnitCode = "AA",
                AreaUnitName = "aaa"
            };
            public RegisterResponseModel Data0RegistExpected = new RegisterResponseModel()
            {
                id = $"API~IntegratedTest~ExternalAttachfileAzureBlob~1~AA"
            };
            public AreaUnitModel Data0Get = new AreaUnitModel()
            {
                id = $"API~IntegratedTest~ExternalAttachfileAzureBlob~1~AA",
                _Owner_Id = WILDCARD,
                AreaUnitCode = "AA",
                AreaUnitName = "aaa"
            };

            public CreateAttachFileRequestModel Data1 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
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

            public CreateAttachFileRequestModel Data2 = new CreateAttachFileRequestModel()
            {
                fileName = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890ab.jpg",
                contentType = "image/jpeg",
                fileLength = 4076717,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    FilePath = "integratedtest-externalattachfile/TestFiles/IMG_20171118_122534916.jpg"
                }
            };
            public GetAttachFileResponseModel Data2MetaExpected = new GetAttachFileResponseModel()
            {
                fileId = WILDCARD,
                fileName = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890ab.jpg",
                contentType = "image/jpeg",
                fileLength = 4076717,
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

            public CreateAttachFileRequestModel DataBadRequest1 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true
            };

            public CreateAttachFileRequestModel DataBadRequest2 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "hoge",
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
                }
            };

            public CreateAttachFileRequestModel DataBadRequest3 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    Credentials = new List<string>(),
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
                }
            };

            public CreateAttachFileRequestModel DataBadRequest4 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    Credentials = new List<string>(),
                    FilePath = "integratedtest-externalattachfile"
                }
            };

            public CreateAttachFileRequestModel DataBadRequest5 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    Credentials = new List<string>() { "hogehoge" },
                    FilePath = "integratedtest-externalattachfile/TestFiles/tractor_man.png"
                }
            };

            public CreateAttachFileRequestModel DataBadRequest6 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key2",
                        metaValue = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    FilePath = "integratedtest-externalattachfile/TestFiles/hogehoge.png"
                }
            };

            public ExternalAttachfileAzureBlobTestData(string resourceUrl, Repository repository = Repository.Default, IntegratedTestClient client = null) : 
                base(repository, resourceUrl, true, false, client) { }
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
        public void ExternalAttachfileAzureBlobNormalSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IExternalAttachfileAzureBlobApi>();
            var testData = new ExternalAttachfileAzureBlobTestData(api.ResourceUrl, repository, client);

            var testKey = Guid.NewGuid().ToString();
            var credential = AppConfig.ExternalStorageAzureBlob;
            testData.Data1.metaList.First().metaValue = testKey;
            testData.Data1.ExternalAttachFile.Credentials = new List<string>() { credential };

            // データ削除
            client.GetWebApiResponseResult(api.DeleteAll()).Assert(DeleteExpectStatusCodes);

            // データ1を１件登録
            client.GetWebApiResponseResult(api.Register(testData.Data0)).Assert(RegisterSuccessExpectStatusCode, testData.Data0RegistExpected);

            // メタ作成
            var fileid = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data1)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileDownload           
            client.GetWebApiResponseResult(api.GetAttachFile(fileid)).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.SmallContentsPath));

            // MetaGet
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileid)).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExpected);

            // MetaGet(X-GetInternalAllField)
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileid)).Assert(GetSuccessExpectStatusCode, testData.Data1MetaExExpected);
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // MetaGetList
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={Guid.NewGuid()}")).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKey}")).Assert(GetSuccessExpectStatusCode, new List<GetAttachFileResponseModel>() { testData.Data1MetaExpected });

            // MetaGetList(X-GetInternalAllField)
            api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={Guid.NewGuid()}")).Assert(NotFoundStatusCode);
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKey}")).Assert(GetSuccessExpectStatusCode, new List<GetAttachFileResponseModel>() { testData.Data1MetaExExpected });
            api.AddHeaders.Remove(HeaderConst.X_GetInternalAllField);

            // データ取得(AttachFileが混ざっていないことの確認)
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetSuccessExpectStatusCode, new List<AreaUnitModel>() { testData.Data0Get });

            // FileDelete
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileid)).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileid)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            client.GetWebApiResponseResult(api.GetAttachFile(fileid)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ExternalAttachfileAzureBlobNormalSenarioLargeFileAndKey(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IExternalAttachfileAzureBlobApi>();
            var testData = new ExternalAttachfileAzureBlobTestData(api.ResourceUrl, repository, client);

            var testKey = Guid.NewGuid().ToString();
            var credential = AppConfig.ExternalStorageAzureBlob;
            testData.Data2.metaList.First().metaValue = testKey;
            testData.Data2.ExternalAttachFile.Credentials = new List<string>() { credential };

            // メタ作成
            var fileid = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data2)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // Keyなし
            client.GetWebApiResponseResult(api.GetAttachFile(fileid)).AssertErrorCode(BadRequestStatusCode, "E20402");

            // FileDownload  
            client.GetWebApiResponseResult(api.GetAttachFile(fileid, $"Key={testData.Data2.key}")).Assert(GetSuccessExpectStatusCode, GetContentsByte(client.LargeContentsPath));

            // MetaGet
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileid)).Assert(GetSuccessExpectStatusCode, testData.Data2MetaExpected);

            // MetaGetList
            client.GetWebApiResponseResult(api.GetAttachFileMetaList($"TestKey={testKey}")).Assert(GetSuccessExpectStatusCode, new List<GetAttachFileResponseModel>() { testData.Data2MetaExpected });

            // FileDeleteKeyなし
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileid)).Assert(BadRequestStatusCode);

            // FileDelete
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileid, $"Key={testData.Data2.key}")).Assert(DeleteSuccessStatusCode);

            // MetaGet (削除確認)
            client.GetWebApiResponseResult(api.GetAttachFileMeta(fileid)).Assert(NotFoundStatusCode);

            // FileDownload(削除確認)
            client.GetWebApiResponseResult(api.GetAttachFile(fileid)).Assert(NotFoundStatusCode);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void ExternalAttachFileBadRequestSenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IExternalAttachfileAzureBlobApi>();
            var testData = new ExternalAttachfileAzureBlobTestData(api.ResourceUrl, repository, client);

            var credential = AppConfig.ExternalStorageAzureBlob;

            // ExternalAttachFileなし
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest1)).AssertErrorCode(BadRequestStatusCode, "E20413");

            // DataSourceType不正
            testData.DataBadRequest2.ExternalAttachFile.Credentials = new List<string>() { credential };
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest2)).AssertErrorCode(BadRequestStatusCode, "E20414");

            // Credentialsなし
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest3)).AssertErrorCode(BadRequestStatusCode, "E20414");

            // FilePath不正
            testData.DataBadRequest4.ExternalAttachFile.Credentials = new List<string>() { credential };
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest4)).AssertErrorCode(BadRequestStatusCode, "E20414");

            // ストレージ接続エラー
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest5)).AssertErrorCode(BadRequestStatusCode, "E20415");

            // ファイル存在せず
            testData.DataBadRequest6.ExternalAttachFile.Credentials = new List<string>() { credential };
            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataBadRequest6)).AssertErrorCode(BadRequestStatusCode, "E20416");
        }
    }
}
