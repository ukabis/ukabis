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
    [TestCategory("AOP")]
    public class AttachFileOverPartitionTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        [DataRow(Repository.CosmosDb)]
        [DataRow(Repository.SqlServer)]
        public void AttachFileOverPartitionScenario(Repository repository)
        {
            var clientA = new IntegratedTestClient("test1", "SmartFoodChainAdmin") { TargetRepository = repository };
            var clientB = new IntegratedTestClient("test2", "SmartFoodChainAdmin") { TargetRepository = repository };
            var api = UnityCore.Resolve<IAttachfileOverPartitionApi>();

            // メタ作成
            var data = new CreateAttachFileRequestModel()
            {
                fileName = "tenputest.jpg",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>()
                {
                    new CreateAttachFileRequestModel.MetaInfo()
                    {
                        metaKey = Guid.NewGuid().ToString(),
                        metaValue = Guid.NewGuid().ToString()
                    }
                }
            };
            var fileId = clientA.GetWebApiResponseResult(api.CreateAttachFile(data)).Assert(RegisterSuccessExpectStatusCode).Result.fileId;

            // FileUpload
            clientA.ChunkUploadAttachFile(fileId, clientA.SmallContentsPath, api);

            // FileDownload           
            clientA.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(GetSuccessExpectStatusCode, GetContentsByte(clientA.SmallContentsPath));

            // 別のユーザーからも取得できることを確認
            clientB.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(GetSuccessExpectStatusCode, GetContentsByte(clientA.SmallContentsPath));

            // メタは取得できない(AOP設定してないので)
            clientB.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(NotFoundStatusCode);

            // 削除
            clientA.GetWebApiResponseResult(api.DeleteAttachFile(fileId)).Assert(DeleteSuccessStatusCode);
        }
    }
}
