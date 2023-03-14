using System.Net;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi;
using IT.JP.DataHub.SmartFoodChainAOP.WebApi.Models;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using Microsoft.Azure.Amqp.Framing;

namespace IT.JP.DataHub.SmartFoodChainAOP.TestCase
{
    [TestClass]
    public class GetAttachFileFilterTest : ItTestCaseBase
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

        /// <summary>
        /// 各APIに添付ファイルのAOPが適用されていることを確認する。
        /// </summary>
        [DataRow("/API/SmartFoodChain/V2/Private/Shipment")]
        [DataRow("/API/SmartFoodChain/V2/Private/Arrival")]
        [DataRow("/API/SmartFoodChain/V2/Private/PartyProduct")]
        [DataRow("/API/SmartFoodChain/V2/Private/ProductAppeal")]
        [TestMethod]
        public void GetAttachFileFilter_NormalScenario(string apiUrl)
        {
            // 登録用ユーザーでファイル登録
            var regClient = this.CreateApiClient("test1");
            regClient.api.ResourceUrl = apiUrl;
            regClient.api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var regResponse = regClient.client.Request(regClient.api.CreateAttachFile(CreateAttachFileCreateModel())).ToWebApiResponseResult<ResponseFileIdModel>();
            regResponse.StatusCode.Is(HttpStatusCode.Created);
            var fileId = regResponse.Result.FileId;

            // ファイルアップロード
            var uploadResponse = regClient.client.Request(regClient.api.UploadAttachFile(fileId, new MemoryStream(Properties.Resources.strawberry))).ToWebApiResponseResult<ResponseFileIdModel>();
            uploadResponse.StatusCode.Is(HttpStatusCode.OK);

            // 取得
            var getResponse = regClient.client.Request(regClient.api.GetAttachFile(fileId)).ToWebApiResponseResult<List<Stream>>();
            getResponse.StatusCode.Is(HttpStatusCode.OK);
            
            // 取得用ユーザーでデータ取得
            var getClient = this.CreateApiClient("test3");
            getClient.api.ResourceUrl = apiUrl;
            getClient.api.AddHeaders.Add(HeaderConst.X_GetInternalAllField, "true");
            var getFullResponse = getClient.client.Request(getClient.api.GetAttachFileFullAccess(fileId)).ToWebApiResponseResult<List<Stream>>();
            getFullResponse.StatusCode.Is(HttpStatusCode.OK);

            // それぞれのデータが一致するかを確認
            getFullResponse.RawContentString.Length.Is(getResponse.RawContentString.Length);

            // 取得用ユーザーだとGetAttachFileで404になることを確認
            getResponse = getClient.client.Request(getClient.api.GetAttachFile(fileId)).ToWebApiResponseResult<List<Stream>>();
            getResponse.StatusCode.Is(HttpStatusCode.NotFound);

            // データ消す
            var delResponse= regClient.client.Request(regClient.api.DeleteAttachFile(fileId)).ToWebApiResponseResult<ResponseFileIdModel>();
            delResponse.StatusCode.Is(HttpStatusCode.NoContent);
        }

        private (IDynamicApiClient client, AttachFileResource api) CreateApiClient(string authId)
        {
            var env = UnityCore.Resolve<IServerEnvironment>();
            var auth = AuthenticationInfoFactory.Create(env).Merge(this.AccountManager.Find(authId));
            return (UnityCore.Resolve<IDynamicApiClient>(env.ToCI(), auth.ToCI()), new AttachFileResource(env));
        }

        private AttachFileCreateModel CreateAttachFileCreateModel()
        {
            return new AttachFileCreateModel
            {
                FileName = "itAttachFile",
                ContentType = "image/jpeg",
                FileLength = 1000,
                MetaList = new List<AttachFileCreateMetaModel>
                {
                    new AttachFileCreateMetaModel
                    {
                        MetaKey = "itKey",
                        MetaValue = "itValue"
                    }
                }

            };
        }
    }
}
