using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using JP.DataHub.Com.Authentication;
using JP.DataHub.Com.Consts;
using JP.DataHub.Com.Net.Http;
using JP.DataHub.Com.Net.Http.Models;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.Web;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.UnitTest.Com.Extensions;
using IT.JP.DataHub.ApiWeb.WebApi;
using IT.JP.DataHub.ApiWeb.WebApi.Models;

namespace IT.JP.DataHub.ApiWeb
{
    public enum Repository
    {
        Default,
        CosmosDb,
        MongoDb,
        MongoDbCds,
        SqlServer
    }

    public enum Accept
    {
        Json,
        Xml,
        Csv,
        GeoJson
    }


    public class IntegratedTestClient : DynamicApiClient
    {
        public string SmallContentsPath = Path.GetFullPath("TestContents/AttachFile/tractor_man.png");
        public string SmallContentsPath2 = Path.GetFullPath("TestContents/AttachFile/nougyou_inekari.png");
        public string LargeContentsPath = Path.GetFullPath("TestContents/AttachFile/IMG_20171118_122534916.jpg");
        public string Base648MbOverContentsPath = Path.GetFullPath("TestContents/AttachFile/base64_8mbover.jpg");

        public Repository TargetRepository { get; set; }
        public VendorAuthenticationServerInfo.VendorSystemInfo VendorSystemInfo { get; set; }


        protected RetryPolicy<HttpResponseMessage> retryStatusPolicy = Policy
            .HandleResult<HttpResponseMessage>(r =>
            {
                if (!r.IsSuccessStatusCode)
                {
                    return true;
                }

                var status = JsonConvert.DeserializeObject<GetStatusResponseModel>(r.Content.ReadAsStringAsync().Result);
                status.Status.IsNot("Error");
                return status.Status != "End";
            })
            .WaitAndRetry(9, i => TimeSpan.FromSeconds(20));


        public IntegratedTestClient()
            : base()
        {
        }

        public IntegratedTestClient(string nameAccount)
            : base(nameAccount)
        {
            var vendorAuthServerInfo = (VendorAuthenticationServerInfo)ServerEnvironment.ServerAuthenticationList[AuthenticationServerType.Vendor];
            VendorSystemInfo = vendorAuthServerInfo.VendorSystemList.FirstOrDefault(x => x.ClientId == vendorAuthServerInfo.ClientId);
        }

        public IntegratedTestClient(string nameAccount = null, string vendorSystemAuthName = null, string clientCertificatePath = null)
        {
            ServerEnvironment = UnityCore.Resolve<IServerEnvironment>();
            if (string.IsNullOrEmpty(clientCertificatePath))
            {
                WebRequestManager = new WebRequestManager();
            }
            else
            {
                var clientCertificate = new X509Certificate2(File.ReadAllBytes(clientCertificatePath), (string)null, X509KeyStorageFlags.MachineKeySet);
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(clientCertificate);
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;

                WebRequestManager = new WebRequestManager(clientCertificatePath, handler);
            }

            // OpenID認証情報取得
            IAuthenticationInfo openIdAuthInfo = null;
            if (!string.IsNullOrEmpty(nameAccount))
            {
                var accountManager = UnityCore.Resolve<IAuthenticationManager>(AuthenticationManager.AccountJsonFileName.ToCI());
                openIdAuthInfo = accountManager.Find(nameAccount);
            }

            // ベンダーシステム認証情報取得
            IAuthenticationInfo vendorAuthInfo = null;
            if (!string.IsNullOrEmpty(vendorSystemAuthName))
            {
                var vendorAuthServerInfo = (VendorAuthenticationServerInfo)ServerEnvironment.ServerAuthenticationList[AuthenticationServerType.Vendor];
                VendorSystemInfo = vendorAuthServerInfo.VendorSystemList.First(x => x.Name == vendorSystemAuthName);
                vendorAuthInfo = new VendorAuthenticationInfo() 
                { 
                    Vendor = new VendorInfo() 
                    { 
                        ClientId = VendorSystemInfo.ClientId, 
                        ClientSecret = VendorSystemInfo.ClientSecret, 
                        Url = VendorSystemInfo.Url 
                    } 
                };
            }

            // 認証情報を設定
            if (openIdAuthInfo != null && vendorAuthInfo != null)
            {
                AuthenticationInfo = openIdAuthInfo.Merge(vendorAuthInfo);
            }
            else if (openIdAuthInfo != null)
            {
                AuthenticationInfo = openIdAuthInfo;
            }
            else if (vendorAuthInfo != null)
            {
                AuthenticationInfo = vendorAuthInfo;
            }
            else
            {
                AuthenticationInfo = new NothingAuthenticationInfo();
            }
        }


