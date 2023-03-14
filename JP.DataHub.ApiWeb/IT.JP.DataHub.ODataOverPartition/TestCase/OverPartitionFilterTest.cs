using System.Net;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.ODataOverPartition.WebApi;
using IT.JP.DataHub.ODataOverPartition.WebApi.Models;
using IT.JP.DataHub.SmartFoodChainAOP;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.UnitTest.Com.Extensions;
using Microsoft.Azure.Amqp.Framing;

namespace IT.JP.DataHub.ODataOverPartition.TestCase
{
    [TestClass]
    public class OverPartitionFilterTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize(true);
        }

        [TestCleanup]
        public new void TestCleanup()
        {
            base.TestCleanup();
        }
        
        [TestMethod]
        public void OverPartitionFilter_NormalScenario()
        {
            var regClient = new IntegratedTestClient(AppConfig.Account);
            var getClient = new IntegratedTestClient("test3");
            var api = UnityCore.Resolve<IAttachFileOverPartitionApi>();

            // 登録用ユーザーでファイル登録
            var regData = new CreateAttachFileRequestModel
            {
                fileName = "itAttachFile",
                contentType = "image/jpeg",
                fileLength = 1000,
                metaList = new List<CreateAttachFileRequestModel.MetaInfo>
                {
                    new()
                    {
                        metaKey = "itKey",
                        metaValue = "itValue"
                    }
                }
            };
            var regResponse = regClient.GetWebApiResponseResult(api.CreateAttachFile(regData)).Assert(RegisterSuccessExpectStatusCode);
            var fileId = regResponse.Result.fileId;

            // ファイルアップロード
            regClient.GetWebApiResponseResult(api.UploadAttachFile(new MemoryStream(Properties.Resources.strawberry), fileId)).Assert(HttpStatusCode.OK);

            // 取得
            var getResponse = regClient.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(GetSuccessExpectStatusCode);
            
            // 取得用ユーザーでデータ取得
            var getOverPartitionResponse = getClient.GetWebApiResponseResult(api.GetAttachFile(fileId)).Assert(GetSuccessExpectStatusCode);

            // それぞれのデータが一致するかを確認
            getOverPartitionResponse.RawContentString.Length.Is(getResponse.RawContentString.Length);
            
            // Filterが適用されてないメソッドでは取れないことを確認
            getClient.GetWebApiResponseResult(api.GetAttachFileMeta(fileId)).Assert(HttpStatusCode.NotFound);
            
            // データ消す
            regClient.GetWebApiResponseResult(api.DeleteAttachFile(fileId)).Assert(DeleteSuccessStatusCode);
        }
    }
}
