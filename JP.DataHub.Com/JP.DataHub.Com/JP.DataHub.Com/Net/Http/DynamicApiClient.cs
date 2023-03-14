using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Polly.Retry;
using JP.DataHub.Com.Extensions;
using JP.DataHub.Com.Web.WebRequest;
using JP.DataHub.Com.Web.Authentication;
using JP.DataHub.Com.Unity;
using JP.DataHub.Com.RFC7807;

namespace JP.DataHub.Com.Net.Http
{
    /// <summary>
    /// 基盤のDynamicApiを呼び出す便利クラス
    /// </summary>
    public class DynamicApiClient : WebApiClient, IDynamicApiClient
    {
        /// <summary>
        /// 何もしないコンストラクタ
        /// </summary>
        public DynamicApiClient()
            : base()
        {
        }

        /// <summary>
        /// アカウント名を指定してDynamicApiClientを作成する
        /// ※環境情報などはDIするため、Unity(DI)の初期化は必須である
        /// </summary>
        /// <param name="nameAccount"></param>
        public DynamicApiClient(string nameAccount)
            : base(nameAccount)
        {
        }

        /// <summary>
        /// DynamicApiClientを作成する
        /// 環境情報および認証情報、認証結果はデフォルトのものを使う
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="authenticationInfo"></param>
        /// <param name="authenticationResult"></param>
        public DynamicApiClient(IServerEnvironment environment, IAuthenticationInfo authenticationInfo = null, IAuthenticationResult authenticationResult = null)
            : base(environment, authenticationInfo, authenticationResult)
        {
        }

        /// <summary>
        /// DynamicApiClientを作成する
        /// 環境情報および認証結果情報はデフォルトのものを使う
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="authenticationResult"></param>
        public DynamicApiClient(IServerEnvironment environment, IAuthenticationResult authenticationResult = null)
            : base(environment, authenticationResult)
        {
        }

        /// <summary>
        /// DynamicApiClientを作成する
        /// 環境情報および認証情報はデフォルトのものを使う
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="authenticationInfo"></param>
        public DynamicApiClient(IServerEnvironment environment, IAuthenticationInfo authenticationInfo = null)
            : base(environment, authenticationInfo)
        {
        }

        public override HttpResponseMessage Request(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            // 認証が取れていない場合は取得する
            if (AuthenticationResult == null)
            {
                var service = AuthenticationInfo?.GetAuthenticationService();
                AuthenticationResult = service?.Authentication(this.ServerEnvironment, AuthenticationInfo);
            }

            var existsErrorCode = new List<string>();
            for (; ; )
            {
                var message = GenerateRequestMessage(req, addtionalHeader, replacementHeader);
                var result = WebRequestManager.Send(message);
                var expiredInfo = TokenExpiredToRetry(result);
                if (expiredInfo.IsRetry == false)
                {
                    return result;
                }
                if (expiredInfo.ErrorCode != null)
                {
                    if (existsErrorCode.Contains(expiredInfo.ErrorCode))
                    {
                        // RFCエラーがすでに発生してリトライしても同じならエラーを返す
                        return result;
                    }
                    existsErrorCode.Add(expiredInfo.ErrorCode);
                }
            }
        }

