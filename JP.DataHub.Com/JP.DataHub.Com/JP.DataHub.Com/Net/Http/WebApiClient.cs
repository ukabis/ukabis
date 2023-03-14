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

namespace JP.DataHub.Com.Net.Http
{
    public class WebApiClient : IWebApiClient
    {
        [Dependency]
        public IWebRequestManager WebRequestManager { get; set; } = null;

        public IServerEnvironment ServerEnvironment { get; set; }
        public IAuthenticationInfo AuthenticationInfo { get; set; }
        public IAuthenticationResult AuthenticationResult { get; set; }

        public static Dictionary<string, string[]> AdditionalHeader { get; set; } = new Dictionary<string, string[]>();

        public WebApiClient()
        {
            if (WebRequestManager == null)
            {
                WebRequestManager = new WebRequestManager();
            }
        }

        /// <summary>
        /// アカウント名を指定してDynamicApiClientを作成する
        /// ※環境情報などはDIするため、Unity(DI)の初期化は必須である
        /// </summary>
        /// <param name="nameAccount"></param>
        public WebApiClient(string nameAccount)
        {
            if (UnityCore.UnityContainer == null)
            {
                throw new Exception("DI(Unity)の初期化がされていません。このコンストラクタはDIの利用を前提としているためDIの初期化をしてください。");
            }

            if (WebRequestManager == null)
            {
                WebRequestManager = new WebRequestManager();
            }

            ServerEnvironment = UnityCore.Resolve<IServerEnvironment>();
            AuthenticationInfo = UnityCore.Resolve<IAuthenticationInfo>(nameAccount);
            // 認証結果を取得
            var AuthenticationService = AuthenticationInfo.ToAuthenticationType().CreateAuthenticationService();
            AuthenticationResult = AuthenticationService.Authentication(ServerEnvironment, AuthenticationInfo);
        }

        public WebApiClient(IServerEnvironment environment, IAuthenticationInfo authenticationInfo = null)
        {
            ServerEnvironment = environment;
            AuthenticationInfo = authenticationInfo;
            if (WebRequestManager == null)
            {
                WebRequestManager = new WebRequestManager();
            }

            // 認証結果を取得
            var AuthenticationService = authenticationInfo.ToAuthenticationType().CreateAuthenticationService();
            AuthenticationResult = AuthenticationService.Authentication(environment, authenticationInfo);
        }

        public WebApiClient(IServerEnvironment environment, IAuthenticationInfo authenticationInfo = null, IAuthenticationResult authenticationResult = null)
        {
            ServerEnvironment = environment;
            AuthenticationInfo = authenticationInfo;
            if (WebRequestManager == null)
            {
                WebRequestManager = new WebRequestManager();
            }

            // 認証結果を取得
            var AuthenticationService = authenticationInfo?.ToAuthenticationType().CreateAuthenticationService();
            if (AuthenticationService != null)
            {
                AuthenticationResult = AuthenticationService.Authentication(environment, authenticationInfo);
            }
            if (authenticationResult != null)
            {
                AuthenticationResult = authenticationResult;
            }
        }

        public WebApiClient(IServerEnvironment environment, IAuthenticationResult authenticationResult = null)
        {
            ServerEnvironment = environment;
            AuthenticationResult = authenticationResult;
            if (WebRequestManager == null)
            {
                WebRequestManager = new WebRequestManager();
            }
        }

        public void SwitchAuthentication(IAuthenticationInfo authenticationInfo)
        {
            AuthenticationInfo = authenticationInfo;
            if (AuthenticationInfo != null)
            {
                var AuthenticationService = authenticationInfo?.ToAuthenticationType().CreateAuthenticationService();
                AuthenticationResult = AuthenticationService.Authentication(ServerEnvironment, authenticationInfo);
            }
        }

        public void SwitchAuthenticationResult(IAuthenticationResult authenticationResult)
        {
            AuthenticationResult = authenticationResult;
        }

        public virtual HttpResponseMessage Request(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
            => WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader));

