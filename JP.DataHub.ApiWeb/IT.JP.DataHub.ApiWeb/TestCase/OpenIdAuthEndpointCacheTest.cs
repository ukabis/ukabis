using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Unity;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb.TestCase
{
    [TestClass]
    public class OpenIdAuthEndpointCacheTest : ApiWebItTestCase
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            // ※必ず TestInitialize の最初で base.Initialize();を呼び出すこと
            base.TestInitialize(true, null);
        }

        [TestMethod]
        public void EndpointCache_UhAuthorizedtCheck()
        {
            var api = UnityCore.Resolve<IOpenIdAuthApi>();
            var client = new IntegratedTestClient(AppConfig.Account);

            // キャッシュ作成。キャッシュがある状態でも正常にエラーになればOK
            client.GetWebApiResponseResult(api.GetAll()).Assert(GetExpectStatusCodes);

            // invalidSignature
            var invalidToken = "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJpc3MiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vNjJmMTQzYWMtNTkxZS00M2VjLWE1M2ItNjVjOWZiODQxZTVkL3YyLjAvIiwiZXhwIjoxNTcyNTc2MDA5LCJuYmYiOjE1NzI1NzI0MDksImF1ZCI6ImIzYWYxNWYzLWUzNWEtNGFkMC05NTJiLThlOGQ0ZjVjYmNmMSIsIm9pZCI6IjEyNjZhYWY3LTIwNDgtNGJjNC1iODg0LTkwNzJiZTE1MmQ5OSIsInN1YiI6IjEyNjZhYWY3LTIwNDgtNGJjNC1iODg0LTkwNzJiZTE1MmQ5OSIsIm5hbWUiOiLnlLDkuK0g5pybIiwiZW1haWxzIjpbIm50YW5ha2FAbmV4dHNjYXBlLm5ldCJdLCJ0ZnAiOiJCMkNfMV9KcERhdGFIdWJfU2lVcEluIiwibm9uY2UiOiJkZWZhdWx0Tm9uY2UiLCJzY3AiOiJ3cml0ZSByZWFkIiwiYXpwIjoiYjNhZjE1ZjMtZTM1YS00YWQwLTk1MmItOGU4ZDRmNWNiY2YxIiwidmVyIjoiMS4wIiwiaWF0IjoxNTcyNTcyNDA5fQ.xxxxxx";
            var header = new Dictionary<string, string[]>() { { HeaderConst.Authorization, new string[] { invalidToken } } };
            client.GetWebApiResponseResult(api.GetAll(), null, header).AssertErrorCode(HttpStatusCode.Unauthorized, "E01406");

            // aud missing 
            var missngToken = "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiYWRtaW4iOnRydWUsImlhdCI6MTUxNjIzOTAyMn0.POstGetfAytaZS82wHcjoTyoqhMyxXiWdR7Nn7A29DNSl0EiXLdwJ6xC6AfgZWF1bOsS_TuYI3OG85AmiExREkrS6tDfTQ2B3WXlrr-wp5AokiRbz3_oB4OxG-W9KcEEbDRcZc0nH3L7LzYptiy1PtAylQGxHTWZXtGz4ht0bAecBgmpdgXMguEIcoqPJ1n3pIWk_dUZegpqx0Lka21H6XxUTxiy8OcaarA8zdnPUnV6AmNP3ecFawIFYdvJB_cm-GvpCSbr8G8y_Mllj8f4x9nBH8pQux89_6gUY618iYv7tuPWBFfEbLxtF2pZS6YC1aSfLQxeNe8djT9YjpvRZA";
            header = new Dictionary<string, string[]>() { { HeaderConst.Authorization, new string[] { missngToken } } };
            client.GetWebApiResponseResult(api.GetAll(), null, header).AssertErrorCode(HttpStatusCode.Unauthorized, "E01406");

            // expired
            var expiredToken = AppConfig.ExpiredToken;
            header = new Dictionary<string, string[]>() { { HeaderConst.Authorization, new string[] { $"Bearer {expiredToken}" } } };
            client.GetWebApiResponseResult(api.GetAll(), null, header).AssertErrorCode(HttpStatusCode.Unauthorized, "E01405");
        }
    }
}