        public override HttpResponseMessage Request(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            ChangeResourceUrl(req);
            return base.Request(req, addtionalHeader, replacementHeader);
        }

        public override HttpResponseMessage Request(WebApiRequestModel req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            ChangeResourceUrl(req);
            return base.Request(req, retryPolicy, addtionalHeader, replacementHeader);
        }

        public override HttpResponseMessage Request<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            ChangeResourceUrl(req);
            return base.Request<T>(req, addtionalHeader, replacementHeader);
        }

        public override HttpResponseMessage Request<T>(WebApiRequestModel<T> req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            ChangeResourceUrl(req);
            return base.Request<T>(req, retryPolicy, addtionalHeader, replacementHeader);
        }

        public string GetOpenId()
        {
            // 認証が取れていない場合は取得する
            if (AuthenticationResult == null)
            {
                var service = AuthenticationInfo?.GetAuthenticationService();
                AuthenticationResult = service?.Authentication(this.ServerEnvironment, AuthenticationInfo);
            }

            var authResults = new List<IAuthenticationResult>();
            if (AuthenticationResult is CombinationAuthenticationResult combinationResult)
            {
                authResults.AddRange(combinationResult);
            }
            else
            {
                authResults.Add(AuthenticationResult);
            }

            HttpResponseResult<TokenInfo> openIdAuthResult = null;
            foreach (var authResult in authResults.Where(x => x.IsSuccessfull))
            {
                if (authResult is AuthenticationResult aResult && aResult.OpenId != null)
                {
                    openIdAuthResult = aResult.OpenId;
                }
                else if (authResult is OpenIdAuthenticationResult openIdResult && openIdResult.OpenId != null)
                {
                    openIdAuthResult = openIdResult.OpenId;
                }
            }

            if (openIdAuthResult != null)
            {
                return openIdAuthResult.Result.access_token.ParseJwt().oid;
            }

            return null;
        }

        public string DisableAdminAuthentication()
        {
            string oldKey = null;

            foreach (var info in ServerEnvironment.ServerAuthenticationList.Values)
            {
                if (info is VendorAuthenticationServerInfo vendorInfo)
                {
                    oldKey = vendorInfo.AdminKey;
                    vendorInfo.AdminKey = null;
                }
            }

            return oldKey;
        }

        public void EnableAdminAuthentication(string adminKey)
        {
            foreach (var info in ServerEnvironment.ServerAuthenticationList.Values)
            {
                if (info is VendorAuthenticationServerInfo vendorInfo)
                {
                    vendorInfo.AdminKey = adminKey;
                }
            }
        }


        private void ChangeResourceUrl(WebApiRequestModel model)
        {
            if(model.ResourceUrl.StartsWith("IgnoreOverride:"))
            {
                model.ResourceUrl = model.ResourceUrl.Replace("IgnoreOverride:", "");
                return;
            }
            // ManageAPIが別URLの場合はドメインを変更
            if (model.ResourceUrl.StartsWith("/Manage/") && !string.IsNullOrEmpty(ServerEnvironment.Url2))
            {
                model.ServerUrl = ServerEnvironment.Url2;
            }

            // リポジトリに応じてURLを変更
            switch (TargetRepository)
            {
                case Repository.CosmosDb:
                    return;
                case Repository.MongoDb:
                    model.ResourceUrl = model.ResourceUrl.Replace("/IntegratedTest/", "/IntegratedTest/Mongo/");
                    return;
                case Repository.MongoDbCds:
                    model.ResourceUrl = model.ResourceUrl.Replace("/IntegratedTest/", "/IntegratedTest/MongoCDS/");
                    return;
                case Repository.SqlServer:
                    model.ResourceUrl = model.ResourceUrl.Replace("/IntegratedTest/", "/IntegratedTest/SqlServer/");
                    return;
                default:
                    return;
            }
        }

