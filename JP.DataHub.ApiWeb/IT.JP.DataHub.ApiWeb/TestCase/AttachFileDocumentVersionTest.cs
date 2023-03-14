using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Polly;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class AttachFileDocumentVersionTest : ApiWebItTestCase
    {
        private const string HIGH_PERFORMANCE = "HighPerformance";
        private const string LOW_PERFORMANCE = "LowPerformance";
        private const string DELETE = "Delete";

        #region TestData

        private class AttachFileDocumentVersionTestData : TestDataBase
        {
            public string SmallContentsPath = Path.GetFullPath("TestContents/AttachFile/tractor_man.png");
            public string SmallContentsPath2 = Path.GetFullPath("TestContents/AttachFile/nougyou_inekari.png");

            public CreateAttachFileRequestModel Data2 = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 4076717,
                key = "TEST_KEY",
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "TestKey",
                        metaValue = "TEST_KEY"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaValue = "Key2",
                        metaKey = "KeyValue2"
                    }
                }
            };

            public CreateAttachFileRequestModel DataEx = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 4076717,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = "Key1",
                        metaValue = "KeyValue1"
                    },
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaValue = "Key2",
                        metaKey = "KeyValue2"
                    }
                },
                IsExternalAttachFile = true,
                ExternalAttachFile = new ExternalAttachFileModel()
                {
                    DataSourceType = "az-blob",
                    Credentials = new List<string>() { "hoge", "fuga" },
                    FilePath = "piyo.png"
                }
            };

            public AttachFileDocumentVersionTestData(Repository repository, string resourceUrl, bool isVendor = false) : 
                base(repository, resourceUrl, isVendor)
            {
            }
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
        public void AttachFileDocumentVersion_NormalScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account) { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachFileDocumentVersionApi>();
            var testData = new AttachFileDocumentVersionTestData(repository, api.ResourceUrl);

            var versions = new List<KeyValuePair<string, string>>();
            var headerExpect = new Dictionary<string, string[]>() { { HeaderConst.X_DocumentHistory, new string[] { "*" } } };
            void versionSaveAction(string key, IEnumerable<string> vals)
            {
                if (key == HeaderConst.X_DocumentHistory)
                {
                    var historyHeaders = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(vals.Single());
                    historyHeaders.Single().documents.ToList().ForEach(x => versions.Add(new KeyValuePair<string, string>(x.documentKey, x.versionKey)));
                }
            }

            // メタデータの作成
            var fileId = client.GetWebApiResponseResult(api.CreateAttachFile(testData.Data2)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            var response = client.ChunkUploadAttachFile(fileId, testData.SmallContentsPath, api);
            response.Headers.ToList().ForEach(x => versionSaveAction(x.Key, x.Value));
            versions.Single().Key.Is(fileId);

            // 履歴が記録されているか
            var version1 = client.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            var upload_version1 = version1.Single(x => versions.Any(y => y.Value == x.VersionKey));
            upload_version1.Location.Is(fileId);
            upload_version1.LocationType.Is(HIGH_PERFORMANCE);
            upload_version1.VersionNo.Is(1);

            // fileupload 2回目
            response = client.ChunkUploadAttachFile(fileId, testData.SmallContentsPath2, api);
            response.Headers.ToList().ForEach(x => versionSaveAction(x.Key, x.Value));

            // 履歴が記録されているか
            var version2 = client.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            version2.Count().Is(2);
            var upload_version2 = version2.Single(x => versions.ElementAt(1).Value == x.VersionKey);
            upload_version2.Location.Is(fileId);
            upload_version2.LocationType.Is(HIGH_PERFORMANCE);
            upload_version2.RepositoryGroupId.Is(upload_version1.RepositoryGroupId);
            upload_version2.PhysicalRepositoryId.Is(upload_version1.PhysicalRepositoryId);
            upload_version2.VersionNo.Is(2);

            var upload_version2_1 = version2.ElementAt(0);
            upload_version2_1.LocationType.Is(LOW_PERFORMANCE);
            Assert.IsTrue(upload_version1.Location != upload_version2_1.Location);
            upload_version2_1.VersionKey.Is(upload_version1.VersionKey);
            upload_version2_1.VersionNo.Is(upload_version1.VersionNo);
            upload_version2_1.CreateDate.Is(upload_version1.CreateDate);

            // 1回目と2回目を履歴から復元(from guid) 
            client.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionGuidAndKey(fileId, Guid.Parse(upload_version1.VersionKey), testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath));
            client.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionGuidAndKey(fileId, Guid.Parse(upload_version2.VersionKey), testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath2));

            // 1回目と2回目を履歴から復元(from versionnumber) 
            client.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionNoAndKey(fileId, upload_version1.VersionNo, testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath));
            client.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionNoAndKey(fileId, upload_version2.VersionNo, testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath2));

            // driveout
            client.GetWebApiResponseResult(api.DriveOutAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);

            // DLしようとしてもできない
            client.GetWebApiResponseResult(api.GetAttachFile(fileId, $"Key={testData.Data2.key}")).Assert(NotFoundStatusCode);

            // GetAttachFileDocumentHistoryならDL可
            client.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionNoAndKey(fileId, upload_version2.VersionNo, testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath2));

            // 履歴チェック
            var versionDod = client.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            var driveoutVersion = versionDod.ElementAt(1);
            driveoutVersion.VersionNo.Is(2);
            driveoutVersion.LocationType.Is(LOW_PERFORMANCE);
            driveoutVersion.RepositoryGroupId.Is(upload_version2_1.RepositoryGroupId);
            driveoutVersion.PhysicalRepositoryId.Is(upload_version2_1.PhysicalRepositoryId);

            // return
            client.GetWebApiResponseResult(api.ReturnAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);

            // DLできるか確認
            client.GetWebApiResponseResult(api.GetAttachFile(fileId, $"Key={testData.Data2.key}")).Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath2));

            // 履歴チェック
            var versionRtd = client.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            var returnVersion = versionRtd.ElementAt(1);
            returnVersion.VersionNo.Is(2);
            returnVersion.Location.Is(fileId);
            returnVersion.LocationType.Is(HIGH_PERFORMANCE);
            returnVersion.RepositoryGroupId.Is(upload_version1.RepositoryGroupId);
            returnVersion.PhysicalRepositoryId.Is(upload_version1.PhysicalRepositoryId);

            // こちらのAPIはベンダー依存無しなので、ベンダーBでも操作可能なことを確認する
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };

            clientB.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode);
            clientB.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionGuidAndKey(fileId, Guid.Parse(upload_version1.VersionKey), testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath));
            clientB.GetWebApiResponseResult(api.DriveOutAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);
            clientB.GetWebApiResponseResult(api.ReturnAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);

            // file delete
            client.GetWebApiResponseResult(api.DeleteAttachFile(fileId, $"Key={testData.Data2.key}")).Assert(DeleteSuccessStatusCode, headerExpect, null, versionSaveAction);

            // 履歴が記録されているか
            var version3 = client.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            version3.Count().Is(3);
            var delete_version3 = version3.Single(x => x.VersionKey == versions.ElementAt(2).Value);
            delete_version3.VersionNo.Is(3);
            delete_version3.Location.IsNull();
            delete_version3.LocationType.Is(DELETE);
            delete_version3.PhysicalRepositoryId.IsNull();
            delete_version3.RepositoryGroupId.IsNull();

            var upload_version3_1 = version3.Single(x => x.VersionKey == versions.ElementAt(0).Value);
            upload_version3_1.Location.Is(upload_version2_1.Location);
            upload_version3_1.LocationType.Is(LOW_PERFORMANCE);
            upload_version3_1.VersionKey.Is(upload_version1.VersionKey);
            upload_version3_1.VersionNo.Is(upload_version1.VersionNo);
            upload_version3_1.CreateDate.Is(upload_version1.CreateDate);
            upload_version3_1.RepositoryGroupId.Is(upload_version2_1.RepositoryGroupId);
            upload_version3_1.PhysicalRepositoryId.Is(upload_version2_1.PhysicalRepositoryId);

            var upload_version3_2 = version3.Single(x => x.VersionKey == versions.ElementAt(1).Value);
            Assert.IsTrue(upload_version3_2.Location != upload_version2.Location);
            upload_version3_2.LocationType.Is(LOW_PERFORMANCE);
            upload_version3_2.VersionNo.Is(2);
            upload_version3_2.CreateDate.Is(upload_version2.CreateDate);
            upload_version3_2.RepositoryGroupId.Is(upload_version2_1.RepositoryGroupId);
            upload_version3_2.PhysicalRepositoryId.Is(upload_version2_1.PhysicalRepositoryId);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileDocumentVersion_NotFoundByOtherVendorTest(Repository repository)
        {
            var clientA = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainPortal") { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachFileDocumentVersionVendorDependencyApi>();
            var testData = new AttachFileDocumentVersionTestData(repository, api.ResourceUrl, true);

            var versions = new List<KeyValuePair<string, string>>();
            var headerExpect = new Dictionary<string, string[]>() { { "X-DocumentHistory", new string[] { "*" } } };
            void versionSaveAction(string key, IEnumerable<string> vals)
            {
                if (key == "X-DocumentHistory")
                {
                    var historyHeaders = JsonConvert.DeserializeObject<IEnumerable<DocumentHistoryHeader>>(vals.Single());
                    historyHeaders.Single().documents.ToList().ForEach(x => versions.Add(new KeyValuePair<string, string>(x.documentKey, x.versionKey)));
                }
            }

            // メタデータの作成
            var fileId = clientA.GetWebApiResponseResult(api.CreateAttachFile(testData.Data2)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            var response = clientA.ChunkUploadAttachFile(fileId, testData.SmallContentsPath, api);
            response.Headers.ToList().ForEach(x => versionSaveAction(x.Key, x.Value));
            versions.Single().Key.Is(fileId);

            // ベンダーAで一通り実施する
            var version1 = clientA.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            var upload_version1 = version1.Single(x => versions.Any(y => y.Value == x.VersionKey));

            // fileupload 2回目
            response = clientA.ChunkUploadAttachFile(fileId, testData.SmallContentsPath2, api);
            response.Headers.ToList().ForEach(x => versionSaveAction(x.Key, x.Value));

            var version2 = clientA.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode).Result;
            var upload_version2 = version2.Single(x => versions.ElementAt(1).Value == x.VersionKey);
            var upload_version2_1 = version2.ElementAt(0);

            // 1回目と2回目を履歴から復元(from guid) 
            clientA.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionGuidAndKey(fileId, Guid.Parse(upload_version1.VersionKey), testData.Data2.key))
                .Assert(GetSuccessExpectStatusCode, GetContentsByte(testData.SmallContentsPath));

            // driveout
            clientA.GetWebApiResponseResult(api.DriveOutAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);

            // return
            clientA.GetWebApiResponseResult(api.ReturnAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);

            // ベンダーBでNotFoundテスト
            // GetDocumentVersion ベンダーBから取得できないことを確認
            clientB.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(NotFoundStatusCode);

            // GetAttachFileDocumentHistory ベンダーBから取得できないことを確認
            clientB.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionGuidAndKey(fileId, Guid.Parse(upload_version1.VersionKey), testData.Data2.key)).Assert(NotFoundStatusCode);

            // DriveOutAttachFileDocument ベンダーBからは、エラーになることを確認
            clientB.GetWebApiResponseResult(api.DriveOutAttachFileDocument(fileId)).Assert(NotFoundStatusCode);

            // ReturnAttachFileDocument ベンダーBからは、エラーになることを確認(DriveOutしていないので、Returnできないが、念のため)
            clientB.GetWebApiResponseResult(api.ReturnAttachFileDocument(fileId)).Assert(NotFoundStatusCode);

            // 念のため、Aでは、同じ操作してエラーにならないことを再確認
            // GetDocumentVersion
            clientA.GetWebApiResponseResult(api.GetAttachFileDocumentVersion(fileId)).Assert(GetSuccessExpectStatusCode);

            // GetAttachFileDocumentHistory 
            clientA.GetWebApiResponseResult(api.GetAttachFileDocumentHistoryWithVersionGuidAndKey(fileId, Guid.Parse(upload_version1.VersionKey), testData.Data2.key)).Assert(GetSuccessExpectStatusCode);

            // DriveOutAttachFileDocument 
            clientA.GetWebApiResponseResult(api.DriveOutAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);

            // ReturnAttachFileDocument 
            clientA.GetWebApiResponseResult(api.ReturnAttachFileDocument(fileId)).Assert(HttpStatusCode.NoContent);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileDocumentVersion_ExternalAttachFileScenario(Repository repository)
        {
            var client = new IntegratedTestClient(AppConfig.Account);
            var api = UnityCore.Resolve<IAttachFileDocumentVersionApi>();
            var testData = new AttachFileDocumentVersionTestData(repository, api.ResourceUrl);

            client.GetWebApiResponseResult(api.CreateAttachFile(testData.DataEx)).AssertErrorCode(BadRequestStatusCode, "E20418");
        }
    }
}