        public virtual HttpResponseMessage Request(WebApiRequestModel req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            return retryPolicy != null
                ? retryPolicy.Execute(() => WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader)))
                : WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader));
        }

        public virtual HttpResponseMessage Request<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
            => WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader));

        public virtual HttpResponseMessage Request<T>(WebApiRequestModel<T> req, RetryPolicy<HttpResponseMessage> retryPolicy, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            return retryPolicy != null
                ? retryPolicy.Execute(() => WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader)))
                : WebRequestManager.Send(GenerateRequestMessage(req, addtionalHeader, replacementHeader));
        }

        protected HttpRequestMessage GenerateRequestMessage(WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            var message = new HttpRequestMessage()
            {
                Method = req.HttpMethod,
                RequestUri = req.ServerUrl == null ? req.RequestRelativeUri.ToUri() : req.ServerUrl?.UriCombine(req.RequestRelativeUri)
            };

            if (!string.IsNullOrEmpty(req.Contents))
            {
                message.Content = new StringContent(req.Contents);
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(req.MediaType);
                if (req.ContentRange != null)
                {
                    message.Content.Headers.ContentRange = new ContentRangeHeaderValue(req.ContentRange.From, req.ContentRange.To, req.ContentRange.Length);
                }
            }
            else if (req.ContentsStream != null)
            {
                req.ContentsStream.Seek(0, System.IO.SeekOrigin.Begin);
                message.Content = new StreamContent(req.ContentsStream);
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(req.MediaType);
                if (req.ContentRange != null)
                {
                    message.Content.Headers.ContentRange = new ContentRangeHeaderValue(req.ContentRange.From, req.ContentRange.To, req.ContentRange.Length);
                }
            }

            AddHeaders(message, req, addtionalHeader, replacementHeader);
            return message;
        }

        protected HttpRequestMessage GenerateRequestMessage<T>(WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            var message = new HttpRequestMessage()
            {
                Method = req.HttpMethod,
                RequestUri = req.ServerUrl == null ? req.RequestRelativeUri.ToUri() : req.ServerUrl?.UriCombine(req.RequestRelativeUri)
            };

            if (!string.IsNullOrEmpty(req.Contents))
            {
                message.Content = new StringContent(req.Contents);
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(req.MediaType);
                if (req.ContentRange != null)
                {
                    message.Content.Headers.ContentRange = new ContentRangeHeaderValue(req.ContentRange.From, req.ContentRange.To, req.ContentRange.Length);
                }
            }
            else if (req.ContentsStream != null)
            {
                req.ContentsStream.Seek(0, System.IO.SeekOrigin.Begin);
                message.Content = new StreamContent(req.ContentsStream);
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(req.MediaType);
                if (req.ContentRange != null)
                {
                    message.Content.Headers.ContentRange = new ContentRangeHeaderValue(req.ContentRange.From, req.ContentRange.To, req.ContentRange.Length);
                }
            }
            else if (req.ContentsStream == null)
            {
                message.Content = new StringContent("");
                message.Content.Headers.ContentType = new MediaTypeHeaderValue(req.MediaType);
            }

            AddHeaders(message, req, addtionalHeader, replacementHeader);
            return message;
        }

        protected virtual void AddHeaders(HttpRequestMessage message, WebApiRequestModel req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            if (addtionalHeader?.Any() == true)
            {
                addtionalHeader.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
            }
            if (req.Header?.Any() == true)
            {
                req.Header.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
            }
            if (AdditionalHeader?.Any() == true)
            {
                AdditionalHeader.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
            }
            if (UnityCore.IsRegistered<IWebApiClientAddHeader>())
            {
                var hedaer = UnityCore.Resolve<IWebApiClientAddHeader>();
                if (hedaer?.Header?.Any() == true)
                {
                    hedaer.Header.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
                }
            }
            if (replacementHeader?.Any() == true)
            {
                replacementHeader.ToList().ForEach(x =>
                {
                    if (message.Headers.Contains(x.Key))
                    {
                        message.Headers.Remove(x.Key);
                    }
                    message.Headers.TryAddWithoutValidation(x.Key, x.Value);
                });
            }
        }

        /// <summary>
        /// ヘッダ設定
        /// </summary>
        protected virtual void AddHeaders<T>(HttpRequestMessage message, WebApiRequestModel<T> req, IDictionary<string, string[]> addtionalHeader = null, IDictionary<string, string[]> replacementHeader = null)
        {
            if (addtionalHeader?.Any() == true)
            {
                addtionalHeader.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
            }
            if (req.Header?.Any() == true)
            {
                req.Header.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
            }
            if (AdditionalHeader?.Any() == true)
            {
                AdditionalHeader.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
            }
            if (UnityCore.IsRegistered<IWebApiClientAddHeader>())
            {
                var hedaer = UnityCore.Resolve<IWebApiClientAddHeader>();
                if (hedaer?.Header?.Any() == true)
                {
                    hedaer.Header.ToList().ForEach(x => message.Headers.TryAddWithoutValidation(x.Key, x.Value));
                }
            }
            if (replacementHeader?.Any() == true)
            {
                replacementHeader.ToList().ForEach(x =>
                {
                    if (message.Headers.Contains(x.Key))
                    {
                        message.Headers.Remove(x.Key);
                    }
                    message.Headers.TryAddWithoutValidation(x.Key, x.Value);
                });
            }
        }
    }
}