        #region 非同期

        /// <summary>
        /// 非同期APIを実行する（Endになるまで）（Json）
        /// </summary>
        public string ExecAsyncApiJson(WebApiRequestModel request, RetryPolicy<HttpResponseMessage> policy = null, Dictionary<string, string[]> additionalHeaders = null)
        {
            request.Header.Remove("Accept");
            var headers = new Dictionary<string, string[]> { { HeaderConst.X_IsAsync, "true" } };
            if (additionalHeaders != null)
            {
                foreach (var kv in additionalHeaders)
                {
                    headers.Add(kv.Key, kv.Value);
                }
            }
            var response = this.GetWebApiResponseResult(request, headers).Assert(System.Net.HttpStatusCode.Accepted);

            string requestId;
            if (response.Result is AsyncRequestResponseModel tmp)
            {
                requestId = tmp.RequestId;
            }
            else
            {
                requestId = JsonConvert.DeserializeObject<AsyncRequestResponseModel>(response.ContentString).RequestId;
            }

            // StatusがEndになるまでStatusをGet
            WaitForAsyncEnd(requestId, policy);

            return requestId;
        }

        /// <summary>
        /// 非同期APIを実行する（Endになるまで）（Json）
        /// </summary>
        public string ExecAsyncApiXml(WebApiRequestModel request)
        {
            request.Header.Remove("Accept");
            var headers = new Dictionary<string, string[]> { { HeaderConst.X_IsAsync, "true" }, { HeaderConst.Accept, "application/xml" } };
            var response = this.GetWebApiResponseResult(request, headers).Assert(System.Net.HttpStatusCode.Accepted);

            const string asyncResponseExpectXmlHead = @"<?xml version=""1.0"" encoding=""utf-8""?><root xmlns:dh=""http://example.com/XMLSchema-instance/""><RequestId>";
            const string asyncResponseExpectXmlTail = "</RequestId></root>";
            var requestId = response.ContentString.Replace(asyncResponseExpectXmlHead, "").Replace(asyncResponseExpectXmlTail, "").Trim();

            // StatusがEndになるまでStatusをGet
            WaitForAsyncEnd(requestId);

            return requestId;
        }

        /// <summary>
        /// 非同期APIを実行する（Endになるまで）（Json）
        /// </summary>
        public string ExecAsyncApiCsv(WebApiRequestModel request)
        {
            request.Header.Remove("Accept");
            var headers = new Dictionary<string, string[]> { { HeaderConst.X_IsAsync, "true" }, { HeaderConst.Accept, "text/csv" } };
            var response = this.GetWebApiResponseResult(request, headers).Assert(System.Net.HttpStatusCode.Accepted);

            var requestId = response.ContentString.Replace("RequestId", "").Trim();

            // StatusがEndになるまでStatusをGet
            WaitForAsyncEnd(requestId);

            return requestId;
        }

        /// <summary>
        /// 非同期APIを実行する（Endになるまで）（Json）
        /// </summary>
        public string ExecAsyncApiGeoJson(WebApiRequestModel request)
        {
            request.Header.Remove("Accept");
            var headers = new Dictionary<string, string[]> { { HeaderConst.X_IsAsync, "true" }, { HeaderConst.Accept, "application/geo+json" } };
            var response = this.GetWebApiResponseResult(request, headers).Assert(System.Net.HttpStatusCode.Accepted);

            string requestId;
            if (response.Result is AsyncRequestResponseModel tmp)
            {
                requestId = tmp.RequestId;
            }
            else
            {
                requestId = JsonConvert.DeserializeObject<AsyncRequestResponseModel>(response.ContentString).RequestId;
            }

            // StatusがEndになるまでStatusをGet
            WaitForAsyncEnd(requestId);

            return requestId;
        }

