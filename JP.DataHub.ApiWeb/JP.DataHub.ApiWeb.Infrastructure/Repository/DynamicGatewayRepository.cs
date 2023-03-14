using System.Text;
using System.Security.Cryptography;
using JP.DataHub.Com.Unity;
using JP.DataHub.ApiWeb.Domain.Context.Common;
using JP.DataHub.ApiWeb.Domain.Context.DynamicApi.Actions;
using JP.DataHub.ApiWeb.Domain.Repository;
using JP.DataHub.ApiWeb.Infrastructure.WebRequest;

namespace JP.DataHub.ApiWeb.Infrastructure.Repository
{
    // .NET6
    internal class DynamicGatewayRepository : AbstractRepository, IDynamicGatewayRepository
    {
        private IWebRequestManager WebRequestManager => _lazyWebRequestManager.Value;
        private Lazy<IWebRequestManager> _lazyWebRequestManager => new Lazy<IWebRequestManager>(() => UnityCore.Resolve<IWebRequestManager>());

        private int GatewayClientTiemoutSec => UnityCore.Resolve<int>("GatewayClientTiemoutSec");


        public GatewayResponse Put(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contentsStream, bool isCache = false)
            => new GatewayResponse(WebRequestManager.Put(url.Value, contentsStream.Value, gatewayInfo.CredentialUsername, gatewayInfo.CredentialPassword, GetRelayHeader(gatewayInfo.GatewayRelayHeader), GetTimeout()), isCache);

        public GatewayResponse Delete(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contentsStream, bool isCache = false)
            => new GatewayResponse(WebRequestManager.Delete(url.Value, gatewayInfo.CredentialUsername, gatewayInfo.CredentialPassword, GetRelayHeader(gatewayInfo.GatewayRelayHeader), GetTimeout()), isCache);

        public GatewayResponse Get(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contentsStream, bool isCache = false)
            => new GatewayResponse(WebRequestManager.Get(url.Value, gatewayInfo.CredentialUsername, gatewayInfo.CredentialPassword, GetRelayHeader(gatewayInfo.GatewayRelayHeader), HttpCompletionOption.ResponseHeadersRead, GetTimeout()), isCache);

        public GatewayResponse Post(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contentsStream, bool isCache = false)
            => new GatewayResponse(WebRequestManager.Post(url.Value, contentsStream.Value, gatewayInfo.CredentialUsername, gatewayInfo.CredentialPassword, GetRelayHeader(gatewayInfo.GatewayRelayHeader), GetTimeout()), isCache);

        public GatewayResponse Patch(RequestGatewayUrl url, GatewayInfo gatewayInfo, Contents contentsStream, bool isCache = false)
            => new GatewayResponse(WebRequestManager.Patch(url.Value, contentsStream.Value, gatewayInfo.CredentialUsername, gatewayInfo.CredentialPassword, GetRelayHeader(gatewayInfo.GatewayRelayHeader), GetTimeout()), isCache);

        public string CreateCacheKey(IGatewayAction action)
        {
            var headers = GetRelayHeader(action.GatewayInfo.GatewayRelayHeader);
            StringBuilder sb = new StringBuilder();
            foreach (var header in headers)
            {
                sb.Append(header.Key.ToLower());
                sb.Append(":");
                sb.Append(header.Value ?? "");
                sb.Append("~");
            }
            if (action.GatewayInfo.CredentialPassword != null && action.GatewayInfo.CredentialUsername != null)
            {
                sb.Append(action.GatewayInfo.CredentialUsername);
                sb.Append(action.GatewayInfo.CredentialPassword);
            }
            var hash = GetHashString(sb.ToString());

            //キーはDnamicApiのURL+QueryString+Hedderと認証情報をハッシュ化したもの
            return $"{action.RelativeUri.Value}?{action.Query?.OriginalQueryString}~{hash}";
        }

        private Dictionary<string, string> GetRelayHeader(string targetHeaderKeyList)
        {
            Dictionary<string, string> relayHeader = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(targetHeaderKeyList))
            {
                return relayHeader;
            }

            var keys = targetHeaderKeyList.Split(',');

            foreach (var key in keys)
            {
                // 中継対象ヘッダの大文字小文字は無視(リクエスト側のヘッダ名で中継)
                var requestHeader = this.PerRequestDataContainer.RequestHeaders.Where(x => x.Key.ToLower() == key.ToLower());
                if (requestHeader.Any())
                {
                    var keyValue = requestHeader.First();
                    relayHeader.Add(keyValue.Key, keyValue.Value.FirstOrDefault());
                }
            }

            // Open ID Connect認証必須のGatewayが、Basic認証APIへと中継できるようにするため、Authorizationヘッダの値の再設定を行う
            relayHeader = NormalizeAuthorizationHeader(relayHeader);

            return relayHeader;
        }

        /// <summary>
        /// Gateway先にリレーするHttpヘッダの組み合わせのうち、Authenticationヘッダについて、代替ヘッダ値からBasic認証用のトークンを取得し再設定する
        /// </summary>
        /// <param name="relayHeader"></param>
        /// <returns></returns>
        private Dictionary<string, string> NormalizeAuthorizationHeader(Dictionary<string, string> relayHeader)
        {
            const string AlternativeHeader = "X-Relay-Authorization";
            if (!relayHeader.ContainsKey(AlternativeHeader))
            {
                return relayHeader;
            }
            const string FormalHeader = "Authorization";
            if (!relayHeader.ContainsKey(FormalHeader))
            {
                relayHeader.Add(FormalHeader, relayHeader[AlternativeHeader]);
            }
            relayHeader[FormalHeader] = relayHeader[AlternativeHeader];
            return relayHeader;
        }

        private static string GetHashString(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            StringBuilder result = new StringBuilder();
            using (var algorithm = new SHA256CryptoServiceProvider())
            {
                byte[] bs = algorithm.ComputeHash(data);
                algorithm.Clear();
                foreach (byte b in bs)
                {
                    result.Append(b.ToString("X2"));
                }
            }
            return result.ToString();
        }

        private TimeSpan GetTimeout()
        {
            if (GatewayClientTiemoutSec <= 0)
            {
                return Timeout.InfiniteTimeSpan;
            }
            else
            {
                return new TimeSpan(0, 0, GatewayClientTiemoutSec);
            }
        }
    }
}
