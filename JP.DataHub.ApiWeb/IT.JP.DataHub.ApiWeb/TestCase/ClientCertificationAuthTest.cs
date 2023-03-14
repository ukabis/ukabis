using System.Collections.Generic;
using System.IO;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    /// <summary>
    /// クライアント証明書を使用した認証テスト
    /// ********* 注意 ****************
    /// このテストをローカルで動かす場合は、以下の設定の上、動かしてください。
    /// １．ITのTestContents\ClientCertificationの下にある、client.crt を、PCの証明書ストアの、
    ///      「信頼されたルート証明機関」にインストールする
    /// ２．「.vs\JP.DataHub\config\applicationhost.config」 の、以下の項目
    ///  　＜access sslFlags="SslNegotiateCert" ／＞ <- コメントアウトが消えるので、わざと全角にしてます
    /// ３．ApiWebのweb.config の、apiweb:UseClientCertificateAuth を trueに設定
    /// ４．ローカルでApiWebのHTTPSサイトの設定が無い場合は、設定する
    /// 
    ///  .vs がGitの対象外なのと、証明書ストアの状態でテストが失敗するので、
    ///  このテストは、使う環境以外、ローカルも含めてSkip設定してます
    /// </summary>
    [TestClass]
    public class ClientCertificationAuthTest : ApiWebItTestCase
    {
        public string ValidCertificationPathPfx = Path.GetFullPath("TestContents/ClientCertification/valid/client.pfx");
        public string ExpiredCertificationPathPfx = Path.GetFullPath("TestContents/ClientCertification/expired/client.pfx");
        public string InvalidCertificationPathPfx = Path.GetFullPath("TestContents/ClientCertification/invalid/client.pfx");


        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        /// <summary>
        /// クライアント証明書を設定してリクエスト
        /// </summary>
        [TestMethod]
        public void ClientCertificationAuthTest_NormalSenario_AuthByClientCert()
        {
            var client = new IntegratedTestClient(AppConfig.Account, null, ValidCertificationPathPfx);
            var api = UnityCore.Resolve<IClientCertificationAuthApi>();

            // NotFound(unauthorize/forbidden ではない)
            client.GetWebApiResponseResult(api.GetByClientCert()).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// 通常のベンダー証明を設定してリクエスト
        /// </summary>
        [TestMethod]
        public void ClientCertificationAuthTest_NormalSenario_AuthByVendorToken()
        {
            var client = new IntegratedTestClient(AppConfig.Account, "SmartFoodChainAdmin", ValidCertificationPathPfx);
            var api = UnityCore.Resolve<IClientCertificationAuthApi>();

            // NotFound(unauthorize/forbidden ではない)
            client.GetWebApiResponseResult(api.GetByNormal()).Assert(NotFoundStatusCode);
        }

        /// <summary>
        /// 未登録のクライアント証明書を設定してリクエスト
        /// </summary>
        [TestMethod]
        public void ClientCertificationAuthTest_Abnormal_InvalidCertificate()
        {
            var client = new IntegratedTestClient(AppConfig.Account, null, InvalidCertificationPathPfx);
            var api = UnityCore.Resolve<IClientCertificationAuthApi>();

            //// Unauthorized
            //client.GetWebApiResponseResult(api.GetByClientCert()).AssertErrorCode(HttpStatusCode.Unauthorized, "E02409");
            client.GetWebApiResponseResult(api.GetByClientCert()).Assert(HttpStatusCode.Forbidden);
        }

        /// <summary>
        /// 有効期限切れクライアント証明書を設定してリクエスト
        /// </summary>
        [TestMethod]
        public void ClientCertificationAuthTest_Abnormal_ExpiredCertificate()
        {
            var client = new IntegratedTestClient(AppConfig.Account, null, ExpiredCertificationPathPfx);
            var api = UnityCore.Resolve<IClientCertificationAuthApi>();

            // Forbidden or unauthorize(ローカルと他で挙動が違う。ローカルはforbidden)
            var response = client.GetWebApiResponseResult(api.GetByClientCert()).Assert(new HttpStatusCode[] { HttpStatusCode.Forbidden, HttpStatusCode.Unauthorized });

            // ローカル以外は有効期限切れエラーメッセージの検証ができるのでチェックする
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var body = response.RawContentString;
                // 環境によってはExpiredのクライアント証明書を持ってきてくれないので、E02409orE02410どちらかであればOKとする
                var flg = body.Contains("E02410");
                if (!flg)
                {
                    flg = body.Contains("E02409");
                }
                flg.Is(true);
            }
        }

        /// <summary>
        /// クライアント認証が設定されているAPIからRoslynでクライアント認証設定されていないベンダーシステム認証のAPIが呼び出し可能なこと
        /// </summary>
        [TestMethod]
        public void ClientCertificationAuthTest_Roslyn_認証引継ぎ()
        {
            // RoslynCall => ベンダーシステム認証OFF、OpenID認証ON。クライアント証明書認証ON。Roslyn内でRoslynInternalCallの呼び出し
            // RoslynInternalCall => ベンダーシステム認証OFF、OpenID認証ON。クライアント証明書認証OFF。

            var client = new IntegratedTestClient(AppConfig.Account, null, ValidCertificationPathPfx);
            var api = UnityCore.Resolve<IClientCertificationAuthApi>();

            // エラーにならない
            var response = client.GetWebApiResponseResult(api.RoslynCall()).Assert(NotFoundStatusCode);
        }
    }
}