        public void WaitForAsyncEnd(string requestId, RetryPolicy<HttpResponseMessage> policy = null)
        {
            var api = UnityCore.Resolve<IAsyncApi>();
            this.GetWebApiResponseResult(api.GetStatus(requestId), policy ?? retryStatusPolicy);
        }

        /// <summary>
        /// ページングで取得する
        /// </summary>
        public string GetResultPaging<T>(string requestId, string continuation, Accept accept, T expected, bool isLastPage = false)
        {
            var api = UnityCore.Resolve<IAsyncApi>();
            api.AddHeaders.Add(HeaderConst.X_RequestContinuation, continuation);
            var response = this.GetWebApiResponseResult(api.GetResult(requestId)).Assert(System.Net.HttpStatusCode.OK);

            switch (accept)
            {
                case Accept.Json:
                case Accept.GeoJson:
                    var resultJson = JsonConvert.DeserializeObject<T>(response.ContentString);
                    resultJson.IsStructuralEqual(expected);
                    break;
                case Accept.Xml:
                    var resultXml = response.ContentString.StringToXml();
                    resultXml.Is((expected as string).StringToXml());
                    break;
                case Accept.Csv:
                    var resultCsv = response.ContentString;
                    resultCsv.Is(expected as string);
                    break;
            }

            response.Headers.Contains(HeaderConst.X_ResponseContinuation).Is(true);

            if (isLastPage)
            {
                //最終ページは空なのを確認
                string.IsNullOrEmpty(response.Headers.GetValues(HeaderConst.X_ResponseContinuation).FirstOrDefault()).Is(true);
            }

            return response.Headers.GetValues(HeaderConst.X_ResponseContinuation).FirstOrDefault();
        }

        /// <summary>
        /// ページングで取得する
        /// </summary>
        public string GetResultPaging<T>(WebApiRequestModel request, string continuation, Accept accept, T expected, bool isLastPage = false)
        {
            var headers = new Dictionary<string, string[]>() { { HeaderConst.X_RequestContinuation, continuation } };
            var response = this.GetWebApiResponseResult(request, headers).Assert(System.Net.HttpStatusCode.OK);

            switch (accept)
            {
                case Accept.Json:
                case Accept.GeoJson:
                    var resultJson = JsonConvert.DeserializeObject<T>(response.ContentString);
                    resultJson.IsStructuralEqual(expected);
                    break;
                case Accept.Xml:
                    var resultXml = response.ContentString.StringToXml();
                    resultXml.Is((expected as string).StringToXml());
                    break;
                case Accept.Csv:
                    var resultCsv = response.ContentString;
                    resultCsv.Is(expected as string);
                    break;
            }

            response.Headers.Contains(HeaderConst.X_ResponseContinuation).Is(true);

            if (isLastPage)
            {
                //最終ページは空なのを確認
                string.IsNullOrEmpty(response.Headers.GetValues(HeaderConst.X_ResponseContinuation).FirstOrDefault()).Is(true);
            }

            return response.Headers.GetValues(HeaderConst.X_ResponseContinuation).FirstOrDefault();
        }

        #endregion

        #region 添付ファイル