        public WebApiResponseResult GetWebApiResponseResult(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
            => Request(req, addtionalHeader, replacementHeader).ToWebApiResponseResult();

        public override HttpResponseMessage Request(WebApiRequestModel req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            // 認証が取れていない場合は取得する
            if (AuthenticationResult == null)
            {
                var service = AuthenticationInfo?.GetAuthenticationService();
                AuthenticationResult = service?.Authentication(this.ServerEnvironment, AuthenticationInfo);
            }

            var existsErrorCode = new List<string>();
            for (; ; )
            {
                var result = retryPolicy != null
                    ? retryPolicy.Execute(() => WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader)))
                    : WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader));
                var expiredInfo = TokenExpiredToRetry(result);
                if (expiredInfo.IsRetry == false)
                {
                    return result;
                }
                if (expiredInfo.ErrorCode != null)
                {
                    if (existsErrorCode.Contains(expiredInfo.ErrorCode))
                    {
                        // RFCエラーがすでに発生してリトライしても同じならエラーを返す
                        return result;
                    }
                    existsErrorCode.Add(expiredInfo.ErrorCode);
                }
            }
        }

        public WebApiResponseResult GetWebApiResponseResult(WebApiRequestModel req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
            => Request(req, retryPolicy, addtionalHeader, replacementHeader).ToWebApiResponseResult();

        public override HttpResponseMessage Request<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            // 認証が取れていない場合は取得する
            if (AuthenticationResult == null)
            {
                var service = AuthenticationInfo?.GetAuthenticationService();
                AuthenticationResult = service?.Authentication(this.ServerEnvironment, AuthenticationInfo);
            }

            var existsErrorCode = new List<string>();
            for (; ; )
            {
                var message = GenerateRequestMessage(req, addtionalHeader, replacementHeader);
                var result = WebRequestManager.Send(message);
#if (DEBUG)
                if (result.IsSuccessStatusCode == false && result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var content = result.Content.ReadAsStringAsync().Result;
                    System.Diagnostics.Debug.WriteLine(content);
                }
#endif
                var expiredInfo = TokenExpiredToRetry(result);
                if (expiredInfo.IsRetry == false)
                {
                    return result;
                }
                if (expiredInfo.ErrorCode != null)
                {
                    if (existsErrorCode.Contains(expiredInfo.ErrorCode))
                    {
                        // RFCエラーがすでに発生してリトライしても同じならエラーを返す
                        return result;
                    }
                    existsErrorCode.Add(expiredInfo.ErrorCode);
                }
            }
        }

        public WebApiResponseResult<T> GetWebApiResponseResult<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null) where T : new()
            => Request<T>(req, addtionalHeader, replacementHeader).ToWebApiResponseResult<T>();

        public override HttpResponseMessage Request<T>(WebApiRequestModel<T> req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            // 認証が取れていない場合は取得する
            if (AuthenticationResult == null)
            {
                var service = AuthenticationInfo?.GetAuthenticationService();
                AuthenticationResult = service?.Authentication(this.ServerEnvironment, AuthenticationInfo);
            }

            var existsErrorCode = new List<string>();
            for (; ; )
            {
                var result = retryPolicy != null
                    ? retryPolicy.Execute(() => WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader)))
                    : WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader));
                var expiredInfo = TokenExpiredToRetry(result);
                if (expiredInfo.IsRetry == false)
                {
                    return result;
                }
                if (expiredInfo.ErrorCode != null)
                {
                    if (existsErrorCode.Contains(expiredInfo.ErrorCode))
                    {
                        // RFCエラーがすでに発生してリトライしても同じならエラーを返す
                        return result;
                    }
                    existsErrorCode.Add(expiredInfo.ErrorCode);
                }
            }
        }

        public WebApiResponseResult<T> GetWebApiResponseResult<T>(WebApiRequestModel<T> req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null) where T : new()
            => Request<T>(req, retryPolicy, addtionalHeader, replacementHeader).ToWebApiResponseResult<T>();

        private (bool IsRetry, string ErrorCode) TokenExpiredToRetry(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode == true)
            {
                // 成功した場合はリトライしない
                return (false, null);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var detail = response.Content.ReadAsStringAsync().Result;
                var rfc7807 = detail.ToRFC7807ProblemDetail();
                if (rfc7807?.ErrorCode == "E02407"/*Vendor Token Expired*/ || rfc7807?.ErrorCode == "E01405"/*openid token expired*/)
                {
                    var info = AuthenticationResult.Info;
                    if (info != null)
                    {
                        info.AuthenticationResult = null;
                    }
                    if (ServerEnvironment == null)
                    {
                        throw new Exception("WebApiClientのインスタンスを生成する際に ServerEnvironment をコンストラクタで指定してください。");
                    }
                    //var authService = UnityCore.Resolve<IAuthenticationService>(ServerEnvironment.Parent.AuthenticationType.ToString());
                    var authService = UnityCore.Resolve<IAuthenticationService>();
                    var result = authService.Authentication(ServerEnvironment, AuthenticationResult.Info);
                    if (result.IsSuccessfull == false)
                    {
                        // トークン取得で失敗した時は諦めるためfalse
                        return (false, null);
                    }
                    AuthenticationResult = result;
                    info.AuthenticationResult = result;
                    return (true, rfc7807?.ErrorCode);
                }
            }
            // Unauthorizedではない、またはE02407,E01405以外はリトライしない（再認証する必要が無い場合だから）
            return (false, null);
        }

        protected override void AddHeaders(HttpRequestMessage message, WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            AuthenticationResult?.AddHeader(message.Headers);
            ServerEnvironment?.AddHeader(message.Headers);
            base.AddHeaders(message, req, addtionalHeader, replacementHeader);
        }

        protected override void AddHeaders<T>(HttpRequestMessage message, WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            AuthenticationResult?.AddHeader(message.Headers);
            ServerEnvironment?.AddHeader(message.Headers);
            base.AddHeaders<T>(message, req, addtionalHeader, replacementHeader);
        }
    }
}