        public WebApiResponseResult ChunkUploadAttachFile<T>(string fileId, string filePath, ICommonResource<T> resource)
        {
            var info = new FileInfo(filePath);
            List<byte[]> sendList = new List<byte[]>();
            long contentsLength = 0;

            //1MBでスプリット
            int splitByte = 1024 * 1024;
            using (FileStream st = info.OpenRead())
            {
                contentsLength = st.Length;
                for (int j = 0; (st.Length / splitByte) > j; j++)
                {
                    byte[] by = new byte[splitByte];

                    for (int i = 0; i < splitByte; i++)
                    {
                        by[i] = (byte)st.ReadByte();
                    }
                    sendList.Add(by);
                }
                if ((st.Length % splitByte) != 0)
                {
                    byte[] eby = new byte[st.Length % splitByte];

                    for (int i = 0; i < (st.Length % splitByte); i++)
                    {
                        eby[i] = (byte)st.ReadByte();
                    }
                    sendList.Add(eby);
                }
            }

            int count = 0;
            WebApiResponseResult endResponse = null;
            foreach (var r in sendList)
            {
                long from = count * splitByte;
                long to = from + splitByte;

                if (to > contentsLength - 1)
                {
                    to = contentsLength - 1;
                }

                var stream = new MemoryStream(r);
                WebApiRequestModel request;
                if (resource is IStaticApiAttachFileApi staticApi)
                {
                    request = staticApi.UploadFile(stream, fileId);
                }
                else
                {
                    request = resource.UploadAttachFile(stream, fileId);
                }

                request.MediaType = "application/octet-stream";
                request.ContentRange = new ContentRange()
                {
                    From = from,
                    To = to,
                    Length = contentsLength
                };

                var response = this.GetWebApiResponseResult(request).Assert(System.Net.HttpStatusCode.OK);
                count++;

                if (to == contentsLength - 1)
                {
                    endResponse = response;
                }
            }

            resource.AddHeaders.Remove(HeaderConst.ContentType);
            resource.AddHeaders.Remove(HeaderConst.ContentRange);

            return endResponse;
        }

        #endregion

        #region Logging

        public void VerificationLogging(IManageLoggingApi manageApi, string expectedHttpStatusCode, string httpMethod, string requestBody, WebApiResponseResult response, LoggingModel target, bool isGateway = false, bool isAttachFileDownload = false)
        {
            target.HttpStatusCode.Is(expectedHttpStatusCode);
            target.ExecuteTime.IsNot(0);
            target.ProviderSystemId.IsNotNull();
            target.ProviderVendorId.IsNotNull();
            target.VendorId.ToString().ToLower().Is(VendorSystemInfo.VendorId.ToLower());
            target.SystemId.ToString().ToLower().Is(VendorSystemInfo.SystemId.ToLower());
            target.OpenId.ToString().ToLower().IsNotNull();
            target.HttpMethodType.ToLower().Is(httpMethod.ToLower());
            target.ResponseLength.Is(response.RawContent.Headers.ContentLength.ToString());

            // Bodyは記録しない
            var requestBodyResult = this.GetWebApiResponseResult(manageApi.GetRequestBody(target.LogId.ToString())).RawContentString;
            string.IsNullOrEmpty(requestBodyResult).IsTrue();

            var responseBodyResult = this.GetWebApiResponseResult(manageApi.GetResponseBody(target.LogId.ToString())).RawContentString;
            string.IsNullOrEmpty(responseBodyResult).IsTrue();
        }

        public void VerificationLogging<T>(IManageLoggingApi manageApi, string expectedHttpStatusCode, string httpMethod, string requestBody, WebApiResponseResult<T> response, LoggingModel target, bool isGateway = false, bool isAttachFileDownload = false) where T : new()
        {
            target.HttpStatusCode.Is(expectedHttpStatusCode);
            target.ExecuteTime.IsNot(0);
            target.ProviderSystemId.IsNotNull();
            target.ProviderVendorId.IsNotNull();
            target.VendorId.ToString().ToLower().Is(VendorSystemInfo.VendorId.ToLower());
            target.SystemId.ToString().ToLower().Is(VendorSystemInfo.SystemId.ToLower());
            target.OpenId.ToString().ToLower().IsNotNull();
            target.HttpMethodType.ToLower().Is(httpMethod.ToLower());
            target.ResponseLength.Is(response.RawContent.Headers.ContentLength.ToString());

            // Bodyは記録しない
            var requestBodyResult = this.GetWebApiResponseResult(manageApi.GetRequestBody(target.LogId.ToString())).RawContentString;
            string.IsNullOrEmpty(requestBodyResult).IsTrue();

            var responseBodyResult = this.GetWebApiResponseResult(manageApi.GetResponseBody(target.LogId.ToString())).RawContentString;
            string.IsNullOrEmpty(responseBodyResult).IsTrue();
        }

        #endregion
    }
}
